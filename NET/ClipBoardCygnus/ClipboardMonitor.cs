using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    public sealed class ClipboardMonitor : NativeWindow, IDisposable
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private const int WM_HOTKEY = 0x0312;
        private const int WM_APP_TRIGGER_HOTKEY = 0x8001;
        private const int HOTKEY_ID = 0x4359;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;
        private const uint MOD_NOREPEAT = 0x4000;

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const ushort VK_SHIFT = 0x10;
        private const ushort VK_CONTROL = 0x11;
        private const ushort VK_MENU = 0x12;
        private const ushort VK_LWIN = 0x5B;
        private const ushort VK_RWIN = 0x5C;
        private const ushort VK_V = 0x56;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private bool _suppressNextEvent;
        private readonly SqliteStorageService _storage;
        private uint _hotkeyModifiers;
        private uint _hotkeyVk;
        private IntPtr _llHookId = IntPtr.Zero;
        private LowLevelKeyboardProc _llHookProc;
        private bool _llHotkeyLatched;
        private int _lastHotkeyTriggerTick;

        public bool IsRecordingHotkey { get; set; }
        public bool IsLowLevelHookActive => _llHookId != IntPtr.Zero;

        public event Action<ClipItem> ClipCaptured;
        public event Action HotkeyPressed;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public ClipboardMonitor(SqliteStorageService storage)
        {
            _storage = storage;
            CreateHandle(new CreateParams());
            AddClipboardFormatListener(Handle);
            InstallLowLevelKeyboardHook();
            RegisterConfiguredHotkey(_storage.Settings.GlobalHotkey);
        }

        private void InstallLowLevelKeyboardHook()
        {
            try
            {
                _llHookProc = LowLevelKeyboardCallback;
                using (var curProcess = Process.GetCurrentProcess())
                using (var curModule = curProcess.MainModule)
                {
                    IntPtr hMod = curModule != null ? GetModuleHandle(curModule.ModuleName) : IntPtr.Zero;
                    _llHookId = SetWindowsHookEx(WH_KEYBOARD_LL, _llHookProc, hMod, 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ClipBoardCygnus Hook Error: " + ex.Message);
            }
        }

        private IntPtr LowLevelKeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && !IsRecordingHotkey && _hotkeyVk != 0)
            {
                int msg = unchecked((int)wParam.ToInt64());
                uint vkCode = unchecked((uint)Marshal.ReadInt32(lParam));

                if (msg == WM_KEYUP || msg == WM_SYSKEYUP)
                {
                    if (vkCode == _hotkeyVk) _llHotkeyLatched = false;
                }
                else if (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN)
                {
                    if (vkCode == _hotkeyVk && ModifiersMatch(_hotkeyModifiers))
                    {
                        if (!_llHotkeyLatched)
                        {
                            _llHotkeyLatched = true;
                            PostMessage(Handle, WM_APP_TRIGGER_HOTKEY, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                }
            }
            return CallNextHookEx(_llHookId, nCode, wParam, lParam);
        }

        private static bool ModifiersMatch(uint requiredModifiers)
        {
            bool ctrl = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
            bool alt = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;
            bool shift = (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;
            bool win = ((GetAsyncKeyState(VK_LWIN) & 0x8000) != 0) || ((GetAsyncKeyState(VK_RWIN) & 0x8000) != 0);

            bool reqCtrl = (requiredModifiers & MOD_CONTROL) != 0;
            bool reqAlt = (requiredModifiers & MOD_ALT) != 0;
            bool reqShift = (requiredModifiers & MOD_SHIFT) != 0;
            bool reqWin = (requiredModifiers & MOD_WIN) != 0;

            return ctrl == reqCtrl && alt == reqAlt && shift == reqShift && win == reqWin;
        }

        public bool RegisterConfiguredHotkey(string shortcut)
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
            ParseHotkey(shortcut, out _hotkeyModifiers, out _hotkeyVk);

            bool ok = RegisterHotKey(Handle, HOTKEY_ID, _hotkeyModifiers | MOD_NOREPEAT, _hotkeyVk);
            if (!ok)
            {
                ok = RegisterHotKey(Handle, HOTKEY_ID, _hotkeyModifiers, _hotkeyVk);
            }
            return ok;
        }

        private static void ParseHotkey(string shortcut, out uint modifiers, out uint vk)
        {
            modifiers = 0;
            vk = (uint)Keys.V;

            if (string.IsNullOrWhiteSpace(shortcut))
            {
                modifiers = MOD_CONTROL | MOD_SHIFT;
                return;
            }

            string[] tokens = shortcut.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawToken in tokens)
            {
                string token = rawToken.Trim();
                string upper = token.ToUpperInvariant();

                if (upper == "CTRL" || upper == "CONTROL") modifiers |= MOD_CONTROL;
                else if (upper == "SHIFT") modifiers |= MOD_SHIFT;
                else if (upper == "ALT") modifiers |= MOD_ALT;
                else if (upper == "WIN" || upper == "META") modifiers |= MOD_WIN;
                else if (token == "<" || token == ">" || upper == "OEMBACKSLASH" || upper == "OEM102" || upper == "LESS")
                {
                    short scan = VkKeyScan('<');
                    if (scan != -1 && (scan & 0xFF00) == 0)
                        vk = (uint)(scan & 0xFF);
                    else
                        vk = (uint)Keys.OemBackslash;
                }
                else if (token == "`" || upper == "OEM3" || upper == "BACKQUOTE") vk = (uint)Keys.Oem3;
                else if (token == "," || upper == "OEMCOMMA") vk = (uint)Keys.Oemcomma;
                else if (token == "." || upper == "OEMPERIOD") vk = (uint)Keys.OemPeriod;
                else if (token == "-" || upper == "OEMMINUS") vk = (uint)Keys.OemMinus;
                else if (token.Length == 1 && char.IsDigit(token[0]))
                    vk = (uint)(Keys.D0 + (token[0] - '0'));
                else if (Enum.TryParse(token, true, out Keys parsedKey))
                    vk = (uint)parsedKey;
                else if (token.Length == 1)
                {
                    short scan = VkKeyScan(token[0]);
                    if (scan != -1) vk = (uint)(scan & 0xFF);
                }
            }
        }

        private static void ExecuteClipboardWrite(Action writeAction)
        {
            for (int attempt = 0; attempt < 6; attempt++)
            {
                try
                {
                    writeAction();
                    return;
                }
                catch
                {
                    Thread.Sleep(30);
                }
            }
        }

        public static void SendCtrlV()
        {
            var inputs = new System.Collections.Generic.List<INPUT>();

            if ((GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0)
                inputs.Add(MakeKeyInput(VK_SHIFT, true));
            if ((GetAsyncKeyState(VK_MENU) & 0x8000) != 0)
                inputs.Add(MakeKeyInput(VK_MENU, true));
            if ((GetAsyncKeyState(VK_LWIN) & 0x8000) != 0)
                inputs.Add(MakeKeyInput(VK_LWIN, true));
            if ((GetAsyncKeyState(VK_RWIN) & 0x8000) != 0)
                inputs.Add(MakeKeyInput(VK_RWIN, true));

            inputs.Add(MakeKeyInput(VK_CONTROL, false));
            inputs.Add(MakeKeyInput(VK_V, false));
            inputs.Add(MakeKeyInput(VK_V, true));
            inputs.Add(MakeKeyInput(VK_CONTROL, true));

            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        private static INPUT MakeKeyInput(ushort vk, bool keyUp)
        {
            return new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vk,
                        wScan = 0,
                        dwFlags = keyUp ? KEYEVENTF_KEYUP : 0,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
        }

        public void RestoreTextToSystem(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            _suppressNextEvent = true;
            try
            {
                ExecuteClipboardWrite(() => System.Windows.Forms.Clipboard.SetDataObject(text, true, 5, 25));
            }
            finally
            {
                var resetTimer = new System.Windows.Forms.Timer { Interval = 350 };
                resetTimer.Tick += (s, e) =>
                {
                    _suppressNextEvent = false;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();
            }
        }

        public void RestoreClipToSystem(ClipItem item)
        {
            _suppressNextEvent = true;
            try
            {
                ClipKind kind = item.GetKind();
                if (kind == ClipKind.Text && !string.IsNullOrEmpty(item.Content))
                {
                    ExecuteClipboardWrite(() => System.Windows.Forms.Clipboard.SetDataObject(item.Content, true, 5, 25));
                }
                else if (kind == ClipKind.Image)
                {
                    Image img = LoadImageFromClip(item);
                    if (img != null)
                    {
                        using (img)
                        {
                            ExecuteClipboardWrite(() => System.Windows.Forms.Clipboard.SetImage(img));
                        }
                    }
                }
                else if (kind == ClipKind.File && !string.IsNullOrEmpty(item.LocalBlobPath))
                {
                    string fullPath = Path.Combine(_storage.BaseDataPath, item.LocalBlobPath);
                    if (File.Exists(fullPath))
                    {
                        var files = new StringCollection { fullPath };
                        ExecuteClipboardWrite(() => System.Windows.Forms.Clipboard.SetFileDropList(files));
                    }
                }
            }
            finally
            {
                var resetTimer = new System.Windows.Forms.Timer { Interval = 350 };
                resetTimer.Tick += (s, e) =>
                {
                    _suppressNextEvent = false;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();
            }
        }

        public void RestoreMultipleClipsToSystem(System.Collections.Generic.List<ClipItem> items)
        {
            if (items == null || items.Count == 0) return;
            if (items.Count == 1)
            {
                RestoreClipToSystem(items[0]);
                return;
            }

            _suppressNextEvent = true;
            try
            {
                bool allFiles = true;
                var filePaths = new StringCollection();
                foreach (var item in items)
                {
                    if (item.GetKind() != ClipKind.File || string.IsNullOrEmpty(item.LocalBlobPath))
                    {
                        allFiles = false;
                        break;
                    }
                    string fullPath = Path.Combine(_storage.BaseDataPath, item.LocalBlobPath);
                    if (!File.Exists(fullPath))
                    {
                        allFiles = false;
                        break;
                    }
                    filePaths.Add(fullPath);
                }

                if (allFiles && filePaths.Count > 0)
                {
                    ExecuteClipboardWrite(() => System.Windows.Forms.Clipboard.SetFileDropList(filePaths));
                }
                else
                {
                    var lines = new System.Collections.Generic.List<string>();
                    foreach (var item in items)
                    {
                        if (item.GetKind() == ClipKind.Text && !string.IsNullOrEmpty(item.Content))
                            lines.Add(item.Content);
                        else
                            lines.Add(item.FileName ?? item.Title);
                    }
                    string joined = string.Join(Environment.NewLine, lines);
                    ExecuteClipboardWrite(() => System.Windows.Forms.Clipboard.SetDataObject(joined, true, 5, 25));
                }
            }
            finally
            {
                var resetTimer = new System.Windows.Forms.Timer { Interval = 350 };
                resetTimer.Tick += (s, e) =>
                {
                    _suppressNextEvent = false;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();
            }
        }

        private Image LoadImageFromClip(ClipItem item)
        {
            if (!string.IsNullOrEmpty(item.LocalBlobPath))
            {
                string full = Path.Combine(_storage.BaseDataPath, item.LocalBlobPath);
                if (File.Exists(full)) return Image.FromFile(full);
            }
            if (!string.IsNullOrEmpty(item.Content) && item.Content.StartsWith("data:image"))
            {
                int comma = item.Content.IndexOf(',');
                if (comma > 0)
                {
                    byte[] raw = Convert.FromBase64String(item.Content.Substring(comma + 1));
                    using (var ms = new MemoryStream(raw))
                        return Image.FromStream(ms);
                }
            }
            return null;
        }

        private void TriggerHotkeyOnce()
        {
            int now = Environment.TickCount;
            if (unchecked(now - _lastHotkeyTriggerTick) < 200) return;
            _lastHotkeyTriggerTick = now;
            HotkeyPressed?.Invoke();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                if (!_suppressNextEvent) CaptureClipboard();
            }
            else if ((m.Msg == WM_HOTKEY && unchecked((int)m.WParam.ToInt64()) == HOTKEY_ID) ||
                     m.Msg == WM_APP_TRIGGER_HOTKEY)
            {
                if (!IsRecordingHotkey) TriggerHotkeyOnce();
            }
            base.WndProc(ref m);
        }

        private void CaptureClipboard()
        {
            try
            {
                if (_storage.Settings.IgnorePasswordManagers &&
                    System.Windows.Forms.Clipboard.ContainsData("Clipboard Viewer Ignore"))
                {
                    return;
                }

                string sourceApp = GetActiveWindowTitle();
                string nowIso = DateTime.UtcNow.ToString("o");
                ClipItem captured = null;

                if (System.Windows.Forms.Clipboard.ContainsText())
                {
                    string text = System.Windows.Forms.Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(text)) return;

                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    string firstLine = text.Trim().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (firstLine.Length > 96) firstLine = firstLine.Substring(0, 96);

                    captured = new ClipItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        KindString = "text",
                        Title = firstLine,
                        Content = text,
                        MimeType = "text/plain",
                        SizeBytes = bytes.Length,
                        Hash = SqliteStorageService.ComputeSha256(bytes),
                        SourceApp = sourceApp,
                        CreatedAt = nowIso,
                        UpdatedAt = nowIso
                    };
                }
                else if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    using (Image img = System.Windows.Forms.Clipboard.GetImage())
                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Png);
                        byte[] pngBytes = ms.ToArray();
                        string fileName = "Captura_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
                        string relPath = _storage.SaveBlobBytes(fileName, pngBytes);

                        captured = new ClipItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            KindString = "image",
                            Title = fileName,
                            FileName = fileName,
                            MimeType = "image/png",
                            SizeBytes = pngBytes.Length,
                            Width = img.Width,
                            Height = img.Height,
                            LocalBlobPath = relPath,
                            Content = "data:image/png;base64," + Convert.ToBase64String(pngBytes),
                            Hash = SqliteStorageService.ComputeSha256(pngBytes),
                            SourceApp = sourceApp,
                            CreatedAt = nowIso,
                            UpdatedAt = nowIso
                        };
                    }
                }
                else if (System.Windows.Forms.Clipboard.ContainsFileDropList())
                {
                    StringCollection files = System.Windows.Forms.Clipboard.GetFileDropList();
                    if (files.Count == 0) return;

                    string firstFile = files[0];
                    var info = new FileInfo(firstFile);
                    if (!info.Exists) return;

                    const long maxLocalBlobBytes = 25L * 1024L * 1024L;
                    byte[] raw = info.Length <= maxLocalBlobBytes ? File.ReadAllBytes(firstFile) : Encoding.UTF8.GetBytes(firstFile);
                    string relPath = info.Length <= maxLocalBlobBytes ? _storage.SaveBlobBytes(info.Name, raw) : null;

                    captured = new ClipItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        KindString = "file",
                        Title = files.Count > 1 ? $"{info.Name} (+{files.Count - 1} más)" : info.Name,
                        FileName = info.Name,
                        MimeType = "application/octet-stream",
                        SizeBytes = info.Length,
                        LocalBlobPath = relPath,
                        Content = firstFile,
                        Hash = SqliteStorageService.ComputeSha256(raw),
                        SourceApp = sourceApp,
                        CreatedAt = nowIso,
                        UpdatedAt = nowIso
                    };
                }

                if (captured != null)
                {
                    var saved = _storage.Upsert(captured);
                    ClipCaptured?.Invoke(saved);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ClipBoardCygnus Capture Error: " + ex.Message);
            }
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle, buff, nChars) > 0 ? buff.ToString() : "Windows";
        }

        public void Dispose()
        {
            if (_llHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_llHookId);
                _llHookId = IntPtr.Zero;
            }
            UnregisterHotKey(Handle, HOTKEY_ID);
            RemoveClipboardFormatListener(Handle);
            DestroyHandle();
        }
    }
}

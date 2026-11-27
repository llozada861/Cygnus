using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    public class MainSettingsForm : Form
    {
        private sealed class HotkeyCaptureBox : TextBox
        {
            public event Action<Keys> HotkeyCaptured;

            protected override bool IsInputKey(Keys keyData)
            {
                return true;
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                HotkeyCaptured?.Invoke(keyData);
                return true;
            }
        }

        private readonly SqliteStorageService _storage;
        private readonly ClipboardMonitor _monitor;

        private CheckBox _portableChk;
        private CheckBox _startupChk;
        private CheckBox _trayChk;
        private HotkeyCaptureBox _hotkeyBox;
        private Label _hotkeyStatusLabel;
        private string _recordedHotkey;

        public MainSettingsForm(
            SqliteStorageService storage,
            ClipboardMonitor monitor)
        {
            _storage = storage;
            _monitor = monitor;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "ClipBoardCygnus — Configuración";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(460, 315);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);
            Shown += (s, e) =>
            {
                Activate();
                BringToFront();
            };
            FormClosed += (s, e) =>
            {
                _monitor.IsRecordingHotkey = false;
            };

            var cfg = _storage.Settings;
            _recordedHotkey = string.IsNullOrWhiteSpace(cfg.GlobalHotkey) ? "Ctrl + Shift + V" : cfg.GlobalHotkey;

            _portableChk = new CheckBox
            {
                Text = "Modo Portable (SQLite en .\\CygnusData\\ClipBoardCygnus.db)",
                Checked = cfg.PortableMode,
                Location = new Point(24, 22),
                Width = 415
            };

            _startupChk = new CheckBox
            {
                Text = "Iniciar automáticamente con Windows",
                Checked = cfg.StartWithWindows,
                Location = new Point(24, 52),
                Width = 415
            };

            _trayChk = new CheckBox
            {
                Text = "Iniciar minimizado en la bandeja del sistema",
                Checked = cfg.StartMinimized,
                Location = new Point(24, 82),
                Width = 415
            };

            var lblHotkey = new Label
            {
                Text = "Atajo de teclado para abrir la lista del portapapeles:",
                Location = new Point(24, 118),
                Width = 410
            };

            _hotkeyBox = new HotkeyCaptureBox
            {
                Text = _recordedHotkey,
                ReadOnly = true,
                ShortcutsEnabled = false,
                Location = new Point(24, 144),
                Width = 310,
                BackColor = Color.FromArgb(248, 250, 252),
                Font = new Font("Consolas", 10f),
                Cursor = Cursors.Hand
            };

            _hotkeyBox.Enter += (s, e) =>
            {
                _monitor.IsRecordingHotkey = true;
                _hotkeyBox.BackColor = Color.FromArgb(239, 246, 255);
                _hotkeyBox.Text = "Pulsa la combinación ahora...";
            };

            _hotkeyBox.Leave += (s, e) =>
            {
                _monitor.IsRecordingHotkey = false;
                _hotkeyBox.BackColor = Color.FromArgb(248, 250, 252);
                _hotkeyBox.Text = _recordedHotkey;
            };

            _hotkeyBox.HotkeyCaptured += OnHotkeyCaptured;

            var resetHotkeyBtn = new Button
            {
                Text = "Restablecer",
                Location = new Point(342, 142),
                Size = new Size(92, 27),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            resetHotkeyBtn.Click += (s, e) =>
            {
                _recordedHotkey = "Ctrl + Shift + V";
                _hotkeyBox.Text = _recordedHotkey;
                _hotkeyStatusLabel.Text = "Atajo restablecido.";
                _hotkeyStatusLabel.ForeColor = Color.FromArgb(71, 85, 105);
            };

            _hotkeyStatusLabel = new Label
            {
                Text = "Haz clic en el cuadro y pulsa Ctrl / Alt / Shift + una tecla (ej. Ctrl + <).",
                Location = new Point(24, 174),
                Width = 410,
                Font = new Font("Segoe UI", 8.25f),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            var lblDbPath = new Label
            {
                Text = "BD SQLite: " + _storage.DatabaseFilePath,
                Location = new Point(24, 212),
                Width = 410,
                Font = new Font("Consolas", 7.75f),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            var saveBtn = new Button
            {
                Text = "Guardar cambios",
                Location = new Point(304, 252),
                Size = new Size(130, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White
            };
            saveBtn.Click += (s, e) =>
            {
                cfg.PortableMode = _portableChk.Checked;
                cfg.StartWithWindows = _startupChk.Checked;
                cfg.StartMinimized = _trayChk.Checked;
                cfg.GlobalHotkey = _recordedHotkey;

                _storage.SaveSettings(cfg);
                WindowsStartupService.SetEnabled(cfg.StartWithWindows, cfg.StartMinimized);

                bool registered = _monitor.RegisterConfiguredHotkey(cfg.GlobalHotkey);
                if (!registered && !_monitor.IsLowLevelHookActive)
                {
                    _hotkeyStatusLabel.Text = "Windows rechazó el atajo (puede estar ocupado por otra app).";
                    _hotkeyStatusLabel.ForeColor = Color.FromArgb(185, 28, 28);
                    return;
                }

                Close();
            };

            var cancelBtn = new Button
            {
                Text = "Cancelar",
                Location = new Point(206, 252),
                Size = new Size(90, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            cancelBtn.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                _portableChk, _startupChk, _trayChk,
                lblHotkey, _hotkeyBox, resetHotkeyBtn, _hotkeyStatusLabel,
                lblDbPath, cancelBtn, saveBtn
            });
        }

        private void OnHotkeyCaptured(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            bool ctrl = (keyData & Keys.Control) == Keys.Control;
            bool alt = (keyData & Keys.Alt) == Keys.Alt;
            bool shift = (keyData & Keys.Shift) == Keys.Shift;

            if (keyCode == Keys.Escape && !ctrl && !alt && !shift)
            {
                _hotkeyBox.Text = _recordedHotkey;
                ActiveControl = null;
                return;
            }

            if (keyCode == Keys.None ||
                keyCode == Keys.ControlKey || keyCode == Keys.LControlKey || keyCode == Keys.RControlKey ||
                keyCode == Keys.ShiftKey || keyCode == Keys.LShiftKey || keyCode == Keys.RShiftKey ||
                keyCode == Keys.Menu || keyCode == Keys.LMenu || keyCode == Keys.RMenu ||
                keyCode == Keys.LWin || keyCode == Keys.RWin)
            {
                return;
            }

            bool isFunctionKey = keyCode >= Keys.F1 && keyCode <= Keys.F12;
            if (!ctrl && !alt && !shift && !isFunctionKey)
            {
                _hotkeyStatusLabel.Text = "Usa al menos Ctrl, Alt o Shift junto con la tecla (o F1–F12).";
                _hotkeyStatusLabel.ForeColor = Color.FromArgb(180, 83, 9);
                return;
            }

            var parts = new System.Collections.Generic.List<string>();
            if (ctrl) parts.Add("Ctrl");
            if (alt) parts.Add("Alt");
            if (shift) parts.Add("Shift");

            string keyName = FormatKeyDisplay(keyCode);
            parts.Add(keyName);

            _recordedHotkey = string.Join(" + ", parts);
            _hotkeyBox.Text = _recordedHotkey;
            _hotkeyStatusLabel.Text = "Atajo listo: " + _recordedHotkey;
            _hotkeyStatusLabel.ForeColor = Color.FromArgb(22, 163, 74);
            ActiveControl = null;
        }

        private static string FormatKeyDisplay(Keys keyCode)
        {
            if (keyCode == Keys.OemBackslash || keyCode == Keys.Oem102) return "<";
            if (keyCode == Keys.Oem3) return "`";
            if (keyCode == Keys.Oemcomma) return ",";
            if (keyCode == Keys.OemPeriod) return ".";
            if (keyCode == Keys.OemMinus) return "-";

            string keyName = keyCode.ToString();
            if (keyName.Length == 2 && keyName[0] == 'D' && char.IsDigit(keyName[1]))
            {
                return keyName.Substring(1);
            }
            return keyName;
        }
    }
}

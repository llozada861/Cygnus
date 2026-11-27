using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    public class ClipboardPopupForm : Form
    {
        private sealed class BufferedListBox : ListBox
        {
            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_LBUTTONDBLCLK = 0x0203;
            public Func<Point, bool> HandleActionClick;
            public Func<Point, bool> IsOnActionButton;

            public BufferedListBox()
            {
                DoubleBuffered = true;
                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_LBUTTONDOWN && HandleActionClick != null)
                {
                    int lParam = unchecked((int)m.LParam.ToInt64());
                    var pt = new Point((short)(lParam & 0xFFFF), (short)((lParam >> 16) & 0xFFFF));
                    if (HandleActionClick(pt))
                    {
                        return;
                    }
                }
                else if (m.Msg == WM_LBUTTONDBLCLK && IsOnActionButton != null)
                {
                    int lParam = unchecked((int)m.LParam.ToInt64());
                    var pt = new Point((short)(lParam & 0xFFFF), (short)((lParam >> 16) & 0xFFFF));
                    if (IsOnActionButton(pt))
                    {
                        return;
                    }
                }
                base.WndProc(ref m);
            }
        }

        private readonly SqliteStorageService _storage;
        private readonly ClipboardMonitor _monitor;

        private Panel _captionBar;
        private Label _titleLabel;
        private Button _templatesBtn;
        private Button _configBtn;
        private Button _closeBtn;
        private TextBox _searchBox;
        private ComboBox _filterBox;
        private Panel _multiBar;
        private Label _multiCountLabel;
        private BufferedListBox _listBox;
        private Panel _previewPanel;
        private PictureBox _imagePreview;
        private Label _previewMetaLabel;
        private Button _previewCopyBtn;
        private NotifyIcon _trayIcon;
        private ToolTip _toolTip;

        private List<ClipItem> _visibleItems = new List<ClipItem>();
        private IntPtr _previousForegroundWindow;
        private IntPtr _lastExternalWindow;
        private IntPtr _lastPasteTargetWindow;
        private IntPtr _lastOwnWindow;
        private Timer _boundsSaveTimer;
        private Timer _foregroundWatchTimer;
        private Timer _pasteTimer;
        private int _suppressDeactivateUntilTick;
        private bool _applyingSavedBounds;
        private bool _isResizingOrMoving;
        private bool _isModalOpen;

        private int _hoveredIndex = -1;
        private int _hoveredButton = 0; // 0 = none, 1 = pin, 2 = delete

        private const int WM_NCHITTEST = 0x0084;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int EM_SETCUEBANNER = 0x1501;

        private const int HTCAPTION = 2;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int SW_RESTORE = 9;
        private const uint GW_HWNDNEXT = 2;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentProcessId();

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        public ClipboardPopupForm(
            SqliteStorageService storage,
            ClipboardMonitor monitor,
            Icon appIcon = null)
        {
            _storage = storage;
            _monitor = monitor;

            InitializeMinimalistUi(appIcon);
            InitializeTrayIcon(appIcon);
            RefreshList();
        }

        private void InitializeMinimalistUi(Icon appIcon)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            DoubleBuffered = true;
            BackColor = Color.White;
            MinimumSize = new Size(380, 360);
            Padding = new Padding(5);
            if (appIcon != null) Icon = appIcon;

            _toolTip = new ToolTip();
            _boundsSaveTimer = new Timer { Interval = 250 };
            _boundsSaveTimer.Tick += (s, e) =>
            {
                _boundsSaveTimer.Stop();
                PersistCurrentBounds();
            };

            _foregroundWatchTimer = new Timer { Interval = 350 };
            _foregroundWatchTimer.Tick += (s, e) =>
            {
                if (Visible) return;
                IntPtr fg = GetForegroundWindow();
                if (IsExternalAppWindow(fg))
                {
                    _lastExternalWindow = fg;
                    _lastPasteTargetWindow = fg;
                }
                else if (IsPasteTargetWindow(fg))
                {
                    _lastPasteTargetWindow = fg;
                }
            };
            _foregroundWatchTimer.Start();

            ApplySavedWindowBounds(false);

            // 1. Draggable Caption Bar with Configurar & Close (×) buttons
            _captionBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(8, 5, 6, 5),
                BackColor = Color.FromArgb(248, 250, 252),
                Cursor = Cursors.SizeAll
            };
            _captionBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, 0, _captionBar.Height - 1, _captionBar.Width, _captionBar.Height - 1);
            };
            _captionBar.MouseDown += OnCaptionMouseDown;

            _titleLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"ClipBoardCygnus · {_storage.Settings.GlobalHotkey}",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.SizeAll,
                AutoEllipsis = true
            };
            _titleLabel.MouseDown += OnCaptionMouseDown;

            _templatesBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 76,
                Text = "Plantillas",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Regular),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(37, 99, 235),
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 0, 4, 0)
            };
            _templatesBtn.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            _templatesBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 246, 255);
            _templatesBtn.Click += (s, e) => ClipboardManager.ShowTemplatesWindow();

            _configBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 82,
                Text = "Configurar",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Regular),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 0, 4, 0)
            };
            _configBtn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            _configBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            _configBtn.Click += (s, e) => ClipboardManager.ShowSettingsWindow();

            _closeBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 30,
                Text = "×",
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(100, 116, 139),
                Cursor = Cursors.Hand
            };
            _closeBtn.FlatAppearance.BorderSize = 0;
            _closeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 226, 226);
            _closeBtn.MouseEnter += (s, e) => _closeBtn.ForeColor = Color.FromArgb(185, 28, 28);
            _closeBtn.MouseLeave += (s, e) => _closeBtn.ForeColor = Color.FromArgb(100, 116, 139);
            _closeBtn.Click += (s, e) =>
            {
                PersistCurrentBounds();
                Hide();
            };

            _captionBar.Controls.Add(_titleLabel);
            _captionBar.Controls.Add(_templatesBtn);
            _captionBar.Controls.Add(_configBtn);
            _captionBar.Controls.Add(_closeBtn);

            // 2. Search & Type Filter Bar
            var searchBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10, 7, 10, 6),
                BackColor = Color.White
            };
            searchBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, 0, searchBar.Height - 1, searchBar.Width, searchBar.Height - 1);
            };

            _searchBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            _searchBox.HandleCreated += (s, e) =>
            {
                SendMessage(_searchBox.Handle, EM_SETCUEBANNER, (IntPtr)1, "Buscar texto, imagen o archivo...");
            };

            _filterBox = new ComboBox
            {
                Dock = DockStyle.Right,
                Width = 92,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            _filterBox.Items.AddRange(new object[] { "Todo", "Texto", "Imágenes", "Archivos", "Fijados" });
            _filterBox.SelectedIndex = 0;

            searchBar.Controls.Add(_searchBox);
            searchBar.Controls.Add(_filterBox);

            // 3. Multi-select Action Bar
            _multiBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34,
                Padding = new Padding(10, 4, 10, 4),
                BackColor = Color.FromArgb(239, 246, 255),
                Visible = false
            };
            _multiBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(191, 219, 254), 1))
                    e.Graphics.DrawLine(pen, 0, _multiBar.Height - 1, _multiBar.Width, _multiBar.Height - 1);
            };

            _multiCountLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var multiCopyBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 148,
                Text = "Copiar combinados (Ctrl+C)",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7.75f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            multiCopyBtn.FlatAppearance.BorderSize = 0;
            multiCopyBtn.Click += (s, e) => CopySelectedToClipboard(false);

            var multiClearBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 62,
                Text = "Limpiar",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                BackColor = Color.FromArgb(239, 246, 255),
                ForeColor = Color.FromArgb(51, 65, 85),
                Cursor = Cursors.Hand
            };
            multiClearBtn.FlatAppearance.BorderSize = 0;
            multiClearBtn.Click += (s, e) =>
            {
                _listBox.ClearSelected();
                if (_listBox.Items.Count > 0) _listBox.SelectedIndex = 0;
            };

            _multiBar.Controls.Add(_multiCountLabel);
            _multiBar.Controls.Add(multiClearBtn);
            _multiBar.Controls.Add(multiCopyBtn);

            // 4. Preview & Quick Action Strip
            _previewPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 96,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10, 8, 14, 8),
                Visible = false
            };
            _previewPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, 0, 0, _previewPanel.Width, 0);
            };

            _imagePreview = new PictureBox
            {
                Dock = DockStyle.Left,
                Width = 112,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            var previewRight = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 0, 0, 0)
            };

            _previewMetaLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8.25f),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            var previewActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 28
            };

            _previewCopyBtn = new Button
            {
                Dock = DockStyle.Left,
                Width = 180,
                Text = "Copiar / Pegar (Ctrl+C / Enter)",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _previewCopyBtn.FlatAppearance.BorderSize = 0;
            _previewCopyBtn.Click += (s, e) => PasteSelected();

            previewActions.Controls.Add(_previewCopyBtn);
            previewRight.Controls.Add(_previewMetaLabel);
            previewRight.Controls.Add(previewActions);

            _previewPanel.Controls.Add(previewRight);
            _previewPanel.Controls.Add(_imagePreview);

            // 5. Clipboard ListBox with Visible Pin & Delete Buttons
            _listBox = new BufferedListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                DrawMode = DrawMode.OwnerDrawFixed,
                SelectionMode = SelectionMode.MultiExtended,
                ItemHeight = 36,
                BackColor = Color.White,
                IntegralHeight = false
            };

            _listBox.HandleActionClick = HandleListActionClick;
            _listBox.IsOnActionButton = IsPointOnActionButton;
            _listBox.DrawItem += OnDrawItem;
            _listBox.MouseMove += OnListMouseMove;
            _listBox.MouseLeave += (s, e) =>
            {
                if (_hoveredIndex != -1 || _hoveredButton != 0)
                {
                    _hoveredIndex = -1;
                    _hoveredButton = 0;
                    _listBox.Cursor = Cursors.Default;
                    _listBox.Invalidate();
                }
            };
            _listBox.SelectedIndexChanged += (s, e) => UpdatePreviewPane();
            _listBox.MouseDoubleClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                int idx = _listBox.IndexFromPoint(e.Location);
                if (idx < 0 || idx >= _visibleItems.Count) return;
                if (IsPointOnActionButton(e.Location)) return;
                PasteItemAtIndex(idx);
            };
            _searchBox.TextChanged += (s, e) => RefreshList();
            _filterBox.SelectedIndexChanged += (s, e) => RefreshList();
            _searchBox.KeyDown += OnSearchKeyDown;
            _listBox.KeyDown += OnListKeyDown;

            Deactivate += (s, e) =>
            {
                if (_isModalOpen || _isResizingOrMoving) return;
                if (unchecked(Environment.TickCount - _suppressDeactivateUntilTick) < 0) return;
                if (!ContainsFocus)
                {
                    PersistCurrentBounds();
                    Hide();
                }
            };

            SizeChanged += (s, e) =>
            {
                _listBox?.Invalidate();
                if (_applyingSavedBounds || WindowState != FormWindowState.Normal) return;
                _boundsSaveTimer.Stop();
                _boundsSaveTimer.Start();
            };

            LocationChanged += (s, e) =>
            {
                if (_applyingSavedBounds || !Visible || WindowState != FormWindowState.Normal) return;
                _boundsSaveTimer.Stop();
                _boundsSaveTimer.Start();
            };

            ResizeEnd += (s, e) =>
            {
                _isResizingOrMoving = false;
                PersistCurrentBounds();
            };

            FormClosing += (s, e) => PersistCurrentBounds();

            Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(203, 213, 225), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
                using (var gripPen = new Pen(Color.FromArgb(148, 163, 184), 1.5f))
                {
                    e.Graphics.DrawLine(gripPen, Width - 13, Height - 3, Width - 3, Height - 13);
                    e.Graphics.DrawLine(gripPen, Width - 8, Height - 3, Width - 3, Height - 8);
                }
            };

            Controls.Add(_listBox);
            Controls.Add(_previewPanel);
            Controls.Add(_multiBar);
            Controls.Add(searchBar);
            Controls.Add(_captionBar);
        }

        private void OnCaptionMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _isResizingOrMoving = true;
            try
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            }
            finally
            {
                _isResizingOrMoving = false;
                PersistCurrentBounds();
            }
        }

        public void RunWithModalPause(Action action)
        {
            _isModalOpen = true;
            bool prevTopMost = TopMost;
            TopMost = false;
            try
            {
                action?.Invoke();
            }
            finally
            {
                TopMost = prevTopMost;
                _isModalOpen = false;
                if (_titleLabel != null)
                {
                    _titleLabel.Text = $"ClipBoardCygnus · {_storage.Settings.GlobalHotkey}";
                }
                if (Visible)
                {
                    Activate();
                    BringToFront();
                }
            }
        }

        private void ApplySavedWindowBounds(bool positionWindow)
        {
            _storage.GetWindowBounds(out int? savedX, out int? savedY, out int w, out int h);
            var targetScreen = savedX.HasValue && savedY.HasValue
                ? Screen.FromPoint(new Point(savedX.Value + 20, savedY.Value + 20)).WorkingArea
                : Screen.FromPoint(Cursor.Position).WorkingArea;

            int clampedW = Math.Max(MinimumSize.Width, Math.Min(w, Math.Max(MinimumSize.Width, targetScreen.Width - 24)));
            int clampedH = Math.Max(MinimumSize.Height, Math.Min(h, Math.Max(MinimumSize.Height, targetScreen.Height - 24)));

            _applyingSavedBounds = true;
            try
            {
                Size = new Size(clampedW, clampedH);

                if (positionWindow)
                {
                    int x, y;
                    if (savedX.HasValue && savedY.HasValue)
                    {
                        x = Math.Max(targetScreen.Left, Math.Min(savedX.Value, targetScreen.Right - clampedW));
                        y = Math.Max(targetScreen.Top, Math.Min(savedY.Value, targetScreen.Bottom - clampedH));
                    }
                    else
                    {
                        x = targetScreen.Left + (targetScreen.Width - clampedW) / 2;
                        y = targetScreen.Top + (targetScreen.Height - clampedH) / 2;
                    }
                    Location = new Point(x, y);
                }
            }
            finally
            {
                _applyingSavedBounds = false;
            }
        }

        private void PersistCurrentBounds()
        {
            if (_applyingSavedBounds || WindowState != FormWindowState.Normal) return;
            if (Width >= MinimumSize.Width && Height >= MinimumSize.Height)
            {
                if (Visible)
                    _storage.SaveWindowBounds(Left, Top, Width, Height);
                else
                    _storage.SaveWindowSize(Width, Height);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ENTERSIZEMOVE)
            {
                _isResizingOrMoving = true;
            }
            else if (m.Msg == WM_EXITSIZEMOVE)
            {
                _isResizingOrMoving = false;
                PersistCurrentBounds();
            }
            else if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if ((int)m.Result == 1) // HTCLIENT on Form border padding
                {
                    int lp = unchecked((int)m.LParam.ToInt64());
                    Point pt = PointToClient(new Point((short)(lp & 0xFFFF), (short)((lp >> 16) & 0xFFFF)));
                    const int grip = 8;
                    bool left = pt.X <= grip;
                    bool right = pt.X >= ClientSize.Width - grip;
                    bool top = pt.Y <= grip;
                    bool bottom = pt.Y >= ClientSize.Height - grip;

                    if (top && left) m.Result = (IntPtr)HTTOPLEFT;
                    else if (top && right) m.Result = (IntPtr)HTTOPRIGHT;
                    else if (bottom && left) m.Result = (IntPtr)HTBOTTOMLEFT;
                    else if (bottom && right) m.Result = (IntPtr)HTBOTTOMRIGHT;
                    else if (left) m.Result = (IntPtr)HTLEFT;
                    else if (right) m.Result = (IntPtr)HTRIGHT;
                    else if (top) m.Result = (IntPtr)HTTOP;
                    else if (bottom) m.Result = (IntPtr)HTBOTTOM;
                }
                return;
            }
            base.WndProc(ref m);
        }

        private void InitializeTrayIcon(Icon appIcon)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Abrir ClipBoardCygnus (" + _storage.Settings.GlobalHotkey + ")", null, (s, e) => TogglePopup());
            menu.Items.Add("Plantillas de código...", null, (s, e) => ClipboardManager.ShowTemplatesWindow());
            menu.Items.Add("Configuración y atajo...", null, (s, e) => ClipboardManager.ShowSettingsWindow());
            menu.Items.Add(new ToolStripSeparator());

            var startupItem = new ToolStripMenuItem("Iniciar con Windows")
            {
                Checked = WindowsStartupService.IsEnabled(),
                CheckOnClick = true
            };
            startupItem.CheckedChanged += (s, e) =>
            {
                WindowsStartupService.SetEnabled(startupItem.Checked, _storage.Settings.StartMinimized);
                var cfg = _storage.Settings;
                cfg.StartWithWindows = startupItem.Checked;
                _storage.SaveSettings(cfg);
            };
            menu.Items.Add(startupItem);

            var portableItem = new ToolStripMenuItem("Base SQLite Portable (ClipBoardCygnus.db)")
            {
                Checked = _storage.Settings.PortableMode,
                Enabled = false
            };
            menu.Items.Add(portableItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Salir de ClipBoardCygnus", null, (s, e) =>
            {
                _trayIcon.Visible = false;
                Application.Exit();
            });

            _trayIcon = new NotifyIcon
            {
                Icon = appIcon ?? SystemIcons.Application,
                Text = "ClipBoardCygnus — Portapapeles SQLite Portable",
                Visible = true,
                ContextMenuStrip = menu
            };
            _trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) TogglePopup();
            };
        }

        public void TrackOwnWindow(IntPtr hWnd)
        {
            if (IsPasteTargetWindow(hWnd))
            {
                _lastOwnWindow = hWnd;
                _lastPasteTargetWindow = hWnd;
            }
        }

        public void HidePopup()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HidePopup));
                return;
            }

            if (Visible)
            {
                PersistCurrentBounds();
                Hide();
            }
        }

        public void ShowPopupForTargetHandle(IntPtr targetHwnd)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowPopupForTargetHandle(targetHwnd)));
                return;
            }

            if (Visible)
            {
                PersistCurrentBounds();
                Hide();
                return;
            }

            if (IsPasteTargetWindow(targetHwnd))
            {
                _previousForegroundWindow = targetHwnd;
                _lastPasteTargetWindow = targetHwnd;
                _lastOwnWindow = targetHwnd;
            }
            else
            {
                _previousForegroundWindow = ResolveTargetPasteWindow();
            }

            ShowAndFocusPopup();
        }

        public void TogglePopup()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TogglePopup));
                return;
            }

            if (Visible)
            {
                PersistCurrentBounds();
                Hide();
                return;
            }

            IntPtr fg = GetForegroundWindow();
            if (IsExternalAppWindow(fg))
            {
                _previousForegroundWindow = fg;
                _lastExternalWindow = fg;
                _lastPasteTargetWindow = fg;
            }
            else if (IsPasteTargetWindow(fg))
            {
                _previousForegroundWindow = fg;
                _lastPasteTargetWindow = fg;
            }
            else if (IsPasteTargetWindow(_lastPasteTargetWindow))
            {
                _previousForegroundWindow = _lastPasteTargetWindow;
            }
            else if (IsPasteTargetWindow(_lastOwnWindow))
            {
                _previousForegroundWindow = _lastOwnWindow;
            }
            else if (IsExternalAppWindow(_lastExternalWindow))
            {
                _previousForegroundWindow = _lastExternalWindow;
            }
            else
            {
                _previousForegroundWindow = FindTopPasteTargetWindow();
            }

            ShowAndFocusPopup();
        }

        private void ShowAndFocusPopup()
        {
            if (_titleLabel != null)
            {
                _titleLabel.Text = $"ClipBoardCygnus · {_storage.Settings.GlobalHotkey}";
            }
            ApplySavedWindowBounds(true);
            RefreshList();

            _suppressDeactivateUntilTick = Environment.TickCount + 250;
            Show();
            ForceForegroundWindow(Handle);
            Activate();
            BringToFront();
            _searchBox.Focus();
            _searchBox.SelectAll();
        }

        private static bool IsShellOrToolWindow(IntPtr hWnd)
        {
            var cls = new System.Text.StringBuilder(128);
            GetClassName(hWnd, cls, cls.Capacity);
            string className = cls.ToString();
            if (className == "Shell_TrayWnd" ||
                className == "Shell_SecondaryTrayWnd" ||
                className == "NotifyIconOverflowWindow" ||
                className == "TaskListThumbnailWnd" ||
                className == "Progman" ||
                className == "WorkerW" ||
                className == "Windows.UI.Core.CoreWindow")
            {
                return true;
            }

            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOOLWINDOW) != 0;
        }

        private bool IsExternalAppWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero || !IsWindow(hWnd) || !IsWindowVisible(hWnd)) return false;

            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);
            if (pid == GetCurrentProcessId()) return false;

            return !IsShellOrToolWindow(hWnd);
        }

        private bool IsPasteTargetWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero || hWnd == Handle || !IsWindow(hWnd) || !IsWindowVisible(hWnd)) return false;
            return !IsShellOrToolWindow(hWnd);
        }

        private IntPtr FindTopExternalWindow()
        {
            IntPtr h = GetWindow(Handle, GW_HWNDNEXT);
            int guard = 0;
            while (h != IntPtr.Zero && guard++ < 80)
            {
                if (IsExternalAppWindow(h)) return h;
                h = GetWindow(h, GW_HWNDNEXT);
            }
            return IntPtr.Zero;
        }

        private IntPtr FindTopPasteTargetWindow()
        {
            IntPtr h = GetWindow(Handle, GW_HWNDNEXT);
            int guard = 0;
            while (h != IntPtr.Zero && guard++ < 80)
            {
                if (IsPasteTargetWindow(h)) return h;
                h = GetWindow(h, GW_HWNDNEXT);
            }
            return IntPtr.Zero;
        }

        private IntPtr ResolveTargetPasteWindow()
        {
            if (IsPasteTargetWindow(_previousForegroundWindow)) return _previousForegroundWindow;
            if (IsPasteTargetWindow(_lastPasteTargetWindow)) return _lastPasteTargetWindow;
            if (IsExternalAppWindow(_lastExternalWindow)) return _lastExternalWindow;
            return FindTopPasteTargetWindow();
        }

        private IntPtr ResolveExternalTargetWindow()
        {
            if (IsExternalAppWindow(_lastExternalWindow)) return _lastExternalWindow;
            return FindTopExternalWindow();
        }

        private static void ForceForegroundWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero || !IsWindow(hWnd)) return;
            if (IsIconic(hWnd)) ShowWindow(hWnd, SW_RESTORE);

            IntPtr fg = GetForegroundWindow();
            uint unusedPid;
            uint fgThread = fg != IntPtr.Zero ? GetWindowThreadProcessId(fg, out unusedPid) : 0;
            uint targetThread = GetWindowThreadProcessId(hWnd, out unusedPid);
            uint currentThread = GetCurrentThreadId();
            bool attachedFg = false;
            bool attachedTarget = false;

            try
            {
                if (fgThread != 0 && fgThread != currentThread)
                    attachedFg = AttachThreadInput(currentThread, fgThread, true);
                if (targetThread != 0 && targetThread != currentThread && targetThread != fgThread)
                    attachedTarget = AttachThreadInput(currentThread, targetThread, true);

                SetForegroundWindow(hWnd);
                BringWindowToTop(hWnd);
            }
            finally
            {
                if (attachedTarget) AttachThreadInput(currentThread, targetThread, false);
                if (attachedFg) AttachThreadInput(currentThread, fgThread, false);
            }
        }

        public void RefreshList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshList));
                return;
            }

            string filterKey = "all";
            if (_filterBox != null)
            {
                switch (_filterBox.SelectedIndex)
                {
                    case 1: filterKey = "text"; break;
                    case 2: filterKey = "image"; break;
                    case 3: filterKey = "file"; break;
                    case 4: filterKey = "pinned"; break;
                }
            }

            _visibleItems = _storage.Search(_searchBox.Text, filterKey);
            _listBox.BeginUpdate();
            _listBox.Items.Clear();
            foreach (var item in _visibleItems)
            {
                _listBox.Items.Add(item);
            }
            if (_listBox.Items.Count > 0) _listBox.SelectedIndex = 0;
            _listBox.EndUpdate();
            UpdatePreviewPane();
        }

        private void UpdatePreviewPane()
        {
            int selCount = _listBox.SelectedIndices.Count;
            _multiBar.Visible = selCount > 1;
            if (selCount > 1)
            {
                _multiCountLabel.Text = $"{selCount} elementos seleccionados";
                _imagePreview.Visible = false;
                _previewMetaLabel.Text = $"{selCount} elementos listos para copiar juntos\r\nCtrl+C: Copiar · Enter: Pegar en la app activa";
                _previewCopyBtn.Text = $"Copiar {selCount} combinados (Ctrl+C)";
                _previewPanel.Visible = true;
                return;
            }

            if (_listBox.SelectedIndex < 0 || _listBox.SelectedIndex >= _visibleItems.Count)
            {
                _previewPanel.Visible = false;
                return;
            }

            ClipItem item = _visibleItems[_listBox.SelectedIndex];
            _previewCopyBtn.Text = "Copiar / Pegar (Ctrl+C / Enter)";

            if (item.GetKind() == ClipKind.Image)
            {
                Image img = LoadThumbnail(item);
                if (_imagePreview.Image != null)
                {
                    _imagePreview.Image.Dispose();
                    _imagePreview.Image = null;
                }
                _imagePreview.Image = img;
                _imagePreview.Visible = img != null;
                _previewMetaLabel.Text = $"{item.Title}\r\n{item.Width ?? 0}×{item.Height ?? 0} px · {item.SourceApp ?? "Windows"}";
                _previewPanel.Visible = true;
            }
            else if (item.GetKind() == ClipKind.File)
            {
                _imagePreview.Visible = false;
                _previewMetaLabel.Text = $"Archivo: {item.FileName ?? item.Title}\r\nTamaño: {Math.Max(1, item.SizeBytes / 1024)} KB · {item.SourceApp ?? "Windows"}";
                _previewPanel.Visible = true;
            }
            else
            {
                _imagePreview.Visible = false;
                string snippet = (item.Content ?? string.Empty).Replace("\r\n", " ").Replace("\n", " ").Trim();
                if (snippet.Length > 120) snippet = snippet.Substring(0, 120) + "...";
                _previewMetaLabel.Text = $"{snippet}\r\n{item.SizeBytes} B · {item.SourceApp ?? "Windows"}";
                _previewPanel.Visible = true;
            }
        }

        private Image LoadThumbnail(ClipItem item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.LocalBlobPath))
                {
                    string full = Path.Combine(_storage.BaseDataPath, item.LocalBlobPath);
                    if (File.Exists(full))
                    {
                        using (var fs = new FileStream(full, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(fs);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(item.Content) && item.Content.StartsWith("data:image"))
                {
                    int comma = item.Content.IndexOf(',');
                    if (comma > 0)
                    {
                        byte[] raw = Convert.FromBase64String(item.Content.Substring(comma + 1));
                        using (var ms = new MemoryStream(raw))
                        {
                            return Image.FromStream(ms);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private void PasteSelected()
        {
            CopySelectedToClipboard(true);
        }

        private void PasteItemAtIndex(int idx)
        {
            if (idx < 0 || idx >= _visibleItems.Count) return;
            var single = new List<ClipItem> { _visibleItems[idx] };
            CopyItemsToClipboard(single, true);
        }

        public void PasteTextIntoTarget(string text, string sourceApp, bool pasteIntoExternalApp)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => PasteTextIntoTarget(text, sourceApp, pasteIntoExternalApp)));
                return;
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            string[] lines = text.Trim().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string firstLine = lines.Length > 0 ? lines[0] : "Plantilla de código";
            if (firstLine.Length > 96) firstLine = firstLine.Substring(0, 96);
            string nowIso = DateTime.UtcNow.ToString("o");

            var clip = new ClipItem
            {
                Id = Guid.NewGuid().ToString(),
                KindString = "text",
                Title = firstLine,
                Content = text,
                MimeType = "text/plain",
                SizeBytes = bytes.Length,
                Hash = SqliteStorageService.ComputeSha256(bytes),
                SourceApp = sourceApp ?? "Plantilla",
                CreatedAt = nowIso,
                UpdatedAt = nowIso
            };

            _monitor.RestoreTextToSystem(text);
            _storage.Upsert(clip);
            RefreshList();

            if (pasteIntoExternalApp)
            {
                IntPtr targetHwnd = ResolveExternalTargetWindow();
                PersistCurrentBounds();
                Hide();

                if (targetHwnd != IntPtr.Zero)
                {
                    _pasteTimer?.Stop();
                    _pasteTimer?.Dispose();
                    _pasteTimer = new Timer { Interval = 95 };
                    _pasteTimer.Tick += (s, e) =>
                    {
                        _pasteTimer.Stop();
                        _pasteTimer.Dispose();
                        _pasteTimer = null;

                        ForceForegroundWindow(targetHwnd);
                        System.Threading.Thread.Sleep(40);
                        ClipboardMonitor.SendCtrlV();
                    };
                    _pasteTimer.Start();
                }
            }
            else
            {
                _imagePreview.Visible = false;
                _previewMetaLabel.Text = "Plantilla generada y copiada al portapapeles.";
                _previewPanel.Visible = true;
            }
        }

        private void CopySelectedToClipboard(bool pasteIntoPreviousWindow)
        {
            if (_listBox.SelectedIndices.Count == 0) return;

            var selected = new List<ClipItem>();
            foreach (int idx in _listBox.SelectedIndices)
            {
                if (idx >= 0 && idx < _visibleItems.Count)
                {
                    selected.Add(_visibleItems[idx]);
                }
            }
            if (selected.Count == 0) return;

            CopyItemsToClipboard(selected, pasteIntoPreviousWindow);
        }

        private void CopyItemsToClipboard(List<ClipItem> selected, bool pasteIntoPreviousWindow)
        {
            if (selected == null || selected.Count == 0) return;

            var ids = new List<string>(selected.Count);
            foreach (var item in selected) ids.Add(item.Id);

            if (selected.Count == 1)
                _monitor.RestoreClipToSystem(selected[0]);
            else
                _monitor.RestoreMultipleClipsToSystem(selected);

            _storage.MarkAsUsed(ids);
            RefreshList();

            if (pasteIntoPreviousWindow)
            {
                IntPtr targetHwnd = ResolveTargetPasteWindow();
                PersistCurrentBounds();
                Hide();

                if (targetHwnd != IntPtr.Zero)
                {
                    _pasteTimer?.Stop();
                    _pasteTimer?.Dispose();
                    _pasteTimer = new Timer { Interval = 95 };
                    _pasteTimer.Tick += (s, e) =>
                    {
                        _pasteTimer.Stop();
                        _pasteTimer.Dispose();
                        _pasteTimer = null;

                        ForceForegroundWindow(targetHwnd);
                        Control targetCtrl = Control.FromHandle(targetHwnd);
                        if (targetCtrl is Form ownForm && !ownForm.IsDisposed)
                        {
                            ownForm.Activate();
                            var sendTimer = new Timer { Interval = 45 };
                            sendTimer.Tick += (s2, e2) =>
                            {
                                sendTimer.Stop();
                                sendTimer.Dispose();
                                ClipboardMonitor.SendCtrlV();
                            };
                            sendTimer.Start();
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(40);
                            ClipboardMonitor.SendCtrlV();
                        }
                    };
                    _pasteTimer.Start();
                }
            }
            else
            {
                _imagePreview.Visible = false;
                _previewMetaLabel.Text = selected.Count > 1
                    ? $"{selected.Count} elementos copiados al portapapeles y movidos al inicio."
                    : "Elemento copiado al portapapeles y movido al inicio.";
                _previewPanel.Visible = true;
            }
        }

        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                PersistCurrentBounds();
                Hide();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C && _searchBox.SelectionLength == 0 && _listBox.SelectedIndices.Count > 0)
            {
                CopySelectedToClipboard(false);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down && _listBox.Items.Count > 0)
            {
                _listBox.Focus();
                if (_listBox.SelectedIndex < _listBox.Items.Count - 1) _listBox.SelectedIndex++;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                PasteSelected();
                e.Handled = true;
            }
        }

        private void OnListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                PersistCurrentBounds();
                Hide();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedToClipboard(false);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter) { PasteSelected(); e.Handled = true; }
            else if (e.KeyCode == Keys.P && _listBox.SelectedIndex >= 0)
            {
                _storage.TogglePin(_visibleItems[_listBox.SelectedIndex].Id);
                RefreshList();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete && _listBox.SelectedIndex >= 0)
            {
                _storage.Delete(_visibleItems[_listBox.SelectedIndex].Id);
                RefreshList();
                e.Handled = true;
            }
        }

        private static Rectangle GetPinButtonRect(Rectangle bounds)
        {
            int y = bounds.Y + (bounds.Height - 26) / 2;
            return new Rectangle(bounds.Right - 64, y, 26, 26);
        }

        private static Rectangle GetDeleteButtonRect(Rectangle bounds)
        {
            int y = bounds.Y + (bounds.Height - 26) / 2;
            return new Rectangle(bounds.Right - 34, y, 26, 26);
        }

        private bool IsPointOnActionButton(Point pt)
        {
            int idx = _listBox.IndexFromPoint(pt);
            if (idx < 0 || idx >= _visibleItems.Count) return false;
            Rectangle rowBounds = _listBox.GetItemRectangle(idx);
            return GetPinButtonRect(rowBounds).Contains(pt) || GetDeleteButtonRect(rowBounds).Contains(pt);
        }

        private bool HandleListActionClick(Point pt)
        {
            int idx = _listBox.IndexFromPoint(pt);
            if (idx < 0 || idx >= _visibleItems.Count) return false;

            Rectangle rowBounds = _listBox.GetItemRectangle(idx);
            if (GetPinButtonRect(rowBounds).Contains(pt))
            {
                _storage.TogglePin(_visibleItems[idx].Id);
                RefreshList();
                return true;
            }

            if (GetDeleteButtonRect(rowBounds).Contains(pt))
            {
                _storage.Delete(_visibleItems[idx].Id);
                RefreshList();
                return true;
            }

            return false;
        }

        private void OnListMouseMove(object sender, MouseEventArgs e)
        {
            int idx = _listBox.IndexFromPoint(e.Location);
            int btn = 0;

            if (idx >= 0 && idx < _visibleItems.Count)
            {
                Rectangle rowBounds = _listBox.GetItemRectangle(idx);
                if (GetPinButtonRect(rowBounds).Contains(e.Location)) btn = 1;
                else if (GetDeleteButtonRect(rowBounds).Contains(e.Location)) btn = 2;
            }

            if (idx != _hoveredIndex || btn != _hoveredButton)
            {
                int prevIdx = _hoveredIndex;
                _hoveredIndex = idx;
                _hoveredButton = btn;
                _listBox.Cursor = btn != 0 ? Cursors.Hand : Cursors.Default;

                if (btn == 1 && idx >= 0 && idx < _visibleItems.Count)
                    _toolTip.SetToolTip(_listBox, _visibleItems[idx].Pinned ? "Desfijar elemento (P)" : "Fijar arriba (P)");
                else if (btn == 2)
                    _toolTip.SetToolTip(_listBox, "Eliminar del historial (Supr)");
                else
                    _toolTip.SetToolTip(_listBox, null);

                if (prevIdx >= 0 && prevIdx < _listBox.Items.Count)
                    _listBox.Invalidate(_listBox.GetItemRectangle(prevIdx));
                if (idx >= 0 && idx < _listBox.Items.Count)
                    _listBox.Invalidate(_listBox.GetItemRectangle(idx));
            }
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _visibleItems.Count) return;
            ClipItem item = _visibleItems[e.Index];
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool rowHovered = e.Index == _hoveredIndex;

            Color bg = selected
                ? Color.FromArgb(219, 234, 254)
                : rowHovered
                ? Color.FromArgb(248, 250, 252)
                : Color.White;

            using (var bgBrush = new SolidBrush(bg))
                e.Graphics.FillRectangle(bgBrush, e.Bounds);

            using (var divPen = new Pen(Color.FromArgb(241, 245, 249), 1))
                e.Graphics.DrawLine(divPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Left type badge
            string tag = item.GetKind() == ClipKind.Image ? "IMG" : item.GetKind() == ClipKind.File ? "ARCH" : "TXT";
            Color badgeBg = item.GetKind() == ClipKind.Image
                ? Color.FromArgb(238, 242, 255)
                : item.GetKind() == ClipKind.File
                ? Color.FromArgb(236, 253, 245)
                : Color.FromArgb(241, 245, 249);
            Color badgeFg = item.GetKind() == ClipKind.Image
                ? Color.FromArgb(67, 56, 202)
                : item.GetKind() == ClipKind.File
                ? Color.FromArgb(4, 120, 87)
                : Color.FromArgb(71, 85, 105);

            var badgeRect = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + (e.Bounds.Height - 20) / 2, 38, 20);
            using (var badgeBrush = new SolidBrush(badgeBg))
            using (var badgeFont = new Font("Consolas", 7.25f, FontStyle.Bold))
            using (var badgeTextBrush = new SolidBrush(badgeFg))
            {
                e.Graphics.FillRectangle(badgeBrush, badgeRect);
                var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(tag, badgeFont, badgeTextBrush, badgeRect, sfCenter);
            }

            // Text column bounded before right action icons (value only, no gray metadata line)
            int textLeft = e.Bounds.X + 56;
            int textRight = e.Bounds.Right - 72;
            int textWidth = Math.Max(60, textRight - textLeft);

            string titleText = item.Title ?? "Elemento";
            using (var titleFont = new Font("Segoe UI", 9.25f, selected || item.Pinned ? FontStyle.Bold : FontStyle.Regular))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    titleText,
                    titleFont,
                    new Rectangle(textLeft, e.Bounds.Y + 4, textWidth, e.Bounds.Height - 8),
                    item.Pinned ? Color.FromArgb(30, 64, 175) : Color.FromArgb(15, 23, 42),
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter
                );
            }

            // Right Action Icon 1: Pin button
            Rectangle pinRect = GetPinButtonRect(e.Bounds);
            bool pinHover = rowHovered && _hoveredButton == 1;
            if (item.Pinned || pinHover)
            {
                Color pinBg = pinHover ? Color.FromArgb(219, 234, 254) : Color.FromArgb(239, 246, 255);
                using (var b = new SolidBrush(pinBg))
                    e.Graphics.FillRectangle(b, pinRect);
            }
            Color pinColor = item.Pinned || pinHover ? Color.FromArgb(37, 99, 235) : Color.FromArgb(148, 163, 184);
            DrawPinIcon(e.Graphics, pinRect, pinColor, item.Pinned);

            // Right Action Icon 2: Delete (Trash) button
            Rectangle delRect = GetDeleteButtonRect(e.Bounds);
            bool delHover = rowHovered && _hoveredButton == 2;
            if (delHover)
            {
                using (var b = new SolidBrush(Color.FromArgb(254, 226, 226)))
                    e.Graphics.FillRectangle(b, delRect);
            }
            Color delColor = delHover ? Color.FromArgb(220, 38, 38) : Color.FromArgb(148, 163, 184);
            DrawTrashIcon(e.Graphics, delRect, delColor);
        }

        private static void DrawPinIcon(Graphics g, Rectangle rect, Color color, bool filled)
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            using (var pen = new Pen(color, 1.5f))
            using (var brush = new SolidBrush(color))
            {
                var head = new Point[]
                {
                    new Point(cx - 4, cy - 5),
                    new Point(cx + 4, cy - 5),
                    new Point(cx + 3, cy),
                    new Point(cx + 5, cy + 2),
                    new Point(cx - 5, cy + 2),
                    new Point(cx - 3, cy)
                };
                if (filled) g.FillPolygon(brush, head);
                else g.DrawPolygon(pen, head);
                g.DrawLine(pen, cx, cy + 2, cx, cy + 7);
            }
        }

        private static void DrawTrashIcon(Graphics g, Rectangle rect, Color color)
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            using (var pen = new Pen(color, 1.5f))
            {
                // Lid + handle
                g.DrawLine(pen, cx - 5, cy - 4, cx + 5, cy - 4);
                g.DrawLine(pen, cx - 2, cy - 6, cx + 2, cy - 6);
                // Bin body
                g.DrawRectangle(pen, cx - 4, cy - 3, 8, 9);
                // Inner vertical bars
                g.DrawLine(pen, cx - 1, cy - 1, cx - 1, cy + 4);
                g.DrawLine(pen, cx + 2, cy - 1, cx + 2, cy + 4);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _boundsSaveTimer?.Stop();
                _boundsSaveTimer?.Dispose();
                _foregroundWatchTimer?.Stop();
                _foregroundWatchTimer?.Dispose();
                _pasteTimer?.Stop();
                _pasteTimer?.Dispose();
                _toolTip?.Dispose();
                _imagePreview?.Image?.Dispose();
                _trayIcon?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

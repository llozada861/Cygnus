using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    /// <summary>
    /// Integración principal de ClipBoardCygnus (.NET Framework 4.7.2) en modo escritorio local con SQLite.
    /// </summary>
    public static class ClipboardManager
    {
        private static SqliteStorageService _storage;
        private static ClipboardMonitor _monitor;
        private static ClipboardPopupForm _popup;
        private static TemplatesForm _templatesForm;
        private static Icon _appIcon;

        public static SqliteStorageService Storage => _storage;

        public static void Initialize(Form mainForm = null, PortableSettings settings = null)
        {
            if (_storage != null) return;

            _storage = new SqliteStorageService(settings ?? new PortableSettings());

            if (_storage.Settings.StartWithWindows != WindowsStartupService.IsEnabled())
            {
                WindowsStartupService.SetEnabled(
                    _storage.Settings.StartWithWindows,
                    _storage.Settings.StartMinimized
                );
            }

            _monitor = new ClipboardMonitor(_storage);

            _appIcon = mainForm?.Icon ?? Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _popup = new ClipboardPopupForm(_storage, _monitor, _appIcon);

            _monitor.ClipCaptured += item => _popup.RefreshList();
            _monitor.HotkeyPressed += () => _popup.TogglePopup();

            Application.ApplicationExit += (s, e) => Shutdown();
        }

        public static void ShowPopup()
        {
            _popup?.TogglePopup();
        }

        public static void ShowPopupForTarget(Form targetForm)
        {
            if (_popup == null || _popup.IsDisposed) return;
            IntPtr targetHwnd = (targetForm != null && !targetForm.IsDisposed) ? targetForm.Handle : IntPtr.Zero;
            _popup.ShowPopupForTargetHandle(targetHwnd);
        }

        public static void TrackActiveOwnWindow(Form form)
        {
            if (_popup == null || _popup.IsDisposed || form == null || form.IsDisposed) return;
            _popup.TrackOwnWindow(form.Handle);
        }

        public static void ShowSettingsWindow()
        {
            if (_popup != null && !_popup.IsDisposed && _popup.Visible)
            {
                _popup.RunWithModalPause(() =>
                {
                    using (var form = new MainSettingsForm(_storage, _monitor))
                    {
                        form.ShowDialog(_popup);
                    }
                });
                return;
            }

            using (var form = new MainSettingsForm(_storage, _monitor))
            {
                form.ShowDialog();
            }
        }

        public static void HidePopup()
        {
            if (_popup != null && !_popup.IsDisposed)
            {
                _popup.HidePopup();
            }
        }

        public static void ShowTemplatesWindow()
        {
            if (_storage == null) return;

            HidePopup();

            if (_templatesForm != null && !_templatesForm.IsDisposed)
            {
                if (_templatesForm.WindowState == FormWindowState.Minimized)
                    _templatesForm.WindowState = FormWindowState.Normal;
                _templatesForm.Show();
                _templatesForm.Activate();
                _templatesForm.BringToFront();
                return;
            }

            _templatesForm = new TemplatesForm(_storage, _appIcon);
            _templatesForm.FormClosed += (s, e) => _templatesForm = null;
            _templatesForm.Show();
            _templatesForm.Activate();
            _templatesForm.BringToFront();
        }

        public static void ApplyRenderedTemplate(string renderedCode, string templateName, bool pasteIntoExternalApp)
        {
            if (string.IsNullOrEmpty(renderedCode)) return;

            if (_popup != null && !_popup.IsDisposed)
            {
                _popup.PasteTextIntoTarget(renderedCode, "Plantilla: " + (templateName ?? "Código"), pasteIntoExternalApp);
                return;
            }

            _monitor?.RestoreTextToSystem(renderedCode);
        }

        public static void Shutdown()
        {
            if (_templatesForm != null && !_templatesForm.IsDisposed)
            {
                _templatesForm.Close();
                _templatesForm.Dispose();
            }
            _templatesForm = null;
            _monitor?.Dispose();
            _popup?.Dispose();
            _monitor = null;
            _popup = null;
            _storage = null;
        }
    }
}

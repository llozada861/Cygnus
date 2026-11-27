using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    public static class WindowsStartupService
    {
        private const string RunRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppKeyName = "ClipBoardCygnus";

        public static bool IsEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunRegistryPath, false))
                {
                    if (key == null) return false;
                    object value = key.GetValue(AppKeyName);
                    return value != null && !string.IsNullOrWhiteSpace(value.ToString());
                }
            }
            catch { return false; }
        }

        public static void SetEnabled(bool enabled, bool startMinimized = true)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunRegistryPath, true))
                {
                    if (key == null) return;
                    if (enabled)
                    {
                        string exePath = Application.ExecutablePath;
                        string args = startMinimized ? " --tray" : string.Empty;
                        key.SetValue(AppKeyName, $"\"{exePath}\"{args}");
                    }
                    else
                    {
                        if (key.GetValue(AppKeyName) != null)
                            key.DeleteValue(AppKeyName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ClipBoardCygnus Startup Error: " + ex.Message);
            }
        }
    }
}

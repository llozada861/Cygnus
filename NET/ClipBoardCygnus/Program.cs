using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "ClipBoardCygnus_SingleInstance_Mutex", out createdNew))
            {
                if (!createdNew) return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Inicializa SQLite portable, bandeja del sistema, monitor y atajo global
                ClipboardManager.Initialize();

                bool startInTray = args != null && args.Contains("--tray");
                if (!startInTray)
                {
                    ClipboardManager.ShowPopup();
                }

                Application.Run();
            }
        }
    }
}

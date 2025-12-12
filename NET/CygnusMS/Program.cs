// C# Application: Mouse KeepAlive with Scheduling, CLI Args, GUI Config, and Service Mode (Skeleton)
// Features added:
// 1. CLI scheduling arguments
// 2. Ready-to-compile EXE structure
// 4. Windows Service mode (ServiceBase skeleton)
// 5. Simple WinForms GUI to set schedule

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ServiceProcess;

namespace MouseKeepAlive
{
    static class Program
    {
        [DllImport("user32.dll")] public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")] public static extern bool GetCursorPos(out POINT p);
        public struct POINT { public int X; public int Y; }

        // Globals
        public static int HoraInicio = 6;
        public static int HoraFin = 16;
        static bool ServiceMode = false;
        static bool GuiMode = true;

        [STAThread]
        static void Main(string[] args)
        {
            ParseArgs(args);

            if (GuiMode)
            {
                Application.EnableVisualStyles();
                Application.Run(new ConfigForm());
                return;
            }

            if (ServiceMode)
            {
                ServiceBase.Run(new KeepAliveService());
                return;
            }

            StartConsoleMode();
        }

        static void ParseArgs(string[] args)
        {
            foreach (var a in args)
            {
                if (a.StartsWith("--inicio=")) HoraInicio = int.Parse(a.Substring(10));
                else if (a.StartsWith("--fin=")) HoraFin = int.Parse(a.Substring(6));
                else if (a == "--gui") GuiMode = true;
                else if (a == "--service") ServiceMode = true;
            }
        }

        public static void StartConsoleMode()
        {
            Console.WriteLine($"Cygnus entre {HoraInicio}:00 y {HoraFin}:00\nCTRL+C para salir.\n");

            while (true)
            {
                DateTime now = DateTime.Now;
                if (now.Hour >= HoraInicio && now.Hour < HoraFin)
                {
                    GetCursorPos(out POINT p);
                    SetCursorPos(p.X + 1, p.Y);
                    Thread.Sleep(150);
                    SetCursorPos(p.X, p.Y);
                    Console.WriteLine($"Movimiento enviado: {now:T}");
                }
                else
                {
                    Console.WriteLine($"Fuera de horario: {now:T}");
                }
                Thread.Sleep(30000);
            }
        }
    }

    // ===============================
    // 4. WINDOWS SERVICE MODE
    // ===============================
    public class KeepAliveService : ServiceBase
    {
        Thread worker;
        bool running = false;

        protected override void OnStart(string[] args)
        {
            running = true;
            worker = new Thread(ServiceLoop);
            worker.Start();
        }

        protected override void OnStop()
        {
            running = false;
        }

        void ServiceLoop()
        {
            while (running)
            {
                DateTime now = DateTime.Now;
                if (now.Hour >= Program.HoraInicio && now.Hour < Program.HoraFin)
                {
                    Program.GetCursorPos(out Program.POINT p);
                    Program.SetCursorPos(p.X + 1, p.Y);
                    Thread.Sleep(150);
                    Program.SetCursorPos(p.X, p.Y);
                }
                Thread.Sleep(30000);
            }
        }
    }

    // ===============================
    // 5. SIMPLE GUI CONFIGURATOR
    // ===============================
    public class ConfigForm : Form
    {
        NumericUpDown nInicio = new NumericUpDown();
        NumericUpDown nFin = new NumericUpDown();
        Button btnSave = new Button();

        public ConfigForm()
        {
            Text = "Cygnus Config";
            Width = 400;
            Height = 200;

            nInicio.Minimum = 0; nInicio.Maximum = 50; nInicio.Value = Program.HoraInicio;
            nFin.Minimum = 0; nFin.Maximum = 50; nFin.Value = Program.HoraFin;
            btnSave.Text = "Guardar";
            btnSave.Top = 80;
            btnSave.Click += (s, e) => Save();

            Controls.Add(new Label() { Text = "Inicio", Top = 10, Left = 10 });
            nInicio.Top = 10; nInicio.Left = 150; Controls.Add(nInicio);
            Controls.Add(new Label() { Text = "Fin", Top = 40, Left = 10 });
            nFin.Top = 40; nFin.Left = 150; Controls.Add(nFin);
            Controls.Add(btnSave);
        }

        void Save()
        {
            Program.HoraInicio = (int)nInicio.Value;
            Program.HoraFin = (int)nFin.Value;
            MessageBox.Show("Configuración guardada.");
        }
    }
}

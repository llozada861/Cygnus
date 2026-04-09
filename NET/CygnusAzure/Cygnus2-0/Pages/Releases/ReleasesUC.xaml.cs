using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cygnus2_0.Pages.Releases
{
    /// <summary>
    /// Lógica de interacción para ReleasesUC.xaml
    /// </summary>
    public partial class ReleasesUC : UserControl
    {
        private Handler handler;
        public ReleasesUC()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            InitializeComponent();
            pAbrirApp();
        }

        private void btnApp_Click(object sender, RoutedEventArgs e)
        {
            pAbrirApp();
        }

        private void pAbrirApp()
        {
            string processToEnd = "Manager de Release";
            try
            {
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    if (process.ProcessName == processToEnd)
                    {
                        process.Kill();
                    }
                }

            }
            catch (Exception)
            { }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "ReleaseMgr.jar ";
            startInfo.Arguments = handler.Azure.Token;
            Process.Start(startInfo);
        }
    }
}

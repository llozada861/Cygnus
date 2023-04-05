using Cygnus2_0.Pages.Objects;
using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
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
using FirstFloor.ModernUI.Windows.Navigation;
using Notifications.Wpf;
using System.Timers;
using Cygnus2_0.General;
using Cygnus2_0.Security;
using Cygnus2_0.ViewModel.Home;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Cygnus2_0.Model.Settings;
using System.Diagnostics;

namespace Cygnus2_0.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl,IContent
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
          (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        private readonly NotificationManager _notificationManager = new NotificationManager();
        private Handler handler;
        private MainWindow myWin;
        private bool settingsReturn, refreshReturn;
        public Home()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            DataContext = handler;
            InitializeComponent();

            //Notificación para nueva versión de Cygnus
            /*var timerUp = new Stopwatch();
            timer.Start();
            pConsultaArchivoVersion();
            timer.Stop();*/
        }        
        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            handler.IsAdmin = false;
            handler.IsEspecialist = false;
            handler.IsUser = false;
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        #region Eventos-Timer
        public void generaNotificacion()
        {
            if (handler.DesblockViewModel != null && handler.ConexionOracle.ConexionOracleSQL != null && handler.ConexionOracle.ConexionOracleSQL.State == System.Data.ConnectionState.Open && handler.DesblockViewModel.ListaArchivosBloqueo.Count > 0)
            {
                foreach (Archivo archivo in handler.DesblockViewModel.ListaArchivosBloqueo)
                {
                    if (DateTime.Today > Convert.ToDateTime(archivo.FechaEstLib))
                    {
                        try
                        {
                            var content = new NotificationContent
                            {
                                Title = "Desbloquear Objeto!",
                                Message = "El objeto [" + archivo.FileName + "] está pendiente por liberar, fecha estimada de liberación [" + archivo.FechaEstLib + "]",
                                Type = NotificationType.Warning
                            };
                            _notificationManager.Show(content, "WindowArea");
                        }
                        catch
                        {
                        }
                    }
                }                        
            }
        }

        public void pConsultaArchivoVersion()
        {

        }

        public void desactivaProxy()
        {
            if (handler.ConfGeneralView.Model.Proxy)
            {
                RegistryKey registry = Registry.CurrentUser.OpenSubKey
                   ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

                registry.SetValue("ProxyEnable", 0);
                registry.SetValue("ProxyServer", 0);
                registry.SetValue("AutoDetect", 0);
                registry.SetValue("AutoConfigURL", 0);

                registry.Close();
                settingsReturn = InternetSetOption
                (IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                refreshReturn = InternetSetOption
                (IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            }
        }
        #endregion Eventos-Timer
    }
}

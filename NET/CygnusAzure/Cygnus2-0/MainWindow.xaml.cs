using Cygnus2_0.General;
using Cygnus2_0.Pages;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
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
using FirstFloor.ModernUI.Windows.Navigation;
using Notifications.Wpf;
using Cygnus2_0.Security;
using Cygnus2_0.ViewModel.Security;
using System.Threading;
using Cygnus2_0.Pages.Settings;
using Cygnus2_0.Pages.Settings.General;
using Cygnus2_0.BaseDatos.sqlite;
using Cygnus2_0.DAO;
using Cygnus2_0.Pages.SolInfo;
using Cygnus2_0.Pages.Settings.AzureData;
using res = Cygnus2_0.Properties.Resources;
using System.Reflection;
using System.IO;
using System.Security;
using Cygnus2_0.Model.Settings;
using System.Globalization;
using Cygnus2_0.Pages.Settings.Git;
using Cygnus2_0.Pages.Settings.Sonar;
using Cygnus2_0.Pages.Settings.AdminGeneral;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Cygnus2_0.Pages.Settings.Database;

namespace Cygnus2_0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private Handler handler;
        UserControl userControls;
        
        public MainWindow()
        {
            InitializeComponent();
            
            try
            {                
                handler = new Handler();

                //Se actualizan datos si hay disponibles
                handler.UpdateModel_.pActualizaDataApp();

                handler.pInicializar();
                pVersion();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public Handler Handler
        {
            get { return handler; }
            set { handler = value; }
        }

        public void pVersion()
        {
            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            string versionBD = handler.ListaConfiguracion.Where(x => x.Text == "VERSION_BD").FirstOrDefault().Value;
                            
            try
            {
                this.Title = "Cygnus [ App: " + fieVersionInfo.FileVersion + " - BD: "+ versionBD+"] - [" + handler.ConfGeneralView.Model.Empresa.Descripcion + 
                 "] - ["+ handler.ConnView.Model.Conexion.Etiqueta + "]";
            }
            catch
            {
                 this.Title = "Cygnus [" + fieVersionInfo.FileVersion + "]";
            }

            handler.fsbVersion = fieVersionInfo.FileVersion;
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string version;
            Boolean actualiza = false;

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-CO");

            //se busca actualización en git
            handler.UpdateModel_.pActualizaApp();

            Next = true;

            try
            {
                if (handler.ConfGeneralView.Model.Empresa == null)
                {
                    userControls = new UCGeneral();
                    RequetInfo request = new RequetInfo(userControls,handler, this, "Antes de empezar, configure la Empresa...",res.KEY_EMPRESA);
                    request.ShowDialog();
                }

                if (handler.ConnView.Model.Usuario == null)
                {
                    userControls = new UCConection(); //(res.Nuevo);
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura la conexión a la base de datos...",res.CONEXION_BD);
                    request.ShowDialog();
                }

                if (handler.ConfGeneralView.Model.RutaSqlplus.Equals(res.RutaSqlplusDefault))
                {
                    userControls = new PathsUserControl();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura la ruta del sqlplus...",res.SQLPLUS);
                    request.ShowDialog();
                }

                if (handler.ConfGeneralView.Model.Empresa.Sonar == res.YES && string.IsNullOrEmpty(handler.RutaSonar))
                {
                    userControls = new UserControlSonar();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura o instala el Sonar...",null);
                    request.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            try
            {
                if (handler.ConfGeneralView.Model.Empresa.Azure == res.No && Next && string.IsNullOrEmpty(handler.ConnView.Model.UsuarioAzure)) 
                {
                    userControls = new Azure();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura el acceso a AzureDevops",null);
                    request.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            if (!actualiza)
            {
                ContentSource = new Uri("/Pages/Home.xaml", UriKind.Relative);                
            }
        }

        private Boolean next;

        public Boolean Next
        {
            get { return next; }
            set { next = value; }
        }

        private void WindowArea_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void pObtParamEntrada()
        {
            string cmdLn = "";
            string up = "";

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                cmdLn += arg;
            }

            if (cmdLn.IndexOf('|') > -1)
            {
                string[] tmpCmd = cmdLn.Split('|');

                for (int i = 1; i < tmpCmd.GetLength(0); i++)
                {
                    if (tmpCmd[i] == "Up") up = tmpCmd[i + 1];
                    i++;
                }
            }
        }

        private string pTraeVersion()
        {
            string file;
            string sbLine;
            string[] substrings;
            char delimiter = ';';
            string Version = "";

            file = System.IO.Path.Combine(Environment.CurrentDirectory, res.ArchivoVersion);

            using (StreamReader streamReader = new StreamReader(file, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();
                substrings = sbLine.Split(delimiter);
                Version = substrings[0]; //Version
            }

            return Version;
        }

        private void pObtenerCredencialesOD(out string url, out string usuario, out string pass, out string rutaversion, out string rutaZip)
        {
            string sbLine;
            string sbLineUnWrap;
            string[] substrings;
            char delimiter = '|';

            url = "";
            usuario = "";
            pass = "";
            rutaversion = "";
            rutaZip = "";

            string ArchivoCred = System.IO.Path.Combine(Environment.CurrentDirectory, res.ArchivoCredenciales);

            using (StreamReader streamReader = new StreamReader(ArchivoCred, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();

                if (!string.IsNullOrEmpty(sbLine))
                {
                    //Se desencripta la linea con las credenciales
                    sbLineUnWrap = EncriptaPass.Desencriptar(sbLine);

                    substrings = sbLineUnWrap.Split(delimiter);
                    url = substrings[0];
                    usuario = substrings[1];
                    pass = substrings[2];
                    rutaversion = substrings[3];
                    rutaZip = substrings[4];
                }
            }
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (handler.GuardarTiempos)
            {
                handler.MensajeError("Hay cambios pendientes por guardar en la hoja de horas.");
                e.Cancel = true;
            }
        }
    }
}
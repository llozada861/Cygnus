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
            //pObtParamEntrada();

            string fechaExp = "01/08/2021";
            
            if (DateTime.Today > Convert.ToDateTime(fechaExp))
            {
                System.Windows.MessageBox.Show("La licencia de uso de Cygnus ha vencido. Por favor contacta al admin para renovar tú licencia.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            InitializeComponent();

            try
            {                
                handler = new Handler();                
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
            this.Title = "Cygnus [version " + fieVersionInfo.FileVersion + "]";
            handler.fsbVersion = fieVersionInfo.FileVersion;
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string version;
            Boolean actualiza = false;

            Next = true;

            if (Next && handler.ConnViewModel.Usuario.ToUpper().Equals(res.UsuarioSQLDefault) || string.IsNullOrEmpty(handler.ConnViewModel.Usuario)) //while (handler.ConnViewModel.Usuario.ToUpper().Equals(res.UsuarioSQLDefault) || string.IsNullOrEmpty(handler.ConnViewModel.Usuario))
            {
                userControls = new UCConection();
                RequetInfo request = new RequetInfo(userControls, this, "Antes de empezar configura la conexión a la base de datos...");
                request.ShowDialog();
            }

            if (Next && handler.ConfGeneralViewModel.RutaSqlplus.Equals(res.RutaSqlplusDefault)) //while (handler.ConfGeneralViewModel.RutaSqlplus.Equals(res.RutaSqlplusDefault))
            {
                userControls = new PathsUserControl();
                RequetInfo request = new RequetInfo(userControls, this, "Antes de empezar configura la ruta del sqlplus");
                request.ShowDialog();
            }

            try
            {
                handler.pRealizaConexion();

                if (Next && string.IsNullOrEmpty(handler.ConnViewModel.UsuarioAzure)) //while (string.IsNullOrEmpty(handler.ConnViewModel.Correo) || handler.ConnViewModel.ListaAreaAzure.Count == 0)
                {
                    userControls = new Azure();
                    RequetInfo request = new RequetInfo(userControls, this, "Antes de empezar configura el acceso a AzureDevops");
                    request.ShowDialog();
                }

                //Se valida si se debe actualizar automáticamente
                if (handler.ConexionOracle.ConexionOracleSQL != null)
                {
                    if (handler.ConexionOracle.ConexionOracleSQL.State == System.Data.ConnectionState.Open)
                    {
                        version = handler.DAO.pObtCodigoVersion();

                        if (!handler.fsbVersion.Equals(version))
                        {
                            try
                            {
                                actualiza = true;
                                UpdateModel.pDescargarActualizacion(handler.ConnViewModel.Usuario,handler.ConnViewModel.Pass, version,handler.ConnViewModel.Servidor, handler.ConnViewModel.BaseDatos, handler.ConnViewModel.Puerto);
                                this.Close();
                            }
                            catch { }
                        }
                    }
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

        /*private void pDescargaOneDrive(string archivo, string ruta, Service service, string archivoDescarga)
        {
            Stream inputStream = service.GetFileStream(archivoDescarga);

            FileStream outputStream = new FileStream(ruta + "\\" + archivo, FileMode.CreateNew);

            using (inputStream)
            {
                using (outputStream)
                {
                    byte[] buffer = new byte[8192];
                    int len = 0;

                    while ((len = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, len);
                    }
                }
            }
        }*/

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

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
            pInstalarActuaLocal();

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

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-CO");

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
                RequetInfo request = new RequetInfo(userControls, this, "Antes de empezar configura la ruta del sqlplus...");
                request.ShowDialog();
            }

            if (string.IsNullOrEmpty(handler.RutaGitObjetos) || string.IsNullOrEmpty(handler.RutaGitDatos) || string.IsNullOrEmpty(handler.RutaGitBash))
            {
                userControls = new UserControlGit();
                RequetInfo request = new RequetInfo(userControls, this, "Antes de empezar configura o clona el repositorio Git...");
                request.ShowDialog();
            }

            if (string.IsNullOrEmpty(handler.RutaSonar))
            {
                userControls = new UserControlSonar();
                RequetInfo request = new RequetInfo(userControls, this, "Antes de empezar configura o instala el Sonar...");
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

            try
            {
                string fechaExp = "31/12/2021";

                if (DateTime.Today > Convert.ToDateTime(fechaExp))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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

        public void pInstalarActuaLocal()
        {
            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);

            if (fieVersionInfo.FileVersion.Equals("1.0.6.4"))
            {
                string[] query = {"drop table comments",
                            "drop table comment_type",
                            "drop table packages_comments",
                            "drop table packages",
                            "drop table statement",
                            "alter table object_type ADD column path TEXT",
                            "alter table object_head ADD column company INTEGER",
                            "alter table conection ADD column company INTEGER",
                            "alter table configuration ADD column company INTEGER",
                            "alter table documentation ADD column company INTEGER",
                            "alter table html ADD column company INTEGER",
                            "alter table object_type ADD column company INTEGER",
                            "alter table paths ADD column company INTEGER",
                            "alter table user_grants ADD column company INTEGER",
                            "alter table userbd ADD column company INTEGER",
                            "alter table version ADD column company INTEGER",
                            "create table company (codigo integer, descripcion text)",
                            "insert into company (codigo,descripcion) values (99,'EPM')",
                            "insert into object_type (object,count_slash,priority,grant) values('llave_primaria',0,110,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('llave_foranea',0,111,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('llave_unica',0,113,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('vista_mat',0,114,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('job',0,401,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('grant',0,402,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('EA',0,501,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('GI',0,502,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('GR',0,503,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('MD',0,504,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('OB',0,505,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('OP',0,506,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('PB',0,507,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('PI',0,508,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('RU',0,509,'No')",
                            "insert into object_type (object,count_slash,priority,grant) values('TC',0,510,'No')",
                            "update conection set company = 99",
                            "update configuration set company = 99",
                            "update documentation set company = 99",
                            "update html set company = 99",
                            "update object_type set company = 99",
                            "update paths set company = 99",
                            "update conection set company = 99",
                            "update user_grants set company = 99",
                            "update userbd set company = 99",
                            "update version set company = 99",
                            "update conection set company = 99",
                            "update object_type set path = '\\server\\sql\\00dir\\[nombre]\\' where object = 'directorio'",
                            "update object_type set path = '\\server\\sql\\01seq\\[nombre]\\' where object = 'secuencia'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\00tbl\\' where object = 'tabla'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\00tbl\\' where object = 'alter'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\00tbl\\' where object = 'drop'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\01pkey\\' where object = 'llave_primaria'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\02idx\\' where object = 'indice'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\03fkey\\' where object = 'llave_foranea'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\04chk\\' where object = 'llave_unica'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\05trg\\' where object = 'trigger'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\06data\\' where object = 'insert'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\06data\\' where object = 'update'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\06data\\' where object = 'delete'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\06data\\' where object = 'merge'",
                            "update object_type set path = '\\server\\sql\\02tbls\\[nombre]\\11syn\\' where object = 'sinonimo'",
                            "update object_type set path = '\\server\\sql\\03fnc\\[nombre]\\' where object = 'Funcion'",
                            "update object_type set path = '\\server\\sql\\04proc\\[nombre]\\' where object = 'procedimiento'",
                            "update object_type set path = '\\server\\sql\\05pkg\\[nombre]\\' where object = 'paquete'",
                            "update object_type set path = '\\server\\sql\\06view\\[nombre]\\' where object = 'vista'",
                            "update object_type set path = '\\server\\sql\\07vwmat\\[nombre]\\' where object = 'vista_mat'",
                            "update object_type set path = '\\server\\sql\\09job\\[nombre]\\' where object = 'job'",
                            "update object_type set path = '\\server\\sql\\10grt\\[nombre]\\' where object = 'grant'",
                            "update object_type set path = '\\client\\framework\\EA\\[nombre]\\' where object = 'EA'",
                            "update object_type set path = '\\client\\framework\\GI\\[nombre]\\' where object = 'GI'",
                            "update object_type set path = '\\client\\framework\\GR\\[nombre]\\' where object = 'GR'",
                            "update object_type set path = '\\client\\framework\\MD\\[nombre]\\' where object = 'MD'",
                            "update object_type set path = '\\client\\framework\\OB\\[nombre]\\' where object = 'OB'",
                            "update object_type set path = '\\client\\framework\\OP\\[nombre]\\' where object = 'OP'",
                            "update object_type set path = '\\client\\framework\\PB\\[nombre]\\' where object = 'PB'",
                            "update object_type set path = '\\client\\framework\\PI\\[nombre]\\' where object = 'PI'",
                            "update object_type set path = '\\client\\framework\\RU\\[nombre]\\' where object = 'RU'",
                            "update object_type set path = '\\client\\framework\\TC\\[nombre]\\' where object = 'TC'",
                            "delete from object_type where object = 'otros'"
                };

                /*foreach (string sql in query)
                {
                    SqliteDAO.pExecuteNonQuery(sql);
                }*/
            }
        }
    }
}

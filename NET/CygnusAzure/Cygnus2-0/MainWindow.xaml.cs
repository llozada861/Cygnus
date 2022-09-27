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

            InitializeComponent();

            try
            {                
                handler = new Handler();

                pInstalarActuaLocal();

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
                            
            try
            {
                this.Title = "Cygnus [" + fieVersionInfo.FileVersion + "] - Empresa [" + handler.ConfGeneralView.Model.Empresa.Descripcion + "] - Base Datos ["+ handler.ConnView.Model.Conexion.Etiqueta + "]";
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
                    userControls = new UCConection(res.Nuevo);
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

                /*if (string.IsNullOrEmpty(handler.Azure.Usuario))
                {
                    userControls = new Azure();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura Azure...", null);
                    request.ShowDialog();
                }*/
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            try
            {
                handler.pRealizaConexion();

                if (handler.ConfGeneralView.Model.Empresa.Azure == res.No && Next && string.IsNullOrEmpty(handler.ConnView.Model.UsuarioAzure)) 
                {
                    userControls = new Azure();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura el acceso a AzureDevops",null);
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

                                UpdateModel.pDescargarActualizacion(handler.ConnView.Model.Usuario,handler.ConnView.Model.Pass, version,handler.ConnView.Model.Servidor, handler.ConnView.Model.BaseDatos, handler.ConnView.Model.Puerto);
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

            //versiones progresivas
            if (!SqliteDAO.pblValidaVersion("1.1.3.6") && !SqliteDAO.pEsNueva())
            {
                string[] query =
                 {
                    "delete from object_path",
                    "DROP table object_type",
                    "CREATE TABLE object_type(codigo INTEGER PRIMARY KEY AUTOINCREMENT, object TEXT,slash TEXT,count_slash INTEGER,    priority    INTEGER)",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('1', 'FUNCION', 'S', '1', '300')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('2', 'PROCEDIMIENTO', 'S', '1', '301')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('3', 'PAQUETE', 'S', '1', '302')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('4', 'TRIGGER', 'S', '1', '303')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('5', 'TABLA', 'N', '0', '100')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('6', 'SECUENCIA', 'N', '0', '104')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('7', 'VISTA', 'N', '0', '103')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('8', 'INDICE', 'N', '0', '101')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('9', 'SINONIMO', 'N', '0', '108')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('10', 'ALTER', 'N', '0', '105')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('11', 'DROP', 'N', '0', '1')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('12', 'INSERT', 'N', '0', '201')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('13', 'UPDATE', 'N', '0', '202')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('14', 'DELETE', 'N', '0', '200')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('15', 'SCRIPT', 'S', '1', '400')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('16', 'OTROS', 'N', '0', '500')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('17', 'MERGE', 'N', '0', '203')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('18', 'LLAVE_PRIMARIA', '', '0', '110')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('19', 'LLAVE_FORANEA', '', '0', '111')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('20', 'LLAVE_UNICA', '', '0', '113')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('21', 'VISTA_MAT', '', '0', '114')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('22', 'JOB', '', '0', '401')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('23', 'GRANT', '', '0', '402')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('24', 'EA', '', '0', '501')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('25', 'GI', '', '0', '502')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('26', 'GR', '', '0', '503')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('27', 'MD', '', '0', '504')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('28', 'OB', '', '0', '505')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('29', 'OP', '', '0', '506')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('30', 'PB', '', '0', '507')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('31', 'PI', '', '0', '508')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('32', 'RU', '', '0', '509')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('33', 'TC', '', '0', '510')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('34', 'APLICA', '', '0', '200')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('1', '1', '[usuario]\\server\\sql\\03fnc\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('2', '2', '[usuario]\\server\\sql\\04proc\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('3', '3', '[usuario]\\server\\sql\\05pkg\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('4', '4', '[usuario]\\server\\sql\\02tbls\\[nombre]\\05trg', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('5', '5', '[usuario]\\server\\sql\\02tbls\\[nombre]\\00tbl', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('6', '6', '[usuario]\\server\\sql\\01seq\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('7', '7', '[usuario]\\server\\sql\\06view\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('8', '8', '[usuario]\\server\\sql\\02tbls\\[nombre]\\02idx', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('9', '9', '[usuario]\\server\\sql\\02tbls\\[nombre]\\11syn', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('10', '10', '[usuario]\\server\\sql\\02tbls\\[nombre]\\00tbl', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('11', '11', '[usuario]\\server\\sql\\02tbls\\[nombre]\\00tbl', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('12', '12', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('13', '13', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('14', '14', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('16', '16', 'Despliegues\\[hu]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('17', '17', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('18', '18', '[usuario]\\server\\sql\\02tbls\\[nombre]\\01pkey', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('19', '19', '[usuario]\\server\\sql\\02tbls\\[nombre]\\03fkey', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('20', '20', '[usuario]\\server\\sql\\02tbls\\[nombre]\\04chk', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('21', '21', '[usuario]\\server\\sql\\07vwmat\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('22', '22', '[usuario]\\server\\sql\\09job\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('23', '23', '[usuario]\\server\\sql\\10grt\\[nombre]', '', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('24', '24', 'client\\framework\\EA\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('25', '25', 'client\\framework\\GI\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('26', '26', 'client\\framework\\GR\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('27', '27', 'client\\framework\\MD\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('28', '28', 'client\\framework\\OB\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('29', '29', 'client\\framework\\OP\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('30', '30', 'client\\framework\\PB\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('31', '31', 'client\\framework\\PI\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('32', '32', 'client\\framework\\RU\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('33', '33', 'client\\framework\\TC\\[nombre]', 'FLEX', '99')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('34', '34', 'Despliegues\\[hu]', '', '99')",
                    "drop table user_grants",
                    "CREATE TABLE user_grants (codigo	INTEGER PRIMARY KEY AUTOINCREMENT,user	TEXT,company	INTEGER)",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('1', 'SYSTEM_OBJ_PRIVS_ROLE', '99')",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('2', 'FLEX', '99')",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('3', 'FLEX_CUSTOMIZACION', '99')",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('4', 'CONSULTA_TODAS_LAS_TABLAS', '99')",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('5', 'OPENSIRIUS', '99')",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('6', 'EJECUTA_TODOS_LOS_PROC', '99')",
                    "INSERT INTO user_grants (codigo, user, company) VALUES ('7', 'MODIFICA_TODAS_LAS_TABLAS', '99')",
                    "CREATE TABLE grants (	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,	descripcion	TEXT)",
                    "INSERT INTO grants (codigo, descripcion) VALUES ('1', 'EXECUTE')",
                    "INSERT INTO grants (codigo, descripcion) VALUES ('2', 'SELECT')",
                    "INSERT INTO grants (codigo, descripcion) VALUES ('3', 'UPDATE')",
                    "INSERT INTO grants (codigo, descripcion) VALUES ('4', 'DELETE')",
                    "INSERT INTO grants (codigo, descripcion) VALUES ('5', 'INSERT')",
                    "drop table object_head",
                    "CREATE TABLE object_head ( Codigo INTEGER PRIMARY KEY AUTOINCREMENT,head    TEXT,type    INTEGER,priority    INTEGER,end    TEXT,company    INTEGER,FOREIGN KEY(type) REFERENCES object_type(codigo))",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('2', 'CREATE OR REPLACE FUNCTION', '1', '1', 'END')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('3', 'CREATE OR REPLACE PROCEDURE', '2', '2', 'END')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('4', 'CREATE OR REPLACE PACKAGE', '3', '3', 'END')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('5', 'CREATE OR REPLACE TRIGGER', '4', '4', 'END')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('6', 'CREATE TABLE', '5', '5', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('7', 'CREATE SEQUENCE', '6', '6', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('8', 'CREATE OR REPLACE VIEW', '7', '7', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('9', 'CREATE INDEX', '8', '8', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('10', 'CREATE SYNONYM', '9', '9', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('11', 'CREATE PUBLIC SYNONYM', '9', '10', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('12', 'ALTER TABLE', '10', '12', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('13', 'DROP TABLE', '11', '13', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('14', 'DROP PACKAGE', '11', '14', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('15', 'DROP INDEX', '11', '15', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('16', 'DROP SYNONYM', '11', '16', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('17', 'INSERT INTO', '12', '17', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('18', 'UPDATE', '13', '18', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('19', 'DELETE FROM', '14', '19', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('20', 'MERGE INTO', '17', '21', 'PC')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('21', 'DECLARE', '15', '23', 'END')",
                    "INSERT INTO object_head (Codigo, head, type, priority, end) VALUES ('22', 'BEGIN', '15', '24', 'END')",
                    "CREATE TABLE object_grants ( codigo INTEGER PRIMARY KEY AUTOINCREMENT, objeto INTEGER, usuario_grant INTEGER, permiso INTEGER, company INTEGER, FOREIGN KEY(objeto) REFERENCES object_type(codigo), FOREIGN KEY(usuario_grant) REFERENCES user_grants(codigo), FOREIGN KEY(company) REFERENCES company(codigo), FOREIGN KEY(permiso) REFERENCES grants(codigo))",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('2', '1', '1', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('3', '1', '2', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('4', '1', '3', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('5', '1', '5', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('7', '2', '2', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('8', '2', '3', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('9', '2', '5', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('10', '2', '1', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('12', '3', '2', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('13', '3', '3', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('14', '3', '5', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('15', '3', '1', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('16', '3', '6', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('17', '2', '6', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('18', '1', '6', '1', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('19', '5', '2', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('20', '5', '2', '3', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('21', '5', '2', '4', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('22', '5', '2', '5', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('23', '5', '3', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('24', '5', '3', '3', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('25', '5', '3', '4', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('26', '5', '3', '5', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('27', '5', '5', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('28', '5', '5', '3', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('29', '5', '5', '4', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('30', '5', '5', '5', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('31', '5', '4', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('32', '5', '7', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('33', '5', '7', '3', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('34', '5', '7', '4', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('35', '5', '7', '5', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('36', '6', '2', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('37', '6', '3', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('38', '6', '5', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('39', '6', '1', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('40', '7', '2', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('41', '7', '3', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('42', '7', '5', '2', '99')",
                    "INSERT INTO object_grants (codigo, objeto, usuario_grant, permiso, company) VALUES ('43', '7', '1', '2', '99')",
                    "drop table documentation",
                    "CREATE TABLE documentation ( codigo INTEGER PRIMARY KEY AUTOINCREMENT, tag_ini TEXT, tag_fin TEXT, attributes TEXT, type TEXT, end TEXT, company INTEGER)",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('1', '<package', '</package>', 'Fuente', 'P', '>', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('2', '<procedure', '</procedure>', 'Fuente', 'P', '>', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('3', '<unidad', '</unidad>', '', 'S', '', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('4', '<descripcion', '</descripcion>', '', 'S', '', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('5', '<autor', '</autor>', '', 'S', '', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('6', '<fecha', '</fecha>', '', 'S', '', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('7', '<retorno', '</retorno>', 'Nombre|Tipo', 'S', '>', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('8', '<param', '</param>', 'Nombre|Tipo|Direccion', 'S', '>', '99')",
                    "INSERT INTO documentation (codigo, tag_ini, tag_fin, attributes, type, end, company) VALUES ('9', '<modificacion', '</modificacion>', 'Autor|Fecha|Inc', 'S', '>', '99')"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion("1.1.3.6");
            }

            if (!SqliteDAO.pblValidaVersion("1.1.3.8") && !SqliteDAO.pEsNueva())
            {
                SqliteDAO.pCreaConfiguracion(res.KeyProyectoSonar, "Open.BaseDeDatos.Actualizacion");

                SqliteDAO.pActualizaVersion("1.1.3.8");
            }

            if (!SqliteDAO.pblValidaVersion("1.1.4.2") && !SqliteDAO.pEsNueva())
            {
                string[] query =
                 {
                    "INSERT INTO words (description) VALUES ('drop')",
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion("1.1.4.2");
            }

            if (!SqliteDAO.pblValidaVersion("1.1.4.3") && !SqliteDAO.pEsNueva())
            {
                string[] query =
                 {
                    "delete from object_grants where codigo = 31",
                    "delete from user_grants where codigo = 4"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion("1.1.4.3");
            }

            //ultima versión
            if (!SqliteDAO.pblValidaVersion(fieVersionInfo.FileVersion))
            {
                //SqliteDAO.pCreaConfiguracion(res.KEY_EMPRESA, "99");
                //handler.pRegeneraIndexListas();

                SqliteDAO.pCreaConfiguracion(res.KEY_LLAVEW, @"HKEY_CURRENT_USER\Software\Classes\osfweb\shell\open\command");

                //Se actualiza la versión
                SqliteDAO.pActualizaVersion(fieVersionInfo.FileVersion);                
            }
        }
    }
}
/*"drop table repositories",
                        "CREATE TABLE repositories (codigo	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	descripcion	TEXT,	documento	TEXT,	ruta_local	TEXT,empresa	INTEGER,	FOREIGN KEY(empresa) REFERENCES company(codigo))",
                        "drop table repository_branch",
                        "CREATE TABLE repository_branch (codigo	INTEGER PRIMARY KEY AUTOINCREMENT,repositorio_id	INTEGER,rama	TEXT,estandar TEXT, lbase TEXT)",
                        RepoObj,
                        RepoDat,
                        "insert into repository_branch (repositorio_id,rama,estandar,lbase) values (1,'desarrollo','feature/[HU]_[USUARIO]_DLL','N')",
                        "insert into repository_branch (repositorio_id,rama,estandar,lbase) values (1,'pruebas','feature/[HU]_[USUARIO]_PRU','N')",
                        "insert into repository_branch (repositorio_id,rama,estandar,lbase) values (1,'produccion','feature/[HU]_[USUARIO]_PDN','S')",
                        "insert into repository_branch (repositorio_id,rama,estandar,lbase) values (2,'master-datos','feature/[HU]_[USUARIO]','S')",
                        "delete from object_path",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('1', '1', '[usuario]\\server\\sql\\03fnc\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('2', '2', '[usuario]\\server\\sql\\04proc\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('3', '3', '[usuario]\\server\\sql\\05pkg\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('4', '4', '[usuario]\\server\\sql\\02tbls\\[nombre]\\05trg', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('5', '5', '[usuario]\\server\\sql\\02tbls\\[nombre]\\00tbl', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('6', '6', '[usuario]\\server\\sql\\01seq\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('7', '7', '[usuario]\\server\\sql\\06view\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('8', '8', '[usuario]\\server\\sql\\02tbls\\[nombre]\\02idx', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('9', '9', '[usuario]\\server\\sql\\02tbls\\[nombre]\\11syn', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('10', '10', '[usuario]\\server\\sql\\02tbls\\[nombre]\\00tbl', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('11', '11', '[usuario]\\server\\sql\\02tbls\\[nombre]\\00tbl', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('12', '12', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('13', '13', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('14', '14', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('15', '15', '', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('16', '16', 'Despliegues\\[hu]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('17', '17', '[usuario]\\server\\sql\\02tbls\\[nombre]\\06data', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('18', '18', '[usuario]\\server\\sql\\02tbls\\[nombre]\\01pkey', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('19', '19', '[usuario]\\server\\sql\\02tbls\\[nombre]\\03fkey', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('20', '20', '[usuario]\\server\\sql\\02tbls\\[nombre]\\04chk', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('21', '21', '[usuario]\\server\\sql\\07vwmat\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('22', '22', '[usuario]\\server\\sql\\09job\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('23', '23', '[usuario]\\server\\sql\\10grt\\[nombre]', '', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('24', '24', 'client\\framework\\EA\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('25', '25', 'client\\framework\\GI\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('26', '26', 'client\\framework\\GR\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('27', '27', 'client\\framework\\MD\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('28', '28', 'client\\framework\\OB\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('29', '29', 'client\\framework\\OP\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('30', '30', 'client\\framework\\PB\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('31', '31', 'client\\framework\\PI\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('32', '32', 'client\\framework\\RU\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('33', '33', 'client\\framework\\TC\\[nombre]', 'FLEX', '99')",
                        "INSERT INTO object_type (codigo, object, slash, count_slash, priority, grant) VALUES ('34', 'Aplica', '', '0', '200', 'No')",
                        "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('34', '34', 'Despliegues\\[hu]', '', '99')"
                        "CREATE TABLE story_user ( 	codigo	INTEGER, 	descripcion	TEXT, 	usuario	TEXT, 	empresa	INTEGER, 	FOREIGN KEY(empresa) REFERENCES company(codigo), 	PRIMARY KEY(codigo) )",
                        "CREATE TABLE task_user ( 	codigo	INTEGER PRIMARY KEY AUTOINCREMENT, 	descripcion	TEXT, 	estado	TEXT, 	fecha_actualiza	TEXT, 	usuario	TEXT, 	completado	NUMERIC, 	fecha_display	TEXT, 	fecha_registro	TEXT, 	hist_usuario	INTEGER, 	fecha_inicio	TEXT, 	empresa	INTEGER, 	FOREIGN KEY(empresa) REFERENCES company(codigo), 	FOREIGN KEY(hist_usuario) REFERENCES story_user(codigo) )",
                        "CREATE TABLE week ( 	codigo	INTEGER PRIMARY KEY AUTOINCREMENT, 	fecha_ini	TEXT, 	fecha_fin	TEXT, 	descripcion	TEXT )",
                        "CREATE TABLE timexweek ( 	codigo	INTEGER PRIMARY KEY AUTOINCREMENT, 	id_hoja	INTEGER, 	fecha_registro	TEXT, 	usuario	BLOB, 	lunes	NUMERIC, 	martes	NUMERIC, 	miercoles	NUMERIC, 	jueves	NUMERIC, 	viernes	NUMERIC, 	sabado	NUMERIC, 	domingo	NUMERIC, 	fecha_actualiza	TEXT, 	observacion	TEXT, 	requerimiento	INTEGER, 	FOREIGN KEY(id_hoja) REFERENCES week(codigo), 	FOREIGN KEY(requerimiento) REFERENCES task_user(codigo) )",
                        "CREATE TABLE sequence ( 	codigo	INTEGER PRIMARY KEY AUTOINCREMENT ) ",
                        "insert into sequence (codigo) values (null)",
                        "CREATE TABLE task_pred ( 	codigo	INTEGER, 	descripcion	TEXT, 	PRIMARY KEY(codigo) )",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-1, 'Vacaciones')",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-2, 'Beneficio')",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-3, 'Calamidad')",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-4, 'Incapacidad')",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-5, 'Permiso')",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-6, 'Compensatorio')",
                        "insert into task_pred (CODIGO, DESCRIPCION) values (-7, 'Día Familia')",
                        "INSERT INTO azure (codigo,usuario, correo, dias, url, empresa, defecto, token, proyecto) VALUES (1,'', '', '10', 'https://grupoepm.visualstudio.com', '99', 'S', '"+res.TokenAzureConn+"', 'OPEN')",
                        "alter table company add defecto text",
                        "update company set defecto = 'Y' where codigo = 99"
                        "DROP table object_type",
                        "CREATE TABLE object_type (    codigo  INTEGER PRIMARY KEY AUTOINCREMENT,    object  TEXT,    slash   TEXT,    count_slash INTEGER,    priority    TEXT,    grant   BLOB)",
                        "CREATE TABLE object_path (    codigo  INTEGER PRIMARY KEY AUTOINCREMENT,    object_type INTEGER,    path    TEXT,    user_default    TEXT,    company INTEGER,    FOREIGN KEY(object_type) REFERENCES object_type(codigo))",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (1,'Funcion','S',1,300,'E')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (2,'procedimiento','S',1,301,'E')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (3,'paquete','S',1,302,'E')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (4,'trigger','S',1,303,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (5,'tabla','N',0,100,'S|I|U|D')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (6,'secuencia','N',0,104,'S')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (7,'vista','N',0,103,'S')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (8,'indice','N',0,101,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (9,'sinonimo','N',0,108,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (10,'alter','N',0,105,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (11,'drop','N',0,1,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (12,'insert','N',0,201,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (13,'update','N',0,202,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (14,'delete','N',0,200,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (15,'script','S',1,400,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (16,'otros','N',0,500,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (17,'merge','N',0,203,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (18,'llave_primaria','',0,110,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (19,'llave_foranea','',0,111,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (20,'llave_unica','',0,113,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (21,'vista_mat','',0,114,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (22,'job','',0,401,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (23,'grant','',0,402,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (24,'EA','',0,501,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (25,'GI','',0,502,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (26,'GR','',0,503,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (27,'MD','',0,504,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (28,'OB','',0,505,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (29,'OP','',0,506,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (30,'PB','',0,507,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (31,'PI','',0,508,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (32,'RU','',0,509,'N')",
                        "insert into object_type (codigo,object,slash,count_slash,priority,grant) values (33,'TC','',0,510,'N')",
                        "insert into object_path (object_type,path,user_default,company) values (1,'server\\sql\\03fnc\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (2,'server\\sql\\04proc\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (3,'server\\sql\\05pkg\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (4,'server\\sql\\02tbls\\[nombre]\\05trg','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (5,'server\\sql\\02tbls\\[nombre]\\00tbl','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (6,'server\\sql\\01seq\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (7,'server\\sql\\06view\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (8,'server\\sql\\02tbls\\[nombre]\\02idx','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (9,'server\\sql\\02tbls\\[nombre]\\11syn','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (10,'server\\sql\\02tbls\\[nombre]\\00tbl','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (11,'server\\sql\\02tbls\\[nombre]\\00tbl','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (12,'server\\sql\\02tbls\\[nombre]\\06data','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (13,'server\\sql\\02tbls\\[nombre]\\06data','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (14,'server\\sql\\02tbls\\[nombre]\\06data','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (15,'','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (16,'','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (17,'server\\sql\\02tbls\\[nombre]\\06data','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (18,'server\\sql\\02tbls\\[nombre]\\01pkey','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (19,'server\\sql\\02tbls\\[nombre]\\03fkey','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (20,'server\\sql\\02tbls\\[nombre]\\04chk','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (21,'server\\sql\\07vwmat\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (22,'server\\sql\\09job\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (23,'server\\sql\\10grt\\[nombre]','',99)",
                        "insert into object_path (object_type,path,user_default,company) values (24,'client\\framework\\EA\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (25,'client\\framework\\GI\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (26,'client\\framework\\GR\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (27,'client\\framework\\MD\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (28,'client\\framework\\OB\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (29,'client\\framework\\OP\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (30,'client\\framework\\PB\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (31,'client\\framework\\PI\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (32,'client\\framework\\RU\\[nombre]','FLEX',99)",
                        "insert into object_path (object_type,path,user_default,company) values (33,'client\\framework\\TC\\[nombre]','FLEX',99)",
                        "INSERT INTO configuration (key, value) VALUES ('SQLPLUS', (select path from paths where name = 'SQLPLUS'))",
                        "INSERT INTO configuration (key,value) VALUES ('EMPRESA','-')",
                        */

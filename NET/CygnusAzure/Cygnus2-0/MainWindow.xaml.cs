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
                this.Title = "Cygnus [" + fieVersionInfo.FileVersion + "] - Empresa [" + handler.ConfGeneralView.Model.Empresa.Text + "] - Base Datos ["+ handler.ConnView.Model.Conexion.Etiqueta + "]";
            }
            catch
            {
                 this.Title = "Cygnus [" + fieVersionInfo.FileVersion + "]";
            }

            handler.fsbVersion = fieVersionInfo.FileVersion;
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pInstalarActuaLocal();

            string version;
            Boolean actualiza = false;

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-CO");

            Next = true;

            try
            {
                if (handler.ConfGeneralView.Model.Empresa == null || string.IsNullOrEmpty(handler.ConfGeneralView.Model.Empresa.Value) || handler.ConfGeneralView.Model.Empresa.Value == "-")
                {
                    userControls = new UCGeneral();
                    RequetInfo request = new RequetInfo(userControls,handler, this, "Antes de empezar, configure la Empresa...",res.KEY_EMPRESA);
                    request.ShowDialog();
                }

                if (Next && handler.ConnView.Model.Usuario == null || handler.ConnView.Model.Usuario.ToUpper().Equals(res.UsuarioSQLDefault) || string.IsNullOrEmpty(handler.ConnView.Model.Usuario))
                {
                    userControls = new UCConection(res.Nuevo);
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura la conexión a la base de datos...",res.CONEXION_BD);
                    request.ShowDialog();
                }

                if (Next && handler.ConfGeneralView.Model.RutaSqlplus.Equals(res.RutaSqlplusDefault))
                {
                    userControls = new PathsUserControl();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura la ruta del sqlplus...",res.SQLPLUS);
                    request.ShowDialog();
                }

                if (handler.ConfGeneralView.Model.Empresa.Git && string.IsNullOrEmpty(handler.RutaGitObjetos) || string.IsNullOrEmpty(handler.RutaGitDatos) || string.IsNullOrEmpty(handler.RutaGitBash))
                {
                    userControls = new UserControlGit();
                    RequetInfo request = new RequetInfo(userControls, handler, this, "Antes de empezar, configura el repositorio Git...",null);
                    request.ShowDialog();
                }

                if (handler.ConfGeneralView.Model.Empresa.Sonar && string.IsNullOrEmpty(handler.RutaSonar))
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
                handler.pRealizaConexion();

                if (handler.ConfGeneralView.Model.Empresa.Azure && Next && string.IsNullOrEmpty(handler.ConnView.Model.UsuarioAzure)) 
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
            if (!SqliteDAO.pblValidaVersion(handler))
            {
                string[] query = 
                    {
                        /*"DROP table object_type",
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
                        "drop table company",
                        "CREATE TABLE company (    codigo  INTEGER PRIMARY KEY AUTOINCREMENT,    descripcion text,    azure   TEXT,    git TEXT,    sonar   TEXT)",
                        "insert into company (codigo,descripcion,azure,git,sonar) values (99,'EPM','Y','Y','Y')",
                        "ALTER TABLE conection RENAME TO old_conection",
                        "CREATE TABLE conection (    user    TEXT,    pass	TEXT,    bd	TEXT,    server  TEXT,    port    TEXT,    active  TEXT,    company INTEGER,    PRIMARY KEY(user,bd))",
                        "INSERT INTO conection SELECT * FROM old_conection",
                        "update conection set company = 99",
                        "drop table paths",
                        "drop table old_conection",
                        "alter table company add documentoad TEXT",
                        "update company set documentoad = 'OpenEPM10_Analisis_Diseno' where codigo = 99"
                        "ALTER TABLE conection RENAME TO old_conection",
                        "CREATE TABLE conection (user  TEXT,pass TEXT,bd TEXT,server TEXT,port TEXT,active TEXT,company INTEGER, PRIMARY KEY(user,bd,server))",
                        "INSERT INTO conection SELECT * FROM old_conection",
                        "drop table old_conection",
                        "update conection set active = 'S' WHERE rowid=1"
                        "ALTER TABLE conection RENAME TO old_conection",
                        "CREATE TABLE conection (user  TEXT,pass TEXT,bd TEXT,server TEXT,port TEXT,active TEXT,company INTEGER,name_ text, PRIMARY KEY(name_))",
                        "INSERT INTO conection (name_,user,pass,bd,server,port,active,company) SELECT 'OSF_7',user,pass,bd,server,port,active,company FROM old_conection where lower(server) like '%epm-do08%'",
                        "INSERT INTO conection (name_,user,pass,bd,server,port,active,company) SELECT 'OSF_8',user,pass,bd,server,port,active,company FROM old_conection where lower(server) like '%epm-do13%'",
                        "drop table old_conection",*/
                    };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch(Exception ex) { }
                }

                //SqliteDAO.pCreaConfiguracion(res.KEY_EMPRESA, "99");
                //handler.pRegeneraIndexListas();

                SqliteDAO.pCreaConfiguracion(res.KEY_LLAVEW, @"HKEY_CURRENT_USER\Software\Classes\osfweb\shell\open\command");

                //Se actualiza la versión
                SqliteDAO.pActualizaVersion(handler.fsbVersion);                
            }
        }
    }
}
/*"drop table comments",
                        "drop table comment_type",
                        "drop table packages_comments",
                        "drop table packages",
                        "drop table statement",
                        "alter table object_type ADD column path TEXT",
                        "alter table object_type ADD column user_default TEXT",
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
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('EA',0,501,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('GI',0,502,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('GR',0,503,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('MD',0,504,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('OB',0,505,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('OP',0,506,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('PB',0,507,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('PI',0,508,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('RU',0,509,'No','FLEX')",
                        "insert into object_type (object,count_slash,priority,grant,user_default) values('TC',0,510,'No','FLEX')",
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
                        "update object_type set path = 'server\\sql\\00dir\\[nombre]' where object = 'directorio'",
                        "update object_type set path = 'server\\sql\\01seq\\[nombre]' where object = 'secuencia'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\00tbl' where object = 'tabla'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\00tbl' where object = 'alter'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\00tbl' where object = 'drop'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\01pkey' where object = 'llave_primaria'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\02idx' where object = 'indice'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\03fkey' where object = 'llave_foranea'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\04chk' where object = 'llave_unica'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\05trg' where object = 'trigger'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\06data' where object = 'insert'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\06data' where object = 'update'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\06data' where object = 'delete'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\06data' where object = 'merge'",
                        "update object_type set path = 'server\\sql\\02tbls\\[nombre]\\11syn' where object = 'sinonimo'",
                        "update object_type set path = 'server\\sql\\03fnc\\[nombre]' where object = 'Funcion'",
                        "update object_type set path = 'server\\sql\\04proc\\[nombre]' where object = 'procedimiento'",
                        "update object_type set path = 'server\\sql\\05pkg\\[nombre]' where object = 'paquete'",
                        "update object_type set path = 'server\\sql\\06view\\[nombre]' where object = 'vista'",
                        "update object_type set path = 'server\\sql\\07vwmat\\[nombre]' where object = 'vista_mat'",
                        "update object_type set path = 'server\\sql\\09job\\[nombre]' where object = 'job'",
                        "update object_type set path = 'server\\sql\\10grt\\[nombre]' where object = 'grant'",
                        "update object_type set path = 'client\\framework\\EA\\[nombre]' where object = 'EA'",
                        "update object_type set path = 'client\\framework\\GI\\[nombre]' where object = 'GI'",
                        "update object_type set path = 'client\\framework\\GR\\[nombre]' where object = 'GR'",
                        "update object_type set path = 'client\\framework\\MD\\[nombre]' where object = 'MD'",
                        "update object_type set path = 'client\\framework\\OB\\[nombre]' where object = 'OB'",
                        "update object_type set path = 'client\\framework\\OP\\[nombre]' where object = 'OP'",
                        "update object_type set path = 'client\\framework\\PB\\[nombre]' where object = 'PB'",
                        "update object_type set path = 'client\\framework\\PI\\[nombre]' where object = 'PI'",
                        "update object_type set path = 'client\\framework\\RU\\[nombre]' where object = 'RU'",
                        "update object_type set path = 'client\\framework\\TC\\[nombre]' where object = 'TC'",
                        "insert into words values('alter')",
                        "insert into words values('index')",
                        "insert into words values('unique')",
                        "insert into words values('materialized')",
                        "insert into words values('on')",
                        "insert into words values('merge')",
                        "insert into words values('into')",
                        "insert into words values('insert')",
                        "insert into words values('update')",
                        "insert into words values('delete')",
                        "insert into words values('trigger')",
                        "insert into words values('before')",
                        "insert into words values('after')",
                        "insert into words values('for')",
                        "INSERT INTO user_grants (user,company) VALUES ('OPENSIRIUS',99)",
                        "INSERT INTO object_head (head, type, priority, end) VALUES ('grant', 'grant', '10', 'pc');",
                        "update object_type set path = 'server\\sql\\10grt' where object = 'grant'",
                        "insert into words values('from')"
                        "insert into object_type (object,count_slash,priority,grant,path) values('Type_head',0,115,'No','server\\sql\\08typ\\01obj\\[nombre]')",
                        "insert into object_type (object,count_slash,priority,grant,path) values('Type_body',0,115,'No','server\\sql\\08typ\\02col\\[nombre]')",
                        "insert into object_type (object,count_slash,priority,grant,path) values('Migra',0,115,'No','server\\sql\\21mig\\[nombre]')",
                        "insert into object_type (object,count_slash,priority,grant,path) values('Java',0,500,'No','server\\sql\\22java\\[nombre]')"*/

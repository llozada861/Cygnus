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
            string sbVersion;

            /*** ULTIMA VERSION AL FINAL!!!!! ***/
            
            sbVersion = "1.1.9.9";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    "CREATE TABLE apl_hist (codigo\tINTEGER PRIMARY KEY AUTOINCREMENT,caso\tTEXT NOT NULL,\tuser\tTEXT NOT NULL,\tfile\tTEXT NOT NULL,    path\tTEXT NOT NULL, tipo integer)",
                    "CREATE TABLE history (\tcodigo\tINTEGER PRIMARY KEY AUTOINCREMENT,\tstory\tTEXT NOT NULL)",
                    "delete from user_grants where user = 'FLEX'"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.0.3";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    "alter TABLE repositories add rama text",
                    "alter TABLE repositories add sonar text",
                    "update repositories set sonar = 'N'",
                    "update repositories set sonar = 'S' where descripcion = 'BaseDeDatos'",
                    "update repositories set rama = 'master-datos' where descripcion = 'ActualizacionDatos'"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.1.4";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    "update html \r\nset documentation = 'REM Archivo Generado Automáticamente\r\nREM Nombre      = insmensaje_EPM-CUZ-:CODIGO.sql\r\nREM Descripcion = script que registra en la tabla mensaje\r\n\r\nBEGIN\r\n    --realiza la insercción del mensaje EPM - CUZ - <codigo>\r\n    INSERT INTO mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) \r\n    VALUES\r\n    (\r\n        :CODIGO,\r\n        :DESCRIPCION,\r\n        ''EPM'',\r\n        ''CUZ'',\r\n        :CAUSA,\r\n        :SOLUCION\r\n    );\r\n    COMMIT;    \r\nEND;\r\n/'\r\nwhere name = 'PLANTILLA_MENSAJE';"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.2.5";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    "update html \r\nset documentation = 'DECLARE\r\n    CURSOR cuValidaMsj\r\n    IS  \r\n        SELECT *\r\n         FROM mensaje \r\n        WHERE mensdivi = ''EPM''\r\n          AND mensmodu = ''CUZ''\r\n          AND mensdesc = :DESCRIPCION;\r\n          \r\n    rcmensaje  mensaje%ROWTYPE;\r\nBEGIN\r\n    \r\n    rcmensaje := null;\r\n    \r\n    OPEN cuValidaMsj;\r\n    FETCH  cuValidaMsj into rcmensaje;\r\n    CLOSE cuValidaMsj;\r\n    \r\n    IF(rcmensaje.menscodi IS NOT NULL)THEN\r\n        MERGE INTO MENSAJE A USING\r\n         (SELECT\r\n          :CODIGO as MENSCODI,\r\n          :DESCRIPCION as MENSDESC,\r\n          ''EPM'' as MENSDIVI,\r\n          ''CUZ'' as MENSMODU,\r\n          :CAUSA as MENSCAUS,\r\n          :SOLUCION  as MENSPOSO\r\n          FROM DUAL) B\r\n        ON (A.MENSDIVI = B.MENSDIVI and A.MENSMODU = B.MENSMODU and A.MENSCODI = B.MENSCODI)\r\n        WHEN NOT MATCHED THEN \r\n        INSERT (\r\n          MENSCODI, MENSDESC, MENSDIVI, MENSMODU, MENSCAUS, \r\n          MENSPOSO)\r\n        VALUES (\r\n          B.MENSCODI, B.MENSDESC, B.MENSDIVI, B.MENSMODU, B.MENSCAUS, \r\n          B.MENSPOSO)\r\n        WHEN MATCHED THEN\r\n        UPDATE SET \r\n          A.MENSDESC = B.MENSDESC,\r\n          A.MENSCAUS = B.MENSCAUS,\r\n          A.MENSPOSO = B.MENSPOSO;    \r\n    ELSE    \r\n        --realiza la insercción del mensaje EPM - CUZ - <codigo>\r\n        INSERT INTO mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) \r\n        VALUES\r\n        (\r\n            :CODIGO,\r\n            :DESCRIPCION,\r\n            ''EPM'',\r\n            ''CUZ'',\r\n            :CAUSA,\r\n            :SOLUCION\r\n        );\r\n    END IF;\r\n    COMMIT;    \r\nEND;\r\n/'\r\nwhere name = 'PLANTILLA_MENSAJE';"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.2.8";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('35', 'APLICA GRANT', '', '0', '200')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('36', 'FLEX_MANAGER', '', '0', '800')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('35', '36', '[hu]', '', '99')",
                    "INSERT INTO object_type (codigo, object, slash, count_slash, priority) VALUES ('37', 'PLANTILLA', '', '0', '530')",
                    "INSERT INTO object_path (codigo, object_type, path, user_default, company) VALUES ('37', '37', '[usuario]\\server\\sql\\02tbls\\epm_script\\06data\\plantillas\\scripts\\UPD_[nombre]', 'FLEX', '99')"

                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.3.2";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    "update userbd set main = 'N'",
                    "update userbd set main = 'S' where user = 'FLEX'"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.3.3";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    //SFDLLO
                    "update cy_userbd set password_ = 'Oj0-m3+B1oqu34*Pu35' where codigo = 1",
                    //SFUAT
                    "update cy_userbd set password_ = 'Oj0-m3+B1oqu34*Pu35' where codigo = 2",
                    //SFPDN
                    "update cy_userbd set password_ = 'Oj0-m3+B1oqu34*Pu35' where codigo = 3"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.3.6";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    //SFPDN
                    "update cy_userbd set password_ = 'trgs5W8JxUwejKz*+' where codigo = 3"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.3.8";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                 {
                    //SFPDN
                    "update cy_userbd set password_ = 'lB3TXf?+l-i-QGU1' where codigo = 3",
                    "INSERT INTO html (name, documentation, company, filename) VALUES ('SQL_REGLAS', 'SELECT CONFIG_EXPRESSION_ID AS regla_id,\r\n       DESCRIPTION AS descripcion,\r\n       AUTHOR AS autor,\r\n       GENERATION_DATE AS fecha_generacion,\r\n       LAST_MODIFI_DATE AS fecha_modificacion,\r\n       CODE AS CODE\r\nFROM gr_config_expression\r\nWHERE CONFIG_EXPRESSION_ID = :regla_id\r\n', '99', '')",
                    "INSERT INTO html (name, documentation, company, filename) VALUES ('PLANTILLA_GENERA_REGLA', 'DECLARE\r\n        \r\n    PROCEDURE pCreaRegla\r\n    (\r\n        id_regla    in number,\r\n        oclSalida   out clob,\r\n        onuErrorCode    OUT NUMBER,\r\n        osbErrorMessage OUT VARCHAR2 \r\n    )\r\n    is\r\n        sql_ins VARCHAR2(32000):=NULL;\r\n        sbVariable varchar2(15);\r\n        nuConfExpreId gr_config_expression.config_expression_id%type;\r\n        sbObjectName varchar2(100);\r\n        \r\n        CURSOR cuGetConfExpre\r\n        (\r\n            inuConfExpreID IN gr_config_expression.config_expression_id%type\r\n        )\r\n        IS\r\n            SELECT rowid,a.* FROM gr_config_expression a\r\n            WHERE config_expression_id in (inuConfExpreID);\r\n\r\n        rcGetConfExpre  cuGetConfExpre%rowtype;\r\n        \r\n        sql_ins2 VARCHAR2(32000);\r\n        sql_ins3  VARCHAR2(32000):=NULL;\r\n        sbentrega_1 VARCHAR2(8000);\r\n        sbentrega_2 VARCHAR2(8000);\r\n        \r\n        TYPE tyTabla IS TABLE OF VARCHAR2(2000) INDEX BY BINARY_INTEGER;\r\n        tbstring tyTabla;\r\n        \r\n        PROCEDURE ParseString\r\n        (\r\n            ivaCadena               IN  VARCHAR2,\r\n            ivaToken                IN  VARCHAR2,\r\n            otbSalida               OUT tyTabla\r\n        )\r\n        IS\r\n            csbMETHODNAME           CONSTANT VARCHAR2(30) := ''ParseString'';\r\n\r\n            nuIniBusqueda           NUMBER := 1;\r\n            nuFinBusqueda           NUMBER := 1;\r\n            sbArgumento             VARCHAR2( 2000 );\r\n            nuIndArgumentos         NUMBER := 1;\r\n            nuLongitudArg           NUMBER;\r\n        BEGIN\r\n\r\n            -- Recorre la lista de argumentos y los guarda en un tabla pl-sql\r\n            WHILE( ivaCadena IS NOT NULL ) LOOP\r\n                -- Busca el separador en la cadena y almacena su posicion\r\n                nuFinBusqueda := INSTR( ivaCadena, ivaToken, nuIniBusqueda );\r\n\r\n                -- Si no exite el pipe, debe haber un argumento\r\n                IF ( nuFinBusqueda = 0 ) THEN\r\n                    -- Obtiene el argumento\r\n                    sbArgumento := SUBSTR( ivaCadena, nuIniBusqueda );\r\n                    otbSalida( nuIndArgumentos ) := sbArgumento;\r\n\r\n                    -- Termina el ciclo\r\n                    EXIT;\r\n                END IF;\r\n\r\n                -- Obtiene el argumento hasta el separador\r\n                nuLongitudArg := nuFinBusqueda - nuIniBusqueda;\r\n                sbArgumento := SUBSTR( ivaCadena, nuIniBusqueda, nuLongitudArg );\r\n                -- Lo adiciona a la tabla de argumentos, quitando espacios y ENTER a los lados\r\n                otbSalida( nuIndArgumentos ) := TRIM( REPLACE( sbArgumento, CHR( 13 ), '''' ));\r\n                -- Inicializa la posicion inicial con la posicion del caracterer\r\n                -- despues del pipe\r\n                nuIniBusqueda := nuFinBusqueda + 1;\r\n                -- Incrementa el indice de la tabla de argumentos\r\n                nuIndArgumentos := nuIndArgumentos + 1;\r\n            END LOOP;\r\n        EXCEPTION\r\n            WHEN OTHERS THEN\r\n                dbms_output.put_line(''ERROR OTHERS ''||SQLERRM);\r\n        END ParseString;\r\n    BEGIN\r\n    \r\n        onuErrorCode := 0;\r\n        osbErrorMessage := null;\r\n\r\n        nuConfExpreId := id_regla;\r\n\r\n        sbVariable := ''IdConfExpre'';\r\n        \r\n        dbms_lob.createtemporary(lob_loc => oclSalida, cache => true, dur => dbms_lob.session);\r\n        \r\n        OPEN cuGetConfExpre(nuConfExpreId);\r\n        FETCH cuGetConfExpre INTO rcGetConfExpre;\r\n        CLOSE cuGetConfExpre;\r\n\r\n        sbObjectName := replace(rcGetConfExpre.OBJECT_NAME, nuConfExpreId, ''''''||''||sbVariable||''||'''''');\r\n        \r\n        sql_ins := sbVariable||'',''||\r\n        rcGetConfExpre.CONFIGURA_TYPE_ID||'','''''';\r\n\r\n        sql_ins2:=rcGetConfExpre.EXPRESSION||'''''','''''';\r\n\r\n        sql_ins3:=\r\n        rcGetConfExpre.AUTHOR||'''''', to_date (''''''||\r\n        rcGetConfExpre.CREATION_DATE||'''''',''''DD/MM/YYYY HH24:MI:SS''''),to_date (''''''||\r\n        rcGetConfExpre.GENERATION_DATE||'''''',''''DD/MM/YYYY HH24:MI:SS''''),to_date (''''''||\r\n        rcGetConfExpre.LAST_MODIFI_DATE||'''''',''''DD/MM/YYYY HH24:MI:SS''''),''''''||\r\n        rcGetConfExpre.STATUS||'''''',''''''||\r\n        rcGetConfExpre.USED_OTHER_EXPRESION||'''''',''''''||\r\n        rcGetConfExpre.MODIFICATION_TYPE||'''''',''''''||\r\n        rcGetConfExpre.PASSWORD||'''''',''''''||\r\n        rcGetConfExpre.EXECUTION_TYPE||'''''',''''''||\r\n        rcGetConfExpre.DESCRIPTION||'''''',''''''||\r\n        sbObjectName||'''''',''''''||\r\n        rcGetConfExpre.OBJECT_TYPE||'''''''';\r\n\r\n        ParseString(sql_ins2,'','',tbstring);\r\n\r\n        \r\n        sbentrega_1 := ''/******************************************************************\r\nPropiedad Intelectual de Empresas Publicas de Medellín\r\nArchivo     ins_<DDMMYYYY>_gr_config_expression.sql\r\nAutor       <Nombre autor>\r\nFecha       <AAAAMMDD>\r\n\r\nDescripción\r\nObservaciones\r\n\r\nHistoria de Modificaciones\r\nFecha         Autor               Modificación\r\n<AAAAMMDD>  <Nombre Autor>              Creación\r\n******************************************************************/\r\ndeclare\r\n    tbConfigs   dagr_config_expression.tytbConfig_Expression_Id;\r\n    onuExprId gr_config_expression.config_expression_id%type := NULL;\r\n    nuCountErr number := 0;\r\n    nuProc number := 0;\r\n    nuErrorCode          NUMBER(15);\r\n    sbErrorMsg           VARCHAR2(2000);\r\n\r\n    IdConfExpre GR_CONFIG_EXPRESSION.config_expression_id%type;\r\nBEGIN\r\n\r\n    dbms_output.put_line(''''Inicia Proceso ''''||sysdate);\r\n    IdConfExpre := SEQ_GR_CONFIG_EXPRESSION.NEXTVAL;\r\n\r\n    dbms_output.put_line(''''Insertando Regla: ''''||IdConfExpre);\r\n\r\n    INSERT INTO GR_CONFIG_EXPRESSION\r\n    (config_expression_id,configura_type_id,expression,author,creation_date,generation_date,last_modifi_date,status,\r\n    used_other_expresion,modification_type,password,execution_type,description,object_name,object_type)\r\n    VALUES\r\n    ('';\r\n\r\n        sbentrega_2 := '');\r\n\r\n    dbms_output.put_line(''''Regenerando Regla: ''''||IdConfExpre);\r\n\r\n    BEGIN\r\n        GR_BOINTERFACE_BODY.CreateStprByConfExpreId(IdConfExpre);\r\n        dbms_output.put_line(''''Expresion Generada = ''''||IdConfExpre);\r\n\r\n        EXCEPTION\r\n            when ex.CONTROLLED_ERROR then\r\n                 Errors.getError(nuErrorCode,sbErrorMsg);\r\n                 dbms_output.put_line(substr(''''ExprId = ''''||IdConfExpre||'''', Err : ''''||nuErrorCode||'''', ''''||sbErrorMsg,1,250));\r\n\r\n            when others then\r\n                 Errors.setError;\r\n                        Errors.getError(nuErrorCode,sbErrorMsg);\r\n                 dbms_output.put_line(substr(''''ExprId = ''''||IdConfExpre||'''', Err : ''''||nuErrorCode||'''', ''''||sbErrorMsg,1,250));\r\n\r\n    END;\r\n        \r\n    dbms_output.put_line(''''Termina regenerar Regla: ''''||IdConfExpre);\r\n\r\n    --<INSERT_TABLA> o <UPDATE_TABLA>\r\n\r\n    commit;\r\n\r\n    dbms_output.put_line(''''Fin Proceso ''''||sysdate);\r\nEXCEPTION\r\nwhen ex.CONTROLLED_ERROR  then\r\n    rollback;\r\n    Errors.getError(nuErrorCode, sbErrorMsg);\r\n    dbms_output.put_line(''''ERROR CONTROLLED '''');\r\n    dbms_output.put_line(''''error onuErrorCode: ''''||nuErrorCode);\r\n    dbms_output.put_line(''''error osbErrorMess: ''''||sbErrorMsg);\r\n\r\nwhen OTHERS then\r\n    rollback;\r\n    Errors.setError;\r\n    Errors.getError(nuErrorCode, sbErrorMsg);\r\n    dbms_output.put_line(''''ERROR OTHERS '''');\r\n    dbms_output.put_line(''''error onuErrorCode: ''''||nuErrorCode);\r\n    dbms_output.put_line(''''error osbErrorMess: ''''||sbErrorMsg);\r\nEND;'';\r\n        \r\n        DBMS_LOB.APPEND(oclSalida,sbentrega_1);\r\n        DBMS_LOB.APPEND(oclSalida,sql_ins);\r\n\r\n        FOR i IN 1..tbstring.count\r\n        LOOP\r\n              if i<> tbstring.count then\r\n                if (i= tbstring.first) then\r\n                  DBMS_LOB.APPEND(oclSalida,tbstring(i)||'','');\r\n                else\r\n                  if i<> (tbstring.count-1) then\r\n                    DBMS_LOB.APPEND(oclSalida,tbstring(i)||'',''||''''''||''||chr(10)||'''''''');\r\n\r\n                  else\r\n                    DBMS_LOB.APPEND(oclSalida,tbstring(i)||'','');\r\n                  END if;\r\n                END if;\r\n              else\r\n                DBMS_LOB.APPEND(oclSalida,tbstring(i));\r\n              END if;\r\n        END LOOP;\r\n\r\n        DBMS_LOB.APPEND(oclSalida,sql_ins3);\r\n        DBMS_LOB.APPEND(oclSalida,sbentrega_2);\r\n        DBMS_LOB.APPEND(oclSalida,chr(10)||''/'');\r\n\r\n    EXCEPTION\r\n        /*when ex.CONTROLLED_ERROR  then\r\n            Errors.getError(onuErrorCode, osbErrorMessage);\r\n            dbms_output.put_line(''ERROR CONTROLLED '');\r\n            dbms_output.put_line(''error onuErrorCode: ''||onuErrorCode);\r\n            dbms_output.put_line(''error osbErrorMess: ''||osbErrorMessage);*/\r\n\r\n        when OTHERS then\r\n            --Errors.setError;\r\n            --Errors.getError(onuErrorCode, osbErrorMessage);\r\n            dbms_output.put_line(''ERROR OTHERS '');\r\n            dbms_output.put_line(''error onuErrorCode: ''||onuErrorCode);\r\n            dbms_output.put_line(''error osbErrorMess: ''||osbErrorMessage);\r\n    END;\r\nbegin\r\n    pCreaRegla\r\n    (\r\n        :id_regla,\r\n        :oclFile,\r\n        :onuErrorCode,\r\n        :osbErrorMessage\r\n     );\r\nend;\r\n', '99', '')"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.3.9";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                {
                    "INSERT INTO user_grants (user,company) VALUES ('ROL_CONSULTA_DISECO',99)"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.4.0";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                {
                    "UPDATE user_grants set company = 99 where user = 'ROL_CONSULTA_DISECO'",
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.4.3";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                {
                    "INSERT INTO html (name, documentation, company, filename) VALUES ('PLANTILLA_REGENERA_REGLA', 'DECLARE       \r\n    PROCEDURE pReplicaRegla\r\n    (\r\n        id_regla    in number,\r\n        oclSalida   out clob,\r\n        onuErrorCode    OUT NUMBER,\r\n        osbErrorMessage OUT VARCHAR2 \r\n    )\r\n    is\r\n        sql_ins VARCHAR2(32000):=NULL;\r\n        sbVariable varchar2(15);\r\n        nuConfExpreId gr_config_expression.config_expression_id%type;\r\n        sbObjectName varchar2(100);\r\n        \r\n        csbCARACTER_SEPA    VARCHAR2(1) := '';'';\r\n        \r\n        CURSOR cuGetConfExpre\r\n        (\r\n            inuConfExpreID IN gr_config_expression.config_expression_id%type\r\n        )\r\n        IS\r\n            SELECT rowid,a.* FROM gr_config_expression a\r\n            WHERE config_expression_id in (inuConfExpreID);\r\n\r\n        rcGetConfExpre  cuGetConfExpre%rowtype;\r\n        \r\n        sql_ins2 VARCHAR2(32000);\r\n        sql_ins3  VARCHAR2(32000):=NULL;\r\n        sbentrega_1 VARCHAR2(8000);\r\n        sbentrega_2 VARCHAR2(8000);\r\n        \r\n        TYPE tyTabla IS TABLE OF VARCHAR2(2000) INDEX BY BINARY_INTEGER;\r\n        tbstring tyTabla;\r\n        \r\n        PROCEDURE ParseString\r\n        (\r\n            ivaCadena               IN  VARCHAR2,\r\n            ivaToken                IN  VARCHAR2,\r\n            otbSalida               OUT tyTabla\r\n        )\r\n        IS\r\n            csbMETHODNAME           CONSTANT VARCHAR2(30) := ''ParseString'';\r\n\r\n            nuIniBusqueda           NUMBER := 1;\r\n            nuFinBusqueda           NUMBER := 1;\r\n            sbArgumento             VARCHAR2( 2000 );\r\n            nuIndArgumentos         NUMBER := 1;\r\n            nuLongitudArg           NUMBER;\r\n        BEGIN\r\n\r\n            -- Recorre la lista de argumentos y los guarda en un tabla pl-sql\r\n            WHILE( ivaCadena IS NOT NULL ) LOOP\r\n                -- Busca el separador en la cadena y almacena su posicion\r\n                nuFinBusqueda := INSTR( ivaCadena, ivaToken, nuIniBusqueda );\r\n\r\n                -- Si no exite el pipe, debe haber un argumento\r\n                IF ( nuFinBusqueda = 0 ) THEN\r\n                    -- Obtiene el argumento\r\n                    sbArgumento := SUBSTR( ivaCadena, nuIniBusqueda );\r\n                    otbSalida( nuIndArgumentos ) := sbArgumento;\r\n\r\n                    -- Termina el ciclo\r\n                    EXIT;\r\n                END IF;\r\n\r\n                -- Obtiene el argumento hasta el separador\r\n                nuLongitudArg := nuFinBusqueda - nuIniBusqueda;\r\n                sbArgumento := SUBSTR( ivaCadena, nuIniBusqueda, nuLongitudArg );\r\n                -- Lo adiciona a la tabla de argumentos, quitando espacios y ENTER a los lados\r\n                otbSalida( nuIndArgumentos ) := TRIM( REPLACE( sbArgumento, CHR( 13 ), '''' ));\r\n                -- Inicializa la posicion inicial con la posicion del caracterer\r\n                -- despues del pipe\r\n                nuIniBusqueda := nuFinBusqueda + 1;\r\n                -- Incrementa el indice de la tabla de argumentos\r\n                nuIndArgumentos := nuIndArgumentos + 1;\r\n            END LOOP;\r\n        EXCEPTION\r\n            WHEN OTHERS THEN\r\n                dbms_output.put_line(''ERROR OTHERS ''||SQLERRM);\r\n        END ParseString;\r\n    BEGIN\r\n    \r\n        onuErrorCode := 0;\r\n        osbErrorMessage := null;\r\n\r\n        nuConfExpreId := id_regla;\r\n\r\n        sbVariable := ''IdConfExpre'';\r\n        \r\n        dbms_lob.createtemporary(lob_loc => oclSalida, cache => true, dur => dbms_lob.session);\r\n        \r\n        OPEN cuGetConfExpre(nuConfExpreId);\r\n        FETCH cuGetConfExpre INTO rcGetConfExpre;\r\n        CLOSE cuGetConfExpre;\r\n        \r\n        sql_ins := ''EXPRESSION = '''''';\r\n\r\n        sql_ins2:=rcGetConfExpre.EXPRESSION||'''''',''||chr(10)||''           LAST_MODIFI_DATE = '';\r\n\r\n        sql_ins3:=''to_date (''''''||\r\n        rcGetConfExpre.LAST_MODIFI_DATE||'''''',''''DD/MM/YYYY HH24:MI:SS''''),''||chr(10)||''           MODIFICATION_TYPE =''''''||\r\n        rcGetConfExpre.MODIFICATION_TYPE||'''''', ''||chr(10)||''           EXECUTION_TYPE = ''''''||\r\n        rcGetConfExpre.EXECUTION_TYPE||'''''', ''||chr(10)||''           DESCRIPTION = ''''''||\r\n        rcGetConfExpre.DESCRIPTION||'''''',''||chr(10)||''           OBJECT_TYPE = ''''''||rcGetConfExpre.OBJECT_TYPE||'''''',''||chr(10)||\r\n        ''           expression_notes = null,''||chr(10)||''           code = null'';\r\n\r\n        ParseString(sql_ins2,csbCARACTER_SEPA,tbstring);\r\n\r\n        \r\n        sbentrega_1 := ''/******************************************************************\r\nPropiedad Intelectual de Empresas Publicas de Medellín\r\nArchivo     up_<DDMMYYYY>_gr_config_expression.sql\r\nAutor       <Nombre autor>\r\nFecha       <AAAAMMDD>\r\n\r\nDescripción\r\nObservaciones\r\n\r\nHistoria de Modificaciones\r\nFecha         Autor               Modificación\r\n<AAAAMMDD>  <Nombre Autor>              Creación\r\n******************************************************************/\r\ndeclare\r\n    tbConfigs   dagr_config_expression.tytbConfig_Expression_Id;\r\n    onuExprId gr_config_expression.config_expression_id%type := NULL;\r\n    nuCountErr number := 0;\r\n    nuProc number := 0;\r\n    nuErrorCode          NUMBER(15);\r\n    sbErrorMsg           VARCHAR2(2000);\r\n\r\n    IdConfExpre GR_CONFIG_EXPRESSION.config_expression_id%type;\r\nBEGIN\r\n\r\n    dbms_output.put_line(''''Inicia Proceso ''''||sysdate);\r\n    IdConfExpre := ''||nuConfExpreId||'';\r\n\r\n    dbms_output.put_line(''''Actualizando la Regla: ''''||IdConfExpre);\r\n\r\n    UPDATE GR_CONFIG_EXPRESSION\r\n       SET '';\r\n\r\n        sbentrega_2 := ''\r\n    where config_expression_id = IdConfExpre;\r\n\r\n    dbms_output.put_line(''''Regenerando Regla: ''''||IdConfExpre);\r\n\r\n    BEGIN\r\n        GR_BOINTERFACE_BODY.CreateStprByConfExpreId(IdConfExpre);\r\n        dbms_output.put_line(''''Expresion Generada = ''''||IdConfExpre);\r\n\r\n        EXCEPTION\r\n            when ex.CONTROLLED_ERROR then\r\n                 Errors.getError(nuErrorCode,sbErrorMsg);\r\n                 dbms_output.put_line(substr(''''ExprId = ''''||IdConfExpre||'''', Err : ''''||nuErrorCode||'''', ''''||sbErrorMsg,1,250));\r\n\r\n            when others then\r\n                 Errors.setError;\r\n                        Errors.getError(nuErrorCode,sbErrorMsg);\r\n                 dbms_output.put_line(substr(''''ExprId = ''''||IdConfExpre||'''', Err : ''''||nuErrorCode||'''', ''''||sbErrorMsg,1,250));\r\n\r\n    END;\r\n        \r\n    dbms_output.put_line(''''Termina regenerar Regla: ''''||IdConfExpre);\r\n\r\n    --<INSERT_TABLA> o <UPDATE_TABLA>\r\n\r\n    commit;\r\n\r\n    dbms_output.put_line(''''Fin Proceso ''''||sysdate);\r\nEXCEPTION\r\nwhen ex.CONTROLLED_ERROR  then\r\n    rollback;\r\n    Errors.getError(nuErrorCode, sbErrorMsg);\r\n    dbms_output.put_line(''''ERROR CONTROLLED '''');\r\n    dbms_output.put_line(''''error onuErrorCode: ''''||nuErrorCode);\r\n    dbms_output.put_line(''''error osbErrorMess: ''''||sbErrorMsg);\r\n\r\nwhen OTHERS then\r\n    rollback;\r\n    Errors.setError;\r\n    Errors.getError(nuErrorCode, sbErrorMsg);\r\n    dbms_output.put_line(''''ERROR OTHERS '''');\r\n    dbms_output.put_line(''''error onuErrorCode: ''''||nuErrorCode);\r\n    dbms_output.put_line(''''error osbErrorMess: ''''||sbErrorMsg);\r\nEND;'';\r\n        \r\n        DBMS_LOB.APPEND(oclSalida,sbentrega_1);\r\n        DBMS_LOB.APPEND(oclSalida,sql_ins);\r\n\r\n        FOR i IN 1..tbstring.count\r\n        LOOP\r\n              if i<> tbstring.count then\r\n                if (i= tbstring.first) then\r\n                  DBMS_LOB.APPEND(oclSalida,tbstring(i)||csbCARACTER_SEPA);\r\n                else\r\n                  if i<> (tbstring.count-1) then\r\n                    DBMS_LOB.APPEND(oclSalida,tbstring(i)||csbCARACTER_SEPA||''''''||''||chr(10)||''                        '''''');\r\n\r\n                  else\r\n                    DBMS_LOB.APPEND(oclSalida,tbstring(i)||csbCARACTER_SEPA);\r\n                  END if;\r\n                END if;\r\n              else\r\n                DBMS_LOB.APPEND(oclSalida,tbstring(i));\r\n              END if;\r\n        END LOOP;\r\n\r\n        DBMS_LOB.APPEND(oclSalida,sql_ins3);\r\n        DBMS_LOB.APPEND(oclSalida,sbentrega_2);\r\n        DBMS_LOB.APPEND(oclSalida,chr(10)||''/'');\r\n\r\n    EXCEPTION\r\n        when OTHERS then\r\n            dbms_output.put_line(''ERROR OTHERS '');\r\n            dbms_output.put_line(''error onuErrorCode: ''||onuErrorCode);\r\n            dbms_output.put_line(''error osbErrorMess: ''||osbErrorMessage);\r\n    END;\r\nbegin\r\n    pReplicaRegla\r\n    (\r\n        :id_regla,\r\n        :oclFile,\r\n        :onuErrorCode,\r\n        :osbErrorMessage\r\n     );\r\nend;\r\n', '99', '')",
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            sbVersion = "1.2.4.5";

            if (!SqliteDAO.pblValidaVersion(sbVersion))
            {
                string[] query =
                {
                    //SFPDN
                    "update cy_userbd set password_ = 'A1vaR4C0+a-L4*C4rc3L' where codigo = 3"
                };

                foreach (string sql in query)
                {
                    try
                    {
                        SqliteDAO.pExecuteNonQuery(sql);
                    }
                    catch (Exception ex) { }
                }

                SqliteDAO.pActualizaVersion(sbVersion);
            }

            //ultima versión
            /*if (!SqliteDAO.pblValidaVersion(fieVersionInfo.FileVersion))
            {
                //SqliteDAO.pCreaConfiguracion(res.KEY_EMPRESA, "99");
                //handler.pRegeneraIndexListas();

                //SqliteDAO.pCreaConfiguracion(res.KEY_LLAVEW, @"HKEY_CURRENT_USER\Software\Classes\osfweb\shell\open\command");

                //Se actualiza la versión
                SqliteDAO.pActualizaVersion(fieVersionInfo.FileVersion);                
            }*/
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

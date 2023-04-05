using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Compila;
using Cygnus2_0.ViewModel.Data;
using Cygnus2_0.ViewModel.Objects;
using Cygnus2_0.ViewModel.Refresh;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using res = Cygnus2_0.Properties.Resources;
using Cygnus2_0.General.Times;
using Cygnus2_0.ViewModel.Time;
using System.Collections.ObjectModel;
using Cygnus2_0.Model.Audit;
using Cygnus2_0.Model.Compila;
using Cygnus2_0.Model.Data;
using Cygnus2_0.Model.Security;
using Cygnus2_0.Model.Time;
using Microsoft.Office.Interop.Excel;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Cygnus2_0.Model.User;
using System.Xml.Linq;
using Microsoft.Office.Interop.Outlook;
using System.Drawing;

namespace Cygnus2_0.DAO
{
    public class MarkDAO
    {
        #region VariablesPrivadas
        private Handler handler;
        #endregion VariablesPrivadas

        #region Constructor
        public MarkDAO(Handler miHandler)
        {
            this.handler = miHandler;
        }
        #endregion Constructor

        #region GestionDatos
        public void pCreaParametro(ParameterModel parametro)
        {
            string query = "INSERT INTO FLEX.epm_parametr (PARAMETER_ID, DESCRIPTION, VALUE, VAL_FUNCTION, MODULE_ID, DATA_TYPE, ALLOW_UPDATE)" +
                             "VALUES" +
                             "(" +
                                 ":isbParamId," +
                                 ":isbDescrip," +
                                 ":isbValor," +
                                 ":isbFuncion," +
                                 "-99," +
                                 ":isbTipo," +
                                 "'Y'" +
                             ")";

            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":isbParamId", parametro.ParameterId);
                cmd.Parameters.Add(":isbDescrip", parametro.Descripcion);
                cmd.Parameters.Add(":isbValor", parametro.Valor);
                cmd.Parameters.Add(":isbFuncion", parametro.Funcion);
                cmd.Parameters.Add(":isbTipo", parametro.Tipo.Text);
                cmd.ExecuteNonQuery();
            }
        }

        public void pCreaMensaje(MessageModel mensajesModel)
        {
            string query = "INSERT INTO flex.mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) " +
                            " VALUES" +
                            "(" +
                                ":isbCodigo," +
                                ":isbDescrip," +
                                "'EPM'," +
                                "'CUZ'," +
                                ":isbCausa," +
                                ":isbSolucion" +
                            ")";

            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":isbCodigo", mensajesModel.Codigo);
                cmd.Parameters.Add(":isbDescrip", mensajesModel.Descripcion);
                cmd.Parameters.Add(":isbCausa", mensajesModel.Causa);
                cmd.Parameters.Add(":isbSolucion", mensajesModel.Solucion);
                cmd.ExecuteNonQuery();
            }
        }

        public string pObtCodigoMensaje()
        {
            string sql = "SELECT max(menscodi) nuCodigo" +
                        " FROM flex.mensaje " +
                        " WHERE mensdivi = 'EPM'" +
                        " AND mensmodu = 'CUZ'" +
                        " AND menscodi < 900196";

            int Codigo = 0;

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader()) 
                {
                    while (rdr.Read())
                    {
                        Codigo = Convert.ToInt32(rdr["nuCodigo"]);
                    }
                    rdr.Close();
                }
            }

            Codigo = Codigo + 1;

            return Codigo+"";
        }
        #endregion GetionDatos

        #region Refrescamiento
        #endregion Refrescamiento

        #region Updater
        internal string pObtCodigoVersion()
        {
            string sbCodigoVersion = "";

            string sql = "SELECT version" +
                        " FROM flex.ll_version" +
                        " ORDER BY fecha_ini desc";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        sbCodigoVersion = rdr["version"].ToString();
                        break;
                    }
                    rdr.Close();
                }
            }

            return sbCodigoVersion;
        }

        internal void pCargaVersion(byte[] bytes, string archivo, string version)
        {
            string query = "insert into ll_version values (:version,sysdate,sysdate,:data)";

            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":version", version);
                cmd.Parameters.Add(":data", bytes);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion Updater

        #region GestionObjetos
        #endregion GestionObjetos

        #region GestionUsuarios
        #endregion GetionUsuarios

        #region CompilacionObjetos
        private string pDevuelveUsuariosIn()
        {
            string usuariosIn = "";

            foreach (UsuarioModel usuario in handler.ListaUsuarios)
            {
                usuariosIn = usuariosIn + "'" + usuario.Usuariobd + "',";
            }

            usuariosIn = usuariosIn.Remove(usuariosIn.Length - 1);

            return usuariosIn;
        }
        public void pObtErrores(Archivo archivo, CompilaModel model)
        {
            string sql;

            sql = "SELECT " +
                               "NAME ," +
                               "TYPE ," +
                               "LINE ," +
                               "TEXT " +
                        " FROM   all_errors " +
                        " WHERE  name = upper('" + archivo.NombreObjeto +"')"+
                        " AND    OWNER IN(" + pDevuelveUsuariosIn() + ") ";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["NAME"]);
                        dato.Value = Convert.ToString(reader["TEXT"]);
                        dato.Observacion = Convert.ToString(reader["TYPE"]);
                        dato.Prioridad = Convert.ToInt32(reader["LINE"].ToString());
                        model.ListaObservaciones.Add(dato);
                    }
                    reader.Close();
                }
            }
        }

        public void pEjecutarScriptBD(string codigo)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetScriptCommand(codigo))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }
            }
        }

        public void pValidaUsuarioCompila(Archivo archivo, Handler handler)
        {
            string sql;
            Int32 nuCantidad = 0;

            sql = "SELECT COUNT(1) cantidad" +
                         "   FROM " +
                         "   ( " +
                         "       SELECT DISTINCT OWNER " +
                         "       FROM   all_objects " +
                         "       WHERE  object_name = upper('" + archivo.NombreObjeto +"') " +
                         "       AND    OWNER IN("+ pDevuelveUsuariosIn() + ") " +
                         "   )"; 

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        nuCantidad = Int32.Parse(rdr["cantidad"].ToString());
                        break;
                    }
                    rdr.Close();
                }
            }

            if(nuCantidad > 1)
            {
                throw new System.Exception("El objeto ["+ archivo.NombreObjeto+"] solo puede existir en un esquema!");
            }
        }

        public void pValidaObjEsquema(Archivo archivo, string usuario)
        {
            string sql;
            string ownerBd = null;

            sql =   "       SELECT DISTINCT OWNER " +
                    "       FROM   all_objects " +
                    "       WHERE  object_name = upper('" + archivo.NombreObjeto + "') " +
                    "       AND    OWNER IN(" + pDevuelveUsuariosIn() + ") ";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ownerBd = rdr["OWNER"].ToString();
                        break;
                    }
                    rdr.Close();
                }
            }

            if (!string.IsNullOrEmpty(ownerBd) && !ownerBd.Equals(usuario))
            {
                throw new System.Exception("El objeto [" + archivo.NombreObjeto + "] ya existe en el esquema ["+ ownerBd+"]");
            }
        }
        public string pObtCantObjsInvalidos()
        {
            string stCantidadObjetosInvalidos = null;
            string sql;

            sql = "select count(distinct name)  cantidad "+
                    "from all_errors "+
                    "where owner in ("+ pDevuelveUsuariosIn()+") "+
                    "and attribute = 'ERROR'";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Connection = con;

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            stCantidadObjetosInvalidos = rdr["cantidad"].ToString();
                            break;
                        }
                        rdr.Close();
                    }
                }
            }

            return stCantidadObjetosInvalidos;
        }

        public void pObtConsultaObjetos(string nombreObjeto, BlockViewModel view)
        {
            string sql;

            sql = "SELECT DISTINCT a.owner, " +
                    "a.object_name, " +
                    "a.OBJECT_TYPE, " +
                    "a.STATUS " +
                    "FROM all_objects a " +
                    "WHERE a.object_name LIKE "+ nombreObjeto+" "+
                    "AND a.OWNER IN ( " + pDevuelveUsuariosIn()+") ";


            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Archivo dato = new Archivo();

                        dato.FileName = Convert.ToString(reader["object_name"]);
                        dato.Usuario = Convert.ToString(reader["owner"]);
                        dato.Observacion = Convert.ToString(reader["OBJECT_TYPE"]);
                        dato.OrdenCambio = Convert.ToString(reader["STATUS"]);
                        dato.Owner = Convert.ToString(reader["owner"]);

                        //Si no existe que adicione el registro
                        if (!view.ListaArchivosEncontrados.ToList().Exists(x => (x.FileName.Equals(dato.FileName) && x.Owner.Equals(dato.Owner))))
                            view.ListaArchivosEncontrados.Add(dato);
                    }
                    reader.Close();
                }
            }
        }
        public string pExecuteSqlplus(string credentials, List<Archivo> archivos,string ruta,string usuario)
        {
            string scriptDir = ruta;
            string sbNombreAplica;
            string sbAplica = "";
            string rutaLog;
            string nombreLog;
            StringBuilder sbAplicaBody = new StringBuilder();
            Process process = new Process();
            string output = null;
            List<Archivo> archivosOrdenados;
            string rutaArchivo;

            if (archivos.Count > 0)
            {
                sbNombreAplica = "apl_temp_" + usuario + ".apl";

                sbAplica = Path.Combine(handler.PathTempAplica, sbNombreAplica);

                if (File.Exists(sbAplica))
                {
                    File.Delete(sbAplica);
                }

                nombreLog = "Log_" + usuario + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                rutaLog = Path.Combine(ruta, nombreLog);

                if (File.Exists(rutaLog))
                {
                    File.Delete(rutaLog);
                }

                sbAplicaBody.Append(res.EncabezadoAplSqlplus);
                sbAplicaBody.Replace("<nombre_spool>", "'" + rutaLog + "'");
                sbAplicaBody.Append(Environment.NewLine);
                sbAplicaBody.Append(Environment.NewLine);

                archivosOrdenados = archivos.OrderBy(x => x.Tipo).ToList();

                foreach (Archivo archivo in archivosOrdenados)
                {
                    if (archivo.Tipo != Int32.Parse(res.TipoAplica) && archivo.Tipo != Int32.Parse(res.TipoAplicaGrant))
                    {
                        sbAplicaBody.Append("@" + "'" + archivo.RutaConArchivo + "'");
                        sbAplicaBody.Append(Environment.NewLine);
                        sbAplicaBody.Append(Environment.NewLine);
                        archivo.AplicaTemporal = nombreLog;
                    }
                    else
                    {
                        if(string.IsNullOrEmpty(archivo.RutaConArchivo))
                            rutaArchivo = archivo.Ruta+""+archivo.FileName;
                        else
                            rutaArchivo = archivo.RutaConArchivo;

                        sbAplicaBody.Append("@" + "'" + rutaArchivo + "'");
                        sbAplicaBody.Append(Environment.NewLine);
                    }
                }

                sbAplicaBody.Append(res.FinAplSqlplus);

                using (StreamWriter str = new StreamWriter(sbAplica))
                {
                    str.Write(sbAplicaBody.ToString());
                }

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = scriptDir;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardInput = false;
                process.StartInfo.FileName = Path.Combine(handler.ConfGeneralView.Model.RutaSqlplus, "sqlplus.exe"); 
                process.StartInfo.Arguments = string.Format("{0} @\"{1}\" ", credentials, sbAplica);
                process.StartInfo.CreateNoWindow = false;

                process.Start();
                //string output = null; //process.StandardOutput.ReadToEnd();
                //process.StandardInput.WriteLine("exit;");
                process.Close();
            }

            return output;
        }
        #endregion CompilacionObjetos
                        
        #region GenereacionPaquetes
        internal OracleClob pGeneraPktbl(string tabla, SelectListItem usuarioBD, string caso,Handler handler)
        {
            OracleConnection conn = handler.ConexionOracle.ConexionOracleSQL;

            OracleCommand sqlPktbl = new OracleCommand(handler.ListaHTML.Where(x=>x.Nombre.Equals(res.KEY_PKTBL)).FirstOrDefault().Documentacion.Replace("\r\n", "\n"), conn);

            sqlPktbl.BindByName = true;

            OracleParameter sbTabla = new OracleParameter("isbTabla", OracleDbType.Varchar2);
            sbTabla.Value = tabla.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbTabla);

            OracleParameter sbOwner = new OracleParameter("isbOwner", OracleDbType.Varchar2);
            sbOwner.Value = usuarioBD.Text;
            sqlPktbl.Parameters.Add(sbOwner);

            OracleParameter sbOrder = new OracleParameter("isOrder", OracleDbType.Varchar2);
            sbOrder.Value = caso.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbOrder);

            OracleParameter clFile = new OracleParameter("oclFile", OracleDbType.Clob);
            clFile.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(clFile);

            OracleParameter nuErrorCode = new OracleParameter("onuErrorCode", OracleDbType.Int64);
            nuErrorCode.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(nuErrorCode);

            OracleParameter nuErrorMessage = new OracleParameter("osbErrorMessage", OracleDbType.Int64);
            nuErrorMessage.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(nuErrorMessage);

            sqlPktbl.ExecuteReader();

            return (OracleClob)sqlPktbl.Parameters["oclFile"].Value;
        }
        #endregion GenereacionPaquetes

        #region Reporte
        #endregion Reporte

        #region Auditoria
        internal void pGeneraAuditoria(TbAuditoriaModel model, out OracleClob tabla, out OracleClob trigger)
        {
            OracleConnection conn = handler.ConexionOracle.ConexionOracleSQL;

            OracleCommand sqlPktbl = new OracleCommand(handler.ListaHTML.Where(x => x.Nombre.Equals(res.KEY_AUDIT_TABLA)).FirstOrDefault().Documentacion.Replace("\r\n", "\n"), conn);

            sqlPktbl.BindByName = true;

            OracleParameter sbTabla = new OracleParameter("isbTableName", OracleDbType.Varchar2);
            sbTabla.Value = model.Tabla.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbTabla);

            OracleParameter sbOwner = new OracleParameter("isbAutor", OracleDbType.Varchar2);
            sbOwner.Value = model.Autor.Trim();
            sqlPktbl.Parameters.Add(sbOwner);

            OracleParameter sbLogin = new OracleParameter("isbLogin", OracleDbType.Varchar2);
            sbLogin.Value = model.Login.Trim();
            sqlPktbl.Parameters.Add(sbLogin);

            OracleParameter sbTicket = new OracleParameter("isbTicket", OracleDbType.Varchar2);
            sbTicket.Value = model.Ticket.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbTicket);

            OracleParameter sbPk = new OracleParameter("isbPK", OracleDbType.Varchar2);
            sbPk.Value = model.Primaria.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbPk);

            OracleParameter oclScript = new OracleParameter("osbScript", OracleDbType.Clob);
            oclScript.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(oclScript);

            OracleParameter oclTrg = new OracleParameter("osbTrgScript", OracleDbType.Clob);
            oclTrg.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(oclTrg);

            sqlPktbl.ExecuteReader();

            tabla = (OracleClob)sqlPktbl.Parameters["osbScript"].Value;
            trigger = (OracleClob)sqlPktbl.Parameters["osbTrgScript"].Value;
        }
        #endregion Auditoria

        #region Estandar
        internal ObservableCollection<SelectListItem> pObtListaEstandar()
        {
            ObservableCollection<SelectListItem> listaEstandar = new ObservableCollection<SelectListItem>();

            string sql = "SELECT token,descripcion,valor FROM flex.ll_estandar";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader()) // execute the oracle sql and start reading it
                {
                    while (rdr.Read()) // loop through each row from oracle
                    {
                        listaEstandar.Add(new SelectListItem
                        {
                            Value = rdr["token"].ToString(),
                            Text = rdr["descripcion"].ToString(),
                            Observacion = rdr["valor"].ToString()
                        });
                    }
                    rdr.Close(); // close the oracle reader
                }
            }

            return listaEstandar;
        }
        internal void pAdicionarEstandar(string value1, string text, string value2)
        {
            string query = "insert into flex.ll_estandar (token,descripcion,valor) values (:token,:descripcion,:valor)";

            //using (OracleCommand cmd = new OracleCommand(query))
            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":token", value1);
                cmd.Parameters.Add(":descripcion", text);
                cmd.Parameters.Add(":valor", value2);
                cmd.ExecuteNonQuery();
            }
        }
        internal void pModificarEstandar(string value1, string text, string value2)
        {
            string query = "update flex.ll_estandar set descripcion = :descripcion,valor = :valor where token = :token";

            //using (OracleCommand cmd = new OracleCommand(query))
            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":descripcion", text);
                cmd.Parameters.Add(":valor", value2);
                cmd.Parameters.Add(":token", value1);
                cmd.ExecuteNonQuery();
            }
        }
        internal void pEliminarEstandar(string value)
        {
            string query = "delete from flex.ll_estandar where token = :token";

            //using (OracleCommand cmd = new OracleCommand(query))
            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":token", value);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion Estandar
        public string prueba()
        {
            string sql = "select * from ll_prueba";
            string resultado = "";

            using (OracleCommand comm = new OracleCommand(sql, handler.ConexionOracle.ConexionOracleSQL)) // create the oracle sql command
            {
                using (OracleDataReader rdr = comm.ExecuteReader()) // execute the oracle sql and start reading it
                {
                    while (rdr.Read()) // loop through each row from oracle
                    {
                        //Console.WriteLine(rdr[0]);             // You can do this
                        //Console.WriteLine(rdr.GetString(0); );  // or this
                        Console.WriteLine(rdr["sbvalor"]); // or this
                        resultado = rdr["sbvalor"].ToString();
                    }
                    rdr.Close(); // close the oracle reader
                }
            }
            handler.ConexionOracle.ConexionOracleSQL.Close(); // close the oracle connection

            return resultado;
        }

        public void AplicarEnBD()
        {
            try
            {
                //pExecuteSqlplus("sql_llozada/NOV0118@sfba0708", "E:\\Trabajo\\Net\\WpfMark53\\pruebas\\OC824070\\Flex", "824070_apl_FLEX_CUSTOMIZACION.sql");
                /*var content = File.ReadAllText("E:\\pkg_epm_liquidabenefmunicipio.sql");

                OracleCommand cmd = new OracleCommand(content);
                cmd.Connection = handler.ConexionOracle;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();*/

            }
            catch (System.Exception ex)
            {

            }
        }

        /*public static void GetTags(string empresa, string tipo_objeto)
        {
            string sbprueba = "";
            string sbQuery = "SELECT tipo_objeto,prefijo,sub_metodo,tag_inicio,tag_fin " + 
                             "FROM   ll_estandar, ll_documentacion "+ 
                             "WHERE  codigo = estandar_id "+
                             "AND    empresa = "+ empresa +" "+
                             "AND    tipo_objeto = "+ tipo_objeto;

            using (MySqlCommand cmd = ConexionMySql.ExecuteQuery(sbQuery))
            {
                MySqlDataReader reader = null;

                try
                { 
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                try
                {
                    while (reader.Read())
                    {
                        sbprueba = reader.GetString(0);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }*/

        /// <summary>
        /// Obtiene los datos de la base datos
        /// </summary>
        /// <returns></returns>
        /*public void pGetDatos(FileObjects objetoBd, List<ObjectType> datos, out string user) //(Varchar2 metodo, Boolean nuevo, List<ObjectType> datos, Varchar2 nombre_archivo, Varchar2 archivo_aplica, out Varchar2 user)
        {
            objetoBd.Owner = "";

            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pObtDatos"))
            {
                Conexion.AddInParameter(cmd, "isbMetodo", OracleDbType.Varchar2, objetoBd.ObjetoBd);
                Conexion.AddInParameter(cmd, "isbNovedad", OracleDbType.Varchar2, objetoBd.FlagNuevo ? "Nuevo" : "Cambio");
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddOutParameter(cmd, "osbUser", OracleDbType.Varchar2);
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        ObjectType dato = new ObjectType();

                        dato.Nombre = Convert.ToVarchar2(reader["nombre"]);
                        dato.Tipo = Convert.ToVarchar2(reader["tipo"]);
                        dato.Novedad = Convert.ToVarchar2(objetoBd.FlagNuevo ? "Nuevo" : "Cambio");
                        dato.Owner = Convert.ToVarchar2(reader["owner"]);
                        objetoBd.Owner = dato.Owner;
                        dato.NombreArchivo = objetoBd.Descripcion;
                        dato.Aplica = objetoBd.File;
                        datos.Add(dato);
                    }

                    user = Convert.ToString(Conexion.GetParameterValue(cmd, "osbUser"));
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }

        /// <summary>
        /// Guarda los datos en memoria
        /// </summary>
        /// <param name="metodo"></param>
        internal void psetDatos(string metodo)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.psetDatos"))
            {
                Conexion.AddInParameter(cmd, "isbMetodo", OracleDbType.Varchar2, metodo);
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );
            }

        }

        internal List<ObjetosAtributos> pGetAtributtes(Varchar2 nombre, Varchar2 tipo, Varchar2 novedad, Varchar2 numeroOC)
        {
            List<ObjetosAtributos> datos = new List<ObjetosAtributos>(); ;

            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pObtMetodos"))
            {
                Conexion.AddInParameter(cmd, "isbMetodo", OracleDbType.Varchar2, nombre);
                Conexion.AddInParameter(cmd, "isbTipo", OracleDbType.Varchar2, tipo);
                Conexion.AddInParameter(cmd, "isbOcDatos", OracleDbType.Varchar2, numeroOC);
                Conexion.AddInParameter(cmd, "isbNovedad", OracleDbType.Varchar2, novedad);
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        ObjetosAtributos dato = new ObjetosAtributos();

                        dato.Paquete = nombre;
                        dato.Metodo = Convert.ToVarchar2(reader["metodo"]);
                        dato.Novedad = novedad;
                        dato.TipoServicio = "Expuesto";
                        dato.Comportamiento = Convert.ToVarchar2(reader["descripcion"]);
                        dato.EntradaVariable = Convert.ToVarchar2(reader["entrada"]);
                        dato.Variables = Convert.ToVarchar2(reader["nombre"]);
                        datos.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }

            return datos;
        }

        internal void pGeneraDocHtml(Varchar2 nombre, Varchar2 tipo, Handler handler)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pObtDocumentacion"))
            {
                Conexion.AddInParameter(cmd, "isbPackage", OracleDbType.Varchar2, nombre);
                Conexion.AddInParameter(cmd, "isbTipo", OracleDbType.Varchar2, tipo);
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    handler.ListDocParametrosHtml.Clear();
                    handler.ListDocModificacionesHTML.Clear();

                    //Se obtienen los parámetros
                    while (reader.Read())
                    {
                        ParametrosHTML dato = new ParametrosHTML();

                        dato.Nombre = Convert.ToVarchar2(reader["nombre"]);
                        dato.Tipo = Convert.ToVarchar2(reader["tipo"]);
                        dato.DescripcionFuente = Convert.ToVarchar2(reader["descripcionFuente"]);
                        dato.DescripcionPara = Convert.ToVarchar2(reader["descripcionPara"]);
                        dato.FechaFuente = Convert.ToVarchar2(reader["fechaFuente"]);
                        dato.Metodo = Convert.ToVarchar2(reader["metodo"]);
                        dato.Entrada = Convert.ToVarchar2(reader["entrada"]);
                        dato.AutorFuente = Convert.ToVarchar2(reader["autorFuente"]);
                        dato.Fuente = Convert.ToVarchar2(reader["fuente"]);

                        handler.ListDocParametrosHtml.Add(dato);
                    }

                    reader.NextResult();

                    //Se obtienen las modificaciones
                    while (reader.Read())
                    {
                        ModificacionesHTML datomod = new ModificacionesHTML();

                        datomod.Autor = Convert.ToVarchar2(reader["autor"]);
                        datomod.DescripcionMod = Convert.ToVarchar2(reader["descripcionMod"]);
                        datomod.FechaFuente = Convert.ToVarchar2(reader["fechaFuente"]);
                        datomod.FechaMod = Convert.ToVarchar2(reader["fechaMod"]);
                        datomod.Metodo = Convert.ToVarchar2(reader["metodo"]);
                        datomod.Numero_OC = Convert.ToVarchar2(reader["numero_OC"]);
                        datomod.DescripcionFuente = Convert.ToVarchar2(reader["descripcionFuente"]);
                        datomod.AutorFuente = Convert.ToVarchar2(reader["autorFuente"]);
                        datomod.Fuente = Convert.ToVarchar2(reader["fuente"]);

                        handler.ListDocModificacionesHTML.Add(datomod);
                    }

                    reader.NextResult();

                    //Se obtienen las modificaciones
                    while (reader.Read())
                    {
                        RetornoHTML retorno = new RetornoHTML();

                        retorno.Nombre = Convert.ToVarchar2(reader["nombre"]);
                        retorno.Tipo = Convert.ToVarchar2(reader["tipo"]);
                        retorno.FechaFuente = Convert.ToVarchar2(reader["fechaFuente"]);
                        retorno.Descripcion = Convert.ToVarchar2(reader["descripcion"]);
                        retorno.Metodo = Convert.ToVarchar2(reader["metodo"]);
                        retorno.DescripcionFuente = Convert.ToVarchar2(reader["descripcionFuente"]);
                        retorno.AutorFuente = Convert.ToVarchar2(reader["autorFuente"]);
                        retorno.Fuente = Convert.ToVarchar2(reader["fuente"]);

                        handler.ListDocRetornoHTML.Add(retorno);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }

        internal void getUser(out string user)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.fsbObtUsuario"))
            {
                Conexion.AddInParameter(cmd, "isbLogin", OracleDbType.Varchar2, null);
                Conexion.AddOutParameter(cmd, "osbUser", OracleDbType.Varchar2);
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                user = Convert.ToVarchar2(Conexion.GetParameterValue(cmd, "osbUser"));
            }
        }

        internal List<DatosTabla> pGetDataTable(Varchar2 nombre, List<DatosTabla> datos)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pObtDatosTabla"))
            {
                Conexion.AddInParameter(cmd, "isbTabla", OracleDbType.Varchar2, nombre);
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        DatosTabla dato = new DatosTabla();

                        dato.NombreTabla = Convert.ToVarchar2(reader["nombre"]); ;
                        dato.NombreColumna = Convert.ToVarchar2(reader["columna"]);
                        dato.Comentario = Convert.ToVarchar2(reader["comentario"]); ;
                        dato.Escala = Convert.ToVarchar2(reader["escala"]); ;
                        dato.Longitud = Convert.ToVarchar2(reader["longitud"]);
                        dato.Precision = Convert.ToVarchar2(reader["precision_"]);
                        dato.Nulo = Convert.ToVarchar2(reader["nulo"]);
                        dato.TipoDato = Convert.ToVarchar2(reader["tipo_dato"]);
                        datos.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }

            return datos;
        }

        internal void GuardarObjeto(Varchar2 nombreArchivo, Varchar2 tipo, Varchar2 numeroOC)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pGuardarObjetos"))
            {
                Conexion.AddInParameter(cmd, "isbNombre", OracleDbType.Varchar2, nombreArchivo);
                Conexion.AddInParameter(cmd, "isbTipo", OracleDbType.Varchar2, tipo);
                Conexion.AddInParameter(cmd, "isbRequ", OracleDbType.Varchar2, numeroOC);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }
            }
        }

        internal void EliminaDatos(Varchar2 numeroOC)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pEliminaDatos"))
            {
                Conexion.AddInParameter(cmd, "isbReque", OracleDbType.Varchar2, numeroOC);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }
            }
        }

        internal void Guardar()
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pGuardar"))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }
            }
        }

        internal void ObtenerEstandarEmpresa(Handler handler)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pObtEstandar"))
            {
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        EstandarEmpresa dato = new EstandarEmpresa();

                        dato.Codigo = Convert.ToVarchar2(reader["codigo"]);
                        dato.Empresa = Convert.ToVarchar2(reader["empresa"]);
                        dato.Prefijo = Convert.ToVarchar2(reader["prefijo"]);
                        dato.SubMetodo = Convert.ToVarchar2(reader["sub_metodo"]).Equals("S") ? Properties.Resources.TipoSubMetodo : "N";
                        dato.TipoUso = Convert.ToVarchar2(reader["tipo_uso"]);
                        dato.Tipo_Objeto = Convert.ToVarchar2(reader["tipo_objeto"]);
                        dato.Usuario = Convert.ToVarchar2(reader["usuario"]);
                        handler.EstandarEmpresa.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }

        internal void pObtReporteObjetosEntregados(Handler handler)
        {
            using (OracleCommand cmd = Conexion.GetStoredProcCommand("pkg_epm_utilidadesword.pReporteObjetosEntregados"))
            {
                Conexion.AddInParameter(cmd, "inuNumeroOC", OracleDbType.Varchar2, handler.NumeroOC);
                Conexion.AddParameterRefCursor(cmd,"");
                Conexion.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                Conexion.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                ExceptionHandler.EvaluateErrorCode
                (
                    Conexion.GetParameterValue(cmd, "onuErrorCode"),
                    Conexion.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        ObjetosEntregados dato = new ObjetosEntregados();

                        dato.Codigo = Convert.ToVarchar2(reader["codigo"]);
                        dato.Fecha_Registro = Convert.ToVarchar2(reader["fecha_registro"]);
                        dato.NumeroOc = Convert.ToVarchar2(reader["requerimiento"]);
                        dato.Objeto = Convert.ToVarchar2(reader["objeto"]);
                        dato.Tipo = Convert.ToVarchar2(reader["tipo"]);
                        dato.Usuario = Convert.ToVarchar2(reader["usuario"]);
                        handler.ListObjetosEntregados.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }*/
    }

}
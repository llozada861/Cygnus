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
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Conn;

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
            string query = handler.ListaHTML.Where(x => x.Nombre.Equals(res.KEY_SQL_PARAMETRO)).FirstOrDefault().Documentacion.Replace("\r\n", "\n");

            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":PARAMETRO_ID", parametro.ParameterId);
                cmd.Parameters.Add(":DESCRIPCION", parametro.Descripcion);
                cmd.Parameters.Add(":VALOR", parametro.Valor);
                cmd.Parameters.Add(":FUNCION", parametro.Funcion);
                cmd.Parameters.Add(":TIPO", parametro.Tipo.Text);
                cmd.ExecuteNonQuery();
            }
        }

        public void pCreaMensaje(MessageModel mensajesModel)
        {
            string query = handler.ListaHTML.Where(x => x.Nombre.Equals("SQL_MENSAJE")).FirstOrDefault().Documentacion.Replace("\r\n", "\n");

            using (OracleCommand cmd = new OracleCommand(query, handler.ConexionOracle.ConexionOracleSQL))
            {
                cmd.Parameters.Add(":CODIGO", mensajesModel.Codigo);
                cmd.Parameters.Add(":DESCRIPCION", mensajesModel.Descripcion);
                cmd.Parameters.Add(":CAUSA", mensajesModel.Causa);
                cmd.Parameters.Add(":SOLUCION", mensajesModel.Solucion);
                cmd.ExecuteNonQuery();
            }
        }

        public string pObtCodigoMensaje()
        {
            string sql = handler.ListaHTML.Where(x => x.Nombre.Equals("CODIGO_MENSAJE")).FirstOrDefault().Documentacion.Replace("\r\n", "\n");

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
                        " FROM   dba_errors " +
                        " WHERE  name = upper('" + archivo.NombreObjeto +"')"+
                        " AND    OWNER IN(" + pDevuelveUsuariosIn() + ") ";

            UsuarioModel userCompila = handler.ListaUsuarios.Where(x => x.Principal.Equals(res.Si)).FirstOrDefault();
            handler.pObtenerUsuarioCompilacion(userCompila.Usuariobd);

            OracleConnection con = handler.ConexionOracle.ConexionOracleCompila;

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

        public void pEjecutarScriptBD(string codigo,string usuario)
        {
            if(!string.IsNullOrEmpty(usuario))
                handler.pObtenerUsuarioCompilacion(usuario);

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
                         "       FROM   dba_objects " +
                         "       WHERE  object_name = upper('" + archivo.NombreObjeto +"') " +
                         "       AND    OWNER IN("+ pDevuelveUsuariosIn() + ") " +
                         "   )";

            UsuarioModel userCompila = handler.ListaUsuarios.Where(x => x.Principal.Equals(res.Si)).FirstOrDefault();
            handler.pObtenerUsuarioCompilacion(userCompila.Usuariobd);

            OracleConnection con = handler.ConexionOracle.ConexionOracleCompila;

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
                    "       FROM   dba_objects " +
                    "       WHERE  object_name = upper('" + archivo.NombreObjeto + "') " +
                    "       AND    OWNER IN(" + pDevuelveUsuariosIn() + ") ";

            UsuarioModel userCompila = handler.ListaUsuarios.Where(x => x.Principal.Equals(res.Si)).FirstOrDefault();
            handler.pObtenerUsuarioCompilacion(userCompila.Usuariobd);

            OracleConnection con = handler.ConexionOracle.ConexionOracleCompila;

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

            UsuarioModel userCompila = handler.ListaUsuarios.Where(x => x.Principal.Equals(res.Si)).FirstOrDefault();
            handler.pObtenerUsuarioCompilacion(userCompila.Usuariobd);

            OracleConnection con = handler.ConexionOracle.ConexionOracleCompila;

            sql = "select count(distinct name)  cantidad "+
                    "from dba_errors "+
                    "where owner in ("+ pDevuelveUsuariosIn()+") "+
                    "and attribute = 'ERROR'";

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

            return stCantidadObjetosInvalidos;
        }

        public void pObtConsultaObjetos(string nombreObjeto, BlockViewModel view, UsuariosPDN conexion)
        {
            string sql;

            sql = "SELECT a.owner, " +
                    "a.object_name, " +
                    "a.OBJECT_TYPE, " +
                    "a.STATUS " +
                    "FROM dba_objects a " +
                    "WHERE a.object_name LIKE '%"+ nombreObjeto.ToUpper()+"%' "+
                    "AND a.OWNER IN ( " + pDevuelveUsuariosIn()+") ";


            handler.ConexionOracle.RealizarConexionProd(conexion);
            OracleConnection con = handler.ConexionOracle.ConexionOracleProd;

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
                        if (!view.Model.ListaArchivosEncontrados.ToList().Exists(x => (x.FileName.Equals(dato.FileName) && x.Owner.Equals(dato.Owner))))
                            view.Model.ListaArchivosEncontrados.Add(dato);
                    }
                    reader.Close();
                }
            }

            handler.ConexionOracle.ConexionOracleProd.Close();
        }
        public List<ConexionModel> pObtListaBD()
        {
            List<ConexionModel> lista = new List<ConexionModel>();
            
            string sql = "SELECT DISTINCT user_, " +
                        "password_, " +
                        "basedatos, " +
                        "servidor, "+
                        "puerto "+
                        "FROM cy_userbd";


            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Connection = con;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ConexionModel dato = new ConexionModel();

                        dato.Usuario = Convert.ToString(reader["user_"]);
                        dato.Pass = Convert.ToString(reader["password_"]);
                        dato.BaseDatos = Convert.ToString(reader["basedatos"]);
                        dato.Servidor = Convert.ToString(reader["servidor"]);
                        dato.Puerto = Convert.ToString(reader["puerto"]);

                        //Si no existe que adicione el registro
                        if (!lista.Exists(x => (x.BaseDatos.Equals(dato.BaseDatos) )))
                            lista.Add(dato);
                    }
                    reader.Close();
                }
            }

            return lista;
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

                try
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WorkingDirectory = scriptDir;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.RedirectStandardInput = false;
                    process.StartInfo.FileName = Path.Combine(handler.ConfGeneralView.Model.RutaSqlplus, "sqlplus.exe");
                    process.StartInfo.Arguments = string.Format("{0} @\"{1}\" ", credentials, sbAplica);
                    process.StartInfo.CreateNoWindow = false;

                    process.Start();
                    process.WaitForExit();
                    process.Close();
                }
                catch 
                {
                    throw new System.Exception("Verifique que la ruta del SqlPlus.exe [" + handler.ConfGeneralView.Model.RutaSqlplus + "] se la correcta en [Ajustes/General/Rutas]");
                }

                //string output = null; //process.StandardOutput.ReadToEnd();
                //process.StandardInput.WriteLine("exit;");
            }

            return output;
        }
        #endregion CompilacionObjetos
                        
        #region GenereacionPaquetes
        internal OracleClob pGeneraPktbl(string tabla, SelectListItem usuarioBD, string caso,Handler handler)
        {
            UsuarioModel userCompila = handler.ListaUsuarios.Where(x => x.Usuariobd.Equals(usuarioBD.Text.ToUpper())).FirstOrDefault();
            handler.pObtenerUsuarioCompilacion(userCompila.Usuariobd);

            OracleConnection conn = handler.ConexionOracle.ConexionOracleCompila;

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

            OracleParameter sbErrorMessage = new OracleParameter("osbErrorMessage", OracleDbType.Varchar2);
            sbErrorMessage.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(sbErrorMessage);

            sqlPktbl.ExecuteReader();

            return (OracleClob)sqlPktbl.Parameters["oclFile"].Value;
        }
        #endregion GenereacionPaquetes

        #region Fuentes PL
        internal OracleClob pGeneraFuente(string nombre, string owner, UsuariosPDN conexion)
        {
            handler.ConexionOracle.RealizarConexionProd(conexion);
            OracleConnection conn = handler.ConexionOracle.ConexionOracleProd;

            OracleCommand sqlPktbl = new OracleCommand(handler.ListaHTML.Where(x => x.Nombre.Equals("PLANTILLA_FUENTES")).FirstOrDefault().Documentacion.Replace("\r\n", "\n"), conn);

            sqlPktbl.BindByName = true;

            OracleParameter sbTabla = new OracleParameter("isbNombre", OracleDbType.Varchar2);
            sbTabla.Value = nombre.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbTabla);

            OracleParameter sbOwner = new OracleParameter("isbOwner", OracleDbType.Varchar2);
            sbOwner.Value = owner.ToUpper().Trim();
            sqlPktbl.Parameters.Add(sbOwner);

            OracleParameter clFile = new OracleParameter("oclObjeto", OracleDbType.Clob);
            clFile.Direction = ParameterDirection.Output;
            sqlPktbl.Parameters.Add(clFile);

            sqlPktbl.ExecuteReader();

            return (OracleClob)sqlPktbl.Parameters["oclObjeto"].Value;
        }
        #endregion Fuentes PL

        #region Auditoria
        internal void pGeneraAuditoria(TbAuditoriaModel model, out OracleClob tabla, out OracleClob trigger)
        {
            UsuarioModel userCompila = handler.ListaUsuarios.Where(x => x.Principal.Equals(res.Si)).FirstOrDefault();
            handler.pObtenerUsuarioCompilacion(userCompila.Usuariobd);

            OracleConnection conn = handler.ConexionOracle.ConexionOracleCompila;

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
    }

}
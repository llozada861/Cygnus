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
        internal void pObtBackup(RefreshViewModel view)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtBackup"))
            {
                handler.ConexionOracle.AddOutParameter(cmd, "onuSeqObjBl", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "onuSeqLogap", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "onuSeqRq", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "onuSeqHH", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "onuSeqNeg", OracleDbType.Int64);
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    view.ListaInsertCM.Clear();
                    view.ListaUsuarios.Clear();
                    view.ListaObjetosBl.Clear();
                    view.ListaSaUser.Clear();
                    view.ListaPerson.Clear();
                    view.ListaHoja.Clear();
                    view.ListaRQ.Clear();
                    view.ListaHH.Clear();

                    reader = cmd.ExecuteReader();

                    //CredMark
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaInsertCM.Add(dato);
                    }

                    reader.NextResult();

                    //ll_usuarios
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaUsuarios.Add(dato);
                    }

                    reader.NextResult();

                    //Ll_objetosbl
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaObjetosBl.Add(dato);
                    }

                    reader.NextResult();

                    //sa_user
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaSaUser.Add(dato);
                    }

                    reader.NextResult();

                    //ge_person
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaPerson.Add(dato);
                    }

                    reader.NextResult();

                    //ll_hojas
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaHoja.Add(dato);
                    }

                    reader.NextResult();

                    //ll_requerimiento
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaRQ.Add(dato);
                    }

                    reader.NextResult();

                    //ll_horashoja
                    while (reader.Read())
                    {
                        SelectListItem dato = new SelectListItem();

                        dato.Text = Convert.ToString(reader["VALOR"]);
                        view.ListaHH.Add(dato);
                    }
                }
                catch (InvalidCastException)
                {}

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                view.ObjetosBl = handler.ConexionOracle.GetParameterValue(cmd, "onuSeqObjBl");
                view.ObjetosLog = handler.ConexionOracle.GetParameterValue(cmd, "onuSeqLogap");
                view.onuSeqRq = handler.ConexionOracle.GetParameterValue(cmd, "onuSeqRq");
                view.onuSeqHH = handler.ConexionOracle.GetParameterValue(cmd, "onuSeqHH");
                view.onuSeqNeg = handler.ConexionOracle.GetParameterValue(cmd, "onuSeqNeg");
            }
        }
        internal void pObtBackupNuevo(RefreshViewModel view)
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
        }
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


        internal void pObtCodigoSql()
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtCodigoSql"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbVersion", OracleDbType.Varchar2, handler.fsbVersion);
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                reader = cmd.ExecuteReader();

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        OracleBlob blob = reader.GetOracleBlob(0);
                        MemoryStream ms = new MemoryStream(blob.Value);
                        StreamReader sr = new StreamReader(ms);
                        string text = sr.ReadToEnd();
                        SqliteDAO.pExecuteNonQuery(text);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }
        #endregion Updater

        #region GestionObjetos
        internal void pActualizaFecha(Archivo archivo, DateTime fecha)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pActualizaFecha"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbNombreObj", OracleDbType.Varchar2, archivo.FileName);
                handler.ConexionOracle.AddInParameter(cmd, "isbOwnerObj", OracleDbType.Varchar2, archivo.Owner);
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, handler.ConnView.Model.Usuario.ToUpper());
                handler.ConexionOracle.AddInParameter(cmd, "isbFechaLib", OracleDbType.Varchar2, fecha.Day + "-" + fecha.Month + "-" + fecha.Year);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }
        internal string pObtGrupoCorreo()
        {
            string sbGrupo = "";

            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtGrupoCorreo"))
            {
                handler.ConexionOracle.AddOutParameter(cmd, "osbGrupoCorreo", OracleDbType.Varchar2);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                sbGrupo = handler.ConexionOracle.GetParameterValue(cmd, "osbGrupoCorreo").ToString();
            }

            return sbGrupo;
        }
        public void pBloqueaObjeto(Archivo archivo, BlockViewModel view)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pBloqueaObjeto"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbNombreObj", OracleDbType.Varchar2, archivo.FileName);
                handler.ConexionOracle.AddInParameter(cmd, "isbOwnerObj", OracleDbType.Varchar2, archivo.Owner);
                handler.ConexionOracle.AddInParameter(cmd, "isbNumCaso", OracleDbType.Varchar2, view.Codigo);
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, handler.ConnView.Model.Usuario.ToUpper());
                handler.ConexionOracle.AddInParameter(cmd, "isbFechaLib", OracleDbType.Varchar2, view.Fecha.Day + "-" + view.Fecha.Month + "-" + view.Fecha.Year);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }

        public void pObtObjetosBloqueados(DesblockViewModel view)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtObjetosBloqueados"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, handler.ConnView.Model.Usuario.ToUpper());
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        Archivo dato = new Archivo();

                        dato.FileName = Convert.ToString(reader["objeto"]);
                        dato.Owner = Convert.ToString(reader["owner"]);
                        dato.OrdenCambio = Convert.ToString(reader["orden"]);
                        dato.FechaBloqueo = Convert.ToString(reader["fecha_bloqueo"]);
                        dato.FechaEstLib = Convert.ToString(reader["fecha_liberacion"]);

                        view.ListaArchivosBloqueo.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }

        public void pDesbloqueaObjeto(Archivo archivo)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pDesbloqueaObjeto"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbNombreObj", OracleDbType.Varchar2, archivo.FileName);
                handler.ConexionOracle.AddInParameter(cmd, "isbOwnerObj", OracleDbType.Varchar2, archivo.Owner);
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, handler.ConnView.Model.Usuario.ToUpper());
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }
        public void pObtObjetosBloqTodos(string objeto)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtObjetosBloqTodos"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbObjeto", OracleDbType.Varchar2, objeto.ToUpper().Trim());
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        Archivo dato = new Archivo();

                        dato.FileName = Convert.ToString(reader["objeto"]);
                        dato.Owner = Convert.ToString(reader["owner"]);
                        dato.OrdenCambio = Convert.ToString(reader["orden"]);
                        dato.FechaBloqueo = Convert.ToString(reader["fecha_bloqueo"]);
                        dato.FechaEstLib = Convert.ToString(reader["fecha_est_liberacion"]);
                        dato.Usuario = Convert.ToString(reader["usuario"]);

                        //handler.ListaArchivosBloqueo.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
        }
        #endregion GestionObjetos

        #region GestionUsuarios
        public void pGuardaPass(string usuario, string pass)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pGuardaCodigo"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, usuario);
                handler.ConexionOracle.AddInParameter(cmd, "isbCodigo", OracleDbType.Varchar2, pass);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }

        public void pGuardaRol(string usuario, string pass, string email)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pGuardaRol"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, usuario);
                handler.ConexionOracle.AddInParameter(cmd, "isbCodigo", OracleDbType.Varchar2, pass);
                handler.ConexionOracle.AddInParameter(cmd, "isbEmail", OracleDbType.Varchar2, email);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }
        public int pObtRol(string username)
        {
            string sbRol = "";
            int nuRol = 0;
            string[] rolList;
            string rolEncrypt;

            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtRol"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, username.ToUpper().Trim());
                handler.ConexionOracle.AddOutParameter(cmd, "osbRol", OracleDbType.Varchar2);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                rolEncrypt = handler.ConexionOracle.GetParameterValue(cmd, "osbRol");

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                sbRol = EncriptaPass.Desencriptar(rolEncrypt);

                rolList = sbRol.Split('-');

                if (rolList.Count() > 0)
                {
                    if (rolList[0].Equals(username.ToUpper().Trim()))
                    {
                        nuRol = Convert.ToInt32(rolList[1]);
                    }
                }
            }

            return nuRol;
        }
        #endregion GetionUsuarios

        #region CompilacionObjetos
        public void pObtErrores(Archivo archivo, CompilaModel model)
        {
            string sql = "SELECT " +
                               "NAME ," +
                               "TYPE ," +
                               "LINE ," +
                               "TEXT " +
                        " FROM   all_errors " +
                        " WHERE  name = '" + archivo.NombreObjeto +"'"+
                        " AND    owner IN (SELECT usuario FROM FLEX.ll_credmark)";

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

        public string pObtenerUsuarioCompilacion(string usuario)
        {
            string credenciales = "";

            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtieneCodigo"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, usuario);
                handler.ConexionOracle.AddOutParameter(cmd, "osbCodigo", OracleDbType.Varchar2);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                credenciales = handler.ConexionOracle.GetParameterValue(cmd, "osbCodigo");
            }

            return credenciales;
        }

        public void pValidaUsuarioCompila(Archivo archivo, Handler handler)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pValidaUsuarioApl"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbObjeto", OracleDbType.Varchar2, archivo.NombreObjeto.ToUpper());
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }

        public void pValidaObjEsquema(Archivo archivo, string usuario)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pValidaObjEsquema"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbObjeto", OracleDbType.Varchar2, archivo.NombreObjeto.ToUpper());
                handler.ConexionOracle.AddInParameter(cmd, "isbUsuario", OracleDbType.Varchar2, usuario.ToUpper());
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );
            }
        }
        public string pObtCantObjsInvalidos()
        {
            string stCantidadObjetosInvalidos = null;

            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {

                using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtCantObjsInvalidos"))
                {
                    handler.ConexionOracle.AddOutParameter(cmd, "onuCantObjetos", OracleDbType.Int64);
                    handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                    handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (InvalidCastException)
                    {
                    }

                    handler.EvaluateErrorCode
                    (
                        handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                        handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                    );

                    stCantidadObjetosInvalidos = handler.ConexionOracle.GetParameterValue(cmd, "onuCantObjetos").ToString();
                }
            }

            return stCantidadObjetosInvalidos;
        }

        public void pObtConsultaObjetos(string nombreObjeto, BlockViewModel view)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pObtConsultaObjetos"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbNombreObj", OracleDbType.Varchar2, nombreObjeto.ToUpper().Trim());
                handler.ConexionOracle.AddParameterRefCursor(cmd, "");
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                OracleDataReader reader = null;

                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                try
                {
                    while (reader.Read())
                    {
                        Archivo dato = new Archivo();

                        dato.FileName = Convert.ToString(reader["object_name"]);
                        dato.Usuario = Convert.ToString(reader["usuario"]);
                        dato.Observacion = Convert.ToString(reader["bloqueado"]);
                        dato.FechaBloqueo = Convert.ToString(reader["fecha_bloqueo"]);
                        dato.FechaEstLib = Convert.ToString(reader["fecha_est_lib"]);
                        dato.OrdenCambio = Convert.ToString(reader["orden"]);
                        dato.Owner = Convert.ToString(reader["owner"]);

                        //Si no existe que adicione el registro
                        if (!view.ListaArchivosEncontrados.ToList().Exists(x => (x.FileName.Equals(dato.FileName) && x.Owner.Equals(dato.Owner))))
                            view.ListaArchivosEncontrados.Add(dato);
                    }
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
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
        internal OracleClob pGeneraPktbl(string tabla, SelectListItem usuarioBD, string caso)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("pkg_utilmark.pGenerapktbl"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbTabla", OracleDbType.Varchar2, tabla.ToUpper().Trim());
                handler.ConexionOracle.AddInParameter(cmd, "isbOwner", OracleDbType.Varchar2, usuarioBD.Text);
                handler.ConexionOracle.AddInParameter(cmd, "isOrder", OracleDbType.Varchar2, caso.ToUpper().Trim());
                handler.ConexionOracle.AddOutParameter(cmd, "oclFile", OracleDbType.Clob);
                handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);
                
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );

                return (OracleClob)cmd.Parameters["oclFile"].Value;
            }
        }
        #endregion GenereacionPaquetes

        #region Reporte
        public List<TareaHoja> pObtListaTareas(ReportViewModel view)
        {
            List<TareaHoja> listaTareas = new List<TareaHoja>();

            string sql = "SELECT * FROM (" +
                        "            SELECT fecha_ini fecha, " +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   lunes horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND lunes > 0                        " +
                        "            UNION" +
                        "            SELECT fecha_ini+1 fecha," +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   martes horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND   hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND martes > 0" +
                        "            UNION" +
                        "            SELECT fecha_ini+2 fecha," +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   miercoles horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND miercoles > 0" +
                        "            UNION" +
                        "            SELECT fecha_ini+3 fecha," +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   jueves horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND jueves > 0" +
                        "            UNION" +
                        "            SELECT fecha_ini+4 fecha," +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   viernes horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND viernes > 0" +
                        "            UNION" +
                        "            SELECT fecha_ini+5 fecha," +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   sabado horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND sabado > 0" +
                        "            UNION" +
                        "            SELECT fecha_ini+6 fecha," +
                        "                   nvl(rq.hist_usuario,0) hist_usuario," +
                        "                   rq.id_azure," +
                        "                   rq.descripcion," +
                        "                   domingo horaCygnus," +
                        "                   nvl(rq.completado,0) horaAzure" +
                        "            FROM ll_horashoja hh,ll_hoja ho, ll_requerimiento rq" +
                        "            WHERE hh.usuario = :usuario" +
                        "            AND   ho.fecha_ini >= :fecha_i" +
                        "            AND   ho.fecha_ini <= :fecha_f" +
                        "            AND hh.id_hoja = ho.codigo" +
                        "            AND hh.requerimiento = rq.codigo" +
                        "            AND domingo > 0 " +
                        "            )" +
                        "            ORDER BY fecha";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.Add(":usuario", handler.ConnView.Model.Usuario.ToUpper());
                cmd.Parameters.Add(":fecha_i", view.FechaDesde.ToShortDateString());
                cmd.Parameters.Add(":fecha_f", view.FechaHasta.ToShortDateString());
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader()) // execute the oracle sql and start reading it
                {
                    while (rdr.Read()) // loop through each row from oracle
                    {
                        listaTareas.Add(new TareaHoja { FechaCreacion = rdr["fecha"].ToString(),
                                                        HU = Convert.ToInt32(rdr["hist_usuario"]),
                                                        IdAzure = Convert.ToInt32(rdr["id_azure"].ToString()),
                                                        Descripcion = rdr["descripcion"].ToString(),
                                                        Total = Convert.ToDouble(rdr["horaCygnus"].ToString())
                        });
                    }
                    rdr.Close(); // close the oracle reader
                }
            }

            return listaTareas;
        }
        public List<TareaHoja> pObtListaTaskAzure(ReportViewModel view)
        {
            List<TareaHoja> listaTareas = new List<TareaHoja>();

            string sql = "SELECT * FROM (" +
                        "SELECT fecha_inicio fecha," +
                        "       nvl(hist_usuario,0) hist_usuario," +
                        "       id_azure," +
                        "       descripcion," +
                        "       estado," +
                        "       nvl(completado,0) completado " +
                        "FROM ll_requerimiento " +
                        "WHERE usuario = :usuario " +
                        "AND fecha_inicio >= :fecha_i " +
                        "AND fecha_inicio <= :fecha_f "+
                        ") ORDER BY fecha";

            OracleConnection con = handler.ConexionOracle.ConexionOracleSQL;

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.Add(":usuario", handler.ConnView.Model.Usuario.ToUpper());
                cmd.Parameters.Add(":fecha_i", view.FechaDesde.ToShortDateString());
                cmd.Parameters.Add(":fecha_f", view.FechaHasta.ToShortDateString());
                cmd.Connection = con;

                using (OracleDataReader rdr = cmd.ExecuteReader()) // execute the oracle sql and start reading it
                {
                    while (rdr.Read()) // loop through each row from oracle
                    {
                        listaTareas.Add(new TareaHoja
                        {
                            FechaCreacion = rdr["fecha"].ToString(),
                            HU = Convert.ToInt32(rdr["hist_usuario"]),
                            IdAzure = Convert.ToInt32(rdr["id_azure"].ToString()),
                            Descripcion = rdr["descripcion"].ToString(),
                            Estado = rdr["estado"].ToString(),
                            Total = Convert.ToDouble(rdr["completado"].ToString())
                        });
                    }
                    rdr.Close(); // close the oracle reader
                }
            }

            return listaTareas;
        }
        #endregion Reporte

        #region Auditoria
        internal void pGeneraAuditoria(TbAuditoriaModel model, out OracleClob tabla, out OracleClob trigger)
        {
            using (OracleCommand cmd = handler.ConexionOracle.GetStoredProcCommand("flex.p_DC_GeneraAudit"))
            {
                handler.ConexionOracle.AddInParameter(cmd, "isbTableName", OracleDbType.Varchar2, model.Tabla.ToUpper().Trim());
                handler.ConexionOracle.AddInParameter(cmd, "isbAutor", OracleDbType.Varchar2, model.Autor.Trim());
                handler.ConexionOracle.AddInParameter(cmd, "isbLogin", OracleDbType.Varchar2, model.Login.Trim());
                handler.ConexionOracle.AddInParameter(cmd, "isbTicket", OracleDbType.Varchar2, model.Ticket.ToUpper().Trim());
                handler.ConexionOracle.AddInParameter(cmd, "isbPK", OracleDbType.Varchar2, model.Primaria.ToUpper().Trim());
                handler.ConexionOracle.AddOutParameter(cmd, "osbScript", OracleDbType.Clob);
                handler.ConexionOracle.AddOutParameter(cmd, "osbTrgScript", OracleDbType.Clob);
                //handler.ConexionOracle.AddOutParameter(cmd, "onuErrorCode", OracleDbType.Int64);
                //handler.ConexionOracle.AddOutParameter(cmd, "osbErrorMessage", OracleDbType.Varchar2);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (InvalidCastException)
                {
                }

                /*handler.EvaluateErrorCode
                (
                    handler.ConexionOracle.GetParameterValue(cmd, "onuErrorCode"),
                    handler.ConexionOracle.GetParameterValue(cmd, "osbErrorMessage")
                );*/

                tabla = (OracleClob)cmd.Parameters["osbScript"].Value;
                trigger = (OracleClob)cmd.Parameters["osbTrgScript"].Value;
            }
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
            catch (Exception ex)
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
using Cygnus2_0.General;
using Cygnus2_0.Security;
using Cygnus2_0.ViewModel.Settings;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Conn
{
    public class ConexionOracle
    {
        #region Variables
        private ConexionViewModel conexion;
        private OracleConnection conexionOracleSql;
        private OracleConnection conexionOracleCompila;
        #endregion Variables

        public ConexionOracle(ConexionViewModel model)
        {
            this.conexion = model;
        }

        public void RealizarConexion()
        {
            string connectionstring = OracleConnString
                                        (
                                            conexion.Model.Servidor,
                                            conexion.Model.Puerto,
                                            conexion.Model.BaseDatos,
                                            conexion.Model.Usuario,
                                            conexion.Model.Pass
                                        );

            conexionOracleSql = new OracleConnection(connectionstring);
            conexionOracleSql.Open();
        }

        public void RealizarConexionCompilacion()
        {
            string connectionstring = OracleConnString
                                        (
                                            conexion.Model.Servidor,
                                            conexion.Model.Puerto,
                                            conexion.Model.BaseDatos,
                                            conexion.Model.UsuarioCompila,
                                            conexion.Model.PassCompila
                                        );

            conexionOracleCompila = new OracleConnection(connectionstring);
            conexionOracleCompila.Open();
        }

        public OracleConnection ConexionOracleSQL
        {
            get { return conexionOracleSql; }
            set { conexionOracleSql = value; }
        }

        public OracleConnection ConexionOracleCompila
        {
            get { return conexionOracleCompila; }
            set { conexionOracleCompila = value; }
        }

        public string OracleConnString(string host, string port, string servicename, string user, string pass)
        {
            return String.Format
            (
                  "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})" +
                  "(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};",
                  host,
                  port,
                  servicename,
                  user,
                  pass
            );
        }

        public OracleCommand GetStoredProcCommand(string metodo)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = ConexionOracleSQL;
            cmd.CommandText = metodo;
            cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }

        public OracleCommand GetScriptCommand(string script)
        {
            OracleCommand cmd = new OracleCommand(script);
            cmd.Connection = ConexionOracleCompila;
            cmd.CommandType = CommandType.Text;

            return cmd;
        }

        public void GetStoredProcCommand(OracleCommand cmd, string metodo)
        {
            cmd.CommandText = metodo;
            cmd.CommandType = CommandType.StoredProcedure;
        }

        public void AddInParameter(OracleCommand cmd, string nombre, OracleDbType tipo, string valor)
        {
            cmd.Parameters.Add(nombre, tipo).Value = valor;
        }

        public void AddOutParameter(OracleCommand cmd, string nombre, OracleDbType tipo)
        {
            cmd.Parameters.Add(nombre, tipo, 32767).Direction = ParameterDirection.Output;
        }

        public void AddParameterRefCursor(OracleCommand cmd, string nombre)
        {
            cmd.Parameters.Add(nombre, OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        }

        public string GetParameterValue(OracleCommand cmd, string nombre)
        {
            return cmd.Parameters[nombre].Value.ToString();
        }
    }
}

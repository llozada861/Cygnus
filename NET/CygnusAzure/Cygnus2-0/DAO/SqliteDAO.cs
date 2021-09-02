using Cygnus2_0.BaseDatos.sqlite;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.DAO
{
    public class SqliteDAO
    {
        public static void pCreaConfiguracion(string key, string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {               
                if (ifExist("configuration","key='"+key+"'",conn))
                {
                    query = "update configuration set value ='" + value + "' where key = '" + key+"'";
                    ExecuteNonQuery(query, conn);
                }
                else
                {
                    query = "insert into configuration (key,value) VALUES('"+key+"','"+value+"')";
                    ExecuteNonQuery(query, conn);
                }
            }
        }
        public static void pGuardarPalabra(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into words (description) VALUES('" + value + "')";
                ExecuteNonQuery(query, conn);                
            }
        }
        public static void pGuardarUsuario(string value,Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into user_grants (user,company) VALUES('" + value + "',"+ handler.ConfGeneralView.Model.Empresa.Value+ ")";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarUsuarioBD(string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into userbd (user,company) VALUES('" + value + "',"+ handler.ConfGeneralView.Model.Empresa.Value+")";
                ExecuteNonQuery(query, conn);
            }
        }

        public static void pGuardarTipoObjeto(string objeto, string slash, string cantidad, string proridad, string permiso)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into object_type (object,slash,count_slash,priority,grant) VALUES('" + objeto +"','"+ slash + "'," + cantidad + "," + proridad+ ",'" + permiso+"')";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarEncabezado(string sbEncabezado, string tipo, string proridad, string fin, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into object_head (head,type,priority,end,company) VALUES('" + sbEncabezado + "','" + tipo + "'," + proridad + ",'" + fin + "',"+ handler.ConfGeneralView.Model.Empresa.Value+")";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarConfHtml(string inicio, string fin, string atributos, string finEnca, string tipo, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into documentation (tag_ini,tag_fin,attributes,type,end,company) VALUES('" + inicio + "','" + fin + "','" + atributos + "','" + tipo+ "','"+finEnca+ "',"+ handler.ConfGeneralView.Model.Empresa.Value+")";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pActualizaRuta(string key, string path,int tipo_objeto, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "update object_path set path ='" + path + "' where object_type =" + tipo_objeto + " and company = "+ handler.ConfGeneralView.Model.Empresa.Value;
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pActualizaVersion(string version)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "update version set apply ='Y' where version_name ='" + version + "'";
                ExecuteNonQuery(query, conn);
            }
        }

        public static void pActualizaHtml(string key, string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "update html set documentation ='" + value + "' where name ='" + key + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Value;
                ExecuteNonQuery(query, conn);
            }
        }

        public static void pCreaConexion(Handler handler,string pass)
        {
            string query;
            string etiqueta = handler.ConnView.Model.Conexion.Etiqueta.ToUpper();
            string user = handler.ConnView.Model.Conexion.Usuario;
            string serv = handler.ConnView.Model.Conexion.Servidor;
            string bd = handler.ConnView.Model.Conexion.Bd;
            string port = handler.ConnView.Model.Conexion.Puerto;

            handler.ConnView.Model.Usuario = user;
            handler.ConnView.Model.Pass = pass;
            handler.ConnView.Model.BaseDatos = bd;
            handler.ConnView.Model.Servidor = serv;
            handler.ConnView.Model.Puerto = port;

            string defecto = handler.ConnView.Model.Conexion.BlValor ? "S" : "N";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {

                try
                {
                    query = "insert into conection (user,pass,bd,server,port,company,active,name_) VALUES('" + user + "','" + pass + "','" + bd + "','" + serv + "','" + port + "'," + handler.ConfGeneralView.Model.Empresa.Value + ",'"+ defecto+"','"+etiqueta.ToUpper()+"')";
                    ExecuteNonQuery(query, conn);
                }
                catch
                {
                    query = "update conection set pass ='" + pass + "'," +
                                                 "port = '" + port + "'," +
                                                 "active = '" + defecto + "'," +
                                                 "user = '" + user + "'," +
                                                 "bd = '" + bd + "'," +
                                                 "server = '" + serv + "'" +
                                                " where name_ = '" + etiqueta + "'";
                    ExecuteNonQuery(query, conn);
                }
            }
        }

        internal static void pEliminarConexion(SelectListItem conexion)
        {
            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                string query = "delete from conection where name_ = '" + conexion.Etiqueta+"'";
                ExecuteNonQuery(query, conn);
            }
        }

        public static Boolean pblValidaVersion(Handler handler)
        {
            string query = "select * from version where version_name = '"+handler.fsbVersion+"'";
            Boolean blExists = false;
            string apply = "N";
            bool blResultado;
            bool blNuevo = false;

            try
            {
                using (SQLiteConnection conn = DbContext.GetInstance())
                {
                    using (var command = new SQLiteCommand(query, conn))
                    {
                        SQLiteDataReader reader = command.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            if (!String.IsNullOrEmpty(reader.GetString(1)))
                            {
                                apply = reader.GetString(2);
                                blExists = true;
                            }
                        }

                        if (!ifExist("version", "version_name like '1.%'", conn))
                            blNuevo = true;
                    }
                }
            }
            catch
            {
                query = "CREATE TABLE version (id INTEGER PRIMARY KEY AUTOINCREMENT,version_name TEXT, apply   TEXT)";
                pExecuteNonQuery(query);
            }

            if (!blExists)
            {
                query = "insert into version (version_name,apply) values('" + handler.fsbVersion + "','N')";
                pExecuteNonQuery(query);
            }

            if (apply.Equals("Y"))
                blResultado = true;
            else
            {                
                if (blNuevo)
                {
                    //Se actualiza la versión
                    SqliteDAO.pActualizaVersion(handler.fsbVersion);
                    blResultado = true;
                }
                else
                    blResultado = false;
                
            }

            return blResultado;
        }

        public static void pCargaConfiguracion(Handler handler)
        {
            string query = "select * from configuration";
            string valor;
            string llave;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        llave = reader.GetString(0);
                        valor = reader.GetString(1);

                        handler.ListaConfiguracion.Add
                        (
                            new SelectListItem
                            {
                                Text = llave,
                                Value = valor
                            }
                        );
                    }
                }
            }
        }

        public static void pListaEncabezadoObjetos(Handler handler)
        {
            string query = "select * from object_head order by priority";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaEncabezadoObjetos.Add
                        (
                            new SelectListItem
                            {
                                Text = reader.GetString(0),
                                Value = reader.GetString(1),
                                Prioridad = reader.GetInt32(2),
                                Fin = reader.GetString(3)
                            }
                        );
                    }
                }
            }
        }

        public static void pListaTiposObjetos(Handler handler)
        {
            string query = "select codigo,object,slash,count_slash,priority,grant from object_type order by priority";

            if (!string.IsNullOrEmpty(handler.ConfGeneralView.Model.Empresa.Value))
            {
                using (SQLiteConnection conn = DbContext.GetInstance())
                {
                    using (var command = new SQLiteCommand(query, conn))
                    {
                        SQLiteDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = reader.GetInt32(0).ToString();
                            item.Text = reader.GetString(1);

                            if (!reader.IsDBNull(3))
                                item.CantidadSlash = reader.GetInt32(3);
                            else
                                item.CantidadSlash = 0;

                            item.Prioridad = Convert.ToInt32(reader.GetString(4));
                            item.Grant = reader.GetString(5);

                            /*if (!reader.IsDBNull(3))
                                item.Value = reader.GetString(2);
                            else
                                item.Value = res.No;*/

                            /*if (!reader.IsDBNull(5))
                                item.Path = reader.GetString(5);
                            else
                                item.Path = "";

                            if (!reader.IsDBNull(6))
                                item.Usuario = reader.GetString(6);
                            else
                                item.Usuario = "";*/

                            handler.ListaTiposObjetos.Add(item);
                        }

                        handler.ListaTiposObjetos.Add
                        (
                            new SelectListItem
                            {
                                Text = res.TipoAplica,
                                Value = "-1",
                                CantidadSlash = 0,
                                Prioridad = 200,
                                Grant = res.No
                            }
                        );
                    }
                }
            }
        }

        public static void pListaUsGrants(Handler handler)
        {
            string query = "select * from user_grants where company ="+handler.ConfGeneralView.Model.Empresa.Value;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaUsGrants.Add
                        (
                            new SelectListItem
                            {
                                Text = reader.GetString(0)
                            }
                        );
                    }
                }
            }
        }
        public static void pListaPalabrasReservadas(Handler handler)
        {
            string query = "select * from words";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaPalabrasReservadas.Add
                        (
                            new SelectListItem
                            {
                                Text = reader.GetString(0)
                            }
                        );
                    }
                }
            }
        }
        public static void pListaUsuarios(Handler handler)
        {
            string query = "select * from userbd where company = "+ handler.ConfGeneralView.Model.Empresa.Value;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaUsuarios.Add
                        (
                            new SelectListItem
                            {
                                Text = reader.GetString(0)
                            }
                        );
                    }
                }
            }
        }

        public static void pListaRutas(Handler handler)
        {
            string query = "select object_type,path,user_default,company from object_path where company = "+handler.ConfGeneralView.Model.Empresa.Value;

            if (!string.IsNullOrEmpty(handler.ConfGeneralView.Model.Empresa.Value))
            {
                using (SQLiteConnection conn = DbContext.GetInstance())
                {
                    using (var command = new SQLiteCommand(query, conn))
                    {
                        SQLiteDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem();

                            item.Value = reader.GetInt32(0).ToString();

                            if (!reader.IsDBNull(1))
                                item.Path = reader.GetString(1);
                            else
                                item.Path = "";

                            if (!reader.IsDBNull(2))
                                item.Usuario = reader.GetString(2);
                            else
                                item.Usuario = "";

                            handler.ListaRutas.Add(item);
                        }
                    }
                }

                if (handler.ListaRutas.Count() > 0)
                {
                    foreach (SelectListItem item in handler.ListaRutas)
                    {
                        handler.ListaTiposObjetos.ToList().Find(x => x.Value == item.Value).Usuario = item.Usuario;
                        handler.ListaTiposObjetos.ToList().Find(x => x.Value == item.Value).Path = item.Path;
                    }
                }
            }
        }
        public static void pListaHTML(Handler handler)
        {
            string query = "select * from html where company = "+ handler.ConfGeneralView.Model.Empresa.Value;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaHTML.Add
                        (
                            new SelectListItem
                            {
                                Text = reader.GetString(0),
                                Value = reader.GetString(1)
                            }
                        );
                    }
                }
            }
        }
        public static void pListaEmpresas(Handler handler)
        {
            string query = "select codigo, descripcion, azure,git,sonar,documentoad from company";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = reader.GetInt32(0).ToString();
                        item.Text = reader.GetString(1);

                        if (!reader.IsDBNull(2))
                            item.Azure = reader.GetString(2) == res.Si ? true : false; 
                        else
                            item.Azure = false;

                        if (!reader.IsDBNull(3))
                            item.Git = reader.GetString(3) == res.Si ? true : false; 
                        else
                            item.Git = false;

                        if (!reader.IsDBNull(4))
                            item.Sonar = reader.GetString(4) == res.Si ? true : false; 
                        else
                            item.Sonar = false;

                        if (!reader.IsDBNull(5))
                            item.DocumentoAD = reader.GetString(5);
                        else
                            item.DocumentoAD = "";

                        handler.ConfGeneralView.Model.ListaEmpresas.Add(item);
                    }
                }
            }
        }
        public static void pDatosBd(Handler handler, SelectListItem conexionActual)
        { 
            string query = "select * from conection where company = "+handler.ConfGeneralView.Model.Empresa.Value;

            handler.ConnView.Model.ListaConexiones.Clear();
            handler.ConnView.Model.Conexion = new SelectListItem();

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        SelectListItem item = new SelectListItem();
                        item.Usuario = reader.GetString(0);

                        if (!item.Usuario.ToLower().Equals("sql_usuario"))
                        {
                            item.Pass = reader.GetString(1);
                            item.Bd = reader.GetString(2);
                            item.Servidor = reader.GetString(3);
                            item.Puerto = reader.GetString(4);
                            item.Activo = reader.GetString(5);

                            if (!reader.IsDBNull(7))
                                item.Etiqueta = reader.GetString(7);

                            item.BlValor = false;

                            if (item.Activo == res.Si)
                                item.BlValor = true;

                            if (item.BlValor && conexionActual == null)
                            {
                                handler.ConnView.Model.Usuario = item.Usuario;
                                handler.ConnView.Model.Pass = item.Pass;
                                handler.ConnView.Model.BaseDatos = item.Bd;
                                handler.ConnView.Model.Servidor = item.Servidor;
                                handler.ConnView.Model.Puerto = item.Puerto;
                                handler.ConnView.Model.Conexion.Usuario = handler.ConnView.Model.Usuario;
                                handler.ConnView.Model.Conexion.Pass = handler.ConnView.Model.Pass;
                                handler.ConnView.Model.Conexion.Bd = handler.ConnView.Model.BaseDatos;
                                handler.ConnView.Model.Conexion.Puerto = handler.ConnView.Model.Puerto;
                                handler.ConnView.Model.Conexion.Servidor = handler.ConnView.Model.Servidor;
                                handler.ConnView.Model.Conexion.BlValor = item.BlValor;
                                handler.ConnView.Model.Conexion.Etiqueta = item.Etiqueta;
                            }
                            else
                            {
                                if (conexionActual != null && conexionActual.Usuario.Equals(item.Usuario) && conexionActual.Bd.Equals(item.Bd) && conexionActual.Servidor.Equals(item.Servidor))
                                {
                                    handler.ConnView.Model.Usuario = item.Usuario;
                                    handler.ConnView.Model.Pass = item.Pass;
                                    handler.ConnView.Model.BaseDatos = item.Bd;
                                    handler.ConnView.Model.Servidor = item.Servidor;
                                    handler.ConnView.Model.Puerto = item.Puerto;
                                    handler.ConnView.Model.Conexion.Usuario = handler.ConnView.Model.Usuario;
                                    handler.ConnView.Model.Conexion.Pass = handler.ConnView.Model.Pass;
                                    handler.ConnView.Model.Conexion.Bd = handler.ConnView.Model.BaseDatos;
                                    handler.ConnView.Model.Conexion.Puerto = handler.ConnView.Model.Puerto;
                                    handler.ConnView.Model.Conexion.Servidor = handler.ConnView.Model.Servidor;
                                    handler.ConnView.Model.Conexion.BlValor = item.BlValor;
                                    handler.ConnView.Model.Conexion.Etiqueta = item.Etiqueta;
                                }
                            }

                            handler.ConnView.Model.ListaConexiones.Add(item);
                        }
                    }
                }
            }
        }
        public static void pDocumentacionHtml(Handler handler)
        {
            string query = "select * from documentation where company ="+ handler.ConfGeneralView.Model.Empresa.Value;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DocumentacionHTML doc = new DocumentacionHTML();

                        doc.TagInicio = reader.GetString(0);
                        doc.TagFin = reader.GetString(1);
                        doc.Atributos = reader.GetString(2);
                        doc.Tipo = reader.GetString(3);
                        doc.TagEncabezadoFin = reader.GetString(4);

                        handler.ListaDocHtml.Add(doc);
                    }
                }
            }
        }
        public static void ExecuteNonQuery(string query, SQLiteConnection conn)
        {
            using (var command = new SQLiteCommand(query, conn))
            {
                command.ExecuteNonQuery();
            }
        }
        public static void pExecuteNonQuery(string query)
        {
            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                ExecuteNonQuery(query, conn);
            }
        }

        //Elimina
        public static void pEliminaPalabra(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from words where description = '" + value + "'";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pEliminaUsuario(string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from user_grants where user = '" + value + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Value;
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pEliminaUsuarioBD(string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from userbd where user = '" + value + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Value;
                ExecuteNonQuery(query, conn);
            }
        }

        public static void pEliminaTipoObjeto(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from object_type where object = '" + value + "'";
                ExecuteNonQuery(query, conn);
            }
        }

        public static void pEliminaEncabezado(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from object_head where head = '" + value + "'";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pEliminaConfHtml(string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from documentation where tag_ini = '" + value + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Value;
                ExecuteNonQuery(query, conn);
            }
        }

        public static Boolean ifExist(string table, string where, SQLiteConnection conn)
        {
            string query = "select count(1) from "+ table +" where " + where;
            int nuCantidad = 0;

            using (var command = new SQLiteCommand(query, conn))
            {
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    nuCantidad = reader.GetInt16(0);
                }
            }
            return nuCantidad >0 ? true : false; 
        }
    }
}

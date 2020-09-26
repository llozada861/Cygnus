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
        public static void pGuardarUsuario(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into user_grants (user) VALUES('" + value + "')";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarUsuarioBD(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into userbd (user) VALUES('" + value + "')";
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
        public static void pGuardarEncabezado(string sbEncabezado, string tipo, string proridad, string fin)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into object_head (head,type,priority,end) VALUES('" + sbEncabezado + "','" + tipo + "'," + proridad + ",'" + fin + "')";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarConfHtml(string inicio, string fin, string atributos, string finEnca, string tipo)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into documentation (tag_ini,tag_fin,attributes,type,end) VALUES('" + inicio + "','" + fin + "','" + atributos + "','" + tipo+ "','"+finEnca+ "')";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pActualizaRuta(string key, string path)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "update paths set path ='" + path + "' where name ='" + key + "'";
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

        public static void pActualizaHtml(string key, string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "update html set documentation ='" + value + "' where name ='" + key + "'";
                ExecuteNonQuery(query, conn);
            }
        }

        public static void pCreaConexion(Handler handler)
        {
            string query;
            string user = handler.ConnViewModel.Usuario;
            string pass = handler.ConnViewModel.Pass;
            string serv = handler.ConnViewModel.Servidor;
            string bd = handler.ConnViewModel.BaseDatos;
            string port = handler.ConnViewModel.Puerto;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from conection";
                ExecuteNonQuery(query, conn);

                query = "insert into conection (user,pass,bd,server,port) VALUES('" + user + "','" + pass + "','" + bd + "','" + serv + "','" + port + "')";
                ExecuteNonQuery(query, conn);

                /*if (ifExist("conection", "user='" + user + "'", conn))
                {
                    query = "update conection set pass ='" + pass + "',"+
                                                 "bd ='"+bd+"',"+
                                                 "server = '"+serv+"',"+
                                                 "port = '"+port+"' where user = '" + user + "'";
                    ExecuteNonQuery(query, conn);
                }
                else
                {
                    query = "insert into conection (user,pass,bd,server,port) VALUES('" + user + "','" + pass + "','"+bd+ "','" +serv+ "','" +port+ "')";
                    ExecuteNonQuery(query, conn);
                }*/
            }
        }
        public static Boolean pblValidaVersion(Handler handler)
        {
            string query = "select * from version where version_name = '"+handler.fsbVersion+"'";
            Boolean blExists = false;
            string apply = "N";

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
                return true;
            else
                return false;
        }

        public static void pCargaConfiguracion(Handler handler)
        {
            string query = "select * from configuration";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaConfiguracion.Add
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
            string query = "select * from object_type order by priority";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = reader.GetString(0);

                        if (!reader.IsDBNull(1))
                            item.Value = reader.GetString(1);
                        else
                            item.Value = "No";

                        item.CantidadSlash = reader.GetInt32(2);
                        item.Prioridad = reader.GetInt32(3);
                        item.Grant = reader.GetString(4);

                        if (!reader.IsDBNull(5))
                            item.Path = reader.GetString(5);
                        else
                            item.Path = "";

                        handler.ListaTiposObjetos.Add(item);
                    }

                    handler.ListaTiposObjetos.Add
                    (
                        new SelectListItem
                        {
                            Text = res.TipoAplica,
                            Value = res.TipoAplica,
                            CantidadSlash = 0,
                            Prioridad = 200,
                            Grant = "No"
                        }
                    );
                }
            }
        }
        public static void pListaUsGrants(Handler handler)
        {
            string query = "select * from user_grants";

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
            string query = "select * from userbd";

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
            string query = "select * from paths";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ListaRutas.Add
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
        public static void pListaHTML(Handler handler)
        {
            string query = "select * from html";

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
        public static void pDatosBd(Handler handler)
        {
            string query = "select * from conection";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        handler.ConnViewModel.Usuario = reader.GetString(0);
                        handler.ConnViewModel.Pass = reader.GetString(1);
                        handler.ConnViewModel.BaseDatos = reader.GetString(2);
                        handler.ConnViewModel.Servidor = reader.GetString(3);
                        handler.ConnViewModel.Puerto = reader.GetString(4);
                    }
                }
            }
        }
        public static void pDocumentacionHtml(Handler handler)
        {
            string query = "select * from documentation";

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
        public static void pEliminaUsuario(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from user_grants where user = '" + value + "'";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pEliminaUsuarioBD(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from userbd where user = '" + value + "'";
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
        public static void pEliminaConfHtml(string value)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from documentation where tag_ini = '" + value + "'";
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

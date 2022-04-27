using Cygnus2_0.BaseDatos.sqlite;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.General.Times;
using Cygnus2_0.Model.Azure;
using Cygnus2_0.Model.Empresa;
using Cygnus2_0.Model.Time;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;
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
                query = "insert into user_grants (user,company) VALUES('" + value + "',"+ handler.ConfGeneralView.Model.Empresa.Codigo+ ")";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarUsuarioBD(string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into userbd (user,company) VALUES('" + value + "',"+ handler.ConfGeneralView.Model.Empresa.Codigo+")";
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
                query = "insert into object_head (head,type,priority,end,company) VALUES('" + sbEncabezado + "','" + tipo + "'," + proridad + ",'" + fin + "',"+ handler.ConfGeneralView.Model.Empresa.Codigo+")";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pGuardarConfHtml(string inicio, string fin, string atributos, string finEnca, string tipo, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into documentation (tag_ini,tag_fin,attributes,type,end,company) VALUES('" + inicio + "','" + fin + "','" + atributos + "','" + tipo+ "','"+finEnca+ "',"+ handler.ConfGeneralView.Model.Empresa.Codigo+")";
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pActualizaRuta(string key, string path,int tipo_objeto, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "update object_path set path ='" + path + "' where object_type =" + tipo_objeto + " and company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;
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
                query = "update html set documentation ='" + value + "' where name ='" + key + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;
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
                    query = "insert into conection (user,pass,bd,server,port,company,active,name_) VALUES('" + user + "','" + pass + "','" + bd + "','" + serv + "','" + port + "'," + handler.ConfGeneralView.Model.Empresa.Codigo + ",'"+ defecto+"','"+etiqueta.ToUpper()+"')";
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

        public static Boolean pblValidaVersion(string version)
        {
            string query = "select * from version where version_name = '"+ version + "'";
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
                query = "insert into version (version_name,apply) values('" + version + "','N')";
                pExecuteNonQuery(query);
            }

            if (apply.Equals("Y"))
                blResultado = true;
            else
            {                
                if (blNuevo)
                {
                    //Se actualiza la versión
                    SqliteDAO.pActualizaVersion(version);
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

            if (!string.IsNullOrEmpty(handler.ConfGeneralView.Model.Empresa.Codigo))
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
            string query = "select * from user_grants where company ="+handler.ConfGeneralView.Model.Empresa.Codigo;

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
            string query = "select * from userbd where company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;

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
            string query = "select object_type,path,user_default,company from object_path where company = "+handler.ConfGeneralView.Model.Empresa.Codigo;

            if (!string.IsNullOrEmpty(handler.ConfGeneralView.Model.Empresa.Codigo))
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
            string query = "select * from html where company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;

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
        public static void pDatosBd(Handler handler, SelectListItem conexionActual)
        { 
            string query = "select * from conection where company = "+handler.ConfGeneralView.Model.Empresa.Codigo;

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
            string query = "select * from documentation where company ="+ handler.ConfGeneralView.Model.Empresa.Codigo;

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
                query = "delete from user_grants where user = '" + value + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;
                ExecuteNonQuery(query, conn);
            }
        }
        public static void pEliminaUsuarioBD(string value, Handler handler)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "delete from userbd where user = '" + value + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;
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
                query = "delete from documentation where tag_ini = '" + value + "' and company = "+ handler.ConfGeneralView.Model.Empresa.Codigo;
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

        #region Azure
        public static void pInsertaTareaAzure(TareaHoja tareaAzure, Handler handler, string accion,string desde)
        {
            string query;
            string fecha;
            DateTime fechaParseada;
            string fecha_inicio, fecha_actualiza;

            if(string.IsNullOrEmpty(handler.Azure.Usuario))
            {
                handler.MensajeError("Configure el usuario para Azure en [Ajustes/Herramientas Gestión/Azure]");
                return;
            }

            string usuario = handler.Azure.Usuario.ToUpper();
            string completado = tareaAzure.Completed.ToString(System.Globalization.CultureInfo.InvariantCulture);


            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                fecha = string.IsNullOrEmpty(tareaAzure.IniFecha) ? tareaAzure.FechaCreacion : tareaAzure.IniFecha;
                fechaParseada = DateTime.Parse(fecha);
                fecha_inicio = fechaParseada.ToString("yyyy-MM-dd");
                fecha_actualiza = DateTime.Now.ToString("yyyy-MM-dd");

                //Historia de usuario
                if (ifExist("story_user", "codigo =" + tareaAzure.HU, conn))
                {
                    query = "update story_user set descripcion ='" + tareaAzure.DescripcionHU + "' where codigo = " + tareaAzure.HU;
                    ExecuteNonQuery(query, conn);
                }
                else
                {
                    query = "insert into story_user (codigo,descripcion,usuario,empresa) "+
                            "VALUES(" + tareaAzure.HU + ",'" + tareaAzure.DescripcionHU + "','"+ usuario + "',"+ handler.ConfGeneralView.Model.Empresa.Codigo+")";
                    ExecuteNonQuery(query, conn);
                }

                //Tarea
                if (ifExist("task_user", " codigo =" + tareaAzure.IdAzure, conn))
                {
                    query = "update task_user set descripcion ='" + tareaAzure.Descripcion +"' " + 
                            ", estado = '"+tareaAzure.Estado+"' "+
                            ", completado = "+ completado +
                            ", hist_usuario = "+ tareaAzure.HU+
                            ", fecha_inicio = '"+ fecha_inicio+"' "+
                            ", fecha_actualiza = '"+fecha_actualiza +"' "+
                            " where codigo = " + tareaAzure.IdAzure;
                    ExecuteNonQuery(query, conn);
                }
                else
                {
                    query = "insert into task_user (codigo,descripcion,estado,usuario,completado,fecha_registro,hist_usuario,fecha_inicio,empresa) " +
                            "VALUES(" + tareaAzure.IdAzure + ",'" + tareaAzure.Descripcion + "','"+tareaAzure.Estado+"','" + usuario + "',"+ completado + ",'" + fecha_actualiza + "',"+ tareaAzure.HU+",'"+ fecha_inicio +"',"+ handler.ConfGeneralView.Model.Empresa.Codigo + ")";
                    ExecuteNonQuery(query, conn);

                    //Se crea el registro inicial para la hoja
                    pActualizaHorasHoja(tareaAzure, fechaParseada, handler, usuario,null);
                }

                //solo aplica para la actualización
                if(accion == "A")
                {
                    //Se crea el registro inicial para la hoja
                    pActualizaHorasHoja(tareaAzure, fechaParseada, handler, usuario,desde);
                }
            }
        }

        public static void pActualizaHorasHoja(TareaHoja tareaAzure,DateTime fechaParseada, Handler handler,string usuario,string desde)
        {
            string query;
            int codigoHoja = 0;
            string fechaFinHoja;
            int diadelasemana;
            string horas = tareaAzure.Completed.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lunes = "0", martes = "0", miercoles = "0", jueves = "0", viernes = "0", sabado = "0", domingo = "0";
            string fecha_actualiza = DateTime.Now.ToString("yyyy-MM-dd");
            string idHorasHojas = tareaAzure.Id != null ? tareaAzure.Id.ToString() : "0";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                diadelasemana = (int)fechaParseada.DayOfWeek;

                query = "select codigo, fecha_fin from week where '" + fechaParseada.ToString("yyyy-MM-dd") + "' between date(fecha_ini) and date(fecha_fin)";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        codigoHoja = Convert.ToInt32(reader.GetInt32(0));
                        fechaFinHoja = reader.GetString(1);
                    }
                }

                if(codigoHoja > 0)
                {
                    if (tareaAzure.HU > 0 && string.IsNullOrEmpty(desde))
                    {                       
                        switch (diadelasemana)
                        {
                            case 1:
                                lunes = horas;
                                break;
                            case 2:
                                martes = horas;
                                break;
                            case 3:
                                miercoles = horas;
                                break;
                            case 4:
                                jueves = horas;
                                break;
                            case 5:
                                viernes = horas;
                                break;
                            case 6:
                                sabado = horas;
                                break;
                            default:
                                domingo = horas;
                                break;
                        }
                    }
                    else
                    {
                        lunes = tareaAzure.Mon.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        martes = tareaAzure.Tue.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        miercoles = tareaAzure.Wed.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        jueves = tareaAzure.Thu.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        viernes = tareaAzure.Fri.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        sabado = tareaAzure.Sat.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        domingo = tareaAzure.Sun.Horas.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                if (ifExist("timexweek", " codigo =" + idHorasHojas + " and id_hoja ="+ codigoHoja, conn))
                {
                    query = "update timexweek set "+
                            " lunes = " + lunes +
                            ", martes = " + martes +
                            ", miercoles = " + miercoles +
                            ", jueves = " + jueves +
                            ", viernes = " + viernes +
                            ", sabado = " + sabado +
                            ", domingo = " + domingo +
                            ", fecha_actualiza = '" + fecha_actualiza +"' "+
                            " where codigo = " + tareaAzure.Id;

                    ExecuteNonQuery(query, conn);
                }
                else
                {
                    query = "insert into timexweek (id_hoja,fecha_registro,usuario,lunes,martes,miercoles,jueves,viernes,sabado,domingo,requerimiento) " +
                            "VALUES(" + codigoHoja + ",'" + fecha_actualiza + "','" + usuario + "'," + lunes + "," + martes + "," + miercoles + "," + jueves + "," + viernes + "," + sabado + ","+ domingo + "," + tareaAzure.IdAzure + ")";
                    
                    ExecuteNonQuery(query, conn);
                }
            }
        }

        public static void pObtTareasBD(TimeModel view, Handler handler)
        {
            string query;
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");
            string usuario = handler.Azure.Usuario != null ? handler.Azure.Usuario.ToUpper() : "";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "WITH qHojaActual AS "+
                        "( " +
                            "SELECT fecha_fin " +
                            "FROM week " +
                            "WHERE '"+ fechaActual+"' BETWEEN fecha_ini AND fecha_fin " +
                        ") " +
                        "SELECT " +
                               "hh.requerimiento idAzure, " +
                               "rq.descripcion, " +
                               "estado, " +
                               "rq.codigo id_rq, " +
                               "hh.codigo id, " +
                               "hh.id_hoja, " +
                               "hh.lunes, " +
                               "hh.martes, " +
                               "hh.miercoles, " +
                               "hh.jueves, " +
                               "hh.viernes, " +
                               "hh.sabado, " +
                               "hh.domingo, " +
                               "qHojaActual.fecha_fin, " +
                               "rq.completado, " +
                               "rq.hist_usuario, " +
                               "rq.fecha_inicio, " +
                               "(SELECT(sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas " +
                               "FROM timexweek hh " +
                                "WHERE requerimiento = rq.codigo) total_rq, " +
                                "(select descripcion from story_user where codigo = rq.hist_usuario) desc_hu "+
                        "FROM timexweek hh, task_user rq,qHojaActual " +
                         "WHERE hh.requerimiento = rq.codigo " +
                        "AND hh.usuario = '"+ usuario + "' " +
                        "AND hh.id_hoja = "+ view.HojaActual.Id.ToString() +
                        " ORDER BY idAzure DESC";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    view.ListaHojas.ToList().Find(x => x.Id == view.HojaActual.Id).ListaTareas.Clear();

                    while (reader.Read())
                    {
                        TareaHoja tarea = new TareaHoja();

                        tarea.Id = Convert.ToString(reader["id"]);
                        tarea.IdAzure = Convert.ToInt32(reader["idAzure"]);
                        tarea.Descripcion = Convert.ToString(reader["descripcion"]);
                        tarea.Requerimiento = Convert.ToString(reader["id_rq"]);
                        tarea.Estado = Convert.ToString(reader["estado"]);
                        tarea.IdHoja = Convert.ToInt32(reader["id_hoja"]);
                        tarea.Mon = new Day();
                        tarea.Mon.Horas = Math.Round(Convert.ToDouble(reader["lunes"]), 1);
                        tarea.Tue = new Day();
                        tarea.Tue.Horas = Math.Round(Convert.ToDouble(reader["martes"]), 1);
                        tarea.Wed = new Day();
                        tarea.Wed.Horas = Math.Round(Convert.ToDouble(reader["miercoles"]), 1);
                        tarea.Thu = new Day();
                        tarea.Thu.Horas = Math.Round(Convert.ToDouble(reader["jueves"]), 1);
                        tarea.Fri = new Day();
                        tarea.Fri.Horas = Math.Round(Convert.ToDouble(reader["viernes"]), 1);
                        tarea.Sat = new Day();
                        tarea.Sat.Horas = Math.Round(Convert.ToDouble(reader["sabado"]), 1);
                        tarea.Sun = new Day();
                        tarea.Sun.Horas = Math.Round(Convert.ToDouble(reader["domingo"]), 1);
                        tarea.Completed = Convert.ToDouble(reader["completado"]);
                        tarea.HU = Convert.ToInt32(reader["hist_usuario"]);
                        tarea.Tipo = "F";
                        tarea.IniFecha = !String.IsNullOrEmpty(Convert.ToString(reader["fecha_inicio"])) ? Convert.ToString(reader["fecha_inicio"]) : "";
                        tarea.TotalRQ = Convert.ToDouble(reader["total_rq"]);
                        tarea.DescripcionHU = Convert.ToString(reader["desc_hu"]);
                        tarea.pCalcularTotal();

                        view.ListaHojas.ToList().Find(x => x.Id == view.HojaActual.Id).ListaTareas.Add(tarea);
                    }
                }
            }
        }

        public static void pObtHojasBD(TimeModel view, Handler handler)
        {
            string query;
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");
            string fechaSiguiente = DateTime.Now.AddDays(8).ToString("yyyy-MM-dd");
            string usuario = handler.Azure.Usuario != null ? handler.Azure.Usuario.ToUpper() : "";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "SELECT * FROM ( " +
                        "SELECT codigo, " +
                               "fecha_ini, " +
                               "fecha_fin, " +
                               "descripcion,                    " +
                               "(SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo) " +
                                "FROM timexweek hh " +
                                "WHERE hh.id_hoja = h.codigo " +
                                "AND hh.usuario = '" + usuario + "' ) horas " +
                        "FROM week h " +
                        "WHERE '" + fechaSiguiente + "' BETWEEN date(fecha_ini) AND date(fecha_fin) " +
                        "UNION " +
                        "SELECT codigo, " +
                               "fecha_ini, " +
                               "fecha_fin, " +
                               "descripcion, " +
                               "(SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo) " +
                                "FROM timexweek hh " +
                                "WHERE hh.id_hoja = h.codigo " +
                                "AND hh.usuario = '" + usuario + "' ) horas " +
                        "FROM week h " +
                        "WHERE date(fecha_fin) < '" + fechaActual + "' " +
                        "OR   '" + fechaActual + "' BETWEEN date(fecha_ini) AND date(fecha_fin) " +
                        "ORDER BY fecha_fin DESC) "+
                        "order by date(fecha_fin) desc LIMIT 15";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    view.ListaHojas.Clear();

                    while (reader.Read())
                    {
                        Hoja hoja = new Hoja();

                        hoja.Id = Convert.ToInt32(reader["codigo"]);
                        hoja.FechaIni = Convert.ToDateTime(reader["fecha_ini"]);
                        hoja.FechaFin = Convert.ToDateTime(reader["fecha_fin"]);

                        if (!string.IsNullOrEmpty(reader["horas"].ToString()))
                            hoja.Horas = Math.Round(Convert.ToDouble(reader["horas"]), 1);

                        hoja.Text = hoja.FechaIni.ToShortDateString() + " - " + hoja.FechaFin.ToShortDateString() + " - [Horas: " + hoja.Horas + "]";

                        if (DateTime.Now.Date >= hoja.FechaIni && DateTime.Now.Date <= hoja.FechaFin)
                        {
                            hoja.Text = hoja.Text + "- [Semana Actual]";
                        }

                        hoja.ListaTareas = new ObservableCollection<TareaHoja>();
                        view.ListaHojas.Add(hoja);
                    }
                }
            }
        }

        public static int pObtSecuencia()
        {
            string query;
            int nuSecuencia = 0;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "insert into sequence (codigo) values (null)";

                ExecuteNonQuery(query, conn);

                query = "select seq from sqlite_sequence where name = 'sequence' ";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        nuSecuencia = reader.GetInt16(0);
                    }
                }
            }

            return -1*nuSecuencia;
        }

        public static void pEliminaTareaAzure(TareaHoja tareaAzure)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "DELETE FROM timexweek WHERE codigo =  "+ tareaAzure.Id;

                ExecuteNonQuery(query, conn);
            }
        }
        public static void pObtDetalleRq(TimeModel view, TareaHoja tarea, Handler handler)
        {
            string query;
            string usuario = handler.Azure.Usuario.ToUpper();

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "SELECT (sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas "+
                        "FROM timexweek hh "+
                        "WHERE requerimiento = "+ tarea.Requerimiento;

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        view.TotalRequerimiento = reader.GetDouble(0);
                    }
                }

                query = "SELECT completado,hist_usuario "+
                        "FROM task_user "+
                        "WHERE codigo = "+ tarea.Requerimiento;

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        view.TotalRequerimientoAzure = reader.GetDouble(0);
                        view.HU = reader.GetInt32(1);
                    }
                }

                query = "SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo) " +
                        "FROM timexweek hh " +
                        "WHERE hh.usuario = '"+ usuario+"' " +
                        "AND EXISTS(SELECT 1 " +
                                    "FROM task_user rq " +
                                    "WHERE rq.hist_usuario = "+ view.HU+" " +
                                    "AND rq.usuario = '"+ usuario+"' " +
                                    "AND rq.codigo = hh.requerimiento)";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        view.TotalHU = reader.GetDouble(0);
                    }
                }

                query = "SELECT fecha_ini fecha, lunes hora, rq.descripcion descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = "+ tarea.Requerimiento+" " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND lunes > 0 " +
                        "UNION " +
                        "SELECT date(fecha_ini, '+1 day'),martes, rq.descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = " + tarea.Requerimiento + " " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND martes > 0 " +
                        "UNION " +
                        "SELECT date(fecha_ini, '+2 day'),miercoles, rq.descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = " + tarea.Requerimiento + " " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND miercoles > 0 " +
                        "UNION " +
                        "SELECT date(fecha_ini, '+3 day'),jueves, rq.descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = " + tarea.Requerimiento + " " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND jueves > 0 " +
                        "UNION " +
                        "SELECT date(fecha_ini, '+4 day'),viernes, rq.descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = " + tarea.Requerimiento + " " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND viernes > 0 " +
                        "UNION " +
                        "SELECT date(fecha_ini, '+5 day'),sabado, rq.descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = " + tarea.Requerimiento + " " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND sabado > 0 " +
                        "UNION " +
                        "SELECT date(fecha_ini, '+6 day'),domingo, rq.descripcion " +
                        "FROM timexweek hh, week ho, task_user rq " +
                        "WHERE requerimiento = " + tarea.Requerimiento + " " +
                        "AND hh.id_hoja = ho.codigo " +
                        "AND hh.requerimiento = rq.codigo " +
                        "AND domingo > 0";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    view.ListaDetalleTarea = new ObservableCollection<Hoja>();

                    while (reader.Read())
                    {
                        Hoja hoja = new Hoja();

                        hoja.FechaIni = Convert.ToDateTime(reader["fecha"]);
                        hoja.Horas = Convert.ToDouble(reader["hora"]);
                        hoja.Text = Convert.ToString(reader["descripcion"]);
                        view.ListaDetalleTarea.Add(hoja);
                    }
                }
            }
        }

        public static void pCargarTareasPred(TimeModel view)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "select * from task_pred";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    view.ListaTareasPred.Clear();

                    while (reader.Read())
                    {
                        SelectListItem req = new SelectListItem();

                        req.Value = Convert.ToString(reader["codigo"]);
                        req.Text = Convert.ToString(reader["descripcion"]);

                        view.ListaTareasPred.Add(req);
                    }
                }
            }
        }

        public static void pObtListaAzure(Handler handler)
        {
            string query;
            string defecto;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "select * from azure where empresa = "+handler.ConfGeneralView.Model.Empresa.Codigo;

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    handler.ListaAzure.Clear();

                    while (reader.Read())
                    {
                        AzureModel azure = new AzureModel();

                        azure.Codigo = Convert.ToInt32(reader["codigo"]);
                        azure.Url = Convert.ToString(reader["url"]);
                        azure.Usuario = Convert.ToString(reader["usuario"]);
                        azure.Correo = Convert.ToString(reader["correo"]);
                        azure.Dias = Convert.ToInt32(reader["dias"]);
                        azure.Token = Convert.ToString(reader["token"]);
                        azure.Proyecto = Convert.ToString(reader["proyecto"]);
                        azure.Empresa = Convert.ToInt32(reader["empresa"]);

                        defecto = Convert.ToString(reader["defecto"]);
                        azure.Defecto = false;

                        if (defecto.Equals(res.Si))
                        {
                            azure.Defecto = true;
                            handler.Azure = new AzureModel();
                            handler.Azure.Codigo = azure.Codigo;
                            handler.Azure.Url = azure.Url;
                            handler.Azure.Usuario = azure.Usuario;
                            handler.Azure.Correo = azure.Correo;
                            handler.Azure.Dias = azure.Dias;
                            handler.Azure.Defecto = azure.Defecto;
                            handler.Azure.Token = azure.Token;
                            handler.Azure.Proyecto = azure.Proyecto;
                            handler.Azure.Empresa = azure.Empresa;
                        }

                        handler.ListaAzure.Add(azure);
                    }
                }
            }
        }

        public static void pCreaRegistroAzure(AzureModel model,string empresa)
        {
            string query;
            string defecto;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                defecto = model.Defecto ? "S" : "N";

                if (model.Codigo == 0)
                    query = "insert into azure (usuario,correo,dias,url,empresa,defecto,token,proyecto) values ('" + model.Usuario + "','" + model.Correo + "'," + model.Dias + ",'" + model.Url + "'," + empresa + ",'"+ defecto + "','"+model.Token+"','"+model.Proyecto+"')";
                else
                    query = "update azure set " +
                            "usuario = '" + model.Usuario + "' " +
                            ", correo = '" + model.Correo + "' " +
                            ", dias = " + model.Dias + " " +
                            ", url = '" + model.Url + "' " +
                            ", empresa = " + model.Empresa + " " +
                            ", defecto = '"+defecto + "' "+
                            ", token = '"+model.Token + "' "+
                            ", proyecto = '" + model.Proyecto + "' " +
                            "where codigo = " + model.Codigo;

                ExecuteNonQuery(query, conn);
            }
        }

        public static int pContarSemanas()
        {
            string query;
            int nuSecuencia = 0;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query = "select count(1) from week";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        nuSecuencia = reader.GetInt16(0);
                    }
                }
            }

            return nuSecuencia;
        }

        public static ObservableCollection<SelectListItem> pObtListaHUAzure(Handler handler)
        {
            ObservableCollection<SelectListItem> listaTareas = new ObservableCollection<SelectListItem>();

            string query;
            string usuario = handler.Azure.Usuario != null ? handler.Azure.Usuario.ToUpper() : "";

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                query =     "    SELECT * FROM " +
                            "    (" +
                            "        SELECT codigo,descripcion" +
                            "        FROM story_user " +
                            "        WHERE usuario = '" + usuario + "'"+
                            "        AND descripcion IS NOT  NULL" +
                            "    )" +
                            "    ORDER BY codigo DESC";

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read()) // loop through each row from oracle
                    {
                        listaTareas.Add(new SelectListItem
                        {
                            Text = reader["codigo"].ToString() + " - " + reader["descripcion"].ToString(),
                            Value = reader["codigo"].ToString()
                        });
                    }
                }
            }

            return listaTareas;
        }
        #endregion Azure

        #region Empresa
        public static bool pExisteEmpresa(string value)
        {
            string query;
            int valor = 0;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                string codigo = value == null ? "0" : value;
                query = " SELECT codigo FROM company where codigo = " + codigo;

                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read()) 
                    {
                        valor = Convert.ToInt32(reader["codigo"]);
                    }
                }
            }

            return valor > 0 ? true:false;
        }

        public static void pInsertaEmpresa(EmpresaModel empresa)
        {
            string query;
            string defecto;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                defecto = empresa.Azure;

                query = "insert into company (codigo,descripcion,azure,git,sonar,defecto) values (" + empresa.Codigo + ",'" + empresa.Descripcion + "','" + empresa.Azure + "','"+ empresa.Git + "','"  + empresa.Sonar + "','" + empresa.Defecto + "')";

                ExecuteNonQuery(query, conn);
            }
        }
        public static void pEliminaEmpresa(EmpresaModel empresa)
        {
            string query;
            string defecto;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                defecto = empresa.Azure;

                query = "delete from company where codigo = " + empresa.Codigo;

                ExecuteNonQuery(query, conn);
            }
        }
        public static void pActualizaEmpresa(EmpresaModel empresa)
        {
            string query;

            using (SQLiteConnection conn = DbContext.GetInstance())
            {

                query = "update company set "+ 
                        "descripcion = '" + empresa.Descripcion + "'"+
                        ",azure = '" + empresa.Azure + "'" +
                        ",git = '" +empresa.Git+ "'" +
                        ",sonar = '" + empresa.Sonar + "'" +
                        ",defecto = '" + empresa.Defecto + "'" +
                        " where codigo = " + empresa.Codigo;

                ExecuteNonQuery(query, conn);
            }
        }
        public static void pListaEmpresas(Handler handler)
        {
            string query = "select codigo, descripcion, azure,git,sonar,defecto from company";
            handler.ConfGeneralView.Model.ListaEmpresas.Clear();

            using (SQLiteConnection conn = DbContext.GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        EmpresaModel item = new EmpresaModel();
                        item.Codigo = reader.GetInt32(0).ToString();
                        item.Descripcion = reader.GetString(1);
                        item.Azure = res.No;
                        item.Git = res.No;
                        item.Sonar = res.No;
                        item.Defecto = "";

                        if (!reader.IsDBNull(2))
                            item.Azure = reader.GetString(2);

                        if (!reader.IsDBNull(3))
                            item.Git = reader.GetString(3);                            

                        if (!reader.IsDBNull(4))
                            item.Sonar = reader.GetString(4);

                        if (!reader.IsDBNull(5))
                            item.Defecto = reader.GetString(5);

                        if(item.Defecto.Equals(res.YES))
                        {
                            handler.ConfGeneralView.Model.Empresa = item;
                        }

                        handler.ConfGeneralView.Model.ListaEmpresas.Add(item);
                    }
                }
            }
        }
        #endregion Empresa
    }
}

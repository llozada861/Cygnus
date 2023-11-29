using Cygnus2_0.BaseDatos.sqlite;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.General.Times;
using Cygnus2_0.Model.Azure;
using Cygnus2_0.Model.Conexion;
using Cygnus2_0.Model.Empresa;
using Cygnus2_0.Model.History;
using Cygnus2_0.Model.Html;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.Permisos;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Model.Time;
using Cygnus2_0.Model.User;
using Cygnus2_0.Model.Version;
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
        #region Palabras Reservadas
        public static void pGuardarPalabra(string value)
        {
            PalabrasClaves palabra;

            using (DataBaseContext context = new DataBaseContext())
            {
                palabra = new PalabrasClaves() { Palabra = value };

                if(pExistePalabra(palabra))
                {
                    palabra = context.PalabrasReservadas.Where(x => x.Palabra.Equals(value)).First();
                    palabra.Palabra = value;
                    context.Entry(palabra).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    palabra = new PalabrasClaves();
                    palabra.Palabra = value;
                    context.PalabrasReservadas.Add(palabra);
                    context.SaveChanges();
                }
            }
        }
        public static void pEliminaPalabra(string value)
        {
            PalabrasClaves objeto = new PalabrasClaves();
            objeto.Palabra = value;

            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pListaPalabrasReservadas(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaPalabrasReservadas = new ObservableCollection<PalabrasClaves>(context.PalabrasReservadas.ToList());
            }
        }
        public static bool pExistePalabra(PalabrasClaves palabra)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.PalabrasReservadas.FirstOrDefault(x => x.Palabra.Equals(palabra.Palabra));

                if(objeto != null)
                    existe = true;
            }

            return existe;
        }
        #endregion Palabras Reservadas 

        #region Grants
        public static void pGuardarUsuarioGrant(GrantsModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.UsuariosGrant.Add(objeto);
                context.SaveChanges();
            }
        }
        public static bool pExisteUsGrant(GrantsModel item)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.UsuariosGrant.FirstOrDefault(x => x.Codigo == item.Codigo);

                if (objeto != null)
                    existe = true;
            }

            return existe;
        }
        public static void pGuardarUsuarioBD(UsuarioModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Usuarios.Add(objeto);
                context.SaveChanges();                
            }
        }
        public static void pEliminaUsuario(string value, Handler handler)
        {
            GrantsModel objeto = new GrantsModel();
            objeto.Usuario = value;
            objeto.Empresa = handler.ConfGeneralView.Model.Empresa.Codigo;

            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pListaUsGrants(Handler handler)
        {
            if(handler.ListaUsGrants != null)
                handler.ListaUsGrants.Clear();

            if(handler.ListaPermisos != null)
                handler.ListaPermisos.Clear();

            using (DataBaseContext context = new DataBaseContext())
            {
                List<GrantsModel> lista = context.UsuariosGrant.Where(x => x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).ToList();

                foreach (GrantsModel objeto in lista)
                {
                    SelectListItem item = new SelectListItem() { Text = objeto.Usuario, Value = objeto.Codigo.ToString(), Empresa = objeto.Empresa};
                    handler.ListaUsGrants.Add(item);
                }

                handler.ListaPermisos = new ObservableCollection<PermisosModel>(context.ListaPermisos.ToList());
            }
        }
        public static void pGuardarPermiso(PermisosModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.ListaPermisos.Add(objeto);
                context.SaveChanges();
            }
        }
        #endregion Grants

        #region Permisos Usuarios
        public static void pAdicionaPermiso(PermisosObjeto objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.ListaPermisosObjeto.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pGuardarPermisoObjeto(PermisosObjeto objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.ListaPermisosObjeto.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pListaPermisosObjetos(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaPermisosObjeto = new ObservableCollection<PermisosObjeto>(context.ListaPermisosObjeto.ToList());
            }
        }
        #endregion Permisos Usuarios

        #region Tipo Objetos
        public static void pGuardarTipoObjeto(TipoObjetos objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.TipoObjetos.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pListaTiposObjetos(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaTiposObjetos = new ObservableCollection<TipoObjetos>(context.TipoObjetos.OrderBy(x => x.Descripcion).ToList());
            }
        }

        public static void pEliminaTipoObjeto(TipoObjetos objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pActualizaTipoObjeto(TipoObjetos objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        #endregion Tipo Objetos

        #region Ruta Objetos
        public static void pAdicionaRuta(RutaObjetos objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.RutaObjetos.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pActualizaRuta(RutaObjetos objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static void pListaRutas(Handler handler)
        {
            if (handler.ConfGeneralView.Model.Empresa.Codigo != null)
            {
                using (DataBaseContext context = new DataBaseContext())
                {
                    handler.ListaRutas = new ObservableCollection<RutaObjetos>(context.RutaObjetos.Where(x => x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).ToList());
                }
            }
        }
        public static void pEliminaRuta(RutaObjetos objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        #endregion Ruta Objetos

        #region Encabezados
        public static void pGuardarEncabezado(HeadModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.EncabezadosObjetos.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pActualizaEncabezado(HeadModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static void pEliminaEncabezado(HeadModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pListaEncabezadoObjetos(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaEncabezadoObjetos = new ObservableCollection<HeadModel>(context.EncabezadosObjetos.ToList());
            }
        }
        #endregion Encabezados

        #region Documentacion
        public static void pGuardarConfHtml(DocumentacionHTML objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {                
                context.Documentacion.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pEliminaConfHtml(string value, Handler handler)
        {
            DocumentacionHTML objeto = new DocumentacionHTML() { TagIni = value,Empresa= handler.ConfGeneralView.Model.Empresa.Codigo };

            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pDocumentacionHtml(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaDocHtml = new ObservableCollection<DocumentacionHTML>(context.Documentacion.Where(x=>x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).ToList());
            }
        }
        #endregion Documentacion

        #region Plantillas
        public static void pInsertaPlantilla(PlantillasHTMLModel objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.PlantillasHTML.Add(objeto);
                context.SaveChanges();
            }
        }
        public static void pListaHTML(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaHTML = new ObservableCollection<PlantillasHTMLModel>(context.PlantillasHTML.Where(x=>x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).ToList());
            }
        }
        #endregion Plantillas

        #region Version
        public static void pActualizaVersion(string version)
        {
            VersionBD version_;

            using (DataBaseContext context = new DataBaseContext())
            {
                version_ = context.Versiones.FirstOrDefault(x => x.Version.Equals(version));

                if (version_ == null)
                {
                    version_ = new VersionBD();
                    version_.Version = version;
                    version_.Aplicada = "Y";
                    version_.Empresa = 99;
                    context.Versiones.Add(version_);
                    context.SaveChanges();
                }
                else
                {
                    version_.Aplicada = "Y";
                    context.Entry(version_).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public static Boolean pblValidaVersion(string version)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.Versiones.FirstOrDefault(x => x.Version.Equals(version) && x.Aplicada.Equals("Y"));

                if (objeto != null)
                    existe = true;
            }

            return existe;
        }

        public static Boolean pEsNueva()
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                int cantidad = context.Versiones.Count();

                if (cantidad == 0)
                    existe = true;
            }

            return existe;
        }
        #endregion Version

        #region Conexion
        public static bool pExisteConexion(string etiqueta)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.Conexiones.FirstOrDefault(x => x.Nombre.Equals(etiqueta));

                if (objeto != null)
                    existe = true;
            }

            return existe;
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

            ConnModel objeto;

            using (DataBaseContext context = new DataBaseContext())
            {
                objeto = new ConnModel() { Nombre = etiqueta };

                if (pExisteConexion(etiqueta))
                {
                    objeto = context.Conexiones.Where(x => x.Nombre.Equals(etiqueta)).First();
                    objeto.Usuario = user;
                    objeto.BaseDatos = bd;
                    objeto.Servidor = serv;
                    objeto.Puerto = port;
                    objeto.Pass = pass;
                    objeto.Activo = defecto;
                    context.Entry(objeto).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    objeto = new ConnModel() {Usuario=user,BaseDatos=bd,Servidor=serv,Pass=pass,Nombre=etiqueta,Puerto=port,Activo=defecto,Empresa= handler.ConfGeneralView.Model.Empresa.Codigo };
                    context.Conexiones.Add(objeto);
                    context.SaveChanges();
                }
            }
        }

        internal static void pEliminarConexion(SelectListItem conexion)
        {
            ConnModel objeto = new ConnModel() {Nombre= conexion.Etiqueta };

            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pDatosBd(Handler handler, SelectListItem conexionActual)
        {
            handler.ConnView.Model.ListaConexiones.Clear();
            handler.ConnView.Model.Conexion = new SelectListItem();

            using (DataBaseContext context = new DataBaseContext())
            {
                List<ConnModel> lista = context.Conexiones.Where(x=>x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).ToList();

                foreach (ConnModel objeto in lista)
                {
                    SelectListItem item = new SelectListItem();
                    item.Usuario = objeto.Usuario;

                    if (!item.Usuario.ToLower().Equals("sql_usuario"))
                    {
                        item.Pass = objeto.Pass;
                        item.Bd = objeto.BaseDatos;
                        item.Servidor = objeto.Servidor;
                        item.Puerto = objeto.Puerto;
                        item.Activo = objeto.Activo;
                        item.Etiqueta = objeto.Nombre;
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
        #endregion Conexion

        #region Usuarios BD
        public static bool pExisteUsuario(UsuarioModel item)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.Usuarios.FirstOrDefault(x => x.Usuariobd.Equals(item.Usuariobd));

                if (objeto != null)
                    existe = true;
            }

            return existe;
        }
        public static void pListaUsuarios(Handler handler)
        {
            handler.ListaUsuarios.Clear();

            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ListaUsuarios = new ObservableCollection<UsuarioModel>(context.Usuarios.Where(x => x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).ToList());
            }
        }
        
        public static void pEliminaUsuarioBD(string value, Handler handler)
        {
            UsuarioModel objeto = new UsuarioModel() { Usuariobd=value };

            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        #endregion Usuarios BD

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


            using (SQLiteConnection conn = DataBaseContext.GetInstance())
            {
                fecha = string.IsNullOrEmpty(tareaAzure.IniFecha) ? tareaAzure.FechaCreacion : tareaAzure.IniFecha;
                fechaParseada = DateTime.Parse(fecha);
                fecha_inicio = fechaParseada.ToString("yyyy-MM-dd");
                fecha_actualiza = DateTime.Now.ToString("yyyy-MM-dd");

                //Historia de usuario
                if (ifExist("story_user", "codigo =" + tareaAzure.HU, conn))
                {
                    query = "update story_user set descripcion ='" + tareaAzure.DescripcionHU.Replace("'", "''") + "' where codigo = " + tareaAzure.HU;
                    ExecuteNonQuery(query, conn);
                }
                else
                {
                    query = "insert into story_user (codigo,descripcion,usuario,empresa) "+
                            "VALUES(" + tareaAzure.HU + ",'" + tareaAzure.DescripcionHU.Replace("'", "''") + "','"+ usuario + "',"+ handler.ConfGeneralView.Model.Empresa.Codigo+")";
                    ExecuteNonQuery(query, conn);
                }

                //Tarea
                if (ifExist("task_user", " codigo =" + tareaAzure.IdAzure, conn))
                {
                    query = "update task_user set descripcion ='" + tareaAzure.Descripcion.Replace("'", "''") +"' " + 
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
                            "VALUES(" + tareaAzure.IdAzure + ",'" + tareaAzure.Descripcion.Replace("'", "''") + "','"+tareaAzure.Estado+"','" + usuario + "',"+ completado + ",'" + fecha_actualiza + "',"+ tareaAzure.HU+",'"+ fecha_inicio +"',"+ handler.ConfGeneralView.Model.Empresa.Codigo + ")";
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
            {
                query = "DELETE FROM timexweek WHERE codigo =  "+ tareaAzure.Id;

                ExecuteNonQuery(query, conn);
            }

            try
            {

                using (SQLiteConnection conn = DataBaseContext.GetInstance())
                {
                    query = "DELETE FROM task_user WHERE codigo =  " + tareaAzure.IdAzure;

                    ExecuteNonQuery(query, conn);
                }
            }
            catch (Exception ex) { }
        }
        public static void pObtDetalleRq(TimeModel view, TareaHoja tarea, Handler handler)
        {
            string query;
            string usuario = handler.Azure.Usuario.ToUpper();

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

        public static void pCreaRegistroAzure(AzureModel model,int? empresa)
        {
            string query;
            string defecto;

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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

            using (SQLiteConnection conn = DataBaseContext.GetInstance())
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
        public static bool pExisteEmpresa(EmpresaModel empresa)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.Empresas.FirstOrDefault(x => x.Codigo == empresa.Codigo);

                if (objeto != null)
                    existe = true;
            }

            return existe;
        }

        public static void pInsertaEmpresa(EmpresaModel empresa)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Empresas.Add(empresa);
                context.SaveChanges();
            }
        }
        public static void pEliminaEmpresa(EmpresaModel empresa)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(empresa).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pActualizaEmpresa(EmpresaModel empresa)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(empresa).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void pListaEmpresas(Handler handler)
        {
            handler.ConfGeneralView.Model.ListaEmpresas.Clear();

            using (DataBaseContext context = new DataBaseContext())
            {
                handler.ConfGeneralView.Model.ListaEmpresas = new ObservableCollection<EmpresaModel>(context.Empresas.ToList());
            }
        }
        #endregion Empresa

        #region repositorios
        public static void pEditaRepositorio(Repositorio repo)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(repo).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static void pCreaRepositorio(Repositorio repo)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Repositorios.Add(repo);
                context.SaveChanges();
            }
        }
        public static void pEditaRamRepositorio(RamaRepositorio rama)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(rama).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static void pCreaRamaRepositorio(RamaRepositorio rama)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.RamaRepositorios.Add(rama);
                context.SaveChanges();
            }
        }
        public static ObservableCollection<Repositorio> pListaRepositorios()
        {
            ObservableCollection<Repositorio> repositorios;

            using (DataBaseContext context = new DataBaseContext())
            {
                repositorios = new ObservableCollection<Repositorio>(context.Repositorios.ToList());
            }

            return repositorios;
        }
        public static ObservableCollection<RamaRepositorio> pListaRamaRepositorios(Repositorio repo)
        {
            ObservableCollection<RamaRepositorio> repositoriosRama = new ObservableCollection<RamaRepositorio>();

            if(repo != null)
            {
                using (DataBaseContext context = new DataBaseContext())
                {
                    repositoriosRama = new ObservableCollection<RamaRepositorio>(context.RamaRepositorios.Where(x => x.RepositorioId == repo.Codigo).ToList());
                }
            }

            return repositoriosRama;
        }

        public static void pEliminaRepo(Repositorio repo)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(repo).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pEliminaRamaRepo(RamaRepositorio rama)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(rama).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        #endregion repositorios

        #region Configuraciones
        public static string pObtValorConfiguracion(string llave)
        {
            Configuracion valor;

            using (DataBaseContext context = new DataBaseContext())
            {
                valor = context.Configuraciones.Where(x => x.Key == llave).First();
            }            

            return valor.Valor;
        }

        public static void pCreaConfiguracion(string key, string value)
        {
            Configuracion conf;

            using (DataBaseContext context = new DataBaseContext())
            {
                try
                {
                    conf = context.Configuraciones.Where(x => x.Key.Equals(key)).First();
                    conf.Valor = value;
                    context.Entry(conf).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    conf = new Configuracion();
                    conf.Key = key;
                    conf.Valor = value;
                    context.Configuraciones.Add(conf);
                    context.SaveChanges();
                }
            }
        }
        public static void pCargaConfiguracion(Handler handler)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                List<Configuracion> lista = context.Configuraciones.ToList();

                foreach (Configuracion objeto in lista)
                {
                    SelectListItem item = new SelectListItem() { Value = objeto.Valor, Text = objeto.Key };
                    handler.ListaConfiguracion.Add(item);
                }
            }
        }
        #endregion Configuraciones

        #region Historia
        public static void pCreaAplicaHistoria(AplicaHistoriaModel item)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.AplicaHistoria.Add(item);
                context.SaveChanges();
            }
        }

        public static ObservableCollection<Archivo> pListaAplicaHistoria(string caso)
        {
            ObservableCollection<AplicaHistoriaModel> aplicaHistorias;
            ObservableCollection<Archivo> aplicaArchivos = new ObservableCollection<Archivo>();

            using (DataBaseContext context = new DataBaseContext())
            {
                aplicaHistorias = new ObservableCollection<AplicaHistoriaModel>(context.AplicaHistoria.Where(x=> x.Caso.Equals(caso)).ToList());
            }

            int index = 1;

            foreach (AplicaHistoriaModel archivo in aplicaHistorias.OrderBy(x=>x.Tipo))
            {
                aplicaArchivos.Add(new Archivo { FileName = archivo.Archivo, Owner = archivo.Usuario, Tipo = archivo.Tipo, OrdenCambio = archivo.Caso, Ruta = archivo.Ruta, Index =  index, Codigo = archivo.Codigo});
                index++;
            }

            return aplicaArchivos;
        }

        public static void pCreaHistoria(HistoriaModel repo)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Historia.Add(repo);
                context.SaveChanges();
            }
        }

        public static ObservableCollection<HistoriaModel> pListaHistoria()
        {
            ObservableCollection<HistoriaModel> historias;

            using (DataBaseContext context = new DataBaseContext())
            {
                historias = new ObservableCollection<HistoriaModel>(context.Historia.ToList());
            }

            return historias;
        }

        public static bool pExisteAplHist(AplicaHistoriaModel item)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.AplicaHistoria.FirstOrDefault(x => x.Caso.ToUpper().Equals(item.Caso.ToUpper()) && x.Archivo.ToUpper().Equals(item.Archivo.ToUpper()));

                if (objeto != null)
                    existe = true;
            }

            return existe;
        }
        public static bool pExisteHistoria(string caso)
        {
            bool existe = false;

            using (DataBaseContext context = new DataBaseContext())
            {
                var objeto = context.Historia.FirstOrDefault(x => x.Historia.ToUpper().Equals(caso.ToUpper()));

                if (objeto != null)
                    existe = true;
            }

            return existe;
        }
        #endregion Historia

        #region Genericos
        public static void pEliminaObjeto(object objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public static void pActualizaObjeto(object objeto)
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Entry(objeto).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static List<UsuariosPDN> pObtListaBD()
        {
            List<UsuariosPDN> lista = new List<UsuariosPDN>();

            using (DataBaseContext context = new DataBaseContext())
            {
                lista = context.ListaUsuariosPDN.ToList();
            }

            return lista;
        }
        #endregion Genericos

        #region Ejecuta SQL
        public static void ExecuteNonQuery(string query, SQLiteConnection conn)
        {
            using (var command = new SQLiteCommand(query, conn))
            {
                command.ExecuteNonQuery();
            }
        }
        public static void pExecuteNonQuery(string query)
        {
            using (SQLiteConnection conn = DataBaseContext.GetInstance())
            {
                ExecuteNonQuery(query, conn);
            }
        }
        public static Boolean ifExist(string table, string where, SQLiteConnection conn)
        {
            string query = "select count(1) from " + table + " where " + where;
            int nuCantidad = 0;

            using (var command = new SQLiteCommand(query, conn))
            {
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    nuCantidad = reader.GetInt16(0);
                }
            }
            return nuCantidad > 0 ? true : false;
        }
        #endregion Ejecuta SQL
    }
}

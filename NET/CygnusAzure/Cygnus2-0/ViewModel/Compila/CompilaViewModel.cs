using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Compila;
using Cygnus2_0.Model.Git;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Compila
{
    public class CompilaViewModel: IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conectar;
        private readonly DelegateCommand _examinar;

        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conectar;
        public ICommand Examinar => _examinar;
        public CompilaViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conectar = new DelegateCommand(OnConection);
            _examinar = new DelegateCommand(pExaminar);

            handler = hand;
            this.Model = new CompilaModel(handler);

            this.Model.ListaObservaciones = new ObservableCollection<SelectListItem>();
            this.Model.ListaArchivosCargados = new ObservableCollection<Archivo>();
            this.Model.ListaUsuarios = handler.ListaUsuarios;

            try
            {
                this.Model.ArchivosCompilados = pObtCantObjsInvalidos();
                this.Model.ArchivosDescompilados = this.Model.ArchivosCompilados;
                this.Model.ListaHU = handler.DAO.pObtListaHUAzure();
            }
            catch(Exception ex)
            {
                handler.MensajeError(res.MensajeNoConexion + ". [" + ex.Message + "]");
                this.Model.ArchivosCompilados = "0";
                this.Model.ArchivosDescompilados = "0";
            }
        }
        public CompilaModel Model { get; set; }

        public void pCompilar()
        {
            pCompilarObjetos();
        }
        public void OnProcess(object commandParameter)
        {            
        }
        public void OnClean(object commandParameter)
        {
            try
            {
                this.Model.ListaArchivosCargados.Clear();
                this.Model.ListaObservaciones.Clear();
                this.Model.ArchivosCompilados = pObtCantObjsInvalidos();
                this.Model.ArchivosDescompilados = "0";
                this.Model.ListaUsuarios = null;
                this.Model.ListaUsuarios = handler.ListaUsuarios;
                this.Model.EstadoConn = "1";
                this.Model.ListaHU = null;
                this.Model.ListaHU = handler.DAO.pObtListaHUAzure();
                this.Model.Comentario = "";
                this.Model.Codigo = "";
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnConection(object commandParameter)
        {
            try
            {
                //se intenta realizar la conexión con la base de datos
                handler.pRealizaConexion();
                this.Model.ArchivosCompilados = pObtCantObjsInvalidos();
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            this.Model.EstadoConn = handler.fsbValidaConexion();
        }
        public void pExaminar(object commandParameter)
        {
            string[] archivos = handler.pCargarArchivos();
            pListaArchivos(archivos,"");
        }

        public void pListaArchivos(string[] DropPath,string from)
        {
            try
            {
                foreach (string dropfilepath in DropPath)
                {
                    if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dropfilepath)))
                    {
                        string[] DropPath1 = System.IO.Directory.GetFiles(dropfilepath + "\\", "*", System.IO.SearchOption.AllDirectories);
                        //pListaArchivosCarpeta(dropfilepath, from);
                        pListaArchivos(DropPath1, from);
                    }
                    else
                    {
                        Archivo archivo = new Archivo();
                        archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                        archivo.RutaConArchivo = dropfilepath;
                        archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                        archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                        archivo.Extension = System.IO.Path.GetExtension(dropfilepath);
                        archivo.ListaTipos = handler.ListaTiposObjetos;
                        ObtenerTipoArchivoComp(archivo);
                        archivo.BloquesCodigo = new List<string>();

                        if (!archivo.Observacion.Equals(res.No_aplica))
                            archivo.TipoAplicacion = res.SQLPLUS;

                        this.Model.ListaArchivosCargados.Add(archivo);
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        private void pListaArchivosCarpeta(string path, string from)
        {
            string[] DropPath = System.IO.Directory.GetFiles(path + "\\", "*", System.IO.SearchOption.AllDirectories);

            if (from.Equals("G"))
            {
                foreach (string dropfilepath in DropPath)
                {
                    Archivo archivo = new Archivo();
                    archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                    archivo.RutaConArchivo = dropfilepath;
                    archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                    archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                    archivo.Extension = System.IO.Path.GetExtension(dropfilepath);
                    archivo.ListaTipos = handler.ListaTiposObjetos;
                    handler.ObtenerTipoArchivo(archivo, res.No_aplica);
                    this.Model.ListaArchivosCargados.Add(archivo);
                }
            }
        }

        public void ObtenerTipoArchivoComp(Archivo archivo)
        {
            string sbLine = "";
            bool existeOn;
            string sbLineSpace = "";
            Int64 nuMenosUno = Convert.ToInt64(res.MenosUno);

            string nombreArchivo = archivo.NombreSinExt;
            archivo.Observacion = "";

            if (res.Extensiones.IndexOf(archivo.Extension.ToLower()) > -1)
            {
                using (StreamReader streamReader = new StreamReader(archivo.RutaConArchivo))
                {
                    sbLine = streamReader.ReadLine();

                    while (sbLine != null)
                    {
                        sbLineSpace = Regex.Replace(sbLine, @"\s+", " ");

                        if (sbLineSpace.StartsWith(res.Arroa))
                        {
                            archivo.Tipo = res.TipoAplica;
                            archivo.Observacion = res.No_aplica;
                            break;
                        }

                        archivo.TipoAplicacion = res.SQLPLUS;

                        foreach (SelectListItem prefijo in handler.ListaEncabezadoObjetos.OrderBy(x => x.Prioridad))
                        {
                            if (sbLineSpace.ToLower().IndexOf(prefijo.Text.ToLower()) > nuMenosUno)
                            {
                                archivo.Tipo = prefijo.Value;
                                archivo.SelectItemTipo = prefijo;
                                archivo.NombreObjeto = handler.pObtenerNombreObjeto(sbLineSpace, out existeOn);
                                archivo.FinArchivo = prefijo.Fin.Equals(res.PuntoYComa) ? ";" : prefijo.Fin;
                                archivo.TipoAplicacion = archivo.FinArchivo.Equals(res.END) ? res.SQL : res.SQLPLUS;
                                archivo.InicioArchivo = prefijo.Text.ToLower();
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(archivo.Tipo))
                        {
                            sbLine = streamReader.ReadLine();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            //Sino se encuentra el tipo dentro del archivo
            if (string.IsNullOrEmpty(archivo.Tipo))
            {
                /*foreach (SelectListItem prefijo in handler.ListaTipoArchivos)
                {
                    if (archivo.FileName.ToLower().IndexOf(prefijo.Text.ToLower()) > nuMenosUno)
                    {
                        archivo.Tipo = prefijo.Value;
                        handler.SavePath = archivo.Ruta + "\\";
                        break;
                    }
                }*/

                if (string.IsNullOrEmpty(archivo.NombreObjeto))
                {
                    archivo.NombreObjeto = nombreArchivo;
                }
            }
        }

        public string pObtCantObjsInvalidos()
        {
            return handler.DAO.pObtCantObjsInvalidos();
        }

        public void pCompilarObjetos()
        {
            handler.pObtenerUsuarioCompilacion(this.Model.Usuario.Text);

            foreach (Archivo archivo in this.Model.ListaArchivosCargados.ToList().Where(x => x.TipoAplicacion.Equals(res.SQL)))
            {
                if (archivo.TipoAplicacion != res.SQLPLUS)
                {
                    handler.pObtieneBloquesCodigo(archivo);
                    pCompilaObjetosBD(archivo);
                    handler.pGeneraArchivosPermisosArchivo(archivo, this.Model.Usuario.Text);
                }
            }

            if (handler.ConexionOracle.ConexionOracleCompila.State == System.Data.ConnectionState.Open)
            {
                handler.ConexionOracle.ConexionOracleCompila.Close();
            }

            pExeSqlplus();

            this.Model.ArchivosDescompilados = pObtCantObjsInvalidos();

            if (Convert.ToInt64(this.Model.ArchivosCompilados) < Convert.ToInt64(this.Model.ArchivosDescompilados))
            {
                throw new Exception("Nuevos Objetos Descompilados. Antes [" + this.Model.ArchivosCompilados + "]. Ahora [" + this.Model.ArchivosDescompilados + "]. Por favor revisar lo que se aplicó!");
            }
        }

        public void pCompilaObjetosBD(Archivo archivo)
        {
            if (!string.IsNullOrEmpty(archivo.NombreObjeto))
            {
                //Valida que el objeto no se encuentra aplicado en más de un esquema
                handler.DAO.pValidaUsuarioCompila(archivo, handler);
                //Valida que solo se aplique en un esquema
                handler.DAO.pValidaObjEsquema(archivo, this.Model.Usuario.Text);
            }

            this.Model.ArchivosCompilados = handler.DAO.pObtCantObjsInvalidos();

            //Se guarda el estado de los objetos compilados antes de la ejecución
            //dao.pGuardaLogCompilacion(archivo, Environment.MachineName, Environment.UserName, stObjetosInvalidosAntes, res.Antes, handler.DatosConexion.UsuarioCompila);
            handler.pGuardaLogCompilacion(archivo, this.Model.ArchivosCompilados, res.Antes);

            for (int i = archivo.BloquesCodigo.Count - 1; i >= 0; i--)
            {
                //Console.WriteLine(archivo.BloquesCodigo.ElementAt(i));
                handler.DAO.pEjecutarScriptBD(archivo.BloquesCodigo.ElementAt(i));
            }

            this.Model.ArchivosDescompilados = handler.DAO.pObtCantObjsInvalidos();

            //dao.pGuardaLogCompilacion(archivo, Environment.MachineName, Environment.UserName, stObjetosInvalidosDespues, res.Despues, handler.DatosConexion.UsuarioCompila);
            handler.pGuardaLogCompilacion(archivo, this.Model.ArchivosDescompilados, res.Despues);
            pObtieneErroresAplicacion(archivo);
        }

        public void pObtieneErroresAplicacion(Archivo archivo)
        {
            handler.DAO.pObtErrores(archivo, Model);

            if (!this.Model.ListaObservaciones.ToList().Exists(x => x.Text.Equals(archivo.NombreObjeto)) && string.IsNullOrEmpty(archivo.AplicaTemporal))
                this.Model.ListaObservaciones.Add(new SelectListItem() { Text = archivo.NombreObjeto, Value = "Sin Errores.", Observacion = archivo.Ruta });
            else
                if (!string.IsNullOrEmpty(archivo.AplicaTemporal))
                this.Model.ListaObservaciones.Add(new SelectListItem() { Text = archivo.AplicaTemporal, Value = "Ver log de la aplicación.", Observacion = archivo.Ruta });

            this.Model.ListaObservaciones = this.Model.ListaObservaciones;
        }

        public void pExeSqlplus()
        {
            string credenciales;

            if (this.Model.ListaArchivosCargados.Count > 0)
            {
                handler.pObtenerUsuarioCompilacion(this.Model.Usuario.Text);
                credenciales = handler.ConnViewModel.UsuarioCompila + "/" + handler.ConnViewModel.PassCompila + "@" + handler.ConnViewModel.BaseDatos;

                foreach (Archivo archivo in this.Model.ListaArchivosCargados.ToList().Where(x => x.TipoAplicacion.Equals(res.SQLPLUS)))
                {
                    if (!string.IsNullOrEmpty(archivo.NombreObjeto))
                    {
                        //Valida que el objeto no se encuentra aplicado en más de un esquema
                        handler.DAO.pValidaUsuarioCompila(archivo, handler);
                        //Valida que solo se aplique en un esquema
                        handler.DAO.pValidaObjEsquema(archivo, this.Model.Usuario.Text);
                    }

                    handler.pGuardaLogCompilacion(archivo, this.Model.ArchivosCompilados, res.Antes);
                    handler.DAO.pExecuteSqlplus(credenciales, archivo);
                }
            }

            Thread.Sleep(3000);

            foreach (Archivo archivo in this.Model.ListaArchivosCargados.ToList().Where(x => x.TipoAplicacion.Equals(res.SQLPLUS)))
            {
                try
                {
                    pObtieneErroresAplicacion(archivo);
                    handler.pGuardaLogCompilacion(archivo, this.Model.ArchivosDescompilados, res.Despues);
                    handler.pGeneraArchivosPermisosArchivo(archivo, this.Model.Usuario.Text);
                }
                catch { }
            }
        }

    }
}

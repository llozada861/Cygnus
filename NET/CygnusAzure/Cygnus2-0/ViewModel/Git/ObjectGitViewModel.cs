using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Git;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Pages.General;
using Cygnus2_0.Pages.Git;
using Cygnus2_0.ViewModel.Sonar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Git
{
    public class ObjectGitViewModel
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _buscar;
        private readonly DelegateCommand _limpiar;
        private readonly DelegateCommand _entrega;
        private readonly DelegateCommand _examinar;
        private readonly DelegateCommand _renombrar;
        private readonly DelegateCommand _gitBash;

        public ICommand Procesar => _process;
        public ICommand Buscar => _buscar;
        public ICommand Limpiar => _limpiar;
        public ICommand Entrega => _entrega;
        public ICommand Examinar => _examinar;
        public ICommand Renombrar => _renombrar;
        public ICommand GitBash => _gitBash;

        public ObjectGitViewModel(Handler handler)
        {
            this.handler = handler;
            this.GitModel = new ObjectGitModel(this);
            _process = new DelegateCommand(OnProcess);
            _buscar = new DelegateCommand(pBuscar);
            _limpiar = new DelegateCommand(pLimpiar);
            _entrega = new DelegateCommand(pEntrega);
            _examinar = new DelegateCommand(pExaminar);
            _renombrar = new DelegateCommand(pRenombrar);
            _gitBash = new DelegateCommand(pEjecutaGitBash);
        }
        public ObjectGitModel GitModel { get; set; }
        public List<SelectListItem> ListaSino
        {
            get { return handler.ListaSiNO; }
            set { handler.ListaSiNO = value; }
        }
        public ObservableCollection<Repositorio> ListaGit
        {
            get { return handler.RepositorioVM.ListaGit; }
            set { handler.RepositorioVM.ListaGit = value; }
        }

        public Repositorio GitSeleccionado { get; set; }
        public RamaRepositorio RamaSeleccionada { get; set; }

        public void OnProcess(object commandParameter)
        {
            if(string.IsNullOrEmpty(GitModel.Codigo))
            {
                handler.MensajeError("Ingrese un nombre para la línea base.");
                return;
            }

            try
            {
                handler.CursorWait();
                RepoGit.pCreaLineaBase(GitModel, handler, GitSeleccionado);
                GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(handler,GitSeleccionado);
                handler.CursorNormal();
                GitModel.ListaArchivosEncontrados.Clear();
                handler.MensajeOk("Línea Base ["+ GitModel.Codigo.ToUpper()+"] Creada con Éxito!");
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void pBuscar(object commandParameter)
        {
            try
            {
                if(GitModel.RamaLBSeleccionada == null)
                {
                    handler.MensajeError("Seleccione una rama.");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.ObjetoBuscar))
                {
                    handler.MensajeError("Ingrese el nombre del objeto a buscar.");
                    return;
                }

                handler.CursorWait();

                GitModel.ListaArchivosEncontrados.Clear();
                RepoGit.pSetearLineaBase(GitModel.RamaLBSeleccionada.Text, GitSeleccionado);
                List<Archivo> archivos = new List<Archivo>();
                handler.pListaArchivosCarpeta(GitSeleccionado.Ruta, archivos);
                archivos = archivos.FindAll(x => x.FileName.ToUpper().IndexOf(GitModel.ObjetoBuscar.ToUpper()) > -1);

                foreach(Archivo archivo in archivos)
                {
                    GitModel.ListaArchivosEncontrados.Add(archivo);
                }

                handler.CursorNormal();
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void pLimpiar(object commandParameter)
        {
            GitModel.Codigo = "";
            GitModel.ObjetoBuscar = "";
            GitModel.ListaArchivosEncontrados.Clear();
            GitModel.ListaRamasLB = null;
            ListaGit.Clear();
            ListaGit = SqliteDAO.pListaRepositorios();

            //if (GitSeleccionado != null)
            //    GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(handler,GitSeleccionado);

            GitModel.Comentario = "";
            GitModel.HU = "";
            GitModel.ListaArchivos.Clear();
            GitModel.ListaCarpetas.Clear();
            GitModel.ActivaAprobRamas = false;
            GitModel.NuevaRama = "";
            GitModel.ListaRamasCreadas.Clear();
            try
            {
                this.GitModel.ListaHU = null;
                this.GitModel.ListaHU = SqliteDAO.pObtListaHUAzure(handler);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pEntrega(object commandParameter)
        {
            bool archivosNoRepo = false;
            bool blDocArquitectura = false;
            string archivoRojo = "";

            try
            {
                if (GitSeleccionado == null)
                {
                    handler.MensajeError("Seleccione un repositorio");
                    return;
                }

                if (GitModel.RamaLBSeleccionada == null)
                {
                    handler.MensajeError("Seleccione la línea base del repositorio");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.Comentario))
                {
                    handler.MensajeError("Ingrese un comentario para el commit");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.HU))
                {
                    handler.MensajeError("Ingrese la carpeta despliegue");
                    return;
                }

                if (GitModel.ListaArchivos.Count() == 0)
                {
                    handler.MensajeError("Debe colocar los objetos que se van a versionar");
                    return;
                }

                foreach(Archivo archivo in GitModel.ListaArchivos)
                {
                    archivoRojo = archivo.FileName;

                    if (archivo.Tipo == null && archivo.Tipo == null)
                    {
                        archivosNoRepo = true;
                        break;
                    }

                    if (string.IsNullOrEmpty(archivo.NombreObjeto) && archivo.Tipo != Int32.Parse(res.TipoOtros) && archivo.Tipo != Int32.Parse(res.TipoAplica))
                    {
                        if (string.IsNullOrEmpty(archivo.Usuario) && archivo.Tipo != Int32.Parse(res.TipoOtros))
                        {
                            archivosNoRepo = true;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(GitSeleccionado.Documento) || archivo.FileName.ToUpper().StartsWith(GitSeleccionado.Documento.ToUpper()))
                    {
                        blDocArquitectura = true;
                    }
                }

                if(archivosNoRepo)
                {
                    handler.MensajeError("El archivo ["+archivoRojo+"] debe tener TIPO, USUARIO y REPOSITORIO. Por favor revisar.");
                    return;
                }

                if (!blDocArquitectura)
                {
                    handler.MensajeError("Debe adicionar el documento de arquitectura con prefijo ["+ GitSeleccionado.Documento + "]");
                    return;
                }

                if (!GitModel.ActivaAprobRamas)
                {
                    handler.MensajeError("Por favor revise y apruebe la estructura de archivos que detectó la aplicación.");
                    return;
                }

                if (handler.MensajeConfirmacion("Seguro que quiere procesar el repositorio [ " + GitSeleccionado.Descripcion.ToUpper() + " ]?") == res.No)
                {
                    return;
                }

                if(string.IsNullOrEmpty(handler.Azure.Correo))
                {
                    handler.MensajeError("Configure el correo empresarial [Ajustes/Herramientas Gestión/Azure]");
                    return;
                }

                handler.CursorWait();
                RepoGit.pVersionarObjetos(GitModel,handler,GitSeleccionado);
                
                GitModel.ActivaAprobRamas = false;

                if(GitModel.EjecutaSonar)
                    pEjecutarSonar();

                handler.CursorNormal();
                handler.MensajeOk("Proceso terminó!");
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        private void pEjecutarSonar()
        {
            ObservableCollection<Archivo> archivosSonar = new ObservableCollection<Archivo>();

            if(GitSeleccionado == null)
            {
                handler.MensajeError("Debe seleccionar un repositorio");
                return;
            }

            if (GitModel.RamaLBSeleccionada == null)
            {
                handler.MensajeError("Debe seleccionar una rama del repositorio");
                return;
            }

            foreach (Archivo item in GitModel.ListaArchivos)
            {
                string path = handler.ListaRutas.FirstOrDefault(x => x.TipoObjeto == item.Tipo && x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).Ruta;

                if ( item.SelectItemTipo != null && res.Extensiones.IndexOf(item.Extension.ToLower()) > -1 && !String.IsNullOrEmpty(path) && !string.IsNullOrEmpty(item.NombreObjeto))
                {
                    item.Ruta = GitSeleccionado.Ruta + "\\" + path;

                    item.Ruta = item.Ruta.Replace("[usuario]", item.Usuario != null ? item.Usuario : "");
                    item.Ruta = item.Ruta.Replace(res.TagHTML_nombre,item.NombreObjeto);
                    archivosSonar.Add(item);
                }
            }

            if (archivosSonar.Count == 0)
                return;

            if(string.IsNullOrEmpty(handler.RutaSonar))
            {
                handler.MensajeError("Configure Sonar [Ajustes/Herramientas Gestión/Sonar]");
                return;
            }

            if (string.IsNullOrEmpty(handler.ProyectoSonar))
            {
                handler.MensajeError("Configure Sonar [Ajustes/Herramientas Gestión/Sonar]");
                return;
            }

            List<string> salida = SonarViewModel.pSonar(GitModel.RamaLBSeleccionada.Text, handler, archivosSonar,GitSeleccionado);

            string exito = salida.Find(x => x.IndexOf("ANALYSIS SUCCESSFUL, you can browse") > -1);

            StringBuilder salidaBuild = new StringBuilder();

            foreach (string linea in salida)
            {
                salidaBuild.AppendLine(linea);
            }

            if (exito != null)
            {
                string[] vecExito = exito.Split(' ');
                string url = vecExito[vecExito.Length - 1];
                Process.Start(url);
            }

            handler.CursorNormal();

            UserControl log = new UserControlLog(salidaBuild);
            WinImage request = new WinImage(log, "Traza");
            RepoGit.pRemoverCambiosSonar(handler,GitSeleccionado);
            request.ShowDialog();
        }

        public void pExaminar(object commandParameter)
        {
            if (GitModel.RamaLBSeleccionada == null)
            {
                handler.MensajeError("Seleccione una rama de línea base.");
                return;
            }

            string[] archivos = handler.pCargarArchivos();
            ListarArchivos(archivos);
            pArmarArbol(null,null);
        }

        public void pRenombrar(object commandParameter)
        {
            try
            {
                if (GitModel.RamaLBSeleccionada == null)
                {
                    handler.MensajeError("Seleccione una rama de línea base.");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.NuevaRama))
                {
                    handler.MensajeError("Ingrese un nuevo nombre para la rama.");
                    return;
                }

                handler.CursorWait();
                RepoGit.pRenombrar(handler, GitModel.RamaLBSeleccionada.Text, GitModel.NuevaRama,GitSeleccionado);
                handler.CursorNormal();
                handler.MensajeOk("Rama Renombrada con éxito");
                pLimpiar(null);
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        internal void pCreaRama()
        {
            try
            {
                if(string.IsNullOrEmpty(handler.RepositorioVM.RutaGitBash))
                {
                    handler.MensajeError("Debe configurar la ruta del GitBash.exe [Ajustes/Herramientas Gestión/GIT]");
                    return;
                }

                handler.CursorWait();

                RepoGit.pCreaRamaRepo(handler,GitSeleccionado.Ruta, RamaSeleccionada.Rama, RamaSeleccionada.Estandar);

                handler.CursorNormal();
                handler.MensajeOk("Rama Creada!");
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void pEjecutaGitBash(object commandParameter)
        {
            RepoGit.ExecuteGitBash(handler.RepositorioVM.RutaGitBash+"\\"+res.GitBashExe,GitSeleccionado.Ruta);
        }

        public void pArmarArbol(TipoObjetos tipo, Archivo itemModif)
        {
            Folder raiz;
            string usuarioTipo;

            GitModel.ListaCarpetas.Clear();

            if (GitModel.RamaLBSeleccionada == null)
            {
                return;
            }

            if (GitSeleccionado != null)
            {
                var Imgcarpeta = new BitmapImage(new Uri(String.Format("img/{0}", "icons8-folder-48.png"), UriKind.Relative));
                Imgcarpeta.Freeze();

                var ImgArchivo = new BitmapImage(new Uri(String.Format("img/{0}", "icons8-archivo-de-codigo-32.png"), UriKind.Relative));
                ImgArchivo.Freeze();

                raiz = new Folder { FolderLabel = GitSeleccionado.Descripcion,Icon = Imgcarpeta, FullPath = "", IsNodeExpanded = true, Folders = new List<Folder>() };

                foreach (Archivo archivo in GitModel.ListaArchivos)
                {
                    if (tipo != null && itemModif != null && archivo.FileName.Equals(itemModif.FileName))
                    {
                        usuarioTipo = handler.pObtUsuarioTipo(archivo.Tipo);
                        archivo.Usuario = !string.IsNullOrEmpty(usuarioTipo) ? usuarioTipo : archivo.Usuario;
                        archivo.NombreObjeto = !string.IsNullOrEmpty(usuarioTipo) ? archivo.NombreSinExt.ToLower() : "";
                    }

                    if (itemModif != null && archivo.FileName.Equals(itemModif.FileName))
                    {
                        archivo.NombreObjeto = itemModif.NombreObjeto;
                    }

                    pGeneraHijos(archivo, raiz, Imgcarpeta, ImgArchivo);
                }

                GitModel.ListaCarpetas.Add(raiz);
            }
        }
        public void pGeneraHijos(Archivo archivo, Folder carpetaPadre, ImageSource Imgcarpeta, ImageSource ImgArchivo)
        {
            if (archivo.SelectItemTipo == null)
                return;

            Folder carpetaHija = new Folder();
            string path;

            if (archivo.Tipo == null)
                return;

            path = handler.ListaRutas.FirstOrDefault(x => x.TipoObjeto == archivo.Tipo && x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo).Ruta;

            if (string.IsNullOrEmpty(path))
                return;

            path = path.Replace("[nombre]", archivo.NombreObjeto);
            path = path.Replace("[usuario]", archivo.Usuario != null ? archivo.Usuario : "");
            path = path.Replace("[hu]", GitModel.HU);

            archivo.RutaRepo = Path.Combine(path, archivo.FileName);
            string carpetaPadreRepo = handler.pObtCarpetaPadre(archivo.RutaRepo);

            List<String> Carpetashijas = new List<String>();

            string[] carpetas = path.Split('\\');

            for (int i = 0; i < carpetas.Length; i++)
            {
                Carpetashijas.Add(carpetas[i]);
            }

            for (int i = 0; i < Carpetashijas.Count; i++)
            {
                path = Carpetashijas[i];

                if (!string.IsNullOrEmpty(path))
                {
                    carpetaHija = carpetaPadre.Folders.ToList().Find(x => x.FolderLabel.Equals(path));

                    if (carpetaHija == null)
                    {
                        carpetaHija = new Folder();
                        carpetaHija.FolderLabel = path;
                        carpetaHija.Icon = Imgcarpeta;
                        carpetaPadre.Folders.Add(carpetaHija);
                    }

                    carpetaPadre = carpetaHija;
                }
            }

            carpetaHija.Folders.Add(new Folder { FolderLabel = archivo.FileName,Icon = ImgArchivo, IsNodeExpanded = false, FullPath = archivo.RutaConArchivo });
        }

        public void ListarArchivos(string[] DropPath)
        {
            List<Archivo> archivos = new List<Archivo>();
            handler.pListaArchivos(DropPath, archivos,res.GIT);

            foreach (Archivo archivo in archivos)
            {
                /*if (archivo.Extension.Equals(res.ExtensionHtml) && archivos.Exists(x => x.NombreObjeto.Equals(archivo.NombreObjeto) && !x.Extension.Equals(res.ExtensionHtml)))
                {
                    string tipoNuevo = archivos.Find(x => x.NombreObjeto.Equals(archivo.NombreObjeto) && !x.Extension.Equals(res.ExtensionHtml)).Tipo;
                    archivo.Tipo = string.IsNullOrEmpty(tipoNuevo)?archivo.Tipo:tipoNuevo;
                }*/

                if (!this.GitModel.ListaArchivos.ToList().Exists(x => x.FileName.Equals(archivo.FileName)))
                    this.GitModel.ListaArchivos.Add(archivo);
            }

            if(this.GitModel.ListaArchivos.Count > 0)
            {
                List<Archivo> archivosHtml = this.GitModel.ListaArchivos.ToList().FindAll(x=>x.Extension.Equals(res.ExtensionHtml));

                if(archivosHtml.Count > 0)
                {
                    foreach (Archivo archivo in archivosHtml)
                    {
                        if (this.GitModel.ListaArchivos.ToList().Exists(x => x.NombreObjeto.Equals(archivo.NombreObjeto) && !x.Extension.Equals(res.ExtensionHtml)))
                        {
                            int? tipoNuevo = this.GitModel.ListaArchivos.ToList().Find(x => x.NombreObjeto.Equals(archivo.NombreObjeto) && !x.Extension.Equals(res.ExtensionHtml)).Tipo;

                            if(this.GitModel.ListaArchivos.ToList().Exists(x => x.FileName.Equals(archivo.FileName)))
                                this.GitModel.ListaArchivos.ToList().Find(x => x.FileName.Equals(archivo.FileName)).Tipo = tipoNuevo == null?archivo.Tipo:tipoNuevo;
                        }
                    }
                }
            }

            pArmarArbol(null, null);
        }

        public void pPonerUsuarioArchivos(Archivo archivo, SelectListItem usuario)
        {
            foreach(Archivo fila in GitModel.ListaArchivos)
            {
                if(fila.CarpetaPadre.Equals(archivo.CarpetaPadre))
                {
                    fila.Usuario = usuario.Text;
                }
            }
        }
        public void pCreaRamas()
        {
            if(GitSeleccionado != null)
            {
                GitModel.ListaRamasCreadas = SqliteDAO.pListaRamaRepositorios(GitSeleccionado);

                foreach (RamaRepositorio rama in GitModel.ListaRamasCreadas)
                {
                    rama.Estandar = rama.Estandar.Replace(res.TagHU, GitModel.HU);
                    rama.Estandar = rama.Estandar.Replace(res.TagUsuario, Environment.UserName.ToUpper());
                }
            }
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Git;
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
            this.GitModel = new ObjectGitModel();
            _process = new DelegateCommand(OnProcess);
            _buscar = new DelegateCommand(pBuscar);
            _limpiar = new DelegateCommand(pLimpiar);
            _entrega = new DelegateCommand(pEntrega);
            _examinar = new DelegateCommand(pExaminar);
            _renombrar = new DelegateCommand(pRenombrar);
            _gitBash = new DelegateCommand(pEjecutaGitBash);

            this.GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(this.handler);
        }
        public ObjectGitModel GitModel { get; set; }

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
                RepoGit.pCreaLineaBase(GitModel, handler);
                GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(handler);
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
                RepoGit.pSetearLineaBase(GitModel.RamaLBSeleccionada.Text, handler);
                List<Archivo> archivos = new List<Archivo>();
                handler.pListaArchivosCarpeta(handler.RutaGitObjetos, archivos);
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
            if (!string.IsNullOrEmpty(handler.RutaGitObjetos))
                GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(handler);
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
                this.GitModel.ListaHU = handler.DAO.pObtListaHUAzure();
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
                if (GitModel.RamaLBSeleccionada == null)
                {
                    handler.MensajeError("Seleccione una rama de línea base.");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.Comentario))
                {
                    handler.MensajeError("Ingrese un comentario para el commit.");
                    return;
                }

                if (GitModel.ListaArchivos.Count() == 0)
                {
                    handler.MensajeError("Debe colocar los objetos que se van a versionar.");
                    return;
                }

                foreach(Archivo archivo in GitModel.ListaArchivos)
                {
                    archivoRojo = archivo.FileName;

                    if (archivo.Tipo == null && string.IsNullOrEmpty(archivo.Tipo))
                    {
                        archivosNoRepo = true;
                        break;
                    }

                    if (string.IsNullOrEmpty(archivo.NombreObjeto) && !archivo.Tipo.ToLower().Equals(res.TipoOtros.ToLower()) && !archivo.Tipo.ToLower().Equals(res.TipoAplica.ToLower()))
                    {
                        if (string.IsNullOrEmpty(archivo.Usuario) && !archivo.Tipo.ToLower().Equals(res.TipoOtros.ToLower()))
                        {
                            archivosNoRepo = true;
                            break;
                        }
                    }

                    if (archivo.FileName.ToUpper().StartsWith(handler.ConfGeneralView.Model.Empresa.DocumentoAD.ToUpper()))
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
                    handler.MensajeError("Debe adicionar el documento de arquitectura con prefijo ["+ handler.ConfGeneralView.Model.Empresa.DocumentoAD+"]");
                    return;
                }

                if (!GitModel.ActivaAprobRamas)
                {
                    handler.MensajeError("Por favor revise y apruebe la estructura de archivos que detectó la aplicación.");
                    return;
                }

                handler.CursorWait();
                RepoGit.pVersionarObjetos(GitModel,handler);
                
                GitModel.ActivaAprobRamas = false;

                if(GitModel.EjecutaSonar)
                    pEjecutarSonar();

                handler.CursorNormal();
                handler.MensajeOk("Continue creando manualmente las ramas Feature!");
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

            foreach(Archivo item in GitModel.ListaArchivos)
            {
                if( item.SelectItemTipo != null && res.Extensiones.IndexOf(item.Extension.ToLower()) > -1 && !String.IsNullOrEmpty(item.SelectItemTipo.Path) && !string.IsNullOrEmpty(item.NombreObjeto))
                {
                    item.Ruta = handler.RutaGitObjetos +"\\"+item.Usuario+ "\\" + item.SelectItemTipo.Path;
                    item.Ruta = item.Ruta.Replace(res.TagHTML_nombre,item.NombreObjeto);
                    archivosSonar.Add(item);
                }
            }

            if (archivosSonar.Count == 0)
                return;

            List<string> salida = SonarViewModel.pSonar(GitModel.RamaLBSeleccionada.Text, handler, archivosSonar);

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
            RepoGit.pRemoverCambiosSonar(GitModel, handler);
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
                RepoGit.pRenombrar(handler, GitModel.RamaLBSeleccionada.Text, GitModel.NuevaRama);
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

        internal void pCreaRama(Archivo archivo)
        {
            try
            {
                handler.CursorWait();

                if (archivo.FileName.EndsWith("DLL"))
                    RepoGit.pCreaRamaRepo(handler, res.RamaDesarrollo, archivo.FileName);
                else if (archivo.FileName.EndsWith("PRU"))
                    RepoGit.pCreaRamaRepo(handler, res.RamaPruebas, archivo.FileName);
                else if (archivo.FileName.EndsWith("PDN"))
                    RepoGit.pCreaRamaRepo(handler, res.RamaProduccion, archivo.FileName);

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
            RepoGit.ExecuteGitBash(handler.RutaGitBash+"\\"+res.GitBashExe,handler.RutaGitObjetos);
        }

        public void pArmarArbol(SelectListItem tipo, Archivo itemModif)
        {
            string usuario = "-";
            Folder CarpetaPadre;

            if(GitModel.RamaLBSeleccionada == null)
            {
                return;
            }

            string carpetaDespliegue = res.Despliegues+"\\" + GitModel.RamaLBSeleccionada.Text;

            GitModel.ListaCarpetas.Clear();

            Folder raiz = new Folder { FolderLabel = res.Carpetas, FullPath = "", IsNodeExpanded = true, Folders = new List<Folder>() };
            Folder despliegue = new Folder { FolderLabel = carpetaDespliegue, FullPath = "", Folders = new List<Folder>() };

            raiz.Folders.Add(despliegue);

            var usuarios = GitModel.ListaArchivos.Select(x => x.Usuario).Distinct();

            foreach (var archivo in usuarios)
            {
                if (!string.IsNullOrEmpty(archivo))
                {
                    CarpetaPadre = new Folder { FolderLabel = archivo, FullPath = "", IsNodeExpanded = true, Folders = new List<Folder>() };
                    raiz.Folders.Add(CarpetaPadre);
                }
            }

           foreach (Archivo archivo in GitModel.ListaArchivos)
           {
                if (tipo != null && itemModif != null && archivo.FileName.Equals(itemModif.FileName))
                {
                    archivo.Usuario = !string.IsNullOrEmpty(tipo.Usuario) ? tipo.Usuario : archivo.Usuario;
                    archivo.NombreObjeto = !string.IsNullOrEmpty(tipo.Usuario) ? archivo.NombreSinExt.ToLower() : "";
                }

                if(itemModif != null && archivo.FileName.Equals(itemModif.FileName))
                {
                    archivo.NombreObjeto = itemModif.NombreObjeto;
                }

                if (!string.IsNullOrEmpty(archivo.Usuario))
                {
                    CarpetaPadre = raiz.Folders.Find(x => x.FolderLabel.Equals(archivo.Usuario)); 

                    if (CarpetaPadre != null)
                    {
                        pGeneraHijos(archivo, CarpetaPadre);
                    }
                }

                if (archivo.SelectItemTipo != null && !string.IsNullOrEmpty(archivo.SelectItemTipo.Path) || archivo.Extension.ToLower().Equals(res.ExtensionHtml))
                    continue;

                despliegue.Folders.Add(new Folder { FolderLabel = archivo.FileName, IsNodeExpanded = false, FullPath = archivo.RutaConArchivo });
           }

            GitModel.ListaCarpetas.Add(raiz);
        }

        public void pGeneraHijos(Archivo archivo, Folder carpetaPadre)
        {
            if (archivo.SelectItemTipo == null)
                return;

            Folder carpetaHija;
            string path;

            if (string.IsNullOrEmpty(archivo.SelectItemTipo.Path))
                return;

            path = archivo.SelectItemTipo.Path.Replace("[nombre]", archivo.NombreObjeto);
            Boolean blExistePath = true;

            archivo.RutaRepo = Path.Combine(path, archivo.FileName);
            string carpetaPadreRepo = handler.pObtCarpetaPadre(archivo.RutaRepo);

            List<String> Carpetashijas = new List<String>();

            int nuIndex = path.IndexOf(carpetaPadreRepo);

            if (nuIndex < 0)
                return;

            string rutaparcial = path.Substring(0, nuIndex);
            string rutaparcial2 = archivo.RutaRepo.Substring(nuIndex);

            Carpetashijas.Add(rutaparcial);

            string[] carpetas = rutaparcial2.Split('\\');

            for (int i=0;i< carpetas.Length;i++)
            {
                Carpetashijas.Add(carpetas[i]);
            }

            for (int i=0;i<Carpetashijas.Count-1;i++)
            {
                path = Carpetashijas[i];

                if (!string.IsNullOrEmpty(path))
                {
                    carpetaHija = carpetaPadre.Folders.ToList().Find(x => x.FolderLabel.Equals(path));

                    if (carpetaHija == null)
                    {
                        carpetaHija = new Folder();
                        carpetaHija.FolderLabel = path;

                        if (i == Carpetashijas.Count - 2)
                        {
                            carpetaHija.Folders.Add(new Folder { FolderLabel = archivo.FileName, IsNodeExpanded = false, FullPath = archivo.RutaConArchivo });
                        }

                        carpetaPadre.Folders.Add(carpetaHija);
                        blExistePath = false;
                    }

                    carpetaPadre = carpetaHija;
                }
            }

            if (blExistePath)
            {
                carpetaPadre.Folders.Add(new Folder { FolderLabel = archivo.FileName, FullPath = archivo.RutaConArchivo });
            }

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
                            string tipoNuevo = this.GitModel.ListaArchivos.ToList().Find(x => x.NombreObjeto.Equals(archivo.NombreObjeto) && !x.Extension.Equals(res.ExtensionHtml)).Tipo;

                            if(this.GitModel.ListaArchivos.ToList().Exists(x => x.FileName.Equals(archivo.FileName)))
                                this.GitModel.ListaArchivos.ToList().Find(x => x.FileName.Equals(archivo.FileName)).Tipo = string.IsNullOrEmpty(tipoNuevo)?archivo.Tipo:tipoNuevo;
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
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Git;
using Cygnus2_0.Pages.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public ICommand Process => _process;
        public ICommand Buscar => _buscar;
        public ICommand Limpiar => _limpiar;
        public ICommand Entrega => _entrega;
        public ICommand Examinar => _examinar;

        public ObjectGitViewModel(Handler handler)
        {
            this.handler = handler;
            this.GitModel = new ObjectGitModel();
            _process = new DelegateCommand(OnProcess);
            _buscar = new DelegateCommand(pBuscar);
            _limpiar = new DelegateCommand(pLimpiar);
            _entrega = new DelegateCommand(pEntrega);
            _examinar = new DelegateCommand(pExaminar);

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
            GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(handler);
            GitModel.Comentario = "";
            GitModel.HU = "";
            GitModel.ListaArchivos.Clear();
        }
        public void pEntrega(object commandParameter)
        {
            try
            {
                if (GitModel.RamaLBSeleccionada == null)
                {
                    handler.MensajeError("Seleccione una rama.");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.HU))
                {
                    handler.MensajeError("Ingrese la historia de usuario.");
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


            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void pExaminar(object commandParameter)
        {
            string usuario = "-";
            Folder CarpetaPadre;
            string[] archivos = handler.pCargarArchivos();
            ListarArchivos(archivos);

            GitModel.ListaCarpetas.Clear();

            foreach (Archivo archivo in GitModel.ListaArchivos)
            {
                if(!string.IsNullOrEmpty(archivo.Usuario))
                {
                    if(!usuario.Equals(archivo.Usuario))
                    {
                        usuario = archivo.Usuario;

                        if(!GitModel.ListaCarpetas.ToList().Exists(x=>x.FolderLabel.Equals(usuario)))
                            GitModel.ListaCarpetas.Add(new Folder { FolderLabel = archivo.Usuario});
                    }

                    CarpetaPadre = GitModel.ListaCarpetas.ToList().Find(x=>x.FolderLabel.Equals(usuario));

                    if (CarpetaPadre != null)
                    {
                        //CarpetaPadre.Folders.Add(new Folder {FolderLabel= "prueba" });
                        pGeneraHijos(archivo, CarpetaPadre);
                    }

                }
            }
        }

        public void pGeneraHijos(Archivo archivo, Folder carpetaPadre)
        {
            Folder carpetaHija;
            string[] Carpetashijas = archivo.SelectItemTipo.Path.Split('\\');
            string path;

            for(int i=0;i<Carpetashijas.Length;i++)
            {
                path = Carpetashijas[i];

                if (!string.IsNullOrEmpty(path))
                {
                    path = path.Replace("[nombre]", archivo.NombreObjeto);
                    carpetaHija = carpetaPadre.Folders.ToList().Find(x => x.FolderLabel.Equals(path));

                    if (carpetaHija == null)
                    {
                        carpetaHija = new Folder();
                        carpetaHija.FolderLabel = path;

                        if (i == Carpetashijas.Length-2)
                        {
                            carpetaHija.Folders.Add(new Folder { FolderLabel = archivo.FileName });
                        }

                        carpetaPadre.Folders.Add(carpetaHija);
                    }

                    carpetaPadre = carpetaHija;
                }
            }
        }

        public void ListarArchivos(string[] DropPath)
        {
            List<Archivo> archivos = new List<Archivo>();
            handler.pListaArchivos(DropPath, archivos);

            foreach (Archivo archivo in archivos)
            {
                this.GitModel.ListaArchivos.Add(archivo);
            }
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

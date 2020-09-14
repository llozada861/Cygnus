using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Git;
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

        public ICommand Process => _process;
        public ICommand Buscar => _buscar;
        public ICommand Limpiar => _limpiar;

        public ObjectGitViewModel(Handler handler)
        {
            this.handler = handler;
            this.GitModel = new ObjectGitModel();
            _process = new DelegateCommand(OnProcess);
            _buscar = new DelegateCommand(pBuscar);
            _limpiar = new DelegateCommand(pLimpiar);

            this.handler.RutaGitObjetos = @"D:\RepoGitEPM\BaseDeDatos";
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
        }
    }
}

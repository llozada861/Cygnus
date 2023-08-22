using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Repository
{
    public class RepositorioViewModel: ViewModelBase,IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _agregar;
        private readonly DelegateCommand _eliminar;
        private readonly DelegateCommand _eliminarRama;
        private ObservableCollection<Repositorio> listaGit;
        private ObservableCollection<RamaRepositorio> listaRamaGit;
        public RepositorioViewModel(Handler handler)
        {
            this.handler = handler;
            _process = new DelegateCommand(OnProcess);
            _agregar = new DelegateCommand(Agregar);
            _eliminar = new DelegateCommand(OnDelete);
            _eliminarRama = new DelegateCommand(OnDeleteRama);
            ListaGit = SqliteDAO.pListaRepositorios();
            RutaGitBash = SqliteDAO.pObtValorConfiguracion(res.KeyRutaGitBash);
        }
        public ICommand Add => _agregar;
        public ICommand Process => _process;
        public ICommand Delete => _eliminar;
        public ICommand DeleteRama => _eliminarRama;
        public Repositorio Model { get; set; }
        public ObservableCollection<Repositorio> ListaGit
        {
            get { return listaGit; }
            set 
            {
                SetProperty(ref listaGit, value);

                foreach (Repositorio objeto in ListaGit)
                {
                    objeto.ListaTipos = new ObservableCollection<TipoObjetos>(handler.ListaTiposObjetos);
                }
            }
        }
        public ObservableCollection<RamaRepositorio> ListaRamaGit
        {
            get { return listaRamaGit; }
            set 
            { 
                SetProperty(ref listaRamaGit, value);

                foreach (RamaRepositorio objeto in listaRamaGit)
                {
                    objeto.ListaSINO = new ObservableCollection<SelectListItem>(handler.ListaSiNO);
                }
            }
        }
        public Repositorio RepoSeleccionado { get; set; }
        public RamaRepositorio RamaSeleccionada { get; set; }
        public Boolean TabRepo { get; set; }
        public Boolean TabRama { get; set; }
        public string RutaGitBash { get; set; }
        public void OnClean(object commandParameter)
        {
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if(RutaGitBash == null || string.IsNullOrEmpty(RutaGitBash))
                {
                    handler.MensajeError("Agregue la ruta del GitBash.exe");
                    return;
                }

                if (!string.IsNullOrEmpty(RutaGitBash))
                {
                    SqliteDAO.pCreaConfiguracion(res.KeyRutaGitBash, RutaGitBash);
                    handler.RepositorioVM.RutaGitBash = RutaGitBash;
                }

                if (handler.RepositorioVM.TabRepo)
                {
                    foreach (Repositorio repo in handler.RepositorioVM.ListaGit)
                    {
                        if(string.IsNullOrEmpty(repo.Descripcion))
                        {
                            handler.MensajeError("Ingrese una descripción");
                            return;
                        }

                        if (string.IsNullOrEmpty(repo.Ruta))
                        {
                            handler.MensajeError("Ingrese una ruta local del repositorio");
                            return;
                        }

                        if (repo.Codigo != null)
                            SqliteDAO.pEditaRepositorio(repo);
                        else
                            SqliteDAO.pCreaRepositorio(repo);
                    }
                }

                if (handler.RepositorioVM.TabRama)
                {
                    foreach (RamaRepositorio rama in ListaRamaGit)
                    {
                        if (string.IsNullOrEmpty(rama.Rama))
                        {
                            handler.MensajeError("Ingrese una Rama del repositorio");
                            continue;
                        }

                        if (string.IsNullOrEmpty(rama.LBase))
                        {
                            handler.MensajeError("Identifique si la rama es Línea Base");
                            continue;
                        }

                        if (rama.Codigo != null)
                            SqliteDAO.pEditaRamRepositorio(rama);
                        else
                            SqliteDAO.pCreaRamaRepositorio(rama);
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void Agregar(object commandParameter)
        {
            try
            {
                if (handler.RepositorioVM.TabRepo)
                {
                    Repositorio repo = new Repositorio();
                    repo.Empresa = Convert.ToInt32(handler.ConfGeneralView.Model.Empresa.Codigo);
                    repo.ListaTipos = new ObservableCollection<TipoObjetos>(handler.ListaTiposObjetos);
                    ListaGit.Add(repo);
                }

                if (handler.RepositorioVM.TabRama)
                {
                    if (RepoSeleccionado == null)
                    {
                        handler.MensajeError("Para agregar una Rama debe seleccionar un Repositorio");
                        return;
                    }

                    if (RepoSeleccionado.Codigo == null)
                    {
                        handler.MensajeError("Guarde el repositorio");
                        return;
                    }

                    RamaRepositorio rama = new RamaRepositorio();
                    rama.RepositorioId = RepoSeleccionado.Codigo;
                    rama.Estandar = "feature/[HU]_[USUARIO]";
                    rama.ListaSINO = new ObservableCollection<SelectListItem>(handler.ListaSiNO);
                    ListaRamaGit.Add(rama);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnDelete(object commandParameter)
        {
            try
            {
                if(RepoSeleccionado != null && RepoSeleccionado.Codigo == null)
                {
                    ListaGit.Remove(RepoSeleccionado);
                }
                else if (RepoSeleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar el repositorio [" + RepoSeleccionado.Descripcion + " - " + RepoSeleccionado.Ruta + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaRepo(RepoSeleccionado);
                    ListaGit = SqliteDAO.pListaRepositorios();
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnDeleteRama(object commandParameter)
        {
            try
            {
                if (RamaSeleccionada != null && RamaSeleccionada.Codigo == null)
                {
                    ListaRamaGit.Remove(RamaSeleccionada);
                }
                else if (RamaSeleccionada != null && handler.MensajeConfirmacion("Seguro que desea eliminar la rama [" + RamaSeleccionada.Rama + " - " + RamaSeleccionada.Estandar + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaRamaRepo(RamaSeleccionada);
                    ListaRamaGit = SqliteDAO.pListaRamaRepositorios(RepoSeleccionado);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

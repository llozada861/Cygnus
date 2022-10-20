using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Pages.Git;
using Cygnus2_0.ViewModel.Git;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Git
{
    public class ObjectGitModel: ViewModelBase
    {
        private ObservableCollection<SelectListItem> listaRamasLB;
        private ObservableCollection<Archivo> listaArchivosEncontrados;
        private ObservableCollection<Archivo> listaArchivos;
        private ObservableCollection<RamaRepositorio> listaRamasCreadas;
        private ObservableCollection<Archivo> listaArchivosRamas;
        private ObservableCollection<Folder> listacarpetas;
        private ObservableCollection<SelectListItem> listaHU;
        private ObservableCollection<Repositorio> listaGit;
        private string codigo;
        private string objetoBuscar;
        private string hu;
        private string comentario;
        private SelectListItem _selectHU;
        private SelectListItem ramaLBSeleccionada;
        private bool activaAprobRamas;
        private bool ejecutarSonar;
        private string nuevaRama;
        private ObjectGitViewModel view_;
        public ObjectGitModel(ObjectGitViewModel view)
        {
            this.ListaRamasLB = new ObservableCollection<SelectListItem>();
            this.ListaArchivosEncontrados = new ObservableCollection<Archivo>();
            this.ListaArchivos = new ObservableCollection<Archivo>();
            this.ListaRamasCreadas = new ObservableCollection<RamaRepositorio>();
            this.ListaArchivosRamas = new ObservableCollection<Archivo>();
            this.ListaCarpetas = new ObservableCollection<Folder>();
            this.ListaHU = new ObservableCollection<SelectListItem>();
            this.view_ = view;
        }
        public string Codigo
        {
            get { return codigo; }
            set {
                SetProperty(ref codigo, value.Trim());
            }
        }
        public string ObjetoBuscar
        {
            get { return objetoBuscar; }
            set { SetProperty(ref objetoBuscar, value.Trim()); }
        }
        public string HU
        {
            get { return hu; }
            set
            {
                SetProperty(ref hu, value.Trim().ToUpper());
                view_.pCreaRamas();
                view_.pArmarArbol(null, null);
            }
        }

        public SelectListItem SelectHU
        {
            get { return _selectHU; }
            set {
                SetProperty(ref _selectHU, value);
            }
        }
        public string Comentario
        {
            get { return comentario; }
            set { SetProperty(ref comentario, value.Trim()); }
        }
        public string NuevaRama
        {
            get { return nuevaRama; }
            set { SetProperty(ref nuevaRama, value.Trim()); }
        }
        public SelectListItem RamaLBSeleccionada
        {
            get { return ramaLBSeleccionada; }
            set { SetProperty(ref ramaLBSeleccionada, value);
            }
        }
        public ObservableCollection<SelectListItem> ListaRamasLB
        {
            get { return listaRamasLB; }
            set { SetProperty(ref listaRamasLB, value); }
        }
        public ObservableCollection<SelectListItem> ListaHU
        {
            get { return listaHU; }
            set { SetProperty(ref listaHU, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosEncontrados 
        {
            get { return listaArchivosEncontrados; }
            set { SetProperty(ref listaArchivosEncontrados, value); }
        }
        public ObservableCollection<Archivo> ListaArchivos
        {
            get { return listaArchivos; }
            set { SetProperty(ref listaArchivos, value); }
        }
        public ObservableCollection<RamaRepositorio> ListaRamasCreadas
        {
            get { return listaRamasCreadas; }
            set { SetProperty(ref listaRamasCreadas, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosRamas
        {
            get { return listaArchivosRamas; }
            set { SetProperty(ref listaArchivosRamas, value); }
        }

        public ObservableCollection<Folder> ListaCarpetas
        {
            get { return listacarpetas; }
            set { SetProperty(ref listacarpetas, value); }
        }

        public bool ActivaAprobRamas
        {
            get { return activaAprobRamas; }
            set { SetProperty(ref activaAprobRamas, value); }
        }
        public bool EjecutaSonar {
            get { return ejecutarSonar; }
            set { SetProperty(ref ejecutarSonar, value); }
        }
        public ObservableCollection<Repositorio> ListaGit
        {
            get { return listaGit; }
            set 
            {
                SetProperty(ref listaGit, value); 
            }
        }
    }
}

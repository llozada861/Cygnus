using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Git
{
    public class ObjectGitModel: ViewModelBase
    {
        private ObservableCollection<SelectListItem> listaRamasLB;
        private ObservableCollection<Archivo> listaArchivosEncontrados;
        private ObservableCollection<Archivo> listaArchivos;
        private ObservableCollection<Archivo> listaRamasCreadas;
        private ObservableCollection<Archivo> listaArchivosRamas;
        private string codigo;
        private string objetoBuscar;
        private string hu;
        private string comentario;
        private SelectListItem ramaLBSeleccionada;
        public ObjectGitModel()
        {
            this.ListaRamasLB = new ObservableCollection<SelectListItem>();
            this.ListaArchivosEncontrados = new ObservableCollection<Archivo>();
            this.ListaArchivos = new ObservableCollection<Archivo>();
            this.ListaRamasCreadas = new ObservableCollection<Archivo>();
            this.ListaArchivosRamas = new ObservableCollection<Archivo>();

        }
        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }
        public string ObjetoBuscar
        {
            get { return objetoBuscar; }
            set { SetProperty(ref objetoBuscar, value); }
        }
        public string HU
        {
            get { return hu; }
            set { SetProperty(ref hu, value); }
        }
        public string Comentario
        {
            get { return comentario; }
            set { SetProperty(ref comentario, value); }
        }
        public SelectListItem RamaLBSeleccionada
        {
            get { return ramaLBSeleccionada; }
            set { SetProperty(ref ramaLBSeleccionada, value); }
        }
        public ObservableCollection<SelectListItem> ListaRamasLB
        {
            get { return listaRamasLB; }
            set { SetProperty(ref listaRamasLB, value); }
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
        public ObservableCollection<Archivo> ListaRamasCreadas
        {
            get { return listaRamasCreadas; }
            set { SetProperty(ref listaRamasCreadas, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosRamas
        {
            get { return listaArchivosRamas; }
            set { SetProperty(ref listaArchivosRamas, value); }
        }
    }
}

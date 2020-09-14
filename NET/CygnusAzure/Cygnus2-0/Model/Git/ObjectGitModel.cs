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
        private string codigo;
        private string objetoBuscar;
        private SelectListItem ramaLBSeleccionada;
        public ObjectGitModel()
        {
            this.ListaRamasLB = new ObservableCollection<SelectListItem>();
            this.ListaArchivosEncontrados = new ObservableCollection<Archivo>();
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
    }
}

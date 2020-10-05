using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Pages.Git;
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
        private ObservableCollection<Archivo> listaRamasCreadas;
        private ObservableCollection<Archivo> listaArchivosRamas;
        private ObservableCollection<Folder> listacarpetas;
        private string codigo;
        private string objetoBuscar;
        private string hu;
        private string comentario;
        private SelectListItem ramaLBSeleccionada;
        private bool activaAprobRamas;
        private string nuevaRama;
        public ObjectGitModel()
        {
            this.ListaRamasLB = new ObservableCollection<SelectListItem>();
            this.ListaArchivosEncontrados = new ObservableCollection<Archivo>();
            this.ListaArchivos = new ObservableCollection<Archivo>();
            this.ListaRamasCreadas = new ObservableCollection<Archivo>();
            this.ListaArchivosRamas = new ObservableCollection<Archivo>();
            this.ListaCarpetas = new ObservableCollection<Folder>();

        }
        public string Codigo
        {
            get { return codigo; }
            set {
                SetProperty(ref codigo, value);
            }
        }
        public string ObjetoBuscar
        {
            get { return objetoBuscar; }
            set { SetProperty(ref objetoBuscar, value); }
        }
        public string HU
        {
            get { return hu; }
            set
            {
                SetProperty(ref hu, value);
                pCreaRamas();
            }
        }
        public string Comentario
        {
            get { return comentario; }
            set { SetProperty(ref comentario, value); }
        }
        public string NuevaRama
        {
            get { return nuevaRama; }
            set { SetProperty(ref nuevaRama, value); }
        }
        public SelectListItem RamaLBSeleccionada
        {
            get { return ramaLBSeleccionada; }
            set { SetProperty(ref ramaLBSeleccionada, value);
                pCreaRamas();
            }
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

        public void pCreaRamas()
        {
            if(RamaLBSeleccionada != null)
            { 
                string ramaDll = res.Feature + HU + "_" + RamaLBSeleccionada.Text + "_" + Environment.UserName.ToUpper() + "_DLL";
                string ramaPru = res.Feature + HU + "_" + RamaLBSeleccionada.Text + "_" + Environment.UserName.ToUpper() + "_PRU";
                string ramaPdn = res.Feature + HU + "_" + RamaLBSeleccionada.Text + "_" + Environment.UserName.ToUpper() + "_PDN";

                if (!string.IsNullOrEmpty(HU) && !string.IsNullOrEmpty(RamaLBSeleccionada.Text))
                {
                    ListaRamasCreadas.Clear();
                    ListaRamasCreadas.Add(new Archivo { FileName = ramaDll });
                    ListaRamasCreadas.Add(new Archivo { FileName = ramaPru });
                    ListaRamasCreadas.Add(new Archivo { FileName = ramaPdn });
                }
            }
        }
    }
}

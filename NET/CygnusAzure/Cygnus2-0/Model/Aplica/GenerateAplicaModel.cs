using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.History;
using Cygnus2_0.Model.User;
using Cygnus2_0.ViewModel.Aplica;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Aplica
{
    public class GenerateAplicaModel: ViewModelBase
    {
        private Handler handler;
        private SelectListItem _usuario;
        private string archivosGenerados;
        private string archivosCargados;
        private Boolean objetos;
        private Boolean datos;
        private Boolean aprobarOrden;
        private string codigo;
        private ObservableCollection<Archivo> listaArchivosGenerados;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<SelectListItem> listaUsuarios;
        private ObservableCollection<Archivo> listaAplicaHistoria;
        private ObservableCollection<HistoriaModel> listaHistoria;
        private Visibility visibleObjetos;

        public GenerateAplicaModel(Handler handler)
        {
            this.handler = handler;
        }

        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value.ToUpper()); }
        }
        public SelectListItem Usuario
        {
            get { return _usuario; }
            set { SetProperty(ref _usuario, value); }
        }
        public string ArchivosCargados
        {
            get { return archivosCargados; }
            set { SetProperty(ref archivosCargados, value); }
        }
        public string ArchivosGenerados
        {
            get { return archivosGenerados; }
            set { SetProperty(ref archivosGenerados, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosCargados
        {
            get { return listaArchivosCargados; }
            set { SetProperty(ref listaArchivosCargados, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosGenerados
        {
            get { return listaArchivosGenerados; }
            set { SetProperty(ref listaArchivosGenerados, value); }
        }
        public ObservableCollection<SelectListItem> ListaUsuarios
        {
            get { return listaUsuarios; }
            set { SetProperty(ref listaUsuarios, value); }
        }
        public ObservableCollection<HistoriaModel> ListaHistoria
        {
            get { return listaHistoria; }
            set { SetProperty(ref listaHistoria, value); }
        }
        public Boolean Datos
        {
            get { return datos; }
            set { SetProperty(ref datos, value); }
        }
        public Boolean Objetos
        {
            get { return objetos; }
            set 
            {
                SetProperty(ref objetos, value);

                if (objetos)
                    VisibleObjetos = Visibility.Visible;
                else
                {
                    VisibleObjetos = Visibility.Hidden;
                    AprobarOrden = false;
                }
            }
        }
        public Boolean AprobarOrden
        {
            get { return aprobarOrden; }
            set { SetProperty(ref aprobarOrden, value); }
        }

        public ObservableCollection<Archivo> ListaArchivosNoOrden { get; set; }
        public ObservableCollection<Archivo> ListaAplicaHistoria
        {
            get { return new ObservableCollection<Archivo>(listaAplicaHistoria.OrderBy(x=>x.Tipo)); }
            set { SetProperty(ref listaAplicaHistoria, value); }
        }
        public HistoriaModel Historia { get; set; }

        public Visibility VisibleObjetos
        {
            get { return visibleObjetos; }
            set { SetProperty(ref visibleObjetos, value); }
        }
    }
}

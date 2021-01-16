using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
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
        private string codigo;
        private ObservableCollection<Archivo> listaArchivosGenerados;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<SelectListItem> listaUsuarios;

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
        public Boolean Datos
        {
            get { return datos; }
            set { SetProperty(ref datos, value); }
        }
        public Boolean Objetos
        {
            get { return objetos; }
            set { SetProperty(ref objetos, value); }
        }
    }
}

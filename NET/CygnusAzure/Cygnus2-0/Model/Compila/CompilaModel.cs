using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.ViewModel.Compila;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Compila
{
    public class CompilaModel: ViewModelBase
    {
        private Handler handler;
        private CompilaViewModel view;
        private SelectListItem _usuario;
        private SelectListItem _selectHU;
        private string archivosDescomp;
        private string archivosCompila;
        private string estadoConn;
        private string _codigo;
        private string _hu;
        private string _comentario;
        private ObservableCollection<SelectListItem> listaObservaciones;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<SelectListItem> listaUsuarios;
        private ObservableCollection<SelectListItem> listaHU;
        public CompilaModel(Handler handler)
        {
            this.handler = handler;
        }
        public SelectListItem Usuario
        {
            get { return _usuario; }
            set { SetProperty(ref _usuario, value); }
        }
        public string Codigo
        {
            get { return _codigo; }
            set { SetProperty(ref _codigo, value); }
        }
        public string Comentario
        {
            get { return _comentario; }
            set { SetProperty(ref _comentario, value); }
        }
        public string HU
        {
            get { return _hu; }
            set { SetProperty(ref _hu, value); }
        }
        public string ArchivosCompilados
        {
            get { return archivosCompila; }
            set { SetProperty(ref archivosCompila, value); }
        }
        public string ArchivosDescompilados
        {
            get { return archivosDescomp; }
            set { SetProperty(ref archivosDescomp, value); }
        }
        public string EstadoConn
        {
            get { return handler.EstadoConn; }
            set { SetProperty(ref estadoConn, handler.EstadoConn); }
        }
        public ObservableCollection<Archivo> ListaArchivosCargados
        {
            get { return listaArchivosCargados; }
            set { SetProperty(ref listaArchivosCargados, value); }
        }
        public ObservableCollection<SelectListItem> ListaObservaciones
        {
            get { return listaObservaciones; }
            set { SetProperty(ref listaObservaciones, value); }
        }
        public ObservableCollection<SelectListItem> ListaUsuarios
        {
            get { return listaUsuarios; }
            set { SetProperty(ref listaUsuarios, value); }
        }
        public ObservableCollection<SelectListItem> ListaHU
        {
            get { return listaHU; }
            set { SetProperty(ref listaHU, value); }
        }
        public SelectListItem SelectHU
        {
            get { return _selectHU; }
            set { SetProperty(ref _selectHU, value); }
        }
    }
}

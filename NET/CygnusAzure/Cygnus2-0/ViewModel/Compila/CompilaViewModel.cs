using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Compila;
using Cygnus2_0.Model.Git;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Compila
{
    public class CompilaViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conectar;
        private readonly DelegateCommand _examinar;
        private CompilaModel model;
        private SelectListItem _usuario;
        private string archivosDescomp;
        private string archivosCompila;
        private string estadoConn;
        private string _codigo;
        private string _hu;
        private string _comentario;
        private ObservableCollection<SelectListItem> listaObservaciones;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<SelectListItem> listaUsuarios;

        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conectar;
        public ICommand Examinar => _examinar;
        public CompilaViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conectar = new DelegateCommand(OnConection);
            _examinar = new DelegateCommand(pExaminar);

            handler = hand;
            model = new CompilaModel(handler, this);

            this.ListaObservaciones = new ObservableCollection<SelectListItem>();
            this.ListaArchivosCargados = new ObservableCollection<Archivo>();
            ListaUsuarios = handler.ListaUsuarios;

            try
            {
                this.ArchivosCompilados = model.pObtCantObjsInvalidos();
                this.ArchivosDescompilados = this.ArchivosCompilados;
            }
            catch(Exception ex)
            {
                handler.MensajeError(res.MensajeNoConexion + ". [" + ex.Message + "]");
                this.ArchivosCompilados = "0";
                this.ArchivosDescompilados = "0";
            }
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
        public void pCompilar()
        {
            model.pCompilarObjetos();
        }
        public void OnProcess(object commandParameter)
        {            
        }
        public void OnClean(object commandParameter)
        {
            try
            {
                model.pCleanView();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnConection(object commandParameter)
        {
            try
            {
                //se intenta realizar la conexión con la base de datos
                handler.pRealizaConexion();
                this.ArchivosCompilados = model.pObtCantObjsInvalidos();
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            this.EstadoConn = handler.fsbValidaConexion();
        }
        public void pExaminar(object commandParameter)
        {
            string[] archivos = handler.pCargarArchivos();
            pListaArchivos(archivos,"");
        }

        public void pListaArchivos(string[] DropPath,string from)
        {
            try
            {
                model.pListaArchivos(DropPath, from);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

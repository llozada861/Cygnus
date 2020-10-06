using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Documentation
{
    public class GeneratHtmlViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private ObservableCollection<SelectListItem> listaObservaciones;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<ParametrosModel> listaParametros;
        private String usuario;
        private String descripcion;
        private String wo;
        public GeneratHtmlViewModel(Handler handler)
        {
            this.handler = handler;
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);

            this.ListaObservaciones = new ObservableCollection<SelectListItem>();
            this.ListaArchivosCargados = new ObservableCollection<Archivo>();
            this.listaParametros = new ObservableCollection<ParametrosModel>();
        }
        public ICommand Process => _process;
        public ICommand Clean => _clean;
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
        public ObservableCollection<ParametrosModel> ListaParametros
        {
            get { return listaParametros; }
            set { SetProperty(ref listaParametros, value); }
        }
        public String Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public String Usuario
        {
            get { return usuario; }
            set { SetProperty(ref usuario, value); }
        }
        public String Wo
        {
            get { return wo; }
            set { SetProperty(ref wo, value); }
        }

        public void OnProcess(object commandParameter)
        {
            foreach(Archivo archivo in ListaArchivosCargados)
            {
                //Se instancian las listas del archivo
                archivo.DocumentacionSinDepurar = new List<StringBuilder>();
                archivo.Modificaciones = new List<ModificacionModel>();
                archivo.ListDocumentacionDepurada = new List<DocumentacionModel>();
                handler.ObtenerTipoArchivo(archivo, res.No_aplica);
                if (handler.pDepuraDocumentacion(archivo))
                {
                    pAdicionarArchivo
                    (
                        archivo.NombreObjeto + res.ExtensionHtml,
                        res.TipoHtml,
                        "Documentación HTML",
                        archivo.Ruta
                    );
                }
            }
        }

        public void OnClean(object commandParameter)
        {
            this.ListaArchivosCargados.Clear();
            this.ListaObservaciones.Clear();
        }

        public void OnConection(object commandParameter)
        {

        }
        public void pListaArchivos(string[] DropPath)
        {
            foreach (string dropfilepath in DropPath)
            {
                Archivo archivo = new Archivo();
                archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                archivo.RutaConArchivo = dropfilepath;
                archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                archivo.Extension = System.IO.Path.GetExtension(dropfilepath);
                archivo.BloquesCodigo = new List<string>();

                this.ListaArchivosCargados.Add(archivo);
            }
        }
        internal void pAdicionarArchivo(string archivoSalida, string tipo, string observacion, string ruta)
        {
            this.ListaObservaciones.Add(new SelectListItem { Text = archivoSalida });
        }
    }
}

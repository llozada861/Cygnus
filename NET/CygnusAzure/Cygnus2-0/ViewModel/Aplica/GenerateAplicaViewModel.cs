using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Aplica;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Aplica
{
    public class GenerateAplicaViewModel: ViewModelBase
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _sqlplus;
        private readonly DelegateCommand _examinar;
        private GenerateAplicaModel model;
        private string codigo;
        private SelectListItem _usuario;
        private string archivosGenerados;
        private string archivosCargados;
        private ObservableCollection<Archivo> listaArchivosGenerados;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<SelectListItem> listaUsuarios;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Sqlplus => _sqlplus;
        public ICommand Examinar => _examinar;

        public GenerateAplicaViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _sqlplus = new DelegateCommand(OnSqlplus);
            _examinar = new DelegateCommand(pExaminar);

            handler = hand;
            model = new GenerateAplicaModel(handler,this);

            listaArchivosGenerados = new ObservableCollection<Archivo>();
            listaArchivosCargados = new ObservableCollection<Archivo>();
            ListaUsuarios = handler.ListaUsuarios;
        }
        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
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
        public void OnProcess(object commandParameter)
        {
            bool Noselec = false;

            if (String.IsNullOrEmpty(this.Codigo))
            {
                handler.MensajeError("Ingrese número de caso");
                return;
            }

            if (this.Usuario == null)
            {
                handler.MensajeError("Debe ingresar el usuario para la entrega.");
                return;
            }

            if (this.ListaArchivosCargados.Count == 0)
            {
                handler.MensajeError("Se deben tener archivos para procesar.");
                return;
            }

            foreach (Archivo archivo in this.ListaArchivosCargados)
            {
                if (string.IsNullOrEmpty(archivo.Tipo))
                {
                    Noselec = true;
                }
            }

            if (Noselec)
            {
                handler.MensajeError("Todos los archivos deben tener un tipo. Seleccione un tipo para el archivo.");
                return;
            }

            model.pProcesar();

            if (handler.MensajeConfirmacion("Desea ejecutar el aplica por SQLPLUS?") == "Y")
            {
                this.OnSqlplus(null);
            }
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
        public void OnSqlplus(object commandParameter)
        {
            try
            {
                model.pEjecutaSqlplus();
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pListaArchivos(string[] DropPath)
        {
            try
            {
                model.pListaArchivos(DropPath);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pRefrescaConteo()
        {
            model.pRefrescaConteo();
        }
        public void pExaminar(object commandParameter)
        {
            string[] archivos = handler.pCargarArchivos();
            //model.pListaArchivos(archivos);
            ListarArchivos(archivos);
        }
        public void ListarArchivos(string[] DropPath)
        {
            List<Archivo> archivos = new List<Archivo>();
            handler.pListaArchivos(DropPath, archivos, res.No_aplica);

            foreach (Archivo archivo in archivos)
            {
                this.ListaArchivosCargados.Add(archivo);
            }
        }
    }
}

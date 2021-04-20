using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Documentation;
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
    public class GeneratHtmlViewModel: IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;

        public GeneratHtmlViewModel(Handler handler)
        {
            this.handler = handler;
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);

            this.Model = new HtmlModel();

            this.Model.ListaObservaciones = new ObservableCollection<SelectListItem>();
            this.Model.ListaArchivosCargados = new ObservableCollection<Archivo>();
            this.Model.ListaParametros = new ObservableCollection<ParametrosModel>();
        }
        public HtmlModel Model { get; set; }
        public ICommand Process => _process;
        public ICommand Clean => _clean;

        public void OnProcess(object commandParameter)
        {
            try
            {
                handler.pGeneraArchivoHtml(this.Model.ListaArchivosCargados, this.Model.ListaObservaciones);
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void OnClean(object commandParameter)
        {
            this.Model.ListaArchivosCargados.Clear();
            this.Model.ListaObservaciones.Clear();
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

                this.Model.ListaArchivosCargados.Add(archivo);
            }
        }
    }
}

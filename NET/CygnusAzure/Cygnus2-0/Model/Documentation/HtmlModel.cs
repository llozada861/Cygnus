using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Documentation
{
    public class HtmlModel: ViewModelBase
    {
        private ObservableCollection<SelectListItem> listaObservaciones;
        private ObservableCollection<Archivo> listaArchivosCargados;
        private ObservableCollection<ParametrosModel> listaParametros;
        private String usuario;
        private String descripcion;
        private String wo;

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
    }
}

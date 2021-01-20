using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Threats
{
    public class ThreatsModel: ViewModelBase
    {
        private string nombre;
        private string hilos;
        private string parametro;
        private string tipo;
        private string vistaPrevia;
        private string descripcion;
        private ObservableCollection<SelectListItem> listaParametros;

        public ObservableCollection<SelectListItem> ListaParametros
        {
            get { return listaParametros; }
            set { SetProperty(ref listaParametros, value); }
        }
        public string Nombre
        {
            get { return nombre; }
            set { SetProperty(ref nombre, value); }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public string Hilos
        {
            get { return hilos; }
            set { SetProperty(ref hilos, value); }
        }
        public string Parametro
        {
            get { return parametro; }
            set { SetProperty(ref parametro, value); }
        }
        public string Tipo
        {
            get { return tipo; }
            set { SetProperty(ref tipo, value); }
        }
        public string VistaPrevia
        {
            get { return vistaPrevia; }
            set { SetProperty(ref vistaPrevia, value); }
        }
        public Boolean ApiPre { get; set; }
        public Boolean ApiPost { get; set; }
        public Boolean ApiCantidad { get; set; }
    }
}

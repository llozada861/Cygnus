using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Data
{
    public class ParameterModel: ViewModelBase
    {
        private string parameterId;
        private string descripcion;
        private string valor;
        private string funcion;
        private string tittle;
        private List<SelectListItem> listaTipoDatos;
        private SelectListItem tipo;

        public string Tittle
        {
            get { return tittle; }
            set { SetProperty(ref tittle, value); }
        }

        public string ParameterId
        {
            get { return parameterId; }
            set { SetProperty(ref parameterId, value); }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public string Valor
        {
            get { return valor; }
            set { SetProperty(ref valor, value); }
        }
        public SelectListItem Tipo
        {
            get { return tipo; }
            set { SetProperty(ref tipo, value); }
        }
        public string Funcion
        {
            get { return funcion; }
            set { SetProperty(ref funcion, value); }
        }
        public List<SelectListItem> ListaTipoDatos
        {
            get { return listaTipoDatos; }
            set { SetProperty(ref listaTipoDatos, value); }
        }
    }
}

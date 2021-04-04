using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Estandar
{
    public class EstandarModel : ViewModelBase
    {
        ObservableCollection<SelectListItem> listaEstandar;
        SelectListItem _estandar;
        public EstandarModel()
        {
            Estandar = new SelectListItem();
            ListaEstandar = new ObservableCollection<SelectListItem>();
        }
        public SelectListItem Estandar
        {
            get { return _estandar; }
            set { SetProperty(ref _estandar, value); }
        }
        public ObservableCollection<SelectListItem> ListaEstandar
        {
            get { return listaEstandar; }
            set { SetProperty(ref listaEstandar, value); }
        }
    }
}

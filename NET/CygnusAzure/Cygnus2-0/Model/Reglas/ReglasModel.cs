using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Reglas
{
    public class ReglasModel: ViewModelBase
    {
        ObservableCollection<UsuariosPDN> listaBD;
        ObservableCollection<SelectListItem> listaReglas;
        public UsuariosPDN BdSeleccionada { get; set; }

        public ObservableCollection<UsuariosPDN> ListaBD
        {
            get { return listaBD; }
            set
            {
                SetProperty(ref listaBD, value);
            }
        }
        public ObservableCollection<SelectListItem> ListaReglas
        {
            get { return listaReglas; }
            set
            {
                SetProperty(ref listaReglas, value);
            }
        }
    }
}

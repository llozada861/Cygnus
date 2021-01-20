using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Package
{
    public class pkgModel: ViewModelBase
    {
        private string tabla;
        private string caso;
        private SelectListItem usuario;
        private ObservableCollection<SelectListItem> listaUsuarios;

        public string Tabla
        {
            get { return tabla; }
            set { SetProperty(ref tabla, value); }
        }
        public string Caso
        {
            get { return caso; }
            set { SetProperty(ref caso, value); }
        }
        public SelectListItem Usuario
        {
            get { return usuario; }
            set { SetProperty(ref usuario, value); }
        }
        public ObservableCollection<SelectListItem> ListaUsuarios
        {
            get { return listaUsuarios; }
            set { SetProperty(ref listaUsuarios, value); }
        }
    }
}

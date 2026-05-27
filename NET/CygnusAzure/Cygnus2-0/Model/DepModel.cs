using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Azure;
using Cygnus2_0.Model.User;
using Microsoft.TeamFoundation.Build.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model
{
    public class DepModel: ViewModelBase
    {
        private ObservableCollection<Dependencia> lista;
        private string objeto;
        private string instancia;
        ObservableCollection<UsuariosPDN> listaBD;

        public DepModel() { }
        public string Objeto
        {
            get { return objeto; }
            set { SetProperty(ref objeto, value); }
        }
        public UsuariosPDN BdSeleccionada { get; set; }
        public ObservableCollection<UsuariosPDN> ListaBD
        {
            get { return listaBD; }
            set
            {
                SetProperty(ref listaBD, value);
            }
        }
        public ObservableCollection<Dependencia> ListaDependencias
        {
            get { return lista; }
            set { SetProperty(ref lista, value); }
        }
    }
}

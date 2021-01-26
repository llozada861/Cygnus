using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Security
{
    public class RoleModel: ViewModelBase
    {
        private string usuario;
        private string email;
        private List<SelectListItem> listaRoles;
        private SelectListItem rol;
        private string password;
        public RoleModel()
        {
        }

        public List<SelectListItem> ListaRoles
        {
            get { return listaRoles; }
            set { SetProperty(ref listaRoles, value); }
        }
        public SelectListItem Rol
        {
            get { return rol; }
            set { SetProperty(ref rol, value); }
        }
        public string Usuario
        {
            get { return usuario; }
            set { SetProperty(ref usuario, value.ToUpper().Trim()); }
        }
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
    }
}

using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Security
{
    public class RoleModel
    {
        private Handler handler;
        private RoleViewModel view;
        public RoleModel(Handler hand, RoleViewModel view)
        {
            this.view = view;
            this.handler = hand;
        }

        public void pGuardaRol()
        {
            string pass = view.Usuario.Trim()+"-"+view.Rol.Value;
            handler.DAO.pGuardaRol(view.Usuario.Trim(), EncriptaPass.Encriptar(pass), view.Email);
            SqliteDAO.pCreaConfiguracion(res.KeyEmail, view.Email);
            handler.ConnViewModel.Correo = view.Email;
        }

        public void pLimpiar()
        {
            view.Rol = new SelectListItem();
            view.Usuario = "";
            view.ListaRoles = null;
            view.ListaRoles = handler.ListaRoles;
            view.Email = "";
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Security
{
    public class RoleViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private List<SelectListItem> listaUsuarios;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private string usuario;
        private string email;
        private List<SelectListItem> listaRoles;
        private SelectListItem rol;
        private RoleModel model;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public RoleViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            
            handler = hand;
            model = new RoleModel(handler, this);

            handler.ListaRoles = new List<SelectListItem>();
            handler.ListaRoles.Add(new SelectListItem { Text = "Especialista", Value = "1" });
            handler.ListaRoles.Add(new SelectListItem { Text = "Usuario", Value = "0" });

            OnClean("");
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
            set { SetProperty(ref usuario, value); }
        }
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }
        public void OnClean(object commandParameter)
        {            
            try
            {
                model.pLimpiar();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Usuario))
                {
                    handler.MensajeError("Debe ingresar el usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Rol.Text))
                {
                    handler.MensajeError("Debe ingresar el rol.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Email))
                {
                    handler.MensajeError("Debe ingresar un Email.");
                    return;
                }

                model.pGuardaRol();
                handler.MensajeOk("Rol asignado con éxito.");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Security;
using Cygnus2_0.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Security
{
    public class EncryptViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private SelectListItem usuario;
        private string pass;
        private string bd;
        private ObservableCollection<SelectListItem> listaUsuarios;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private EncryptModel model;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public EncryptViewModel(Handler hand)
        {
            this.handler = hand;
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);

            model = new EncryptModel(handler, this);

            ListaUsuarios = handler.pObtlistaUsuarios();
        }
        public SelectListItem Usuario
        {
            get { return usuario; }
            set { SetProperty(ref usuario, value); }
        }
        public string Pass
        {
            get { return pass; }
            set { SetProperty(ref pass, value); }
        }
        public string BD
        {
            get { return bd; }
            set { SetProperty(ref bd, value); }
        }
        public ObservableCollection<SelectListItem> ListaUsuarios
        {
            get { return listaUsuarios; }
            set { SetProperty(ref listaUsuarios, value); }
        }
        public void OnClean(object commandParameter)
        {
            ListaUsuarios = null;
            ListaUsuarios = handler.pObtlistaUsuarios();
            Pass = "";
            BD = "";
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if(string.IsNullOrEmpty(this.Usuario.Text))
                {
                    handler.MensajeError("Debe ingresar el usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Pass))
                {
                    handler.MensajeError("Debe ingresar la contraseña.");
                    return;
                }

                if (string.IsNullOrEmpty(this.BD))
                {
                    handler.MensajeError("Debe ingresar la base de datos.");
                    return;
                }

                model.pGuardaPass();
                handler.MensajeOk("Contraseña Guardada con Éxito");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

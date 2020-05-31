using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Security;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Settings
{
    public class ConexionViewModel:ViewModelBase
    {
        private string _usuario;
        private string _pass;
        private string _basedatos;
        private string _puerto;
        private string _servidor;
        private string _usuariocompila;
        private string _passcompila;
        private Handler handler;
        private ConexionModel conexionModel;
        private string correo;
        private string usuarioAzure;
        private string urlAzure;
        ObservableCollection<SelectListItem> listaAreaAzure;

        private readonly DelegateCommand _process;
        public readonly DelegateCommand _test;
        public ICommand Process => _process;        
        public ICommand Test => _test;

        public ConexionViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _test = new DelegateCommand(OnTest);
            handler = hand;
            conexionModel = new ConexionModel(handler);
            UrlAzure = "https://grupoepm.visualstudio.com";
        }

        public string Usuario
        {
            get { return _usuario; }
            set { SetProperty(ref _usuario, value); } 
        }

        public string Pass
        {
            get { return _pass; }
            set { SetProperty(ref _pass, value); }
        }
        public string BaseDatos
        {
            get { return _basedatos; }
            set { SetProperty(ref _basedatos, value); }
        }
        public string Puerto
        {
            get { return _puerto; }
            set { SetProperty(ref _puerto, value); }
        }
        public string Servidor
        {
            get { return _servidor; }
            set { SetProperty(ref _servidor, value); }
        }
        public string UsuarioCompila
        {
            get { return _usuariocompila; }
            set { SetProperty(ref _usuariocompila, value); }
        }
        public string PassCompila
        {
            get { return _passcompila; }
            set { SetProperty(ref _passcompila, value); }
        }
        public string Correo
        {
            get { return correo; }
            set { SetProperty(ref correo, value); }
        }
        public string UsuarioAzure
        {
            get { return usuarioAzure; }
            set { SetProperty(ref usuarioAzure, value); }
        }
        public string UrlAzure
        {
            get { return urlAzure; }
            set { SetProperty(ref urlAzure, value); }
        }

        public ObservableCollection<SelectListItem> ListaAreaAzure
        {
            get { return listaAreaAzure; }
            set { SetProperty(ref listaAreaAzure, value); }
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                this.Pass = ((PasswordBox)commandParameter).Password;

                if (string.IsNullOrEmpty(Usuario))
                {
                    handler.MensajeError("Ingrese una usuario.");
                    return;
                }
                
                if (string.IsNullOrEmpty(this.Pass))
                {
                    handler.MensajeError("Ingrese una contraseña.");
                    return;
                }

                if (string.IsNullOrEmpty(BaseDatos))
                {
                    handler.MensajeError("Ingrese una usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(Puerto))
                {
                    handler.MensajeError("Ingrese un puerto.");
                    return;
                }

                if (string.IsNullOrEmpty(Servidor))
                {
                    handler.MensajeError("Ingrese un servidor.");
                    return;
                }

                this.conexionModel.SaveData();
                handler.MensajeOk("Conexión éxitosa.");
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void OnTest(object commandParameter)
        {
            try
            {
                this.Pass = ((PasswordBox)commandParameter).Password;
                this.conexionModel.TestConection();
                handler.MensajeOk("Conexión éxitosa.");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        } 
    }
}

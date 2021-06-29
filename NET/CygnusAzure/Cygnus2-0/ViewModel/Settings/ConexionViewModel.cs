using Cygnus2_0.DAO;
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
using System.Windows;
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
        ObservableCollection<SelectListItem> listaConexiones;
        private SelectListItem conexion;

        private readonly DelegateCommand _process;
        public readonly DelegateCommand _test;
        public readonly DelegateCommand _eliminar;
        public ICommand Process => _process;        
        public ICommand Test => _test;
        public ICommand Eliminar => _eliminar;

        public ConexionViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _test = new DelegateCommand(OnTest);
            _eliminar = new DelegateCommand(OnEliminar);
            handler = hand;
            conexionModel = new ConexionModel(handler);
            UrlAzure = "https://grupoepm.visualstudio.com";
            ListaConexiones = new ObservableCollection<SelectListItem>();
            Conexion = new SelectListItem();
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
        public string BdCompila{ set; get; }
        
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
        public ObservableCollection<SelectListItem> ListaConexiones
        {
            get { return listaConexiones; }
            set { SetProperty(ref listaConexiones, value); }
        }
        public SelectListItem Conexion
        {
            get { return conexion; }
            set { SetProperty(ref conexion, value);}
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                this.Pass = ((PasswordBox)commandParameter).Password;

                if (string.IsNullOrEmpty(Conexion.Usuario))
                {
                    handler.MensajeError("Ingrese un usuario.");
                    return;
                }
                
                if (string.IsNullOrEmpty(this.Pass))
                {
                    handler.MensajeError("Ingrese una contraseña.");
                    return;
                }

                if (string.IsNullOrEmpty(Conexion.Bd))
                {
                    handler.MensajeError("Ingrese una base de datos.");
                    return;
                }

                if (string.IsNullOrEmpty(Conexion.Puerto))
                {
                    handler.MensajeError("Ingrese un puerto.");
                    return;
                }

                if (string.IsNullOrEmpty(Conexion.Servidor))
                {
                    handler.MensajeError("Ingrese un servidor.");
                    return;
                }

                handler.CursorWait();
                this.conexionModel.SaveData(this.Pass);
                SqliteDAO.pDatosBd(handler, Conexion);
                handler.CursorNormal();
                var myWin = (MainWindow)Application.Current.MainWindow;
                myWin.pVersion();
                handler.MensajeOk("Conexión éxitosa.");
                onClean();
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
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

        public void OnEliminar(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Conexion.Usuario))
                {
                    handler.MensajeError("Ingrese un usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(Conexion.Bd))
                {
                    handler.MensajeError("Ingrese una base de datos.");
                    return;
                }

                if(handler.MensajeConfirmacion("Seguro que desea eliminar la conexión ["+ Conexion.Usuario+" - "+ Conexion.Bd+"]?") == "Y")
                {
                    handler.CursorWait();
                    SqliteDAO.pEliminarConexion(Conexion);
                    SqliteDAO.pDatosBd(handler, null);
                    handler.CursorNormal();
                    handler.MensajeOk("Proceso terminó con éxito.");
                }
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void onClean()
        {
            handler.ConnViewModel.Conexion.Usuario = "";
            handler.ConnViewModel.Conexion.Servidor = "";
            handler.ConnViewModel.Conexion.Bd = "";
            handler.ConnViewModel.Conexion.BlValor = false;
            handler.ConnViewModel.Conexion.Puerto = "";
            handler.ConnViewModel.Conexion.Pass = "";
        }
    }
}

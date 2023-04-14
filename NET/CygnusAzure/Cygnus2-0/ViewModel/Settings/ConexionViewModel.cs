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
    public class ConexionViewModel
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        public readonly DelegateCommand _test;
        public readonly DelegateCommand _eliminar;
        private readonly DelegateCommand _nuevo;
        public ICommand Process => _process;        
        public ICommand Test => _test;
        public ICommand Eliminar => _eliminar;
        public ICommand Nuevo => _nuevo;

        public ConexionViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _test = new DelegateCommand(OnTest);
            _eliminar = new DelegateCommand(OnEliminar);
            _nuevo = new DelegateCommand(OnNuevo);
            handler = hand;
            this.Model = new ConexionModel();
            this.Model.ListaConexiones = new ObservableCollection<SelectListItem>();
            this.Model.Conexion = new SelectListItem();
        }

        public ConexionModel Model { set; get; }

        public void OnProcess(object commandParameter)
        {
            try
            {
                this.Model.Pass = ((PasswordBox)commandParameter).Password;

                if (string.IsNullOrEmpty(this.Model.Conexion.Etiqueta))
                {
                    handler.MensajeError("Ingrese una etiqueta para la conexión.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Conexion.Usuario))
                {
                    handler.MensajeError("Ingrese un usuario.");
                    return;
                }
                
                if (string.IsNullOrEmpty(this.Model.Pass))
                {
                    handler.MensajeError("Ingrese una contraseña.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Conexion.Bd))
                {
                    handler.MensajeError("Ingrese una base de datos.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Conexion.Puerto))
                {
                    handler.MensajeError("Ingrese un puerto.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Conexion.Servidor))
                {
                    handler.MensajeError("Ingrese un servidor.");
                    return;
                }

                handler.CursorWait();

                SqliteDAO.pCreaConexion(handler, this.Model.Pass);
                handler.pRealizaConexion();

                SqliteDAO.pDatosBd(handler, this.Model.Conexion);
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
        }

        public void OnEliminar(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Model.Conexion.Usuario))
                {
                    handler.MensajeError("Ingrese un usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Conexion.Bd))
                {
                    handler.MensajeError("Ingrese una base de datos.");
                    return;
                }

                if(handler.MensajeConfirmacion("Seguro que desea eliminar la conexión [ "+ this.Model.Conexion.Etiqueta+" ]?") == "Y")
                {
                    handler.CursorWait();
                    SqliteDAO.pEliminarConexion(this.Model.Conexion);
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
            Model.Conexion.Usuario = "";
            Model.Conexion.Servidor = "";
            Model.Conexion.Bd = "";
            Model.Conexion.BlValor = false;
            Model.Conexion.Puerto = "";
            Model.Conexion.Pass = "";
            Model.Conexion.Etiqueta = "";
        }

        public void OnNuevo(object commandParameter)
        {
            onClean();
        }
    }
}

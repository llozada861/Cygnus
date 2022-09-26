using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cygnus2_0.Pages.Settings.Database
{
    /// <summary>
    /// Interaction logic for UserBDUserControl.xaml
    /// </summary>
    public partial class UserBDUserControl : UserControl
    {
        private Handler handler;
        public UserBDUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            handler.ListaUsuarios.Add(new UsuarioModel() { Empresa = handler.ConfGeneralView.Model.Empresa.Codigo });
        }

        private void btnGuar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (UsuarioModel item in handler.ListaUsuarios)
                {
                    if (String.IsNullOrEmpty(item.Usuariobd))
                    {
                        handler.MensajeError("Debe ingresar un usuario.");
                        return;
                    }

                    if (SqliteDAO.pExisteUsuario(item))
                    {
                        SqliteDAO.pActualizaObjeto(item);
                    }
                    else
                    {
                        SqliteDAO.pGuardarUsuarioBD(item);
                    }
                }

                pCargarLista();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            btnGuar.Visibility = Visibility.Hidden;
        }

        private void btnElim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (handler.UsuarioSeleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar el usuario o rol [" + handler.UsuarioSeleccionado.Usuariobd + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaObjeto(handler.UsuarioSeleccionado);
                    handler.ListaUsuarios.Remove(handler.UsuarioSeleccionado);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            btnGuar.Visibility = Visibility.Hidden;
        }
        public void pCargarLista()
        {
            handler.ListaUsuarios.Clear();
            SqliteDAO.pListaUsuarios(handler);
        }
        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnGuar.Visibility = Visibility.Visible;
        }
    }
}

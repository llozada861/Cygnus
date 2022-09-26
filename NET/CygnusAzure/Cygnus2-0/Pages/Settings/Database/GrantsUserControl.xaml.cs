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
    /// Interaction logic for GrantsUserControl.xaml
    /// </summary>
    public partial class GrantsUserControl : UserControl
    {
        private Handler handler;
        public GrantsUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }

        public void pCargarLista()
        {
            handler.ListaUsGrants.Clear();
            SqliteDAO.pListaUsGrants(handler);
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnGuar.Visibility = Visibility.Visible;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            handler.ListaUsGrants.Add(new SelectListItem() { Empresa = handler.ConfGeneralView.Model.Empresa.Codigo });
        }

        private void btnGuar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (SelectListItem item in handler.ListaUsGrants)
                {
                    if (String.IsNullOrEmpty(item.Text))
                    {
                        handler.MensajeError("Debe ingresar un usuario o rol.");
                        return;
                    }

                    var objeto = new GrantsModel();

                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        objeto = new GrantsModel() { Codigo = Int32.Parse(item.Value), Usuario = item.Text, Empresa = item.Empresa };
                        SqliteDAO.pActualizaObjeto(objeto);
                    }
                    else
                    {
                        objeto = new GrantsModel() { Usuario = item.Text, Empresa = item.Empresa };
                        SqliteDAO.pGuardarUsuarioGrant(objeto);
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            SqliteDAO.pListaUsGrants(handler);
            btnGuar.Visibility = Visibility.Hidden;
        }

        private void btnElim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (handler.Generico != null && handler.MensajeConfirmacion("Seguro que desea eliminar el usuario o rol [" + handler.Generico.Text + "] ?") == "Y")
                {
                    var objeto = new GrantsModel() { Codigo = Int32.Parse(handler.Generico.Value), Usuario = handler.Generico.Text };
                    SqliteDAO.pEliminaObjeto(objeto);
                    handler.ListaUsGrants.Remove(handler.Generico);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            btnGuar.Visibility = Visibility.Hidden;
        }
    }
}

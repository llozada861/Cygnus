using Cygnus2_0.DAO;
using Cygnus2_0.General;
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

        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
                return;

            txtEliminar.Text = ((SelectListItem)dataGridDatos.SelectedItem).Text;
        }

        private void tbnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string palabra = txtUser.Text.Trim().ToUpper();

            if (String.IsNullOrEmpty(palabra))
            {
                handler.MensajeError("Debe ingresar un usuario.");
                return;
            }

            try
            {
                SqliteDAO.pGuardarUsuarioBD(palabra);
                pCargarLista();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            string sbPalabra = txtEliminar.Text.Trim();

            try
            {
                if (!String.IsNullOrEmpty(sbPalabra))
                {
                    if (handler.MensajeConfirmacion("Está seguro que desea borrar el usuario [" + sbPalabra + "]") == "Y")
                    {
                        SqliteDAO.pEliminaUsuarioBD(sbPalabra);
                        pCargarLista();
                        txtEliminar.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pCargarLista()
        {
            handler.ListaUsuarios.Clear();
            SqliteDAO.pListaUsuarios(handler);
        }
    }
}

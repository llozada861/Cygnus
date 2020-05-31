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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Settings.General
{
    /// <summary>
    /// Interaction logic for ObjectTypeUserControl.xaml
    /// </summary>
    public partial class ObjectTypeUserControl : UserControl
    {
        private Handler handler;
        public ObjectTypeUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
            txtCantidad.Text = "0";
        }

        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
                return;

            txtEliminar.Text = ((SelectListItem)dataGridDatos.SelectedItem).Text;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string objeto = txtObject.Text.Trim().ToLower(); ;
            string slash = handler.Generico.Value;
            string cantidad = txtCantidad.Text;
            string prioridad = txtPriodad.Text;
            string permiso = handler.Generico2.Value;

            if (String.IsNullOrEmpty(objeto))
            {
                handler.MensajeError("Debe ingresar un tipo de objeto.");
                return;
            }

            if (String.IsNullOrEmpty(slash))
            {
                handler.MensajeError("Debe definir si el objeto lleva slash al final.");
                return;
            }

            if (String.IsNullOrEmpty(cantidad) && slash.Equals(res.Si))
            {
                handler.MensajeError("Debe ingresar una cantidad.");
                return;
            }
            else if(slash.Equals(res.Si))
            {
                try
                {
                    if (Convert.ToInt32(cantidad) == 0)
                    {
                        handler.MensajeError("La cantidad debe ser mayor que cero.");
                        return;
                    }
                }
                catch
                {
                    handler.MensajeError("La cantidad debe ser numérica.");
                    return;
                }
            }

            if (String.IsNullOrEmpty(prioridad))
            {
                handler.MensajeError("Debe ingresar una prioridad.");
                return;
            }

            try
            {
                int prioridad_ = Convert.ToInt32(prioridad);
            }
            catch
            {
                handler.MensajeError("La prioridad debe ser numérica.");
                return;
            }

            if (String.IsNullOrEmpty(permiso))
            {
                handler.MensajeError("Debe ingresar un permiso para el objeto.");
                return;
            }

            try
            {
                SqliteDAO.pGuardarTipoObjeto(objeto, slash, cantidad, prioridad, permiso);
                pCargarLista();
            }
            catch (Exception ex)
            {
                handler.MensajeOk(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            string sbPalabra = txtEliminar.Text.Trim();

            try
            {
                if (!String.IsNullOrEmpty(sbPalabra))
                {
                    if (handler.MensajeConfirmacion("Está seguro que desea borrar el tipo de objeto [" + sbPalabra + "]") == "Y")
                    {
                        SqliteDAO.pEliminaTipoObjeto(sbPalabra);
                        pCargarLista();
                        txtEliminar.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeOk(ex.Message);
            }
        }

        public void pCargarLista()
        {
            handler.ListaTiposObjetos.Clear();
            SqliteDAO.pListaTiposObjetos(handler);
        }
    }
}

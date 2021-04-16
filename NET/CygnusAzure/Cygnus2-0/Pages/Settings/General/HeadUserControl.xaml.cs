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

namespace Cygnus2_0.Pages.Settings.General
{
    /// <summary>
    /// Interaction logic for EncabezadosUserControl.xaml
    /// </summary>
    public partial class EncabezadosUserControl : UserControl
    {
        private Handler handler;
        public EncabezadosUserControl()
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

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            string sbPalabra = txtEliminar.Text.Trim();

            try
            {
                if (!String.IsNullOrEmpty(sbPalabra))
                {
                    if (handler.MensajeConfirmacion("Está seguro que desea borrar el encabezado [" + sbPalabra + "]") == "Y")
                    {
                        SqliteDAO.pEliminaEncabezado(sbPalabra);
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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string sbEncabezado = txtHead.Text.Trim().ToLower();
            string tipo = handler.Generico.Text;
            string prioridad = txtPriodad.Text;
            string fin = handler.Generico2.Value;

            if(String.IsNullOrEmpty(sbEncabezado))
            {
                handler.MensajeError("Debe ingresar un encabezado.");
                return;
            }

            if (String.IsNullOrEmpty(tipo))
            {
                handler.MensajeError("Debe ingresar un tipo.");
                return;
            }

            if (String.IsNullOrEmpty(fin))
            {
                handler.MensajeError("Debe ingresar una etiqueta de fin.");
                return;
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

            try
            {
                SqliteDAO.pGuardarEncabezado(sbEncabezado, tipo, prioridad, fin,handler);
                pCargarLista();
            }
            catch (Exception ex)
            {
                handler.MensajeOk(ex.Message);
            }
        }

        public void pCargarLista()
        {
            handler.ListaEncabezadoObjetos.Clear();
            SqliteDAO.pListaEncabezadoObjetos(handler);
        }
    }
}

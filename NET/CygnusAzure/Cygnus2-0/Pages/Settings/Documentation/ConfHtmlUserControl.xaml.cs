using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
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

namespace Cygnus2_0.Pages.Settings.Documentation
{
    /// <summary>
    /// Interaction logic for ConfHtmlUserControl.xaml
    /// </summary>
    public partial class ConfHtmlUserControl : UserControl
    {
        private Handler handler;
        public ConfHtmlUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }

        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
                return;

            txtEliminar.Text = ((DocumentacionHTML)dataGridDatos.SelectedItem).TagInicio;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string inicio = txtTagIni.Text.Trim().ToLower();
            string fin = txtTagFin.Text.Trim().ToLower();
            string atributos = txtAtributos.Text.Trim();
            string finEnca = txtFinEnca.Text.Trim();
            string tipo = handler.Generico.Value;

            if (String.IsNullOrEmpty(inicio))
            {
                handler.MensajeError("Debe ingresar un tag de inicio.");
                return;
            }
            if (String.IsNullOrEmpty(fin))
            {
                handler.MensajeError("Debe ingresar un tag de fin.");
                return;
            }

            if (String.IsNullOrEmpty(tipo))
            {
                handler.MensajeError("Debe seleccionar un tipo.");
                return;
            }

            try
            {
                SqliteDAO.pGuardarConfHtml(inicio, fin, atributos, finEnca, tipo,handler);
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
                    if (handler.MensajeConfirmacion("Está seguro que desea borrar la documentación [" + sbPalabra + "]") == "Y")
                    {
                        SqliteDAO.pEliminaConfHtml(sbPalabra,handler);
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
            handler.ListaDocHtml.Clear();
            SqliteDAO.pDocumentacionHtml(handler);
        }
    }
}

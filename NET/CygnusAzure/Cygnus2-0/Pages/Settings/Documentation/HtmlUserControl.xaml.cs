using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Model.Html;
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
    /// Interaction logic for HtmlUserControl.xaml
    /// </summary>
    public partial class HtmlUserControl : UserControl
    {
        private Handler handler;
        public HtmlUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }

        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
                return;

            txtName.Text = ((PlantillasHTMLModel)dataGridDatos.SelectedItem).Nombre;
            txtFileName.Text = ((PlantillasHTMLModel)dataGridDatos.SelectedItem).NombreArchivo;

            richTextBoxProce.Document.Blocks.Clear();
            richTextBoxProce.Document.Blocks.Add(new Paragraph(new Run(((PlantillasHTMLModel)dataGridDatos.SelectedItem).Documentacion)));
            txtName.IsEnabled = false;
        }

        private void btnModif_Click(object sender, RoutedEventArgs e)
        {
            int? nuEmpresa;
            string key = txtName.Text;
            string value = new TextRange(richTextBoxProce.Document.ContentStart, richTextBoxProce.Document.ContentEnd).Text;
            string fileName = txtFileName.Text;

            if (String.IsNullOrEmpty(key))
            {
                handler.MensajeError("El tag no puede ser null");
                return;
            }

            PlantillasHTMLModel seleccionado = ((PlantillasHTMLModel)dataGridDatos.SelectedItem);

            if(seleccionado != null)
            {
                nuEmpresa = seleccionado.Empresa;
            }
            else
            {
                nuEmpresa = handler.ConfGeneralView.Model.Empresa.Codigo;
            }

            try
            {
                seleccionado.Documentacion = value;
                seleccionado.NombreArchivo = fileName;
                SqliteDAO.pActualizaObjeto(seleccionado);                
            }
            catch (Exception ex)
            {
                PlantillasHTMLModel nuevo = new PlantillasHTMLModel();
                nuevo.Nombre = key;
                nuevo.Documentacion = value;
                nuevo.NombreArchivo = fileName;
                nuevo.Empresa = nuEmpresa;
                SqliteDAO.pInsertaPlantilla(nuevo);
            }

            pCargarLista();
            richTextBoxProce.Document.Blocks.Clear();
            txtName.Text = "";
            txtName.IsEnabled = false;
        }
    
        public void pCargarLista()
        {
            handler.ListaHTML.Clear();
            SqliteDAO.pListaHTML(handler);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            PlantillasHTMLModel seleccionado = ((PlantillasHTMLModel)dataGridDatos.SelectedItem);

            if(seleccionado != null && handler.MensajeConfirmacion("Desea eliminar la plantilla ["+seleccionado.Nombre+"]") == "Y")
            {
                SqliteDAO.pEliminaObjeto(seleccionado);
                pCargarLista();
                richTextBoxProce.Document.Blocks.Clear();
                txtName.Text = "";
                txtName.IsEnabled = true;
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            richTextBoxProce.Document.Blocks.Clear();
            txtName.Text = "";
            txtName.IsEnabled = true;
            txtFileName.Text = "";
            dataGridDatos.SelectedItem = null;
        }
    }
}

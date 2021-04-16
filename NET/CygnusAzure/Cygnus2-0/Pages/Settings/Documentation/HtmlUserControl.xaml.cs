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

            txtName.Text = ((SelectListItem)dataGridDatos.SelectedItem).Text;

            richTextBoxProce.Document.Blocks.Clear();
            richTextBoxProce.Document.Blocks.Add(new Paragraph(new Run(((SelectListItem)dataGridDatos.SelectedItem).Value)));
        }

        private void btnModif_Click(object sender, RoutedEventArgs e)
        {
            string key = txtName.Text;
            string value = new TextRange(richTextBoxProce.Document.ContentStart, richTextBoxProce.Document.ContentEnd).Text;

            if(String.IsNullOrEmpty(key))
            {
                handler.MensajeError("Debe seleccionar un ítem de la grilla.");
                return;
            }

            try
            {
                SqliteDAO.pActualizaHtml(key, value,handler);
                pCargarLista();

                richTextBoxProce.Document.Blocks.Clear();
                txtName.Text = "";
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    
        public void pCargarLista()
        {
            handler.ListaHTML.Clear();
            SqliteDAO.pListaHTML(handler);
        }
    }
}

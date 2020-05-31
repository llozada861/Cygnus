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
    /// Interaction logic for PathsUserControl.xaml
    /// </summary>
    public partial class PathsUserControl : UserControl
    {
        private Handler handler;
        public PathsUserControl()
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
            txtPath.Text = ((SelectListItem)dataGridDatos.SelectedItem).Value;
        }

        private void btnModif_Click(object sender, RoutedEventArgs e)
        {
            string key = txtName.Text;
            string ruta = txtPath.Text;

            if(String.IsNullOrEmpty(ruta))
            {
                handler.MensajeError("Ingrese una ruta valida.");
                return;
            }

            try
            {
                SqliteDAO.pActualizaRuta(key,ruta);
                pCargarLista();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void pCargarLista()
        {
            handler.ListaRutas.Clear();
            SqliteDAO.pListaRutas(handler);
            handler.ConfGeneralViewModel.RutaSqlplus = handler.ListaRutas.ToList().Find(x => x.Text.Equals(res.SQLPLUS)).Value;
        }
    }
}

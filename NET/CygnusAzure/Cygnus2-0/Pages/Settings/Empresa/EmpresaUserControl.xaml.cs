using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Settings;
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

namespace Cygnus2_0.Pages.Settings.Empresa
{
    /// <summary>
    /// Lógica de interacción para EmpresaUserControl.xaml
    /// </summary>
    public partial class EmpresaUserControl : UserControl
    {
        private const string cstAdicionar = "Adicionar";
        private const string cstModificar = "Modificar";
        private Handler handler;
        public EmpresaUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            
            DataContext = handler.ConfGeneralViewModel;
            InitializeComponent();
            btnModif.Content = cstAdicionar;
            grAddMod.Header = cstAdicionar;
            txtKey.IsEnabled = true;
        }

        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
            {
                btnModif.Content = cstAdicionar;
                grAddMod.Header = cstAdicionar;
                txtKey.IsEnabled = true;
                btnElimi.Visibility = Visibility.Hidden;
                return;
            }

            if (btnModif != null)
            {
                btnModif.Content = cstModificar;
                grAddMod.Header = cstModificar;
                txtKey.IsEnabled = false;
                btnElimi.Visibility = Visibility.Visible;
            }
                
        }
        public void pLimpiar()
        {
            btnModif.Content = cstAdicionar;
            grAddMod.Header = cstAdicionar;
            txtKey.IsEnabled = true;
            btnElimi.Visibility = Visibility.Hidden;
        }

        private void BtnElimi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnModif_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

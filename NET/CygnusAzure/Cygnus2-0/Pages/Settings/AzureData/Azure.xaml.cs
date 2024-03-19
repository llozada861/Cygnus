using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Pages.General;
using Cygnus2_0.Pages.SolInfo;
using Cygnus2_0.Pages.Time;
using Cygnus2_0.ViewModel.Azure;
using FirstFloor.ModernUI.Windows;
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

namespace Cygnus2_0.Pages.Settings.AzureData
{
    /// <summary>
    /// Interaction logic for Azure.xaml
    /// </summary>
    public partial class Azure : UserControl, IContent
    {
        private Handler handler;
        private AzureViewModel view;
        private const string cstAdicionar = "Adicionar";
        private const string cstModificar = "Modificar";
        public Azure()
        {
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            view = new AzureViewModel(handler);
            DataContext = view;

            InitializeComponent();
            btnModif.Content = cstAdicionar;
            grAddMod.Header = cstAdicionar;
        }

        private void btnAyuda_Click(object sender, RoutedEventArgs e)
        {
            UserControl help = new HelpArea("\\img\\ayudaAzure.png", "De AzureDevops debe tomar la info de la columna Area Path (recuadro ROJO):");
            WinImage request = new WinImage(help,"Ayuda",800,600);
            request.ShowDialog();
        }

        private void btnAyudaFullName_Click(object sender, RoutedEventArgs e)
        {
            UserControl help = new HelpArea("\\img\\Fullname.png", "Del profile de AzureDevops debe tomar el FULL NAME (recuadro ROJO):");
            WinImage request = new WinImage(help, "FulName",800,600);
            request.ShowDialog();
        }

        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
            {
                btnModif.Content = cstAdicionar;
                grAddMod.Header = cstAdicionar;
                btnElimi.Visibility = Visibility.Hidden;
                return;
            }

            btnModif.Content = cstModificar;
            grAddMod.Header = cstModificar;
            btnElimi.Visibility = Visibility.Visible;
        }

        private void btnModif_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnElimi_Click(object sender, RoutedEventArgs e)
        {

        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            
        }
    }
}

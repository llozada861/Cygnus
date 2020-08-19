using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Aplica;
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
using FirstFloor.ModernUI.Windows.Navigation;
using System.Security.Permissions;

namespace Cygnus2_0.Pages.Aplica
{
    /// <summary>
    /// Interaction logic for UCGenerateAplica.xaml
    /// </summary>
    //[PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
    public partial class UCGenerateAplica : UserControl,IContent
    {
        private Handler handler;
        private GenerateAplicaViewModel generateAplicaViewModel;
        public UCGenerateAplica()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            generateAplicaViewModel = new GenerateAplicaViewModel(handler);

            DataContext = generateAplicaViewModel;
            InitializeComponent();

            generateAplicaViewModel.ArchivosCargados = "0";
            generateAplicaViewModel.ArchivosGenerados = "0";
            btnSqlPlus.Visibility = Visibility.Hidden;
        }

        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            string[] DropPath;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    DropPath = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                    generateAplicaViewModel.pListaArchivos(DropPath);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        private void dataGridArchivosCargados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            generateAplicaViewModel.pRefrescaConteo();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            generateAplicaViewModel.OnClean("");
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void BotonGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                generateAplicaViewModel.OnProcess(null);
                btnSqlPlus.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
                btnSqlPlus.Visibility = Visibility.Hidden;
            }
        }

        private void BotonLimpiar_Click(object sender, RoutedEventArgs e)
        {
            btnSqlPlus.Visibility = Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnSqlPlus.Visibility = Visibility.Hidden;
            generateAplicaViewModel.Codigo = "";
        }

        private void DataGridArchivosCargados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (dataGridArchivosCargados.SelectedItem == null)
                return; // return if there's no row selected

            Archivo archivo = (Archivo)dataGridArchivosCargados.SelectedItem;
            handler.pAbrirArchivo(archivo.RutaConArchivo);
        }

        private void DataGridArchivosGen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridArchivosGen.SelectedItem == null)
                return; // return if there's no row selected

            Archivo archivo = (Archivo)dataGridArchivosGen.SelectedItem;
            handler.pAbrirArchivo(archivo.Ruta+"\\"+archivo.FileName);
        }
    }
}

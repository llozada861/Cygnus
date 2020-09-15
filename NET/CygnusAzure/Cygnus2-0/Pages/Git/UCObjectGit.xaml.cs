using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Git;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace Cygnus2_0.Pages.Git
{
    /// <summary>
    /// Lógica de interacción para UCObjectGit.xaml
    /// </summary>
    public partial class UCObjectGit : UserControl, IContent
    {
        private Handler handler;
        private ObjectGitViewModel objectViewModel;
        public UCObjectGit()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            objectViewModel = new ObjectGitViewModel(handler);

            DataContext = objectViewModel;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            rdbLineaBase.IsChecked = true;
            LineaBase.Visibility = Visibility.Visible;
            Entrega.Visibility = Visibility.Hidden;
        }

        private void RdbLineaBase_Checked(object sender, RoutedEventArgs e)
        {
            LineaBase.Visibility = Visibility.Visible;
            Entrega.Visibility = Visibility.Hidden;
        }

        private void RdbEntrega_Checked(object sender, RoutedEventArgs e)
        {
            LineaBase.Visibility = Visibility.Hidden;
            Entrega.Visibility = Visibility.Visible;
        }

        private void DataGridResultado_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
        private void CopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            StringCollection paths = new StringCollection();
            List<String> archivos = new List<string>();

            IList<DataGridCellInfo> filas = dataGridResultado.SelectedCells;

            foreach (DataGridCellInfo fila in filas)
            {
                Archivo archivo = (Archivo)fila.Item;

                if (!archivos.Exists(x => x.Equals(archivo.RutaConArchivo)))
                {
                    paths.Add(archivo.RutaConArchivo);
                    archivos.Add(archivo.RutaConArchivo);
                }
            }

            Clipboard.SetFileDropList(paths);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objectViewModel.GitModel.ListaArchivosEncontrados.Clear();
        }

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                objectViewModel.GitModel.ObjetoBuscar = txtBuscar.Text;
                objectViewModel.pBuscar(null);
            }
        }
        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            string[] DropPath;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    DropPath = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                    List<Archivo> archivos = new List<Archivo>();
                    handler.pListaArchivos(DropPath, archivos);

                    foreach(Archivo archivo in archivos)
                    {
                        objectViewModel.GitModel.ListaArchivos.Add(archivo);
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

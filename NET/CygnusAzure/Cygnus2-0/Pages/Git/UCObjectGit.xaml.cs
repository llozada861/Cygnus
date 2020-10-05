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
using System.Windows.Controls.Primitives;
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
        private Brush Colobk;
        public UCObjectGit()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            objectViewModel = new ObjectGitViewModel(handler);

            DataContext = objectViewModel;
            InitializeComponent();

            chAprobar.IsEnabled = false;

            dataGridArch.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
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

                    objectViewModel.ListarArchivos(DropPath);

                    if(objectViewModel.GitModel.ListaCarpetas.Count() > 0)
                    {
                        chAprobar.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        private void TipoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox = sender as ComboBox;
            SelectListItem tipo = (SelectListItem)comboBox.SelectedItem;
            objectViewModel.GitModel.ListaCarpetas.Clear();
            objectViewModel.pArmarArbol(tipo,null);
            //dataGridArch.ItemsSource = objectViewModel.GitModel.ListaArchivos;

            /*var comboBox = sender as ComboBox;
            SelectListItem tipo = (SelectListItem)comboBox.SelectedItem;

            if (tipo != null)
            {
                if(tipo.Text.Equals("paquete"))
                {
                    Archivo selectedItem = (Archivo)this.dataGridArch.CurrentItem;

                    if (selectedItem.FileName.EndsWith(".html"))
                    {
                        selectedItem.NombreObjeto = selectedItem.NombreSinExt;
                    }
                }
            }*/
        }

        private void UsuarioSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objectViewModel.GitModel.ListaCarpetas.Clear();
            objectViewModel.pArmarArbol(null, null);

            /*var comboBox = sender as ComboBox;
            SelectListItem usuario = (SelectListItem)comboBox.SelectedItem;

            if (usuario != null)
            {
                Archivo selectedItem = (Archivo)this.dataGridArch.CurrentItem;
                objectViewModel.pPonerUsuarioArchivos(selectedItem, usuario);
            }*/
        }

        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            chAprobar.IsChecked = false;
        }

        private void ChAprobar_Checked(object sender, RoutedEventArgs e)
        {
            objectViewModel.GitModel.ActivaAprobRamas = (bool)chAprobar.IsChecked;
        }

        private void BtnExaminar_Click(object sender, RoutedEventArgs e)
        {
            chAprobar.IsEnabled = true;
        }

        private void DataGridArch_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Only handles cases where the cell contains a TextBox
            var editedTextbox = e.EditingElement as TextBox;

            if (editedTextbox != null)
            {
                Archivo item = (Archivo)e.Row.Item;

                //if (editedTextbox != null)
                //    MessageBox.Show("Value after edit: " + editedTextbox.Text);

                item.NombreObjeto = editedTextbox.Text;
                objectViewModel.GitModel.ListaCarpetas.Clear();
                objectViewModel.pArmarArbol(null, item);
            }
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (dataGridArch.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                pCambiarColorFila();
            }
        }
        private void pCambiarColorFila()
        {
            foreach (Archivo item in dataGridArch.ItemsSource)
            {
                var row = dataGridArch.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

                if (row != null)
                {
                    if (row.Background != Brushes.Red)
                        Colobk = row.Background;

                    if (item.Tipo == null || string.IsNullOrEmpty(item.Usuario) || string.IsNullOrEmpty(item.NombreObjeto))
                    {
                        row.Background = Brushes.Red;
                    }
                    else
                    {
                        row.Background = Colobk;
                    }
                }
            }
        }

        private void MenuItemCr_Click(object sender, RoutedEventArgs e)
        {
            Archivo archivo = dataGridRamas.SelectedItem as Archivo;

            if (archivo != null)
            {
                objectViewModel.pCreaRama(archivo);
            }
        }
    }
}

using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Pages.General;
using Cygnus2_0.ViewModel.Git;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Git
{
    /// <summary>
    /// Lógica de interacción para EntregaUserControl.xaml
    /// </summary>
    public partial class EntregaUserControl : UserControl, IContent
    {
        private Handler handler;
        private ObjectGitViewModel objectViewModel;
        private Brush Colobk;
        public EntregaUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            objectViewModel = new ObjectGitViewModel(handler);

            DataContext = objectViewModel;
            InitializeComponent();

            dataGridArch.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.objectViewModel.GitModel.EjecutaSonar = true;
            try
            {
                this.objectViewModel.GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(this.handler, objectViewModel.GitSeleccionado);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objectViewModel.GitModel.ListaArchivosEncontrados.Clear();
            objectViewModel.pArmarArbol(null, null);
        }
        private void AucomboBoxRepo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (objectViewModel.GitSeleccionado != null)
            {
                objectViewModel.GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(this.handler, objectViewModel.GitSeleccionado);
            }
        }
        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            string[] DropPath;

            /*if (objectViewModel.GitModel.RamaLBSeleccionada == null)
            {
                handler.MensajeError("Seleccione una rama de línea base.");
                return;
            }*/

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    DropPath = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                    objectViewModel.ListarArchivos(DropPath);

                    if (objectViewModel.GitModel.ListaCarpetas.Count() > 0)
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
            ComboBox comboBox = sender as ComboBox;
            TipoObjetos tipo = (TipoObjetos)comboBox.SelectedItem;

            if (tipo != null)
            {
                Archivo selectedItem = (Archivo)this.dataGridArch.CurrentItem;
                objectViewModel.pArmarArbol(tipo, selectedItem);
            }
        }
        private void UsuarioSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objectViewModel.pArmarArbol(null, null);
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

                item.NombreObjeto = editedTextbox.Text;
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

                    if (item.Tipo != null && item.Tipo == Int32.Parse(res.TipoOtros) || item.Tipo == Int32.Parse(res.TipoAplica))
                    {
                        row.Background = Colobk;
                    }
                }
            }
        }
        private void MenuItemCr_Click(object sender, RoutedEventArgs e)
        {
            if (objectViewModel.RamaSeleccionada != null)
            {
                objectViewModel.pCreaRama();
            }
        }
        private void DataGridArch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objectViewModel.pArmarArbol(null, null);
        }
        protected void AucomboBoxHU_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = objectViewModel.GitModel.ListaHU.Where((hu, match) => hu.Text.ToLower().Contains(args.Pattern.ToLower()));
        }
        protected void AuListaRepoGit_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = objectViewModel.GitModel.ListaGit.Where((hu, match) => hu.Descripcion.ToLower().Contains(args.Pattern.ToLower()));
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
        protected void AuListaRamasLB_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = objectViewModel.GitModel.ListaRamasLB.Where((hu, match) => hu.Text.ToLower().Contains(args.Pattern.ToLower()));
        }
    }
}

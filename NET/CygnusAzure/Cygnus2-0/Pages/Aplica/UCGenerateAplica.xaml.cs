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
using System.Windows.Controls.Primitives;
using res = Cygnus2_0.Properties.Resources;
using Cygnus2_0.Pages.General;
using Cygnus2_0.DAO;
using Cygnus2_0.Model.History;

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
        private Brush Colobk;
        public UCGenerateAplica()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            generateAplicaViewModel = new GenerateAplicaViewModel(handler);

            DataContext = generateAplicaViewModel;
            generateAplicaViewModel.Model.VisibleObjetos = Visibility.Hidden;
            InitializeComponent();

            generateAplicaViewModel.Model.ArchivosCargados = "0";
            generateAplicaViewModel.Model.ArchivosGenerados = "0";
            btnSqlPlus.Visibility = Visibility.Hidden;

            dataGridArchivosCargados.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            string[] DropPath;
            generateAplicaViewModel.Model.AprobarOrden = false;

            try
            {
                if (String.IsNullOrEmpty(generateAplicaViewModel.Model.Codigo))
                {
                    handler.MensajeError("Ingrese número de caso");
                    return;
                }

                if (generateAplicaViewModel.Model.Usuario == null)
                {
                    handler.MensajeError("Debe ingresar el usuario para la entrega.");
                    return;
                }

                if (!generateAplicaViewModel.Model.Datos && !generateAplicaViewModel.Model.Objetos)
                {
                    handler.MensajeError("Selecciona el tipo de entrega Datos u Objetos.");
                    return;
                }

                generateAplicaViewModel.Model.ListaArchivosCargados.Clear();
                generateAplicaViewModel.Model.ListaArchivosNoOrden.Clear();

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    DropPath = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                    generateAplicaViewModel.pListaArchivos(DropPath);
                    dataGridArchivosCargados.Items.Refresh();
                }

            }
            catch (Exception ex)
            {
                generateAplicaViewModel.Model.ListaArchivosCargados.Clear();
                generateAplicaViewModel.Model.ListaArchivosNoOrden.Clear();
                handler.MensajeError(ex.Message);
            }

            //dataGridArchivosCargados.ItemsSource = generateAplicaViewModel.Model.ListaArchivosCargados.OrderBy(x=>x.OrdenAplicacion);
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
            generateAplicaViewModel.Model.Objetos = false;
            generateAplicaViewModel.Model.Datos = false;
            generateAplicaViewModel.Model.VisibleObjetos = Visibility.Hidden;
            generateAplicaViewModel.Model.AprobarOrden = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnSqlPlus.Visibility = Visibility.Hidden;
            generateAplicaViewModel.Model.Codigo = "";
        }

        private void DataGridArchivosCargados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (dataGridArchivosCargados.SelectedItem == null)
                return; // return if there's no row selected

            Archivo archivo = (Archivo)dataGridArchivosCargados.SelectedItem;
        }

        private void DataGridArchivosGen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridArchivosGen.SelectedItem == null)
                return; // return if there's no row selected

            Archivo archivo = (Archivo)dataGridArchivosGen.SelectedItem;
            handler.pAbrirArchivo(archivo.Ruta+"\\"+archivo.FileName);
        }

        private void ChAprobar_Checked(object sender, RoutedEventArgs e)
        {
            //generateAplicaViewModel.Model.AprobarOrden = (bool)chAprobar.IsChecked;
        }

        private void RdbDatos_Click(object sender, RoutedEventArgs e)
        {
            generateAplicaViewModel.Model.VisibleObjetos = Visibility.Hidden;
        }
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (dataGridArchivosCargados.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                pCambiarColorFila();
            }
        }
        private void pCambiarColorFila()
        {
            foreach (Archivo item in dataGridArchivosCargados.ItemsSource)
            {
                var row = dataGridArchivosCargados.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

                if (row != null)
                {
                    if (row.Background != Brushes.Red)
                        Colobk = row.Background;

                    if (item.Tipo == null || string.IsNullOrEmpty(item.NombreObjeto))
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
        private void BtnExaminar_Click(object sender, RoutedEventArgs e)
        {
            generateAplicaViewModel.Model.AprobarOrden = false;

            try
            {
                if (String.IsNullOrEmpty(generateAplicaViewModel.Model.Codigo))
                {
                    handler.MensajeError("Ingrese número de caso");
                    return;
                }

                if (generateAplicaViewModel.Model.Usuario == null)
                {
                    handler.MensajeError("Debe ingresar el usuario para la entrega.");
                    return;
                }

                if (!generateAplicaViewModel.Model.Datos && !generateAplicaViewModel.Model.Objetos)
                {
                    handler.MensajeError("Selecciona el tipo de entrega Datos u Objetos.");
                    return;
                }

                generateAplicaViewModel.pExaminar(null);
                dataGridArchivosCargados.Items.Refresh();
            }
            catch { }
        }

        protected void AucomboBox_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = generateAplicaViewModel.Model.ListaUsuarios.Where((hu, match) => hu.Text.ToLower().Contains(args.Pattern.ToLower()));
        }
        protected void CasoText_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = generateAplicaViewModel.Model.ListaHistoria.Where(x => x.Historia.ToLower().Contains(args.Pattern.ToLower()));
            generateAplicaViewModel.Model.ListaAplicaHistoria.Clear();
            generateAplicaViewModel.Model.ListaAplicaHistoria = SqliteDAO.pListaAplicaHistoria(args.Pattern.ToUpper());
        }

        private void CasoText_TextChanged(object sender, TextChangedEventArgs e)
        {
            generateAplicaViewModel.Model.ListaAplicaHistoria.Clear();
            generateAplicaViewModel.Model.ListaAplicaHistoria = SqliteDAO.pListaAplicaHistoria(CasoText.Text.ToUpper());
        }

        private void dataGridArchivosGen_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Archivo select = (Archivo)dataGridArchivosGen.SelectedItem;
                SqliteDAO.pEliminaObjeto(new AplicaHistoriaModel { Archivo = select.FileName,Caso = select.OrdenCambio, Codigo = select.Codigo, Ruta = select.Ruta,Tipo = select.Tipo,Usuario = select.Owner});
                generateAplicaViewModel.Model.ListaAplicaHistoria = SqliteDAO.pListaAplicaHistoria(generateAplicaViewModel.Model.Codigo.Trim().ToUpper());
            }
        }

        private void CasoText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            generateAplicaViewModel.Model.ListaAplicaHistoria.Clear();
            generateAplicaViewModel.Model.ListaAplicaHistoria = SqliteDAO.pListaAplicaHistoria(generateAplicaViewModel.Model.Historia.Historia.ToUpper());
        }
    }
}

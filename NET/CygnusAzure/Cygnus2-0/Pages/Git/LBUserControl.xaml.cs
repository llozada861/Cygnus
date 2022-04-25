using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Pages.General;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Git
{
    /// <summary>
    /// Lógica de interacción para LBUserControl.xaml
    /// </summary>
    public partial class LBUserControl : UserControl, IContent
    {
        private Handler handler;
        private ObjectGitViewModel objectViewModel;
        private Brush Colobk;
        public LBUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            objectViewModel = new ObjectGitViewModel(handler);

            DataContext = objectViewModel;
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.objectViewModel.GitModel.ListaHU = SqliteDAO.pObtListaHUAzure(handler);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
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
        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                objectViewModel.GitModel.ObjetoBuscar = txtBuscar.Text;
                objectViewModel.pBuscar(null);
            }
        }
        protected void AuListaRamasLB_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = objectViewModel.GitModel.ListaRamasLB.Where((hu, match) => hu.Text.ToLower().Contains(args.Pattern.ToLower()));
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

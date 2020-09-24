using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Compila;
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
using System.IO;
using System.Diagnostics;
using Cygnus2_0.Pages.General;
using Cygnus2_0.ViewModel.Sonar;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Compila
{
    /// <summary>
    /// Interaction logic for UCCompila.xaml
    /// </summary>
    //[PrincipalPermission(SecurityAction.Demand)]
    public partial class UCSonar : UserControl,IContent
    {
        private Handler handler;
        private SonarViewModel view;
        public UCSonar()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new SonarViewModel(handler);

            DataContext = view;
            InitializeComponent();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            view.pLimpiar("");
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            view.GitModel.ListaArchivosEncontrados.Clear();
            view.GitModel.ListaArchivos.Clear();
        }

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                view.GitModel.ObjetoBuscar = txtBuscar.Text;
                view.pBuscar(null);
            }
        }

        private void BtnPasar_Click(object sender, RoutedEventArgs e)
        {
            var listaSel = dataGridResultado.SelectedItems;

            if (listaSel.Count == 0)
                return;

            foreach (var row in listaSel)
            {
                Archivo archivo = (Archivo)row;

                if(!view.GitModel.ListaArchivos.ToList().Exists(x=>x.FileName.Equals(archivo.FileName)) && archivo.Extension != res.ExtensionHtml)
                    view.GitModel.ListaArchivos.Add(archivo);
            }
        }

        private void DataGridAnalizar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridAnalizar.SelectedItem == null)
                return; // return if there's no row selected

            Archivo archivo = (Archivo)dataGridAnalizar.SelectedItem;
            handler.pAbrirArchivo(archivo.RutaConArchivo);
        }
    }
}

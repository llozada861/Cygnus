using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Objects;
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
using Notifications.Wpf;

namespace Cygnus2_0.Pages.Objects
{
    /// <summary>
    /// Interaction logic for UCDesBlockObjects.xaml
    /// </summary>
    public partial class UCDesBlockObjects : UserControl,IContent
    {
        private Handler handler;
        private DesblockViewModel view;
        private MainWindow myWin;
        public UCDesBlockObjects()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new DesblockViewModel(handler);
            handler.DesblockViewModel = view;

            DataContext = view;
            InitializeComponent();
        }

        private void dataGridObjetos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridObjetos.SelectedItem == null)
                return;

            Archivo objeto = dataGridObjetos.SelectedItem as Archivo;

            view.pAdicionaObjeto(objeto);
        }

        private void dataGridObjsDesbloq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            view.pRefrescaConteo();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            view.OnClean("");
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void dataGridObjetos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridObjetos.SelectedItem == null)
                return;

            Archivo objeto = dataGridObjetos.SelectedItem as Archivo;
            view.Fecha = Convert.ToDateTime(objeto.FechaEstLib);
        }

        private void btnUpd_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridObjetos.SelectedItem == null)
                return;

            Archivo objeto = dataGridObjetos.SelectedItem as Archivo;
            view.OnUpdate(objeto);
        }
    }
}

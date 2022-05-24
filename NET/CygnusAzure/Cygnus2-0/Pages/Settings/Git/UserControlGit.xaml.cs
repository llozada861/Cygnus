using Cygnus2_0.DAO;
using Cygnus2_0.General;
using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Settings.Git
{
    /// <summary>
    /// Lógica de interacción para UserControlSonar.xaml
    /// </summary>
    public partial class UserControlGit : UserControl, IContent
    {
        private Handler handler;
        public UserControlGit()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler.RepositorioVM;
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

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            handler.RepositorioVM.TabRepo = true;
            handler.RepositorioVM.TabRama = false;
        }

        private void TabControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            handler.RepositorioVM.TabRepo = false;
            handler.RepositorioVM.TabRama = true;
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //handler.RepositorioVM.ListaRamaGit.Clear();
            handler.RepositorioVM.ListaRamaGit = SqliteDAO.pListaRamaRepositorios(handler.RepositorioVM.RepoSeleccionado);
            handler.RepositorioVM.TabRepo = true;
            handler.RepositorioVM.TabRama = false;
        }

        private void dgDataRama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            handler.RepositorioVM.TabRepo = false;
            handler.RepositorioVM.TabRama = true;
        }

        private void TabRepo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            handler.RepositorioVM.TabRepo = true;
            handler.RepositorioVM.TabRama = false;
        }

        private void TabRama_MouseUp(object sender, MouseButtonEventArgs e)
        {
            handler.RepositorioVM.TabRepo = false;
            handler.RepositorioVM.TabRama = true;
        }
    }
}

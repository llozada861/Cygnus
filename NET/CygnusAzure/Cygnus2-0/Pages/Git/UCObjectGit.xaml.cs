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
    /// Lógica de interacción para UCObjectGit.xaml
    /// </summary>
    public partial class UCObjectGit : UserControl, IContent
    {
        private Handler handler;
        private ObjectGitViewModel objectViewModel;
        private Brush Colobk;
        private UserControl userControls;
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
            pMostrarLB();
        }

        private void RdbLineaBase_Checked(object sender, RoutedEventArgs e)
        {
            pMostrarLB();
        }

        private void RdbEntrega_Checked(object sender, RoutedEventArgs e)
        {
            pMostrarEntrega();
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

        private void pMostrarLB()
        {
            GridMain.Children.Clear();
            userControls = new LBUserControl();
            GridMain.Children.Add(userControls);
        }

        private void pMostrarEntrega()
        {
            GridMain.Children.Clear();
            userControls = new EntregaUserControl();
            GridMain.Children.Add(userControls);
        }
    }
}

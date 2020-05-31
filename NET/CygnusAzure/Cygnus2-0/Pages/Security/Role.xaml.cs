using Cygnus2_0.General;
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
using Cygnus2_0.ViewModel.Security;

namespace Cygnus2_0.Pages.Security
{
    /// <summary>
    /// Interaction logic for Role.xaml
    /// </summary>
    public partial class Role : UserControl, IContent
    {
        private Handler handler;
        private MainWindow myWin;
        private RoleViewModel roleViewModel;
        public Role()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            roleViewModel = new RoleViewModel(handler);
            DataContext = roleViewModel;
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
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }
}

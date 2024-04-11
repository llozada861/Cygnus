using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Git;
using FirstFloor.ModernUI.Presentation;
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

namespace Cygnus2_0.Pages.Git
{
    /// <summary>
    /// Lógica de interacción para SelectCommitUserControl.xaml
    /// </summary>
    public partial class SelectCommitUserControl : UserControl, IContent
    {
        private Handler handler;
        private ObjectGitViewModel view;
        private bool boProcesar;
        public SelectCommitUserControl(List<SelectListItem> ListaCommitsLB, List<SelectListItem> ListaCommitsFeaure, string lineaBase, string feature)
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            view = new ObjectGitViewModel(handler, ListaCommitsLB, ListaCommitsFeaure);

            DataContext = view;
            InitializeComponent();
            Main.Header = "Commits " + lineaBase;
            Feature.Header = "Commits " + feature;
            boProcesar = false;
        }

        public ObjectGitViewModel View { get { return view; } }

        public bool BoProcesar { get {  return boProcesar; } set {  boProcesar = value; } }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.boProcesar = true;
        }
    }
}

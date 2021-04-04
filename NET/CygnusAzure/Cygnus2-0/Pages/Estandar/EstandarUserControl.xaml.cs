using Cygnus2_0.General;
using Cygnus2_0.Pages.General;
using Cygnus2_0.ViewModel.Estandar;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
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

namespace Cygnus2_0.Pages.Estandar
{
    /// <summary>
    /// Lógica de interacción para EstandarUserControl.xaml
    /// </summary>
    public partial class EstandarUserControl : UserControl, IContent
    {
        private Handler handler;
        private EstandarViewModel view;
        public EstandarUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new EstandarViewModel(handler);

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
            view.pObtenerEstandar();
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        protected void AucomboBox_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = view.Model.ListaEstandar.Where((hu, match) => hu.Text.ToLower().Contains(args.Pattern.ToLower()));
        }
    }
}

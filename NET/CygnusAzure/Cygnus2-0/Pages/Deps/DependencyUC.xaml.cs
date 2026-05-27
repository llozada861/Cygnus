using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Deps;
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

namespace Cygnus2_0.Pages.Deps
{
    /// <summary>
    /// Lógica de interacción para DependencyUC.xaml
    /// </summary>
    public partial class DependencyUC : UserControl
    {
        private Handler handler;
        private DepViewModel view;

        public DependencyUC()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new DepViewModel(handler);

            DataContext = view;
            InitializeComponent();           
        }
    }
}

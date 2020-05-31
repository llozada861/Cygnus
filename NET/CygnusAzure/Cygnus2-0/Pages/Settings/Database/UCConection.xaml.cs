using Cygnus2_0.Interface;
using Cygnus2_0.ViewModel.Settings;
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
using Cygnus2_0.General;

namespace Cygnus2_0.Pages.Settings
{
    /// <summary>
    /// Interaction logic for UserControlConexion.xaml
    /// </summary>
    public partial class UCConection : UserControl
    {
        Handler handler;
        public UCConection()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            DataContext = handler.ConnViewModel;
            InitializeComponent();

            passwordBox.Password = handler.ConnViewModel.Pass;
        }
    }
}

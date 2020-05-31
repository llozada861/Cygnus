using FirstFloor.ModernUI.Presentation;
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

namespace Cygnus2_0.Pages.SolInfo
{
    /// <summary>
    /// Interaction logic for RequetInfo.xaml
    /// </summary>
    public partial class RequetInfo : Window
    {
        private MainWindow parent;

        public RequetInfo(UserControl userControls, MainWindow parent, string titulo)
        {
            InitializeComponent();
            this.Title = titulo;
            gridPrincipal.Children.Add(userControls);
            this.parent = parent;
            parent.Next = false;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            parent.Next = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppearanceManager.Current.ThemeSource == AppearanceManager.DarkThemeSource)
            {
                pWindow.Background = Brushes.DimGray;
            }
            else
            {
                pWindow.Background = Brushes.WhiteSmoke;
            }
        }
    }
}

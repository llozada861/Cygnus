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
    /// Lógica de interacción para UCObjectGit.xaml
    /// </summary>
    public partial class UCObjectGit : UserControl
    {
        public UCObjectGit()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            rdbLineaBase.IsChecked = true;
            LineaBase.Visibility = Visibility.Visible;
            Entrega.Visibility = Visibility.Hidden;
        }

        private void RdbLineaBase_Checked(object sender, RoutedEventArgs e)
        {
            LineaBase.Visibility = Visibility.Visible;
            Entrega.Visibility = Visibility.Hidden;
        }

        private void RdbEntrega_Checked(object sender, RoutedEventArgs e)
        {
            LineaBase.Visibility = Visibility.Hidden;
            Entrega.Visibility = Visibility.Visible;
        }

        private void DataGridResultado_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

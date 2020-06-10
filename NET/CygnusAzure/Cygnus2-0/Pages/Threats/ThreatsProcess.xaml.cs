using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Threats;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Threats
{
    /// <summary>
    /// Lógica de interacción para ThreatsProcess.xaml
    /// </summary>
    public partial class ThreatsProcess : UserControl
    {
        private Handler handler;
        private ThreatsViewModel view;

        public ThreatsProcess()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            view = new ThreatsViewModel(handler);
            DataContext = view;

            InitializeComponent();
            //rdPkg.IsChecked = true;
        }

        private void BtnPre_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPrin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPost_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPkg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnProc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChkPre_Click(object sender, RoutedEventArgs e)
        {
            if (chkPre.IsChecked == true)
            {
                btnPre.Visibility = Visibility.Visible;
                view.ApiPre = true;
            }
            else
            {
                btnPre.Visibility = Visibility.Hidden;
                view.ApiPre = false;
            }
        }

        private void ChkPost_Click(object sender, RoutedEventArgs e)
        {
            if (chkPost.IsChecked == true)
            {
                btnPost.Visibility = Visibility.Visible;
                view.ApiPost = true;
            }
            else
            {
                btnPost.Visibility = Visibility.Hidden;
                view.ApiPost = false;
            }
        }

        private void RdPkg_Click(object sender, RoutedEventArgs e)
        {
            /*if (rdPkg.IsChecked == true)
            {
                btnPkg.Visibility = Visibility.Visible;
            }*/
        }

        private void RdPro_Click(object sender, RoutedEventArgs e)
        {
            /*if (rdPro.IsChecked == true)
            {
                btnPkg.Visibility = Visibility.Hidden;
            }*/
        }

        private void TxtNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            view.VistaPrevia = "";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ChkCant_Click(object sender, RoutedEventArgs e)
        {
            if (chkCant.IsChecked == true)
            {
                btnCant.Visibility = Visibility.Visible;
                view.ApiCantidad = true;
            }
            else
            {
                btnCant.Visibility = Visibility.Hidden;
                view.ApiCantidad = false;
            }
        }

        private void BtnCant_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

using Cygnus2_0.General;
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

namespace Cygnus2_0.Pages.Security
{
    /// <summary>
    /// Interaction logic for OneDriveCred.xaml
    /// </summary>
    public partial class OneDriveCred : UserControl
    {
        private Handler handler;
        private MainWindow myWin;
        public OneDriveCred()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            InitializeComponent();
        }

        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            string cadena = txtUrl.Text + "|" + txtUser.Text + "|" + txtPass.Text + "|" + txtVer.Text + "|" + txtZip.Text;
            string cadenaWrap = EncriptaPass.Encriptar(cadena);
            handler.pGuardaArchivo(res.ArchivoCredenciales, cadenaWrap);
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtUrl.Text = "";
            txtUser.Text = "";
            txtPass.Text = "";
            txtVer.Text = "";
            txtZip.Text = "";
        }
    }
}

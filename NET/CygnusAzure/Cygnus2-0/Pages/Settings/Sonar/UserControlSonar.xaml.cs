using Cygnus2_0.DAO;
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

namespace Cygnus2_0.Pages.Settings.Sonar
{
    /// <summary>
    /// Lógica de interacción para UserControlSonar.xaml
    /// </summary>
    public partial class UserControlSonar : UserControl
    {
        private Handler handler;
        public UserControlSonar()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
        }

        private void RdbExiste_Checked(object sender, RoutedEventArgs e)
        {
            grbDatosInstall.Visibility = Visibility.Hidden;
            grbDatosRequeridos.Visibility = Visibility.Visible;
        }

        private void RdbNuevo_Checked(object sender, RoutedEventArgs e)
        {
            grbDatosInstall.Visibility = Visibility.Visible;
            grbDatosRequeridos.Visibility = Visibility.Hidden;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string rutaSonar = "";

            if (rdbExiste.IsChecked == true)
            {
                if (string.IsNullOrEmpty(txtRuta.Text))
                {
                    handler.MensajeError("Ingrese ruta");
                    return;
                }

                handler.RutaSonar = txtRuta.Text;
            }

            if(rdbNuevo.IsChecked == true)
            {
                if (string.IsNullOrEmpty(txtRutaInstall.Text))
                {
                    handler.MensajeError("Ingrese ruta");
                    return;
                }

                if (string.IsNullOrEmpty(txtUser.Text))
                {
                    handler.MensajeError("Ingrese usuario");
                    return;
                }

                if (string.IsNullOrEmpty(txtPass.Text))
                {
                    handler.MensajeError("Ingrese contraseña");
                    return;
                }

                handler.RutaSonar = txtRutaInstall.Text;
                SonarQ.General.Handler.pInstalaSonar(handler.RutaSonar, txtUser.Text, txtUser.Text);
            }

            SqliteDAO.pCreaConfiguracion(res.KeyRutaSonar, rutaSonar);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            grbDatosInstall.Visibility = Visibility.Hidden;
            grbDatosRequeridos.Visibility = Visibility.Visible;

            rdbExiste.IsChecked = true;

            if (string.IsNullOrEmpty(handler.RutaSonar))
            {
                txtRuta.Text = handler.RutaSonar;
            }
        }
    }
}

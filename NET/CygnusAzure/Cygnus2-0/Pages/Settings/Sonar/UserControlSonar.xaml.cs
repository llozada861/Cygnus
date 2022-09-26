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
            btnGuardar.Content = "Guardar";
        }

        private void RdbNuevo_Checked(object sender, RoutedEventArgs e)
        {
            grbDatosInstall.Visibility = Visibility.Visible;
            grbDatosRequeridos.Visibility = Visibility.Hidden;
            btnGuardar.Content = "Instalar";
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdbExiste.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(txtRuta.Text))
                    {
                        handler.MensajeError("Ingrese ruta");
                        return;
                    }

                    if (string.IsNullOrEmpty(txtProyecto.Text))
                    {
                        handler.MensajeError("Ingrese el proyecto de sonar");
                        return;
                    }

                    handler.RutaSonar = txtRuta.Text;
                    handler.ProyectoSonar = txtProyecto.Text;
                    SqliteDAO.pCreaConfiguracion(res.KeyRutaSonar, handler.RutaSonar);
                    SqliteDAO.pCreaConfiguracion(res.KeyProyectoSonar, handler.ProyectoSonar);
                    handler.MensajeOk("Ya se puede analizar el código al momento de compilar de los objetos.");
                }

                if (rdbNuevo.IsChecked == true)
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
                    handler.ProyectoSonar = txtProyecto2.Text;

                    RepoGit.pCreaDirectorios(handler.RutaSonar);

                    SqliteDAO.pCreaConfiguracion(res.KeyRutaSonar, handler.RutaSonar);
                    SqliteDAO.pCreaConfiguracion(res.KeyProyectoSonar, handler.ProyectoSonar);
                    SonarQube.pInstalaSonar(handler.RutaSonar, txtUser.Text, txtPass.Text);
                    handler.MensajeOk("Se instala con éxito. Ya se puede analizar el código al momento de compilar de los objetos.");
                    txtUser.Text = "";
                    txtPass.Text = "";
                    txtProyecto2.Text = "";
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            grbDatosInstall.Visibility = Visibility.Hidden;
            grbDatosRequeridos.Visibility = Visibility.Visible;

            rdbExiste.IsChecked = true;

            if (!string.IsNullOrEmpty(handler.RutaSonar))
            {
                txtRuta.Text = handler.RutaSonar;
            }

            if (!string.IsNullOrEmpty(handler.ProyectoSonar))
            {
                txtProyecto.Text = handler.ProyectoSonar;
            }
        }
    }
}

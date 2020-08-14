using Cygnus2_0.DAO;
using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.IO;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Settings.Git
{
    /// <summary>
    /// Lógica de interacción para UserControlSonar.xaml
    /// </summary>
    public partial class UserControlGit : UserControl
    {
        private Handler handler;
        public UserControlGit()
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
            btnGuardar.Content = "Clonar";
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdbExiste.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(txtRutaGitBash1.Text))
                    {
                        handler.MensajeError("Ingrese la ruta del git-bash.exe");
                        return;
                    }

                    if (string.IsNullOrEmpty(txtRutaInstall1.Text))
                    {
                        handler.MensajeError("Ingresa la ruta dónde tienes el repositorio.");
                        return;
                    }

                    handler.CursorWait();

                    handler.RutaGitBash = txtRutaGitBash1.Text;
                    SqliteDAO.pCreaConfiguracion(res.KeyRutaGitBash, handler.RutaGitBash);

                    handler.RutaGitDatos = txtRutaGitBash1.Text;
                    SqliteDAO.pCreaConfiguracion(res.KeyRutaGitDatos, handler.RutaGitDatos);

                    handler.CursorNormal();
                    handler.MensajeOk("Ya puedes versionar con Cygnus!");
                }

                if (rdbNuevo.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(txtRutaGitBash2.Text))
                    {
                        handler.MensajeError("Ingrese la ruta del git-bash.exe");
                        return;
                    }

                    if (string.IsNullOrEmpty(txtRutaInstall2.Text))
                    {
                        handler.MensajeError("Ingresa la ruta dónde vas a clonar el respositorio");
                        return;
                    }

                    handler.CursorWait();

                    handler.RutaGitBash = txtRutaGitBash2.Text;
                    SqliteDAO.pCreaConfiguracion(res.KeyRutaGitBash, handler.RutaGitBash);

                    handler.RutaGitDatos = txtRutaInstall2.Text;

                    RepoGit.pCreaDirectorios(handler.RutaGitDatos);

                    RepoGit.pClonarRepo(handler.RutaGitDatos, res.RepoDATOS);

                    handler.RutaGitDatos = Path.Combine(handler.RutaGitDatos, res.CarpetaDatosGIT);
                    SqliteDAO.pCreaConfiguracion(res.KeyRutaGitDatos, handler.RutaGitDatos);

                    handler.CursorNormal();

                    handler.MensajeOk("Ya puedes versionar con Cygnus!");
                    txtRutaGitBash1.Text = handler.RutaGitBash;
                    txtRutaInstall1.Text = handler.RutaGitDatos;
                }
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            grbDatosInstall.Visibility = Visibility.Hidden;
            grbDatosRequeridos.Visibility = Visibility.Visible;

            rdbExiste.IsChecked = true;

            if (!string.IsNullOrEmpty(handler.RutaGitDatos))
            {
                txtRutaInstall1.Text = handler.RutaGitDatos;
            }

            if (!string.IsNullOrEmpty(handler.RutaGitBash))
            {
                txtRutaGitBash1.Text = handler.RutaGitBash;
            }
        }
    }
}

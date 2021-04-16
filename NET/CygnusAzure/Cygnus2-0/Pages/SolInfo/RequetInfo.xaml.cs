using Cygnus2_0.General;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.SolInfo
{
    /// <summary>
    /// Interaction logic for RequetInfo.xaml
    /// </summary>
    public partial class RequetInfo : Window
    {
        private MainWindow parent;
        private string validacion;
        private Handler handler;

        public RequetInfo(UserControl userControls,Handler handler, MainWindow parent, string titulo,string validacion)
        {
            InitializeComponent();
            this.Title = titulo;
            gridPrincipal.Children.Add(userControls);
            this.parent = parent;
            parent.Next = false;
            this.validacion = validacion;
            this.handler = handler;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            parent.Next = true;

            try
            {
                if (validacion == res.KEY_EMPRESA && handler.ConfGeneralViewModel.Model.Empresa == null || string.IsNullOrEmpty(handler.ConfGeneralViewModel.Model.Empresa.Value) || handler.ConfGeneralViewModel.Model.Empresa.Value == "-")
                {
                    MessageBox.Show("Seleccione un Empresa para continuar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (validacion == res.CONEXION_BD && handler.ConnViewModel.Usuario == null || handler.ConnViewModel.Usuario.ToUpper().Equals(res.UsuarioSQLDefault) || string.IsNullOrEmpty(handler.ConnViewModel.Usuario))
                {
                    MessageBox.Show("Configure la conexión a la BD para continuar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (validacion == res.SQLPLUS && handler.ConfGeneralViewModel.Model.RutaSqlplus.Equals(res.RutaSqlplusDefault))
                {
                    MessageBox.Show("Configure la ruta del SQLPLUS para continuar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch
            {

            }

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

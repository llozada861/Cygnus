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
using Cygnus2_0.Pages.General;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Settings.Database
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

            DataContext = handler.ConnView;
            InitializeComponent();

            passwordBox.Password = handler.ConnView.Model.Pass;

            //if(!tipo.Equals(res.Nuevo))
            //    txtEtiqueta.IsEnabled = false;
        }

        protected void AucomboBox_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = handler.ConnView.Model.ListaConexiones.Where((hu, match) => hu.Usuario.ToLower().Contains(args.Pattern.ToLower()));
        }

        private void AucomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (handler.ConnView.Model.Conexion != null)
                {
                    txtEtiqueta.IsEnabled = false;
                    passwordBox.Password = handler.ConnView.Model.Conexion.Pass;

                    handler.ModificaLlaveRegistro();
                }
            }
            catch { }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            handler.ConnView.OnEliminar(null);
            passwordBox.Password = "";
            txtEtiqueta.IsEnabled = true;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            handler.ConnView.OnProcess(passwordBox);
            passwordBox.Password = "";
        }

        private void BtnNueva_Click(object sender, RoutedEventArgs e)
        {
            txtEtiqueta.IsEnabled = true;
            passwordBox.Password = "";
            AucomboBox.SelectedIndex = -1;
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Pages.General;
using Cygnus2_0.Pages.SolInfo;
using Cygnus2_0.Pages.Time;
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

namespace Cygnus2_0.Pages.Settings.AzureData
{
    /// <summary>
    /// Interaction logic for Azure.xaml
    /// </summary>
    public partial class Azure : UserControl
    {
        private Handler handler;
        public Azure()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }


        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            pGuardar();
        }


        public void pGuardar()
        {
            try
            {
                if (string.IsNullOrEmpty(handler.ConnViewModel.UsuarioAzure))
                {
                    handler.MensajeError("Debe ingresar el usuario de Azure.");
                    return;
                }

                if (string.IsNullOrEmpty(handler.ConnViewModel.Correo))
                {
                    handler.MensajeError("Debe ingresar el correo empresarial.");
                    return;
                }

                handler.DAO.pActualizaCorreo();

                handler.MensajeOk("Proceso terminó con éxito");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        private void btnAyuda_Click(object sender, RoutedEventArgs e)
        {
            UserControl help = new HelpArea("\\img\\ayudaAzure.png", "De AzureDevops debe tomar la info de la columna Area Path (recuadro ROJO):");
            WinImage request = new WinImage(help,"Ayuda");
            request.ShowDialog();
        }

        private void btnAyudaFullName_Click(object sender, RoutedEventArgs e)
        {
            UserControl help = new HelpArea("\\img\\Fullname.png", "Del profile de AzureDevops debe tomar el FULL NAME (recuadro ROJO):");
            WinImage request = new WinImage(help, "FulName");
            request.ShowDialog();
        }
    }
}

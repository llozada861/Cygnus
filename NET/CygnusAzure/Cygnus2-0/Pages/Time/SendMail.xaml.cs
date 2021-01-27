using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.General.Times;
using Cygnus2_0.ViewModel.Time;
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

namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Interaction logic for SendMail.xaml
    /// </summary>
    public partial class SendMail : Window
    {
        private Handler handler;
        private TimesViewModel view;
        private TareaHoja tarea;

        public SendMail(TimesViewModel view, Handler handler, TareaHoja tarea)
        {
            this.view = view;
            this.handler = handler;
            DataContext = view;
            this.tarea = tarea;
            InitializeComponent();

            txtCuerpo.IsEnabled = false;

            view.Model.MailCreaRq.Asunto = "Creación Historia de Usuario AzureDevops [" + tarea.Descripcion + "]";
            view.Model.MailCreaRq.Cuerpo = "<p>Buen d&iacute;a, <br/><br/> Se solicita su amable colaboraci&oacute;n para que por favor sea creada la siguiente historia de usuario:</p>"+
                                     "   <ul>"+
                                     "   <li><strong>" + tarea.Descripcion +" </strong></li>" +
                                     "   </ul>" +
                                     "   <p> Muchas gracias!</p>" +
                                     "   <p> Correo enviado a trav&eacute;s de Cygnus.</p> "; 
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

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.PARACREAACTKEY)))
            {
                view.Model.MailCreaRq.Para = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.PARACREAACTKEY)).Value;
            }
            else
            {
                view.Model.MailCreaRq.Para = "";
            }
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(view.Model.MailCreaRq.Para))
            {
                this.handler.MensajeError("Debe ingresar un correo.");
                return;
            }

            if(string.IsNullOrEmpty(view.Model.MailCreaRq.Asunto))
            {
                handler.MensajeError("Debe ingresar un asunto para el correo");
                return;
            }

            this.handler.pEnviarCorreo(view.Model.MailCreaRq.Para, view.Model.MailCreaRq.Asunto, view.Model.MailCreaRq.Cuerpo);
            SqliteDAO.pCreaConfiguracion(res.PARACREAACTKEY, view.Model.MailCreaRq.Para);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

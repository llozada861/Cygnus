using Cygnus2_0.DAO;
using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Cygnus2_0.Pages.Settings.General
{
    /// <summary>
    /// Interaction logic for PathsUserControl.xaml
    /// </summary>
    public partial class PathsUserControl : UserControl
    {
        private Handler handler;
        public PathsUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            DataContext = handler;
            InitializeComponent();
            txtName.Text = res.SQLPLUS;
            txtPath.Text = handler.ConfGeneralView.Model.RutaSqlplus;
        }
        private void btnModif_Click(object sender, RoutedEventArgs e)
        {
            string key = txtName.Text;
            string ruta = txtPath.Text;

            if(String.IsNullOrEmpty(ruta))
            {
                handler.MensajeError("Ingrese una ruta valida.");
                return;
            }

            try
            {
                SqliteDAO.pCreaConfiguracion(key,ruta);
                handler.ConfGeneralView.Model.RutaSqlplus = ruta;
                handler.MensajeOk("Proceso terminó con éxito.");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

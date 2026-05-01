using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Time;
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

namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Lógica de interacción para Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        private Handler handler;
        private ReportViewModel view;
        public Report()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new ReportViewModel(handler);

            DataContext = view;
            InitializeComponent();
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if(view.FechaDesde == null)
            {
                handler.MensajeError("Debe ingresar una Fecha Desde.");
                return;
            }

            if (view.FechaHasta == null)
            {
                handler.MensajeError("Debe ingresar una Fecha Hasta.");
                return;
            }

            view.ListaTask = SqliteDAO.pObtListaTask(handler,view.Descripcion, Convert.ToInt32(view.HU),view.FechaDesde.ToString("yyyy-MM-dd"),view.FechaHasta.ToString("yyyy-MM-dd"));
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            view.HU = "";
            view.Descripcion = "";
            view.ListaTask.Clear();
        }
    }
}

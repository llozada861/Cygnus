using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Deps;
using Cygnus2_0.ViewModel.Plantillas;
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

namespace Cygnus2_0.Pages.Plantillas
{
    /// <summary>
    /// Lógica de interacción para UCPlantillas.xaml
    /// </summary>
    public partial class UCPlantillas : UserControl
    {
        private Handler handler;
        private PlantillaViewModel view;

        public UCPlantillas()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new PlantillaViewModel(handler);

            DataContext = view;

            InitializeComponent();
        }
    }
}

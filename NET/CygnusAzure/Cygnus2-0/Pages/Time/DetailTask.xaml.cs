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

namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Interaction logic for DetailTask.xaml
    /// </summary>
    public partial class DetailTask : Window
    {
        private Handler handler;
        private TimesViewModel view;
        private TareaHoja tarea;

        public DetailTask(TimesViewModel view, Handler handler, TareaHoja tarea)
        {
            this.view = view;
            this.handler = handler;
            DataContext = view;
            this.tarea = tarea;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppearanceManager.Current.ThemeSource == AppearanceManager.DarkThemeSource)
            {
                GridDetails.Background = Brushes.DimGray;
            }
            else
            {
                GridDetails.Background = Brushes.WhiteSmoke;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

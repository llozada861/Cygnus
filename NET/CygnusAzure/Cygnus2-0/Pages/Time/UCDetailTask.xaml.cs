using Cygnus2_0.General.Times;
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
    /// Lógica de interacción para UCDetailTask.xaml
    /// </summary>
    public partial class UCDetailTask : UserControl
    {
        private Handler handler;
        private TimesViewModel view;
        private TareaHoja tarea;

        public UCDetailTask(TimesViewModel view, Handler handler, TareaHoja tarea)
        {
            this.view = view;
            this.handler = handler;
            DataContext = view;
            this.tarea = tarea;
            InitializeComponent();
        }
    }
}

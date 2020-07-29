using SonarQ.General;
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

namespace SonarQ
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<SelectListItem> archivos = new List<SelectListItem>();
            archivos.Add(new SelectListItem { Text = "D:\\", Value = "pkg_epm_bolegacortesirius.sql" });
            Handler.pEjecutarSonar(@"D:\SonarQ\Sonar", archivos);
            //Handler.pInstalaSonar("D:\\SonarQ", "llozada","123");}
            
        }
    }
}

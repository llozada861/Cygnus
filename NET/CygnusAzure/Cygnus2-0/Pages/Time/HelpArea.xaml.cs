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
    /// Interaction logic for HelpArea.xaml
    /// </summary>
    public partial class HelpArea : UserControl
    {
        public HelpArea(string Imagen, string descripcion)
        {
            InitializeComponent();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(Environment.CurrentDirectory+ Imagen);
            bi3.EndInit();
            idImagen.Source = bi3;

            lbldescripcion.Content = descripcion;
        }
    }
}

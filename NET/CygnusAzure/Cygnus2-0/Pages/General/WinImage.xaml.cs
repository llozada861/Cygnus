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

namespace Cygnus2_0.Pages.General
{
    /// <summary>
    /// Interaction logic for WinImage.xaml
    /// </summary>
    public partial class WinImage : Window
    {
        public WinImage(UserControl userControls, string titulo)
        {
            InitializeComponent();
            this.Title = titulo;
            gridPrincipal.Children.Add(userControls);
        }
    }
}

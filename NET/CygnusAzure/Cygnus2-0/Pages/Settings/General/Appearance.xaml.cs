using Cygnus2_0.General;
using Cygnus2_0.Pages.Settings.General;
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

namespace Cygnus2_0.Pages.Settings.General
{
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance : UserControl
    {
        AppearanceViewModel appareance;
        public Appearance()
        {
            InitializeComponent();

            var myWin = (MainWindow)Application.Current.MainWindow;
            appareance = myWin.Handler.Settings;

            // create and assign the appearance view model
            this.DataContext = appareance;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*string culture = "en-US";
            ComboBox miCombo = (ComboBox)sender;

            if (miCombo.SelectedIndex == 0)
            {
                culture = "es-CO";
            }
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            LocUtil.SwitchLanguage((MainWindow)Application.Current.MainWindow, culture);*/
        }              
    }
}

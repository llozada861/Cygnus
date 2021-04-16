using Cygnus2_0.General;
using Cygnus2_0.Pages.General;
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

namespace Cygnus2_0.Pages.Settings.AdminGeneral
{
    /// <summary>
    /// Interaction logic for UCGeneral.xaml
    /// </summary>
    public partial class UCGeneral : UserControl
    {
        Handler handler;

        public UCGeneral()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            DataContext = handler.ConfGeneralViewModel;
            InitializeComponent();
        }
        protected void AucomboBox_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = handler.ConfGeneralViewModel.Model.ListaEmpresas.Where((hu, match) => hu.Text.ToLower().Contains(args.Pattern.ToLower()));
        }

        private void AucomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            handler.pInicializar();
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Documentation;
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

namespace Cygnus2_0.Pages.Documentation
{
    /// <summary>
    /// Interaction logic for DocHtml.xaml
    /// </summary>
    public partial class DocHtml : UserControl
    {
        private Handler handler;
        private GeneratHtmlViewModel generatHtmlViewModel;
        public DocHtml()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            generatHtmlViewModel = new GeneratHtmlViewModel(handler);

            DataContext = generatHtmlViewModel;
            InitializeComponent();
        }

        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            string[] DropPath;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    DropPath = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                    generatHtmlViewModel.pListaArchivos(DropPath);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

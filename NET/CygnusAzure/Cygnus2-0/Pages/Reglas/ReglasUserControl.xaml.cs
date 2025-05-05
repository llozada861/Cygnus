using Cygnus2_0.General.Documentacion;
using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
using res = Cygnus2_0.Properties.Resources;
using Cygnus2_0.ViewModel.Documentation;
using Cygnus2_0.Model.Reglas;
using Cygnus2_0.ViewModel.Reglas;

namespace Cygnus2_0.Pages.Reglas
{
    /// <summary>
    /// Lógica de interacción para ReglasUserControl.xaml
    /// </summary>
    public partial class ReglasUserControl : UserControl
    {
        private Handler handler;
        private ReglasViewModel view;
        public ReglasUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            view = new ReglasViewModel(handler);
            DataContext = view;

            InitializeComponent();
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string richText = new TextRange(richTextBoxProce.Document.ContentStart, richTextBoxProce.Document.ContentEnd).Text;

            if (!string.IsNullOrEmpty(richText) && !richText.Equals("\r\n"))
            {
                if (view.Model.BdSeleccionada == null)
                {
                    handler.MensajeError("Debe seleccionar una base de datos.");
                    return;
                }

                richText = System.Text.RegularExpressions.Regex.Replace(richText, @"\r\n+", "|");
                richText = System.Text.RegularExpressions.Regex.Replace(richText, @"\s+", "");
                richText = System.Text.RegularExpressions.Regex.Replace(richText, @";", "|");
                richText = System.Text.RegularExpressions.Regex.Replace(richText, @",", "|");

                view.Model.ListaReglas.Clear();

                try
                {
                    if (!String.IsNullOrEmpty(richText.Trim()))
                    {
                        string[] codigosReglas = richText.Split(new char[] { '|' });

                        foreach (string codigo in codigosReglas)
                        {
                            view.pBuscarReglas(codigo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    handler.MensajeError(ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            richTextBoxProce.Document.Blocks.Clear();
            view.Model.ListaReglas.Clear();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            richTextBoxProce.Document.Blocks.Clear();
            view.Model.ListaReglas.Clear();
        }
    }
}

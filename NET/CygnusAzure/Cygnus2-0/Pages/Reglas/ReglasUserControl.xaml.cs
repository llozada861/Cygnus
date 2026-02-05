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
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Cygnus2_0.Model.Html;
using Microsoft.TeamFoundation.Common;

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

                txRegla.IsEnabled = false;
                txTabla.IsEnabled = false;

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

                    if(view.Model.ListaReglas.Count > 0)
                    {
                        txRegla.IsEnabled = true;
                        txTabla.IsEnabled = true;
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

        private void txTabla_TextChanged(object sender, TextChangedEventArgs e)
        {
            try 
            { 
                if (!string.IsNullOrEmpty(txTabla.Text) && !string.IsNullOrEmpty(view.Model.Regla))
                {
                    List<SelectListItem> listaPrimarias = handler.DAO.pObtPrimarias(view.Model.Tabla, view.Model.BdSeleccionada);

                    foreach (SelectListItem item in view.Model.ListaReglas)
                    {
                        item.DocumentoAD = view.pObtUpdate(listaPrimarias, txTabla.Text, view.Model.Regla, item.Value);
                    }
                }            
            }
            catch(Exception ex)
            {
            }
        }

        private void txColumna_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(view.Model.Tabla) && !string.IsNullOrEmpty(txRegla.Text))
                {
                    List<SelectListItem> listaPrimarias = handler.DAO.pObtPrimarias(view.Model.Tabla, view.Model.BdSeleccionada);

                    foreach (SelectListItem item in view.Model.ListaReglas)
                    {
                        item.DocumentoAD = view.pObtUpdate(listaPrimarias, view.Model.Tabla, txRegla.Text, item.Value);
                    }
                }
            }
            catch(Exception ex)
            {
            }
        }

        private void dataGridParameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridReglas.SelectedItem == null)
                return;

            string richText = new TextRange(richTextBoxProce.Document.ContentStart, richTextBoxProce.Document.ContentEnd).Text;

            richTextBoxSql.Document.Blocks.Clear();

            SelectListItem seleccionado = ((SelectListItem)dataGridReglas.SelectedItem);

            string idRegla = seleccionado.Value;

            if (!string.IsNullOrEmpty(view.Model.Tabla) && !string.IsNullOrEmpty(view.Model.Regla) && string.IsNullOrEmpty(seleccionado.DocumentoAD))
            {
                try
                {
                    List<SelectListItem> listaPrimarias = handler.DAO.pObtPrimarias(view.Model.Tabla, view.Model.BdSeleccionada);

                    string sbUpdate = view.pObtUpdate(listaPrimarias, view.Model.Tabla, view.Model.Regla, idRegla);

                    seleccionado.DocumentoAD = sbUpdate;
                }
                catch (Exception ex)
                {
                }
            }

            if(!string.IsNullOrEmpty(seleccionado.DocumentoAD))
                richTextBoxSql.Document.Blocks.Add(new Paragraph(new Run((seleccionado.DocumentoAD))));
        }

    }
}

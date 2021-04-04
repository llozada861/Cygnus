using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Estandar;
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

namespace Cygnus2_0.Pages.Settings.Estandar
{
    /// <summary>
    /// Lógica de interacción para CRUDEstandarUserControl.xaml
    /// </summary>
    public partial class CRUDEstandarUserControl : UserControl
    {
        private const string cstAdicionar = "Adicionar";
        private const string cstModificar = "Modificar";
        private Handler handler;
        private EstandarViewModel view;
        public CRUDEstandarUserControl()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new EstandarViewModel(handler);

            DataContext = view;
            InitializeComponent();
            btnModif.Content = cstAdicionar;
            grAddMod.Header = cstAdicionar;
            txtKey.IsEnabled = true;
        }
        private void dataGridDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridDatos.SelectedItem == null)
            {
                btnModif.Content = cstAdicionar;
                grAddMod.Header = cstAdicionar;
                txtKey.IsEnabled = true;
                btnElimi.Visibility = Visibility.Hidden;
                return;
            }
            
            richTextBoxProce.Document.Blocks.Clear();
            richTextBoxProce.Document.Blocks.Add(new Paragraph(new Run(((SelectListItem)dataGridDatos.SelectedItem).Observacion)));
            btnModif.Content = cstModificar;
            grAddMod.Header = cstModificar;
            txtKey.IsEnabled = false;
            btnElimi.Visibility = Visibility.Visible;
        }
        private void btnModif_Click(object sender, RoutedEventArgs e)
        {
            string accion = btnModif.Content.ToString();
            string value = new TextRange(richTextBoxProce.Document.ContentStart, richTextBoxProce.Document.ContentEnd).Text;

            try
            {
                if (accion.Equals(cstAdicionar))
                {
                    if (String.IsNullOrEmpty(view.Model.Estandar.Value))
                    {
                        handler.MensajeError("Debe ingresar una llave.");
                        return;
                    }

                    if (String.IsNullOrEmpty(view.Model.Estandar.Text))
                    {
                        handler.MensajeError("Debe ingresar una Observación.");
                        return;
                    }

                    if (String.IsNullOrEmpty(value))
                    {
                        handler.MensajeError("Debe ingresar el estándar html.");
                        return;
                    }

                    handler.CursorWait();
                    view.pAdicionarEstandar(value);
                    handler.CursorNormal();
                }
                else
                {
                    handler.CursorWait();
                    view.pModificarEstandar(value);
                    handler.CursorNormal();
                }
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }

            pLimpiar();
        }
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            pLimpiar();
        }
        private void btnElimi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (handler.MensajeConfirmacion("Seguro que quiere ELIMINAR el estándar [" + view.Model.Estandar.Value + " - " + view.Model.Estandar.Text + "]?") == "Y")
                {
                    handler.CursorWait();
                    view.pEliminarEstandar();
                    handler.CursorNormal();
                    pLimpiar();
                }
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
        public void pLimpiar()
        {
            richTextBoxProce.Document.Blocks.Clear();
            btnModif.Content = cstAdicionar;
            grAddMod.Header = cstAdicionar;
            txtKey.IsEnabled = true;
            btnElimi.Visibility = Visibility.Hidden;
            view = new EstandarViewModel(handler);
            DataContext = view;
        }
    }
}

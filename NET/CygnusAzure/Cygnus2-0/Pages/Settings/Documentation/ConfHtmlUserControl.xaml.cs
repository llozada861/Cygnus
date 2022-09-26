using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
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

namespace Cygnus2_0.Pages.Settings.Documentation
{
    /// <summary>
    /// Interaction logic for ConfHtmlUserControl.xaml
    /// </summary>
    public partial class ConfHtmlUserControl : UserControl
    {
        private Handler handler;
        public ConfHtmlUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            handler.ListaDocHtml.Add(new DocumentacionHTML() { Empresa = handler.ConfGeneralView.Model.Empresa.Codigo,ListaTipoHTml = new System.Collections.ObjectModel.ObservableCollection<SelectListItem>(handler.ListaTipoHTml) });
        }

        private void btnGuar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DocumentacionHTML item in handler.ListaDocHtml)
                {
                    if (String.IsNullOrEmpty(item.TagIni))
                    {
                        handler.MensajeError("Debe ingresar un tag de Inicio.");
                        return;
                    }

                    if (String.IsNullOrEmpty(item.TagFin))
                    {
                        handler.MensajeError("Debe ingresar un tag de Fin.");
                        return;
                    }

                    if (String.IsNullOrEmpty(item.Tipo))
                    {
                        handler.MensajeError("Debe ingresar un tipo.");
                        return;
                    }

                    if (item.Codigo > 0)
                    {
                        SqliteDAO.pActualizaObjeto(item);
                    }
                    else
                    {
                        SqliteDAO.pGuardarConfHtml(item);
                    }
                }

                pCargarLista();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            btnGuar.Visibility = Visibility.Hidden;
        }

        private void btnElim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (handler.DocHTMLSeleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar la documentación [" + handler.DocHTMLSeleccionado.TagIni + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaObjeto(handler.DocHTMLSeleccionado);
                    handler.ListaDocHtml.Remove(handler.DocHTMLSeleccionado);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            btnGuar.Visibility = Visibility.Hidden;
        }
        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnGuar.Visibility = Visibility.Visible;
        }
        public void pCargarLista()
        {
            handler.ListaDocHtml.Clear();
            SqliteDAO.pDocumentacionHtml(handler);
        }
    }
}

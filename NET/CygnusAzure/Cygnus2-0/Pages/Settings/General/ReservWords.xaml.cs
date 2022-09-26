using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Model.Settings;
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
    /// Interaction logic for ReservWords.xaml
    /// </summary>
    public partial class ReservWords : UserControl
    {
        private Handler handler;
        public ReservWords()
        {
            InitializeComponent();
            
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            DataContext = handler;
        }

        private void tbnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (handler.PalabraSeleccionada == null)
            {
                handler.MensajeError("Debe ingresar una palabra reservada.");
                return;
            }

            try
            {
                SqliteDAO.pGuardarPalabra(handler.PalabraSeleccionada.Palabra.Trim().ToLower());
                //pCargarLista();
                btnGuar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (handler.PalabraSeleccionada != null && handler.MensajeConfirmacion("Está seguro que desea borrar la palabra [" + handler.PalabraSeleccionada.Palabra + "]") == "Y" && SqliteDAO.pExistePalabra(handler.PalabraSeleccionada))
                {
                    SqliteDAO.pEliminaPalabra(handler.PalabraSeleccionada.Palabra.Trim());
                }

                handler.ListaPalabrasReservadas.Remove(handler.PalabraSeleccionada);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pCargarLista()
        {
            handler.ListaPalabrasReservadas.Clear();
            SqliteDAO.pListaPalabrasReservadas(handler);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PalabrasClaves nuevo = new PalabrasClaves();
            handler.ListaPalabrasReservadas.Add(nuevo);
            btnGuar.Visibility = Visibility.Visible;
        }
    }
}

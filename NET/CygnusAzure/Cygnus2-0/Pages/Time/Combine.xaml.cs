using Cygnus2_0.General;
using Cygnus2_0.General.Times;
using Cygnus2_0.ViewModel.Time;
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
    /// Interaction logic for Combine.xaml
    /// </summary>
    public partial class Combine : Window
    {
        private Handler handler;
        private TimesViewModel view;
        private TareaHoja tarea;
        private Boolean modifica;

        public Combine(TimesViewModel view, Handler handler, TareaHoja tarea)
        {
            this.view = view;
            this.handler = handler;
            DataContext = view;
            this.tarea = tarea;
            InitializeComponent();

            view.Model.TareaOrigen = tarea;
            txtOrigen.Text = view.Model.TareaOrigen.DescripcionPlus;
            txtOrigen.IsEnabled = false;
        }

        public Boolean Modifica
        {
            get { return modifica; }
            set { modifica = value; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Modifica = true;

            if (AppearanceManager.Current.ThemeSource == AppearanceManager.DarkThemeSource)
            {
                pWindow.Background = Brushes.DimGray;
            }
            else
            {
                pWindow.Background = Brushes.WhiteSmoke;
            }
        }

        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            if (view.Model.TareaDestino == null)
            {
                handler.MensajeError("Debe seleccionar una tarea destino.");
                return;
            }

            if(view.Model.TareaOrigen.IdAzure.Equals(view.Model.TareaDestino.IdAzure))
            {
                handler.MensajeError("Debe seleccionar una tarea destino diferente al origen.");
                return;
            }

            try
            {
                if (handler.MensajeConfirmacion("Está seguro que desea combinar la tarea Origen: "+ view.Model.TareaOrigen.DescripcionPlus+" con la tarea Destino: "+ view.Model.TareaDestino.DescripcionPlus+" ?") == "Y")
                {
                    //handler.DAO.pCombinarTareas(view.Model.TareaOrigen, view.Model.TareaDestino);
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Modifica = false;
            this.Close();
        }
    }
}

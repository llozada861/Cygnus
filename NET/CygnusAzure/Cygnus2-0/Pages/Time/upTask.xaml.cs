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
using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Time;
using Cygnus2_0.General.Times;
using FirstFloor.ModernUI.Presentation;
using res = Cygnus2_0.Properties.Resources;


namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Interaction logic for upTask.xaml
    /// </summary>
    public partial class upTask : Window
    {
        private Handler handler;
        private TimesViewModel view;
        private TareaHoja tarea;
        private Boolean modifica;

        public upTask(TimesViewModel view, Handler handler,TareaHoja tarea)
        {
            this.view = view;
            this.handler = handler;
            DataContext = view;
            this.tarea = tarea;
            InitializeComponent();
        }

        public Boolean Modifica
        {
            get { return modifica; }
            set { modifica = value; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (rdbNueva.IsChecked == true)
            {
                view.OnAdd(tarea,"N");
            }
            else
            {
                view.OnAdd(tarea,"P");
            }

            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Modifica = false;
            this.Close();
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

            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;

            if (this.tarea != null)
            {
                rdbNueva.IsChecked = true;
                rdbPreder.IsEnabled = false;
            }
            else
            {
                rdbPreder.IsChecked = true;
                rdbPreder.IsEnabled = true;
            }
        }

        private void chkAzure_Checked(object sender, RoutedEventArgs e)
        {
             lblAzureText.Visibility = Visibility.Visible;
             txAzureText.Visibility = Visibility.Visible;
            
        }

        private void chkAzure_Unchecked(object sender, RoutedEventArgs e)
        {
            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;
        }

        private void rdbPreder_Checked(object sender, RoutedEventArgs e)
        {
            pOcultarNuevaTarea();
            pMostrarPrederteminado();
        }

        private void rdbNueva_Checked(object sender, RoutedEventArgs e)
        {
            pOcultarPrederterminada();
            pMostrarNueva();
        }

        public void pOcultarNuevaTarea()
        {
            CasoText.Visibility = Visibility.Hidden;
            chkAzure.Visibility = Visibility.Hidden;
            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;
        }
        public void pMostrarPrederteminado()
        {
            comboBoxPred.Visibility = Visibility.Visible;
        }
        public void pMostrarNueva()
        {
            CasoText.Visibility = Visibility.Visible;
            chkAzure.Visibility = Visibility.Visible;
            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;
        }
        public void pOcultarPrederterminada()
        {
            comboBoxPred.Visibility = Visibility.Hidden;
        }
    }
}

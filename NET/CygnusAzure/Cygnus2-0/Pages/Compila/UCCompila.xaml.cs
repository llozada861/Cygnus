using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Compila;
using FirstFloor.ModernUI.Windows;
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
using FirstFloor.ModernUI.Windows.Navigation;
using System.Security.Permissions;
using System.IO;
using System.Diagnostics;
using Cygnus2_0.Pages.General;

namespace Cygnus2_0.Pages.Compila
{
    /// <summary>
    /// Interaction logic for UCCompila.xaml
    /// </summary>
    //[PrincipalPermission(SecurityAction.Demand)]
    public partial class UCCompila : UserControl,IContent
    {
        private Handler handler;
        private CompilaViewModel compilaViewModel;
        public UCCompila()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            compilaViewModel = new CompilaViewModel(handler);

            DataContext = compilaViewModel;
            InitializeComponent();

            compilaViewModel.ArchivosCompilados = "0";
            compilaViewModel.ArchivosDescompilados = "0";
        }

        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            string[] DropPath;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    DropPath = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                    compilaViewModel.pListaArchivos(DropPath,"C");
                    //pSetValorCombo();
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        private void pSetValorCombo()
        {
            //DataGridComboBoxColumn combo = dataGridArchCompila.Columns[1];
        }
        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            compilaViewModel.OnClean("");
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            bool Noselec = false;

            try
            {
                if (compilaViewModel.Usuario == null)
                {
                    handler.MensajeError("Debe ingresar el usuario.");
                    return;
                }

                foreach (Archivo archivo in compilaViewModel.ListaArchivosCargados)
                {
                    if (string.IsNullOrEmpty(archivo.Tipo))
                    {
                        Noselec = true;
                    }
                }

                if (Noselec)
                {
                    handler.MensajeError("Todos los archivos deben tener un tipo. Seleccione un tipo para el archivo.");
                    return;
                }

                if(compilaViewModel.ListaArchivosCargados.Count == 0)
                {
                    handler.MensajeError("No hay archivos para compilar");
                    return;
                }

                handler.CursorWait();
                compilaViewModel.pCompilar();

                handler.CursorNormal();
                handler.MensajeOk("Proceso terminó con éxito!");
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
    }
}

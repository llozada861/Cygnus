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
    public partial class UCSonar : UserControl,IContent
    {
        private Handler handler;
        private CompilaViewModel compilaViewModel;
        public UCSonar()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            compilaViewModel = new CompilaViewModel(handler);

            DataContext = compilaViewModel;
            InitializeComponent();

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
                    compilaViewModel.pListaArchivos(DropPath,"G");
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
            try
            {
                if (compilaViewModel.Codigo == null)
                {
                    handler.MensajeError("Debe ingresar el número de caso.");
                    return;
                }

                if (compilaViewModel.HU == null)
                {
                    handler.MensajeError("Debe ingresar el número de la HU.");
                    return;
                }

                if (compilaViewModel.ListaArchivosCargados.Count == 0)
                {
                    handler.MensajeError("No hay archivos para analizar");
                    return;
                }

                handler.CursorWait();

                List<string> salida = compilaViewModel.pSonar();
                System.Console.WriteLine(salida);

                string exito = salida.Find(x => x.IndexOf("ANALYSIS SUCCESSFUL, you can browse") > -1);

                StringBuilder salidaBuild = new StringBuilder();

                foreach(string linea in salida)
                {
                    salidaBuild.AppendLine(linea);
                }

                if (exito != null)
                {
                    string[] vecExito = exito.Split(' ');
                    string url = vecExito[vecExito.Length - 1];
                    Process.Start(url);
                }

                handler.CursorNormal();

                UserControl log = new UserControlLog(salidaBuild);
                WinImage request = new WinImage(log, "Traza");
                request.ShowDialog();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
    }
}

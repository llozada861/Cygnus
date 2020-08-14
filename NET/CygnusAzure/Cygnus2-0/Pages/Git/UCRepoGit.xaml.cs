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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Compila
{
    /// <summary>
    /// Interaction logic for UCCompila.xaml
    /// </summary>
    //[PrincipalPermission(SecurityAction.Demand)]
    public partial class UCRepoGit : UserControl,IContent
    {
        private Handler handler;
        private CompilaViewModel compilaViewModel;
        public UCRepoGit()
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

                if (compilaViewModel.Comentario == null)
                {
                    handler.MensajeError("Debe ingresar el comentrio para el commit.");
                    return;
                }

                if (compilaViewModel.ListaArchivosCargados.Count == 0)
                {
                    handler.MensajeError("No hay archivos para versionar");
                    return;
                }

                if (!compilaViewModel.ListaArchivosCargados.ToList().Exists(x => x.FileName.ToUpper().StartsWith("OPENDATOSEPM")))
                {
                    handler.MensajeError("Es importante entregar el documento de arquitectura. Por favor adicionar.");
                    return;
                }

                handler.CursorWait();

                List<SelectListItem> archivosEvaluar = new List<SelectListItem>();

                foreach (Archivo archivo in compilaViewModel.ListaArchivosCargados)
                {
                    archivosEvaluar.Add(new SelectListItem { Text = archivo.Ruta, Value = archivo.FileName });
                }

                string rama = RepoGit.pVersionarDatos(handler.RutaGitDatos, compilaViewModel.HU, compilaViewModel.Codigo,compilaViewModel.Comentario,handler.ConnViewModel.Correo, archivosEvaluar);

                handler.CursorNormal();
                handler.MensajeOk("Versionamiento exitoso! Rama creada ["+ rama+"]");
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
    }
}

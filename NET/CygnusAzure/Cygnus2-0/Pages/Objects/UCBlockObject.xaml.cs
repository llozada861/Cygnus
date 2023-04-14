using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Objects;
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
using Cygnus2_0.Pages.General;

namespace Cygnus2_0.Pages.Objects
{
    /// <summary>
    /// Interaction logic for UCBlockObject.xaml
    /// </summary>
    public partial class UCBlockObject : UserControl,IContent
    {
        private Handler handler;
        private BlockViewModel blockViewModel;
        public UCBlockObject()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            blockViewModel = new BlockViewModel(handler);

            DataContext = blockViewModel;
            InitializeComponent();
        }
        private void dataGridObjetos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridObjetos.SelectedItem == null)
                return;

            Archivo objeto = dataGridObjetos.SelectedItem as Archivo;

            if (objeto.Observacion == "Y")
            {
                handler.MensajeError("El objeto se encuentra bloqueado por el usuario ["+ objeto.Usuario+"]");
                return;
            }

            blockViewModel.pAdicionaObjeto(objeto);
        }

        private void dataGridObjsDesbloq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            blockViewModel.pRefrescaConteo();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            blockViewModel.OnClean("");
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
        protected void AuListaRepoGit_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            args.DataSource = blockViewModel.Model.ListaBD.Where((hu, match) => hu.Displayname.ToLower().Contains(args.Pattern.ToLower()));
        }

        private void AucomboBoxRepo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            blockViewModel.Model.ListaArchivosEncontrados.Clear();
        }
    }
}

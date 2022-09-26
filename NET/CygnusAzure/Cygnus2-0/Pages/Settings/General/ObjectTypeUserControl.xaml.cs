using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Pages.General;
using Cygnus2_0.ViewModel.Objects;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Settings.General
{
    /// <summary>
    /// Interaction logic for ObjectTypeUserControl.xaml
    /// </summary>
    public partial class ObjectTypeUserControl : UserControl
    {
        private Handler handler;
        private TipoObjetosVM viewModel;
        public ObjectTypeUserControl()
        {
            InitializeComponent();
            handler = ((MainWindow)Application.Current.MainWindow).Handler;
            viewModel = new TipoObjetosVM(handler);
            DataContext = viewModel;
        }

        private void TabPrincipal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = true;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel.PrincipalSeleccionado != null) 
            { 
                viewModel.ListaRutas = new System.Collections.ObjectModel.ObservableCollection<Model.Objects.RutaObjetos>(handler.ListaRutas.Where(x => x.TipoObjeto == viewModel.PrincipalSeleccionado.Codigo));
                viewModel.ListaEncabezadoObjetos = new System.Collections.ObjectModel.ObservableCollection<Model.Objects.HeadModel>(handler.ListaEncabezadoObjetos.Where(x => x.Tipo == viewModel.PrincipalSeleccionado.Codigo));
                viewModel.ListaPermisosObjeto = new System.Collections.ObjectModel.ObservableCollection<Model.Objects.PermisosObjeto>(handler.ListaPermisosObjeto.Where(x=>x.TipoObjeto == viewModel.PrincipalSeleccionado.Codigo && x.Empresa == handler.ConfGeneralView.Model.Empresa.Codigo));
            }
            viewModel.TabPrincipal = true;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }

        private void dgDatahijo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = true;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }
        private void dgDataHijo2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = true;
            viewModel.TabHijo3 = false;
        }
        private void TabPrincipal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = true;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }

        private void Tabhijo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = true;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }

        private void TabItemHijo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = true;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }

        private void TabHijo2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = true;
            viewModel.TabHijo3 = false;
        }
        private void TabHijo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = true;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = false;
        }

        private void TabHijo2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = true;
            viewModel.TabHijo3 = false;
        }

        private void TabItemHijo3_MouseUp_(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = true;
        }

        private void TabHijo3_MouseLeftButtonDown_(object sender, MouseButtonEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = true;
        }

        private void dgDatahijo3_SelectionChanged_(object sender, SelectionChangedEventArgs e)
        {
            viewModel.TabPrincipal = false;
            viewModel.TabHijo = false;
            viewModel.TabHijo2 = false;
            viewModel.TabHijo3 = true;
        }
    }
}

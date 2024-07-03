using Cygnus2_0.ViewModel.Aplica;
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

namespace Cygnus2_0.Pages.Aplica
{
    /// <summary>
    /// Lógica de interacción para SelectTipoEntregaUC.xaml
    /// </summary>
    public partial class SelectTipoEntregaUC : UserControl
    {
        GenerateAplicaViewModel generateAplicaViewModel;
        public SelectTipoEntregaUC(GenerateAplicaViewModel generateAplicaViewModel)
        {
            this.generateAplicaViewModel = generateAplicaViewModel;
            InitializeComponent();
        }

        private void rdbDatos_Click(object sender, RoutedEventArgs e)
        {
            generateAplicaViewModel.Model.Datos = (bool)rdbDatos.IsChecked;
            generateAplicaViewModel.Model.Objetos = false;
            rdbObjetos.IsChecked = false;
        }

        private void rdbObjetos_Click(object sender, RoutedEventArgs e)
        {
            generateAplicaViewModel.Model.Objetos = (bool)rdbObjetos.IsChecked;
            generateAplicaViewModel.Model.Datos = false;
            rdbDatos.IsChecked = false;
        }
    }
}

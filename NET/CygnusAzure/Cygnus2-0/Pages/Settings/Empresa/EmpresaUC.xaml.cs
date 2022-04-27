using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Model.Empresa;
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

namespace Cygnus2_0.Pages.Settings.Empresa
{
    /// <summary>
    /// Lógica de interacción para EmpresaUC.xaml
    /// </summary>
    public partial class EmpresaUC : UserControl
    {
        private Handler handler;
        public EmpresaUC()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            InitializeComponent();
            DataContext = handler.ConfGeneralView.Model;
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnGuar.Visibility = Visibility.Visible;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            EmpresaModel nuevEmpresa = new EmpresaModel();
            nuevEmpresa.Defecto = "N";
            handler.ConfGeneralView.Model.ListaEmpresas.Add(nuevEmpresa);
        }

        private void btnGuar_Click(object sender, RoutedEventArgs e)
        {
            foreach(EmpresaModel empresa in handler.ConfGeneralView.Model.ListaEmpresas)
            {
                if(SqliteDAO.pExisteEmpresa(empresa.Codigo))
                    SqliteDAO.pActualizaEmpresa(empresa);
                else 
                    if(!string.IsNullOrEmpty(empresa.Codigo))
                        SqliteDAO.pInsertaEmpresa(empresa);
            }

            SqliteDAO.pListaEmpresas(handler);
            btnGuar.Visibility = Visibility.Hidden;
        }

        private void btnElim_Click(object sender, RoutedEventArgs e)
        {
            EmpresaModel eliminar= handler.ConfGeneralView.Model.EmpresaSeleccionada;

            if (eliminar != null && handler.MensajeConfirmacion("Seguro que desea eliminar la empresa [" + eliminar.Codigo + " - " + eliminar.Descripcion + "] ?") == "Y")
            {
                SqliteDAO.pEliminaEmpresa(eliminar);
                handler.ConfGeneralView.Model.ListaEmpresas.Remove(eliminar);                
            }

            btnGuar.Visibility = Visibility.Hidden;
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Pages.Settings.AdminGeneral;
using Cygnus2_0.Pages.Settings.AzureData;
using Cygnus2_0.Pages.Settings.Database;
using Cygnus2_0.Pages.Settings.Documentation;
using Cygnus2_0.Pages.Settings.Empresa;
using Cygnus2_0.Pages.Settings.Estandar;
using Cygnus2_0.Pages.Settings.General;
using Cygnus2_0.Pages.Settings.Git;
using Cygnus2_0.Pages.Settings.Sonar;
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

namespace Cygnus2_0.Pages.Settings.Treeview
{
    /// <summary>
    /// Interaction logic for TreeViewWpf.xaml
    /// </summary>
    public partial class TreeViewWpf : UserControl
    {
        private Handler handler;
        private MainWindow myWin;
        TabItem _tabUserPage;

        public TreeViewWpf()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            DataContext = handler;
            InitializeComponent();
            TreeViewItem itemA = (TreeViewItem)TreeViewApp.Items[0];
            itemA.IsSelected = true;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UserControl userControls;
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;

            GridMain.Children.Clear();

            int tipo = Convert.ToInt16(item.Uid);

            switch (tipo)
            {
                case 0:
                    userControls = new Appearance();
                    GridMain.Children.Add(userControls);
                    break;
                case 1:
                    userControls = new Appearance();
                    GridMain.Children.Add(userControls);
                    break;
                case 2:
                    userControls = new UCGeneral();
                    GridMain.Children.Add(userControls);
                    break;
                case 3:
                    userControls = new ReservWords();
                    GridMain.Children.Add(userControls);
                    break;
                case 4:
                    userControls = new ObjectTypeUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 5:
                    userControls = new EncabezadosUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 6:
                    userControls = new PathsUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 30:
                    userControls = new UCConection();
                    GridMain.Children.Add(userControls);
                    break;
                case 31:
                    userControls = new UCConection();
                    GridMain.Children.Add(userControls);
                    break;
                case 32:
                    userControls = new GrantsUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 33:
                    userControls = new UserBDUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 60:
                    userControls = new ConfHtmlUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 62:
                    userControls = new ConfHtmlUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 63:
                    userControls = new HtmlUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 64:
                    userControls = new CRUDEstandarUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                case 90:
                    userControls = new About();
                    GridMain.Children.Add(userControls);
                    break;
                case 91:
                    userControls = new About();
                    GridMain.Children.Add(userControls);
                    break;
                case 92:
                    userControls = new UCUpdate();
                    GridMain.Children.Add(userControls);
                    break;
                case 100:
                    userControls = new Azure();
                    GridMain.Children.Add(userControls);
                    break;
                case 101:
                    userControls = new UserControlSonar();
                    GridMain.Children.Add(userControls);
                    break;
                case 102:
                    userControls = new UserControlGit();
                    GridMain.Children.Add(userControls);
                    break;
                case 103:
                    userControls = new EmpresaUserControl();
                    GridMain.Children.Add(userControls);
                    break;
                default:
                    break;                      
            }

            
            /*if (item.Uid.Equals("0") || item.Uid.Equals("1"))
            {    
                var userControls = new Appearance();
                GridMain.Children.Add(userControls);
            }

            if ( item.Uid.Equals("2"))
            {
                var userControls = new UCGeneral();
                GridMain.Children.Add(userControls);
            }

            if (item.Header.Equals("Conexión"))
            {
                var userControls = new UCConection();
                GridMain.Children.Add(userControls);
            }*/


        }
    }
}

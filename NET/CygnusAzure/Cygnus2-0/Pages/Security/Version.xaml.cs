using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Cygnus2_0.Pages.Security
{
    /// <summary>
    /// Interaction logic for OneDriveCred.xaml
    /// </summary>
    public partial class Version : UserControl
    {
        private Handler handler;
        private MainWindow myWin;
        public Version()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;
            InitializeComponent();
        }

        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtZip.Text))
                {
                    handler.MensajeError("Cargue el zip");
                    return;
                }

                if (string.IsNullOrEmpty(txtVersion.Text))
                {
                    handler.MensajeError("Ingrese la version");
                    return;
                }

                handler.CursorWait();

                string filename = System.IO.Path.GetFileName(txtZip.Text);

                using (Stream fs = File.OpenRead(txtZip.Text))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bytes = br.ReadBytes((Int32)fs.Length);

                        handler.DAO.pCargaVersion(bytes, filename, txtVersion.Text);
                    }
                }

                handler.CursorNormal();

                handler.MensajeOk("Proceso terminó con éxito");
                txtZip.Text = "";
                txtVersion.Text = "";
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }

            /*string cadena = txtUrl.Text + "|" + txtUser.Text + "|" + txtPass.Text + "|" + txtVer.Text + "|" + txtZip.Text;
            string cadenaWrap = EncriptaPass.Encriptar(cadena);
            handler.pGuardaArchivo(res.ArchivoCredenciales, cadenaWrap);*/
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtZip.Text = "";
            txtVersion.Text = "";
        }

        private void BtnCargar_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtZip.Text = openFileDialog.FileName; //File.ReadAllText(openFileDialog.FileName);

        }
    }
}

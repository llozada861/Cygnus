using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
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
using UpdaterDB.DropBox;
using res = UpdaterDB.Properties.Resources;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool called = true;
        private string tempDownloadFolder = "";
        private string processToEnd = "";
        private string downloadFile = "";
        private string URL = "";
        private string destinationFolder = "";
        private string Tipo = "";
        private string updateFolder = Environment.CurrentDirectory + @"\updates\";
        private string postProcessFile = "";
        private string postProcessCommand = "";
        private string nombre;
        private byte[] myFile;
        private string path;
        string strAppKey = "9bw0lzlt6zkjqh4";
        DropBoxBase DBB;

        public MainWindow()
        {
            InitializeComponent();
            path = System.IO.Path.Combine(Environment.CurrentDirectory, res.CarpetaBD);
            DBB = new DropBoxBase(strAppKey, "CygnusNet");       
        }

        public string RutaInstalador { set; get; }
        public string ArchivoVersion { set; get; }
        public string Version { set; get; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();

            if (called)
            {
                Show();

                BackgroundWorker bw = new BackgroundWorker();

                bw.DoWork -= new DoWorkEventHandler(backgroundWorker);
                bw.DoWork += new DoWorkEventHandler(backgroundWorker);
                bw.ProgressChanged += worker_ProgressChanged;
                bw.WorkerSupportsCancellation = true;
                bw.WorkerReportsProgress = true;
                bw.RunWorkerAsync();
            }            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;

            if(pbStatus.Value == pbStatus.Maximum)
            {
                this.Close();
            }
        }

        private void backgroundWorker(object sender, DoWorkEventArgs e)
        {           
            preDownload();

            (sender as BackgroundWorker).ReportProgress(10);
            Thread.Sleep(100);

            if (called)
            {
                try
                {
                    Process[] processes = Process.GetProcesses();

                    foreach (Process process in processes)
                    {
                        if (process.ProcessName == processToEnd)
                        {
                            process.Kill();
                        }
                    }

                }
                catch (Exception)
                { }

                pActualiza();

                //(sender as BackgroundWorker).ReportProgress(40);
                Thread.Sleep(8000);

                unZip(tempDownloadFolder + downloadFile, tempDownloadFolder);

                (sender as BackgroundWorker).ReportProgress(60);
                Thread.Sleep(100);

                moveFiles();
                (sender as BackgroundWorker).ReportProgress(80);
                Thread.Sleep(100);

                (sender as BackgroundWorker).ReportProgress(100);
                Thread.Sleep(100);

                postProcessFile = "Cygnus";
                postDownload();

                Thread.Sleep(100);
            }
            //Close();
        }

        private void unpackCommandline()
        {
            string cmdLn = "";

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                cmdLn += arg;
            }

            if (cmdLn.IndexOf('|') > -1)
            {
                string[] tmpCmd = cmdLn.Split('|');

                for (int i = 1; i < tmpCmd.GetLength(0); i++)
                {
                    //if (tmpCmd[i] == "downloadFile") downloadFile = tmpCmd[i + 1];
                    //if (tmpCmd[i] == "URL") URL = tmpCmd[i + 1];
                    if (tmpCmd[i] == "Tipo") Tipo = tmpCmd[i + 1];
                    if (tmpCmd[i] == "processToEnd") processToEnd = tmpCmd[i + 1];
                    if (tmpCmd[i] == "postProcess") postProcessFile = tmpCmd[i + 1];
                    i++;
                }
                destinationFolder = Environment.CurrentDirectory;
            }      
        }

        public void unZip(string file, string unZipTo)
        {            
            try
            {
                ZipFile.ExtractToDirectory(file, unZipTo);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }


        private void preDownload()
        {
            if (!Directory.Exists(updateFolder)) Directory.CreateDirectory(updateFolder);

            tempDownloadFolder = updateFolder + DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + @"\";

            if (Directory.Exists(tempDownloadFolder))
            {
                Directory.Delete(tempDownloadFolder, true);
            }

            Directory.CreateDirectory(tempDownloadFolder);

            unpackCommandline();
        }

        private void postDownload()
        {
            //string cmdLn = "";
            //cmdLn += "|Updater|Y";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = postProcessFile;
            //startInfo.Arguments = cmdLn;
            Process.Start(startInfo);
        }

        public void pDescargarArchivosRed()
        {
            try
            {
                File.Copy(URL + "\\" + downloadFile, tempDownloadFolder + downloadFile, true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        

        public void SetLabel(string texto)
        {
            label.Content = texto;
        }


        private void moveFiles()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(tempDownloadFolder);
                FileInfo[] files = di.GetFiles();
                string archivo_origen;
                string archvo_destino;

                //MessageBox.Show("Inicio destinationFolder: "+ destinationFolder+ ", tempDownloadFolder: "+ tempDownloadFolder);

                foreach (FileInfo fi in files)
                {
                    if (fi.Name != downloadFile)
                    {
                        archvo_destino = System.IO.Path.Combine(destinationFolder, fi.Name);
                        archivo_origen = System.IO.Path.Combine(tempDownloadFolder, fi.Name);

                        if (File.Exists((string)destinationFolder))
                        {
                            File.Delete((string)destinationFolder);
                        }

                        //MessageBox.Show("Archivo: "+ fi.Name+ ", archvo_destino: " + archvo_destino + ", archivo_origen: " + archivo_origen);

                        File.Copy(archivo_origen, archvo_destino, true);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void pActualiza()
        {
            string archivoDestino;

            ArchivoVersion = res.ArchivoVersion;

            archivoDestino = System.IO.Path.Combine(Environment.CurrentDirectory , res.ArchivoVersion);

            if (File.Exists(archivoDestino))
            {
                File.Delete(archivoDestino);
            }

            try
            {
                //DropBox
                pDescargaDropBox(ArchivoVersion, Environment.CurrentDirectory);
                pVerificaVersion("D");
                pDescargaDropBox(downloadFile, tempDownloadFolder);
            }
            catch (Exception ex)
            {
                //DropBox
                MessageBox.Show(ex.Message);
            }           
        }

        public void pVerificaVersion(string tipo)
        {
            string file;
            string sbLine;
            string[] substrings;
            char delimiter = ';';

            file = System.IO.Path.Combine(Environment.CurrentDirectory, ArchivoVersion);

            if (tipo.Equals("D"))
            {
                System.Threading.Thread.Sleep(5000);
            }

            using (StreamReader streamReader = new StreamReader(file, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();
                substrings = sbLine.Split(delimiter);
                Version= substrings[0]; //Version
                downloadFile = substrings[1]; //Instalador
            }
        }

        public void pDescargaDropBox(string archivo, string ruta)
        {
            try
            {                
                DBB.Download("/Instalador", archivo, ruta, archivo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void pCreaArchivoBD(string path, string nombre, byte[] myFile)
        {
            string archivo = System.IO.Path.Combine(path, nombre);

            if (!File.Exists((string)archivo))
            {
                using (FileStream tempFile = File.Create(archivo))
                    tempFile.Write(myFile, 0, myFile.Length);
            }
        }

        public void DatosRutaActualizacion()
        {
            string sbLine = null;
            string file = System.IO.Path.Combine(path, res.NombreArchivoRutaActualiza);
            string[] substrings;

            using (StreamReader streamReader = new StreamReader(file, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();

                while (sbLine != null)
                {
                    substrings = sbLine.Split(';');
                    RutaInstalador = substrings[0];
                    URL = RutaInstalador;
                    ArchivoVersion = substrings[1];
                    sbLine = streamReader.ReadLine();
                }
            }
        }
    }
}

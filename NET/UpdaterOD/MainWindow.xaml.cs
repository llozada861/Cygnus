using Independentsoft.Share;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows;
using res = UpdaterOD.Properties.Resources;

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
        private string path;

        public MainWindow()
        {
            InitializeComponent();
            path = System.IO.Path.Combine(Environment.CurrentDirectory, res.CarpetaBD);
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

            if (called)
            {                
                pActualiza();

                (sender as BackgroundWorker).ReportProgress(60);
                Thread.Sleep(100);

                unZip(tempDownloadFolder + downloadFile, tempDownloadFolder);

                moveFiles();
                (sender as BackgroundWorker).ReportProgress(80);
                Thread.Sleep(100);
                
                (sender as BackgroundWorker).ReportProgress(100);
                Thread.Sleep(100);

                postDownload();
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
            destinationFolder = Environment.CurrentDirectory;
            postProcessFile = "Cygnus";
            processToEnd = "Cygnus";

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

            if (!Directory.Exists(updateFolder))
                Directory.CreateDirectory(updateFolder);
            else
                Directory.Delete(updateFolder, true);

            tempDownloadFolder = updateFolder + DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + @"\";

            if (Directory.Exists(tempDownloadFolder))
            {
                Directory.Delete(tempDownloadFolder, true);
            }

            Directory.CreateDirectory(tempDownloadFolder);
        }

        private void postDownload()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = postProcessFile;
            Process.Start(startInfo);
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

                        if (System.IO.File.Exists((string)destinationFolder))
                        {
                            System.IO.File.Delete((string)destinationFolder);
                        }

                        //MessageBox.Show("Archivo: "+ fi.Name+ ", archvo_destino: " + archvo_destino + ", archivo_origen: " + archivo_origen);

                        System.IO.File.Copy(archivo_origen, archvo_destino, true);
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
            string url;
            string usuario;
            string pass;
            string rutaversion;
            string rutaZip;

            //Se obtienen las credenciales de acceso al share
            pObtenerCredencialesOD(out url, out usuario, out pass, out rutaversion, out rutaZip);

            archivoDestino = System.IO.Path.Combine(Environment.CurrentDirectory , ArchivoVersion);

            if (System.IO.File.Exists(archivoDestino))
            {
                System.IO.File.Delete(archivoDestino);
            }

            try
            {
                
                Service service = new Service(url, usuario, pass);
                //Service service = new Service("https://mvmingenieriadesoftware-my.sharepoint.com/personal/luis_lozada_mvm_com_co", "luis.lozada@mvm.com.co", "dici2018*");

                //Increase timeout to 600000 milliseconds (10 minutes). Useful when downloading large files.
                //Default value is 100000 (100 seconds).
                service.Timeout = 100000;

                pDescargaOneDrive(ArchivoVersion, Environment.CurrentDirectory, service, rutaversion);
                //pDescargaOneDrive(ArchivoVersion, Environment.CurrentDirectory, service, "/personal/luis_lozada_mvm_com_co/Documents/InstaladorCygnus/Actualizacion/Version.txt");
                //pDescargaOneDrive(downloadFile, tempDownloadFolder, service, "/personal/luis_lozada_mvm_com_co/Documents/InstaladorCygnus/Actualizacion/Cygnus.zip");
                pDescargaOneDrive(downloadFile, tempDownloadFolder, service, rutaZip);
            }
            catch (ServiceException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Error: " + ex.ErrorCode);
                Console.WriteLine("Error: " + ex.ErrorString);
                Console.WriteLine("Error: " + ex.RequestUrl);
                Console.Read();
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.Read();
            }
        }

        /*public void pVerificaVersion(string tipo)
        {
            string file;
            string sbLine;
            string[] substrings;
            char delimiter = ';';

            file = System.IO.Path.Combine(Environment.CurrentDirectory, ArchivoVersion);

            using (StreamReader streamReader = new StreamReader(file, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();
                substrings = sbLine.Split(delimiter);
                Version= substrings[0]; //Version
                downloadFile = substrings[1]; //Instalador
            }
        }*/

        public void pDescargaOneDrive(string archivo, string ruta, Service service,string archivoDescarga)
        {
            
            Stream inputStream = service.GetFileStream(archivoDescarga);                

            FileStream outputStream = new FileStream(ruta+"\\"+archivo, FileMode.CreateNew);

            using (inputStream)
            {
                using (outputStream)
                {
                    byte[] buffer = new byte[8192];
                    int len = 0;

                    while ((len = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, len);
                    }
                }
            }
        }

        public void pCreaArchivoBD(string path, string nombre, byte[] myFile)
        {
            string archivo = System.IO.Path.Combine(path, nombre);

            if (!System.IO.File.Exists((string)archivo))
            {
                using (FileStream tempFile = System.IO.File.Create(archivo))
                    tempFile.Write(myFile, 0, myFile.Length);
            }
        }
        private void pObtenerCredencialesOD(out string url, out string usuario, out string pass, out string rutaversion, out string rutaZip)
        {
            string sbLine;
            string sbLineUnWrap;
            string[] substrings;
            char delimiter = '|';
            string[] versionurl;
            string[] archivourl;

            url = "";
            usuario = "";
            pass = "";
            rutaversion = "";
            rutaZip = "";

            string ArchivoCred = System.IO.Path.Combine(Environment.CurrentDirectory, res.ArchivoCredenciales);

            using (StreamReader streamReader = new StreamReader(ArchivoCred, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();

                if (!string.IsNullOrEmpty(sbLine))
                {
                    //"https://mvmingenieriadesoftware-my.sharepoint.com/personal/luis_lozada_mvm_com_co|luis.lozada@mvm.com.co|dici2018*|/personal/luis_lozada_mvm_com_co/Documents/InstaladorCygnus/Actualizacion/Version.txt|/personal/luis_lozada_mvm_com_co/Documents/InstaladorCygnus/Actualizacion/Cygnus.zip"
                    //Se desencripta la linea con las credenciales
                    sbLineUnWrap = EncriptaPass.Desencriptar(sbLine);

                    substrings = sbLineUnWrap.Split(delimiter);
                    url = substrings[0];
                    usuario = substrings[1];
                    pass = substrings[2];
                    rutaversion = substrings[3];
                    rutaZip = substrings[4];

                    versionurl = rutaversion.Split('/');
                    archivourl = rutaZip.Split('/');

                    ArchivoVersion = versionurl[versionurl.Length-1];
                    downloadFile = archivourl[archivourl.Length-1];
                }
            }
        }
    }
}

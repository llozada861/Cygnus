using Oracle.ManagedDataAccess.Client;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows;
using res = Updater.Properties.Resources;

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
        private string downloadFile = "Cygnus.zip";
        private string URL = "";
        private string destinationFolder = "";
        private string Tipo = "";
        private string updateFolder = Environment.CurrentDirectory + @"\updates\";
        private string postProcessFile = "";
        private string path;
        private const string DBName = "Cygnus.db";

        public MainWindow()
        {
            InitializeComponent();
            path = System.IO.Path.Combine(Environment.CurrentDirectory, res.CarpetaBD);
        }

        public string RutaInstalador { set; get; }
        public string ArchivoVersion { set; get; }
        public string Version { set; get; }
        public string Usuario { set; get; }
        public string Pass { set; get; }
        public string BaseDatos { set; get; }
        public string Servidor { set; get; }
        public string Puerto { set; get; }

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
            Thread.Sleep(10000);

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
            string cmdLn = "";

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

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                cmdLn += arg;
            }

            if (cmdLn.IndexOf('|') > -1)
            {
                string[] tmpCmd = cmdLn.Split('|');

                for (int i = 1; i < tmpCmd.GetLength(0); i++)
                {
                    if (tmpCmd[i] == "usuario") Usuario = tmpCmd[i + 1];
                    if (tmpCmd[i] == "pass") Pass = tmpCmd[i + 1];
                    if (tmpCmd[i] == "servidor") Servidor = tmpCmd[i + 1];
                    if (tmpCmd[i] == "puerto") Puerto = tmpCmd[i + 1];
                    if (tmpCmd[i] == "baseDatos") BaseDatos = tmpCmd[i + 1];
                    if (tmpCmd[i] == "version") Version = tmpCmd[i + 1];
                    i++;
                }
            }
            else
            {
                pObtDatosBd();
            }
        }

        private void postDownload()
        {
            string cmdLn = "";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "Cygnus.exe";
            cmdLn += "|Up|Y";
            startInfo.Arguments = cmdLn;
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
            byte[] archivoZip = pDescargarVersionBD();
            File.WriteAllBytes(tempDownloadFolder + "\\" + downloadFile, archivoZip);
        }
       
        internal byte[] pDescargarVersionBD()
        {           
            byte[] bytes;
            OracleConnection con = RealizarConexion();

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = "select objeto from flex.ll_version where version = :actualiza";
                cmd.Parameters.Add(":actualiza", Version);
                cmd.Connection = con;

                using (OracleDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();

                    bytes = (byte[])sdr["objeto"];
                }
                con.Close();
            }

            return bytes;
        }

        public OracleConnection RealizarConexion()
        {
            OracleConnection conexionOracleSql;

            string connectionstring = OracleConnString
                                        (
                                            Servidor,
                                            Puerto,
                                            BaseDatos,
                                            Usuario,
                                            Pass
                                        );

            conexionOracleSql = new OracleConnection(connectionstring);
            conexionOracleSql.Open();
            return conexionOracleSql;
        }

        public string OracleConnString(string host, string port, string servicename, string user, string pass)
        {
            return String.Format
            (
                  "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})" +
                  "(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};",
                  host,
                  port,
                  servicename,
                  user,
                  pass
            );
        }

        public void pObtDatosBd()
        {
            string query = "select * from conection";

            using (SQLiteConnection conn = GetInstance())
            {
                using (var command = new SQLiteCommand(query, conn))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Usuario = reader.GetString(0);
                        Pass = reader.GetString(1);
                        BaseDatos = reader.GetString(2);
                        Servidor = reader.GetString(3);
                        Puerto = reader.GetString(4);
                    }
                }
            }

            Version = pObtVersionBD();
        }

        public static SQLiteConnection GetInstance()
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", DBName)
            );
            db.Open();

            return db;
        }

        public string pObtVersionBD()
        {
            string versionbd = "";

            OracleConnection con = RealizarConexion();

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandText = "SELECT version FROM flex.ll_version ORDER BY fecha_ini desc";
                cmd.Connection = con;

                using (OracleDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    versionbd = (string)sdr["version"];
                }
                con.Close();
            }

            return versionbd;
        }
    }
}

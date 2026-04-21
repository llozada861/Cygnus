using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace UpdaterGit
{
    public partial class Form1 : Form
    {
        // URL de tu XML en GitHub
        string updateUrl = "https://raw.githubusercontent.com/llozada861/Cygnus/refs/heads/Desarrollo/update.json";
        string zipUrl = "https://github.com/llozada861/Cygnus/releases/download/version_1257/Cygnus.zip";
        Button btnBuscarUpdate;
        string zipPath = Path.Combine(Path.GetTempPath(), "update.zip");
        string extractPath = Path.Combine(Path.GetTempPath(), "update");

        public Form1()
        {
            this.btnBuscarUpdate = new Button();
            InitializeComponent();
            this.btnBuscarUpdate.Text = "Buscar";

            this.Controls.Add(this.btnBuscarUpdate);
            this.btnBuscarUpdate.Click += new System.EventHandler(btnBuscarUpdate_Click);
        }

        private void btnBuscarUpdate_Click(object sender, EventArgs e)
        {
            //await CheckUpdate();

            // descargar
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(zipUrl, zipPath);
            }

            // extraer
            ZipFile.ExtractToDirectory(zipPath, extractPath);

            // copiar archivos
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var file in Directory.GetFiles(extractPath, "*", SearchOption.AllDirectories))
            {
                var dest = file.Replace(extractPath, appPath);
                File.Copy(file, dest, true);
            }

            // reiniciar app
            System.Diagnostics.Process.Start(Path.Combine(appPath, "Cygnus.exe"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public class UpdateInfo
        {
            public string Version { get; set; }
            public string Url { get; set; }
            public string Checksum { get; set; }
        }

        async Task CheckUpdate()
        {
            var client = new HttpClient();
            var json = await client.GetStringAsync(updateUrl);

            var info = JsonConvert.DeserializeObject<UpdateInfo>(json);

            Version current = new Version(Application.ProductVersion);
            Version remote = new Version(info.Version);

            if (remote > current)
            {
                // lanzar updater externo
                Process.Start("Updater.exe", info.Url);
                Application.Exit();
            }
        }
    }
}

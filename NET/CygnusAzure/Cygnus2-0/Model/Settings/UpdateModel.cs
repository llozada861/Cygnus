using Cygnus2_0.General;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;
using System.Windows.Forms;
using System.Net;

namespace Cygnus2_0.Model.Settings
{
    public class UpdateModel
    {
        private Handler handler;
        private string updateUrl = "https://raw.githubusercontent.com/llozada861/Cygnus/refs/heads/Desarrollo/update.json";
        public UpdateModel(Handler hand)
        {
            handler = hand;
        }

        #region Actualizacion
        public void pActualizaApp()
        {
            pDescargarActualizacion();
        }

        public async void pDescargarActualizacion()
        {
            await CheckUpdate();
        }

        public async Task CheckUpdate()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new HttpClient();
            var json = await client.GetStringAsync(updateUrl);

            var info = JsonConvert.DeserializeObject<UpdateInfo>(json);

            System.Version current = new System.Version(Application.ProductVersion);
            System.Version remote = new System.Version(info.Version);

            if (remote > current)
            {
                // lanzar updater externo
                Process.Start("UpdaterGit.exe", info.Url);
                Application.Exit();
            }
        }
        #endregion Actualizacion
    }
}

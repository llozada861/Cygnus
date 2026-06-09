using Cygnus2_0.General;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;
using System.Windows.Forms;
using System.Net;
using System.Linq;
using LibGit2Sharp;
using System;
using Cygnus2_0.DAO;
using Cygnus2_0.Pages.Security;
using Cygnus2_0.Model.Version;

namespace Cygnus2_0.Model.Settings
{
    public class UpdateModel
    {
        private Handler handler;
        private string updateUrl = "https://raw.githubusercontent.com/llozada861/Cygnus/refs/heads/Desarrollo/update.json";
        private string updateUrlData = "https://raw.githubusercontent.com/llozada861/Cygnus/refs/heads/Desarrollo/data.json";
        public UpdateModel(Handler hand)
        {
            handler = hand;
        }

        #region Actualizacion
        public void pActualizaApp()
        {
            pDescargarActualizacion();
        }

        public void pActualizaDataApp()
        {
            pDescargarActuaData();
        }

        public async void pDescargarActualizacion()
        {
            await CheckUpdate();
        }

        public void pDescargarActuaData()
        {
            pActualizaDatos();
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

        public void pActualizaDatos()
        {
            using (var client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var json = client.GetStringAsync(updateUrlData)
                                 .GetAwaiter()
                                 .GetResult();

                //var client = new HttpClient();
                //var json = await client.GetStringAsync(updateUrlData);

                VersionesBd jsonVer = JsonConvert.DeserializeObject<VersionesBd>(json);

                string versionBD = handler.ListaConfiguracion.Where(x => x.Text == res.KEY_VERSIONBD).FirstOrDefault().Value;
                string versionNueva = "";

                System.Version current = new System.Version(versionBD);
                System.Version remote;

                foreach (VersionBd version in jsonVer.Versiones)
                {
                    remote = new System.Version(version.Version);

                    if (remote > current)
                    {
                        AjustesBd[] query = version.Ajustes;

                        foreach (AjustesBd ajuste in query)
                        {
                            try
                            {
                                SqliteDAO.pExecuteNonQuery(ajuste.Sql);
                                versionNueva = version.Version;
                            }
                            catch (Exception ex) { }
                        }
                    }
                }



                if (!string.IsNullOrEmpty(versionNueva))
                {
                    SqliteDAO.pCreaConfiguracion(res.KEY_VERSIONBD, versionNueva);

                    MessageBox.Show("La base de datos ha sido actualizada a la versión [" + versionNueva + "]. Se va a reiniciar la aplicación.");

                    System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
        #endregion Actualizacion
    }
}

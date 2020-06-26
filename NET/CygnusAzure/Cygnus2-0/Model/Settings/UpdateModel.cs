using Cygnus2_0.General;
using System.Diagnostics;
using System.IO;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Settings
{
    public class UpdateModel
    {
        private Handler handler;
        public UpdateModel(Handler hand)
        {
            handler = hand;
        }

        #region Actualizacion
        public void pActualizaApp(string usuario, string pass,string version, string servidor, string basedatos, string puerto)
        {
            pDescargarActualizacion(usuario,pass,version,servidor,basedatos,puerto);
        }

        public static void pDescargarActualizacion(string usuario, string pass,string version, string servidor, string basedatos, string puerto)
        {
            string cmdLn = "";
            cmdLn += "|usuario|" + usuario;
            cmdLn += "|pass|" + pass;
            cmdLn += "|servidor|" + servidor;
            cmdLn += "|puerto|" + puerto;
            cmdLn += "|baseDatos|" + basedatos;
            cmdLn += "|version|" + version;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "Updater";
            startInfo.Arguments = cmdLn;
            Process.Start(startInfo);
        }
        #endregion Actualizacion
    }
}

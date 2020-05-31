using Cygnus2_0.General;
using Independentsoft.Share;
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
        public void pActualizaApp()
        {
            pDescargarActualizacion(res.RedLocal);
        }

        public static void pDescargarActualizacion(string tipo)
        {
            string cmdLn = "";
            cmdLn += "|processToEnd|Cygnus";
            cmdLn += "|postProcess|Cygnus";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "Updater";
            startInfo.Arguments = cmdLn;
            Process.Start(startInfo);
        }
        #endregion Actualizacion
    }
}

using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Refresh;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Refresh
{
    public class RefreshModel
    {
        private Handler handler;
        private string nombre;
        private byte[] myFile;
        private string startPath;
        private string nombreOut;
        private string zipPath;
        private string[] DropPath;

        public RefreshModel(Handler handler)
        {
            this.handler = handler;
        }

        public void process(RefreshViewModel view)
        {
            startPath = handler.RutaBk;
            nombreOut = DateTime.Now.Day + ""+DateTime.Now.Month+""+ DateTime.Now.Year+ "_CygnusInstall" + res.ExtensionZIP;
            zipPath = Path.Combine(Environment.CurrentDirectory, nombreOut);

            if (!Directory.Exists(handler.RutaBk))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(handler.RutaBk);
            }
            else
            {
                handler.pDropFiles(handler.RutaBk);
            }

            //Se obtienen los archivos de instalacion
            pArchivosInstall();
            //Se obtiene los datos de la base de datos
            pObtBackup(view);

            if (File.Exists((string)zipPath))
            {
                File.Delete((string)zipPath);
            }

            ZipFile.CreateFromDirectory(startPath, zipPath);
            byte[] bytes = File.ReadAllBytes(zipPath);
            handler.pGeneraArchivoRuta(nombreOut, bytes);
        }

        public void pObtBackup(RefreshViewModel view)
        {
            StringBuilder reiniciaSeq = new StringBuilder();

            nombre = "17_reiniciaSecuencia.sql";

            using (StreamWriter tempFile = new StreamWriter(Path.Combine(handler.RutaBk, nombre)))
            {
                reiniciaSeq.Append(res.ReiniciaSecuencia);
                reiniciaSeq.Replace("<nuObjetosBl>", view.ObjetosBl);
                reiniciaSeq.Replace("<nuObjetosLog>", view.ObjetosLog);
                reiniciaSeq.Replace("<nuRq>", view.onuSeqRq);
                reiniciaSeq.Replace("<nuHorasHoja>", view.onuSeqHH);
                reiniciaSeq.Replace("<nuSeqNeg>", view.onuSeqNeg);
                tempFile.WriteLine(reiniciaSeq);
            }

            pCreaArchivoIns("18_insll_credmark.sql", view.ListaInsertCM);
            pCreaArchivoIns("19_insll_usuarios.sql", view.ListaUsuarios);
            pCreaArchivoIns("20_insll_objetosbl.sql", view.ListaObjetosBl);
            pCreaArchivoIns("21_inssa_user.sql", view.ListaSaUser);
            pCreaArchivoIns("22_insge_person.sql", view.ListaPerson);
            pCreaArchivoIns("23_insll_hojas.sql", view.ListaHoja);
            pCreaArchivoIns("24_insll_requerimiento.sql", view.ListaRQ);
            pCreaArchivoIns("25_insll_horashoja.sql", view.ListaHH);
        }

        private void pArchivosInstall()
        {
            string file;
            string[] dlls = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string nombre;
            int nuIndex;

            for (int i = 0; i < dlls.Length; i++)
            {
                nombre = dlls[i];

                if (nombre.IndexOf("oracleInstall") > 0) //(nombre.EndsWith(".dll") || nombre.EndsWith(".config") || nombre.EndsWith(".exe") || nombre.Equals("od.txt"))
                {
                    nuIndex = nombre.IndexOf("oracleInstall") + 14;

                    nombre = nombre.Substring(nuIndex, nombre.Length - nuIndex);

                    file = System.IO.Path.Combine(handler.RutaBk, nombre);

                    if (!File.Exists((string)file))
                    {
                        handler.CopyResource(dlls[i], file);
                    }
                }
            }
        }

        public void pCreaArchivoIns(string nombre, List<SelectListItem> lista)
        {
            if (lista.Count() > 0)
            {
                using (StreamWriter tempFile = new StreamWriter(Path.Combine(handler.RutaBk, nombre)))
                {
                    foreach (SelectListItem item in lista)
                    {
                        tempFile.WriteLine(item.Text);
                    }

                    tempFile.WriteLine("commit;");
                }
            }
        }
    }
}

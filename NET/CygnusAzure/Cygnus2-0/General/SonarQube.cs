using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.General
{
    public static class SonarQube
    {
        public static List<string> pEjecutarSonar(string codigo, string ruta, List<SelectListItem> listaArchivosCargados, string rutaGitObj)
        {
            List<string> salida = new List<string>();

            pCrearArchivoConf(ruta, codigo, rutaGitObj, listaArchivosCargados);

            //pCopiarArchivos(Path.Combine(ruta,res.CarpetaObjetos),listaArchivosCargados);

            Process process = new Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = rutaGitObj;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c sonar-scanner";
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string line;

            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                salida.Add(line);
                //System.Console.WriteLine(line);
            }

            //string output = process.StandardOutput.ReadToEnd();
            process.Close();

            return salida;
        }

        public static void pCopiarArchivos(string ruta, List<SelectListItem> listaArchivosCargados)
        {
            string sourceFile;
            string destFile;

            pDropFiles(ruta);

            foreach (SelectListItem archivo in listaArchivosCargados)
            {
                sourceFile = Path.Combine(archivo.Text, archivo.Value);
                destFile = Path.Combine(ruta, archivo.Value);

                System.IO.File.Copy(sourceFile, destFile,true);
            }
        }

        public static void pInstalaSonar(string ruta, string usuario, string pass)
        {
            byte[] myFile;
            StringBuilder SonarConn = new StringBuilder();
            string path = ruta;

            pCrearDirectorio(path, res.CarpetaObjetos);

            //Se instala Sonar
            myFile = res.SonarZip;
            pCreaArchivoBD(path, res.ZipZonar, myFile);
            unZip(Path.Combine(path, res.ZipZonar), path);
            File.Delete(Path.Combine(path, res.ZipZonar));

            //archivo de configuración
            //pCrearArchivoConf(path,"1","1","",);

            SonarConn.Append(res.SonarConn);
            SonarConn.Replace("[usuario]", usuario);
            SonarConn.Replace("[pass]", pass);

            string rutaConf = path + "\\sonar-scanner-4.4.0.2170-windows\\conf";

            using (StreamWriter SonarC = new StreamWriter(Path.Combine(rutaConf,res.NombreSonarConn)))
            {
                SonarC.Write(SonarConn);
            }

            string variablePath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            string newPath = path + "\\sonar-scanner-4.4.0.2170-windows\\bin";

            if (variablePath.IndexOf(newPath) < 0)
            {
                string setValor = variablePath + ";" + newPath;
                Environment.SetEnvironmentVariable("Path", setValor, EnvironmentVariableTarget.User);
            }
        }

        private static void pCrearArchivoConf(string path, string codigo, string rutaObjGit, List<SelectListItem> listaArchivos)
        {
            StringBuilder SonarProperties = new StringBuilder();
            string RutaObjetos = "";

            foreach(SelectListItem archivo in listaArchivos)
            {
                RutaObjetos += archivo.Text + ",";
            }

            RutaObjetos = RutaObjetos.Substring(0, RutaObjetos.Length - 1);

            string rama = res.Feature + codigo + "_" + Environment.UserName.ToUpper();
            string archivConf = Path.Combine(rutaObjGit, res.NombreSonarProperties);

            //Se configuran los archivos
            SonarProperties.Append(res.SonarProperties);
            SonarProperties.Replace("[rama]", rama);
            SonarProperties.Replace("[ruta]", RutaObjetos);

            if(File.Exists(archivConf))
            {
                File.Delete(archivConf);
            }

            using (StreamWriter SonarProp = new StreamWriter(archivConf))
            {
                SonarProp.Write(SonarProperties);
            }
        }
        public static void pDropFiles(string ruta)
        {
            string[] DropPath = System.IO.Directory.GetFiles(ruta + "\\", "*", System.IO.SearchOption.AllDirectories);

            foreach (string dropfilepath in DropPath)
            {
                File.Delete(dropfilepath);
            }
        }

        public static void pCreaArchivoBD(string path, string nombre, byte[] myFile)
        {
            string archivo = Path.Combine(path, nombre);

            if (!File.Exists((string)archivo))
            {
                using (FileStream tempFile = File.Create(archivo))
                    tempFile.Write(myFile, 0, myFile.Length);
            }
        }

        public static void unZip(string file, string unZipTo)
        {
            try
            {
                ZipFile.ExtractToDirectory(file, unZipTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void pCrearDirectorio(string ruta, string carpeta)
        {
            string path = Path.Combine(ruta, carpeta);

            // Determine whether the directory exists.
            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
        }
    }
}

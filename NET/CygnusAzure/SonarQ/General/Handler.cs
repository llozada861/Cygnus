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
using res = SonarQ.Properties.Resources;

namespace SonarQ.General
{
    public static class Handler
    {
        public static StreamReader pEjecutarSonar(string ruta, List<SelectListItem> listaArchivosCargados)
        {
            StreamReader stream;

            pCopiarArchivos(Path.Combine(ruta,res.CarpetaObjetos),listaArchivosCargados);

            Process process = new Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = ruta;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c sonar-scanner";
            process.StartInfo.CreateNoWindow = false;

            process.Start();
            stream = process.StandardOutput;
            string output = process.StandardOutput.ReadToEnd();
            process.Close();

            return stream;
        }

        private static void pCopiarArchivos(string ruta, List<SelectListItem> listaArchivosCargados)
        {
            string sourceFile;
            string destFile;

            pDropFiles(ruta);

            foreach (SelectListItem archivo in listaArchivosCargados)
            {
                sourceFile = Path.Combine(archivo.Text, archivo.Value);
                destFile = Path.Combine(ruta, archivo.Value);

                using (StreamReader fuente = new StreamReader(sourceFile))
                {
                    using (StreamWriter origen = new StreamWriter(destFile))
                    {
                        origen.Write(fuente.ReadToEnd());
                    }
                }
            }
        }

        public static void pInstalaSonar(string ruta, string usuario, string pass)
        {
            byte[] myFile;
            StringBuilder SonarProperties = new StringBuilder();
            StringBuilder SonarConn = new StringBuilder();
            string path = Path.Combine(ruta, res.CarpetaSonar);

            pCrearDirectorio(ruta, res.CarpetaSonar);
            pCrearDirectorio(path, res.CarpetaObjetos);

            //Se instala SOnar
            myFile = res.SonarZip;
            pCreaArchivoBD(path, res.ZipZonar, myFile);
            unZip(Path.Combine(path, res.ZipZonar), path);
            File.Delete(Path.Combine(path, res.ZipZonar));

            string RutaObjetos = path + "\\" + res.CarpetaObjetos;
            RutaObjetos = RutaObjetos.Replace(@"\", @"\\");

            //Se configuran los archivos
            SonarProperties.Append(res.SonarProperties);
            SonarProperties.Replace("[rama]", usuario);
            SonarProperties.Replace("[ruta]", RutaObjetos);

            using (StreamWriter SonarProp = new StreamWriter(Path.Combine(path, res.NombreSonarProperties)))
            {
                SonarProp.Write(SonarProperties);
            }

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

using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.General
{
    public static class RepoGit
    {
        public static string pVersionarDatos(string ruta, string hu, string wo, string mensaje,string email,List<SelectListItem> listaArchivosCargados)
        {
            bool blRama = false;

            string ramaWO = wo.ToUpper();
            string rutaObjetos = Path.Combine(ruta, res.Despliegues);
            rutaObjetos = Path.Combine(rutaObjetos, ramaWO);
            string rama = res.Feature + hu + "_" + ramaWO + "_" + Environment.UserName.ToUpper();
            string MensajeCommit = ramaWO + " - " + mensaje;

            using (var repo = new Repository(@ruta))
            {
                Commands.Checkout(repo, res.RamaMasterDatos);
                Commands.Pull(repo, new Signature(Environment.UserName, email, DateTimeOffset.Now), new PullOptions());

                var branches = repo.Branches;
                foreach (Branch b in branches)
                {
                    Console.WriteLine(b.FriendlyName);

                    if (b.FriendlyName.Equals(rama))
                    {
                        blRama = true;
                    }
                }

                if (!blRama)
                {
                    repo.CreateBranch(rama);
                }

                Commands.Checkout(repo, rama);
                pCreaDirectorios(rutaObjetos);
                SonarQube.pCopiarArchivos(rutaObjetos, listaArchivosCargados);

                Commands.Stage(repo, "*");
                Commit comm = repo.Commit(MensajeCommit, new Signature(Environment.UserName, email, DateTimeOffset.Now),new Signature(Environment.UserName, email, DateTimeOffset.Now));

                Remote remote = repo.Network.Remotes["origin"];
                Branch ramaFeat = null ;

                branches = repo.Branches;
                foreach (Branch ramaFeature in branches)
                {
                    if (ramaFeature.FriendlyName.Equals(rama))
                    {
                        ramaFeat = ramaFeature;
                        break;
                    }
                }

                repo.Branches.Update(ramaFeat,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = ramaFeat.CanonicalName);

                repo.Network.Push(ramaFeat);
            }

            return rama;
        }

        /*
         *                 //Se cambia a master datos para crear el feature
                Commands.Checkout(repo, res.RamaMasterDatos);
                Commands.Pull(repo, new Signature(Environment.UserName, email, DateTimeOffset.Now), new PullOptions());
                         /*if (!blRama)
                {
                    repo.CreateBranch(rama);
                }
                
                Commands.Checkout(repo, rama);
                repo.CherryPick(comm, new Signature(Environment.UserName, email, DateTimeOffset.Now));*/
             
        public static string pClonarRepo(string ruta, string url)
        {
            pCreaDirectorios(ruta);

            string command = "Git clone " + url;

            ExecuteGitBashCommand("C:\\Program Files\\Git\\git-bash.exe", command, ruta);

            return ruta + res.CarpetaDatosGIT;
        }

        public static void pCreaDirectorios(string path)
        {
            // Determine whether the directory exists.
            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
        }

        public static void ExecuteGitBashCommand(string fileName, string command, string workingDir)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, "-c \" " + command + " \"")
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            var exitCode = process.ExitCode;

            process.Close();
        }
    }
}

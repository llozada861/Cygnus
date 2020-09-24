using Cygnus2_0.Model.Git;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string pVersionarDatos(string hu, string wo, string mensaje,string email,List<SelectListItem> listaArchivosCargados, Handler handler)
        {
            bool blRama = false;

            string ramaWO = wo.ToUpper();
            string rutaObjetos = Path.Combine(handler.RutaGitDatos, res.Despliegues);
            rutaObjetos = Path.Combine(rutaObjetos, ramaWO);
            string rama = res.Feature + hu + "_" + ramaWO + "_" + Environment.UserName.ToUpper();
            string MensajeCommit = ramaWO + " - " + mensaje;

            using (var repo = new Repository(@handler.RutaGitDatos))
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

        public static void pVersionarObjetos(ObjectGitModel gitModel, Handler handler)
        {
            bool blRama = false;

            string ramaWO = gitModel.RamaLBSeleccionada.Text.ToUpper();
            string rutaObjetos = Path.Combine(handler.RutaGitObjetos, res.Despliegues);
            rutaObjetos = Path.Combine(rutaObjetos, ramaWO);
            string ramaDll = res.Feature + gitModel.HU + "_" + ramaWO + "_" + Environment.UserName.ToUpper()+"_DLL";
            string ramaPru = res.Feature + gitModel.HU + "_" + ramaWO + "_" + Environment.UserName.ToUpper() + "_PRU";
            string ramaPdn = res.Feature + gitModel.HU + "_" + ramaWO + "_" + Environment.UserName.ToUpper() + "_PDN";
            string MensajeCommit = ramaWO + " - " +gitModel.Comentario;

            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                Commands.Checkout(repo, res.RamaProduccion);
                Commands.Pull(repo, new Signature(Environment.UserName, handler.ConnViewModel.Correo, DateTimeOffset.Now), new PullOptions());
                Commands.Checkout(repo, ramaWO);
                pCreaDirectorios(rutaObjetos);

                List<SelectListItem> archivosEvaluar = new List<SelectListItem>();

                foreach (Archivo archivo in gitModel.ListaArchivos)
                {
                    archivosEvaluar.Add(new SelectListItem { Text = archivo.Ruta, Value = archivo.FileName });
                }

                //Se copian los archivos en despliegue
                SonarQube.pCopiarArchivos(rutaObjetos, archivosEvaluar);
                //Se copian los archivos en cada ruta del repo
                pCopiarObjetosRepo(gitModel);
            }
        }

        public static void pCreaLineaBase(ObjectGitModel model, Handler handler)
        {
            bool blRama = false;

            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                Commands.Checkout(repo, res.RamaProduccion);
                Commands.Pull(repo, new Signature(Environment.UserName, handler.ConnViewModel.Correo, DateTimeOffset.Now), new PullOptions());

                var branches = repo.Branches;

                foreach (Branch b in branches)
                {
                    if (b.FriendlyName.ToUpper().Equals(model.Codigo.ToUpper()))
                    {
                        blRama = true;
                    }
                }

                if (blRama)
                {
                    throw new Exception("La línea base ya existe.");
                }

                repo.CreateBranch(model.Codigo.ToUpper());
            }
        }
        public static ObservableCollection<SelectListItem> pObtieneRamasListLB(Handler handler)
        {
            ObservableCollection<SelectListItem> listaRamas = new ObservableCollection<SelectListItem>();

            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                BranchCollection branches = repo.Branches;

                foreach (Branch b in branches)
                {
                    if(b.FriendlyName.ToUpper().IndexOf(res.Feature.ToUpper()) < 0 && b.FriendlyName.ToUpper().IndexOf("ORIGIN") < 0)
                        listaRamas.Add(new SelectListItem { Text = b.FriendlyName });
                }
            }

            return listaRamas;
        }

        public static void pSetearLineaBase(string rama, Handler handler)
        {
            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                Commands.Checkout(repo, rama);
            }
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

        public static string pClonarRepo(string ruta, string url, string rutaGitBash)
        {
            pCreaDirectorios(ruta);

            string command = "Git clone " + url;
            ExecuteGitBashCommand(rutaGitBash+"\\"+res.GitBashExe, command, ruta);

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

        public static void pCopiarObjetosRepo(ObjectGitModel gitModel)
        {

        }
    }
}

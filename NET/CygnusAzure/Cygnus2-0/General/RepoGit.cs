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
                        break;
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
            string MensajeCommit = ramaWO + " - " +gitModel.Comentario;

            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                Commands.Checkout(repo, ramaWO);
                pCreaDirectorios(rutaObjetos);

                //Se copian los archivos en cada ruta del repo
                pCopiarObjetosRepo(gitModel.ListaCarpetas.ToList(), handler.RutaGitObjetos);

                Commands.Stage(repo, "*");
                Commit comm = repo.Commit(MensajeCommit, new Signature(Environment.UserName, handler.ConnViewModel.Correo, DateTimeOffset.Now), new Signature(Environment.UserName, handler.ConnViewModel.Correo, DateTimeOffset.Now));
            }
        }

        public static void pCreaRamaRepo(Handler handler,string ramaPrincipal, string ramaCrear)
        {
            bool blRama = false;

            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                Commands.Checkout(repo, ramaPrincipal);
                //Commands.Pull(repo, new Signature(Environment.UserName, handler.ConnViewModel.Correo, DateTimeOffset.Now), new PullOptions());

                var branches = repo.Branches;
                foreach (Branch b in branches)
                {
                    Console.WriteLine(b.FriendlyName);

                    if (b.FriendlyName.Equals(ramaCrear))
                    {
                        blRama = true;
                        break;
                    }
                }

                if (!blRama)
                {
                    pActualizarRepo(handler, ramaPrincipal);
                    repo.CreateBranch(ramaCrear);
                }

                Commands.Checkout(repo, ramaCrear);
            }
        }

        public static void pActualizarRepo(Handler handler,string rama)
        {
            string command = "git pull origin " + rama;
            string RutagitBash = handler.RutaGitBash + "\\" + res.GitBashExe;
            ExecuteGitBashCommand(RutagitBash, command, handler.RutaGitObjetos);
        }

        public static void pCreaLineaBase(ObjectGitModel model, Handler handler)
        {
            bool blRama = false;

            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                Commands.Checkout(repo, res.RamaProduccion);
                //Commands.Pull(repo, new Signature(Environment.UserName, handler.ConnViewModel.Correo, DateTimeOffset.Now), new PullOptions());

                var branches = repo.Branches;

                foreach (Branch b in branches)
                {
                    if (b.FriendlyName.ToUpper().Equals(model.Codigo.ToUpper()))
                    {
                        blRama = true;
                        break;
                    }
                }

                if (!blRama)
                {
                    pActualizarRepo(handler, res.RamaProduccion);
                    repo.CreateBranch(model.Codigo.ToUpper());
                }
            }
        }

        public static void pRenombrar(Handler handler,string nombreantes, string nuevonombre)
        {
            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                var branches = repo.Branches;

                foreach (Branch b in branches)
                {
                    if (b.FriendlyName.ToUpper().Equals(nombreantes.ToUpper()))
                    {
                        repo.Branches.Rename(b, nuevonombre.ToUpper());
                        break;
                    }
                }
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

        internal static void pRemoverCambiosSonar(ObjectGitModel gitModel, Handler handler)
        {
            using (var repo = new Repository(@handler.RutaGitObjetos))
            {
                string command = "git stash -u";
                string RutagitBash = handler.RutaGitBash + "\\" + res.GitBashExe;
                ExecuteGitBashCommand(RutagitBash, command, handler.RutaGitObjetos);
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

        public static void ExecuteGitBash(string fileName, string workingDir)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, "-c \" git status \"")
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            var process = Process.Start(processStartInfo);
            //process.Close();
        }

        public static void pCopiarObjetosRepo(List<Folder> ListaCarpetas, string path)
        {
            string destino = "";
            string pathIn = "";
            string extension = "";

            foreach (Folder archivo in ListaCarpetas)
            {
                extension = System.IO.Path.GetExtension(archivo.FolderLabel);

                if (string.IsNullOrEmpty(extension))
                {
                    if (!archivo.FolderLabel.Equals(res.Carpetas))
                        pathIn = Path.Combine(path, archivo.FolderLabel);
                    else
                        pathIn = path;

                    pCreaDirectorios(pathIn);
                    pCopiarObjetosRepo(archivo.Folders, pathIn);
                }
                else
                {
                    destino = Path.Combine(path, archivo.FolderLabel);
                    System.IO.File.Copy(archivo.FullPath, destino, true);
                }
            }
        }
    }
}

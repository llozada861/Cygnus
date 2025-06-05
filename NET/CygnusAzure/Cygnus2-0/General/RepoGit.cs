using Cygnus2_0.DAO;
using Cygnus2_0.Model.Git;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Pages.General;
using Cygnus2_0.Pages.Git;
using FirstFloor.ModernUI.Windows.Controls;
using LibGit2Sharp;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
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

        public static void pVersionarObjetos(ObjectGitModel gitModel, Handler handler, Repositorio repositorioGit)
        {
            string ramaWO = gitModel.RamaLBSeleccionada.Text.ToUpper();
            string MensajeCommit;
            Boolean boUnaRama = false;
            
            var ramasGit = SqliteDAO.pListaRamaRepositorios(repositorioGit);

            //Repos con una sola rama
            if(ramasGit.Count == 1)
            {
                ramaWO = ramasGit.ElementAt(0).Estandar;
                ramaWO = ramaWO.Replace(res.TagHU, gitModel.HU);
                ramaWO = ramaWO.Replace(res.TagUsuario, Environment.UserName.ToUpper());
                pCreaRamaRepo(handler, repositorioGit.Ruta, ramasGit.ElementAt(0).Rama, ramaWO);
                boUnaRama = true;
            }

            MensajeCommit = gitModel.HU + " - " + gitModel.Comentario;

            using (var repo = new Repository(repositorioGit.Ruta))
            {
                try
                {
                    Commands.Checkout(repo, ramaWO);
                }
                catch (Exception ex)
                {
                    pCambiarRama(handler, repositorioGit.Ruta, ramaWO);
                }

                //pCreaDirectorios(repositorioGit.Ruta);

                //Se copian los archivos en cada ruta del repo
                pCopiarObjetosRepo(gitModel.ListaCarpetas.ToList(), repositorioGit.Ruta, repositorioGit);

                Commands.Stage(repo, "*");
                Commit comm = repo.Commit(MensajeCommit, new Signature(Environment.UserName, handler.Azure.Correo, DateTimeOffset.Now), new Signature(Environment.UserName, handler.Azure.Correo, DateTimeOffset.Now));

                if (boUnaRama)
                {
                    Remote remote = repo.Network.Remotes["origin"];
                    Branch ramaFeat = null;

                    var branches = repo.Branches;
                    foreach (Branch ramaFeature in branches)
                    {
                        if (ramaFeature.FriendlyName.Equals(ramaWO))
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
            }
        }

        public static List<SelectListItem> pObtenerCommitsRama(Handler handler,string rama, string rutaRepo, string todos = "N")
        {
            RepositoryStatus status;
            List<SelectListItem> ListaCommits= new List<SelectListItem>();
            List<Commit> ListaCommitsRepo = new List<Commit>();

            using (var repo = new Repository(@rutaRepo))
            {
                status = repo.RetrieveStatus();

                if (status.IsDirty)
                {
                    handler.MensajeError("El repositorio tiene un problema, por favor corregir antes de continuar");
                }
                else
                {
                    pCambiarRama(handler, rutaRepo, rama);

                    if(todos == res.Si)
                        ListaCommitsRepo = repo.Commits.OrderByDescending(x => x.Committer.When.LocalDateTime).ToList();
                    else
                        ListaCommitsRepo = repo.Commits.Where(x => x.Message.ToUpper().StartsWith(rama.ToUpper())).OrderByDescending(x => x.Committer.When.DateTime).ToList();

                    for (int i = 0; i < ListaCommitsRepo.Count(); i++)
                    {
                        Commit actual = ListaCommitsRepo.ElementAt(i);
                        ListaCommits.Add(new SelectListItem { BlValor = false, Text = actual.MessageShort, Fecha = actual.Committer.When.LocalDateTime, Commit_ = actual, Usuario = actual.Committer.Name });
                    }
                }
            }

            return ListaCommits;
        }

        public static List<SelectListItem> pObtenerCommitsRamaSinVal(Handler handler, string rama, string rutaRepo)
        {
            List<SelectListItem> ListaCommits = new List<SelectListItem>();
            List<Commit> ListaCommitsRepo = new List<Commit>();

            using (var repo = new Repository(@rutaRepo))
            {
                ListaCommitsRepo = repo.Commits.Where(x => x.Author.When.LocalDateTime >= DateTime.Now.AddMonths(-1)).OrderByDescending(x => x.Committer.When.LocalDateTime).ToList();

                for (int i = 0; i < ListaCommitsRepo.Count(); i++)
                {
                    if (i >= 3)
                        break;

                    Commit actual = ListaCommitsRepo.ElementAt(i);
                    ListaCommits.Add(new SelectListItem { BlValor = false, Text = actual.MessageShort, Fecha = actual.Committer.When.LocalDateTime, Commit_ = actual, Usuario = actual.Committer.Name });
                }
                
            }

            return ListaCommits;
        }

        public static void pCreaRamaRepo(Handler handler,string rutaRepo,string ramaPrincipal, string ramaCrear)
        {
            bool blRama = false;

            using (var repo = new Repository(@rutaRepo))
            {                
                pCambiarRama(handler, @rutaRepo, ramaPrincipal);                

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
                    pActualizarRepo(handler, rutaRepo, ramaPrincipal);
                    repo.CreateBranch(ramaCrear);
                }

                Commands.Checkout(repo, ramaCrear);
            }
        }

        public static void pCambiarRama(Handler handler, string rutaRepo, string rama)
        {
            string command = "git checkout " + rama;
            string RutagitBash = handler.RepositorioVM.RutaGitBash + "\\" + res.GitBashExe;
            ExecuteGitBashCommand(RutagitBash, command, rutaRepo, true);
        }

        public static void pActualizarRepo(Handler handler, string rutaRepo,string rama)
        {
            string command = "git pull origin " + rama;
            string RutagitBash = handler.RepositorioVM.RutaGitBash + "\\" + res.GitBashExe;
            ExecuteGitBashCommand(RutagitBash, command, rutaRepo, true);
        }

        public static void pCreaLineaBase(ObjectGitModel model, Handler handler, Repositorio repositorioGit)
        {
            bool blRama = false;

            RamaRepositorio ramaPrincipal = SqliteDAO.pListaRamaRepositorios(repositorioGit).Where(x => x.LBase.Equals(res.Si)).FirstOrDefault();

            if (ramaPrincipal == null)
                throw new Exception("El repositorio ["+repositorioGit.Descripcion+"] NO cuenta con una rama línea base registrada [Ajustes/Herramientas Gestión/Git]");

            using (var repo = new Repository(@repositorioGit.Ruta))
            {
                try 
                { 
                    Commands.Checkout(repo, ramaPrincipal.Rama);
                }
                catch (Exception ex)
                {
                    pCambiarRama(handler, repositorioGit.Ruta, ramaPrincipal.Rama);
                }

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
                    pActualizarRepo(handler, repositorioGit.Ruta, ramaPrincipal.Rama);
                    repo.CreateBranch(model.Codigo.ToUpper());
                }
            }
        }

        public static void pRenombrar(Handler handler,string nombreantes, string nuevonombre, Repositorio repositorioGit)
        {
            using (var repo = new Repository(@repositorioGit.Ruta))
            {
                var branches = repo.Branches;

                foreach (Branch b in branches)
                {
                    if (b.FriendlyName.ToUpper().Equals(nombreantes.ToUpper()))
                    {
                        repo.Branches.Rename(b, nuevonombre);
                        break;
                    }
                }
            }
        }
        public static ObservableCollection<SelectListItem> pObtieneRamasListLB(Handler handler,Repositorio repositorio)
        {
            ObservableCollection<SelectListItem> listaRamas = new ObservableCollection<SelectListItem>();

            if (repositorio != null && !string.IsNullOrEmpty(repositorio.Ruta))
            {
                try
                {
                    using (var repo = new Repository(repositorio.Ruta))
                    {
                        var branches = repo.Branches.Where(x => x.FriendlyName.ToUpper().IndexOf(res.Feature.ToUpper()) < 0 && x.FriendlyName.ToUpper().IndexOf("ORIGIN") < 0);

                        foreach (Branch b in branches.OrderByDescending(x => x.Tip.Committer.When.LocalDateTime))
                        {
                            //if (b.FriendlyName.ToUpper().IndexOf(res.Feature.ToUpper()) < 0  && b.FriendlyName.ToUpper().IndexOf("ORIGIN") < 0)
                            listaRamas.Add(new SelectListItem { Text = b.FriendlyName });
                        }
                    }
                }
                catch (Exception ex) 
                {
                    handler.MensajeError(ex.Message);
                }
            }

            return listaRamas;
        }

        internal static ObservableCollection<SelectListItem> pObtRamasOrigen(Repositorio repositorio)
        {
            ObservableCollection<SelectListItem> listaRamas = new ObservableCollection<SelectListItem>();

            if (repositorio != null && !string.IsNullOrEmpty(repositorio.Ruta))
            {
                try
                {
                    using (var repo = new Repository(repositorio.Ruta))
                    {
                        var branches = repo.Branches.Where(x => x.IsRemote && Path.GetFileName(x.FriendlyName.ToUpper()).IndexOf("HU") < 0 && Path.GetFileName(x.FriendlyName.ToUpper()).IndexOf("WO") < 0);

                        foreach (Branch b in branches.OrderByDescending(x => x.Tip.Committer.When.LocalDateTime))
                        {
                            listaRamas.Add(new SelectListItem { Text = Path.GetFileName(b.FriendlyName) });
                        }
                    }
                }
                catch(Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "Error", System.Windows.MessageBoxButton.OKCancel);
                }
            }

            return listaRamas;
        }

        public static void pSetearLineaBase(string rama, Repositorio repositorio)
        {
            using (var repo = new Repository(@repositorio.Ruta))
            {
                Commands.Checkout(repo, rama);
            }
        }

        internal static void pRemoverCambiosGit(Handler handler, string ruta)
        {
            using (var repo = new Repository(@ruta))
            {
                string command = "git stash -u";
                string RutagitBash = handler.RepositorioVM.RutaGitBash + "\\" + res.GitBashExe;
                ExecuteGitBashCommand(RutagitBash, command, ruta, false);
            }
        }
        internal static void pLimpiarRepo(Handler handler, string ruta)
        {
            using (var repo = new Repository(@ruta))
            {
                string command = "git clean -fx";
                string RutagitBash = handler.RepositorioVM.RutaGitBash + "\\" + res.GitBashExe;
                ExecuteGitBashCommand(RutagitBash, command, ruta, false);
            }
        }

        public static string pClonarRepo(string ruta, string url, string rutaGitBash)
        {
            pCreaDirectorios(ruta);

            string command = "Git clone " + url;
            ExecuteGitBashCommand(rutaGitBash+"\\"+res.GitBashExe, command, ruta,true);

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

        public static void ExecuteGitBashCommand(string fileName, string command, string workingDir, bool blMostrar)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, "-c \" " + command + " \"")
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false ,
                CreateNoWindow = blMostrar                
            };

            try
            {
                var process = Process.Start(processStartInfo);
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                var exitCode = process.ExitCode;

                process.Close();
            }
            catch 
            {
                throw new Exception("Verifique que la ruta del Git-Bash.exe ["+ fileName+"] se la correcta [Ajustes/Herramientas Gestión/Git]");
            }
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

        public static void pCopiarObjetosRepo(List<Folder> ListaCarpetas, string path, Repositorio GitSeleccionado)
        {
            string destino = "";
            string pathIn = "";
            string extension = "";

            foreach (Folder archivo in ListaCarpetas)
            {
                extension = System.IO.Path.GetExtension(archivo.FolderLabel);

                if (string.IsNullOrEmpty(extension))
                {
                    if (!archivo.FolderLabel.Equals(GitSeleccionado.Descripcion))
                        pathIn = Path.Combine(path, archivo.FolderLabel);
                    else
                        pathIn = path;

                    if (archivo.Folders.Count > 0)
                    {
                        pCreaDirectorios(pathIn);
                        pCopiarObjetosRepo(archivo.Folders, pathIn, GitSeleccionado);
                    }
                }
                else
                {
                    destino = Path.Combine(path, archivo.FolderLabel);
                    System.IO.File.Copy(archivo.FullPath, destino, true);
                }
            }
        }
    }

    public class RepoGitIns
    {
        private System.Windows.Style estiloVentana;
        public RepoGitIns(System.Windows.Style style) {
            this.estiloVentana = style;
        }

        public bool pCherryPick(Handler handler, string lineaBase, string ramaCrear, string rutaRepo, List<SelectListItem> ListaCommitsLB, List<SelectListItem> ListaCommitsFeaure)
        {
            RepositoryStatus status;
            bool boResultado = true;
            string command;
            string RutagitBash = handler.RepositorioVM.RutaGitBash + "\\" + res.GitBashExe; ;

            using (var repo = new Repository(@rutaRepo))
            {
                if (ListaCommitsLB.Count() > 0)
                {
                    foreach(SelectListItem item in ListaCommitsFeaure)
                    {
                        Commit actual = item.Commit_;

                        if (ListaCommitsLB.Exists(x => x.Commit_.MessageShort.Equals(actual.MessageShort) && x.Commit_.Author.When.LocalDateTime == actual.Author.When.LocalDateTime && x.Commit_.Author.Name.Equals(actual.Author.Name)))
                            ListaCommitsLB.Remove(ListaCommitsLB.Where(x => x.Commit_.MessageShort.Equals(actual.MessageShort) && x.Commit_.Author.When.LocalDateTime == actual.Author.When.LocalDateTime && x.Commit_.Author.Name.Equals(actual.Author.Name)).FirstOrDefault());                        
                    }

                    if (ListaCommitsLB.Count() == 0)
                    {
                        handler.MensajeError("No hay commits en la línea base [" + lineaBase + "] para pasar a la rama [" + ramaCrear + "]");
                        return boResultado = false;
                    }

                    handler.CursorNormal();
                    SelectCommitUserControl winCommit = new SelectCommitUserControl(ListaCommitsLB, ListaCommitsFeaure, lineaBase, ramaCrear);

                    var wnd = new ModernDialog
                    {
                        Title = "Commits Linea Base",
                        Content = winCommit,
                        Width = 900,
                        Height = 600,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ResizeMode = ResizeMode.CanResize
                    };

                    wnd.Buttons = new System.Windows.Controls.Button[] { wnd.OkButton, wnd.CancelButton };
                    wnd.ShowDialog();

                    handler.CursorWait();

                    ObservableCollection<SelectListItem> listaCommSelect = new ObservableCollection<SelectListItem> (winCommit.View.GitModel.ListaCommitsLB.Where(x=>x.BlValor));

                    if (listaCommSelect.Count() == 0 || wnd.DialogResult == false)
                    {
                        return boResultado = false;
                    }

                    foreach (SelectListItem item in listaCommSelect.OrderBy(x => x.Fecha))
                    {
                        if (item.BlValor)
                        {
                            if (string.IsNullOrEmpty(handler.Azure.Correo))
                            {
                                handler.MensajeError("Configure el correo empresarial [Ajustes/Herramientas Gestión/Azure]");
                                return boResultado = false;
                            }

                            repo.CherryPick(item.Commit_, new Signature(Environment.UserName, handler.Azure.Correo, DateTimeOffset.Now));

                            status = repo.RetrieveStatus();

                            if (status.IsDirty)
                            {
                                handler.CursorNormal();

                                command = "TortoiseGitProc.exe /command:diff";
                                RepoGit.ExecuteGitBashCommand(RutagitBash, command, rutaRepo, true);

                                status = repo.RetrieveStatus();

                                if (status.IsDirty)
                                    break;
                            }
                            
                        }
                    }

                    handler.CursorWait();
                    status = repo.RetrieveStatus();

                    if (!status.IsDirty && boResultado)
                    {
                        Remote remote = repo.Network.Remotes["origin"];
                        Branch ramaFeat = null;

                        var branches = repo.Branches;
                        foreach (Branch ramaFeature in branches)
                        {
                            if (ramaFeature.FriendlyName.Equals(ramaCrear))
                            {
                                ramaFeat = ramaFeature;
                                break;
                            }
                        }

                        command = "git push origin " + ramaFeat;
                        RepoGit.ExecuteGitBashCommand(RutagitBash, command, rutaRepo, false);
                    }
                    else
                    {
                        List<SelectListItem> ListaCommitsFeature2 = RepoGit.pObtenerCommitsRamaSinVal(handler, ramaCrear, rutaRepo);
                        repo.Reset(ResetMode.Hard, ListaCommitsFeature2.First().Commit_);
                        boResultado = false;
                        handler.CursorNormal();
                        handler.MensajeOk("El proceso no terminó con éxito. Debe resolver los conflictos que se presentaron.");
                    }
                }
            }

            return boResultado;
        }
    }
}

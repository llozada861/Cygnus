using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.General
{
    public static class RepoGit
    {
        public static void pVersionarDatos(string ruta, string hu, string wo, string mensaje,string email,List<SelectListItem> listaArchivosCargados)
        {
            bool blExisteWO = false;
            bool blRama = false;

            string ramaWO = wo.ToUpper();
            string rutaObjetos = Path.Combine(ruta, res.Despliegues);
            rutaObjetos = Path.Combine(rutaObjetos, ramaWO);
            string rama = res.Feature + hu + "_" + ramaWO + "_" + Environment.UserName.ToUpper();
            

            using (var repo = new Repository(@ruta))
            {
                Commands.Checkout(repo, res.RamaMasterDatos);
                Commands.Pull(repo, new Signature(Environment.UserName, email, DateTimeOffset.Now), new PullOptions());

                var branches = repo.Branches;
                foreach (Branch b in branches)
                {
                    Console.WriteLine(b.FriendlyName);

                    if (b.FriendlyName.Equals(ramaWO))
                    {
                        blExisteWO = true;
                    }

                    if (b.FriendlyName.Equals(rama))
                    {
                        blRama = true;
                    }
                }

                if (!blExisteWO)
                {
                    repo.CreateBranch(ramaWO);
                }

                Commands.Checkout(repo, ramaWO);
                pCreaDirectorios(rutaObjetos);
                SonarQube.pCopiarArchivos(rutaObjetos, listaArchivosCargados);

                Commands.Stage(repo, "*");
                Commit comm = repo.Commit(mensaje, new Signature(Environment.UserName, email, DateTimeOffset.Now),new Signature(Environment.UserName, email, DateTimeOffset.Now));

                //Se cambia a master datos para crear el feature
                Commands.Checkout(repo, res.RamaMasterDatos);
                Commands.Pull(repo, new Signature(Environment.UserName, email, DateTimeOffset.Now), new PullOptions());

                if (!blRama)
                {
                    repo.CreateBranch(rama);
                }
                
                Commands.Checkout(repo, rama);
                repo.CherryPick(comm, new Signature(Environment.UserName, email, DateTimeOffset.Now));

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
        }

        public static string pClonarRepo(string ruta, string url, string carpeta)
        {
            string path = System.IO.Path.Combine(ruta, carpeta);

            pCreaDirectorios(path);

            string clonedRepoPath = Repository.Clone(url, path);

            return path;
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
    }
}

using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CygnusGit
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string WO = "WO123";
            string ruta = "D:\\RepoEPM";
            string url = "https://langeles:fas6zsqe7j756lunlqekjqxmb7h3w5btt4vdh723mprwu2kr36uq@grupoepm.visualstudio.com/OPEN/_git/ActualizacionDatos";

            string rutarepo = pClonarRepo(ruta, url, "ActualizacionDatos");

            using (var repo = new Repository(@rutarepo))
            {

                /*var branches = repo.Branches;
                foreach (var b in branches)
                {
                    Console.WriteLine(b.FriendlyName);
                }

                foreach (var commit in repo.Commits)
                {
                    Console.WriteLine(
                        $"{commit.Id.ToString().Substring(0, 7)} " +
                        $"{commit.Author.When.ToLocalTime()} " +
                        $"{commit.MessageShort} " +
                        $"{commit.Author.Name}");
                }

                var branch = repo.Branches["desarrollo"];*/

                repo.CreateBranch(WO);
                Commands.Checkout(repo, WO);
            }
        }

        public string pClonarRepo(string ruta, string url,string carpeta)
        {
            string path = System.IO.Path.Combine(ruta, carpeta);

            // Determine whether the directory exists.
            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }

            string clonedRepoPath = Repository.Clone(url, path);

            return path;
        }
    }
}

using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Git
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string ruta = "D:\\RepoEPM";
            string url = "https://langeles:fas6zsqe7j756lunlqekjqxmb7h3w5btt4vdh723mprwu2kr36uq@grupoepm.visualstudio.com/OPEN/_git/ActualizacionDatos";

            string repo = pClonarRepo(ruta, url);

            using (var repo = new Repository(@repo))
            {               

                var branches = repo.Branches;
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

                var branch = repo.Branches["desarrollo"];
                Commands.Checkout(repo, "desarrollo");
            }
        }

        public string pClonarRepo(string ruta,string url)
        {
            string clonedRepoPath = Repository.Clone(url, ruta);

            return clonedRepoPath;
        }
    }
}

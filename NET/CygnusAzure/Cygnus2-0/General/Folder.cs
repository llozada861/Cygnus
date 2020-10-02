using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General
{
    public class Folder
    {
        public Folder()
        {
            Folders = new List<Folder>();
        }

        public string FullPath { get; set; }
        public string FolderLabel { get; set; }
        public bool IsNodeExpanded { get; set; }
        public List<Folder> Folders { get; set; }
    }
}

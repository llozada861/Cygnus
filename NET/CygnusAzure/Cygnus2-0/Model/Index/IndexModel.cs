using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Index
{
    public class IndexModel
    {
        private Handler handler;
        private string path;
        private string sbLine;
        private string archivoBD;
        private string file;
        private Char delimiter = ';';
        private String[] substrings;
        public IndexModel(Handler hand)
        {
            handler = hand;
        }
    }
}

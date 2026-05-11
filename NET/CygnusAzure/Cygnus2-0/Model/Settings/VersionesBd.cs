using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Settings
{
    public class VersionesBd
    {
        public VersionBd[] Versiones { get; set; }
    }

    public class VersionBd
    {
        public string Version { get; set; }
        public AjustesBd[] Ajustes { get; set; }
    }

    public class AjustesBd
    {
        public string Sql { get; set; }
    }
}

using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Azure
{
    public class AzureModel: ViewModelBase
    {
        public int Codigo { get; set; }
        public string Url { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public int Dias { get; set; }
        public int Empresa { get; set; }
        public bool Defecto { get; set; }
        public string Token { get; set; }
        public string Proyecto { get; set; }
    }
}

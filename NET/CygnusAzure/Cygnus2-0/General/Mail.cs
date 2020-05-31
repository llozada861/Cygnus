using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General
{
    public class Mail: ViewModelBase
    {
        private string para;

        public string Para
        {
            get { return para; }
            set { SetProperty(ref para, value); }
        }

        private string asunto;

        public string Asunto
        {
            get { return asunto; }
            set { SetProperty(ref asunto, value); }
        }

        private string cuerpo;

        public string Cuerpo
        {
            get { return cuerpo; }
            set { SetProperty(ref cuerpo, value); }
        }        
    }
}

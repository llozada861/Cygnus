using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Data
{
    public class MessageModel: ViewModelBase
    {
        private string descripcion;
        private string causa;
        private string solucion;
        private string codigo;
        private Handler handler;

        public MessageModel(Handler handler)
        {
            this.handler = handler;
        }

        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public string Causa
        {
            get { return causa; }
            set { SetProperty(ref causa, value); }
        }
        public string Solucion
        {
            get { return solucion; }
            set { SetProperty(ref solucion, value); }
        }
    }
}

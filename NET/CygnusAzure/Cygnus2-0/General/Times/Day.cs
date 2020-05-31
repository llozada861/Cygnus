using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General.Times
{
    public class Day
    {
        private string dia;

        public string Dia
        {
            get { return dia; }
            set { dia = value; }
        }

        private double horas;

        public double Horas
        {
            get { return horas; }
            set { horas = value; }
        }
        
        private string comentario;

        public string Comentario
        {
            get { return comentario; }
            set { comentario = value; }
        }

    }
}

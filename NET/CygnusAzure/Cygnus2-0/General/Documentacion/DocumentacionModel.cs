using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General.Documentacion
{
    public class DocumentacionModel
    {
        public string Fuente { get; set; }
        public string Unidad { get; set; }
        public string Autor { get; set; }
        public string Fecha { get; set; }
        public string Descripcion { get; set; }
        public List<ModificacionModel> Modificaciones { get; set; }
        public List<ParametrosModel> Parametros { get; set; }
        public RetornoModel Retorno { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.History
{
    [Table(name: "apl_hist")]
    public class AplicaHistoriaModel
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "caso")]
        public string Caso { get; set; }
        [Column(name: "user")]
        public string Usuario { get; set; }

        [Column(name: "file")]
        public string Archivo { get; set; }
        [Column(name: "path")]
        public string Ruta { get; set; }
        [Column(name: "tipo")]
        public int? Tipo { get; set; }
    }
}

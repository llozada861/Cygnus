using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Objects
{
    [Table(name: "object_type")]
    public class TipoObjetos
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "object")]
        public string Descripcion { get; set; }

        [Column(name: "slash")]
        public string Slash { get; set; }

        [Column(name: "count_slash")]
        public int? CantidadSlash { get; set; }

        [Column(name: "priority")]
        public int Prioridad { get; set; }
    }
}

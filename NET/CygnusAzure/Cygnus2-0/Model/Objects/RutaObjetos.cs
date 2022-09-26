using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Objects
{
    [Table(name: "object_path")]
    public class RutaObjetos
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "object_type")]
        public int? TipoObjeto { get; set; }

        [Column(name: "path")]
        public string Ruta { get; set; }

        [Column(name: "user_default")]
        public string Usuario { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

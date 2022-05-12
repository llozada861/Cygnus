using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Settings
{
    [Table(name: "configuration")]
    public class Configuracion
    {
        [Column(name: "key")]
        [Key]
        public string Key { get; set; }

        [Column(name: "value")]
        public string Valor { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

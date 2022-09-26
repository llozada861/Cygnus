using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Permisos
{
    [Table(name: "grants")]
    public class PermisosModel
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }
        [Column(name: "descripcion")]
        public string Descripcion { get; set; }
    }
}

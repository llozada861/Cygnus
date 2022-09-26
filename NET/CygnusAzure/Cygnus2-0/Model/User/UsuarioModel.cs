using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cygnus2_0.Model.User
{
    [Table(name: "userbd")]
    public class UsuarioModel
    {
        [Column(name: "user")]
        [Key]
        public string Usuariobd { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

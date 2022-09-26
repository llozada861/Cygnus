using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.User
{
    [Table(name: "user_grants")]
    public class GrantsModel
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }
        [Column(name: "user")]
        public string Usuario { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

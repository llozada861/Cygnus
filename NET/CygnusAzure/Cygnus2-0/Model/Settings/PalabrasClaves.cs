using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Settings
{
    [Table(name: "words")]
    public class PalabrasClaves
    {
        [Column(name: "description")]
        [Key]
        public string Palabra { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.History
{
    [Table(name: "history")]
    public class HistoriaModel
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "story")]
        public string Historia { get; set; }
    }
}

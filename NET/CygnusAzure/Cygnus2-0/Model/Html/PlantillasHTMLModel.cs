using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Html
{
    [Table(name: "html")]
    public class PlantillasHTMLModel
    {
        [Column(name: "name")]
        [Key]
        public string Nombre { get; set; }

        [Column(name: "documentation")]
        public string Documentacion { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

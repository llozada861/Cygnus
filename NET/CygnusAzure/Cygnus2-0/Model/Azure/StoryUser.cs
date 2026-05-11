using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Azure
{
    [Table(name: "story_user")]
    public class StoryUser
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "descripcion")]
        public string Descripcion { get; set; }

        [Column(name: "usuario")]
        public string Usuario { get; set; }
        [Column(name: "empresa")]
        public int Empresa { get; set; }
    }
}

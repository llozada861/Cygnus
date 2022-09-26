using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Version
{
    [Table(name:"version")]
    public class VersionBD
    {
        [Column(name: "id")]
        [Key]
        public int? Id { get; set; }

        [Column(name: "version_name")]
        public string Version { get; set; }

        [Column(name: "apply")]
        public string Aplicada { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

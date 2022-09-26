using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Conexion
{
    [Table(name: "conection")]
    public class ConnModel
    {
        [Column(name: "name_")]
        [Key]
        public string Nombre { get; set; }

        [Column(name: "user")]
        public string Usuario { get; set; }

        [Column(name: "pass")]
        public string Pass { get; set; }

        [Column(name: "bd")]
        public string BaseDatos { get; set; }
        [Column(name: "server")]
        public string Servidor { get; set; }
        [Column(name: "port")]
        public string Puerto { get; set; }
        [Column(name: "active")]
        public string Activo { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Plantillas
{
    [Table(name: "deta_plantilla")]
    public class DetallePlantilla
    {
        [Column(name: "codigo")]
        [Key]
        public Int64 Codigo { get; set; }
        [Column(name: "parametro")]
        public string Parametro { get; set; }
        [Column(name: "descripcion")]
        public string Descripcion { get; set; }
        [Column(name: "plantilla")]
        public string Plantilla { get; set; }
        [Column(name: "empresa")]
        public int? Empresa { get; set; }
        [Column(name: "direccion")]
        public string Direccion { get; set; }
        [Column(name: "nombre_archivo")]
        public string NombreArchivo { get; set; }
        [NotMapped]
        public string Valor { get; set; }
        [NotMapped]
        public string Objeto { get; set; }
    }
}

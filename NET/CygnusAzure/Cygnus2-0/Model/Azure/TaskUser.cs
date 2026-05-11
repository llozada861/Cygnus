using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Azure
{
    [Table(name: "task_user")]
    public class TaskUser
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "descripcion")]
        public string Descripcion { get; set; }

        [Column(name: "estado")]
        public string Estado { get; set; }

        [Column(name: "fecha_actualiza")]
        public string FechaActualiza { get; set; }

        [Column(name: "usuario")]
        public string Usuario { get; set; }
        [Column(name: "completado")]
        public double Horas { get; set; }
        [Column(name: "fecha_display")]
        public string FechaDisplay { get; set; }
        [Column(name: "fecha_registro")]
        public string FechaRegistro { get; set; }
        [Column(name: "hist_usuario")]
        public int UserStory { get; set; }
        [Column(name: "fecha_inicio")]
        public string FechaInicio { get; set; }
        [Column(name: "empresa")]
        public int Empresa { get; set; }

        [NotMapped]
        public string DescUs { get; set; }
    }
}

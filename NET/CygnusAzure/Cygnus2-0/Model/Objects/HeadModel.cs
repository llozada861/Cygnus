using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Objects
{
    [Table(name: "object_head")]
    public class HeadModel
    {
        [Column(name: "Codigo")]
        [Key]
        public int? Codigo { get; set; }
        [Column(name: "head")]
        public string Descripcion { get; set; }

        [Column(name: "type")]
        public int? Tipo { get; set; }

        [Column(name: "priority")]
        public int Prioridad { get; set; }

        [Column(name: "end")]
        public string Fin { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }

        [NotMapped]
        public ObservableCollection<SelectListItem> ListaTipoFin { set; get; }
    }
}

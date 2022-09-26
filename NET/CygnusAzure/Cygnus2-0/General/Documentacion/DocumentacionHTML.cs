using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General.Documentacion
{
    [Table(name: "documentation")]
    public class DocumentacionHTML
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }
        [Column(name: "tag_ini")]
        public string TagIni { get; set; }

        [Column(name: "tag_fin")]
        public string TagFin { get; set; }

        [Column(name: "attributes")]
        public string Atributos { get; set; }

        [Column(name: "type")]
        public string Tipo { get; set; }
        [Column(name: "end")]
        public string Fin { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }

        [NotMapped]
        public ObservableCollection<SelectListItem> ListaTipoHTml { set; get; }
    }
}

using Cygnus2_0.Model.Plantillas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        [Column(name: "name", Order = 0)]
        [Key]
        public string Nombre { get; set; }
        [Column(name: "company", Order = 1)]
        [Key]
        public int? Empresa { get; set; }

        [Column(name: "documentation")]
        public string Documentacion { get; set; }
        [Column(name: "filename")]
        public string NombreArchivo { get; set; }

        [Column(name: "tipo")]
        public int? Tipo { get; set; }

        [NotMapped]
        public ObservableCollection<DetallePlantilla> ListaDetalleIn { set; get; }
        [NotMapped]
        public ObservableCollection<DetallePlantilla> ListaDetalleOut { set; get; }
    }
}

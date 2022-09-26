using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Repository
{
    [Table(name: "repository_branch")]
    public class RamaRepositorio
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "repositorio_id")]
        public int? RepositorioId { get; set; }

        [Column(name: "rama")]
        public string Rama { get; set; }

        [Column(name: "estandar")]
        public string Estandar { get; set; }

        [Column(name: "lbase")]
        public string LBase { get; set; }

        [NotMapped]
        public ObservableCollection<SelectListItem> ListaSINO { get; set; }
    }
}

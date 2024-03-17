using Cygnus2_0.General;
using Cygnus2_0.Interface;
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
    public class RamaRepositorio: ViewModelBase
    {
        private string rama;
        private string estandar;

        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "repositorio_id")]
        public int? RepositorioId { get; set; }

        [Column(name: "rama")]
        public string Rama
        {
            get { return rama; }
            set { rama = value; Estandar = Estandar + rama.Substring(0, 3).ToUpper(); }
        }

        [Column(name: "estandar")]
        public string Estandar {
            get { return estandar; }
            set { SetProperty(ref estandar, value); }
        }

        [Column(name: "lbase")]
        public string LBase { get; set; }

        [NotMapped]
        public ObservableCollection<SelectListItem> ListaSINO { get; set; }
        [NotMapped]
        public ObservableCollection<SelectListItem> ListaRamaOrigen { get; set; }
    }
}

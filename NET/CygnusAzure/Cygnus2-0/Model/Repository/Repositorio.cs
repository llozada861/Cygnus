using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
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
    [Table(name:"repositories")]
    public class Repositorio
    {
        [Column(name:"codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name:"descripcion")]
        public string Descripcion { get; set; }

        [Column(name: "documento")]
        public string Documento { get; set; }

        [Column(name: "ruta_local")]
        public string Ruta { get; set; }
        [Column(name: "tipo_dato")]
        public int? TipoDato { get; set; }

        [Column(name: "empresa")]
        public int Empresa { get; set; }
        [NotMapped]
        public ObservableCollection<TipoObjetos> ListaTipos { set; get; }
    }
}

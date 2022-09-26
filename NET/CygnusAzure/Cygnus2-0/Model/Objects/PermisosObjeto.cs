using Cygnus2_0.General;
using Cygnus2_0.Model.Permisos;
using Cygnus2_0.Model.User;
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
    [Table(name: "object_grants")]
    public class PermisosObjeto
    {
        [Column(name: "codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name: "objeto")]
        public int? TipoObjeto { get; set; }

        [Column(name: "usuario_grant")]
        public int? Usuario { get; set; }

        [Column(name: "permiso")]
        public int? Permiso { get; set; }

        [Column(name: "company")]
        public int? Empresa { get; set; }

        [NotMapped]
        public ObservableCollection<SelectListItem> ListaUsuarios { set; get; }

        [NotMapped]
        public ObservableCollection<PermisosModel> ListaPermisos { set; get; }
    }
}

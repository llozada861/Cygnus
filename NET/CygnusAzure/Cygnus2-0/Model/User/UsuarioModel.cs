using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;
using Cygnus2_0.General;

namespace Cygnus2_0.Model.User
{
    [Table(name: "userbd")]
    public class UsuarioModel
    {
        [Column(name: "user")]
        [Key]
        public string Usuariobd { get; set; }
        [Column(name: "password")]
        public string Passwordbd { get; set; }
        [Column(name: "basedatos")]
        public string BaseDatos { get; set; }
        [Column(name: "main")]
        public string Principal { get; set; }
        [Column(name: "company")]
        public int? Empresa { get; set; }
        [NotMapped]
        public ObservableCollection<SelectListItem> ListaSINO { get; set; }
    }
}

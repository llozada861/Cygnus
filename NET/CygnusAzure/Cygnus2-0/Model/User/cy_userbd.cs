using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.User
{
    [Table(name: "cy_userbd")]
    public class UsuariosPDN
    {
        [Column(name: "codigo")]
        [Key]
        public int Codigo { get; set; }
        [Column(name: "user_")]
        public string Usuariobd { get; set; }

        [Column(name: "password_")]
        public string Passwordbd { get; set; }

        [Column(name: "basedatos")]
        public string BaseDatos { get; set; }

        [Column(name: "servidor")]
        public string Servidor { get; set; }

        [Column(name: "puerto")]
        public string Puerto { get; set; }

        [NotMapped]
        public string Displayname
        {
            get { return Usuariobd + "@" + BaseDatos; }
        }
    }
}

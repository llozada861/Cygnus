using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Cygnus2_0.Model.Repository
{
    [Table(name:"repositories")]
    public class Repositorio: ViewModelBase
    {
        private string descripcion;
        private string ruta;

        [Column(name:"codigo")]
        [Key]
        public int? Codigo { get; set; }

        [Column(name:"descripcion")]
        public string Descripcion 
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }

        [Column(name: "documento")]
        public string Documento { get; set; }       

        [Column(name: "ruta_local")]
        public string Ruta
        {
            get { return ruta; }
            set 
            { 
                ruta = value; 
                this.Descripcion = Path.GetFileName(ruta);
            }
        }

        [Column(name: "tipo_dato")]
        public int? TipoDato { get; set; }

        [Column(name: "empresa")]
        public int Empresa { get; set; }
        [Column(name: "rama")]
        public string RamaDefecto { get; set; }
        [Column(name: "sonar")]
        public string Sonar { get; set; }
        [NotMapped]
        public ObservableCollection<TipoObjetos> ListaTipos { set; get; }
        [NotMapped]
        public ObservableCollection<SelectListItem> ListaSINO { get; set; }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Empresa
{
    [Table(name: "company")]
    public class EmpresaModel: ViewModelBase
    {
        private int? codigo;
        private string descripcion;
        private string azure;
        private string git;
        private string sonar;
        private string documentoad;
        private string defecto;

        [Column(name: "codigo")]
        [Key]
        public int? Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }

        [Column(name: "descripcion")]
        public string Descripcion 
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }

        [Column(name: "azure")]
        public string Azure 
        {
            get { return azure; }
            set { SetProperty(ref azure, value); }
        }

        [Column(name: "git")]
        public string Git 
        {
            get { return git; }
            set { SetProperty(ref git, value); }
        }

        [Column(name: "sonar")]
        public string Sonar 
        {
            get { return sonar; }
            set { SetProperty(ref sonar, value); }
        }

        [Column(name: "documentoad")]
        public string DocumentoAd 
        {
            get { return documentoad; }
            set { SetProperty(ref documentoad, value); }
        }

        [Column(name: "defecto")]
        public string Defecto 
        {
            get { return defecto; }
            set { SetProperty(ref defecto, value); }
        }
    }
}

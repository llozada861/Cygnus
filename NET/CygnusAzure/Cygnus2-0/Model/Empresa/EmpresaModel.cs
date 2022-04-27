using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Empresa
{
    public class EmpresaModel: ViewModelBase
    {
        private string codigo;
        private string descripcion;
        private string azure;
        private string git;
        private string sonar;
        private string documentoad;
        private string defecto;

        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }
        public string Descripcion 
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public string Azure 
        {
            get { return azure; }
            set { SetProperty(ref azure, value); }
        }
        public string Git 
        {
            get { return git; }
            set { SetProperty(ref git, value); }
        }
        public string Sonar 
        {
            get { return sonar; }
            set { SetProperty(ref sonar, value); }
        }
        public string DocumentoAd 
        {
            get { return documentoad; }
            set { SetProperty(ref documentoad, value); }
        }
        public string Defecto 
        {
            get { return defecto; }
            set { SetProperty(ref defecto, value); }
        }
    }
}

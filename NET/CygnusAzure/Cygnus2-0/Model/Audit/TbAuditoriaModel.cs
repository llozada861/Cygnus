using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Audit
{
    public class TbAuditoriaModel: ViewModelBase
    {
        private string tabla;
        private string primaria;
        private string ticket;
        private string autor;
        private string login;
        public TbAuditoriaModel()
        {
        }
        public string Tabla
        {
            get { return tabla; }
            set
            {
                SetProperty(ref tabla, value.Trim());
            }
        }
        public string Primaria
        {
            get { return primaria; }
            set
            {
                SetProperty(ref primaria, value.Trim());
            }
        }
        public string Ticket
        {
            get { return ticket; }
            set
            {
                SetProperty(ref ticket, value.Trim());
            }
        }
        public string Autor
        {
            get { return autor; }
            set
            {
                SetProperty(ref autor, value.Trim());
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                SetProperty(ref login, value.Trim());
            }
        }
    }
}

using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General.Times
{
    public class TareaHoja: ViewModelBase
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private int idAzure;

        public int IdAzure
        {
            get { return idAzure; }
            set { idAzure = value; }
        }
        private int hu;
        public int HU
        {
            get { return hu; }
            set { hu = value; }
        }
        private int idHoja;
        public int IdHoja
        {
            get { return idHoja; }
            set { idHoja = value; }
        }
        private string requerimiento;
        public string Requerimiento
        {
            get { return requerimiento; }
            set { requerimiento = value; }
        }

        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        public string DescripcionPlus
        {
            get { return "[Azure: "+IdAzure+"] - "+Descripcion; }
        }

        private string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }
        private string fechaCreacion;

        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; }
        }
        private string iniFecha;

        public string IniFecha
        {
            get { return iniFecha; }
            set { iniFecha = value; }
        }
        private string tipo;
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private Day sun;

        public Day Sun
        {
            get { return sun; }
            set { sun = value; }
        }

        private Day mon;

        public Day Mon
        {
            get { return mon; }
            set { mon = value; }
        }

        private Day tue;

        public Day Tue
        {
            get { return tue; }
            set { tue = value; }
        }

        private Day wed;

        public Day Wed
        {
            get { return wed; }
            set { wed = value; }
        }

        private Day thu;

        public Day Thu
        {
            get { return thu; }
            set { thu = value; }
        }

        private Day fri;

        public Day Fri
        {
            get { return fri; }
            set { fri = value; }
        }

        private Day sat;

        public Day Sat
        {
            get { return sat; }
            set { sat = value; }
        }

        private double total;

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        private DateTime fechaDisplay;

        public DateTime FechaDisplay
        {
            get { return fechaDisplay; }
            set { fechaDisplay = value; }
        }
        private double completed;

        public double Completed
        {
            get { return completed; }
            set { completed = value; }
        }
        
        public void pCalcularTotal()
        {
            try
            {
                Total = Mon.Horas + Tue.Horas + Wed.Horas + Thu.Horas + Fri.Horas + Sat.Horas + Sun.Horas;
            }
            catch { }
        }
    }
}

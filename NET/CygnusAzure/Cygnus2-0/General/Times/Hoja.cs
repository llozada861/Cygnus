using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General.Times
{
    public class Hoja: ViewModelBase
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime fechaIni;

        public DateTime FechaIni
        {
            get { return fechaIni; }
            set { fechaIni = value; }
        }
        private DateTime fechaFin;

        public DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }
        
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private double horas;

        public double Horas
        {
            get { return horas; }
            set { horas = value; }
        }


        private ObservableCollection<TareaHoja> listatareas;
        public ObservableCollection<TareaHoja> ListaTareas
        {
            get { return listatareas; }
            set { SetProperty(ref listatareas, value); }
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cygnus2_0.General.Times;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Cygnus2_0.Pages.Time;
using System.Threading;
using System.IO;
using System.Globalization;
using Cygnus2_0.Interface;
using System.Collections.ObjectModel;

namespace Cygnus2_0.Model.Time
{
    public class TimeModel: ViewModelBase
    {
        private ObservableCollection<TareaHoja> listatareas;
        private ObservableCollection<Hoja> listaHojas;
        private List<TareaHoja> listaTareasEliminar;
        private Hoja hoja;
        private Hoja hojaAux;
        private double totalMon;
        private double totalTue;
        private double totalWed;
        private double totalThu;
        private double totalFri;
        private double totalSat;
        private double totalSun;
        private double total;
        private string descWindow;
        private int idAzureWindow;
        public TimeModel()
        {
        }

        public ObservableCollection<TareaHoja> ListaTareas
        {
            get { return listatareas; }
            set { SetProperty(ref listatareas, value); }
        }

        public ObservableCollection<Hoja> ListaHojas
        {
            get { return listaHojas; }
            set { SetProperty(ref listaHojas, value); }
        }

        public Hoja HojaActual
        {
            get { return hoja; }
            set { SetProperty(ref hoja, value); }
        }
        public Hoja HojaActualAux
        {
            get { return hojaAux; }
            set { SetProperty(ref hojaAux, value); }
        }

        public double TotalMon
        {
            get { return totalMon; }
            set { SetProperty(ref totalMon, value); }
        }

        public double TotalTue
        {
            get { return totalTue; }
            set { SetProperty(ref totalTue, value); }
        }
        public double TotalWed
        {
            get { return totalWed; }
            set { SetProperty(ref totalWed, value); }
        }
        public double TotalThu
        {
            get { return totalThu; }
            set { SetProperty(ref totalThu, value); }
        }
        public double TotalFri
        {
            get { return totalFri; }
            set { SetProperty(ref totalFri, value); }
        }
        public double TotalSat
        {
            get { return totalSat; }
            set { SetProperty(ref totalSat, value); }
        }
        public double TotalSun
        {
            get { return totalSun; }
            set { SetProperty(ref totalSun, value); }
        }
        public double Total
        {
            get { return total; }
            set { SetProperty(ref total, value); }
        }

        public string DescWindow
        {
            get { return descWindow; }
            set { SetProperty(ref descWindow, value); }
        }
        public int IdAzureWindow
        {
            get { return idAzureWindow; }
            set { SetProperty(ref idAzureWindow, value); }
        }

        public List<TareaHoja> ListaTareasEliminar
        {
            get { return listaTareasEliminar; }
            set { listaTareasEliminar = value; }
        }

        private double totalRequerimiento;
        public double TotalRequerimiento
        {
            get { return totalRequerimiento; }
            set { totalRequerimiento = value; }
        }
        private double totalRequerimientoazure;
        public double TotalRequerimientoAzure
        {
            get { return totalRequerimientoazure; }
            set { totalRequerimientoazure = value; }
        }

        private ObservableCollection<Hoja> listaDetalleTarea;
        public ObservableCollection<Hoja> ListaDetalleTarea
        {
            get { return listaDetalleTarea; }
            set { SetProperty(ref listaDetalleTarea, value); }
        }
        private ObservableCollection<SelectListItem> listaTareasPred;
        public ObservableCollection<SelectListItem> ListaTareasPred
        {
            get { return listaTareasPred; }
            set { SetProperty(ref listaTareasPred, value); }
        }
        private SelectListItem tareaPred;
        public SelectListItem TareaPred
        {
            get { return tareaPred; }
            set { SetProperty(ref tareaPred, value); }
        }

        private Mail mailCreaRq;
        public Mail MailCreaRq
        {
            get { return mailCreaRq; }
            set { SetProperty(ref mailCreaRq, value); }
        }

        private TareaHoja tareaOrigen;
        public TareaHoja TareaOrigen
        {
            get { return tareaOrigen; }
            set { SetProperty(ref tareaOrigen, value); }
        }
        private TareaHoja tareaDestino;
        public TareaHoja TareaDestino
        {
            get { return tareaDestino; }
            set { SetProperty(ref tareaDestino, value); }
        }
        private int hu;
        public int HU
        {
            get { return hu; }
            set { SetProperty(ref hu, value); }
        }
        private double totalHU;

        public double TotalHU
        {
            get { return totalHU; }
            set { SetProperty(ref totalHU, value); }
        }
    }
}

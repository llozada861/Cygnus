using Cygnus2_0.General;
using Cygnus2_0.General.Times;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Time;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Time
{
    public class TimesViewModel : ViewModelBase, IViews
    {
        private TimeModel timeModel;
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
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conectar;
        private readonly DelegateCommand _refrescar;
        private Handler handler;
        private int idAzureWindow;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conectar;
        public ICommand Refrescar => _refrescar;

        public TimesViewModel(Handler handler)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conectar = new DelegateCommand(OnConection);
            _refrescar = new DelegateCommand(OnRefresh);

            this.handler = handler;
            ListaTareas = new ObservableCollection<TareaHoja>();
            ListaHojas = new ObservableCollection<Hoja>();
            ListaTareasPred = new ObservableCollection<SelectListItem>();

            timeModel = new TimeModel(handler,this);
            ListaTareasEliminar = new List<TareaHoja>();
            this.MailCreaRq = new Mail();
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
            get { return hoja;  }
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
            set { SetProperty(ref mailCreaRq, value);}
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

        internal void pCalcularTotales()
        {            
            if(this.ListaTareas.Count > 0)
            {
                this.TotalMon = 0;
                this.TotalTue = 0;
                this.TotalWed = 0;
                this.TotalThu = 0;
                this.TotalFri = 0;
                this.TotalSat = 0;
                this.TotalSun = 0;
                this.Total = 0;

                if (this.HojaActual != null)
                {
                    foreach (TareaHoja tarea in this.HojaActual.ListaTareas)
                    {
                        this.TotalMon = this.TotalMon + tarea.Mon.Horas;
                        this.TotalTue = this.TotalTue + tarea.Tue.Horas;
                        this.TotalWed = this.TotalWed + tarea.Wed.Horas;
                        this.TotalThu = this.TotalThu + tarea.Thu.Horas;
                        this.TotalFri = this.TotalFri + tarea.Fri.Horas;
                        this.TotalSat = this.TotalSat + tarea.Sat.Horas;
                        this.TotalSun = this.TotalSun + tarea.Sun.Horas;
                        this.Total = this.Total + tarea.Total;
                    }
                }
            }
        }

        internal void pObtDetalleRq(TareaHoja tarea)
        {
            handler.DAO.pObtDetalleRq(this,tarea);
        }

        public void OnProcess(object commandParameter)
        {
            
        }
        internal void pObtTareasPorHoja()
        {
            try
            {
                timeModel.pObtTareasPorHoja();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnAdd(object commandParameter, string tipo)
        {
            int IdAzure;

            if (tipo == "N")
            {
                if (!string.IsNullOrEmpty(this.DescWindow))
                {
                    if (commandParameter == null)
                    {
                        if (this.IdAzureWindow == 0)
                            IdAzure = 0;
                        else
                            IdAzure = this.IdAzureWindow;

                        pAdicionaTarea(this.DescWindow,0, IdAzure);
                    }
                    else
                    {
                        TareaHoja tarea = (TareaHoja)commandParameter;
                        tarea.Descripcion = DescWindow;
                        tarea.IdAzure = IdAzureWindow;
                        handler.DAO.pActualizaTareaAzure(tarea);
                    }
                }
                else
                {
                    handler.MensajeError("Debe ingresar una descripción");
                }
            }
            else
            {
                if(TareaPred != null)
                {
                    pAdicionaTarea(this.TareaPred.Text, Convert.ToInt32(this.TareaPred.Value), 0);
                }
                else
                {
                    handler.MensajeError("Debe ingresar una tarea");
                }
            }
        }
        public void pAdicionaTarea(string descripcion, int requerimiento, int IdAzure)
        {
            TareaHoja tarea = new TareaHoja();
            tarea.Descripcion = descripcion;
            tarea.Id = "0";
            tarea.Estado = "NA";
            tarea.IdHoja = this.HojaActual.Id;
            tarea.Requerimiento = requerimiento.ToString();
            tarea.Mon = new Day();
            tarea.Tue = new Day();
            tarea.Wed = new Day();
            tarea.Thu = new Day();
            tarea.Fri = new Day();
            tarea.Sat = new Day();
            tarea.Sun = new Day();
            tarea.Tipo = "N";
            tarea.IdAzure = IdAzure;

            tarea.pCalcularTotal();
            this.HojaActual.ListaTareas.Add(tarea);
            this.ListaTareas.Add(tarea);
        }
        public void OnClean(object commandParameter)
        {
            throw new NotImplementedException();
        }

        public void OnConection(object commandParameter)
        {
            throw new NotImplementedException();
        }

        public void OnRefresh(object commandParameter)
        {
            timeModel.ConsultarAzure((SynchronizationContext)commandParameter,"1");
        }

        public void ObtTareasAzure(SynchronizationContext context,string sbDiasAtras)
        {
            timeModel.ConsultarAzure(context, sbDiasAtras);
        }

        public void pRefrescaTareas()
        {
            timeModel.pRefrescaTareas();
        }

        public void pLimpiarTareas()
        {
            ListaTareas.Clear();
            ListaTareasEliminar.Clear();

            if(HojaActual != null)
                HojaActual.ListaTareas.ToList().RemoveAll(x => x.Tipo.Equals("L"));
        }
    }
}

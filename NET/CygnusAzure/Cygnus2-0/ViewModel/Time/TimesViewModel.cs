using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.General.Times;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Time;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Time
{
    public class TimesViewModel : IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conectar;
        private readonly DelegateCommand _refrescar;
        private List<TareaHoja> listaTareaAzure;
        private Handler handler;
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

            this.Model = new TimeModel();

            this.handler = handler;
            this.Model.ListaTareas = new ObservableCollection<TareaHoja>();
            this.Model.ListaHojas = new ObservableCollection<Hoja>();
            this.Model.ListaTareasPred = new ObservableCollection<SelectListItem>();

            pRefrescaTareas();
            pCargarTareasPred();
            this.Model.ListaTareasEliminar = new List<TareaHoja>();
            this.Model.MailCreaRq = new Mail();
        }       
        
        public TimeModel Model { set; get; }

        internal void pCalcularTotales()
        {      
            this.Model.TotalMon = 0;
            this.Model.TotalTue = 0;
            this.Model.TotalWed = 0;
            this.Model.TotalThu = 0;
            this.Model.TotalFri = 0;
            this.Model.TotalSat = 0;
            this.Model.TotalSun = 0;
            this.Model.Total = 0;

            if (this.Model.HojaActual != null)
            {
                foreach (TareaHoja tarea in this.Model.HojaActual.ListaTareas)
                {
                    this.Model.TotalMon = this.Model.TotalMon + tarea.Mon.Horas;
                    this.Model.TotalTue = this.Model.TotalTue + tarea.Tue.Horas;
                    this.Model.TotalWed = this.Model.TotalWed + tarea.Wed.Horas;
                    this.Model.TotalThu = this.Model.TotalThu + tarea.Thu.Horas;
                    this.Model.TotalFri = this.Model.TotalFri + tarea.Fri.Horas;
                    this.Model.TotalSat = this.Model.TotalSat + tarea.Sat.Horas;
                    this.Model.TotalSun = this.Model.TotalSun + tarea.Sun.Horas;
                    this.Model.Total = this.Model.Total + tarea.Total;
                }
            }
            
        }

        internal void pObtDetalleRq(TareaHoja tarea)
        {
            SqliteDAO.pObtDetalleRq(this.Model, tarea,handler);
        }

        public void OnProcess(object commandParameter)
        {
            
        }

        public void OnAdd(object commandParameter, string tipo)
        {
            int IdAzure;

            if (tipo == "N")
            {
                if (!string.IsNullOrEmpty(this.Model.DescWindow))
                {
                    if (commandParameter == null)
                    {
                        if (this.Model.IdAzureWindow == 0)
                            IdAzure = 0;
                        else
                            IdAzure = this.Model.IdAzureWindow;

                        pAdicionaTarea(this.Model.DescWindow,0, IdAzure);
                    }
                    else
                    {
                        TareaHoja tarea = (TareaHoja)commandParameter;
                        tarea.Descripcion = this.Model.DescWindow;
                        tarea.IdAzure = this.Model.IdAzureWindow;
                        SqliteDAO.pInsertaTareaAzure(tarea, handler,"I");
                        //handler.DAO.pActualizaTareaAzure(tarea);
                    }
                }
                else
                {
                    handler.MensajeError("Debe ingresar una descripción");
                }
            }
            else
            {
                if(this.Model.TareaPred != null)
                {
                    pAdicionaTarea(this.Model.TareaPred.Text, Convert.ToInt32(this.Model.TareaPred.Value), 0);
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
            tarea.IdHoja = this.Model.HojaActual.Id;
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
            this.Model.HojaActual.ListaTareas.Add(tarea);
            this.Model.ListaTareas.Add(tarea);
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
            ConsultarAzure((SynchronizationContext)commandParameter);
        }

        public void ObtTareasAzure(SynchronizationContext context)
        {
            ConsultarAzure(context);
        }

        public void pLimpiarTareas()
        {
            this.Model.ListaTareas.Clear();
            this.Model.ListaTareasEliminar.Clear();

            if(this.Model.HojaActual != null)
                this.Model.HojaActual.ListaTareas.ToList().RemoveAll(x => x.Tipo.Equals("L"));
        }

        public void pRefrescaTareas()
        {
            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {
                SqliteDAO.pObtHojasBD(this.Model, handler);
            }
        }

        public void pCargarTareasPred()
        {
            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {
                SqliteDAO.pCargarTareasPred(this.Model);
            }
        }

        public void ConsultarAzure(SynchronizationContext uiContext)
        {
            if (string.IsNullOrEmpty(handler.Azure.Usuario))
            {
                return;
            }

            pDescargarTareasAzure(uiContext);
        }

        public IList<WorkItem> pObtItemsAzure()
        {
            List<WorkItem> tareasAzure = new List<WorkItem>();

            var connection = new VssConnection(new Uri(handler.Azure.Url), new VssBasicCredential(string.Empty, handler.Azure.Token));
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var wiql = new Wiql()
            {

                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id] " +
                        "From WorkItems " +
                        "Where [System.WorkItemType] = 'Task' " +
                        "And [System.State] not in ('Removed') " +
                        "And [System.AssignedTo] = '" + handler.Azure.Usuario + "'" +
                        "And [System.CreatedDate] > @today-" + handler.Azure.Dias
            };

            //Proyecto = "OPEN"
            var worItemsIds = workItemTrackingHttpClient.QueryByWiqlAsync(wiql, handler.Azure.Proyecto).Result;

            var ids = worItemsIds.WorkItems.Select(item => item.Id).ToArray();

            // build a list of the fields we want to see
            var fields = new[] { "System.Id", "System.Title", "System.State", "System.AssignedTo", "Microsoft.VSTS.Scheduling.CompletedWork", "System.CreatedDate", "Microsoft.VSTS.Scheduling.StartDate" };

            // get work items for the ids found in query
            tareasAzure = workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields).Result;

            return tareasAzure;
        }

        public void pDescargarTareasAzure(SynchronizationContext uiContext)
        {
            string log = "Antes de traer los items - ";
            CultureInfo culture = new CultureInfo("es-CO");
            //pGeneraLog(log);

            var workItems = pObtItemsAzure();
            listaTareaAzure = new List<TareaHoja>();

            /*log = log +"Query Results: " + workItems.Count+" - ";
            Console.WriteLine("Query Results: {0} items found", workItems.Count);
            pGeneraLog(log);*/

            // loop though work items and write to console
            foreach (var workItem in workItems)
            {
                try
                {
                    TareaHoja tarea = new TareaHoja();
                    tarea.IdAzure = Convert.ToInt32(workItem.Id.ToString());
                    tarea.Descripcion = workItem.Fields["System.Title"].ToString();
                    tarea.Estado = workItem.Fields["System.State"].ToString();
                    tarea.FechaCreacion = Convert.ToDateTime(workItem.Fields["System.CreatedDate"]).ToShortDateString().ToString(culture);
                    tarea.Sun = new Day();
                    tarea.Mon = new Day();
                    tarea.Tue = new Day();
                    tarea.Wed = new Day();
                    tarea.Thu = new Day();
                    tarea.Fri = new Day();
                    tarea.Sat = new Day();

                    WorkItem historiaUsuario = pObtenerHU(tarea.IdAzure);
                    tarea.HU = (int)historiaUsuario.Id;
                    tarea.DescripcionHU = historiaUsuario.Fields.Last().Value.ToString();

                    /*if (tarea.IdAzure == 272078)
                    {
                        int pru = 1;
                    }*/

                    try
                    {
                        tarea.Completed = Math.Round(double.Parse(workItem.Fields["Microsoft.VSTS.Scheduling.CompletedWork"].ToString()), 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0}\t NO tiene horas en Azure: " + ex.Message, workItem.Id);
                        tarea.Completed = 0;
                    }

                    try
                    {
                        tarea.IniFecha = Convert.ToDateTime(workItem.Fields["Microsoft.VSTS.Scheduling.StartDate"]).ToShortDateString().ToString(culture);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0}\t NO tiene fecha en Azure: " + ex.Message, workItem.Id);
                    }

                    this.listaTareaAzure.Add(tarea);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}\t Error: " + ex.Message, workItem.Id);
                }
            }

            if (this.listaTareaAzure.Count > 0)
            {
                TareaHoja hojaActual = null;

                foreach (TareaHoja tareaAzure in this.listaTareaAzure)
                {
                    /*if(tareaAzure.IdAzure == 272078)
                    {
                        int pru = 1;
                    }*/

                    try
                    {
                        if (!this.Model.HojaActual.ListaTareas.ToList().Exists(x => x.IdAzure.Equals(tareaAzure.IdAzure)))
                        {
                            SqliteDAO.pInsertaTareaAzure(tareaAzure, handler,"I");
                            //handler.DAO.pInsertaTareaAzure(tareaAzure);
                        }
                        else
                        {
                            hojaActual = this.Model.HojaActual.ListaTareas.ToList().Find(x => x.IdAzure.Equals(tareaAzure.IdAzure));

                            if (hojaActual != null)
                            {
                                hojaActual.Completed = tareaAzure.Completed;
                                hojaActual.Descripcion = tareaAzure.Descripcion;
                                hojaActual.HU = tareaAzure.HU;
                                hojaActual.IniFecha = tareaAzure.IniFecha;
                                hojaActual.DescripcionHU = tareaAzure.DescripcionHU;
                                //handler.DAO.pActualizaTareaAzure(hojaActual);
                                SqliteDAO.pInsertaTareaAzure(hojaActual, handler,"A");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        handler.MensajeError(ex.Message);
                    }
                }
            }

            uiContext.Send(x => pObtTareasPorHoja(), null);
            uiContext.Send(x => pCalcularTotales(), null);
        }

        public WorkItem pObtenerHU(int idTask)
        {
            WorkItem historiaUsuario = new WorkItem();
            var connection = new VssConnection(new Uri(handler.Azure.Url), new VssBasicCredential(string.Empty, handler.Azure.Token));
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var wiql = new Wiql()
            {

                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = String.Format("SELECT [System.Id],[System.Title] FROM WorkItemLinks WHERE ([Source].[System.WorkItemType] = 'User Story') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Target].[System.Id] = {0}  AND  [Target].[System.WorkItemType] = 'Task') ORDER BY [System.Id] mode(Recursive,ReturnMatchingChildren)", idTask.ToString()),
            };

            var worItemsIds = workItemTrackingHttpClient.QueryByWiqlAsync(wiql, handler.Azure.Proyecto).Result;

            var ids = worItemsIds.WorkItemRelations.Select(item => item.Target.Id).ToArray();

            // build a list of the fields we want to see
            var fields = new[] { "System.Id", "System.Title" };

            // get work items for the ids found in query
            List<WorkItem> hus = workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields).Result;

            historiaUsuario = hus.First();

            return historiaUsuario;
        }

        public void pGeneraLog(string mensaje)
        {
            string sbLinea = "";
            string path = Environment.CurrentDirectory;
            string archivoLog = System.IO.Path.Combine(path, DateTime.Now.ToString("ddMMyyyy_HHMMss") + "_result.txt");

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(archivoLog))
            {
                sbLinea = "Inicia";
                sw.WriteLine(mensaje);
            }
        }

        internal void pObtTareasPorHoja()
        {
            try
            {
                SqliteDAO.pObtTareasBD(this.Model, handler);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }                        
}
}

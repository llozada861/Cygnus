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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Time
{
    public class TimeModel
    {
        private Handler handler;
        private TimesViewModel view;
        private List<TareaHoja> listaTareaAzure;
        public TimeModel(Handler handler, TimesViewModel view)
        {
            this.handler = handler;
            this.view = view;
            pRefrescaTareas();
            pCargarTareasPred();
        }

        public void pRefrescaTareas()
        {
            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {
                handler.DAO.pObtHojasBD(view);
                view.pCalcularTotales();
            }
        }

        public void pCargarTareasPred()
        {
            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {
                handler.DAO.pCargarTareasPred(view);
            }
        }

        public void ConsultarAzure(SynchronizationContext uiContext, string sbDiasAtras)
        {
            if (string.IsNullOrEmpty(handler.ConnViewModel.UsuarioAzure))
            {
                return;
            }

            PrintOpenBugsAsync(uiContext, sbDiasAtras);
        }

        public async Task PrintOpenBugsAsync(SynchronizationContext uiContext, string sbDiasAtras)
        {
            string log = "Antes de traer los items - ";
            CultureInfo culture = new CultureInfo("es-CO");
            //pGeneraLog(log);

            var workItems = await this.QueryOpenBugs(sbDiasAtras).ConfigureAwait(false);
            this.listaTareaAzure = new List<TareaHoja>();
            
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
                    tarea.HU = await pObtenerHU(tarea.IdAzure);

                    /*if (tarea.IdAzure == 117899)
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
                catch(Exception ex)
                {
                    Console.WriteLine("{0}\t Error: "+ ex.Message,workItem.Id);
                }
            }

            if (this.listaTareaAzure.Count > 0)
            {
                TareaHoja hojaActual = null;

                foreach (TareaHoja tareaAzure in this.listaTareaAzure)
                {
                    /*if(tareaAzure.IdAzure == 117899)
                    {
                        int pru = 1;
                    }*/

                    try
                    {
                        if (!view.HojaActual.ListaTareas.ToList().Exists(x => x.IdAzure.Equals(tareaAzure.IdAzure)))
                        {
                            handler.DAO.pInsertaTareaAzure(tareaAzure);
                        }
                        else
                        {
                            hojaActual = view.HojaActual.ListaTareas.ToList().Find(x => x.IdAzure.Equals(tareaAzure.IdAzure));

                            if (hojaActual != null)
                            {
                                if (hojaActual.Completed != tareaAzure.Completed || !hojaActual.Descripcion.Equals(tareaAzure.Descripcion) || hojaActual.HU != tareaAzure.HU)
                                {
                                    hojaActual.Completed = tareaAzure.Completed;
                                    hojaActual.Descripcion = tareaAzure.Descripcion;
                                    hojaActual.HU = tareaAzure.HU;
                                    handler.DAO.pActualizaTareaAzure(hojaActual);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        handler.MensajeError(ex.Message);
                    }
                }
            }

            uiContext.Send(x => view.pObtTareasPorHoja(), null);
            uiContext.Send(x => view.pCalcularTotales(), null);
        }

        public async Task<IList<WorkItem>> QueryOpenBugs(string sbDiasAtras)
        {
            string areas = "";
            string personalAccessToken = res.TokenAzureConn; //"trrveg7rc4kp7fng4gxkp6r527ahwj2qncfvtx7gcoe3ljwpz7tq";

            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);
            VssConnection connection = null;
            connection = new VssConnection(new Uri(handler.ConnViewModel.UrlAzure), credentials);

            // create a wiql object and build our query
            var wiql = new Wiql()
            {               

                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
               Query = "Select [Id] "+
                        "From WorkItems " +
                        "Where [System.WorkItemType] = 'Task' " +
                        "And [System.State] not in ('Removed') " +
                        "And [System.AssignedTo] = '" + handler.ConnViewModel.UsuarioAzure+"'"+
                        "And [System.CreatedDate] > @today-"+ sbDiasAtras
            };
            //"And [System.TeamProject] = '" + project + "' " +

            // create instance of work item tracking http client
            using (var httpClient = new WorkItemTrackingHttpClient(new Uri(handler.ConnViewModel.UrlAzure), credentials))
            {
                // execute the query to get the list of work items in the results
                var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
                var ids = result.WorkItems.Select(item => item.Id).ToArray();

                // build a list of the fields we want to see
                var fields = new[] { "System.Id", "System.Title", "System.State", "System.AssignedTo", "Microsoft.VSTS.Scheduling.CompletedWork", "System.CreatedDate", "Microsoft.VSTS.Scheduling.StartDate" };

                // get work items for the ids found in query
                return await httpClient.GetWorkItemsAsync(ids, fields, result.AsOf).ConfigureAwait(false);
           }
        }

        public async Task<int> pObtenerHU(int idTask)
        {
            int HU = 0;
            string personalAccessToken = res.TokenAzureConn; //"trrveg7rc4kp7fng4gxkp6r527ahwj2qncfvtx7gcoe3ljwpz7tq";

            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);
            VssConnection connection = null;
            connection = new VssConnection(new Uri(handler.ConnViewModel.UrlAzure), credentials);

            // create a wiql object and build our query
            var wiql = new Wiql()
            {

                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = String.Format("SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.WorkItemType] = 'User Story') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Target].[System.Id] = {0}  AND  [Target].[System.WorkItemType] = 'Task') ORDER BY [System.Id] mode(Recursive,ReturnMatchingChildren)", idTask.ToString()),
            };

            using (var httpClient = new WorkItemTrackingHttpClient(new Uri(handler.ConnViewModel.UrlAzure), credentials))
            {
                // execute the query to get the list of work items in the results
                var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
                var ids = result.WorkItemRelations.Select(item => item.Target.Id).ToArray();

                for(int i=0;i<ids.Length;i++)
                {
                    if(ids[i] == idTask)
                    {
                        HU = ids[i - 1];
                    }
                }
            }

            return HU;
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
            handler.DAO.pObtTareasBD(view);

            foreach (TareaHoja tareahoja in view.HojaActual.ListaTareas)
            {
                if (!view.ListaTareas.ToList().Exists(x => x.IdAzure.Equals(tareahoja.IdAzure)))
                {
                    view.ListaTareas.Add(tareahoja);
                }
            }    
            
            if(view.ListaTareas.Count > 0)
            {
                foreach(TareaHoja nuevaTarea in view.ListaTareas)
                {
                    if (!view.HojaActual.ListaTareas.ToList().Exists(x => x.IdAzure.Equals(nuevaTarea.IdAzure)))
                    {
                        nuevaTarea.Mon.Horas = 0;
                        nuevaTarea.Tue.Horas = 0;
                        nuevaTarea.Wed.Horas = 0;
                        nuevaTarea.Thu.Horas = 0;
                        nuevaTarea.Fri.Horas = 0;
                        nuevaTarea.Sat.Horas = 0;
                        nuevaTarea.Sun.Horas = 0;
                        nuevaTarea.pCalcularTotal();
                        nuevaTarea.Id = "0";
                        nuevaTarea.IdHoja = view.HojaActual.Id;
                        nuevaTarea.Tipo = "L";
                        view.ListaHojas.ToList().Find(x => x.Id == view.HojaActual.Id).ListaTareas.Add(nuevaTarea);
                    }
                }
            }        
        }
    }
}

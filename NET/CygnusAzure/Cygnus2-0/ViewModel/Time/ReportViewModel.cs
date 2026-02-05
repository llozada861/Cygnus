using Cygnus2_0.General;
using Cygnus2_0.General.Times;
using Cygnus2_0.Interface;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;
using System.Text.Json;

namespace Cygnus2_0.ViewModel.Time
{
    public class ReportViewModel : ViewModelBase, IViews
    {
        private DateTime fechaDesde;
        private DateTime fechaHasta;
        private Handler handler;
        private Microsoft.Office.Interop.Excel.Application ReporteExcel = null;
        private Microsoft.Office.Interop.Excel.Workbook WB = null;
        public Microsoft.Office.Interop.Excel.Worksheet SheetCygnus = null;
        public Microsoft.Office.Interop.Excel.Worksheet SheetAzure = null;

        public ReportViewModel(Handler handler)
        {
            this.handler = handler;
            FechaDesde = DateTime.Now;
            FechaHasta = DateTime.Now;
        }

        public DateTime FechaDesde
        {
            get { return fechaDesde; }
            set { SetProperty(ref fechaDesde, value); }
        }

        public DateTime FechaHasta
        {
            get { return fechaHasta; }
            set { SetProperty(ref fechaHasta, value); }
        }

        public async Task pGeneraReporteAsync()
        {
            string pat = handler.Azure.Token;
            string auth = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($":{pat}")
            );

            try
            {
                string archivoTemporal = Environment.CurrentDirectory + "\\ReporteHoras.xlsx";

                /*if (string.IsNullOrEmpty(FechaDesde.ToString()))
                {
                    handler.MensajeError("Ingrese fecha desde");
                    return;
                }

                if (string.IsNullOrEmpty(FechaHasta.ToString()))
                {
                    handler.MensajeError("Ingrese fecha hasta");
                    return;
                }

                if (FechaHasta < FechaDesde)
                {
                    handler.MensajeError("La fecha hasta debe ser mayor que la fecha desde");
                    return;
                }*/

                /*if ((FechaHasta.Date - FechaDesde.Date).Days > 90)
                {
                    handler.MensajeError("No se recomienda buscar más de 90 días");
                    return;
                }*/

                //Open(archivoTemporal);
                //CreateHeader();
                //InsertDataCygnus();
                //pInsertaTareaAzure();
                //Close();

                //handler.pGuardaArchivoByte(archivoTemporal, "ReporteHoras[" + FechaDesde.Day + FechaDesde.Month + FechaDesde.Year + "-" + FechaHasta.Day + FechaHasta.Month + FechaHasta.Year + "].xlsx");
                //File.Delete(archivoTemporal);

                string org = "grupoepm";
                string project = "OPEN";

                string minDate = "2026-01-20T17:01:00Z";
                string maxDate = "2026-01-22T17:00:00Z";

                string url =
                    $"https://vsrm.dev.azure.com/{org}/{project}/_apis/release/releases" +
                    $"?minCreatedTime={minDate}" +
                    $"&maxCreatedTime={maxDate}" +
                    $"&status=active" +
                    $"&api-version=7.1";

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", auth);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                foreach (JsonElement item in root.GetProperty("value").EnumerateArray())
                {
                    int id = item.GetProperty("id").GetInt32();
                    string name = item.GetProperty("name").GetString();
                    string status = item.GetProperty("status").GetString();

                    Console.WriteLine($"{id} - {name} - {status}");

                    if (status.Equals("active"))
                    {
                        JsonElement environments = item.GetProperty("releaseDefinition");

                        string envName = environments.GetProperty("path").GetString();

                        //Console.WriteLine($"{envName}");

                        if (envName.ToUpper().IndexOf("PRODUCCION") > 0)
                        {
                            JsonElement creadoPor = item.GetProperty("createdFor");

                            string analista = creadoPor.GetProperty("displayName").GetString();

                            string stDesc = item.GetProperty("description").GetString();

                            string liderTI = "";

                            try
                            {

                                JsonDocument jsonDesc = JsonDocument.Parse(stDesc);

                                JsonElement descripcion = jsonDesc.RootElement;

                                string Grupo = descripcion.GetProperty("Grupo").GetString();
                                string HU = descripcion.GetProperty("HU").GetString();
                                string TipoAT = descripcion.GetProperty("TipoAT").GetString();
                                string Descripcion = descripcion.GetProperty("Descripcion").GetString();
                                string GenIndis = descripcion.GetProperty("GenIndis").GetString();
                                string Precedencia = descripcion.GetProperty("Precedencia").GetString();

                                url =
                                $"https://vsrm.dev.azure.com/{org}/{project}/_apis/release/releases/" +
                                $"{id}" +
                                $"?api-version=7.1";

                                client = new HttpClient();
                                client.DefaultRequestHeaders.Authorization =
                                    new AuthenticationHeaderValue("Basic", auth);

                                response = await client.GetAsync(url);
                                response.EnsureSuccessStatusCode();

                                string jsonRel = await response.Content.ReadAsStringAsync();
                                JsonDocument docRel = JsonDocument.Parse(jsonRel);
                                JsonElement rootRel = docRel.RootElement;

                                string nameCiclos = "";

                                foreach (JsonElement itemEnv in rootRel.GetProperty("environments").EnumerateArray())
                                {
                                    string nameCiclo = itemEnv.GetProperty("name").GetString();
                                    nameCiclo = nameCiclo.Substring(5).Trim();
                                    //Console.WriteLine($"{nameCiclo}");

                                    string statusRel = itemEnv.GetProperty("status").GetString();

                                    if (statusRel.Equals("inProgress"))
                                    {
                                        nameCiclos = nameCiclo + " " + nameCiclos;

                                        foreach (JsonElement itemAprobado in itemEnv.GetProperty("preDeployApprovals").EnumerateArray())
                                        {
                                            JsonElement aprobadoPor = itemAprobado.GetProperty("approvedBy");

                                            string estadoApro = itemAprobado.GetProperty("status").GetString();
                                            string fechaApro = itemAprobado.GetProperty("modifiedOn").GetString();

                                            DateTimeOffset dto = DateTimeOffset.Parse(fechaApro);
                                            DateTime utcFechaAprob = dto.UtcDateTime;

                                            DateTimeOffset dtoLimite = DateTimeOffset.Parse(maxDate);
                                            DateTime utcLimite = dtoLimite.UtcDateTime;


                                            if (estadoApro.Equals("approved") && utcFechaAprob <= utcLimite)
                                            {
                                                liderTI = aprobadoPor.GetProperty("displayName").GetString();
                                            }
                                        }
                                    }
                                }

                                if(!string.IsNullOrEmpty(liderTI))
                                    Console.WriteLine($"---------------|{Grupo}|{HU}|{analista}|{Descripcion}|{TipoAT}|{nameCiclos}|{liderTI}|-|{GenIndis}|{Precedencia}|https://grupoepm.visualstudio.com/OPEN/_releaseProgress?_a=release-pipeline-progress&releaseId={id}");
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"Error --- {stDesc}");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //Close();
                handler.MensajeError(ex.Message);
            }
        }

        private void Open(string Location)
        {
            ReporteExcel = new Microsoft.Office.Interop.Excel.Application();
            ReporteExcel.Visible = false;
            ReporteExcel.DisplayAlerts = false;
            WB = ReporteExcel.Workbooks.Add(Type.Missing);
            SheetCygnus = (Microsoft.Office.Interop.Excel.Worksheet)WB.ActiveSheet;
            SheetCygnus.Name = "Cygnus";

            SheetAzure = (Microsoft.Office.Interop.Excel.Worksheet)WB.Worksheets.Add
                            (System.Reflection.Missing.Value,
                             WB.Worksheets[WB.Worksheets.Count],
                             System.Reflection.Missing.Value,
                             System.Reflection.Missing.Value);
            SheetAzure.Name = "Azure";

            WB.SaveAs(Location);
        }

        private void CreateHeader()
        {
            this.SheetCygnus.Cells[1, 1] = "Fecha";
            this.SheetCygnus.Cells[1, 2] = "HU";
            this.SheetCygnus.Cells[1, 3] = "Task";
            this.SheetCygnus.Cells[1, 4] = "Descripcion";
            this.SheetCygnus.Cells[1, 5] = "Horas";

            this.SheetAzure.Cells[1, 1] = "Fecha";
            this.SheetAzure.Cells[1, 2] = "Task";
            this.SheetAzure.Cells[1, 3] = "Descripcion";
            this.SheetAzure.Cells[1, 4] = "Estado";
            this.SheetAzure.Cells[1, 5] = "Horas";
        }
        private void InsertDataCygnus()
        {
        }

        public void pInsertaTareaAzure()
        {
        }

        private void Close()
        {
            if (this.ReporteExcel.ActiveWorkbook != null)
                this.ReporteExcel.ActiveWorkbook.Save();
            if (this.ReporteExcel != null)
            {
                if (this.WB != null)
                {
                    if (this.SheetCygnus != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(this.SheetCygnus);
                    this.WB.Close(false, Type.Missing, Type.Missing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.WB);
                }
                this.ReporteExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(this.ReporteExcel);
            }
        }

        public void OnClean(object commandParameter)
        {
            throw new NotImplementedException();
        }

        public void OnConection(object commandParameter)
        {
            throw new NotImplementedException();
        }

        public void OnProcess(object commandParameter)
        {
            throw new NotImplementedException();
        }

        /*public async Task InsertDataAzure(SynchronizationContext uiContext, DateTime fechaDesde, DateTime fechaHasta)
        {
            string log = "Antes de traer los items - ";
            CultureInfo culture = new CultureInfo("es-CO");

            var workItems = await QueryOpenBugs(fechaDesde, fechaHasta).ConfigureAwait(false);
            List<TareaHoja>  listaTareaAzure = new List<TareaHoja>();

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

                    listaTareaAzure.Add(tarea);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}\t Error: " + ex.Message, workItem.Id);
                }
            }            

            uiContext.Send(x => pInsertaTareaAzure(listaTareaAzure), null);
        }

        public async Task<IList<WorkItem>> QueryOpenBugs(DateTime fechaDesde, DateTime fechaHasta)
        {
            string personalAccessToken = res.TokenAzureConn; //;

            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);
            VssConnection connection = null;
            connection = new VssConnection(new Uri(handler.View.UrlAzure), credentials);

            // create a wiql object and build our query
            var wiql = new Wiql()
            {

                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id] " +
                        "From WorkItems " +
                        "Where [System.WorkItemType] = 'Task' " +
                        "And [System.State] not in ('Removed') " +
                        "And [System.AssignedTo] = '" + handler.View.UsuarioAzure + "'" +
                        "And [Microsoft.VSTS.Scheduling.StartDate] >= " + fechaDesde+
                        "And [Microsoft.VSTS.Scheduling.StartDate] <= " + fechaHasta
            };
            //"And [System.TeamProject] = '" + project + "' " +

            // create instance of work item tracking http client
            using (var httpClient = new WorkItemTrackingHttpClient(new Uri(handler.View.UrlAzure), credentials))
            {
                // execute the query to get the list of work items in the results
                var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
                var ids = result.WorkItems.Select(item => item.Id).ToArray();

                // build a list of the fields we want to see
                var fields = new[] { "System.Id", "System.Title", "System.State", "System.AssignedTo", "Microsoft.VSTS.Scheduling.CompletedWork", "System.CreatedDate", "Microsoft.VSTS.Scheduling.StartDate" };

                // get work items for the ids found in query
                return await httpClient.GetWorkItemsAsync(ids, fields, result.AsOf).ConfigureAwait(false);
            }
        }*/
    }
}

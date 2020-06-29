using Cygnus2_0.General;
using Cygnus2_0.General.Times;
using Cygnus2_0.Interface;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

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
        SynchronizationContext uiContext;

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

        public void pGeneraReporte()
        {
            if (string.IsNullOrEmpty(FechaDesde.ToString()))
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
            }

            if ((FechaHasta.Date - FechaDesde.Date).Days > 90)
            {
                handler.MensajeError("No se recomienda buscar más de 90 días");
                return;
            }

            uiContext = SynchronizationContext.Current;

            Open(Environment.CurrentDirectory + "\\MyExcel.xlsx");
            CreateHeader();
            InsertDataCygnus();
            InsertDataAzure(uiContext, FechaDesde,fechaHasta);
            Close();
        }

        private void Open(string Location)
        {
            ReporteExcel = new Microsoft.Office.Interop.Excel.Application();

            this.ReporteExcel = new Microsoft.Office.Interop.Excel.Application();
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
            this.SheetAzure.Cells[1, 4] = "Horas";
        }
        private void InsertDataCygnus()
        {
            int ind = 2;
            List<TareaHoja> ListaTareas = handler.DAO.pObtListaTareas(this);

            foreach (TareaHoja field in ListaTareas)
            {
                this.SheetCygnus.Cells[ind, 1] = field.FechaCreacion;
                this.SheetCygnus.Cells[ind, 2] = field.HU;
                this.SheetCygnus.Cells[ind, 3] = field.IdAzure;
                this.SheetCygnus.Cells[ind, 4] = field.Descripcion;
                this.SheetCygnus.Cells[ind, 5] = field.Total;
                ind++;
            }
        }

        public void pInsertaTareaAzure(List<TareaHoja> lista)
        {
            int ind = 2;

            foreach (TareaHoja field in lista)
            {
                this.SheetAzure.Cells[ind, 1] = field.FechaCreacion;
                this.SheetAzure.Cells[ind, 2] = field.IdAzure;
                this.SheetAzure.Cells[ind, 3] = field.Descripcion;
                this.SheetAzure.Cells[ind, 4] = field.Total;
                ind++;
            }
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

        public async Task InsertDataAzure(SynchronizationContext uiContext, DateTime fechaDesde, DateTime fechaHasta)
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
            string personalAccessToken = res.TokenAzureConn; //"trrveg7rc4kp7fng4gxkp6r527ahwj2qncfvtx7gcoe3ljwpz7tq";

            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);
            VssConnection connection = null;
            connection = new VssConnection(new Uri(handler.ConnViewModel.UrlAzure), credentials);

            // create a wiql object and build our query
            var wiql = new Wiql()
            {

                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id] " +
                        "From WorkItems " +
                        "Where [System.WorkItemType] = 'Task' " +
                        "And [System.State] not in ('Removed') " +
                        "And [System.AssignedTo] = '" + handler.ConnViewModel.UsuarioAzure + "'" +
                        "And [Microsoft.VSTS.Scheduling.StartDate] >= " + fechaDesde+
                        "And [Microsoft.VSTS.Scheduling.StartDate] <= " + fechaHasta
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
    }
}

using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Cygnus2_0.General.Times;
using System.Data;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;

namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Times : UserControl
    {
        private Handler handler;
        private TimesViewModel view;
        private MainWindow myWin;
        private Brush backgroundBtn;
        private Brush foregroudnBtn;
        SynchronizationContext uiContext;
        private Boolean blConsultaAsure;
        private string sbDiasAtras;
        private int nuTiempoEsperaAz1;
        private int nuTiempoEsperaAz2;
        private int nuTiempoEsperaAz3;
        private Brush Colobk;
        public Times()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new TimesViewModel(handler);

            DataContext = view;
            InitializeComponent();

            pSeteaFechaActual();
            
            backgroundBtn = btnProcesar.Background;
            foregroudnBtn = btnProcesar.Foreground;

            pbStatus.Visibility = Visibility.Hidden;
            txtProgressBar.Visibility = Visibility.Hidden;

            blConsultaAsure = true;
            sbDiasAtras = "90";
            nuTiempoEsperaAz1 = 20000;
            nuTiempoEsperaAz2 = 10000;
            nuTiempoEsperaAz3 = 5000;

            dataGridObjetos.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        private void dataGridObjetos_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            grid.CommitEdit();

            TareaHoja tarea = grid.CurrentCell.Item as TareaHoja;

            try
            {
                if (tarea != null)
                {
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        TareaHoja fila = ((TareaHoja)grid.Items[i]);
                        fila.pCalcularTotal();
                        fila.Total = fila.Total;
                    }

                    grid.Items.Refresh();
                    view.pCalcularTotales();
                }
            }
            catch { }
        }

        private void btnAdn_Click(object sender, RoutedEventArgs e)
        {
            view.DescWindow = "";
            view.IdAzureWindow = 0;
            upTask ven = new upTask(view,handler,null);
            ven.ShowDialog();

            if(!string.IsNullOrEmpty(view.DescWindow) || view.TareaPred != null)
                pActivarBtnGuardar();
        }
        
        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            int SeqNegativa;
            double TotalHojaActual = 0;
            try
            {
                if (view.ListaTareasEliminar.Count > 0)
                {
                    foreach (TareaHoja tarea in view.ListaTareasEliminar)
                    {
                        handler.DAO.pEliminaTareaAzure(tarea);
                    }

                    view.ListaTareasEliminar.Clear();
                }

                foreach (TareaHoja tarea in view.HojaActual.ListaTareas)
                {
                    if(tarea.Tipo.Equals("L") && tarea.Total == 0)
                    {
                        continue;
                    }

                    SeqNegativa = handler.DAO.pActualizaTareaAzure(tarea);

                    if (SeqNegativa < 0)
                    {
                        view.HojaActual.ListaTareas.ElementAt(view.HojaActual.ListaTareas.IndexOf(tarea)).IdAzure = SeqNegativa;
                    }

                    TotalHojaActual = TotalHojaActual + tarea.Total;
                }
                
                handler.MensajeOk("Hoja guardada con éxito");
                view.ListaTareas.Clear();
                view.ListaHojas.Clear();
                view.pRefrescaTareas();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }

            pDesactivarBtnGuardar();                        
            pSeteaFechaActual();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (blConsultaAsure)
            {
                uiContext = SynchronizationContext.Current;
                pCargarTareasAzure();
                blConsultaAsure = false;
            }
        }

        private void pCargarTareasAzure()
        {
            view.pLimpiarTareas();
            pSeteaFechaActual();

            if (handler.ConexionOracle.ConexionOracleSQL.State != System.Data.ConnectionState.Closed)
            {
                uiContext = SynchronizationContext.Current;

                pbStatus.Visibility = Visibility.Visible;
                txtProgressBar.Visibility = Visibility.Visible;

                BackgroundWorker bw = new BackgroundWorker();

                bw.DoWork -= new DoWorkEventHandler(backgroundWorker);
                bw.DoWork += new DoWorkEventHandler(backgroundWorker);
                bw.ProgressChanged += worker_ProgressChanged;
                bw.WorkerSupportsCancellation = true;
                bw.WorkerReportsProgress = true;
                bw.RunWorkerAsync();
            }
        }

        private void dataGridObjetos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            pActivarBtnGuardar();
        }

        private void pActivarBtnGuardar()
        {
            btnProcesar.IsEnabled = true;
            btnProcesar.Background = Brushes.Orange;
            btnProcesar.Foreground = Brushes.Black;
            handler.GuardarTiempos = true;
        }

        private void pDesactivarBtnGuardar()
        {
            btnProcesar.Background = backgroundBtn;
            btnProcesar.Foreground = foregroudnBtn;
            btnProcesar.IsEnabled = false;
            handler.GuardarTiempos = false;
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            TareaHoja tarea = dataGridObjetos.SelectedItem as TareaHoja;

            if (tarea != null)
            {
                if (handler.MensajeConfirmacion("Está seguro que desea eliminar la tarea [ " + tarea.Descripcion + " ]?") == "Y")
                {
                    view.HojaActual.ListaTareas.Remove(tarea);
                    view.ListaTareas.Remove(tarea);
                    view.ListaTareasEliminar.Add(tarea);
                    pActivarBtnGuardar();
                }
            }            
        }

        private void comboBoxHojas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxHojas.SelectedItem != null)
            {
                pRefrescaEncabezado();

                //dataGridObjetos.Columns[0].SortDirection = ListSortDirection.Descending;

                /*pEstiloNormalGrilla();


                if (fecha_ini.Day == DateTime.Now.Day)
                    pPintarDiaACtual(2);

                if (fecha_ini.AddDays(1).Day == DateTime.Now.Day)
                    pPintarDiaACtual(3);

                if (fecha_ini.AddDays(2).Day == DateTime.Now.Day)
                    pPintarDiaACtual(4);

                if (fecha_ini.AddDays(3).Day == DateTime.Now.Day)
                    pPintarDiaACtual(5);

                if (fecha_ini.AddDays(4).Day == DateTime.Now.Day)
                    pPintarDiaACtual(6);

                if (fecha_ini.AddDays(5).Day == DateTime.Now.Day)
                    pPintarDiaACtual(7);

                if (fecha_ini.AddDays(6).Day == DateTime.Now.Day)
                    pPintarDiaACtual(8);*/
            }            
        }

        public void pRefrescaEncabezado()
        {
            DateTime fecha_ini;

            try
            {
                view.pObtTareasPorHoja();
                view.pCalcularTotales();

                if (DateTime.Now.Date >= view.HojaActual.FechaIni && DateTime.Now.Date <= view.HojaActual.FechaFin)
                {
                    txtTitulo.FontStyle = FontStyles.Italic;
                    txtTitulo.FontWeight = FontWeights.UltraBold;
                    txtTitulo.FontSize = 22;
                }
                else
                {
                    txtTitulo.TextDecorations = null;
                    txtTitulo.FontStyle = FontStyles.Normal;
                    txtTitulo.FontWeight = FontWeights.Normal;
                    txtTitulo.FontSize = 18;
                    txtTitulo.FontWeight = FontWeights.SemiBold;
                }

                fecha_ini = view.HojaActual.FechaIni;

                dataGridObjetos.Columns[3].Header = "Lun/" + fecha_ini.Day + "\n    [" + view.TotalMon + "]";
                dataGridObjetos.Columns[4].Header = "Mar/" + fecha_ini.AddDays(1).Day + "\n    [" + view.TotalTue + "]";
                dataGridObjetos.Columns[5].Header = "Mie/" + fecha_ini.AddDays(2).Day + "\n    [" + view.TotalWed + "]";
                dataGridObjetos.Columns[6].Header = "Jue/" + fecha_ini.AddDays(3).Day + "\n    [" + view.TotalThu + "]";
                dataGridObjetos.Columns[7].Header = "Vie/" + fecha_ini.AddDays(4).Day + "\n    [" + view.TotalFri + "]";
                dataGridObjetos.Columns[8].Header = "Sab/" + fecha_ini.AddDays(5).Day + "\n    [" + view.TotalSat + "]";
                dataGridObjetos.Columns[9].Header = "Dom/" + fecha_ini.AddDays(6).Day + "\n    [" + view.TotalSun + "]";
                dataGridObjetos.Columns[10].Header = "Total" + "\n  [" + view.Total + "]";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void pCambiarColorFila()
        {
            foreach (TareaHoja item in dataGridObjetos.ItemsSource)
            {
                var row = dataGridObjetos.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

                if (row != null)
                {
                    if(row.Background != Brushes.Red)
                        Colobk = row.Background;

                    if (Math.Round(item.TotalRQ,1) != Math.Round(item.Completed,1) && item.IdAzure > 0)
                    {
                        row.Background = Brushes.Red;
                    }
                    else
                    {
                        row.Background = Colobk;
                    }
                }
            }
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (dataGridObjetos.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                pCambiarColorFila();
            }
        }

        public void pPintarDiaACtual(int indice)
        {
            /*var cellStyle = new Style { TargetType = typeof(DataGridCell), };
            cellStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.ExtraBold));
            cellStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.Red));
            dataGridObjetos.Columns[indice].CellStyle = cellStyle;

            var cellHeaderStyle = new Style { TargetType = typeof(DataGridColumnHeader), };
            cellHeaderStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.ExtraBold));
            cellHeaderStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.Red));
            dataGridObjetos.Columns[indice].HeaderStyle = cellHeaderStyle;*/
        }

        private void pEstiloNormalGrilla()
        {
            /*foreach(DataGridColumn column in dataGridObjetos.Columns)
            {
                var cellStyle = new Style { TargetType = typeof(DataGridCell), };
                cellStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.Normal));
                column.CellStyle = cellStyle;

                var cellHeaderStyle = new Style { TargetType = typeof(DataGridColumnHeader), };
                cellHeaderStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.Normal));
                column.HeaderStyle = cellHeaderStyle;
            }*/
        }

        private void MenuItemModif_Click(object sender, RoutedEventArgs e)
        {
            TareaHoja tarea = dataGridObjetos.SelectedItem as TareaHoja;

            if (tarea != null)
            {
                view.DescWindow = tarea.Descripcion;
                view.IdAzureWindow = tarea.IdAzure;
                upTask ven = new upTask(view, handler, tarea);
                ven.ShowDialog();

                if (ven.Modifica)
                {
                    pActivarBtnGuardar();
                    view.HojaActual.ListaTareas.ElementAt(view.HojaActual.ListaTareas.IndexOf(tarea)).IdAzure = view.IdAzureWindow;
                    view.HojaActual.ListaTareas.ElementAt(view.HojaActual.ListaTareas.IndexOf(tarea)).Descripcion = view.DescWindow;
                    dataGridObjetos.Items.Refresh();
                }
            }
        }

        private void MenuItemDeta_Click(object sender, RoutedEventArgs e)
        {
            TareaHoja tarea = dataGridObjetos.SelectedItem as TareaHoja;

            if (tarea != null)
            {
                view.pObtDetalleRq(tarea);
                DetailTask ven = new DetailTask(view, handler, tarea);
                ven.Show();
            }
        }

        private void backgroundWorker(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker).ReportProgress(20);

            view.ObtTareasAzure(uiContext, sbDiasAtras);
            Thread.Sleep(nuTiempoEsperaAz1);

            (sender as BackgroundWorker).ReportProgress(40);
            Thread.Sleep(nuTiempoEsperaAz2);

            (sender as BackgroundWorker).ReportProgress(60);
            Thread.Sleep(nuTiempoEsperaAz3);

            //uiContext.Send(x => view.pObtTareasPorHoja(), null);
            //uiContext.Send(x => view.pCalcularTotales(), null);

            (sender as BackgroundWorker).ReportProgress(90);
            Thread.Sleep(100);

            (sender as BackgroundWorker).ReportProgress(100);
            Thread.Sleep(100);

            uiContext.Send(x => pSetTiemposNuevos(), null);
            uiContext.Send(x => pRefrescaEncabezado(), null);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;

            if (pbStatus.Value == pbStatus.Maximum)
            {
                pbStatus.Visibility=Visibility.Hidden;
                txtProgressBar.Visibility = Visibility.Hidden;
            }
        }

        private void btnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            view.pLimpiarTareas();

            view.ListaTareas.Clear();
            view.ListaHojas.Clear();
            view.pRefrescaTareas();

            pSeteaFechaActual();
            pRefrescaEncabezado();
        }

        public void pSeteaFechaActual()
        {
            //Se selecciona la semana actual
            comboBoxHojas.SelectedIndex = 1;
            view.pCalcularTotales();
        }

        private void MenuItemCorre_Click(object sender, RoutedEventArgs e)
        {
            TareaHoja tarea = dataGridObjetos.SelectedItem as TareaHoja;

            if (tarea != null)
            {
                if (Convert.ToInt64(tarea.IdAzure) > 0)
                {
                    handler.MensajeError("Esta tarea ya tiene creada historia de usuario en Azure. Por favor validar.");
                    return;
                }

                SendMail mail = new SendMail(view,handler,tarea);
                mail.ShowDialog();
            }
        }

        private void MenuItemCombi_Click(object sender, RoutedEventArgs e)
        {
            TareaHoja tarea = dataGridObjetos.SelectedItem as TareaHoja;

            if(btnProcesar.IsEnabled)
            {
                handler.MensajeError("Guarde primero los cambios realizados.");
                return;
            }

            if (tarea != null)
            {
                Combine combine = new Combine(view, handler, tarea);
                combine.ShowDialog();

                if (combine.Modifica)
                {
                    view.ListaHojas.Clear();
                    view.ListaTareas.Clear();
                    view.pRefrescaTareas();
                    pSeteaFechaActual();
                }
            }
        }

        private void BtnDescargar_Click(object sender, RoutedEventArgs e)
        {
            if (btnProcesar.IsEnabled == false)
            {
                pCargarTareasAzure();
            }
            else
            {
                handler.MensajeAdvertencia("Guarde primero la información!");
            }
        }

        private void pSetTiemposNuevos()
        {
            sbDiasAtras = "15";
            nuTiempoEsperaAz1 = 8000;
            nuTiempoEsperaAz2 = 3000;
            nuTiempoEsperaAz3 = 1000;
        }
    }
}

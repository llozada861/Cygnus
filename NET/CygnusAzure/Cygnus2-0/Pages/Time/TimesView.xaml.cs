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
using res = Cygnus2_0.Properties.Resources;
using Cygnus2_0.DAO;
using FirstFloor.ModernUI.Windows.Controls;

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
        private int nuTiempoEsperaAz1;
        private int nuTiempoEsperaAz2;
        private int nuTiempoEsperaAz3;
        private Brush Colobk;
        private Style cellStyleBefore;
        private System.Windows.Media.Color color;
        private SolidColorBrush brush;
        public Times()
        {
            myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;

            view = new TimesViewModel(handler);

            DataContext = view;
            InitializeComponent();
            color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.keyThemeColor)).Value);
            brush = new SolidColorBrush(color);

            pSeteaFechaActual();
            
            backgroundBtn = btnProcesar.Background;
            foregroudnBtn = btnProcesar.Foreground;

            pbStatus.Visibility = Visibility.Hidden;
            txtProgressBar.Visibility = Visibility.Hidden;

            blConsultaAsure = true;
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
            view.Model.DescWindow = "";
            view.Model.IdAzureWindow = 0;

            UCUpTask ven = new UCUpTask(view,handler,null);

            var wnd = new ModernWindow
            {
                Style = (Style)App.Current.Resources["BlankWindow"],
                Title = "Tarea Personalizada",
                IsTitleVisible = true,
                Content = ven,
                Width = 450,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            wnd.ShowDialog();

            if (!string.IsNullOrEmpty(view.Model.DescWindow) || view.Model.TareaPred != null)
                pActivarBtnGuardar();
        }
        
        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            int SeqNegativa;
            double TotalHojaActual = 0;

            try
            {
                if (view.Model.ListaTareasEliminar.Count > 0)
                {
                    foreach (TareaHoja tarea in view.Model.ListaTareasEliminar)
                    {
                        SqliteDAO.pEliminaTareaAzure(tarea);
                    }

                    view.Model.ListaTareasEliminar.Clear();
                }

                foreach (TareaHoja tarea in view.Model.HojaActual.ListaTareas)
                {
                    SeqNegativa = 0;

                    if (tarea.Tipo.Equals("L") && tarea.Total == 0)
                    {
                        continue;
                    }

                    if(tarea.IdAzure == 0)
                    {
                        SeqNegativa = SqliteDAO.pObtSecuencia();
                        tarea.Id = SeqNegativa.ToString();
                        view.Model.HojaActual.ListaTareas.ElementAt(view.Model.HojaActual.ListaTareas.IndexOf(tarea)).IdAzure = SeqNegativa;
                        tarea.HU = SeqNegativa;
                        tarea.IniFecha = view.Model.HojaActual.FechaIni.ToString("yyyy-MM-dd");
                        tarea.DescripcionHU = tarea.Descripcion;
                        SqliteDAO.pInsertaTareaAzure(tarea, handler, "I", "G");
                    }

                    if (SeqNegativa == 0)                    
                        SqliteDAO.pInsertaTareaAzure(tarea, handler, "A","G");
                    
                    tarea.pCalcularTotal();

                    TotalHojaActual = TotalHojaActual + tarea.Total;
                }
                
                handler.MensajeOk("Hoja guardada con éxito");
                view.Model.ListaTareas.Clear();
                view.Model.ListaHojas.Clear();
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

            color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.keyThemeColor)).Value);
            brush = new SolidColorBrush(color);

            if(view.Model.HojaActual != null)
                pintarGrilla(view.Model.HojaActual.FechaIni);
        }

        private void pCargarTareasAzure()
        {
            view.pLimpiarTareas();
            pSeteaFechaActual();

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
                    view.Model.HojaActual.ListaTareas.Remove(tarea);
                    view.Model.ListaTareas.Remove(tarea);
                    view.Model.ListaTareasEliminar.Add(tarea);
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

                //pEstiloNormalGrilla();
            }            
        }

        public void pRefrescaEncabezado()
        {
            DateTime fecha_ini;

            try
            {
                view.pObtTareasPorHoja();
                view.pCalcularTotales();

                if (DateTime.Now.Date >= view.Model.HojaActual.FechaIni && DateTime.Now.Date <= view.Model.HojaActual.FechaFin)
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

                fecha_ini = view.Model.HojaActual.FechaIni;

                dataGridObjetos.Columns[3].Header = "Lun/" + fecha_ini.Day + "\n    [" + view.Model.TotalMon + "]";
                dataGridObjetos.Columns[4].Header = "Mar/" + fecha_ini.AddDays(1).Day + "\n    [" + view.Model.TotalTue + "]";
                dataGridObjetos.Columns[5].Header = "Mie/" + fecha_ini.AddDays(2).Day + "\n    [" + view.Model.TotalWed + "]";
                dataGridObjetos.Columns[6].Header = "Jue/" + fecha_ini.AddDays(3).Day + "\n    [" + view.Model.TotalThu + "]";
                dataGridObjetos.Columns[7].Header = "Vie/" + fecha_ini.AddDays(4).Day + "\n    [" + view.Model.TotalFri + "]";
                dataGridObjetos.Columns[8].Header = "Sab/" + fecha_ini.AddDays(5).Day + "\n    [" + view.Model.TotalSat + "]";
                dataGridObjetos.Columns[9].Header = "Dom/" + fecha_ini.AddDays(6).Day + "\n    [" + view.Model.TotalSun + "]";
                dataGridObjetos.Columns[10].Header = "Total" + "\n  [" + view.Model.Total + "]";

                pintarGrilla(fecha_ini);

                /*for (int i = 0; i < dataGridObjetos.Items.Count; i++)
                {
                    for (int j = 0; j < dataGridObjetos.Columns.Count; j++)
                    {
                        DataGridCell cell = dataGridObjetos.GetValue
                        TextBlock tb = cell.Content as TextBlock;

                        if (j == 1)
                        {
                            double measure = double.Parse(tb.Text);

                            if (measure > 22.5)
                            {
                                cell.Foreground = Brushes.Red;
                            }
                        }
                    }
                }*/
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void pintarGrilla(DateTime fecha_ini)
        {
            var cellStyle = new Style { TargetType = typeof(DataGridCell), };
            cellStyle.Setters.Add(new Setter(BackgroundProperty, brush));

            dataGridObjetos.Columns[3].CellStyle = null;
            dataGridObjetos.Columns[4].CellStyle = null;
            dataGridObjetos.Columns[5].CellStyle = null;
            dataGridObjetos.Columns[6].CellStyle = null;
            dataGridObjetos.Columns[7].CellStyle = null;
            dataGridObjetos.Columns[8].CellStyle = cellStyle;
            dataGridObjetos.Columns[9].CellStyle = cellStyle;

            //Pintar día actual
            if (fecha_ini.Day == DateTime.Now.Day)
                pPintarDiaACtual(3);

            if (fecha_ini.AddDays(1).Day == DateTime.Now.Day)
                pPintarDiaACtual(4);

            if (fecha_ini.AddDays(2).Day == DateTime.Now.Day)
                pPintarDiaACtual(5);

            if (fecha_ini.AddDays(3).Day == DateTime.Now.Day)
                pPintarDiaACtual(6);

            if (fecha_ini.AddDays(4).Day == DateTime.Now.Day)
                pPintarDiaACtual(7);

            if (fecha_ini.AddDays(5).Day == DateTime.Now.Day)
                pPintarDiaACtual(8);

            if (fecha_ini.AddDays(6).Day == DateTime.Now.Day)
                pPintarDiaACtual(9);
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
            var cellStyle = new Style { TargetType = typeof(DataGridCell), };
            cellStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.ExtraBold));
            //cellStyle.Setters.Add(new Setter(BackgroundProperty, Brushes.Purple));
            cellStyle.Setters.Add(new Setter(BorderBrushProperty, brush));
            dataGridObjetos.Columns[indice].CellStyle = cellStyle;

            //cellStyleBefore = dataGridObjetos.Columns[indice].CellStyle;

            /*var cellHeaderStyle = new Style { TargetType = typeof(DataGridColumnHeader), };
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
                view.Model.DescWindow = tarea.Descripcion;
                view.Model.IdAzureWindow = tarea.IdAzure;

                UCUpTask ven = new UCUpTask(view, handler, tarea);

                var wnd = new ModernWindow
                {
                    Style = (Style)App.Current.Resources["BlankWindow"],
                    Title = "Tarea Personalizada",
                    IsTitleVisible = true,
                    Content = ven,
                    Width = 450,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                wnd.ShowDialog();

                if (ven.Modifica)
                {
                    pActivarBtnGuardar();
                    view.Model.HojaActual.ListaTareas.ElementAt(view.Model.HojaActual.ListaTareas.IndexOf(tarea)).IdAzure = view.Model.IdAzureWindow;
                    view.Model.HojaActual.ListaTareas.ElementAt(view.Model.HojaActual.ListaTareas.IndexOf(tarea)).Descripcion = view.Model.DescWindow;
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
                UCDetailTask ven = new UCDetailTask(view, handler, tarea);

                var wnd = new ModernWindow
                {
                    Style = (Style)App.Current.Resources["BlankWindow"],
                    Title = "Detalle de la tarea",
                    IsTitleVisible = true,
                    Content = ven,
                    Width = 600,
                    Height = 450,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                wnd.ShowDialog();
            }
        }

        private void backgroundWorker(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker).ReportProgress(20);

            view.ObtTareasAzure(uiContext);

            (sender as BackgroundWorker).ReportProgress(40);
            Thread.Sleep(100);

            (sender as BackgroundWorker).ReportProgress(60);
            Thread.Sleep(100);

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
            int nuIndex;
            try
            {
                nuIndex = comboBoxHojas.SelectedIndex;

                view.pLimpiarTareas();
                view.Model.ListaTareas.Clear();
                view.Model.ListaHojas.Clear();
                view.pRefrescaTareas();

                if (nuIndex != 1 && handler.MensajeConfirmacion("Desea ir a la Semana Actual?") == "N")
                {
                    comboBoxHojas.SelectedIndex = nuIndex;
                    view.pCalcularTotales();
                    pRefrescaEncabezado();
                }
                else
                {
                    pSeteaFechaActual();
                    pRefrescaEncabezado();
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
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
                    view.Model.ListaHojas.Clear();
                    view.Model.ListaTareas.Clear();
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
            nuTiempoEsperaAz1 = 8000;
            nuTiempoEsperaAz2 = 3000;
            nuTiempoEsperaAz3 = 1000;
        }
    }
}

using Cygnus2_0.Model.Time;
using Cygnus2_0.ViewModel.Time;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Interaction logic for winProgressBar.xaml
    /// </summary>
    public partial class winProgressBar : Window
    {
        private bool called = true;
        private TimeModel model;
        SynchronizationContext uiContext;
        private TimesViewModel view;

        public winProgressBar(TimeModel model, TimesViewModel view, SynchronizationContext uiContext)
        {
            InitializeComponent();
            this.model = model;
            this.uiContext = uiContext;
            this.view = view;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();

            if (called)
            {
                Show();

                BackgroundWorker bw = new BackgroundWorker();

                bw.DoWork -= new DoWorkEventHandler(backgroundWorker);
                bw.DoWork += new DoWorkEventHandler(backgroundWorker);
                bw.ProgressChanged += worker_ProgressChanged;
                bw.WorkerSupportsCancellation = true;
                bw.WorkerReportsProgress = true;
                bw.RunWorkerAsync();
            }

            if (AppearanceManager.Current.ThemeSource == AppearanceManager.LightThemeSource)
            {
                pWindow.Background = Brushes.LightSteelBlue;
            }
            else
            {
                pWindow.Background = Brushes.DimGray;
            }
        }

        private void backgroundWorker(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker).ReportProgress(20);

            if (called)
            {
                model.PrintOpenBugsAsync(uiContext,"1");
                Thread.Sleep(3200);

                (sender as BackgroundWorker).ReportProgress(40);
                Thread.Sleep(200);

                (sender as BackgroundWorker).ReportProgress(60);
                Thread.Sleep(200);
                
                (sender as BackgroundWorker).ReportProgress(90);
                Thread.Sleep(200);

                (sender as BackgroundWorker).ReportProgress(100);
                Thread.Sleep(200);

                //uiContext.Send(x => view.pObtTareasPorHoja(), null);
                //uiContext.Send(x => view.pCalcularTotales(), null);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;

            if (pbStatus.Value == pbStatus.Maximum)
            {
                this.Close();
            }
        }
    }
}

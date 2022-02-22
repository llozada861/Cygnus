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
using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Time;
using Cygnus2_0.General.Times;
using FirstFloor.ModernUI.Presentation;
using res = Cygnus2_0.Properties.Resources;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

namespace Cygnus2_0.Pages.Time
{
    /// <summary>
    /// Interaction logic for upTask.xaml
    /// </summary>
    public partial class upTask : Window
    {
        private Handler handler;
        private TimesViewModel view;
        private TareaHoja tarea;
        private Boolean modifica;

        public upTask(TimesViewModel view, Handler handler,TareaHoja tarea)
        {
            this.view = view;
            this.handler = handler;
            DataContext = view;
            this.tarea = tarea;
            InitializeComponent();
        }

        public Boolean Modifica
        {
            get { return modifica; }
            set { modifica = value; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (rdbNueva.IsChecked == true)
            {
                view.OnAdd(tarea,"N");

                /*string _credentials;
                string batchRequests;

                batchRequests = "[{ \"op\": \"add\", \"path\": \"/fields/System.Title\", \"from\": null, \"value\": \"PRUEBA_LLOZADA\"}]";

                //"[{ \"op\": \"add\", \"path\": \"/fields/System.Title\", \"value\": \"Prueba__LOZADA\" }]";

                "[" +
                              "  {" +
                              "  \"op\": \"add\", " +
                              "  \"path\": \"/fields/System.Title\" ," +
                              "  \"value\": \"Prueba_LLOZADA\" " +
                              "  } " +
                           " ]";

                string json = JsonConvert.SerializeObject(batchRequests, Formatting.Indented);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");
                //var batchRequest = new StringContent(json,Encoding.UTF8, "application/json");
                //var batchRequest = new StringContent(batchRequests, Encoding.UTF8, "application/json");

                string personalAccessToken = res.TokenAzureConn;
                _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var method = new HttpMethod("POST");

                    // send the request
                    var request = new HttpRequestMessage(method, "https://dev.azure.com/grupoepm/OPEN/_apis/wit/workitems/$Task?api-version=6.0") { Content = content };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResponse = response.Content.ReadAsStringAsync();
                        //WorkItemBatchPostResponse batchResponse = response.Content.ReadAsAsync<WorkItemBatchPostResponse>().Result;
                        //return batchResponse;
                    }
                }*/
            }
            else
            {
                view.OnAdd(tarea,"P");
            }

            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Modifica = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Modifica = true;
            
            if (AppearanceManager.Current.ThemeSource == AppearanceManager.DarkThemeSource)
            {
                pWindow.Background = Brushes.DimGray;
            }
            else
            {
                pWindow.Background = Brushes.WhiteSmoke;
            }

            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;

            if (this.tarea != null)
            {
                rdbNueva.IsChecked = true;
                rdbPreder.IsEnabled = false;
            }
            else
            {
                rdbPreder.IsChecked = true;
                rdbPreder.IsEnabled = true;
            }
        }

        private void chkAzure_Checked(object sender, RoutedEventArgs e)
        {
             lblAzureText.Visibility = Visibility.Visible;
             txAzureText.Visibility = Visibility.Visible;
            
        }

        private void chkAzure_Unchecked(object sender, RoutedEventArgs e)
        {
            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;
        }

        private void rdbPreder_Checked(object sender, RoutedEventArgs e)
        {
            pOcultarNuevaTarea();
            pMostrarPrederteminado();
        }

        private void rdbNueva_Checked(object sender, RoutedEventArgs e)
        {
            pOcultarPrederterminada();
            pMostrarNueva();
        }

        public void pOcultarNuevaTarea()
        {
            CasoText.Visibility = Visibility.Hidden;
            chkAzure.Visibility = Visibility.Hidden;
            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;
        }
        public void pMostrarPrederteminado()
        {
            comboBoxPred.Visibility = Visibility.Visible;
        }
        public void pMostrarNueva()
        {
            CasoText.Visibility = Visibility.Visible;
            chkAzure.Visibility = Visibility.Visible;
            lblAzureText.Visibility = Visibility.Hidden;
            txAzureText.Visibility = Visibility.Hidden;
        }
        public void pOcultarPrederterminada()
        {
            comboBoxPred.Visibility = Visibility.Hidden;
        }
    }
}

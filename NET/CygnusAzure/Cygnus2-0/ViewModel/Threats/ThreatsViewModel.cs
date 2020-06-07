using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Threats
{
    public class ThreatsViewModel : ViewModelBase, IViews
    {
        private Handler handler;
        private ObservableCollection<SelectListItem> listaParametros;
        private string nombre;
        private string hilos;
        private string parametro;
        private string tipo;
        private string vistaPrevia;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _add;
        private readonly DelegateCommand _ejecutar;
        private readonly DelegateCommand _principal;
        private readonly DelegateCommand _pre;

        public ThreatsViewModel(Handler handler)
        {
            this.handler = handler;
            this.ListaParametros = new ObservableCollection<SelectListItem>();
            _process = new DelegateCommand(OnProcess);
            _add = new DelegateCommand(OnAdd);
            _ejecutar = new DelegateCommand(OnExecute);
            _principal = new DelegateCommand(OnPrincipal);
            _pre = new DelegateCommand(OnPre);
        }
        public ICommand Process => _process;
        public ICommand Add => _add;
        public ICommand Ejecutar => _ejecutar;
        public ICommand Principal => _principal;
        public ICommand Pre => _pre;

        public ObservableCollection<SelectListItem> ListaParametros
        {
            get { return listaParametros; }
            set { SetProperty(ref listaParametros, value); }
        }
        public string Nombre
        {
            get { return nombre; }
            set { SetProperty(ref nombre, value); }
        }
        public string Hilos
        {
            get { return hilos; }
            set { SetProperty(ref hilos, value); }
        }
        public string Parametro
        {
            get { return parametro; }
            set { SetProperty(ref parametro, value); }
        }
        public string Tipo
        {
            get { return tipo; }
            set { SetProperty(ref tipo, value); }
        }
        public string VistaPrevia
        {
            get { return vistaPrevia; }
            set { SetProperty(ref vistaPrevia, value); }
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
        public void OnPre(object commandParameter)
        {
            throw new NotImplementedException();
        }
        public void OnPrincipal(object commandParameter)
        {
            if (!Validaciones()) return;
            
            VistaPrevia = pObtPrincipal();
        }
        public void OnExecute(object commandParameter)
        {
            StringBuilder ejecucion = new StringBuilder();
            VistaPrevia = "";

            if (!Validaciones()) return;

            ejecucion.AppendLine();
            ejecucion.AppendLine();
            ejecucion.Append(res.EjecutaHilos);
            ejecucion.Replace(res.TagNombreHilos, Nombre.ToUpper());            
            ejecucion.Replace(res.TagParametrosHilos, fsbParametros(0));

            VistaPrevia = ejecucion.ToString();
        }
        public void OnAdd(object commandParameter)
        {
            if(string.IsNullOrEmpty(Parametro))
            {
                handler.MensajeError("Ingrese el nombre del parámetro");
                return;
            }

            if (string.IsNullOrEmpty(Tipo))
            {
                handler.MensajeError("Ingrese el tipo del parámetro");
                return;
            }

            if(ListaParametros.ToList().Exists(x=>x.Text.Equals(Parametro) && x.Observacion.Equals(Tipo)))
            {
                handler.MensajeError("El parámetro ya existe");
                return;
            }

            ListaParametros.Add(new SelectListItem { Text=Parametro,Observacion=Tipo});
        }

        private Boolean Validaciones()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                handler.MensajeError("Ingrese el nombre del proceso sin estándar");
                return false;
            }

            if (string.IsNullOrEmpty(Hilos))
            {
                handler.MensajeError("Ingrese la cantidad de hilos");
                return false;
            }

            return true;
        }

        public string fsbParametros(int tipo)
        {
            string parametros = "";

            if (ListaParametros.Count > 0)
            {
                for (int i = 0; i < ListaParametros.Count; i++)
                {
                    if (tipo == 0)
                    {
                        if (i == ListaParametros.Count - 1)
                            parametros = parametros + "[Valor " + ListaParametros.ElementAt(i).Text + "]";
                        else
                            parametros = parametros + "[Valor " + ListaParametros.ElementAt(i).Text + "],";
                    }
                    else
                    {
                        if (i == ListaParametros.Count - 1)
                            parametros = parametros + ListaParametros.ElementAt(i).Text + "   IN " + ListaParametros.ElementAt(i).Observacion + ",";
                        else
                        {
                            if (i == 0)
                                parametros = parametros + ListaParametros.ElementAt(i).Text + "   IN " + ListaParametros.ElementAt(i).Observacion + ",\n";
                            else
                                parametros = parametros + "        " + ListaParametros.ElementAt(i).Text + "   IN " + ListaParametros.ElementAt(i).Observacion + ",\n";
                        }
                    }
                }
            }
            else
            {
                if(tipo == 0)
                    parametros = "null";
                else
                    parametros = "isbParam        IN  VARCHAR2,";
            }

            return parametros;
        }

        public string fsbParametrosHTML()
        {
            StringBuilder ejecucion = new StringBuilder();

            if (ListaParametros.Count > 0)
            {
                for (int i = 0; i < ListaParametros.Count; i++)
                {
                    ejecucion.Append(handler.HtmlMetodoParam);
                    ejecucion.Replace(res.TagHtmlParamName, ListaParametros.ElementAt(i).Text);
                    ejecucion.Replace(res.TagHtmlParamType, ListaParametros.ElementAt(i).Observacion);
                    ejecucion.Replace(res.TagHtmlParamDir, res.IN);
                    ejecucion.Replace(res.TagHtmlParaDesc, ListaParametros.ElementAt(i).Text);
                }
            }
            else
            {
                ejecucion.Append(handler.HtmlMetodoParam);
                ejecucion.Replace(res.TagHtmlParamName, "isbParam");
                ejecucion.Replace(res.TagHtmlParamType, "VARCHAR2");
                ejecucion.Replace(res.TagHtmlParamDir, res.IN);
                ejecucion.Replace(res.TagHtmlParaDesc, "Parametros adicionales");
            }

            return ejecucion.ToString();
        }
        public string pObtPrincipal()
        {
            StringBuilder ejecucion = new StringBuilder();
            VistaPrevia = "";

            ejecucion.Append(res.PlantillaProcesoMasivo);
            ejecucion.Replace(res.TagNombreHilos, Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, fsbParametros(1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            return ejecucion.ToString();
        }
    }
}

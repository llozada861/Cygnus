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
        private string descripcion;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _add;
        private readonly DelegateCommand _ejecutar;
        private readonly DelegateCommand _principal;
        private readonly DelegateCommand _pre;
        private readonly DelegateCommand _post;
        private readonly DelegateCommand _reg;
        private readonly DelegateCommand _cant;

        public ThreatsViewModel(Handler handler)
        {
            this.handler = handler;
            this.ListaParametros = new ObservableCollection<SelectListItem>();
            _process = new DelegateCommand(OnProcess);
            _add = new DelegateCommand(OnAdd);
            _ejecutar = new DelegateCommand(OnExecute);
            _principal = new DelegateCommand(OnPrincipal);
            _pre = new DelegateCommand(OnPre);
            _post = new DelegateCommand(OnPost);
            _reg = new DelegateCommand(OnReg);
            _cant = new DelegateCommand(OnCant);
        }
        public ICommand Process => _process;
        public ICommand Add => _add;
        public ICommand Ejecutar => _ejecutar;
        public ICommand Principal => _principal;
        public ICommand Pre => _pre;
        public ICommand Post => _post;
        public ICommand Registrar => _reg;
        public ICommand Cantidad => _cant;

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
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
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
        public Boolean ApiPre { get; set; }
        public Boolean ApiPost { get; set; }
        public Boolean ApiCantidad { get; set; }

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
            if (!Validaciones()) return;

            VistaPrevia = pObtPre();
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
        public void OnPost(object commandParameter)
        {
            if (!Validaciones()) return;

            VistaPrevia = pObtPost();
        }
        public void OnCant(object commandParameter)
        {
            if (!Validaciones()) return;

            StringBuilder ejecucion = new StringBuilder();
            VistaPrevia = "";
            string parametros = fsbParametros(1);

            if (!Validaciones()) return;

            ejecucion.Append(res.PlantillaCantidadHilos);
            ejecucion.Replace(res.TagNombreHilos, Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, parametros.Substring(0, parametros.Length - 1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

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
            Parametro = "";
            Tipo = "";
            VistaPrevia = "";
        }
        public void OnReg(object commandParameter)
        {
            if (!Validaciones()) return;

            StringBuilder ejecucion = new StringBuilder();
            VistaPrevia = "";
            string parametros = fsbParametros(1);

            ejecucion.Append(res.PlantillaInsertHilos);
            ejecucion.Replace(res.TagNombreHilos, Nombre.ToUpper());
            ejecucion.Replace(res.TagHTML_Desc, Descripcion.ToUpper());
            ejecucion.Replace(res.TagCantidadHilos, Hilos);
            ejecucion.Replace(res.TagApiPrin, "PAQUETE.pProcesoMasivo" + Nombre.ToUpper());

            if (ApiPre)
            {
                ejecucion.Replace(res.TagApiPre, ",API_CANT_GENERAL");
                ejecucion.Replace(res.TagApiPreVal, ",'PAQUETE.pProcesoPre" + Nombre.ToUpper()+"'");
            }
            else
            {
                ejecucion.Replace(res.TagApiPre, "");
                ejecucion.Replace(res.TagApiPreVal, "");
            }

            if (ApiPost)
            {
                ejecucion.Replace(res.TagApiPost, ",API_POST");
                ejecucion.Replace(res.TagApiPostVal, ",'PAQUETE.pProcesoPost" + Nombre.ToUpper()+"'");
            }
            else
            {
                ejecucion.Replace(res.TagApiPost, "");
                ejecucion.Replace(res.TagApiPostVal, "");
            }

            if (ApiCantidad)
            {
                ejecucion.Replace(res.TagApiCant, ",API_CANTIDAD");
                ejecucion.Replace(res.TagApiCantVal, ",'PAQUETE.fnuObtCantidad" + Nombre.ToUpper()+"'");
            }
            else
            {
                ejecucion.Replace(res.TagApiCant, "");
                ejecucion.Replace(res.TagApiCantVal, "");
            }

            VistaPrevia = ejecucion.ToString();
        }

        private Boolean Validaciones()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                handler.MensajeError("Ingrese el nombre del proceso");
                return false;
            }

            if (string.IsNullOrEmpty(Hilos))
            {
                handler.MensajeError("Ingrese la cantidad de hilos");
                return false;
            }

            if (string.IsNullOrEmpty(Descripcion))
            {
                handler.MensajeError("Ingrese una descripción para el proceso");
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

        public string pObtPre()
        {
            StringBuilder ejecucion = new StringBuilder();
            VistaPrevia = "";
            string parametros = fsbParametros(1);

            ejecucion.Append(res.PlantillaProcesoPre);
            ejecucion.Replace(res.TagNombreHilos, Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, parametros.Substring(0,parametros.Length-1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            return ejecucion.ToString();
        }
        public string pObtPost()
        {
            StringBuilder ejecucion = new StringBuilder();
            VistaPrevia = "";
            string parametros = fsbParametros(1);

            ejecucion.Append(res.PlantillaProcesoPost);
            ejecucion.Replace(res.TagNombreHilos, Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, parametros.Substring(0, parametros.Length - 1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            return ejecucion.ToString();
        }
    }
}

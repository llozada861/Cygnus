using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Threats;
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
    public class ThreatsViewModel : IViews
    {
        private Handler handler;
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

            this.Model = new ThreatsModel();

            this.Model.ListaParametros = new ObservableCollection<SelectListItem>();
            _process = new DelegateCommand(OnProcess);
            _add = new DelegateCommand(OnAdd);
            _ejecutar = new DelegateCommand(OnExecute);
            _principal = new DelegateCommand(OnPrincipal);
            _pre = new DelegateCommand(OnPre);
            _post = new DelegateCommand(OnPost);
            _reg = new DelegateCommand(OnReg);
            _cant = new DelegateCommand(OnCant);
        }
        public ThreatsModel Model { get; set; }

        public ICommand Process => _process;
        public ICommand Add => _add;
        public ICommand Ejecutar => _ejecutar;
        public ICommand Principal => _principal;
        public ICommand Pre => _pre;
        public ICommand Post => _post;
        public ICommand Registrar => _reg;
        public ICommand Cantidad => _cant;

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

            this.Model.VistaPrevia = pObtPre();
        }
        public void OnPrincipal(object commandParameter)
        {
            if (!Validaciones()) return;

            this.Model.VistaPrevia = pObtPrincipal();
        }
        public void OnExecute(object commandParameter)
        {
            StringBuilder ejecucion = new StringBuilder();
            this.Model.VistaPrevia = "";

            if (!Validaciones()) return;

            ejecucion.AppendLine();
            ejecucion.AppendLine();
            ejecucion.Append(res.EjecutaHilos);
            ejecucion.Replace(res.TagNombreHilos, this.Model.Nombre.ToUpper());            
            ejecucion.Replace(res.TagParametrosHilos, fsbParametros(0));

            this.Model.VistaPrevia = ejecucion.ToString();
        }
        public void OnPost(object commandParameter)
        {
            if (!Validaciones()) return;

            this.Model.VistaPrevia = pObtPost();
        }
        public void OnCant(object commandParameter)
        {
            if (!Validaciones()) return;

            StringBuilder ejecucion = new StringBuilder();
            this.Model.VistaPrevia = "";
            string parametros = fsbParametros(1);

            if (!Validaciones()) return;

            ejecucion.Append(res.PlantillaCantidadHilos);
            ejecucion.Replace(res.TagNombreHilos, this.Model.Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, parametros.Substring(0, parametros.Length - 1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            this.Model.VistaPrevia = ejecucion.ToString();
        }
        public void OnAdd(object commandParameter)
        {
            if(string.IsNullOrEmpty(this.Model.Parametro))
            {
                handler.MensajeError("Ingrese el nombre del parámetro");
                return;
            }

            if (string.IsNullOrEmpty(this.Model.Tipo))
            {
                handler.MensajeError("Ingrese el tipo del parámetro");
                return;
            }

            if(this.Model.ListaParametros.ToList().Exists(x=>x.Text.Equals(this.Model.Parametro) && x.Observacion.Equals(this.Model.Tipo)))
            {
                handler.MensajeError("El parámetro ya existe");
                return;
            }

            this.Model.ListaParametros.Add(new SelectListItem { Text= this.Model.Parametro, Observacion= this.Model.Tipo });
            this.Model.Parametro = "";
            this.Model.Tipo = "";
            this.Model.VistaPrevia = "";
        }
        public void OnReg(object commandParameter)
        {
            if (!Validaciones()) return;

            StringBuilder ejecucion = new StringBuilder();
            this.Model.VistaPrevia = "";
            string parametros = fsbParametros(1);

            ejecucion.Append(res.PlantillaInsertHilos);
            ejecucion.Replace(res.TagNombreHilos, this.Model.Nombre.ToUpper());
            ejecucion.Replace(res.TagHTML_Desc, this.Model.Descripcion.ToUpper());
            ejecucion.Replace(res.TagCantidadHilos, this.Model.Hilos);
            ejecucion.Replace(res.TagApiPrin, "PAQUETE.pProcesoMasivo" + this.Model.Nombre.ToUpper());

            if (this.Model.ApiPre)
            {
                ejecucion.Replace(res.TagApiPre, ",API_CANT_GENERAL");
                ejecucion.Replace(res.TagApiPreVal, ",'PAQUETE.pProcesoPre" + this.Model.Nombre.ToUpper()+"'");
            }
            else
            {
                ejecucion.Replace(res.TagApiPre, "");
                ejecucion.Replace(res.TagApiPreVal, "");
            }

            if (this.Model.ApiPost)
            {
                ejecucion.Replace(res.TagApiPost, ",API_POST");
                ejecucion.Replace(res.TagApiPostVal, ",'PAQUETE.pProcesoPost" + this.Model.Nombre.ToUpper()+"'");
            }
            else
            {
                ejecucion.Replace(res.TagApiPost, "");
                ejecucion.Replace(res.TagApiPostVal, "");
            }

            if (this.Model.ApiCantidad)
            {
                ejecucion.Replace(res.TagApiCant, ",API_CANTIDAD");
                ejecucion.Replace(res.TagApiCantVal, ",'PAQUETE.fnuObtCantidad" + this.Model.Nombre.ToUpper()+"'");
            }
            else
            {
                ejecucion.Replace(res.TagApiCant, "");
                ejecucion.Replace(res.TagApiCantVal, "");
            }

            this.Model.VistaPrevia = ejecucion.ToString();
        }

        private Boolean Validaciones()
        {
            if (string.IsNullOrEmpty(this.Model.Nombre))
            {
                handler.MensajeError("Ingrese el nombre del proceso");
                return false;
            }

            if (string.IsNullOrEmpty(this.Model.Hilos))
            {
                handler.MensajeError("Ingrese la cantidad de hilos");
                return false;
            }

            if (string.IsNullOrEmpty(this.Model.Descripcion))
            {
                handler.MensajeError("Ingrese una descripción para el proceso");
                return false;
            }

            return true;
        }

        public string fsbParametros(int tipo)
        {
            string parametros = "";

            if (this.Model.ListaParametros.Count > 0)
            {
                for (int i = 0; i < this.Model.ListaParametros.Count; i++)
                {
                    if (tipo == 0)
                    {
                        if (i == this.Model.ListaParametros.Count - 1)
                            parametros = parametros + "[Valor " + this.Model.ListaParametros.ElementAt(i).Text + "]";
                        else
                            parametros = parametros + "[Valor " + this.Model.ListaParametros.ElementAt(i).Text + "],";
                    }
                    else
                    {
                        if (i == this.Model.ListaParametros.Count - 1)
                            parametros = parametros + this.Model.ListaParametros.ElementAt(i).Text + "   IN " + this.Model.ListaParametros.ElementAt(i).Observacion + ",";
                        else
                        {
                            if (i == 0)
                                parametros = parametros + this.Model.ListaParametros.ElementAt(i).Text + "   IN " + this.Model.ListaParametros.ElementAt(i).Observacion + ",\n";
                            else
                                parametros = parametros + "        " + this.Model.ListaParametros.ElementAt(i).Text + "   IN " + this.Model.ListaParametros.ElementAt(i).Observacion + ",\n";
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

            if (this.Model.ListaParametros.Count > 0)
            {
                for (int i = 0; i < this.Model.ListaParametros.Count; i++)
                {
                    ejecucion.Append(handler.HtmlMetodoParam);
                    ejecucion.Replace(res.TagHtmlParamName, this.Model.ListaParametros.ElementAt(i).Text);
                    ejecucion.Replace(res.TagHtmlParamType, this.Model.ListaParametros.ElementAt(i).Observacion);
                    ejecucion.Replace(res.TagHtmlParamDir, res.IN);
                    ejecucion.Replace(res.TagHtmlParaDesc, this.Model.ListaParametros.ElementAt(i).Text);
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
            this.Model.VistaPrevia = "";

            ejecucion.Append(res.PlantillaProcesoMasivo);
            ejecucion.Replace(res.TagNombreHilos, this.Model.Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, fsbParametros(1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            return ejecucion.ToString();
        }

        public string pObtPre()
        {
            StringBuilder ejecucion = new StringBuilder();
            this.Model.VistaPrevia = "";
            string parametros = fsbParametros(1);

            ejecucion.Append(res.PlantillaProcesoPre);
            ejecucion.Replace(res.TagNombreHilos, this.Model.Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, parametros.Substring(0,parametros.Length-1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            return ejecucion.ToString();
        }
        public string pObtPost()
        {
            StringBuilder ejecucion = new StringBuilder();
            this.Model.VistaPrevia = "";
            string parametros = fsbParametros(1);

            ejecucion.Append(res.PlantillaProcesoPost);
            ejecucion.Replace(res.TagNombreHilos, this.Model.Nombre.ToUpper());
            ejecucion.Replace(res.TagHtml_desarrollador, Environment.UserName);
            ejecucion.Replace(res.TagHtml_fecha, DateTime.Now.ToShortDateString());
            ejecucion.Replace(res.TagParametrosHilos, parametros.Substring(0, parametros.Length - 1));
            ejecucion.Replace(res.TagPARAMETROS_HTML, fsbParametrosHTML());

            return ejecucion.ToString();
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Data
{
    public class ParameterViewModel : ViewModelBase, IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;

        private Handler handler;
        private string parameterId;
        private string descripcion;
        private string valor;
        private string funcion;
        private string tittle;
        private List<SelectListItem> listaTipoDatos;
        private List<SelectListItem> listaTipoDatos2;
        private SelectListItem tipo;

        public ParameterViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);

            handler = hand;
            this.listaTipoDatos2 = new List<SelectListItem>();

            this.listaTipoDatos2.Add(new SelectListItem { Text = "NUMBER" });
            this.listaTipoDatos2.Add(new SelectListItem { Text = "VARCHAR2" });
            this.listaTipoDatos2.Add(new SelectListItem { Text = "DATE" });
            this.ListaTipoDatos = listaTipoDatos2;

            this.Tipo = new SelectListItem();

            /*tittle = "Este tipo de parámetro es para la forma EPMPAR." +
                     "Se utiliza así:" +
                     Environment.NewLine +
                     "Valor numérico: pkg_epm_boparametr.fnuget('PARAMETRO')" +
                     Environment.NewLine +
                     "Valor cadena: pkg_epm_boparametr.fsbget('PARAMETRO')";*/
        }

        public string Tittle
        {
            get { return tittle; }
            set { SetProperty(ref tittle, value); }
        }

        public string ParameterId
        {
            get { return parameterId; }
            set { SetProperty(ref parameterId, value); }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public string Valor
        {
            get { return valor; }
            set { SetProperty(ref valor, value); }
        }
        public SelectListItem Tipo
        {
            get { return tipo; }
            set { SetProperty(ref tipo, value); }
        }
        public string Funcion
        {
            get { return funcion; }
            set { SetProperty(ref funcion, value); }
        }
        public List<SelectListItem> ListaTipoDatos
        {
            get { return listaTipoDatos; }
            set { SetProperty(ref listaTipoDatos, value); }
        }

        public void OnClean(object commandParameter)
        {
            this.ParameterId = "";
            this.Funcion = "";
            this.Valor = "";
            this.ListaTipoDatos = null;
            this.ListaTipoDatos = listaTipoDatos2;
            this.Tipo = new SelectListItem();
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            if (String.IsNullOrEmpty(this.ParameterId))
            {
                handler.MensajeError("Ingrese el identificador del parámetro");
                return;
            }

            if (String.IsNullOrEmpty(this.Tipo.Text))
            {
                handler.MensajeError("Ingrese el tipo del parámetro");
                return;
            }

            pGuardarParametroBD();
            pGenerarArchivo();
        }
        public void pGuardarParametroBD()
        {
            try
            {
                handler.DAO.pCreaParametro(this);
            }
            catch (Exception ex)
            {
                handler.MensajeError("NO se pudo crear el parámetro en base de datos. Establezca nuevamente la conexión desde la ventana PROPIEDADES/CONFIGURACION. [" + ex.Message + "]");
            }
        }
        public void pGenerarArchivo()
        {
            StringBuilder insParam = new StringBuilder();

            insParam.Append(res.PlantillaInsParametr);
            insParam.Replace(res.Tag_parametro, this.ParameterId);
            insParam.Replace(res.Tag_paramvalo, this.Valor);
            insParam.Replace(res.Tag_paramtipo, this.Tipo.Text);
            insParam.Replace(res.Tag_paramdesc, this.Descripcion);
            insParam.Replace(res.Tag_parafunc, this.Funcion);

            handler.pGuardaArchivo(res.NombreArchivoInsepm_parametr + this.ParameterId + res.ExtensionSQL, insParam.ToString());
        }
    }
}

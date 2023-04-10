using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Data;
using Cygnus2_0.Model.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Data
{
    public class ParameterViewModel : IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        private List<SelectListItem> listaTipoDatos2;

        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;

        private Handler handler;

        public ParameterViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);

            handler = hand;

            this.Model = new ParameterModel();

            this.listaTipoDatos2 = new List<SelectListItem>();

            this.listaTipoDatos2.Add(new SelectListItem { Text = "NUMBER" });
            this.listaTipoDatos2.Add(new SelectListItem { Text = "VARCHAR2" });
            this.listaTipoDatos2.Add(new SelectListItem { Text = "DATE" });
            this.Model.ListaTipoDatos = listaTipoDatos2;

            this.Model.Tipo = new SelectListItem();

            /*tittle = "Este tipo de parámetro es para la forma EPMPAR." +
                     "Se utiliza así:" +
                     Environment.NewLine +
                     "Valor numérico: pkg_epm_boparametr.fnuget('PARAMETRO')" +
                     Environment.NewLine +
                     "Valor cadena: pkg_epm_boparametr.fsbget('PARAMETRO')";*/
        }

        public ParameterModel Model { get; set; }

        public void OnClean(object commandParameter)
        {
            this.Model.ParameterId = "";
            this.Model.Funcion = "";
            this.Model.Valor = "";
            this.Model.ListaTipoDatos = null;
            this.Model.ListaTipoDatos = listaTipoDatos2;
            this.Model.Tipo = new SelectListItem();
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            RichTextBox textBox = (RichTextBox) commandParameter;
            this.Model.Valor = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text.TrimEnd('\r', '\n');

            if (String.IsNullOrEmpty(this.Model.ParameterId))
            {
                handler.MensajeError("Ingrese el identificador del parámetro");
                return;
            }

            if (String.IsNullOrEmpty(this.Model.Tipo.Text))
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
                handler.DAO.pCreaParametro(this.Model);
            }
            catch (Exception ex)
            {
                handler.MensajeError("NO se pudo crear el parámetro en base de datos. Establezca nuevamente la conexión desde Ajustes. [" + ex.Message + "]");
            }
        }
        public void pGenerarArchivo()
        {
            StringBuilder insParam = new StringBuilder();

            PlantillasHTMLModel plantilla = handler.ListaHTML.Where(x => x.Nombre.Equals(res.KEY_PLANT_PARAMETRO)).FirstOrDefault();
            insParam.Append(plantilla.Documentacion.Replace("\r\n", "\n"));
            insParam.Replace(":PARAMETRO_ID", "'"+this.Model.ParameterId+"'");
            insParam.Replace(":VALOR", "'" + this.Model.Valor + "'");
            insParam.Replace(":TIPO", "'" + this.Model.Tipo.Text + "'");
            insParam.Replace(":DESCRIPCION", "'" + this.Model.Descripcion + "'");
            insParam.Replace(":FUNCION", "'" + this.Model.Funcion + "'");

            handler.pGuardaArchivo(plantilla.NombreArchivo + this.Model.ParameterId + res.ExtensionSQL, insParam.ToString());
        }
    }
}

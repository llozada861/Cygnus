using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Settings
{
    public class ConfGeneralViewModel
    {
        private readonly DelegateCommand _process;
        private Handler handler;

        public ICommand Process => _process;
        public ConfGeneralViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            handler = hand;
            this.Model = new ConfGeneralModel();
            this.Model.GeneraHtml = true;
            this.Model.ListaEmpresas = new ObservableCollection<SelectListItem>();
            this.Model.Empresa = new SelectListItem();
        }
        public ConfGeneralModel Model { get; set; }
        public void OnProcess(object commandParameter)
        {
            try
            {
                SaveData();
                handler.MensajeOk(res.MensajeExito);
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void SaveData()
        {
            SqliteDAO.pCreaConfiguracion(res.KeyOrdenAutomatico, "" + Model.OrdenAutomatico);
            SqliteDAO.pCreaConfiguracion(res.KeyGeneraGrants, "" + Model.Grant);
            SqliteDAO.pCreaConfiguracion(res.KeyProxy, "" + Model.Proxy);
            SqliteDAO.pCreaConfiguracion(res.KEY_EMPRESA, "" + Model.Empresa.Value);
            //SqliteDAO.pCreaConfiguracion(res.KEY_LLAVEW, "" + Model.LlaveW);
            SqliteDAO.pCreaConfiguracion(res.KEY_VALORW, "" + Model.ValorW);
            handler.ConfGeneralViewModel.Model.Empresa = handler.ConfGeneralViewModel.Model.ListaEmpresas.ToList().Find(x => x.Value.Equals(Model.Empresa.Value));
        }
    }
}

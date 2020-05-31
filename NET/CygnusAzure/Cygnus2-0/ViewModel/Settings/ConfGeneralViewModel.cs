using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Settings
{
    public class ConfGeneralViewModel: ViewModelBase
    {
        private readonly DelegateCommand _process;
        private Handler handler;
        private Boolean ordenAutomatico;
        private Boolean generaHtml;
        private Boolean grant;
        private Boolean entregaPlantilla;
        private Boolean proxy;
        private string rutaSqlplus;
        private ConfGeneralModel confGeneralModel;
        public ICommand Process => _process;
        public ConfGeneralViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            handler = hand;
            confGeneralModel = new ConfGeneralModel(handler);
            this.GeneraHtml = true;
        }

        public Boolean OrdenAutomatico
        {
            get { return ordenAutomatico; }
            set { SetProperty(ref ordenAutomatico, value); }
        }

        public Boolean GeneraHtml
        {
            get { return generaHtml; }
            set { SetProperty(ref generaHtml, value); }
        }

        public Boolean Grant
        {
            get { return grant; }
            set { SetProperty(ref grant, value); }
        }
        public String RutaSqlplus
        {
            get { return rutaSqlplus; }
            set { SetProperty(ref rutaSqlplus, value); }
        }

        public Boolean EntregaPlantilla
        {
            get { return entregaPlantilla; }
            set { SetProperty(ref entregaPlantilla, value); }
        }
        public Boolean Proxy
        {
            get { return proxy; }
            set { SetProperty(ref proxy, value); }
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                /*if(string.IsNullOrEmpty(RutaSqlplus))
                {
                    handler.MensajeError("Ingrese la ruta dónde se encuentra el sqlplus.exe.");
                    return;
                }*/

                this.confGeneralModel.SaveData();
                handler.MensajeOk(res.MensajeExito);
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

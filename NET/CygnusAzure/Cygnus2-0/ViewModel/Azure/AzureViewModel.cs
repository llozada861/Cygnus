using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Azure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Azure
{
    public class AzureViewModel : IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;

        public ICommand Process => _process;

        public AzureViewModel(Handler handler)
        { 
            this.handler = handler;
            _process = new DelegateCommand(OnProcess);
        }
        public AzureModel Model
        {
            get { return handler.Azure; }
            set { handler.Azure = value; }
        }
        public ObservableCollection<AzureModel> ListaAzure
        {
            get { return handler.ListaAzure; }
            set { handler.ListaAzure = value; }
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
            try
            {
                if (string.IsNullOrEmpty(this.Model.Url))
                {
                    handler.MensajeError("Debe ingresar la URL de azure");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Usuario))
                {
                    handler.MensajeError("Debe ingresar el usuario de Azure");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Correo))
                {
                    handler.MensajeError("Debe ingresar el correo empresarial");
                    return;
                }

                if (this.Model.Dias == 0)
                {
                    handler.MensajeError("Debe ingresar el número de días para descarga");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Proyecto))
                {
                    handler.MensajeError("Debe ingresar un proyecto");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Token))
                {
                    handler.MensajeError("Debe ingresar un Token");
                    return;
                }

                handler.CursorWait();
                SqliteDAO.pCreaRegistroAzure(this.Model,handler.ConfGeneralView.Model.Empresa.Value);
                handler.CursorNormal();

                handler.MensajeOk("Proceso terminó con éxito");
                pObtListaAzure();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void pObtListaAzure()
        {
            SqliteDAO.pObtListaAzure(handler);
        }
    }
}

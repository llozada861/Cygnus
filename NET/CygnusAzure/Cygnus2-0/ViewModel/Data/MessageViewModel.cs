using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Data
{
    public class MessageViewModel: IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;
        private Handler handler;
        public MessageViewModel(Handler handler)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);
            this.handler = handler;

            this.Model = new MessageModel(handler);
        }
        public void OnClean(object commandParameter)
        {
            this.Model.Codigo = "";
            this.Model.Causa = "";
            this.Model.Descripcion = "";
            this.Model.Solucion = "";
        }

        public MessageModel Model { get; set; }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if(string.IsNullOrEmpty(this.Model.Descripcion))
                {
                    handler.MensajeError("Debe ingresar una descripción para el mensaje.");
                    return;
                }

                this.Model.Codigo = handler.DAO.pObtCodigoMensaje();
                handler.DAO.pCreaMensaje(this.Model);
                pGenerarArchivo();
            }
            catch(Exception ex)
            {
                handler.MensajeError(res.MensajeNoConexion + "["+ex.Message+"]");
            }
        }

        public void pGenerarArchivo()
        {
            StringBuilder insParam = new StringBuilder();

            insParam.Append(res.PlantillaInsMensaje);
            insParam.Replace(res.Tag_codigo, this.Model.Codigo);
            insParam.Replace(res.Tag_descripcion, this.Model.Descripcion);
            insParam.Replace(res.Tag_causa, this.Model.Causa);
            insParam.Replace(res.Tag_solucion, this.Model.Solucion);

            handler.pGuardaArchivo(res.NombreArchivoInsMensaje + this.Model.Codigo + res.ExtensionSQL, insParam.ToString());
        }
    }
}

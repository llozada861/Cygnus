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
    public class MessageViewModel: ViewModelBase, IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;
        private Handler handler;
        private string descripcion;
        private string causa;
        private string solucion;
        private string codigo;
        public MessageViewModel(Handler Hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);
            handler = Hand;
        }
        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }
        public string Causa
        {
            get { return causa; }
            set { SetProperty(ref causa, value); }
        }
        public string Solucion
        {
            get { return solucion; }
            set { SetProperty(ref solucion, value); }
        }
        public void OnClean(object commandParameter)
        {
            this.Codigo = "";
            this.Causa = "";
            this.Descripcion = "";
            this.Solucion = "";
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if(string.IsNullOrEmpty(this.Descripcion))
                {
                    handler.MensajeError("Debe ingresar una descripción para el mensaje.");
                    return;
                }

                this.Codigo = handler.DAO.pObtCodigoMensaje();
                handler.DAO.pCreaMensaje(this);
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
            insParam.Replace(res.Tag_codigo, this.Codigo);
            insParam.Replace(res.Tag_descripcion, this.Descripcion);
            insParam.Replace(res.Tag_causa, this.Causa);
            insParam.Replace(res.Tag_solucion, this.Solucion);

            handler.pGuardaArchivo(res.NombreArchivoInsMensaje + this.Codigo + res.ExtensionSQL, insParam.ToString());
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Settings
{
    public class UpdateViewModel: ViewModelBase
    {
        private Handler handler;
        private UpdateModel updateModel;
        private string info;
        private readonly DelegateCommand _process;
        public ICommand Process => _process;

        public UpdateViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            handler = hand;
            updateModel = new UpdateModel(handler);

            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            this.Info = "Versión del producto: [" + fieVersionInfo.FileVersion + "]";
        }

        public string Info
        {
            get { return info; }
            set { SetProperty(ref info, value); }
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                updateModel.pActualizaApp();
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

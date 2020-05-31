using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Refresh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Refresh
{
    public class RefreshViewModel : ViewModelBase, IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;
        private Handler handler;
        private RefreshModel model;
        public RefreshViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);
            handler = hand;
            model = new RefreshModel(handler);
            ListaInsertCM = new List<SelectListItem>();
            ListaObjetosBl = new List<SelectListItem>();
            ListaSaUser = new List<SelectListItem>();
            ListaPerson = new List<SelectListItem>();
            ListaUsuarios = new List<SelectListItem>();
            ListaHoja = new List<SelectListItem>();
            ListaRQ = new List<SelectListItem>();
            ListaHH = new List<SelectListItem>();
        }

        public List<SelectListItem> ListaInsertCM { get; set; }
        public List<SelectListItem> ListaObjetosBl { get; set; }
        public List<SelectListItem> ListaSaUser { get; set; }
        public List<SelectListItem> ListaPerson { get; set; }
        public List<SelectListItem> ListaUsuarios { get; set; }
        public List<SelectListItem> ListaHoja { get; set; }
        public List<SelectListItem> ListaRQ { get; set; }
        public List<SelectListItem> ListaHH { get; set; }

        public string ObjetosBl { get; set; }
        public string ObjetosLog { get; set; }
        public string onuSeqRq { get; set; }
        public string onuSeqHH { get; set; }
        public string onuSeqNeg { get; set; }
        public string Objetos { get; set; }

        public void OnClean(object commandParameter)
        {
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                model.process(this);
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

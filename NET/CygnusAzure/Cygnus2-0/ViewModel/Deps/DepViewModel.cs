using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model;
using Cygnus2_0.Model.Aplica;
using Cygnus2_0.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Deps
{
    public class DepViewModel : IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;

        public DepViewModel(Handler handler) 
        { 
            this.handler = handler;
            this.Model = new DepModel();
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            Model.ListaBD = new ObservableCollection<UsuariosPDN>(SqliteDAO.pObtListaBD(handler));
            Model.ListaDependencias = new ObservableCollection<Dependencia>();            
        }

        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public DepModel Model { get; set; }
        public void OnClean(object commandParameter)
        {
            Model.Objeto = "";
            Model.ListaDependencias.Clear();
        }

        public void OnConection(object commandParameter)
        {            
        }

        public void OnProcess(object commandParameter)
        {
            if(string.IsNullOrEmpty(Model.Objeto))
            {
                handler.MensajeError("Debe ingresar un objeto a buscar");
                return;
            }

            if(Model.BdSeleccionada == null)
            {
                handler.MensajeError("Debe ingresar una instancia");
                return;
            }

            try
            {
                handler.CursorWait();
                Model.ListaDependencias = new System.Collections.ObjectModel.ObservableCollection<Dependencia>(handler.DAO.pGenerarDependencias(Model.Objeto, Model.BdSeleccionada));
                handler.CursorNormal();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
    }
}

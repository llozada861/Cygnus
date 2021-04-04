using Cygnus2_0.General;
using Cygnus2_0.Model.Estandar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.ViewModel.Estandar
{
    public class EstandarViewModel
    {
        private Handler handler;
        public EstandarViewModel(Handler handler)
        {
            this.handler = handler;
            this.Model = new EstandarModel();
            pObtenerEstandar();
        }

        public EstandarModel Model { get; set; }

        public void pObtenerEstandar()
        {
            Model.ListaEstandar = handler.DAO.pObtListaEstandar();
        }

        internal void pAdicionarEstandar(string value)
        {
            handler.DAO.pAdicionarEstandar(Model.Estandar.Value.ToUpper(), Model.Estandar.Text, value);
            pObtenerEstandar();
        }

        internal void pModificarEstandar(string value)
        {
            handler.DAO.pModificarEstandar(Model.Estandar.Value, Model.Estandar.Text, value);
            pObtenerEstandar();
        }

        internal void pEliminarEstandar()
        {
            handler.DAO.pEliminarEstandar(Model.Estandar.Value);
            pObtenerEstandar();
        }
    }
}

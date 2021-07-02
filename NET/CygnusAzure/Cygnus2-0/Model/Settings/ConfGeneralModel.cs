using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Settings
{
    public class ConfGeneralModel: ViewModelBase
    {
        private Boolean ordenAutomatico;
        private Boolean generaHtml;
        private Boolean grant;
        private Boolean entregaPlantilla;
        private Boolean proxy;
        private string rutaSqlplus;
        private string valorW;
        private string llaveW;
        private SelectListItem empresa;
        private ObservableCollection<SelectListItem> listaEmpresa;
        public ConfGeneralModel()
        {
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
        public SelectListItem Empresa
        {
            get { return empresa; }
            set { SetProperty(ref empresa, value); }
        }
        public ObservableCollection<SelectListItem> ListaEmpresas
        {
            get { return listaEmpresa; }
            set { SetProperty(ref listaEmpresa, value); }
        }

        public String LlaveW
        {
            get { return llaveW; }
            set { SetProperty(ref llaveW, value); }
        }

        public String ValorW
        {
            get { return valorW; }
            set { SetProperty(ref valorW, value); }
        }
    }
}

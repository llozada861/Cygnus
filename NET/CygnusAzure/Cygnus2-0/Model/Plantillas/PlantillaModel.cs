using Cygnus2_0.Interface;
using Cygnus2_0.Model.Html;
using Cygnus2_0.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Plantillas
{
    public class PlantillaModel : ViewModelBase
    {
        private ObservableCollection<PlantillasHTMLModel> listaPlantilla;
        private PlantillasHTMLModel seleccionado;
        public PlantillaModel() { }

        public PlantillasHTMLModel PlantillaSeleccionada 
        {
            get { return seleccionado; }
            set
            {
                SetProperty(ref seleccionado, value);
            }
        }
        public ObservableCollection<PlantillasHTMLModel> ListaPlantilla
        {
            get { return listaPlantilla; }
            set
            {
                SetProperty(ref listaPlantilla, value);
            }
        }
    }
}

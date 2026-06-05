using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model;
using Cygnus2_0.Model.Html;
using Cygnus2_0.Model.Plantillas;
using Cygnus2_0.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Plantillas
{
    public class PlantillaViewModel : IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;

        public PlantillaViewModel(Handler handler)
        {
            this.handler = handler;
            this.Model = new PlantillaModel();
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            Model.ListaPlantilla = new ObservableCollection<PlantillasHTMLModel>(handler.ListaPlantillas.Where(x=>x.Tipo == 1));
        }
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public PlantillaModel Model { get; set; }
        public void OnClean(object commandParameter)
        {
            Model.ListaPlantilla.Clear();
            Model.ListaPlantilla = new ObservableCollection<PlantillasHTMLModel>(handler.ListaPlantillas.Where(x => x.Tipo == 1));
            Model.PlantillaSeleccionada = new PlantillasHTMLModel();
        }

        public void OnConection(object commandParameter)
        {
            
        }

        public void OnProcess(object commandParameter)
        {
            bool boFaltaParam = false;

            try
            {
                if(Model.PlantillaSeleccionada == null)
                {
                    handler.MensajeError("Debe seleccionar una plantilla");
                    return;
                }

                foreach (var item in Model.PlantillaSeleccionada.ListaDetalleIn)
                {
                    if(string.IsNullOrEmpty(item.Valor))
                    {
                        boFaltaParam = true;
                        break;
                    }
                }

                if (boFaltaParam) 
                {
                    handler.MensajeError("Todos los parámetros deben tener valor!");
                    return;
                }

                handler.CursorWait();
                MarkDAO.pGeneraPlantilla(handler, Model.PlantillaSeleccionada);
                handler.CursorNormal();

                foreach (var item in Model.PlantillaSeleccionada.ListaDetalleOut)
                {
                    if(!string.IsNullOrEmpty(item.Valor))
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = item.Objeto.ToLower().Trim()+ ".sql";

                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            System.IO.File.WriteAllText(saveFileDialog.FileName, item.Valor, Encoding.Default);
                    }
                }
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
    }
}

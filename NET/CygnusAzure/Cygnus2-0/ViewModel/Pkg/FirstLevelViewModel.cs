using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Html;
using Cygnus2_0.Model.Package;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Pkg
{
    public class FirstLevelViewModel : IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;
        private Handler handler;

        public FirstLevelViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);
            handler = hand;

            this.Model = new pkgModel();

            this.Model.ListaUsuarios = handler.pObtlistaUsuarios();
            this.Model.Usuario = new SelectListItem();
        }

        public pkgModel Model { get; set; }

        public void OnClean(object commandParameter)
        {
            this.Model.Caso = "";
            this.Model.Tabla = "";
            this.Model.Usuario = new SelectListItem();
            this.Model.ListaUsuarios = handler.pObtlistaUsuarios();
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                pGeneraPktbl();
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
        
        internal void pGeneraPktbl()
        {
            handler.CursorWait();

            string pktbl = handler.DAO.pGeneraPktbl(this.Model.Tabla, this.Model.Usuario,this.Model.Owner, this.Model.Caso,handler);

            PlantillasHTMLModel plantillaPktbl = handler.ListaPlantillas.Where(x => x.Nombre.Equals(res.KEY_PKTBL)).FirstOrDefault();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = plantillaPktbl.NombreArchivo + this.Model.Tabla.ToLower().Trim() + ".sql";

            handler.CursorNormal();

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                File.WriteAllText(saveFileDialog.FileName, pktbl, Encoding.Default);
        }
    }
}

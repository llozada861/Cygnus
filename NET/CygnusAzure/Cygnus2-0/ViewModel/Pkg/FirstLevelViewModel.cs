using Cygnus2_0.General;
using Cygnus2_0.Interface;
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

namespace Cygnus2_0.ViewModel.Pkg
{
    public class FirstLevelViewModel : ViewModelBase, IViews
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;
        private Handler handler;
        private string tabla;
        private string caso;
        private SelectListItem usuario;
        private ObservableCollection<SelectListItem> listaUsuarios;
        public FirstLevelViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);
            handler = hand;

            this.ListaUsuarios = handler.ListaUsuarios;
            this.Usuario = new SelectListItem();
        }
        public string Tabla
        {
            get { return tabla; }
            set { SetProperty(ref tabla, value); }
        }
        public string Caso
        {
            get { return caso; }
            set { SetProperty(ref caso, value); }
        }
        public SelectListItem Usuario
        {
            get { return usuario; }
            set { SetProperty(ref usuario, value); }
        }
        public ObservableCollection<SelectListItem> ListaUsuarios
        {
            get { return listaUsuarios; }
            set { SetProperty(ref listaUsuarios, value); }
        }

        public void OnClean(object commandParameter)
        {
            this.Caso = "";
            this.Tabla = "";
            this.Usuario = new SelectListItem();
            this.ListaUsuarios = handler.ListaUsuarios;
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

            OracleClob pktbl = handler.DAO.pGeneraPktbl(this.Tabla, this.Usuario, this.Caso);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "pktbl" + this.Tabla.ToLower().Trim() + ".sql";

            handler.CursorNormal();

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                File.WriteAllText(saveFileDialog.FileName, pktbl.Value, Encoding.Default);


            //this.MensajeOk(pktbl.Value.ToString());

            /*using (StreamWriter str = new StreamWriter("D:\\prueba.sql"))
            {
                str.Write(pktbl.Value);
            }*/
        }
    }
}

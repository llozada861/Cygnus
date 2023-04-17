using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Cygnus2_0.DAO;
using Cygnus2_0.Model.User;
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Model.Repository;
using Oracle.ManagedDataAccess.Types;
using System.Windows.Forms;
using System.IO;

namespace Cygnus2_0.ViewModel.Objects
{
    public class BlockViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _descargar;
        private readonly DelegateCommand _search;
        private string objetosBloqueados;
        private string asunto;
        private string body;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Descargar => _descargar;
        public ICommand Search => _search;
        public BlockViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _descargar = new DelegateCommand(onDownload);
            _search = new DelegateCommand(OnSearch);

            handler = hand;
            Model = new BlockModel(handler, this);

            Model.ListaArchivosBloqueo = new ObservableCollection<Archivo>();
            Model.ListaArchivosEncontrados = new ObservableCollection<Archivo>();
            Model.ListaBD = new ObservableCollection<UsuariosPDN>(SqliteDAO.pObtListaBD());
        }

        public BlockModel Model { get; set; }
        public UsuariosPDN BdSeleccionada { get; set; }
        public Archivo ObjetoSeleccionado { get; set; }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Model.Codigo))
                {
                    handler.MensajeError("Ingrese el número de la WO para trabajar sobre los objetos.");
                    return;
                }

                if (Model.ListaArchivosBloqueo.Count == 0)
                {
                    handler.MensajeError("No hay objetos para informar.");
                    return;
                }

                pBloquearObjetos();

                handler.MensajeOk("Proceso terminado.");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void onDownload(object commandParameter)
        {
            try
            {
                if (this.ObjetoSeleccionado == null)
                    return;

                handler.CursorWait();

                OracleClob pktbl = handler.DAO.pGeneraFuente(this.ObjetoSeleccionado.FileName, this.ObjetoSeleccionado.Owner,this.BdSeleccionada);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = this.BdSeleccionada.BaseDatos+"_"+this.ObjetoSeleccionado.FileName.ToLower() + ".sql";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    File.WriteAllText(saveFileDialog.FileName, pktbl.Value, Encoding.Default);

                handler.CursorNormal();

                handler.ConexionOracle.ConexionOracleProd.Close();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
                handler.ConexionOracle.ConexionOracleProd.Close();
            }
        }
        public void OnClean(object commandParameter)
        {
            try
            {
                Model.ListaArchivosEncontrados.Clear();
                Model.Objeto = "";
                Model.ListaBD.Clear();
                Model.ListaBD = new ObservableCollection<UsuariosPDN>(SqliteDAO.pObtListaBD());
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void OnSearch(object commandParameter)
        {
            try
            {
                handler.CursorWait();
                handler.DAO.pObtConsultaObjetos(Model.Objeto.Trim(), this, BdSeleccionada);
                pRefrescaConteo();
                handler.CursorNormal();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        internal void pRefrescaConteo()
        {
            Model.CantidadObjetos = Model.ListaArchivosBloqueo.Count().ToString();
        }

        internal void pAdicionaObjeto(Archivo objeto)
        {
            if (!Model.ListaArchivosBloqueo.ToList().Exists(x => (x.Owner.Equals(objeto.Owner) && x.FileName.Equals(objeto.FileName))))
            {
                Model.ListaArchivosBloqueo.Add(objeto);
            }

            pRefrescaConteo();
        }

        internal void pBloquearObjetos()
        {
            objetosBloqueados = "";

            foreach (Archivo archivo in Model.ListaArchivosBloqueo)
            {
                if (Model.ListaArchivosBloqueo.IndexOf(archivo) == Model.ListaArchivosBloqueo.Count - 1)
                {
                    objetosBloqueados = objetosBloqueados + archivo.FileName;
                }
                else
                {
                    objetosBloqueados = objetosBloqueados + archivo.FileName + ", ";
                }
            }

            if (!string.IsNullOrEmpty(objetosBloqueados))
            {
                if (handler.MensajeConfirmacion("Desea enviar la notificación por correo?") == "Y")
                {
                    asunto = "Notificación para modificar los objetos [" + objetosBloqueados + "]";
                    body = "Buen día, <br><br> Se informa que se va a trabajar sobre los objetos [" + objetosBloqueados + "] " +
                           " con la WO [" + Model.Codigo + "]. <br><br> Correo enviado a través de Cygnus.";

                    handler.sendEMailThroughOUTLOOK(asunto, body);
                }
            }
        }

        public void OnConection(object commandParameter)
        {
            throw new NotImplementedException();
        }
    }
}

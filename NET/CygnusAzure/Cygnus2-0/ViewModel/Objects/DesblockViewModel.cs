using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Objects
{
    public class DesblockViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conectar;
        private ObservableCollection<Archivo> listaArchivosBloqueo;
        private ObservableCollection<Archivo> listaArchivosDesbloq;
        private string cantidadObjetos;
        private string estadoConn;
        private DesblockModel model;
        private DateTime fecha;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conectar;
        public DesblockViewModel(Handler hand)
        {            
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conectar = new DelegateCommand(OnConection);

            handler = hand;
            model = new DesblockModel(handler, this);

            ListaArchivosBloqueo = new ObservableCollection<Archivo>();
            ListaArchivosDesbloq = new ObservableCollection<Archivo>();
            this.CantidadObjetos = "0";
            this.Fecha = DateTime.Now.AddDays(-1);

            try
            {
                if(!handler.ConnViewModel.Usuario.ToUpper().Equals(res.UsuarioSQLDefault) && handler.ConexionOracle.ConexionOracleSQL != null)
                    handler.DAO.pObtObjetosBloqueados(this);
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public string CantidadObjetos
        {
            get { return cantidadObjetos; }
            set { SetProperty(ref cantidadObjetos, value); }
        }
        public string EstadoConn
        {
            get { return handler.EstadoConn; }
            set { SetProperty(ref estadoConn, handler.EstadoConn); }
        }
        public DateTime Fecha
        {
            get { return fecha; }
            set { SetProperty(ref fecha, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosBloqueo
        {
            get { return listaArchivosBloqueo; }
            set { SetProperty(ref listaArchivosBloqueo, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosDesbloq
        {
            get { return listaArchivosDesbloq; }
            set { SetProperty(ref listaArchivosDesbloq, value); }
        }
        public void OnProcess(object commandParameter)
        {
            try
            {
                if (this.ListaArchivosDesbloq.Count == 0)
                {
                    handler.MensajeError("Para desbloquear objetos debe dar doble click al objeto bloqueado para que sea adicionado a la lista de Objetos a Desbloquear.");
                }

                model.pDesbloqueaObjetos();
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnClean(object commandParameter)
        {
            try
            {
                if(!handler.ConnViewModel.Usuario.ToUpper().Equals(res.UsuarioSQLDefault) && handler.ConexionOracle.ConexionOracleSQL != null)
                    model.pLimpiar();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pAdicionaObjeto(Archivo objeto)
        {
            model.pAdicionaObjeto(objeto);
        }
        public void pRefrescaConteo()
        {
            model.pRefrescaConteo();
        }

        public void OnConection(object commandParameter)
        {
            try
            {
                //se intenta realizar la conexión con la base de datos
                handler.pRealizaConexion();
                //this.EstadoConn = "0";
            }
            catch (Exception ex)
            {
                //this.EstadoConn = "1";
                handler.MensajeError(ex.Message);
            }
        }

        public void OnUpdate(Archivo objeto)
        {
            try
            {
                if(this.Fecha <= DateTime.Now)
                {
                    handler.MensajeError("La fecha debe ser mayor a la fecha actual");
                    return;
                }

                handler.DAO.pActualizaFecha(objeto,this.Fecha);
                handler.MensajeOk("La fecha estimada de liberación del objeto ["+objeto.FileName+"] se ha cambiado con éxito.");
                this.OnClean("");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
    }
}

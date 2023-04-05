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

namespace Cygnus2_0.ViewModel.Objects
{
    public class BlockViewModel: ViewModelBase, IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _conection;
        private readonly DelegateCommand _search;
        private ObservableCollection<Archivo> listaArchivosBloqueo;
        private ObservableCollection<Archivo> listaArchivosEncontrados;
        private string cantidadObjetos;
        private string estadoConn;
        private string codigo;
        private string objeto;
        private DateTime fecha;
        private BlockModel model;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand Conectar => _conection;
        public ICommand Search => _search;
        public BlockViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _conection = new DelegateCommand(OnConection);
            _search = new DelegateCommand(OnSearch);

            handler = hand;
            model = new BlockModel(handler, this);

            ListaArchivosBloqueo = new ObservableCollection<Archivo>();
            listaArchivosEncontrados = new ObservableCollection<Archivo>();
            this.CantidadObjetos = "0";
            this.Fecha = DateTime.Now;
        }

        public string EstadoConn
        {
            get { return handler.EstadoConn; }
            set { SetProperty(ref estadoConn, handler.EstadoConn); }
        }
        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }
        public string Objeto
        {
            get { return objeto; }
            set { SetProperty(ref objeto, value); }
        }
        public DateTime Fecha
        {
            get { return fecha; }
            set { SetProperty(ref fecha, value); }
        }
        public string CantidadObjetos
        {
            get { return cantidadObjetos; }
            set { SetProperty(ref cantidadObjetos, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosBloqueo
        {
            get { return listaArchivosBloqueo; }
            set { SetProperty(ref listaArchivosBloqueo, value); }
        }
        public ObservableCollection<Archivo> ListaArchivosEncontrados
        {
            get { return listaArchivosEncontrados; }
            set { SetProperty(ref listaArchivosEncontrados, value); }
        }
        internal void pAdicionaObjeto(Archivo objeto)
        {
            model.pAdicionaObjeto(objeto);
        }
        public void OnProcess(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Codigo))
                {
                    handler.MensajeError("Ingrese el número de la WO para trabajar sobre los objetos.");
                    return;
                }

                if (this.ListaArchivosBloqueo.Count == 0)
                {
                    handler.MensajeError("No hay objetos para informar.");
                    return;
                }

                model.pBloquearObjetos();

                handler.MensajeOk("Proceso terminado.");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnConection(object commandParameter)
        {
            try
            {
                //se intenta realizar la conexión con la base de datos
                handler.pRealizaConexion();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnClean(object commandParameter)
        {
            try
            { 
                model.OnClean();
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
                model.OnSearch();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        internal void pRefrescaConteo()
        {
            model.pRefrescaConteo();
        }
    }
}

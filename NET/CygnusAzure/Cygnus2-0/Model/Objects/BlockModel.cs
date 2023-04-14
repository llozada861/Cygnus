using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Model.User;
using Cygnus2_0.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Objects
{
    public class BlockModel: ViewModelBase
    {
        private Handler handler;
        private BlockViewModel view;
        private string cantidadObjetos;
        private string estadoConn;
        private string codigo;
        private string objeto;
        private DateTime fecha;
        private ObservableCollection<Archivo> listaArchivosBloqueo;
        private ObservableCollection<Archivo> listaArchivosEncontrados;
        ObservableCollection<UsuariosPDN> listaBD;

        public BlockModel(Handler hand, BlockViewModel view)
        {
            handler = hand;
            this.view = view;
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
        public ObservableCollection<UsuariosPDN> ListaBD
        {
            get { return listaBD; }
            set
            {
                SetProperty(ref listaBD, value);
            }
        }
    }
}

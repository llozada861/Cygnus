using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.ViewModel.Settings;
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
    public class ConexionModel: ViewModelBase
    {
        private string _usuario;
        private string _pass;
        private string _basedatos;
        private string _puerto;
        private string _servidor;
        private string _usuariocompila;
        private string _passcompila;
        private string correo;
        private string usuarioAzure;
        private string urlAzure;
        ObservableCollection<SelectListItem> listaAreaAzure;
        ObservableCollection<SelectListItem> listaConexiones;
        private SelectListItem conexion;

        Handler handler;
        public ConexionModel(Handler hand)
        {
            handler = hand;
        }

        public string Usuario
        {
            get { return _usuario; }
            set { SetProperty(ref _usuario, value); }
        }

        public string Pass
        {
            get { return _pass; }
            set { SetProperty(ref _pass, value); }
        }
        public string BaseDatos
        {
            get { return _basedatos; }
            set { SetProperty(ref _basedatos, value); }
        }
        public string Puerto
        {
            get { return _puerto; }
            set { SetProperty(ref _puerto, value); }
        }
        public string Servidor
        {
            get { return _servidor; }
            set { SetProperty(ref _servidor, value); }
        }
        public string UsuarioCompila
        {
            get { return _usuariocompila; }
            set { SetProperty(ref _usuariocompila, value); }
        }
        public string PassCompila
        {
            get { return _passcompila; }
            set { SetProperty(ref _passcompila, value); }
        }
        public string BdCompila { set; get; }

        public string Correo
        {
            get { return correo; }
            set { SetProperty(ref correo, value); }
        }
        public string UsuarioAzure
        {
            get { return usuarioAzure; }
            set { SetProperty(ref usuarioAzure, value); }
        }
        public string UrlAzure
        {
            get { return urlAzure; }
            set { SetProperty(ref urlAzure, value); }
        }

        public ObservableCollection<SelectListItem> ListaAreaAzure
        {
            get { return listaAreaAzure; }
            set { SetProperty(ref listaAreaAzure, value); }
        }
        public ObservableCollection<SelectListItem> ListaConexiones
        {
            get { return listaConexiones; }
            set { SetProperty(ref listaConexiones, value); }
        }
        public SelectListItem Conexion
        {
            get { return conexion; }
            set { SetProperty(ref conexion, value); }
        }
    }
}

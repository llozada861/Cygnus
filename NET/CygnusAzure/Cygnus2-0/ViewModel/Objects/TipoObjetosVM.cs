using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Objects
{
    public class TipoObjetosVM : ViewModelBase, IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _eliminarPadre;
        private readonly DelegateCommand _agregar;
        private readonly DelegateCommand _eliminarHijo;
        private readonly DelegateCommand _eliminarHijo2;
        private readonly DelegateCommand _eliminarHijo3;
        ObservableCollection<RutaObjetos> listaRutas;
        ObservableCollection<HeadModel> listaEncabezadoObjetos;
        ObservableCollection<PermisosObjeto> listaPermisosObjeto;

        public TipoObjetosVM(Handler handler)
        {
            this.handler = handler;
            _eliminarPadre = new DelegateCommand(pEliminarPadre);
            _process = new DelegateCommand(OnProcess);
            _agregar = new DelegateCommand(Agregar);
            _eliminarHijo = new DelegateCommand(OnDeleteHijo);
            _eliminarHijo2 = new DelegateCommand(OnDeleteHijo2);
            _eliminarHijo3 = new DelegateCommand(OnDeleteHijo3);
            ListaRutas = new ObservableCollection<RutaObjetos>();
        }
        public ICommand EliminarPadre => _eliminarPadre;
        public ICommand Add => _agregar;
        public ICommand Process => _process;
        public ICommand ElminarHijo => _eliminarHijo;
        public ICommand ElminarHijo2 => _eliminarHijo2;
        public ICommand ElminarHijo3 => _eliminarHijo3;

        public Handler Handler
        {
            get { return handler; }
            set { SetProperty(ref handler, value); }
        }
        public ObservableCollection<RutaObjetos> ListaRutas
        {
            get { return listaRutas; }
            set { SetProperty(ref listaRutas, value); }
        }
        public ObservableCollection<HeadModel> ListaEncabezadoObjetos
        {
            get { return listaEncabezadoObjetos; }
            set 
            {
                SetProperty(ref listaEncabezadoObjetos, value);

                foreach (HeadModel objeto in listaEncabezadoObjetos)
                {
                    objeto.ListaTipoFin = new ObservableCollection<SelectListItem>(handler.ListaTipoFin);
                }
            }
        }
        public ObservableCollection<PermisosObjeto> ListaPermisosObjeto
        {
            get 
            {
                return listaPermisosObjeto; 
            }
            set 
            { 
                SetProperty(ref listaPermisosObjeto, value); 

                foreach(PermisosObjeto objeto in listaPermisosObjeto)
                {
                    objeto.ListaPermisos = handler.ListaPermisos;
                    objeto.ListaUsuarios = handler.ListaUsGrants;
                }    
            }
        }

        public Boolean TabPrincipal { get; set; }
        public Boolean TabHijo { get; set; }
        public Boolean TabHijo2 { get; set; }
        public Boolean TabHijo3 { get; set; }
        public TipoObjetos PrincipalSeleccionado { get; set; }
        public RutaObjetos HijoSeleccionado { get; set; }
        public HeadModel Hijo2Seleccionado { get; set; }
        public PermisosObjeto Hijo3Seleccionado { get; set; }
        public void OnClean(object commandParameter)
        {
            throw new NotImplementedException();
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if (this.TabPrincipal)
                {
                    foreach (TipoObjetos objeto in handler.ListaTiposObjetos)
                    {
                        if (string.IsNullOrEmpty(objeto.Descripcion))
                        {
                            handler.MensajeError("Ingrese una descripción");
                            return;
                        }

                        if (objeto.Prioridad == null)
                        {
                            handler.MensajeError("Ingrese una prioridad");
                            return;
                        }

                        if (objeto.Codigo != null)
                            SqliteDAO.pActualizaTipoObjeto(objeto);
                        else
                            SqliteDAO.pGuardarTipoObjeto(objeto);
                    }
                }

                if (this.TabHijo)
                {
                    foreach (RutaObjetos objeto in ListaRutas)
                    {
                        if (string.IsNullOrEmpty(objeto.Ruta))
                        {
                            handler.MensajeError("Ingrese una ruta");
                            return;
                        }

                        if (objeto.Codigo != null)
                            SqliteDAO.pActualizaRuta(objeto);
                        else
                            SqliteDAO.pAdicionaRuta(objeto);
                    }
                }

                if (this.TabHijo2)
                {
                    foreach (HeadModel objeto in ListaEncabezadoObjetos)
                    {
                        if (string.IsNullOrEmpty(objeto.Descripcion))
                        {
                            handler.MensajeError("Ingrese una descripción");
                            return;
                        }

                        if (objeto.Codigo != null)
                            SqliteDAO.pActualizaEncabezado(objeto);
                        else
                            SqliteDAO.pGuardarEncabezado(objeto);
                    }
                }

                if (this.TabHijo3)
                {
                    foreach (PermisosObjeto objeto in ListaPermisosObjeto)
                    {
                        if (objeto.Permiso == null)
                        {
                            handler.MensajeError("Ingrese una Permiso");
                            return;
                        }

                        if (objeto.Codigo != null)
                            SqliteDAO.pActualizaObjeto(objeto);
                        else
                            SqliteDAO.pAdicionaPermiso(objeto);
                    }
                }

                SqliteDAO.pListaRutas(handler);
                SqliteDAO.pListaEncabezadoObjetos(handler);
                SqliteDAO.pListaPermisosObjetos(handler);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void Agregar(object commandParameter)
        {
            try
            {
                if (this.TabPrincipal)
                {
                    TipoObjetos objeto = new TipoObjetos();
                    handler.ListaTiposObjetos.Add(objeto);
                }

                if (this.TabHijo)
                {
                    if (PrincipalSeleccionado == null)
                    {
                        handler.MensajeError("Para agregar una Ruta debe seleccionar un Tipo de Objeto");
                        return;
                    }

                    if (PrincipalSeleccionado.Codigo == null)
                    {
                        handler.MensajeError("Guarde el Tipo de Objeto");
                        return;
                    }

                    RutaObjetos objeto2 = new RutaObjetos();
                    objeto2.Empresa = Convert.ToInt32(handler.ConfGeneralView.Model.Empresa.Codigo);
                    objeto2.TipoObjeto = PrincipalSeleccionado.Codigo;
                    ListaRutas.Add(objeto2);
                }

                if (this.TabHijo2)
                {
                    if (PrincipalSeleccionado == null)
                    {
                        handler.MensajeError("Para agregar un Encabezado debe seleccionar un Tipo de Objeto");
                        return;
                    }

                    if (PrincipalSeleccionado.Codigo == null)
                    {
                        handler.MensajeError("Guarde el Tipo de Objeto");
                        return;
                    }

                    HeadModel objeto2 = new HeadModel();
                    objeto2.Tipo = PrincipalSeleccionado.Codigo;
                    objeto2.ListaTipoFin = new ObservableCollection<SelectListItem>(handler.ListaTipoFin);
                    ListaEncabezadoObjetos.Add(objeto2);
                }

                if (this.TabHijo3)
                {
                    if (PrincipalSeleccionado == null)
                    {
                        handler.MensajeError("Para agregar un Permiso debe seleccionar un Tipo de Objeto");
                        return;
                    }

                    if (PrincipalSeleccionado.Codigo == null)
                    {
                        handler.MensajeError("Guarde el Tipo de Objeto");
                        return;
                    }

                    PermisosObjeto objeto2 = new PermisosObjeto();
                    objeto2.Empresa = Convert.ToInt32(handler.ConfGeneralView.Model.Empresa.Codigo);
                    objeto2.TipoObjeto = PrincipalSeleccionado.Codigo;
                    objeto2.ListaUsuarios = handler.ListaUsGrants;
                    objeto2.ListaPermisos = handler.ListaPermisos;
                    ListaPermisosObjeto.Add(objeto2);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pEliminarPadre(object commandParameter)
        {
            try
            {
                if (PrincipalSeleccionado != null && PrincipalSeleccionado.Codigo == null)
                {
                    handler.ListaTiposObjetos.Remove(PrincipalSeleccionado);
                }
                else if (PrincipalSeleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar el tipo de objeto [" + PrincipalSeleccionado.Descripcion + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaTipoObjeto(PrincipalSeleccionado);
                    SqliteDAO.pListaTiposObjetos(handler);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnDeleteHijo(object commandParameter)
        {
            try
            {
                if (HijoSeleccionado != null && HijoSeleccionado.Codigo == null)
                {
                    handler.ListaRutas.Remove(HijoSeleccionado);
                    ListaRutas.Remove(HijoSeleccionado);
                }
                else if (HijoSeleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar la ruta [" + HijoSeleccionado.Ruta + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaRuta(HijoSeleccionado);
                    SqliteDAO.pListaRutas(handler);
                    ListaRutas.Remove(HijoSeleccionado);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnDeleteHijo2(object commandParameter)
        {
            try
            {
                if (Hijo2Seleccionado != null && Hijo2Seleccionado.Codigo == null)
                {
                    handler.ListaEncabezadoObjetos.Remove(Hijo2Seleccionado);
                    ListaEncabezadoObjetos.Remove(Hijo2Seleccionado);
                }
                else if (Hijo2Seleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar el encabezado [" + Hijo2Seleccionado.Descripcion + "] ?") == "Y")
                {
                    SqliteDAO.pEliminaEncabezado(Hijo2Seleccionado);
                    SqliteDAO.pListaEncabezadoObjetos(handler);
                    ListaEncabezadoObjetos.Remove(Hijo2Seleccionado);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnDeleteHijo3(object commandParameter)
        {
            try
            {
                if (Hijo3Seleccionado != null && Hijo3Seleccionado.Codigo == null)
                {
                    handler.ListaPermisosObjeto.Remove(Hijo3Seleccionado);
                    ListaPermisosObjeto.Remove(Hijo3Seleccionado);
                }
                else if (Hijo3Seleccionado != null && handler.MensajeConfirmacion("Seguro que desea eliminar el permiso?") == "Y")
                {
                    SqliteDAO.pEliminaObjeto(Hijo3Seleccionado);
                    SqliteDAO.pListaPermisosObjetos(handler);
                    ListaPermisosObjeto.Remove(Hijo3Seleccionado);
                }
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void OnConection(object commandParameter)
        {
            throw new NotImplementedException();
        }
    }
}

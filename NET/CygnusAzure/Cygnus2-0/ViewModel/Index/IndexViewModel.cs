using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Index;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;
using System.Collections.ObjectModel;
using System.Drawing;
using Cygnus2_0.DAO;
using Cygnus2_0.Model.Azure;
using Cygnus2_0.Model.Empresa;

namespace Cygnus2_0.ViewModel.Index
{
    public class IndexViewModel: ViewModelBase
    {
        private Handler handler;
        private string path;
        private string sbLine;
        private string archivoBD;
        private string file;
        private Char delimiter = ';';
        private String[] substrings;

        public IndexViewModel(Handler hand)
        {
            handler = hand;

            try
            {
                pBorrarListas();
            }
            catch (Exception ex)
            {
            }

            //se generan los archivos de configuración
            pGeneraBaseDatos();

            //Carga las listas en el manejador
            pCargarListasArchivo();
        }

        public void pGeneraBaseDatos()
        {
            string path = handler.RutaBaseDatos;
            string nombre;
            byte[] myFile;
            string pathImg = Path.Combine(Environment.CurrentDirectory, "img");

            // Determine whether the directory exists.
            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(pathImg))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(pathImg);
            }

            if (!Directory.Exists(handler.PathTempAplica))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(handler.PathTempAplica);
            }
            else
            {
                handler.pDropFiles(handler.PathTempAplica);
            }

            nombre = res.NombreArchivoRutaActualiza;
            myFile = res.RutaActualiza;
            handler.pCreaArchivoBD(path, nombre, myFile);

            //se crea la base de datos si no existe
            /*nombre = "Cygnus.db";
            myFile = res.Cygnus;
            handler.pCreaArchivoBD(Environment.CurrentDirectory, nombre, myFile);*/

            //se crea la base de datos si no existe
            nombre = "ayudaAzure.png";
            myFile = ImageToByte(res.ayudaAzure);
            handler.pCreaArchivoBD(pathImg, nombre, myFile);

            //se crea la base de datos si no existe
            nombre = "Fullname.png";
            myFile = ImageToByte(res.Fullname);
            handler.pCreaArchivoBD(pathImg, nombre, myFile);
        }

        public void pCargarListasArchivo()
        {
            path = Environment.CurrentDirectory;

            ListaEmpresas();
            ListaConfiguracion();
            //Carga la configuración
            pCargarConfiguracion();
            ListaAzure();
            ListaEncabezadoObjetos();
            ListaTiposObjetos();
            ListaUsGrants();
            ListaPalabrasReservadas();
            ListaUsuarios();
            DatosBd();
            DatosRutaActualizacion();
            DocumentacionHtml();
            ListaRutas();
            ListaHTML();
        }

        public void pBorrarListas()
        {
            try
            {
                handler.ListaEncabezadoObjetos.Clear();
                handler.ListaTiposObjetos.Clear();
                handler.ListaUsGrants.Clear();
                handler.ListaPalabrasReservadas.Clear();
                handler.ListaConfiguracion.Clear();
                handler.ListaUsuarios.Clear();
                handler.ListaDocHtml.Clear();
                handler.ListaRutas.Clear();
                handler.ListaHTML.Clear();
                handler.HtmlEspecificacion.Clear();
                handler.HtmlMetodo.Clear();
                handler.HtmlMetodoParam.Clear();
                handler.HtmlMetodoReturn.Clear();
                handler.HtmlScript.Clear();
            }
            catch (Exception ex)
            { }
        }
        public void ListaEmpresas()
        {
            try
            { 
                sbLine = null;
                archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoEncabezadoObj);
                file = Path.Combine(path, archivoBD);

                handler.ConfGeneralView.Model.ListaEmpresas = new ObservableCollection<EmpresaModel>();

                SqliteDAO.pListaEmpresas(handler);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void ListaEncabezadoObjetos()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoEncabezadoObj);
            file = Path.Combine(path, archivoBD);

            handler.ListaEncabezadoObjetos = new ObservableCollection<SelectListItem>();

            SqliteDAO.pListaEncabezadoObjetos(handler);
        }
        public void ListaTiposObjetos()
        {
            try
            {
                sbLine = null;
                archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoTiposObj);
                file = Path.Combine(path, archivoBD);

                handler.ListaTiposObjetos = new ObservableCollection<SelectListItem>();
                SqliteDAO.pListaTiposObjetos(handler);
            }
            catch (Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public void ListaUsGrants()
        {
            try
            {
                sbLine = null;
                archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoUsGrants);
                file = Path.Combine(path, archivoBD);

                handler.ListaUsGrants = new ObservableCollection<SelectListItem>();
                SqliteDAO.pListaUsGrants(handler);
            }
            catch(Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public void ListaUsuarios()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoUsuariosBD);
            file = Path.Combine(path, archivoBD);

            try
            {
                handler.ListaUsuarios = new ObservableCollection<SelectListItem>();
                SqliteDAO.pListaUsuarios(handler);
            }
            catch (Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public void ListaConfiguracion()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoConfiguracion);
            file = Path.Combine(path, archivoBD);

            handler.ListaConfiguracion = new List<SelectListItem>();

            SqliteDAO.pCargaConfiguracion(handler);
        }

        public void DatosBd()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoConexion);
            file = Path.Combine(path, archivoBD);

            try
            {
                SqliteDAO.pDatosBd(handler,null);
            }
            catch (Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public void RutaSqlplus()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoRutaSqlplus);
            file = Path.Combine(path, archivoBD);

            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                sbLine = streamReader.ReadLine();
                handler.ConfGeneralView.Model.RutaSqlplus = sbLine;
            }
        }

        public void ListaPalabrasReservadas()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoPalReser);
            file = Path.Combine(path, archivoBD);

            handler.ListaPalabrasReservadas = new ObservableCollection<SelectListItem>();
            SqliteDAO.pListaPalabrasReservadas(handler);
        }
        public void ListaChequeo()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoChequeo);
            file = Path.Combine(path, archivoBD);

            handler.ListaChequeo = new List<SelectListItem>();

            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                sbLine = streamReader.ReadLine();

                while (sbLine != null)
                {
                    substrings = sbLine.Split(delimiter);

                    handler.ListaChequeo.Add
                    (
                        new SelectListItem
                        {
                            Text = substrings[0].Trim(),
                            Value = substrings[1].Trim()
                        }
                    );

                    sbLine = streamReader.ReadLine();
                }
            }
        }

        public void ListaRutas()
        {
            try
            {
                handler.ListaRutas = new ObservableCollection<SelectListItem>();
                SqliteDAO.pListaRutas(handler);
            }
            catch(Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public void ListaAzure()
        {
            try
            {
                handler.ListaAzure= new ObservableCollection<AzureModel>();
                handler.Azure = new AzureModel();
                SqliteDAO.pObtListaAzure(handler);
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void ListaHTML()
        {
            try
            {
                handler.ListaHTML = new ObservableCollection<SelectListItem>();
                SqliteDAO.pListaHTML(handler);

                handler.HtmlEspecificacion.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyEspecificacion)).Value);
                handler.HtmlMetodo.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyMetodo)).Value);
                handler.HtmlMetodoParam.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyMetoParam)).Value);
                handler.HtmlMetodoReturn.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyMetoReturn)).Value);
                handler.HtmlScript.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyScript)).Value);
            }
            catch (Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public void DatosRutaActualizacion()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoRutaActualiza);
            file = Path.Combine(path, archivoBD);

            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                sbLine = streamReader.ReadLine();

                while (sbLine != null)
                {
                    substrings = sbLine.Split(delimiter);
                    //handler.DatosRutaActualiza.Text = substrings[0];
                    //handler.DatosRutaActualiza.Value = substrings[1];
                    sbLine = streamReader.ReadLine();
                }
            }
        }
        public void DocumentacionHtml()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoDocHtml);
            file = Path.Combine(path, archivoBD);

            try
            {
                handler.ListaDocHtml = new ObservableCollection<DocumentacionHTML>();
                SqliteDAO.pDocumentacionHtml(handler);
            }
            catch (Exception ex)
            {
                //handler.MensajeError(ex.Message);
            }
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        #region Configuracion
        public void pCargarConfiguracion()
        {
            handler.ConfGeneralView.Model.OrdenAutomatico = Convert.ToBoolean(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyOrdenAutomatico)).Value);
            handler.ConfGeneralView.Model.Grant = Convert.ToBoolean(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyGeneraGrants)).Value);

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyProxy)))
            {
                handler.ConfGeneralView.Model.Proxy = Convert.ToBoolean(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyProxy)).Value);
            }
            else
            {
                handler.ConfGeneralView.Model.Proxy = false;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaSonar)))
            {
                handler.RutaSonar = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaSonar)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaGitDatos)))
            {
                handler.RutaGitDatos = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaGitDatos)).Value;
            }

            /*if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaGitBash)))
            {
                handler.RutaGitBash = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaGitBash)).Value;
            }*/

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyEmail)))
            {
                handler.ConnView.Model.Correo = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyEmail)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaGitObjetos)))
            {
                handler.RutaGitObjetos = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaGitObjetos)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.SQLPLUS)))
            {
                handler.ConfGeneralView.Model.RutaSqlplus = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.SQLPLUS)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KEY_EMPRESA)))
            {
                string codEmpresa = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KEY_EMPRESA)).Value;
                handler.ConfGeneralView.Model.Empresa = handler.ConfGeneralView.Model.ListaEmpresas.ToList().Find(x=>x.Codigo.Equals(codEmpresa));
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KEY_LLAVEW)))
            {
                handler.ConfGeneralView.Model.LlaveW = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KEY_LLAVEW)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KEY_VALORW)))
            {
                handler.ConfGeneralView.Model.ValorW = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KEY_VALORW)).Value;
            }
        }
        #endregion Configuracion
    }
}

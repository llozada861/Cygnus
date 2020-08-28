using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Index
{
    public class IndexModel
    {
        private Handler handler;
        private string path;
        private string sbLine;
        private string archivoBD;
        private string file;
        private Char delimiter = ';';
        private String[] substrings;
        public IndexModel(Handler hand)
        {
            handler = hand;

            try
            {
                pBorrarListas();
            }
            catch { }

            //se generan los archivos de configuración
            pGeneraBaseDatos();

            //Carga las listas en el manejador
            pCargarListasArchivo();

            //Carga la configuración
            pCargarConfiguracion();
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
            nombre = "Cygnus.db";
            myFile = res.Cygnus;
            handler.pCreaArchivoBD(Environment.CurrentDirectory, nombre, myFile);

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

            ListaEncabezadoObjetos();
            ListaTiposObjetos();
            ListaUsGrants();
            ListaPalabrasReservadas();
            ListaConfiguracion();
            ListaUsuarios();
            DatosBd();
            DatosRutaActualizacion();
            DocumentacionHtml();
            ListaRutas();
            ListaHTML();
        }

        public void pBorrarListas()
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
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoTiposObj);
            file = Path.Combine(path, archivoBD);

            handler.ListaTiposObjetos = new ObservableCollection<SelectListItem>();
            SqliteDAO.pListaTiposObjetos(handler);
        }

        public void ListaUsGrants()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoUsGrants);
            file = Path.Combine(path, archivoBD);

            handler.ListaUsGrants = new ObservableCollection<SelectListItem>();
            SqliteDAO.pListaUsGrants(handler);
        }

        public void ListaUsuarios()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoUsuariosBD);
            file = Path.Combine(path, archivoBD);

            handler.ListaUsuarios = new ObservableCollection<SelectListItem>();
            SqliteDAO.pListaUsuarios(handler);
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

            SqliteDAO.pDatosBd(handler);
        }

        public void RutaSqlplus()
        {
            sbLine = null;
            archivoBD = Path.Combine(res.CarpetaBD, res.NombreArchivoRutaSqlplus);
            file = Path.Combine(path, archivoBD);

            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                sbLine = streamReader.ReadLine();
                handler.ConfGeneralViewModel.RutaSqlplus = sbLine;
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
            handler.ListaRutas = new ObservableCollection<SelectListItem>();
            SqliteDAO.pListaRutas(handler);

            handler.ConfGeneralViewModel.RutaSqlplus = handler.ListaRutas.ToList().Find(x => x.Text.Equals(res.SQLPLUS)).Value;
        }

        public void ListaHTML()
        {
            handler.ListaHTML = new ObservableCollection<SelectListItem>();
            SqliteDAO.pListaHTML(handler);

            handler.HtmlEspecificacion.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyEspecificacion)).Value);
            handler.HtmlMetodo.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyMetodo)).Value);
            handler.HtmlMetodoParam.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyMetoParam)).Value);
            handler.HtmlMetodoReturn.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyMetoReturn)).Value);
            handler.HtmlScript.Append(handler.ListaHTML.ToList().Find(x => x.Text.Equals(res.KeyScript)).Value);
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

            handler.ListaDocHtml = new ObservableCollection<DocumentacionHTML>();
            SqliteDAO.pDocumentacionHtml(handler);
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        #region Configuracion
        public void pCargarConfiguracion()
        {
            handler.ConfGeneralViewModel.OrdenAutomatico = Convert.ToBoolean(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyOrdenAutomatico)).Value);
            handler.ConfGeneralViewModel.Grant = Convert.ToBoolean(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyGeneraGrants)).Value);

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyProxy)))
            {
                handler.ConfGeneralViewModel.Proxy = Convert.ToBoolean(handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyProxy)).Value);
            }
            else
            {
                handler.ConfGeneralViewModel.Proxy = false;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaSonar)))
            {
                handler.RutaSonar = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaSonar)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaGitDatos)))
            {
                handler.RutaGitDatos = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaGitDatos)).Value;
            }

            if (handler.ListaConfiguracion.Exists(x => x.Text.Equals(res.KeyRutaGitBash)))
            {
                handler.RutaGitBash = handler.ListaConfiguracion.Find(x => x.Text.Equals(res.KeyRutaGitBash)).Value;
            }

            /*
            handler.ConfGeneralViewModel.GeneraVersion = Convert.ToBoolean(handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(0).Value);
            handler.ConfGeneralViewModel.AplicaCarpetaArriba = Convert.ToBoolean(handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(1).Value);
            handler.ConfGeneralViewModel.GeneraListaChequeo = Convert.ToBoolean(handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(4).Value);
            handler.ConfGeneralViewModel.CopiaRepo = Convert.ToBoolean(handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(5).Value);
            handler.ConfGeneralViewModel.SolicitudLineaBase = Convert.ToBoolean(handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(6).Value);
            handler.ConfGeneralViewModel.RutaCopiaRepoMantCorr = handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(7).Value;
            handler.ConfGeneralViewModel.RutaCopiaRepoMantPrev = handler.ConfGeneralViewModel.ListaConfiguracion.ElementAt(8).Value;

            handler.ConfGeneralViewModel.MantenimientoCorrectivo.Text = Formulario.RutaCopiaRepoMantCorr;
            handler.ConfGeneralViewModel.MantenimientoCorrectivo.Value = res.CORRECTIVO;
            handler.ConfGeneralViewModel.MantenimientoCorrectivo.Observacion = res.RutaRepoMantCorrectivo;

            handler.ConfGeneralViewModel.MantenimientoPreventivo.Text = Formulario.RutaCopiaRepoMantPrev;
            handler.ConfGeneralViewModel.MantenimientoPreventivo.Value = res.PREVENTIVO;
            handler.ConfGeneralViewModel.MantenimientoPreventivo.Observacion = res.RutaRepoMantPreventivo;*/
        }
        #endregion Configuracion
    }
}

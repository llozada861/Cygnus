using Cygnus2_0.Conn;
using Cygnus2_0.DAO;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Index;
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Security;
using Cygnus2_0.ViewModel.Aplica;
using Cygnus2_0.ViewModel.Compila;
using Cygnus2_0.ViewModel.Index;
using Cygnus2_0.ViewModel.Objects;
using Cygnus2_0.ViewModel.Settings;
using FirstFloor.ModernUI.Windows.Controls;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using res = Cygnus2_0.Properties.Resources;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Xml;
using Cygnus2_0.Pages.Settings.General;
using System.Reflection;
using System.Windows.Input;

namespace Cygnus2_0.General
{
    public class Handler : ViewModelBase
    {
        //View models
        private ConexionViewModel conexionViewModel;
        private IndexViewModel indexViewModel;
        private ConfGeneralViewModel confGeneralViewModel;
        private AppearanceViewModel settings;
        private MarkDAO dao;
        private string estadoConn;
        private bool roleAdmin;
        private bool roleEspecialist;
        private bool roleUser;
        private string email;

        //Conexion
        private ConexionOracle conectionOracle;

        public Handler()
        {
            //Se crean las rutas generales
            this.RutaBaseDatos = Path.Combine(Environment.CurrentDirectory, res.CarpetaBD);
            this.PathTempAplica = Path.Combine(Environment.CurrentDirectory, res.CarpetaAplTemp);
            this.RutaBk = Path.Combine(Environment.CurrentDirectory, res.CarpetaBK);

            this.HtmlEspecificacion = new StringBuilder();
            this.HtmlMetodo = new StringBuilder();
            this.HtmlMetodoParam = new StringBuilder();
            this.HtmlMetodoReturn = new StringBuilder();
            this.HtmlScript = new StringBuilder();

            #region ViewModels
            //Se instancian los view models generales           
            conexionViewModel = new ConexionViewModel(this);
            confGeneralViewModel = new ConfGeneralViewModel(this);
            indexViewModel = new IndexViewModel(this);
            #endregion ViewModels

            #region Appareance
            settings = new AppearanceViewModel();
            System.Uri uri = new Uri(this.ListaConfiguracion.Find(x => x.Text.Equals(res.keyThemeSource)).Value, UriKind.Relative);
            System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(this.ListaConfiguracion.Find(x => x.Text.Equals(res.keyThemeColor)).Value);
            Tema = this.ListaConfiguracion.Find(x => x.Text.Equals(res.keyDisplayName)).Value;
            settings.SetThemeAndColor
            (
                Tema,
                uri,
                color,
                this.ListaConfiguracion.Find(x => x.Text.Equals(res.keyFontSize)).Value
            );
            #endregion Appareance

            #region Conexion
            conectionOracle = new ConexionOracle(conexionViewModel);
            dao = new MarkDAO(this);
            #endregion Conexion

            #region Listas
            this.ListaTipoArchivos = new List<SelectListItem>()
                {
                    new SelectListItem {
                        Text = res.ExtensionPlantilla, Value = res.TipoPlantilla
                    },
                    new SelectListItem {
                        Text = res.ExtensionExcel, Value = res.TipoExcel
                    },
                    new SelectListItem {
                        Text = res.ExtensionExcelX, Value = res.TipoExcel
                    },
                    new SelectListItem {
                        Text = res.ExtensionHtml, Value = res.TipoHtml
                    },
                    new SelectListItem {
                        Text = res.ExtensionWord, Value = res.TipoWord
                    },
                    new SelectListItem {
                        Text = res.ExtensionWordX, Value = res.TipoWord
                    },
                    new SelectListItem {
                        Text = "GRANT", Value = res.Script
                    }
                };

            this.ListaSiNO = new List<SelectListItem>() {
                new SelectListItem {
                        Text = res.Si, Value = res.Si
                    },
                new SelectListItem {
                        Text = res.No, Value = res.No
                    }
            };

            ListaComboGrantTO = new List<SelectListItem>() {
                new SelectListItem {
                        Text = res.GrantEXECUTE, Value = res.TipoGrantExecute
                    },
                new SelectListItem {
                        Text = res.GrantSELECT, Value = res.TipoGrantSelect
                    },
                new SelectListItem {
                        Text = res.GrantSIUD, Value = res.TipoGrantSIUD
                    },
                new SelectListItem {
                        Text = res.No, Value = res.No
                    }
            };

            ListaTipoFin = new List<SelectListItem>() {
                new SelectListItem {
                        Text = "Punto y Coma", Value = res.PuntoYComa
                    },
                new SelectListItem {
                        Text = res.END, Value = res.END
                    }
            };

            ListaTipoHTml = new List<SelectListItem>() {
                new SelectListItem {
                        Text = "Principal", Value = res.Principal
                    },
                new SelectListItem {
                        Text = "Secundario", Value = res.MetodoSecuencia
                    }
            };

            #endregion Listas
        }

        #region AtrViewModels
        public ConexionViewModel ConnViewModel
        {
            get { return conexionViewModel; }
            set { conexionViewModel = value; }
        }
        public ConfGeneralViewModel ConfGeneralViewModel
        {
            get { return confGeneralViewModel; }
            set { confGeneralViewModel = value; }
        }
        #endregion AtrViewModels

        #region Conexion Oracle
        public ConexionOracle ConexionOracle
        {
            get { return conectionOracle; }
            set { conectionOracle = value; }
        }
        public MarkDAO DAO
        {
            get { return dao; }
            set { dao = value; }
        }
        #endregion Conexion Oracle
        public string EstadoConn
        {
            get { return estadoConn = fsbValidaConexion(); }
            set { estadoConn = fsbValidaConexion(); }
        }
        public bool IsAdmin
        {
            get { return roleAdmin; }
            set { SetProperty(ref roleAdmin, pObtRole(2)); }
        }
        public bool IsEspecialist
        {
            get { return roleEspecialist; }
            set { SetProperty(ref roleEspecialist, pObtRole(1)); }
        }
        public bool IsUser
        {
            get { return roleUser; }
            set { SetProperty(ref roleUser, pObtRole(0)); }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public AppearanceViewModel Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        public List<SelectListItem> ListaRoles { get; set; }
        public DesblockViewModel DesblockViewModel { set; get; }
        public string RutaBaseDatos { set; get; }
        public string Tema { set; get; }
        public string RutaBk { set; get; }
        public string PathTempAplica { set; get; }
        public Double Version { set; get; }
        public string fsbVersion { set; get; }
        public string LoadPath { set; get; }
        public string CarpetaPrincipalRepo { set; get; }
        public string SavePath { set; get; }
        public string SavePathAplica { set; get; }
        public string PathArchivos { set; get; }
        public string CarpetaPadre { set; get; }
        public string RutaSonar { set; get; }
        public string RutaGitBash { set; get; }
        public string RutaGitDatos { set; get; }
        public string RutaGitObjetos { set; get; }
        public string EsLlamadoDesdeUpdater { set; get; }
        public List<SelectListItem> ListaChequeo { set; get; }
        public List<SelectListItem> ListaTipoArchivos { get; set; }
        public ObservableCollection<SelectListItem> ListaEncabezadoObjetos { get; set; }
        public ObservableCollection<SelectListItem> ListaTiposObjetos { get; set; }
        public ObservableCollection<SelectListItem> ListaUsGrants { get; set; }
        public List<SelectListItem> ListaTiposRepo { get; set; }
        public List<SelectListItem> ListaTiposRQ { get; set; }
        public ObservableCollection<SelectListItem> ListaPalabrasReservadas { get; set; }
        public ObservableCollection<SelectListItem> ListaRutas { get; set; }
        public ObservableCollection<SelectListItem> ListaUsuarios { get; set; }
        public List<SelectListItem> ListaConfiguracion { get; set; }
        public List<SelectListItem> ListaSiNO { get; set; }
        public List<SelectListItem> ListaTipoFin { get; set; }
        public List<SelectListItem> ListaComboGrantTO { get; set; }
        public List<SelectListItem> ListaTipoHTml { get; set; }
        public ObservableCollection<DocumentacionHTML> ListaDocHtml { get; set; }
        public ObservableCollection<SelectListItem> ListaHTML { get; set; }
        public SelectListItem Generico { get; set; }
        public SelectListItem Generico2 { get; set; }
        public StringBuilder HtmlEspecificacion { set; get; }
        public StringBuilder HtmlMetodo { set; get; }
        public StringBuilder HtmlMetodoParam { set; get; }
        public StringBuilder HtmlMetodoReturn { set; get; }
        public StringBuilder HtmlScript { set; get; }
        public Boolean GuardarTiempos { set; get; }
        public void MensajeError(string mensaje)
        {
            try
            {
                ModernDialog.ShowMessage(mensaje, "Error", System.Windows.MessageBoxButton.OKCancel);
                //System.Windows.MessageBox.Show(mensaje, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch
            {
                System.Windows.MessageBox.Show(mensaje, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        public void MensajeAdvertencia(string mensaje)
        {
            ModernDialog.ShowMessage(mensaje, "Mensaje Informativo", System.Windows.MessageBoxButton.OK);
            //System.Windows.MessageBox.Show(mensaje, "Mensaje Advertencia", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
        public void MensajeOk(string mensaje)
        {
            ModernDialog.ShowMessage(mensaje, "Mensaje Éxito", System.Windows.MessageBoxButton.OK);
        }

        public string MensajeConfirmacion(string mensaje)
        {
            if (System.Windows.Forms.MessageBox.Show(mensaje, "Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                return "Y";
            else
                return "N";
        }

        public void ObtenerTipoArchivo(Archivo archivo)
        {
            string sbLine = "";
            string sbLineSpace = "";
            Int64 nuMenosUno = Convert.ToInt64(res.MenosUno);

            string nombreArchivo = archivo.NombreSinExt;

            if (res.Extensiones.IndexOf(archivo.Extension.ToLower()) > -1)
            {
                using (StreamReader streamReader = new StreamReader(archivo.RutaConArchivo))
                {
                    sbLine = streamReader.ReadLine();

                    while (sbLine != null)
                    {
                        sbLineSpace = Regex.Replace(sbLine, @"\s+", " ");

                        foreach (SelectListItem prefijo in this.ListaEncabezadoObjetos.OrderBy(x => x.Prioridad))
                        {
                            if (sbLineSpace.ToLower().IndexOf(prefijo.Text.ToLower()) > nuMenosUno)
                            {
                                archivo.Tipo = prefijo.Value;
                                archivo.SelectItemTipo = ListaTiposObjetos.ToList().Find(x=>x.Text.Equals(archivo.Tipo));
                                archivo.NombreObjeto = pObtenerNombreObjeto(sbLineSpace);
                                archivo.FinArchivo = prefijo.Fin.Equals(res.PuntoYComa) ? ";" : prefijo.Fin;
                                archivo.InicioArchivo = prefijo.Text.ToLower();

                                if (archivo.Tipo.ToLower().Equals(res.Script.ToLower()))
                                    archivo.NombreObjeto = "";

                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(archivo.Tipo))
                        {
                            sbLine = streamReader.ReadLine();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        public string pObtenerNombreObjeto(string sbLineSpace)
        {
            string[] split;
            string[] splitCualificado;
            Boolean blPalabraReservada = false;
            string sbPalabra = "";
            int indice;
            Boolean blIndex = false;

            split = sbLineSpace.Split();

            for (int i = 0; i < split.Length; i++)
            {
                blPalabraReservada = false;
                sbPalabra = split[i].Trim().ToLower();

                foreach (SelectListItem item in this.ListaPalabrasReservadas)
                {
                    if (sbPalabra.Equals(item.Text))
                    {
                        blPalabraReservada = true;
                    }
                }

                if(blIndex && blPalabraReservada)
                    blIndex = false;

                if (blPalabraReservada && sbPalabra.Equals("index"))
                    blIndex = true;

                if (!blPalabraReservada && sbPalabra.Length > 0 && !blIndex)
                {
                    if (sbPalabra.IndexOf("(") >= 0)
                    {
                        indice = sbPalabra.IndexOf("(");
                        sbPalabra = sbPalabra.Substring(0, indice); //(indice - 1));
                    }
                    else
                        break;
                }
            }

            splitCualificado = sbPalabra.Split('.');

            if (splitCualificado.Length > 1)
            {
                foreach (SelectListItem usuario in this.ListaUsuarios)
                {
                    if (splitCualificado[0].ToLower().IndexOf(usuario.Text.ToLower()) > -1)
                    {
                        throw new Exception("No se pueden trabajar, ni entregar objetos cualificados. Por favor ajustar. [" + sbPalabra + "]");
                    }
                }
            }

            return sbPalabra;
        }

        public void pCrearArchivoDesdeFuente(string sourceFile, string destFile)
        {
            using (StreamReader fuente = new StreamReader(sourceFile))
            {
                using (StreamWriter origen = new StreamWriter(destFile))
                {
                    origen.Write(fuente.ReadToEnd());
                }
            }
        }

        internal void EvaluateErrorCode(string codigo, string error)
        {
            if (Convert.ToInt64(codigo) != 0)
            {
                throw new Exception("Código: " + codigo + ". Mensaje Error: " + error);
            }
        }

        public void pObtieneBloquesCodigo(Archivo archivo)
        {
            string sbLineSpace;
            string sbLine;
            string cuerpo = "";
            Boolean finObjeto = false;
            Boolean iniObjeto = false;

            List<string> lines;

            try
            {
                lines = File.ReadAllLines(archivo.RutaConArchivo, Encoding.Default).ToList();

                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    sbLine = lines[i];
                    sbLineSpace = Regex.Replace(sbLine, @"\s+", " ");

                    if (sbLineSpace.ToLower().IndexOf(archivo.FinArchivo.ToLower()) > -1 && !finObjeto)
                        finObjeto = true;

                    if (finObjeto)
                        cuerpo = sbLine + System.Environment.NewLine + cuerpo;

                    if (sbLineSpace.ToLower().IndexOf(archivo.InicioArchivo.ToLower()) > -1)
                    {
                        iniObjeto = true;
                        finObjeto = false;
                    }

                    if (archivo.Tipo.Equals(res.TipoObjetoPaquete.ToLower()) && iniObjeto)
                    {
                        archivo.BloquesCodigo.Add(cuerpo);
                        cuerpo = "";
                        iniObjeto = false;
                    }
                    else if (iniObjeto)
                    {
                        archivo.BloquesCodigo.Add(cuerpo);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.MensajeError("Error: " + ex.Message);
            }
        }

        public void pGuardaLogCompilacion(Archivo archivo, string objetosInvalidos, string tiempo)
        {
            DAO.pGuardaLogCompilacion(archivo, Environment.MachineName, Environment.UserName, objetosInvalidos, tiempo, this.ConnViewModel.UsuarioCompila);
        }

        public void pGeneraArchivosPermisosArchivo(Archivo archivo, string usuario)
        {
            StringBuilder grant = new StringBuilder();
            string usuarioGrant = "";
            List<string> sql = new List<string>();

            if (this.ListaUsGrants.Count > 0)
            {
                foreach (SelectListItem tipo in this.ListaTiposObjetos)
                {
                    //Si el tipo aplica para grant
                    if (archivo.Tipo.ToLower().Equals(tipo.Text.ToLower()) && !tipo.Grant.ToLower().Equals(res.No.ToLower()))
                    {
                        foreach (SelectListItem item in this.ListaUsGrants)
                        {
                            usuarioGrant = item.Text;
                            grant = new StringBuilder();

                            if (usuarioGrant.ToUpper().Equals(usuario.ToUpper()))
                            {
                                continue;
                            }

                            if (tipo.Grant.ToLower().Equals(res.TipoGrantSelect.ToLower()))
                            {
                                grant.AppendLine(res.PlantillaGrantNP);
                                grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                grant.Replace(res.TagGrantPermiso, res.GrantSELECT);
                                grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                sql.Add(grant.ToString());
                            }

                            //Permisos execute
                            if (tipo.Grant.ToLower().Equals(res.TipoGrantExecute.ToLower()))
                            {
                                grant.AppendLine(res.PlantillaGrantNP);
                                grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                grant.Replace(res.TagGrantPermiso, res.GrantEXECUTE);
                                grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                sql.Add(grant.ToString());
                            }

                            if (tipo.Grant.ToLower().Equals(res.TipoGrantSIUD.ToLower()))
                            {
                                grant.AppendLine(res.PlantillaGrantNP);
                                grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                grant.Replace(res.TagGrantPermiso, res.GrantSIUD);
                                grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                sql.Add(grant.ToString());
                            }
                        }
                        grant = new StringBuilder();
                        grant.AppendLine(res.PlantillaSinonimoNP);
                        grant.Replace(res.TagGrantUsuario, usuario);
                        grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                        sql.Add(grant.ToString());
                    }
                }
            }

            if (sql.Count > 0)
            {
                foreach (string sentence in sql)
                {
                    this.DAO.pEjecutarScriptBD(sentence);
                }
            }
        }
        public string fsbValidaConexion()
        {
            if (this.ConexionOracle.ConexionOracleSQL.State == System.Data.ConnectionState.Open)
            {
                return "Conectado.";
            }
            else
            {
                return "Sin Conexión.";
            }
        }

        public void pGuardaArchivo(string NombreArchivo, string cuerpo)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = NombreArchivo;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                File.WriteAllText(saveFileDialog.FileName, cuerpo, Encoding.Default);
        }

        public void pGuardaArchivoByte(string NombreArchivo, string nuevoNombre)
        {
            byte[] cuerpo;

            using (Stream fs = File.OpenRead(NombreArchivo))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    cuerpo = br.ReadBytes((Int32)fs.Length);
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = nuevoNombre;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, cuerpo);
        }

        public bool pObtRole(int role)
        {
            bool valor = false;

            if (System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                int rol = ((CustomIdentity)System.Threading.Thread.CurrentPrincipal.Identity).Role;

                if (rol == role)
                {
                    valor = true;
                }
            }

            return valor;
        }

        public void pRealizaConexion()
        {
            this.ConexionOracle.RealizarConexion();

            //Establece el rol
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal == null)
                throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

            //string cred = EncriptaPass.Encriptar("SQL_LLOZADA-2");

            int rol = this.DAO.pObtRol(conexionViewModel.Usuario);

            if (rol > 0)
            {
                //Authenticate the user
                customPrincipal.Identity = new CustomIdentity(conexionViewModel.Usuario, "", rol);
            }
            else
            {
                string pass = conexionViewModel.Usuario.Trim() + "-0";
                this.DAO.pGuardaRol(conexionViewModel.Usuario.Trim().ToUpper(), EncriptaPass.Encriptar(pass), "");
            }

            //Se obtiene el usuario azure
            this.DAO.pObtAreaAzure();
            SqliteDAO.pCreaConfiguracion(res.KeyEmail, ConnViewModel.Correo);

            if (!SqliteDAO.pblValidaVersion(this))
            {
                try
                {
                    //Se insertan los datos en la bd
                    //dao.pObtCodigoSql();

                    //Se actualiza la versión
                    SqliteDAO.pActualizaVersion(this.fsbVersion);

                    indexViewModel = new IndexViewModel(this);
                }
                catch
                { }
            }
        }

        public void pCreaArchivoBD(string path, string nombre, byte[] myFile)
        {
            string archivo = Path.Combine(path, nombre);

            if (!File.Exists((string)archivo))
            {
                using (FileStream tempFile = File.Create(archivo))
                    tempFile.Write(myFile, 0, myFile.Length);
            }
        }

        public void pDropFiles(string ruta)
        {
            string[] DropPath = System.IO.Directory.GetFiles(ruta + "\\", "*", System.IO.SearchOption.AllDirectories);

            foreach (string dropfilepath in DropPath)
            {
                File.Delete(dropfilepath);
            }
        }

        public void pGeneraArchivoRuta(string filename, byte[] content)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = filename;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, content);

        }

        //method to send email to Gmail
        public void sendEMailThroughOUTLOOK(string asunto, string body)
        {
            try
            {
                // Create the Outlook application.
                Outlook.Application oApp = new Outlook.Application();
                // Create a new mail item.
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                // Set HTMLBody. 
                //add the body of the email
                oMsg.HTMLBody = body;
                //Add an attachment.
                String sDisplayName = "MyAttachment";
                int iPosition = (int)oMsg.Body.Length + 1;
                int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
                //now attached the file
                //Outlook.Attachment oAttach = oMsg.Attachments.Add
                //                             (@"C:\\fileName.jpg", iAttachType, iPosition, sDisplayName);
                //Subject line
                oMsg.Subject = asunto;
                // Add a recipient.
                Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                // Change the recipient in the next line if necessary.
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(DAO.pObtGrupoCorreo());
                oRecip.Resolve();
                // Send.
                oMsg.Send();
                // Clean up.
                oRecip = null;
                oRecips = null;
                oMsg = null;
                oApp = null;
            }//end of try block
            catch (Exception ex)
            {
                this.MensajeOk("Error: " + ex.Message);
            }//end of catch
        }//end of Email Method

        public void pEnviarCorreo(string para, string asunto, string body)
        {
            try
            {
                // Create the Outlook application.
                Outlook.Application oApp = new Outlook.Application();
                // Create a new mail item.
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                // Set HTMLBody. 
                //add the body of the email
                oMsg.HTMLBody = body;
                //Add an attachment.
                String sDisplayName = "MyAttachment";
                int iPosition = (int)oMsg.Body.Length + 1;
                int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
                //Subject line
                oMsg.Subject = asunto;
                // Add a recipient.
                Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                // Change the recipient in the next line if necessary.
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(para);
                oRecip.Resolve();
                // Send.
                oMsg.Send();
                // Clean up.
                oRecip = null;
                oRecips = null;
                oMsg = null;
                oApp = null;
            }//end of try block
            catch (Exception ex)
            {
                this.MensajeOk("Error: " + ex.Message);
            }//end of catch
        }//end of Email Method

        public Boolean pDepuraDocumentacion(Archivo archivo)
        {
            string sbLine = "";
            string sbLineSpace = "";
            int nuMenosUno = -1;
            string sbTagFin = null;

            StringBuilder documentacion = new StringBuilder();

            if (this.ConfGeneralViewModel.GeneraHtml)
            {
                using (StreamReader streamReader = new StreamReader(archivo.RutaConArchivo, Encoding.Default))
                {
                    sbLine = streamReader.ReadLine();

                    while (sbLine != null)
                    {
                        //Se eliminan los espacios en blanco
                        sbLineSpace = Regex.Replace(sbLine, @"\s+", " ");

                        if (string.IsNullOrEmpty(sbTagFin))
                        {
                            //filtrar por principales
                            foreach (DocumentacionHTML html in this.ListaDocHtml.Where(x => x.Tipo.Equals(res.Principal)))
                            {
                                if (sbLineSpace.ToLower().IndexOf(html.TagInicio.ToLower()) > nuMenosUno)
                                {
                                    sbTagFin = html.TagFin;
                                    documentacion = new StringBuilder();
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(sbTagFin))
                        {
                            documentacion.AppendLine(sbLine);

                            if (sbLineSpace.ToLower().IndexOf(sbTagFin.ToLower()) > nuMenosUno)
                            {
                                sbTagFin = null;
                                archivo.DocumentacionSinDepurar.Add(documentacion);
                                //view.ListaArchivosCargados.ToList().Find(x => x.FileName.Equals(archivo.FileName)).DocumentacionSinDepurar.Add(documentacion);
                            }
                        }

                        sbLine = streamReader.ReadLine();
                    }
                }

                //Si encuentra documentación la procesa
                if (archivo.DocumentacionSinDepurar.Count > 0)
                {
                    foreach (StringBuilder docu in archivo.DocumentacionSinDepurar)
                    {
                        pExtraerXml(docu.ToString(), archivo);
                    }

                    return fblGeneraHtml(archivo);
                }
            }

            return false;
        }

        public void pExtraerXml(string xml, Archivo archivo)
        {
            string strXMLdoc = xml;
            string metodo;
            string NodoPrincipal = "//*[translate(name(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
            string AtributoNodo = "@*[translate(name(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
            XmlNode node;

            XmlNodeList historial;
            XmlNodeList parametros;

            XmlDocument xmlDoc = new XmlDocument();
            DocumentacionModel doc = new DocumentacionModel();
            doc.Modificaciones = new List<ModificacionModel>();
            doc.Parametros = new List<ParametrosModel>();
            doc.Retorno = new RetornoModel();

            using (StringReader reader = new StringReader(strXMLdoc))
            {
                try
                {
                    xmlDoc.Load(reader);
                    metodo = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "unidad")).InnerText;

                    if (metodo.ToLower().Trim().Equals(archivo.NombreObjeto.Trim().ToLower()) && archivo.Tipo.ToLower().Equals(res.TipoObjetoPaquete.ToLower()))
                    {
                        node = xmlDoc.SelectSingleNode(String.Format(NodoPrincipal, "package"));
                        doc.Fuente = node.SelectSingleNode(String.Format(AtributoNodo, "fuente")).InnerText;
                    }
                    else
                    {
                        node = xmlDoc.SelectSingleNode(String.Format(NodoPrincipal, "procedure"));
                        doc.Fuente = node.SelectSingleNode(String.Format(AtributoNodo, "fuente")).InnerText;

                        try
                        {
                            parametros = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "parametros")).ChildNodes;

                            for (int i = 0; i < parametros.Count; i++)
                            {
                                doc.Parametros.Add(new ParametrosModel
                                {
                                    Nombre = parametros[i].SelectSingleNode(String.Format(AtributoNodo, "nombre")).InnerText,
                                    Tipo = parametros[i].SelectSingleNode(String.Format(AtributoNodo, "tipo")).InnerText,
                                    Direccion = parametros[i].SelectSingleNode(String.Format(AtributoNodo, "direccion")).InnerText,
                                    Descripcion = parametros[i].InnerText
                                });
                            }
                        }
                        catch
                        {
                        }

                        try
                        {
                            node = xmlDoc.SelectSingleNode(String.Format(NodoPrincipal, "retorno"));

                            doc.Retorno.Nombre = node.SelectSingleNode(String.Format(AtributoNodo, "nombre")).InnerText;
                            doc.Retorno.Tipo = node.SelectSingleNode(String.Format(AtributoNodo, "tipo")).InnerText;
                            doc.Retorno.Descripcion = node.InnerText;
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    doc.Unidad = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "unidad")).InnerText;
                    doc.Autor = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "autor")).InnerText;
                    doc.Fecha = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "fecha")).InnerText;
                    doc.Descripcion = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "descripcion")).InnerText;

                    try
                    {
                        historial = xmlDoc.DocumentElement.SelectSingleNode(String.Format(NodoPrincipal, "historial")).ChildNodes;

                        for (int i = 0; i < historial.Count; i++)
                        {
                            doc.Modificaciones.Add(new ModificacionModel
                            {
                                Autor = historial[i].SelectSingleNode(String.Format(AtributoNodo, "autor")).InnerText,
                                Fecha = historial[i].SelectSingleNode(String.Format(AtributoNodo, "fecha")).InnerText,
                                Requerimiento = historial[i].SelectSingleNode(String.Format(AtributoNodo, "inc")).InnerText,
                                Descripcion = historial[i].InnerText
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    archivo.ListDocumentacionDepurada.Add(doc);
                }
                catch (Exception ex)
                {
                    string metodoErr = pObtenerSubstring(xml.ToLower(), "<unidad>", "</unidad>", 0);
                    throw new Exception("Objeto: " + archivo.NombreObjeto + ". Método: " + metodoErr + ". Error [" + ex.Message + "]");
                }
            }

        }

        public Boolean fblGeneraHtml(Archivo archivo)
        {
            StringBuilder body = new StringBuilder();
            StringBuilder TablaPrincipal = new StringBuilder();
            StringBuilder TablaMetodo = new StringBuilder();
            string sbNombreMetodo = archivo.NombreObjeto;
            Boolean blEncontroHtml = false;
            string nombreArchivo = archivo.NombreObjeto + res.ExtensionHtml;
            string nombreArchivoHtml = Path.Combine(archivo.Ruta, nombreArchivo);

            body.Append(res.HTMLEncabezado);

            foreach (DocumentacionModel docu in archivo.ListDocumentacionDepurada)
            {
                if (docu.Unidad.Trim().ToLower().Equals(sbNombreMetodo.Trim().ToLower()))
                {
                    if (TablaPrincipal.Length == 0)
                    {
                        pGeneraMetodoHTML(TablaPrincipal, docu, res.Text_Paquete);
                        TablaPrincipal.Append(res.HTMLHistorialHeadModif);
                        pGeneraHistorialHTML(TablaPrincipal, docu);
                    }

                    /*if (TablaMetodo.Length > 0) //(TablaPrincipal.Length > 0)
                    {
                        //pGeneraHistorialHTML(TablaPrincipal, docu);
                        pGeneraHistorialHTML(TablaMetodo, docu);
                    }*/
                }
                else
                {
                    sbNombreMetodo = docu.Unidad.Trim().ToLower();
                    pGeneraMetodoHTML(TablaMetodo, docu, res.TipoExpMetodo + ":");

                    //Se adionan los retornos del método
                    pGeneraRetornosHTML(TablaMetodo, docu);
                    //Se adicionan los parámetros si el método tiene
                    pGeneraParametrosHTML(TablaMetodo, docu);
                    //Se adicionan las modificaciones del método
                    TablaMetodo.Append(res.HTMLHistorialHeadModif);
                    pGeneraHistorialHTML(TablaMetodo, docu);
                }
            }


            if (TablaPrincipal.Length > 0)
            {
                body.Append(TablaPrincipal);
                body.Append(res.HTMLTablaFin);
                blEncontroHtml = true;
            }

            if (TablaMetodo.Length > 0)
            {
                body.Append(TablaMetodo);
                body.Append(res.HTMLTablaFin);
                blEncontroHtml = true;
            }

            body.Append(res.HTMLFin);

            if (blEncontroHtml)
            {
                using (StreamWriter file = new System.IO.StreamWriter(nombreArchivoHtml))
                {
                    file.Write(body);
                }

                return true;
            }

            return false;
        }

        public void pGeneraMetodoHTML(StringBuilder cadena, DocumentacionModel modificacion, string tipo)
        {
            if (tipo.ToLower().Equals(res.Text_Paquete.ToLower()))
            {
                cadena.Append(res.HTMLTablaPaquete);
                //cadena.Append("<br><br><br>");
                cadena.Replace(res.TagHtml_metodo, tipo + modificacion.Unidad);
                cadena.Replace(res.TagHtml_descFuente, modificacion.Fuente);
                cadena.Replace(res.TagHtml_autor, modificacion.Autor);
                cadena.Replace(res.TagHtml_fecha, modificacion.Fecha);
                cadena.Replace(res.TagHtml_descMetodo, modificacion.Descripcion);
            }
            else
            {
                cadena.Append(res.HTMLTablaMetodo);
                cadena.Append("<br><br><br>");
                cadena.Replace(res.TagHtml_metodo, tipo + modificacion.Unidad);
                cadena.Replace(res.TagHtml_descFuente, modificacion.Fuente);
                cadena.Replace(res.TagHtml_autor, modificacion.Autor);
                cadena.Replace(res.TagHtml_fecha, modificacion.Fecha);
                cadena.Replace(res.TagHtml_descMetodo, modificacion.Descripcion);
            }
        }
        public void pGeneraHistorialHTML(StringBuilder cadena, DocumentacionModel docu)
        {
            foreach (ModificacionModel modificacion in docu.Modificaciones)
            {
                cadena.Append(res.HTMLHistorialModif);
                cadena.Replace(res.TagHtml_fecha, modificacion.Fecha);
                cadena.Replace(res.TagHtml_autor, modificacion.Autor);
                cadena.Replace(res.TagHtml_numeroOC, modificacion.Requerimiento);
                cadena.Replace(res.TagHtml_descripcionModi, modificacion.Descripcion);
            }
        }
        public void pGeneraParametrosHTML(StringBuilder cadena, DocumentacionModel docu)
        {
            StringBuilder Parametros = new StringBuilder();
            Boolean blParametros = false;
            Parametros.Append(res.HTMLHeadParametros);

            //Obtiene la descripción del paquete y sus modificaciones
            foreach (ParametrosModel parametro in docu.Parametros)
            {
                Parametros.Append(res.HTMLParametros);
                Parametros.Replace(res.TagHTML_parametro, parametro.Nombre);
                Parametros.Replace(res.TagHTML_tipo, parametro.Tipo);
                Parametros.Replace(res.TagHTML_in, parametro.Direccion.ToUpper().IndexOf("IN") >= 0 ? "X" : "");
                Parametros.Replace(res.TagHTML_out, parametro.Direccion.ToUpper().IndexOf("OUT") >= 0 ? "X" : "");
                Parametros.Replace(res.TagHTML_default, "");
                Parametros.Replace(res.TagHTML_Desc, parametro.Descripcion);
                blParametros = true;
            }

            if (blParametros)
            {
                cadena.Append(Parametros);
            }
        }
        public void pGeneraRetornosHTML(StringBuilder cadena, DocumentacionModel docu)
        {
            StringBuilder Retornos = new StringBuilder();
            Boolean blRetornos = false;
            Retornos.Append(res.HTMLHeadRetornos);

            if (!string.IsNullOrEmpty(docu.Retorno.Nombre))
            {
                Retornos.Append(res.HtmlRetornos);
                Retornos.Replace(res.TagHTML_nombre, docu.Retorno.Nombre);
                Retornos.Replace(res.TagHTML_tipo, docu.Retorno.Tipo);
                Retornos.Replace(res.TagHTML_Desc, docu.Retorno.Descripcion);
                blRetornos = true;
            }

            if (blRetornos)
            {
                cadena.Append(Retornos);
            }
        }
        public string pObtenerSubstring(string cadena, string iniTag, string finTag, int size)
        {
            int iniIndex;
            int finIndex;
            int tamano = 0;
            string resultado = "";

            iniIndex = cadena.IndexOf(iniTag);

            if (String.IsNullOrEmpty(finTag))
            {
                if (size > 0)
                    resultado = cadena.Substring(iniIndex + iniTag.Length, size);
                else
                    resultado = cadena.Substring(iniIndex);
            }
            else
            {
                finIndex = cadena.IndexOf(finTag, iniIndex);
                finIndex = finIndex + finTag.Length;
                tamano = finIndex - iniIndex;
                resultado = cadena.Substring(iniIndex, tamano);
            }

            return resultado;
        }

        public void pObtenerUsuarioCompilacion(string usuario)
        {
            string passEncrypt = DAO.pObtenerUsuarioCompilacion(usuario);
            string credenciales = EncriptaPass.Desencriptar(passEncrypt);
            string[] split = credenciales.Split('-');

            ConnViewModel.UsuarioCompila = split[0];
            ConnViewModel.PassCompila = split[1];

            ConexionOracle.RealizarConexionCompilacion();
        }

        public void CopyResource(string resourceName, string file)
        {
            using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }

                using (Stream output = File.OpenWrite(file))
                {
                    resource.CopyTo(output);
                }
            }
        }

        public void CursorWait()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        }
        public void CursorNormal()
        {
            Mouse.OverrideCursor = null;
        }

        public void pAbrirArchivo(string archivo)
        {
            System.Diagnostics.Process.Start(archivo);
        }

        public void pListaArchivosCarpeta(string Path, List<Archivo> archivos)
        {
            if (Path.ToUpper().IndexOf(".GIT") > -1 || Path.ToUpper().IndexOf("DESPLIEGUES") > -1)
                return;

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(Path);
            foreach (string fileName in fileEntries)
            {
                Archivo archivo = new Archivo();
                archivo.FileName = System.IO.Path.GetFileName(fileName);
                archivo.RutaConArchivo = fileName;
                archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                archivo.Ruta = System.IO.Path.GetDirectoryName(fileName);
                archivo.Extension = System.IO.Path.GetExtension(fileName);
                archivos.Add(archivo);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(Path);
            foreach (string subdirectory in subdirectoryEntries)
                pListaArchivosCarpeta(subdirectory, archivos);
        }

        public void pListaArchivos(string[] DropPath, List<Archivo> archivos)
        {
            foreach (string dropfilepath in DropPath)
            {
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dropfilepath)))
                {
                    string[] DropPath1 = System.IO.Directory.GetFiles(dropfilepath + "\\", "*", System.IO.SearchOption.AllDirectories);
                    pListaArchivos(DropPath1, archivos);
                }
                else
                {
                    Archivo archivo = new Archivo();
                    archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                    archivo.RutaConArchivo = dropfilepath;
                    archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                    archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                    archivo.Extension = System.IO.Path.GetExtension(dropfilepath);
                    archivo.ListaTipos = this.ListaTiposObjetos;
                    archivo.ListaUsuarios = this.ListaUsuarios;
                    archivo.NombreObjeto = archivo.NombreSinExt;
                    archivo.CarpetaPadre = pObtCarpetaPadre(archivo.RutaConArchivo);
                    this.ObtenerTipoArchivo(archivo);
                    archivo.BloquesCodigo = new List<string>();

                    if (archivo.ListaUsuarios.ToList().Exists(x => x.Text.ToUpper().Equals(archivo.CarpetaPadre.Trim().ToUpper())))
                    {
                        archivo.Usuario = archivo.CarpetaPadre.Trim().ToUpper();
                    }

                    if (string.IsNullOrEmpty(archivo.Tipo))
                    {
                        if (archivo.Extension.Equals(res.ExtensionHtml))
                        {
                            archivo.Tipo = res.TipoObjetoPaquete.ToLower();
                            archivo.NombreObjeto = archivo.NombreSinExt;
                        }
                    }

                    archivos.Add(archivo);
                }
            }
        }
        public string pObtCarpetaPadre(string RutaConArchivo)
        {
            System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(RutaConArchivo);
            return directoryInfo.Name;
        }

        public string[] pCargarArchivos()
        {
            string[] archivos = new string[]{ };

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                archivos = openFileDialog.FileNames;

            return archivos;
        }
    }
}
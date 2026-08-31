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
using Cygnus2_0.Model.Azure;
using Cygnus2_0.ViewModel.Azure;
using Cygnus2_0.ViewModel.Repository;
using Cygnus2_0.Model.User;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.Permisos;
using System.Runtime.InteropServices;
using Cygnus2_0.Model.Html;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Model.History;
using PlsqlAnalisisDL.General;
using Microsoft.Office.Interop.Excel;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Windows.Documents;
using System.Diagnostics;
using Cygnus2_0.Model.Plantillas;

namespace Cygnus2_0.General
{
    public class Handler : ViewModelBase
    {
        //View models
        private ConexionViewModel view;
        private IndexViewModel indexViewModel;
        private ConfGeneralViewModel confGeneralViewModel;
        private AppearanceViewModel settings;
        private MarkDAO dao;
        private RepositorioViewModel repositorioVM;
        private UpdateModel updateModel;

        private string estadoConn;
        private bool roleAdmin;
        private bool roleEspecialist;
        private bool roleUser;
        private string email;

        ObservableCollection<TipoObjetos> listaTiposObjetos;
        ObservableCollection<UsuarioModel> listaUsuarios;
        ObservableCollection<DocumentacionHTML> listaDocHtml;
        ObservableCollection<PlantillasHTMLModel> listaHtml;
        ObservableCollection<HistoriaModel> listaHistoria;

        //Conexion
        private ConexionOracle conectionOracle;

        public Handler()
        {
            updateModel = new UpdateModel(this);
        }

        public void pInicializar()
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

            #region ViewModels
            //Se instancian los view models generales           
            view = new ConexionViewModel(this);
            confGeneralViewModel = new ConfGeneralViewModel(this);
            pRegeneraIndexListas();
            repositorioVM = new RepositorioViewModel(this);
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
            conectionOracle = new ConexionOracle(view);
            dao = new MarkDAO(this);
            #endregion Conexion
        }

        internal void ModificaLlaveRegistro()
        {
            if (!string.IsNullOrEmpty(this.ConfGeneralView.Model.ValorW))
            {
                if (this.ConnView.Model.Conexion.Servidor.ToLower().Equals("epm-do13") || this.ConnView.Model.Conexion.Servidor.ToLower().Equals("10.1.16.32"))
                {
                    string valorW = "\"" + this.ConfGeneralView.Model.ValorW + "\\SAEAP.exe\" \"%1\"";
                    Microsoft.Win32.Registry.SetValue(this.ConfGeneralView.Model.LlaveW, null, valorW, Microsoft.Win32.RegistryValueKind.String);
                }
            }
        }

        #region AtrViewModels
        public ConexionViewModel ConnView
        {
            get { return view; }
            set { view = value; }
        }
        public ConfGeneralViewModel ConfGeneralView
        {
            get { return confGeneralViewModel; }
            set { confGeneralViewModel = value; }
        }

        public RepositorioViewModel RepositorioVM
        {
            get { return repositorioVM; }
            set { repositorioVM = value; }
        }

        public UpdateModel UpdateModel_
        {
            get { return updateModel; }
            set { updateModel = value; }
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
        public string CorreoGeneral { set; get; }
        public string RutaBk { set; get; }
        public string PathTempAplica { set; get; }
        public string fsbVersion { set; get; }
        public string LoadPath { set; get; }
        public string SavePath { set; get; }
        public string SavePathAplica { set; get; }
        public string PathArchivos { set; get; }
        public string CarpetaPadre { set; get; }
        public string RutaSonar { set; get; }
        public string ProyectoSonar { set; get; }
        public string RutaGitDatos { set; get; }
        public string RutaGitObjetos { set; get; }
        public List<SelectListItem> ListaChequeo { set; get; }
        public List<SelectListItem> ListaTipoArchivos { get; set; }
        public ObservableCollection<HeadModel> ListaEncabezadoObjetos { get; set; }
        public ObservableCollection<TipoObjetos> ListaTiposObjetos
        {
            get { return listaTiposObjetos; }
            set { SetProperty(ref listaTiposObjetos, value); }
        }
        public ObservableCollection<SelectListItem> ListaUsGrants { get; set; }
        public ObservableCollection<PermisosModel> ListaPermisos { get; set; }
        public List<SelectListItem> ListaTiposRepo { get; set; }
        public List<SelectListItem> ListaTiposRQ { get; set; }
        public ObservableCollection<PalabrasClaves> ListaPalabrasReservadas { get; set; }
        public PalabrasClaves PalabraSeleccionada { get; set; }
        public ObservableCollection<RutaObjetos> ListaRutas { get; set; }
        public ObservableCollection<PermisosObjeto> ListaPermisosObjeto { get; set; }
        public ObservableCollection<UsuarioModel> ListaUsuarios
        {
            get { return listaUsuarios; }
            set { SetProperty(ref listaUsuarios, value);

                foreach (UsuarioModel objeto in listaUsuarios)
                {
                    objeto.ListaSINO = new ObservableCollection<SelectListItem>(this.ListaSiNO);
                }
            }
        }
        public ObservableCollection<HistoriaModel> ListaHistoria
        {
            get { return listaHistoria; }
            set
            {
                SetProperty(ref listaHistoria, value);
            }
        }
        public UsuarioModel UsuarioSeleccionado { get; set; }
        public List<SelectListItem> ListaConfiguracion { get; set; }
        public List<SelectListItem> ListaSiNO { get; set; }
        public List<SelectListItem> ListaTipoFin { get; set; }
        public List<SelectListItem> ListaComboGrantTO { get; set; }
        public List<SelectListItem> ListaTipoHTml { get; set; }
        public ObservableCollection<DocumentacionHTML> ListaDocHtml
        {
            get
            {
                return listaDocHtml;
            }
            set
            {
                SetProperty(ref listaDocHtml, value);

                foreach (DocumentacionHTML objeto in listaDocHtml)
                {
                    objeto.ListaTipoHTml = new ObservableCollection<SelectListItem>(this.ListaTipoHTml);
                }
            }
        }
        public DocumentacionHTML DocHTMLSeleccionado { get; set; }
        public ObservableCollection<PlantillasHTMLModel> ListaPlantillas 
        {
            get
            {
                return listaHtml;
            }
            set
            {
                SetProperty(ref listaHtml, value);
            }
        }

        public SelectListItem Generico { get; set; }
        public SelectListItem Generico2 { get; set; }
        public StringBuilder HtmlEspecificacion { set; get; }
        public StringBuilder HtmlMetodo { set; get; }
        public StringBuilder HtmlMetodoParam { set; get; }
        public StringBuilder HtmlMetodoReturn { set; get; }
        public StringBuilder HtmlScript { set; get; }
        public Boolean GuardarTiempos { set; get; }
        public ObservableCollection<AzureModel> ListaAzure { get; set; }
        public AzureModel Azure { set; get; }
        public void MensajeError(string mensaje)
        {            
            try
            {
                ModernDialog.ShowMessage(mensaje, "Error", System.Windows.MessageBoxButton.OKCancel);
            }
            catch
            {
                System.Windows.MessageBox.Show(mensaje, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        public void MensajeAdvertencia(string mensaje)
        {
            ModernDialog.ShowMessage(mensaje, "Mensaje Informativo", System.Windows.MessageBoxButton.OK);
        }
        public void MensajeOk(string mensaje)
        {
            ModernDialog.ShowMessage(mensaje, "Mensaje Éxito", System.Windows.MessageBoxButton.OK);
        }

        #region Metodos
        public string MensajeConfirmacion(string mensaje)
        {
            if (ModernDialog.ShowMessage(mensaje, "Mensaje De Confirmación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                return "Y";
            else
                return "N";
        }

        public void ObtenerTipoArchivo(Archivo archivo)
        {
            ObservableCollection<TipoObjetos> tiposArchivo = new ObservableCollection<TipoObjetos>();

            //Se instancian las listas del archivo
            archivo.ListDocumentacionOut = new List<InstruccionPL>();
            archivo.ListDocumentacionIn = new List<InstruccionPL>();
            archivo.ListaErrores = new List<InstruccionPL>();
            archivo.ListaDocumentacionPkg = new List<InstruccionPL>();

            try
            {
                List<PlsqlAnalisisDL.General.InstruccionPL> analisisPL = PlsqlAnalisisDL.General.Plsql.AnalizarPL(archivo.RutaConArchivo);

                foreach (PlsqlAnalisisDL.General.InstruccionPL item in analisisPL.Where(x => x.Token == "TIPO"))
                {
                    TipoObjetos tipo = ListaTiposObjetos.Where(x=>x.Descripcion == item.Valor).FirstOrDefault();

                    if (tipo != null)
                    {
                        if(item.NombreObjeto != null)
                            archivo.NombreObjeto = item.NombreObjeto.ToLower();

                        tiposArchivo.Add(tipo);
                    }

                    if(tipo != null && (tipo.Descripcion == "PAQUETE" || tipo.Descripcion == "PROCEDIMIENTO" || tipo.Descripcion == "FUNCION" || tipo.Descripcion == "TRIGGER"))
                    {
                        break;
                    }

                }            

                if (tiposArchivo.Count > 0)
                {
                    var listaOrdenada = tiposArchivo.OrderBy(x => x.Prioridad).ToList();

                    foreach (var tipoOrd in listaOrdenada)
                    {
                        archivo.Tipo = tipoOrd.Codigo;
                        archivo.SelectItemTipo = ListaTiposObjetos.FirstOrDefault(x => x.Codigo == tipoOrd.Codigo);
                    }                

                    archivo.ListaTipos = ListaTiposObjetos;
                    archivo.ListDocumentacionOut = analisisPL.Where(x => x.Token == "COMMENT_OUT").ToList();
                    archivo.ListDocumentacionIn = analisisPL.Where(x => x.Token == "COMMENT_IN").ToList();
                    archivo.ListaDocumentacionPkg = analisisPL.Where(x => x.Token == "COMMENT_PKG").ToList();
                }

                //archivo.ListaErrores = analisisPL.Where(x => x.Token == "ERROR").ToList();
            }
            catch (Exception ex)
            {
                //this.MensajeConfirmacion("Se presentó el si");
            }
        }

        internal string pObtUsuarioTipo(int? tipo)
        {
            string usuario = null;
            RutaObjetos ruta = ListaRutas.FirstOrDefault(x => x.TipoObjeto == tipo);

            if(ruta != null)
            {
                usuario = ruta.Usuario;
            }

            return usuario;
        }

        internal void pGeneraArchivoHtml(ObservableCollection<Archivo> listaArchivosCargados, ObservableCollection<SelectListItem> listaObs)
        {
            foreach (Archivo archivo in listaArchivosCargados)
            {
                this.ObtenerTipoArchivo(archivo);

                if (this.pDepuraDocumentacion(archivo) && listaObs != null)
                {
                    listaObs.Add(new SelectListItem { Text = archivo.NombreObjeto.ToLower() + res.ExtensionHtml });
                }
            }
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

        public void pEjecutaPermisosArchivo(Archivo archivo, string usuario)
        {
            StringBuilder grant = new StringBuilder();
            string usuarioGrant = "";
            List<string> sql = new List<string>();

            if (this.ListaUsGrants.Count > 0)
            {
                foreach (TipoObjetos tipo in this.ListaTiposObjetos)
                {
                    //Si el tipo aplica para grant
                    if (archivo.Tipo == tipo.Codigo)
                    {
                        foreach(PermisosObjeto permisoObj in this.ListaPermisosObjeto)
                        {
                            if(tipo.Codigo == permisoObj.TipoObjeto)
                            {
                                PermisosModel permiso = this.ListaPermisos.FirstOrDefault(x => x.Codigo == permisoObj.Permiso);
                                SelectListItem usGrant = this.ListaUsGrants.FirstOrDefault(x => Int32.Parse(x.Value) == permisoObj.Usuario);

                                if (usGrant != null && !usuario.Equals(usGrant.Text))
                                {
                                    grant = new StringBuilder();
                                    grant.AppendLine(res.PlantillaGrantNP);
                                    grant.Replace(res.TagGrantUsuario, usGrant.Text);
                                    grant.Replace(res.TagGrantPermiso, permiso.Descripcion);
                                    grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                    sql.Add(grant.ToString());
                                }

                                if(!string.IsNullOrEmpty(this.view.Model.Usuario))
                                {
                                    grant = new StringBuilder();
                                    grant.AppendLine(res.PlantillaGrantNP);
                                    grant.Replace(res.TagGrantUsuario, this.view.Model.Usuario);
                                    grant.Replace(res.TagGrantPermiso, permiso.Descripcion);
                                    grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                    sql.Add(grant.ToString());
                                }
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
                    this.DAO.pEjecutarScriptBD(sentence, usuario);
                }
            }
        }

        public string fsbValidaConexion()
        {
            if (this.ConexionOracle.ConexionOracleSQL != null && this.ConexionOracle.ConexionOracleSQL.State == System.Data.ConnectionState.Open)
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

            string resultado = cuerpo.ToString().TrimEnd('\r', '\n');

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                File.WriteAllText(saveFileDialog.FileName, resultado, Encoding.Default);
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
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(this.CorreoGeneral);
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
            StringBuilder procedimientos = new StringBuilder();
            StringBuilder xml = new StringBuilder();

            PlantillasHTMLModel plantillaXML = this.listaHtml.Where(x=>x.Nombre == res.XML_HTML).FirstOrDefault();

            xml.AppendLine(plantillaXML.Documentacion);

            InstruccionPL comentarioPkg = archivo.ListaDocumentacionPkg.FirstOrDefault();

            if (comentarioPkg != null)
            {
                string comentario = comentarioPkg.Valor;

                // quitar inicio /* o /*****
                comentario = Regex.Replace(comentario,@"^\s*/\*+\s*$","",RegexOptions.Multiline);

                // quitar final *****...*****/
                comentario = Regex.Replace(comentario,@"^\s*\*+/+\s*$","",RegexOptions.Multiline);

                // quitar líneas decorativas ***** o +++++
                comentario = Regex.Replace(comentario,@"^\s*[\*\+]{3,}\s*$","",RegexOptions.Multiline);

                comentario = comentario.Trim();

                xml.Replace(res.PACKAGE_XML, comentario);
            }
            else
            {
                xml.Replace(res.PACKAGE_XML, "");
            }

            foreach (var docu in archivo.ListDocumentacionOut.Where(x=>x.Token == res.COMMENT_OUT))
            {
                string comentario = docu.Valor;
                bool blFin = false;

                comentario = Regex.Replace(comentario,@"^\s*/\*+","",RegexOptions.Multiline);

                comentario = Regex.Replace(comentario,@"\*+/\s*$","",RegexOptions.Multiline);

                comentario = Regex.Replace(comentario,@"^\s*\*+","",RegexOptions.Multiline);

                comentario = comentario.Trim();

                StringBuilder comentariosIn = new StringBuilder();

                foreach (var docuIn in archivo.ListDocumentacionIn.Where(x=>x.NombreObjeto == docu.NombreObjeto))
                {
                    string com = Regex.Replace(docuIn.Valor, @"^\s*--\s*", "").ToLower();

                    blFin = false;

                    if (com.StartsWith("<com>"))
                    {
                        string lineaIn = docuIn.Valor.Replace("--", "");
                        comentariosIn.AppendLine(lineaIn);

                        if (com.EndsWith("</com>"))
                        {
                            blFin = true;
                        }
                    }

                    if(com.TrimEnd().EndsWith("</com>") && !blFin)
                    {
                        string lineaIn = docuIn.Valor.Replace("--", "");
                        comentariosIn.AppendLine(lineaIn);
                    }
                }

                if(comentariosIn.Length > 0)
                {
                    comentario = Regex.Replace(comentario,@"</Procedure\s*>","<Comentarios>\r\n        [COMENTARIOS_IN]\r\n    </Comentarios>\r\n    </Procedure>",RegexOptions.IgnoreCase);
                    comentario = comentario.Replace("[COMENTARIOS_IN]", comentariosIn.ToString());
                }

                procedimientos.AppendLine(comentario);
            }

            //Si encuentra documentación la procesa
            if (procedimientos.Length > 0)
            {
                xml.Replace(res.PROCEDURE_XML, procedimientos.ToString());
                archivo.DocuXML = xml.ToString();

                return fblGeneraHtml(archivo);
            }          

            return false;
        }

        public Boolean fblGeneraHtml(Archivo archivo)
        {
            bool resultado = false;
            string nombreArchivo = archivo.NombreObjeto + res.ExtensionHtml;
            string nombreArchivoHtml = Path.Combine(archivo.Ruta, nombreArchivo);
            string pathXML = Path.Combine(Environment.CurrentDirectory, res.CarpetaAplTemp);
            string xmlNormalizado = Path.Combine(pathXML, archivo.NombreObjeto+"_"+DateTime.Now.ToString("HHmmss")+".xml");
            archivo.XmlNormalizado = xmlNormalizado;

            try
            {
                PlantillasHTMLModel plantillaXSL = this.listaHtml.Where(x => x.Nombre == res.PLANTILLA_XSL).FirstOrDefault();

                XDocument doc = XDocument.Parse(archivo.DocuXML);

                Normalize(doc.Root);

                doc.Save(xmlNormalizado);

                XslCompiledTransform transform = new XslCompiledTransform();

                // Cargar XSLT
                using (StringReader sr = new StringReader(plantillaXSL.Documentacion))
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    transform.Load(xr);
                }

                string resul;

                using (StringWriter sw = new StringWriter())
                {
                    transform.Transform(xmlNormalizado, null, sw);

                    resul = sw.ToString().TrimEnd('\r', '\n');
                }

                File.WriteAllText(nombreArchivoHtml, resul);

                resultado = true;
            }
            catch (Exception ex) 
            {
                this.CursorNormal();

                if (!File.Exists(xmlNormalizado))
                    File.WriteAllText(xmlNormalizado, archivo.DocuXML, Encoding.UTF8);

                MensajeErrorLink(ex.Message," * ¡VER XML! *", xmlNormalizado);
            }

            return resultado;
        }

        static void Normalize(XElement element)
        {
            // Normalizar nombre nodo
            element.Name = NormalizeName(element.Name.LocalName);

            // Normalizar atributos
            var attrs = element.Attributes().ToList();

            element.RemoveAttributes();

            foreach (var attr in attrs)
            {
                element.SetAttributeValue(
                    NormalizeName(attr.Name.LocalName),
                    attr.Value
                );
            }

            // Hijos
            foreach (var child in element.Elements())
            {
                Normalize(child);
            }
        }

        static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            return name.ToLower();
        }

        public void pObtenerUsuarioCompilacion(string usuario)
        {
            UsuarioModel userCompila = this.ListaUsuarios.Where(x=>x.Usuariobd.Equals(usuario)).FirstOrDefault();

            if (userCompila != null)
            {
                ConnView.Model.UsuarioCompila = userCompila.Usuariobd;
                ConnView.Model.PassCompila = userCompila.Passwordbd;
                ConnView.Model.BdCompila = userCompila.BaseDatos;

                ConexionOracle.RealizarConexionCompilacion();
            }
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

        public void pListaArchivos(string[] DropPath, List<Archivo> archivos,string llamado)
        {
            foreach (string dropfilepath in DropPath)
            {
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dropfilepath)))
                {
                    string[] DropPath1 = System.IO.Directory.GetFiles(dropfilepath + "\\", "*", System.IO.SearchOption.AllDirectories);
                    pListaArchivos(DropPath1, archivos, llamado);
                }
                else
                {
                    this.LoadPath = System.IO.Path.GetDirectoryName(dropfilepath) + "\\";
                    this.SavePath = this.LoadPath;
                    SavePathAplica = LoadPath;
                    PathArchivos = this.SavePathAplica;

                    Archivo archivo = new Archivo();
                    archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                    archivo.RutaConArchivo = dropfilepath;
                    archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                    archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                    archivo.Extension = System.IO.Path.GetExtension(dropfilepath);

                    if(llamado == res.TipoAplica)
                        if (archivo.Extension.ToLower().Equals(res.ExtensionHtml) || archivo.Extension.ToLower().Equals(res.ExtensionLog))
                            continue;

                    archivo.ListaTipos = this.ListaTiposObjetos;
                    archivo.ListaUsuarios = this.ListaUsuarios;

                    if(llamado.Equals(res.GIT))
                        archivo.NombreObjeto = "";
                    else
                        archivo.NombreObjeto = archivo.NombreSinExt;

                    archivo.CarpetaPadre = pObtCarpetaPadre(archivo.RutaConArchivo);

                    this.ObtenerTipoArchivo(archivo);

                    archivo.BloquesCodigo = new List<string>();

                    if (llamado == res.TipoAplica)
                    {
                        if (archivo.Tipo != null && archivo.Tipo == Int32.Parse(res.TipoAplica))
                            continue;

                        if(archivo.Tipo == null)
                            archivo.NombreObjeto = "";
                    }

                    UsuarioModel usuario = archivo.ListaUsuarios.ToList().Where(x => archivo.CarpetaPadre.ToUpper().Contains(x.Usuariobd.Trim().ToUpper())).LastOrDefault();

                    if (usuario != null)
                    {
                        archivo.Usuario = usuario.Usuariobd;
                    }

                    if (archivo.Tipo == null)
                    {
                        if (archivo.Extension.Equals(res.ExtensionHtml))
                        {
                            archivo.Tipo = Int32.Parse(res.TipoObjetoPaquete);
                            archivo.NombreObjeto = archivo.NombreSinExt;
                        }

                        if (archivo.Extension.Equals(res.ExtensionExcel) || archivo.Extension.Equals(res.ExtensionExcelX) || archivo.Extension.Equals(res.ExtensionWord) || archivo.Extension.Equals(res.ExtensionWordX) || archivo.Extension.Equals(res.ExtensionXLSM))
                        {
                            archivo.Tipo = Int32.Parse(res.TipoOtros);
                            archivo.NombreObjeto = "-";
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

        public void pRegeneraIndexListas()
        {
            indexViewModel = new IndexViewModel(this);
        }

        public ObservableCollection<SelectListItem> pObtlistaUsuarios()
        {
            ObservableCollection<SelectListItem> lista = new ObservableCollection<SelectListItem>();

            foreach (UsuarioModel objeto in this.ListaUsuarios)
            {
                lista.Add(new SelectListItem() { Text = objeto.Usuariobd, Value = objeto.Usuariobd });
            }

            return lista;
        }

        private void MensajeErrorLink(string mensaje,string mensajeLink, string archivo)
        {
            var textBlock = new TextBlock
            {
                Margin = new Thickness(10),
                TextWrapping = TextWrapping.Wrap
            };

            textBlock.Inlines.Add(new Run(mensaje+" "));

            var link = new System.Windows.Documents.Hyperlink(new Run(mensajeLink))
            {
                NavigateUri = new Uri(@archivo)
            };

            link.RequestNavigate += (s, e) =>
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
                {
                    UseShellExecute = true
                });
            };

            textBlock.Inlines.Add(link);

            var dialog = new ModernDialog
            {
                Title = "ERROR",
                Content = textBlock
            };

            dialog.Buttons = new[] { dialog.OkButton };

            dialog.ShowDialog();
        }

        #endregion Metodos
    }
}
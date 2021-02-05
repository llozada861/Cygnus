using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Aplica;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Aplica
{
    public class GenerateAplicaViewModel
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _sqlplus;
        private readonly DelegateCommand _examinar;
        private StringBuilder encabezadoAplica;
        private StringBuilder objetosAplica;
        private StringBuilder finAplica;
        private string rutaGeneracion;
        private string cuerpo;
        public ICommand Procesar => _process;
        public ICommand Clean => _clean;
        public ICommand Sqlplus => _sqlplus;
        public ICommand Examinar => _examinar;

        public GenerateAplicaViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _sqlplus = new DelegateCommand(OnSqlplus);
            _examinar = new DelegateCommand(pExaminar);

            handler = hand;
            this.Model = new GenerateAplicaModel(handler);

            this.Model.ListaArchivosGenerados = new ObservableCollection<Archivo>();
            this.Model.ListaArchivosCargados = new ObservableCollection<Archivo>();
            this.Model.ListaUsuarios = handler.ListaUsuarios;
        }

        public GenerateAplicaModel Model { get; set; }

        public void OnProcess(object commandParameter)
        {
            bool Noselec = false;

            try
            {
                if (!this.Model.Datos && !this.Model.Objetos)
                {
                    handler.MensajeError("Selecciona el tipo de entrega Datos u Objetos.");
                    return;
                }

                if (String.IsNullOrEmpty(this.Model.Codigo))
                {
                    handler.MensajeError("Ingrese número de caso");
                    return;
                }

                if (this.Model.Usuario == null)
                {
                    handler.MensajeError("Debe ingresar el usuario para la entrega.");
                    return;
                }

                if (this.Model.ListaArchivosCargados.Count == 0)
                {
                    handler.MensajeError("Se deben tener archivos para procesar.");
                    return;
                }

                foreach (Archivo archivo in this.Model.ListaArchivosCargados)
                {
                    if (string.IsNullOrEmpty(archivo.Tipo))
                    {
                        Noselec = true;
                    }
                }

                if (Noselec)
                {
                    handler.MensajeError("Todos los archivos deben tener un tipo. Seleccione un tipo para el archivo.");
                    return;
                }

                if (this.Model.Objetos && !this.Model.AprobarOrden)
                {
                    handler.MensajeError("Debe aprobar el orden de los archivos para el aplica!");
                    return;
                }

                this.Model.ListaArchivosGenerados.Clear();

                ProcesaArchivos();

                this.Model.ArchivosGenerados = this.Model.ListaArchivosGenerados.Count().ToString();

                if (handler.MensajeConfirmacion("Desea ejecutar el aplica por SQLPLUS?") == "Y")
                {
                    this.OnSqlplus(null);
                }

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
                this.Model.ListaArchivosCargados.Clear();

                if (this.Model.ListaArchivosGenerados.Count() > 0)
                {
                    this.Model.ListaArchivosGenerados.Clear();
                }

                /*if (view.ListaArchivosRepositorio.Count() > 0)
                {
                    view.ListaArchivosRepositorio.Clear();
                }*/

                this.Model.ArchivosCargados = "0";
                this.Model.ArchivosGenerados = "0";
                this.Model.ListaUsuarios = null;
                this.Model.ListaUsuarios = handler.ListaUsuarios;
                this.Model.Objetos = false;
                this.Model.Datos = false;
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void OnSqlplus(object commandParameter)
        {
            Boolean blAplica = false;

            try
            {
                handler.pObtenerUsuarioCompilacion(this.Model.Usuario.Text);

                string credenciales = handler.ConnViewModel.UsuarioCompila + "/" + handler.ConnViewModel.PassCompila + "@" + handler.ConnViewModel.BaseDatos;

                foreach (var process in Process.GetProcessesByName("sqlplus"))
                {
                    process.Kill();
                }

                if (string.IsNullOrEmpty(handler.ConfGeneralViewModel.RutaSqlplus))
                {
                    throw new Exception("No ha configurado la ruta para el Sqlplus. Vaya a Ajustes/General/Rutas.");
                }

                foreach (Archivo archivo in this.Model.ListaArchivosCargados)
                {
                    if (!string.IsNullOrEmpty(archivo.NombreObjeto))
                    {
                        //Valida que el objeto no se encuentra aplicado en más de un esquema
                        handler.DAO.pValidaUsuarioCompila(archivo, handler);
                        //Valida que solo se aplique en un esquema
                        handler.DAO.pValidaObjEsquema(archivo, this.Model.Usuario.Text);
                    }
                }

                foreach (Archivo archivo in this.Model.ListaArchivosGenerados)
                {
                    if (archivo.Tipo.Equals(res.TipoAplica))
                    {
                        blAplica = true;
                        handler.DAO.pExecuteSqlplus(credenciales, archivo);
                    }
                }
            }
            catch(Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pListaArchivos(string[] DropPath)
        {
            string dropfilepath;
            string[] split;

            try
            {
                if (DropPath.Length == 1)
                {
                    dropfilepath = DropPath[0];

                    if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dropfilepath)))
                    {
                        handler.LoadPath = dropfilepath + "\\";
                        handler.SavePath = handler.LoadPath;

                        /*if (handler.Formulario.AplicaCarpetaArriba)
                            handler.SavePathAplica = System.IO.Path.GetDirectoryName(dropfilepath) + "\\";
                        else*/
                        handler.SavePathAplica = handler.LoadPath;

                        handler.PathArchivos = handler.SavePathAplica;
                        split = dropfilepath.Split('\\');
                        handler.CarpetaPadre = split[split.Length - 1];
                        pListaArchivosCarpeta(dropfilepath);
                    }
                }
                else
                {
                    throw new Exception("Solo debe arrastrar la carpeta que contiene los archivos.");
                }

                pRefrescaConteo();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }
        public void pRefrescaConteo()
        {
            this.Model.ArchivosCargados = this.Model.ListaArchivosCargados.Count().ToString();
        }
        public void pExaminar(object commandParameter)
        {
            string[] archivos = handler.pCargarArchivos();
            ListarArchivos(archivos);
        }
        public void ListarArchivos(string[] DropPath)
        {
            List<Archivo> archivos = new List<Archivo>();
            handler.pListaArchivos(DropPath, archivos, res.TipoAplica);

            foreach (Archivo archivo in archivos)
            {
                this.Model.ListaArchivosCargados.Add(archivo);
            }

            pEstablecerOrdenAplica();
        }
        private void pListaArchivosCarpeta(string path)
        {
            string[] DropPath = System.IO.Directory.GetFiles(path + "\\", "*", System.IO.SearchOption.AllDirectories);

            ListarArchivos(DropPath);

            /*foreach (string dropfilepath in DropPath)
            {
                Archivo archivo = new Archivo();
                archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                archivo.RutaConArchivo = dropfilepath;
                archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                archivo.Extension = System.IO.Path.GetExtension(dropfilepath);

                if (archivo.Extension.ToLower().Equals(res.ExtensionHtml) || archivo.Extension.ToLower().Equals(res.ExtensionLog) || archivo.Extension.ToLower().Equals(res.ExtensionZIP))
                    continue;

                archivo.ListaTipos = handler.ListaTiposObjetos;
                handler.ObtenerTipoArchivo(archivo, res.No_aplica);

                if (archivo.Tipo.Equals(res.TipoAplica))
                    continue;

                this.Model.ListaArchivosCargados.Add(archivo);
                archivo.OrdenAplicacion = this.Model.ListaArchivosCargados.IndexOf(archivo);
            }

            pEstablecerOrdenAplica();*/
        }
        internal void ProcesaArchivos()
        {
            foreach (Archivo archivo in this.Model.ListaArchivosCargados)
            {
                if (!string.IsNullOrEmpty(archivo.NombreObjeto))
                {
                    //Valida que el objeto no se encuentra aplicado en más de un esquema
                    handler.DAO.pValidaUsuarioCompila(archivo, handler);
                    //Valida que solo se aplique en un esquema
                    handler.DAO.pValidaObjEsquema(archivo, this.Model.Usuario.Text);
                }

                //Se instancian las listas del archivo
                /*archivo.DocumentacionSinDepurar = new List<StringBuilder>();
                archivo.Modificaciones = new List<ModificacionModel>();
                archivo.ListDocumentacionDepurada = new List<DocumentacionModel>();
                archivo.ObjetoSql = false;
                pObtenerRutaDentroAplica(archivo);

                foreach (SelectListItem tipo in handler.ListaTiposObjetos)
                {
                    //Si el tipo aplica para obtener las modificaciones
                    if (archivo.Tipo.Trim().Equals(tipo.Text))
                    {
                        if (tipo.Value.Equals(res.Si))
                        {
                            //Se lee el archivo
                            pValidaSlashArchivo(archivo);

                            //Se obtiene la documentación del método
                            if (handler.pDepuraDocumentacion(archivo))
                            {
                                pAdicionarArchivo
                                (
                                    archivo.NombreObjeto + res.ExtensionHtml,
                                    res.TipoHtml,
                                    "Documentación HTML",
                                    archivo.Ruta
                                );
                            }

                            //archivo.NombreSinExt = archivo.FileName.Substring(0, archivo.FileName.Length - 4);
                            archivo.ObjetoSql = true;
                        }

                        archivo.OrdenAplicacion = tipo.Prioridad;

                        if (archivo.ObjetoSql) break;
                    }
                }*/
            }

            //Se genera el aplica de los objetos entregados
            pGeneraAplica();
        }

        private void pEstablecerOrdenAplica()
        {
            foreach (Archivo archivo in this.Model.ListaArchivosCargados)
            {
                //Se instancian las listas del archivo
                archivo.DocumentacionSinDepurar = new List<StringBuilder>();
                archivo.Modificaciones = new List<ModificacionModel>();
                archivo.ListDocumentacionDepurada = new List<DocumentacionModel>();
                archivo.ObjetoSql = false;
                pObtenerRutaDentroAplica(archivo);

                foreach (SelectListItem tipo in handler.ListaTiposObjetos)
                {
                    //Si el tipo aplica para obtener las modificaciones
                    if (archivo.Tipo != null && archivo.Tipo.Trim().Equals(tipo.Text))
                    {
                        if (tipo.Value.Equals(res.Si))
                        {
                            //Se lee el archivo
                            pValidaSlashArchivo(archivo);

                            if (tipo.CantidadSlash > 0 && archivo.CantidadSlahs == 0)
                            {
                                archivo.CantidadSlahs = -1;
                            }

                            //Se obtiene la documentación del método
                            if (handler.pDepuraDocumentacion(archivo))
                            {
                                pAdicionarArchivo
                                (
                                    archivo.NombreObjeto + res.ExtensionHtml,
                                    res.TipoHtml,
                                    "Documentación HTML",
                                    archivo.Ruta
                                );
                            }

                            archivo.ObjetoSql = true;
                        }

                        archivo.OrdenAplicacion = tipo.Prioridad;

                        if (archivo.ObjetoSql) break;
                    }
                }
            }

            pGenerarIndices();
         }

        public void pGenerarIndices()
        {
            int nuIndex = 1;

            foreach (Archivo archivo in this.Model.ListaArchivosCargados.OrderBy(x => x.OrdenAplicacion))
            {
                archivo.Index = nuIndex;
                nuIndex++;
            }
        }

        public void pObtenerRutaDentroAplica(Archivo archivo)
        {
            string[] rutas = archivo.Ruta.Split('\\');
            string rutaDentroAplica = "/";
            Boolean rutaPadre = false;

            for (int i = 0; i < rutas.Length; i++)
            {
                if (rutaPadre)
                {
                    rutaDentroAplica = rutaDentroAplica + rutas[i] + "/";
                }

                if (rutas[i].Equals(handler.CarpetaPadre))
                {
                    rutaPadre = true;
                    continue;
                }
            }

            archivo.RutaDentroAplica = rutaDentroAplica;
        }

        private void pGeneraAplica()
        {
            encabezadoAplica = new StringBuilder();
            objetosAplica = new StringBuilder();
            finAplica = new StringBuilder();
            rutaGeneracion = null;
            cuerpo = null;
            string pathBin = "";
            DirectoryInfo di;
            string sourceFile;
            string destFile;

            string nombreAplica = this.Model.Codigo + res.NombreAplica + this.Model.Usuario.Text.Trim() + res.ExtensionSQL;

            /*if (handler.ConfGeneralViewModel.OrdenAutomatico)
            {
                foreach (Archivo archivo in this.Model.ListaArchivosCargados.OrderBy(x => x.OrdenAplicacion))
                {
                    if (archivo.OrdenAplicacion > 0 && !archivo.Tipo.Equals(res.TipoPlantilla))
                    {
                        pInsertaCuerpo(archivo, objetosAplica);
                    }
                }
            }
            else
            {*/

            foreach (Archivo archivo in this.Model.ListaArchivosCargados.OrderBy(x => x.Index))
            {
                if (!archivo.Tipo.Equals(res.TipoPlantilla))
                    pInsertaCuerpo(archivo, objetosAplica);
            }
            

            if (handler.ConfGeneralViewModel.EntregaPlantilla)
            {
                pathBin = Path.Combine(handler.SavePathAplica, "bin");

                if (!Directory.Exists(pathBin))
                {
                    // Try to create the directory.
                    di = Directory.CreateDirectory(pathBin);
                    sourceFile = Path.Combine(handler.RutaBaseDatos, res.ArchivoCopyFileXSLTemplate);
                    destFile = Path.Combine(pathBin, res.ArchivoCopyFileXSLTemplate);
                    handler.pCrearArchivoDesdeFuente(sourceFile, destFile);

                    pAdicionarArchivo
                    (
                        res.ArchivoCopyFileXSLTemplate,
                        res.Script,
                        "Shell para copiar archivos",
                        handler.SavePath
                    );
                }

                sourceFile = Path.Combine(handler.RutaBaseDatos, res.NombreArchivoScript_Regen_XSL);
                destFile = Path.Combine(handler.SavePath, res.NombreArchivoScript_Regen_XSL);
                handler.pCrearArchivoDesdeFuente(sourceFile, destFile);

                encabezadoAplica.Append(res.EncabezadoAplicaPlantilla);

                //Genera los archivos de versión
                /*if (handler.GeneraVersion)
                {
                    pGeneraArchivoVersionIns();
                }*/

                cuerpo = null;
                finAplica.AppendLine(res.CuerpoAplica);

                /*if (this.Formulario.AplicaCarpetaArriba)
                    cuerpo = this.CarpetaPadre + "/" + res.NombreArchivoScript_Regen_XSL;
                else*/
                cuerpo = res.NombreArchivoScript_Regen_XSL;

                finAplica.Replace(res.TagObjetoAplica, cuerpo);
                finAplica.AppendLine();

                pAdicionarArchivo
                (
                    res.NombreArchivoScript_Regen_XSL,
                    res.Script,
                    "Archivo Compila Plantillas",
                    handler.SavePath
                );

                //Genera los archivos de versión
                /*if (this.Formulario.GeneraVersion)
                {
                    pGeneraArchivoVersionUp();
                }*/
            }
            else
            {
                if (this.Model.Datos)
                {
                    encabezadoAplica.Append(res.EncabezadoAplicaDatos);
                    finAplica.Append(res.FinAplicaDatos);
                }
                else
                {
                    encabezadoAplica.Append(res.EncabezadoAplica);
                    finAplica.Append(res.FinAplica);
                }

                //Genera los archivos de versión
                /* if (this.Formulario.GeneraVersion)
                 {
                     pGeneraArchivoVersionIns();
                     pGeneraArchivoVersionUp();
                 }*/

                //Fin aplica

                finAplica.Replace(res.TagNombreAplica, nombreAplica);
            }

            //Genera encabezado            
            encabezadoAplica.Replace(res.TagNombreAplica, nombreAplica);
            encabezadoAplica.Replace(res.Tag_numero_oc, this.Model.Codigo);
            encabezadoAplica.Replace(res.TagGrantUsuario, this.Model.Usuario.Text);
            encabezadoAplica.AppendLine();

            //Genera el archivo con los permisos
            if (handler.ConfGeneralViewModel.Grant)
            {
                pGeneraArchivosPermisos();
                pGeneraArchivosPermisosOA();
            }

            //Si es de datos, se genera el nuevo archivo
            if (this.Model.Datos)
            {
                pGeneraAplicaDatos(nombreAplica);
            }

            rutaGeneracion = Path.Combine(handler.SavePathAplica, nombreAplica);

            using (StreamWriter aplica = new StreamWriter(rutaGeneracion))
            {
                aplica.Write(encabezadoAplica);
                aplica.Write(objetosAplica);
                aplica.Write(finAplica);
            }

            this.pAdicionarArchivo
            (
                nombreAplica,
                res.TipoAplica,
                "Aplica de la entrega",
                handler.SavePathAplica
            );
        }

        public void pInsertaCuerpo(Archivo archivo, StringBuilder objetosAplica)
        {
            cuerpo = null;
            objetosAplica.AppendLine(res.CuerpoAplica);

            /*if (this.Formulario.AplicaCarpetaArriba)
                cuerpo = this.CarpetaPadre + archivo.RutaDentroAplica + archivo.FileName;
            else*/
            cuerpo = archivo.RutaDentroAplica + archivo.FileName;

            objetosAplica.Replace(res.TagObjetoAplica, cuerpo);
            objetosAplica.AppendLine();
        }

        public void pValidaSlashArchivo(Archivo archivo)
        {
            Int64 nuMenosUno = Convert.ToInt64(res.MenosUno);
            string file = Path.Combine(archivo.Ruta, archivo.FileName);
            int cantidadSlash = 0;

            List<string> lines;

            try
            {
                lines = File.ReadAllLines(archivo.RutaConArchivo, Encoding.Default).ToList();

                for (int i = lines.Count - 1; i > (lines.Count - 6); i--)
                {
                    if (i == 0)
                    {
                        break;
                    }

                    if (lines[i].IndexOf('/') > -1)
                    {
                        cantidadSlash++;
                    }
                }

                archivo.CantidadSlahs = cantidadSlash;

                //Se valida los archivos enviados no tienen el slash
                 /*foreach (SelectListItem tipoObjeto in handler.ListaTiposObjetos)
                {
                    if (archivo.Tipo.Trim().Equals(tipoObjeto.Text))
                    {
                        nuTieneSlash = tipoObjeto.CantidadSlash;

                       if (tipoObjeto.CantidadSlash > 0 && archivo.CantidadSlahs == 0)
                        {
                            boTieneSlash = false;
                            //handler.MensajeError("El archivo [" + archivo.NombreObjeto + "] no tiene el / al final. Por favor ajustar.");
                        }*/

                        /*if (archivo.CantidadSlahs < tipoObjeto.CantidadSlash)
                        {
                            //Si no cumple con los slash, los agrega y crea el nuevo archivo.
                            pGenerarSlashArchivo(archivo, tipoObjeto.CantidadSlash);
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void pGenerarSlashArchivo(Archivo archivo, int nuCantidadSlahs)
        {
            string sbLine = "";
            string sbLineSpace = "";
            string LineaAnterior = "";
            int CantidadSlash = 0;
            string file = archivo.RutaConArchivo;
            string nuevoNombre = "new_" + archivo.FileName.Trim();

            StreamWriter nuevoArchivo = new StreamWriter(handler.SavePath + nuevoNombre);

            using (StreamReader streamReader = new StreamReader(file, Encoding.Default))
            {
                sbLine = streamReader.ReadLine();

                while (sbLine != null)
                {
                    //Se eliminan los espacios en blanco
                    sbLineSpace = Regex.Replace(sbLine, @"\s+", " ");

                    if (sbLine.IndexOf('/') > -1 && !(sbLine.IndexOf("/*") > -1) && !(sbLine.IndexOf("*/") > -1))
                    {
                        CantidadSlash++;
                    }

                    if (archivo.Tipo.Trim().Equals(res.TipoObjetoPaquete.ToLower()))
                    {
                        if (sbLineSpace.Trim().ToLower().IndexOf(res.EncabezadoPaqueteBody) > -1 && CantidadSlash == 0)
                        {
                            nuevoArchivo.WriteLine("/");
                            CantidadSlash++;
                        }
                    }

                    nuevoArchivo.WriteLine(sbLine);

                    LineaAnterior = sbLineSpace;
                    sbLine = streamReader.ReadLine();
                }

                if (CantidadSlash != nuCantidadSlahs)
                {
                    nuevoArchivo.WriteLine("/");
                }

                nuevoArchivo.Flush();
                nuevoArchivo.Close();

                pAdicionarArchivo
                (
                    nuevoNombre,
                    archivo.Tipo,
                    "Se crea porque le falta el /",
                    handler.SavePath
                );
            }
        }

        internal void pAdicionarArchivo(string archivoSalida, string tipo, string observacion, string ruta)
        {
            this.Model.ListaArchivosGenerados.Add(new Archivo { FileName = archivoSalida, Tipo = tipo, Observacion = observacion, Ruta = ruta });
        }

        #region Grants
        public void pGeneraArchivosPermisos()
        {
            StringBuilder grant = new StringBuilder();
            string rutaGeneracion;
            string nombreGrant = this.Model.Codigo + res.NombreArchivoGrant + this.Model.Usuario.Text + res.ExtensionSQL;
            rutaGeneracion = Path.Combine(handler.SavePath, nombreGrant);
            string usuarioGrant = "";

            if (handler.ListaUsGrants.Count > 0)
            {
                foreach (Archivo archivo in this.Model.ListaArchivosCargados)
                {
                    foreach (SelectListItem tipo in handler.ListaTiposObjetos)
                    {
                        //Si el tipo aplica para grant
                        if (archivo.Tipo.Equals(tipo.Text) && !tipo.Grant.Equals(res.No))
                        {
                            foreach (SelectListItem item in handler.ListaUsGrants)
                            {
                                usuarioGrant = item.Text;

                                if (usuarioGrant.ToUpper().Equals(this.Model.Usuario.Text.ToUpper()))
                                {
                                    continue;
                                }

                                if (tipo.Grant.Equals(res.TipoGrantSelect))
                                {
                                    grant.AppendLine(res.PlantillaGrant);
                                    grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                    grant.Replace(res.TagGrantPermiso, res.GrantSELECT);
                                    grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                    grant.AppendLine();
                                }

                                //Permisos execute
                                if (tipo.Grant.Equals(res.TipoGrantExecute))
                                {
                                    if (!usuarioGrant.ToUpper().Trim().Equals("CONSULTA_TODAS_LAS_TABLAS"))
                                    {
                                        grant.AppendLine(res.PlantillaGrant);
                                        grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                        grant.Replace(res.TagGrantPermiso, res.GrantEXECUTE);
                                        grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                        grant.AppendLine();
                                    }
                                }

                                if (tipo.Grant.Equals(res.TipoGrantSIUD))
                                {
                                    if (usuarioGrant.ToUpper().Trim().Equals("CONSULTA_TODAS_LAS_TABLAS"))
                                    {
                                        grant.AppendLine(res.PlantillaGrant);
                                        grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                        grant.Replace(res.TagGrantPermiso, res.GrantSELECT);
                                        grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                        grant.AppendLine();
                                    }
                                    else
                                    {
                                        grant.AppendLine(res.PlantillaGrant);
                                        grant.Replace(res.TagGrantUsuario, usuarioGrant);
                                        grant.Replace(res.TagGrantPermiso, res.GrantSIUD);
                                        grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                                        grant.AppendLine();
                                    }
                                }
                            }

                            grant.AppendLine(res.PlantillaSinonimo);
                            grant.Replace(res.TagGrantUsuario, this.Model.Usuario.Text);
                            grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                            grant.AppendLine();
                        }
                    }
                }

                if (grant.Length > 0)
                {
                    using (StreamWriter versionIns = new StreamWriter(rutaGeneracion))
                    {
                        StringBuilder encabezado = new StringBuilder();
                        encabezado.Append(res.EncabezadoAplicaGrant);
                        encabezado.Replace(res.Tag_numero_oc, this.Model.Codigo);
                        versionIns.WriteLine(encabezado);
                        versionIns.Write(grant);
                        versionIns.WriteLine(res.FinAplicaGrant);
                    }

                    Archivo archivoGrant = new Archivo
                    {
                        FileName = nombreGrant,
                        Tipo = res.TipoAplica,
                        Observacion = "Archivo Grant",
                        Ruta = handler.SavePath,
                        RutaDentroAplica = res.Slash
                    };

                    //pInsertaCuerpo(archivoGrant);
                    this.Model.ListaArchivosGenerados.Add(archivoGrant);
                }
            }
        }
        public void pGeneraArchivosPermisosOA()
        {
            StringBuilder grant = new StringBuilder();
            string rutaGeneracion;
            string nombreGrant = "SFOA_" + this.Model.Codigo + res.NombreArchivoGrant + this.Model.Usuario.Text + res.ExtensionSQL;
            rutaGeneracion = Path.Combine(handler.SavePath, nombreGrant);

            foreach (Archivo archivo in this.Model.ListaArchivosCargados)
            {
                foreach (SelectListItem tipo in handler.ListaTiposObjetos)
                {
                    //Si el tipo aplica para grant
                    if (archivo.Tipo.Equals(tipo.Text) && !tipo.Grant.Equals(res.No))
                    {
                        //Permisos execute
                        if (tipo.Grant.Equals(res.TipoGrantExecute))
                        {
                            grant.AppendLine(res.PlantillaGrant);
                            grant.Replace(res.TagGrantUsuario, "EJECUTA_TODOS_LOS_PROC");
                            grant.Replace(res.TagGrantPermiso, res.GrantEXECUTE);
                            grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                            grant.AppendLine();
                        }

                        if (tipo.Grant.Equals(res.TipoGrantSIUD))
                        {
                            grant.AppendLine(res.PlantillaGrant);
                            grant.Replace(res.TagGrantUsuario, "MODIFICA_TODAS_LAS_TABLAS");
                            grant.Replace(res.TagGrantPermiso, res.GrantSIUD);
                            grant.Replace(res.TagGrantObjeto, archivo.NombreObjeto);
                            grant.AppendLine();
                        }

                        grant.AppendLine();
                    }
                }
            }

            if (grant.Length > 0)
            {
                using (StreamWriter versionIns = new StreamWriter(rutaGeneracion))
                {
                    StringBuilder encabezado = new StringBuilder();
                    encabezado.Append(res.EncabezadoAplicaGrant);
                    encabezado.Replace(res.Tag_numero_oc, this.Model.Codigo);
                    versionIns.WriteLine(encabezado);
                    versionIns.Write(grant);
                    versionIns.WriteLine(res.FinAplicaGrant);
                }

                Archivo archivoGrant = new Archivo
                {
                    FileName = nombreGrant,
                    Tipo = res.TipoAplica,
                    Observacion = "Archivo Grant OA",
                    Ruta = handler.SavePath,
                    RutaDentroAplica = res.Slash
                };

                //pInsertaCuerpo(archivoGrant);
                this.Model.ListaArchivosGenerados.Add(archivoGrant);
            }
        }
        public void pGeneraAplicaDatos(string aplica)
        {
            StringBuilder grant = new StringBuilder();
            string rutaGeneracion;
            string nombre = res.NombreAplicaDatos + res.ExtensionSQL;
            rutaGeneracion = Path.Combine(handler.SavePath, nombre);
            StringBuilder objetosApl = new StringBuilder();

            objetosApl.AppendLine(res.CuerpoAplica);
            objetosApl.Replace(res.TagObjetoAplica, "/" + aplica);
            objetosApl.AppendLine();

            using (StreamWriter versionIns = new StreamWriter(rutaGeneracion))
            {
                StringBuilder encabezado = new StringBuilder();
                encabezado.Append(res.EncabezadoAplicaGenDatos);
                encabezado.Replace(res.Tag_numero_oc, this.Model.Codigo);
                encabezado.AppendLine();
                versionIns.WriteLine(encabezado);
                versionIns.Write(objetosApl);
                versionIns.WriteLine(res.FinAplicaGenDatos);
            }

            Archivo archivoGrant = new Archivo
            {
                FileName = nombre,
                Tipo = res.TipoAplica,
                Observacion = "Aplica Datos",
                Ruta = handler.SavePath,
                RutaDentroAplica = res.Slash
            };

            this.Model.ListaArchivosGenerados.Add(archivoGrant);
        }

        #endregion Grants
    }
}

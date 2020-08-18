using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Compila;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Compila
{
    public class CompilaModel
    {
        private Handler handler;
        private CompilaViewModel view;
        public CompilaModel(Handler hand, CompilaViewModel view)
        {
            handler = hand;
            this.view = view;
        }

        public void pListaArchivos(string[] DropPath, string from)
        {
            foreach (string dropfilepath in DropPath)
            {
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dropfilepath)))
                {
                    string[] DropPath1 = System.IO.Directory.GetFiles(dropfilepath + "\\", "*", System.IO.SearchOption.AllDirectories);
                    //pListaArchivosCarpeta(dropfilepath, from);
                    pListaArchivos(DropPath1, from);
                }
                else
                {
                    Archivo archivo = new Archivo();
                    archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                    archivo.RutaConArchivo = dropfilepath;
                    archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                    archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                    archivo.Extension = System.IO.Path.GetExtension(dropfilepath);
                    archivo.ListaTipos = handler.ListaTiposObjetos;
                    ObtenerTipoArchivoComp(archivo);
                    archivo.BloquesCodigo = new List<string>();

                    if (!archivo.Observacion.Equals(res.No_aplica))
                        archivo.TipoAplicacion = res.SQLPLUS;

                    view.ListaArchivosCargados.Add(archivo);
                }
            }

            return;
        }

        private void pListaArchivosCarpeta(string path, string from)
        {
            string[] DropPath = System.IO.Directory.GetFiles(path + "\\", "*", System.IO.SearchOption.AllDirectories);

            if (from.Equals("G"))
            {
                foreach (string dropfilepath in DropPath)
                {
                    Archivo archivo = new Archivo();
                    archivo.FileName = System.IO.Path.GetFileName(dropfilepath);
                    archivo.RutaConArchivo = dropfilepath;
                    archivo.NombreSinExt = System.IO.Path.GetFileNameWithoutExtension(dropfilepath);
                    archivo.Ruta = System.IO.Path.GetDirectoryName(dropfilepath);
                    archivo.Extension = System.IO.Path.GetExtension(dropfilepath);
                    archivo.ListaTipos = handler.ListaTiposObjetos;
                    handler.ObtenerTipoArchivo(archivo);
                    view.ListaArchivosCargados.Add(archivo);
                }
            }
        }

        public void ObtenerTipoArchivoComp(Archivo archivo)
        {
            string sbLine = "";
            string sbLineSpace = "";
            Int64 nuMenosUno = Convert.ToInt64(res.MenosUno);

            string nombreArchivo = archivo.NombreSinExt;
            archivo.Observacion = "";

            if (res.Extensiones.IndexOf(archivo.Extension.ToLower()) > -1)
            {
                using (StreamReader streamReader = new StreamReader(archivo.RutaConArchivo))
                {
                    sbLine = streamReader.ReadLine();

                    while (sbLine != null)
                    {
                        sbLineSpace = Regex.Replace(sbLine, @"\s+", " ");

                        if (sbLineSpace.StartsWith(res.Arroa))
                        {
                            archivo.Tipo = res.TipoAplica;
                            archivo.Observacion = res.No_aplica;
                            break;
                        }

                        archivo.TipoAplicacion = res.SQLPLUS;

                        foreach (SelectListItem prefijo in handler.ListaEncabezadoObjetos.OrderBy(x => x.Prioridad))
                        {
                            if (sbLineSpace.ToLower().IndexOf(prefijo.Text.ToLower()) > nuMenosUno)
                            {
                                archivo.Tipo = prefijo.Value;
                                archivo.SelectItemTipo = prefijo;
                                archivo.NombreObjeto = handler.pObtenerNombreObjeto(sbLineSpace);
                                archivo.FinArchivo = prefijo.Fin.Equals(res.PuntoYComa) ? ";" : prefijo.Fin;
                                archivo.TipoAplicacion = archivo.FinArchivo.Equals(res.END) ? res.SQL : res.SQLPLUS;
                                archivo.InicioArchivo = prefijo.Text.ToLower();
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

            //Sino se encuentra el tipo dentro del archivo
            if (string.IsNullOrEmpty(archivo.Tipo))
            {
                /*foreach (SelectListItem prefijo in handler.ListaTipoArchivos)
                {
                    if (archivo.FileName.ToLower().IndexOf(prefijo.Text.ToLower()) > nuMenosUno)
                    {
                        archivo.Tipo = prefijo.Value;
                        handler.SavePath = archivo.Ruta + "\\";
                        break;
                    }
                }*/

                if (string.IsNullOrEmpty(archivo.NombreObjeto))
                {
                    archivo.NombreObjeto = nombreArchivo;
                }
            }
        }

        public string pObtCantObjsInvalidos()
        {
            return handler.DAO.pObtCantObjsInvalidos();
        }

        public void pCompilarObjetos()
        {
            handler.pObtenerUsuarioCompilacion(view.Usuario.Text);

            foreach (Archivo archivo in view.ListaArchivosCargados.ToList().Where(x => x.TipoAplicacion.Equals(res.SQL)))
            {
                if (archivo.TipoAplicacion != res.SQLPLUS)
                {
                    handler.pObtieneBloquesCodigo(archivo);
                    pCompilaObjetosBD(archivo);
                    handler.pGeneraArchivosPermisosArchivo(archivo, view.Usuario.Text);
                }
            }

            if (handler.ConexionOracle.ConexionOracleCompila.State == System.Data.ConnectionState.Open)
            {
                handler.ConexionOracle.ConexionOracleCompila.Close();
            }

            pExeSqlplus();

            view.ArchivosDescompilados = pObtCantObjsInvalidos();
        }

        public List<string> pSonar(string codigo, string HU)
        {
            List<string> salida = null;
            List<SelectListItem> archivosEvaluar = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(handler.RutaSonar))
            {
                foreach (Archivo archivo in view.ListaArchivosCargados)
                {
                    archivosEvaluar.Add(new SelectListItem { Text = archivo.Ruta, Value = archivo.FileName });
                }

                salida = SonarQube.pEjecutarSonar(codigo, HU, handler.RutaSonar, archivosEvaluar);
            }

            return salida;
        }

        public void pCompilaObjetosBD(Archivo archivo)
        {
            //Valida que el objeto no se encuentra aplicado en más de un esquema
            handler.DAO.pValidaUsuarioCompila(archivo, handler);
            //Valida que solo se aplique en un esquema
            handler.DAO.pValidaObjEsquema(archivo, view.Usuario.Text);

            view.ArchivosCompilados = handler.DAO.pObtCantObjsInvalidos();

            //Se guarda el estado de los objetos compilados antes de la ejecución
            //dao.pGuardaLogCompilacion(archivo, Environment.MachineName, Environment.UserName, stObjetosInvalidosAntes, res.Antes, handler.DatosConexion.UsuarioCompila);
            handler.pGuardaLogCompilacion(archivo, view.ArchivosCompilados, res.Antes);

            for (int i = archivo.BloquesCodigo.Count - 1; i >= 0; i--)
            {
                //Console.WriteLine(archivo.BloquesCodigo.ElementAt(i));
                handler.DAO.pEjecutarScriptBD(archivo.BloquesCodigo.ElementAt(i));
            }

            view.ArchivosDescompilados = handler.DAO.pObtCantObjsInvalidos();

            //dao.pGuardaLogCompilacion(archivo, Environment.MachineName, Environment.UserName, stObjetosInvalidosDespues, res.Despues, handler.DatosConexion.UsuarioCompila);
            handler.pGuardaLogCompilacion(archivo, view.ArchivosDescompilados, res.Despues);
            pObtieneErroresAplicacion(archivo);
        }

        public void pObtieneErroresAplicacion(Archivo archivo)
        {
            handler.DAO.pObtErrores(archivo, view);

            if (!view.ListaObservaciones.ToList().Exists(x => x.Text.Equals(archivo.NombreObjeto)) && string.IsNullOrEmpty(archivo.AplicaTemporal))
                view.ListaObservaciones.Add(new SelectListItem() { Text = archivo.NombreObjeto, Value = "Sin Errores." });
            else
                if (!string.IsNullOrEmpty(archivo.AplicaTemporal))
                view.ListaObservaciones.Add(new SelectListItem() { Text = archivo.AplicaTemporal, Value = "Ver log de la aplicación." });

            view.ListaObservaciones = view.ListaObservaciones;
        }

        public void pExeSqlplus()
        {
            string credenciales;

            if (view.ListaArchivosCargados.Count > 0)
            {
                handler.pObtenerUsuarioCompilacion(view.Usuario.Text);
                credenciales = handler.ConnViewModel.UsuarioCompila + "/" + handler.ConnViewModel.PassCompila + "@" + handler.ConnViewModel.BaseDatos;

                foreach (Archivo archivo in view.ListaArchivosCargados.ToList().Where(x => x.TipoAplicacion.Equals(res.SQLPLUS)))
                {
                    //Valida que el objeto no se encuentra aplicado en más de un esquema
                    handler.DAO.pValidaUsuarioCompila(archivo, handler);
                    //Valida que solo se aplique en un esquema
                    handler.DAO.pValidaObjEsquema(archivo, view.Usuario.Text);

                    handler.pGuardaLogCompilacion(archivo, view.ArchivosCompilados, res.Antes);
                    handler.DAO.pExecuteSqlplus(credenciales, archivo);
                }
            }

            Thread.Sleep(3000);

            foreach (Archivo archivo in view.ListaArchivosCargados.ToList().Where(x => x.TipoAplicacion.Equals(res.SQLPLUS)))
            {
                try
                {
                    pObtieneErroresAplicacion(archivo);
                    handler.pGuardaLogCompilacion(archivo, view.ArchivosDescompilados, res.Despues);
                    handler.pGeneraArchivosPermisosArchivo(archivo, view.Usuario.Text);
                }
                catch { }
            }

            if (Convert.ToInt64(view.ArchivosCompilados) < Convert.ToInt64(view.ArchivosDescompilados))
            {
                throw new Exception("Los scritps aplicados descompilaron objetos en la base de datos. Antes [" + view.ArchivosCompilados + " objetos descompilados]. Ahora [" + view.ArchivosDescompilados + " objetos descompilados]");
            }
        }

        public void pCleanView()
        {
            view.ListaArchivosCargados.Clear();
            view.ListaObservaciones.Clear();
            view.ArchivosCompilados = pObtCantObjsInvalidos();
            view.ArchivosDescompilados = "0";
            view.ListaUsuarios = null;
            view.ListaUsuarios = handler.ListaUsuarios;
            view.EstadoConn = "1";
        }
    }
}

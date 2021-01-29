using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Git;
using Cygnus2_0.Pages.General;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Sonar
{
    public class SonarViewModel
    {
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _buscar;
        private readonly DelegateCommand _limpiar;
        private Handler handler;

        public ICommand Procesar => _process;
        public ICommand Buscar => _buscar;
        public ICommand Limpiar => _limpiar;
        public SonarViewModel(Handler handler)
        {
            this.handler = handler;
            _process = new DelegateCommand(pProcesar);
            _buscar = new DelegateCommand(pBuscar);
            _limpiar = new DelegateCommand(pLimpiar);

            this.GitModel = new ObjectGitModel();

            if(string.IsNullOrEmpty(handler.RutaGitObjetos))
            {
                handler.MensajeAdvertencia("Configure la ruta del repositorio de Git BaseDeDatos en Ajustes/Mvm/Git");
            }
            else
                this.GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(this.handler);

            this.GitModel.ListaHU = handler.DAO.pObtListaHUAzure();
        }

        public ObjectGitModel GitModel { get; set; }

        public void pProcesar(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(GitModel.HU))
                {
                    handler.MensajeError("Debe seleccionar la HU.");
                    return;
                }

                if (GitModel.ListaArchivos.Count == 0)
                {
                    handler.MensajeError("No hay archivos para analizar");
                    return;
                }

                handler.CursorWait();

                List<string> salida = pSonar(GitModel.RamaLBSeleccionada.Text, GitModel.HU);
                System.Console.WriteLine(salida);

                string exito = salida.Find(x => x.IndexOf("ANALYSIS SUCCESSFUL, you can browse") > -1);

                StringBuilder salidaBuild = new StringBuilder();

                foreach (string linea in salida)
                {
                    salidaBuild.AppendLine(linea);
                }

                if (exito != null)
                {
                    string[] vecExito = exito.Split(' ');
                    string url = vecExito[vecExito.Length - 1];
                    Process.Start(url);
                }

                handler.CursorNormal();

                UserControl log = new UserControlLog(salidaBuild);
                WinImage request = new WinImage(log, "Traza");
                request.ShowDialog();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void pBuscar(object commandParameter)
        {
            try
            {
                if (GitModel.RamaLBSeleccionada == null)
                {
                    handler.MensajeError("Seleccione una rama.");
                    return;
                }

                if (string.IsNullOrEmpty(GitModel.ObjetoBuscar))
                {
                    handler.MensajeError("Ingrese el nombre del objeto a buscar.");
                    return;
                }

                handler.CursorWait();

                GitModel.ListaArchivosEncontrados.Clear();
                RepoGit.pSetearLineaBase(GitModel.RamaLBSeleccionada.Text, handler);
                List<Archivo> archivos = new List<Archivo>();
                handler.pListaArchivosCarpeta(handler.RutaGitObjetos, archivos);
                archivos = archivos.FindAll(x => x.FileName.ToUpper().IndexOf(GitModel.ObjetoBuscar.ToUpper()) > -1);

                foreach (Archivo archivo in archivos)
                {
                    GitModel.ListaArchivosEncontrados.Add(archivo);
                }

                handler.CursorNormal();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        public void pLimpiar(object commandParameter)
        {
            GitModel.Codigo = "";
            GitModel.ObjetoBuscar = "";
            GitModel.ListaArchivosEncontrados.Clear();
            GitModel.ListaRamasLB = null;
            if (!string.IsNullOrEmpty(handler.RutaGitObjetos))
                GitModel.ListaRamasLB = RepoGit.pObtieneRamasListLB(handler);
            GitModel.Comentario = "";
            GitModel.HU = "";
            GitModel.ListaArchivos.Clear();
        }

        public List<string> pSonar(string codigo, string HU)
        {
            string rutaRel = "";
            List<string> salida = null;
            List<SelectListItem> archivosEvaluar = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(handler.RutaSonar))
            {
                foreach (Archivo archivo in GitModel.ListaArchivos)
                {
                    int nuIndex = archivo.Ruta.IndexOf(res.CarpetaObjetosGIT);
                    int nuIndex2 = (nuIndex + res.CarpetaObjetosGIT.Length);
                    rutaRel = archivo.Ruta.Substring(nuIndex2, archivo.Ruta.Length- nuIndex2);
                    rutaRel = rutaRel.Replace("\\","/");
                    rutaRel = "." + rutaRel+"/"+ archivo.FileName;

                    archivosEvaluar.Add(new SelectListItem { Text = rutaRel, Value = archivo.FileName });
                }

                salida = SonarQube.pEjecutarSonar(codigo, HU, handler.RutaSonar, archivosEvaluar,handler.RutaGitObjetos);
            }

            return salida;
        }
    }
}

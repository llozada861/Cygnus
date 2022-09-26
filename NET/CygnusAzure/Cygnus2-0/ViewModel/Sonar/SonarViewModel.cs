using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Git;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Pages.General;
using Cygnus2_0.ViewModel.Git;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            GitModel = new ObjectGitModel(new ObjectGitViewModel(handler));
        }

        public ObjectGitModel GitModel { get; set; }
        public ObservableCollection<Repositorio> ListaGit
        {
            get { return handler.RepositorioVM.ListaGit; }
            set { handler.RepositorioVM.ListaGit = value; }
        }

        public Repositorio GitSeleccionado { get; set; }

        public void pProcesar(object commandParameter)
        {
            try
            {
                if (GitModel.ListaArchivos.Count == 0)
                {
                    handler.MensajeError("No hay archivos para analizar");
                    return;
                }

                if (string.IsNullOrEmpty(handler.RutaSonar))
                {
                    handler.MensajeError("Configure Sonar [Ajustes/Herramientas Gestión/Sonar]");
                    return;
                }

                if (string.IsNullOrEmpty(handler.ProyectoSonar))
                {
                    handler.MensajeError("Configure Sonar [Ajustes/Herramientas Gestión/Sonar]");
                    return;
                }

                handler.CursorWait();

                List<string> salida = pSonar(GitModel.RamaLBSeleccionada.Text, handler, GitModel.ListaArchivos,GitSeleccionado);
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
                RepoGit.pRemoverCambiosSonar(handler,GitSeleccionado);
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
            List<Archivo> archivosAux;
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
                RepoGit.pSetearLineaBase(GitModel.RamaLBSeleccionada.Text, GitSeleccionado);
                List<Archivo> archivos = new List<Archivo>();
                handler.pListaArchivosCarpeta(GitSeleccionado.Ruta, archivos);

                string[] objetosBuscar = GitModel.ObjetoBuscar.Trim().Split(',');

                for(int i = 0; i< objetosBuscar.Length; i++)
                {
                    archivosAux = archivos.FindAll(x => x.FileName.ToUpper().IndexOf(objetosBuscar[i].ToUpper()) > -1);

                    foreach (Archivo archivo in archivosAux)
                    {
                        if(!archivo.Extension.Equals(res.ExtensionHtml))
                            GitModel.ListaArchivosEncontrados.Add(archivo);
                    }
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
            if (GitModel == null)
                return;

            GitModel.Codigo = "";
            GitModel.ObjetoBuscar = "";
            GitModel.ListaArchivosEncontrados.Clear();
            GitModel.ListaRamasLB = null;
            GitModel.Comentario = "";
            GitModel.ListaArchivos.Clear();
        }

        public static List<string> pSonar(string codigo,Handler handler, ObservableCollection<Archivo> ListaArchivos, Repositorio repositorioGit)
        {
            string rutaRel = "";
            List<string> salida = null;
            List<SelectListItem> archivosEvaluar = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(handler.RutaSonar) && !string.IsNullOrEmpty(handler.ProyectoSonar))
            {
                foreach (Archivo archivo in ListaArchivos)
                {
                    int nuIndex = archivo.Ruta.IndexOf(repositorioGit.Descripcion);
                    int nuIndex2 = (nuIndex + repositorioGit.Descripcion.Length);
                    rutaRel = archivo.Ruta.Substring(nuIndex2, archivo.Ruta.Length- nuIndex2);
                    rutaRel = rutaRel.Replace("\\","/");
                    rutaRel = "." + rutaRel+"/"+ archivo.FileName;

                    archivosEvaluar.Add(new SelectListItem { Text = rutaRel, Value = archivo.FileName });
                }

                salida = SonarQube.pEjecutarSonar(codigo, handler.RutaSonar, handler.ProyectoSonar, archivosEvaluar, repositorioGit.Ruta);
            }

            return salida;
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Objects
{
    public class BlockModel
    {
        private Handler handler;
        private BlockViewModel view;
        private string objetosBloqueados;
        private string asunto;
        private string body;

        public BlockModel(Handler hand, BlockViewModel view)
        {
            handler = hand;
            this.view = view;
        }

        internal void OnClean()
        {
            view.ListaArchivosBloqueo.Clear();
            view.ListaArchivosEncontrados.Clear();
            view.EstadoConn = "1";
            view.Fecha = DateTime.Now;
            view.Objeto = "";
            view.Codigo = "";
        }

        internal void OnSearch()
        {
            view.ListaArchivosEncontrados.Clear();
            handler.DAO.pObtConsultaObjetos(view.Objeto.Trim(), view);
            pRefrescaConteo();
        }
        internal void pRefrescaConteo()
        {
            view.CantidadObjetos = view.ListaArchivosBloqueo.Count().ToString();
        }

        internal void pAdicionaObjeto(Archivo objeto)
        {
            if (!view.ListaArchivosBloqueo.ToList().Exists(x => (x.Owner.Equals(objeto.Owner) && x.FileName.Equals(objeto.FileName))))
            {
                view.ListaArchivosBloqueo.Add(objeto);
            }

            pRefrescaConteo();
        }

        internal void pBloquearObjetos()
        {
            objetosBloqueados = "";

            foreach (Archivo archivo in view.ListaArchivosBloqueo)
            {
                handler.DAO.pBloqueaObjeto(archivo, view);

                if (view.ListaArchivosBloqueo.IndexOf(archivo) == view.ListaArchivosBloqueo.Count - 1)
                {
                    objetosBloqueados = objetosBloqueados + archivo.FileName;
                }
                else
                {
                    objetosBloqueados = objetosBloqueados + archivo.FileName + ", ";
                }
            }

            if (!string.IsNullOrEmpty(objetosBloqueados))
            {
                if (handler.MensajeConfirmacion("Desea enviar la notificación por correo?") == "Y")
                {
                    asunto = "Bloqueo de Objetos [" + objetosBloqueados + "]";
                    body = "Buen día, <br><br> Se informa que se ha(n) bloqueado el(los) objeto(s) [" + objetosBloqueados + "] " +
                           " con la WO [" + view.Codigo + "], desde la aplicación Cygnus. <br><br> Correo enviado a través de Cygnus.";

                    handler.sendEMailThroughOUTLOOK(asunto, body);
                }
            }
        }
    }
}

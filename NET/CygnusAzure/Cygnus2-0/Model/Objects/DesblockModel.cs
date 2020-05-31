using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Objects
{
    public class DesblockModel
    {
        private Handler handler;
        private DesblockViewModel view;
        private string objetosDesBloqueados;
        private string asunto;
        private string body;
        public DesblockModel(Handler hand, DesblockViewModel view)
        {
            handler = hand;
            this.view = view;
        }

        public void pDesbloqueaObjetos()
        {
            objetosDesBloqueados = "";

            foreach (Archivo archivo in view.ListaArchivosDesbloq)
            {
                pDesbloqueaObjeto(archivo);

                if (view.ListaArchivosDesbloq.IndexOf(archivo) == view.ListaArchivosDesbloq.Count - 1)
                {
                    objetosDesBloqueados = objetosDesBloqueados + archivo.FileName;
                }
                else
                {
                    objetosDesBloqueados = objetosDesBloqueados + archivo.FileName + ", ";
                }
            }
            pLimpiar();
            handler.DAO.pObtObjetosBloqueados(view);

            if (!string.IsNullOrEmpty(objetosDesBloqueados))
            {
                if (handler.MensajeConfirmacion("Desea enviar la notificación por correo?") == "Y")
                {
                    asunto = "Desbloqueo de Objetos [" + objetosDesBloqueados + "]";
                    body = "Buen día, <br><br> Se informa que se ha(n) desbloqueado el(los) objeto(s) [" + objetosDesBloqueados + "] " +
                             "desde la aplicación Cygnus. <br><br> Correo enviado a través de Cygnus.";

                    handler.sendEMailThroughOUTLOOK(asunto, body);
                }
            }
        }

        public void pDesbloqueaObjeto(Archivo archivo)
        {
            handler.DAO.pDesbloqueaObjeto(archivo);
        }

        public void pLimpiar()
        {
            view.ListaArchivosDesbloq.Clear();
            view.ListaArchivosBloqueo.Clear();
            handler.DAO.pObtObjetosBloqueados(view);
            view.EstadoConn = "1";
            pRefrescaConteo();
        }

        public void pRefrescaConteo()
        {
            view.CantidadObjetos = view.ListaArchivosDesbloq.Count().ToString();
        }
        public void pAdicionaObjeto(Archivo objeto)
        {
            if (!view.ListaArchivosDesbloq.ToList().Exists(x => (x.Owner.Equals(objeto.Owner) && x.FileName.Equals(objeto.FileName))))
                view.ListaArchivosDesbloq.Add(objeto);

            pRefrescaConteo();
        }
    }
}

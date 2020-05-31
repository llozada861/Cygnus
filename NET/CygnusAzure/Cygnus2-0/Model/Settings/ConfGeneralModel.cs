using Cygnus2_0.DAO;
using Cygnus2_0.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Settings
{
    public class ConfGeneralModel
    {
        Handler handler;
        public ConfGeneralModel(Handler hand)
        {
            handler = hand;
        }

        public void SaveData()
        {
            SqliteDAO.pCreaConfiguracion(res.KeyOrdenAutomatico, "" + handler.ConfGeneralViewModel.OrdenAutomatico);
            SqliteDAO.pCreaConfiguracion(res.KeyGeneraGrants, "" + handler.ConfGeneralViewModel.Grant);
            SqliteDAO.pCreaConfiguracion(res.KeyProxy, "" + handler.ConfGeneralViewModel.Proxy);

            /*string filename = Path.Combine(handler.RutaBaseDatos, res.NombreArchivoConfiguracion);
            string nombre;

            if (File.Exists((string)filename))
            {
                File.Delete((string)filename);
            }

            nombre = res.NombreArchivoConfiguracion;

            using (StreamWriter tempFile = new StreamWriter(Path.Combine(handler.RutaBaseDatos, nombre)))
            {
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(0).Text + ";" + "false");
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(1).Text + ";" + "false");
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(2).Text + ";" + handler.ConfGeneralViewModel.OrdenAutomatico);
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(3).Text + ";" + handler.ConfGeneralViewModel.Grant);
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(4).Text + ";" + "false");
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(5).Text + ";" + "false");
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(6).Text + ";" + "false");
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(7).Text + ";" + "false");
                tempFile.WriteLine(handler.ListaConfiguracion.ElementAt(8).Text + ";" + "false");
            }

            /*this.Formulario.MantenimientoCorrectivo.Text = Formulario.RutaCopiaRepoMantCorr;
            this.Formulario.MantenimientoCorrectivo.Value = res.CORRECTIVO;
            this.Formulario.MantenimientoCorrectivo.Observacion = res.RutaRepoMantCorrectivo;

            this.Formulario.MantenimientoPreventivo.Text = Formulario.RutaCopiaRepoMantPrev;
            this.Formulario.MantenimientoPreventivo.Value = res.PREVENTIVO;
            this.Formulario.MantenimientoPreventivo.Observacion = res.RutaRepoMantPreventivo;*/


            /*nombre = res.NombreArchivoRutaSqlplus;

            using (StreamWriter tempFile = new StreamWriter(Path.Combine(handler.RutaBaseDatos, nombre)))
            {
                tempFile.WriteLine(handler.ConfGeneralViewModel.RutaSqlplus);
            }*/
        }
    }
}

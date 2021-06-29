using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Model.Settings
{
    public class ConexionModel
    {
        Handler handler;
        public ConexionModel(Handler hand)
        {
            handler = hand;
        }

        public void SaveData(string pass)
        {
            SqliteDAO.pCreaConexion(handler, pass);
            handler.pRealizaConexion();

            /*string filename;
            string nombre;

            filename = Path.Combine(handler.RutaBaseDatos, res.NombreArchivoConexion);

            if (File.Exists((string)filename))
            {
                File.Delete((string)filename);
            }

            nombre = res.NombreArchivoConexion;

            using (StreamWriter tempFile = new StreamWriter(Path.Combine(handler.RutaBaseDatos, nombre)))
            {
                tempFile.WriteLine(handler.ConnViewModel.Usuario + ";" + handler.ConnViewModel.Pass + ";" + handler.ConnViewModel.Servidor + ";" + handler.ConnViewModel.BaseDatos + ";" + handler.ConnViewModel.Puerto);
            }*/
        }

        public void TestConection()
        {
            
        }
    }
}

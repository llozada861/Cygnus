using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.User;
using PlsqlAnalisisDL.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.General
{
    public class Archivo
    {
        public int Index { get; set; }
        public string NombreSinExt { set; get; }
        public int? Tipo { get; set; }
        public string RutaConArchivo { get; set; }
        public string Extension { get; set; }
        public string Ruta { get; set; }
        public string FileName { get; set; }
        public string RutaDentroAplica { get; set; }
        public string NombreObjeto { get; set; }
        public string Owner { get; set; }
        public string XmlNormalizado { get; set; }
        public string InicioArchivo { get; set; }
        public StreamReader StreamArchivo { set; get; }
        public string OrdenCambio { set; get; }
        public List<StringBuilder> DocumentacionSinDepurar { set; get; }
        public List<ModificacionModel> Modificaciones { set; get; }
        public List<InstruccionPL> ListDocumentacionOut { set; get; }
        public List<InstruccionPL> ListDocumentacionIn { set; get; }
        public List<InstruccionPL> ListaDocumentacionPkg { set; get; }
        public List<InstruccionPL> ListaErrores { set; get; }
        public ObservableCollection<TipoObjetos> ListaTipos { set; get; }
        public ObservableCollection<UsuarioModel> ListaUsuarios { set; get; }
        public TipoObjetos SelectItemTipo { set; get; }
        public bool ObjetoSql { get; set; }
        public int CantidadSlahs { get; set; }
        public string Observacion { get; set; }
        public int OrdenAplicacion { get; set; }
        public List<string> BloquesCodigo { get; set; }
        public string FechaEstLib { get; set; }
        public string Usuario { get; set; }
        public string CarpetaPadre { get; set; }
        public string TipoAplicacion { get; set; }
        public string AplicaTemporal { get; set; }
        public string RutaRepo { get; set; }
        public int? Codigo { get; set; }
        public string DocuXML { get; set; }
    }
}

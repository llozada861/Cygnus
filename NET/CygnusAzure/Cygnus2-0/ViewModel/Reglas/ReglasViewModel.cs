using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.Reglas;
using Cygnus2_0.Model.User;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using RadioButton = System.Windows.Controls.RadioButton;

namespace Cygnus2_0.ViewModel.Reglas
{
    public class ReglasViewModel: IViews
    {
        Handler handler;
        private readonly DelegateCommand _process;
        public ICommand Process => _process;
        public ReglasViewModel(Handler handler) 
        {
            Model = new ReglasModel();
            Model.ListaBD = new ObservableCollection<UsuariosPDN>(SqliteDAO.pObtListaBD());
            this.handler = handler;
            Model.ListaReglas = new ObservableCollection<SelectListItem>();
            _process = new DelegateCommand(OnProcess);
            Model.Contador = 1;
        }

        public ReglasModel Model { get; set; }

        public void OnClean(object commandParameter)
        {
        }

        public void OnConection(object commandParameter)
        {
        }

        public void OnProcess(object commandParameter)
        {
            int contador = this.Model.Contador;
            string nombreArchivo = "";
            string fecha = DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year;
            string plantilla = "";
            string stPrefijo;

            var archivos = new List<(string nombreArchivo, string contenido)>();

            RadioButton generar = (RadioButton)commandParameter;

            foreach (SelectListItem item in Model.ListaReglas)
            {
                if (!item.Value.Equals("-99"))
                {
                    handler.CursorWait();

                    try
                    {
                        if (generar.IsChecked == true)
                        {
                            plantilla = handler.DAO.pGeneraRegla(item.Value, this.Model.BdSeleccionada);

                            if (!string.IsNullOrEmpty(item.DocumentoAD))
                                plantilla = plantilla.Replace("--<INSERT_TABLA> o <UPDATE_TABLA>", item.DocumentoAD);

                            nombreArchivo = "ins_" + fecha + "_gr_config_expression_" + contador.ToString().PadLeft(4, '0') + ".sql";
                        }
                        else
                        {
                            plantilla = handler.DAO.pRegeneraRegla(item.Value, this.Model.BdSeleccionada);

                            if (!string.IsNullOrEmpty(item.DocumentoAD))
                                plantilla = plantilla.Replace("--<INSERT_TABLA> o <UPDATE_TABLA>", item.DocumentoAD);

                            nombreArchivo = "up_" + fecha + "_gr_config_expression_" + item.Value + ".sql";
                        }

                        archivos.Add((nombreArchivo, plantilla));
                        contador++;
                    }
                    catch (Exception ex)
                    {
                        handler.CursorNormal();
                        handler.MensajeError(ex.Message);
                        return;
                    }

                }
            }

            handler.CursorNormal();

            if (generar.IsChecked == true)
                stPrefijo = "Generar_";
            else
                stPrefijo = "Regenerar_";

            if (archivos.Count > 1)            
                ExportarStringsEnZip(archivos, stPrefijo + "reglas_" + Model.Contador + "_" + (contador - 1) + ".zip");
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = archivos.First().nombreArchivo;

                handler.CursorNormal();

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    File.WriteAllText(saveFileDialog.FileName, archivos.First().contenido, Encoding.Default);
            }
        }

        private void ExportarStringsEnZip(List<(string nombreArchivo, string contenido)> archivos, string nombreArchivo)
        {
            if (archivos == null || archivos.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
                sfd.Title = "Guardar ZIP";
                sfd.FileName = nombreArchivo;

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        foreach (var archivo in archivos)
                        {
                            if (string.IsNullOrWhiteSpace(archivo.nombreArchivo))
                                continue;

                            var entry = zip.CreateEntry(archivo.nombreArchivo);

                            using (var entryStream = entry.Open())
                            using (var writer = new StreamWriter(entryStream, Encoding.GetEncoding(1252)))
                            {
                                writer.Write(archivo.contenido ?? "");
                            }
                        }
                    }

                    handler.MensajeOk("ZIP generado correctamente.");
                }
                catch (Exception ex)
                {
                    handler.MensajeError("Error al generar ZIP: " + ex.Message);
                }
            }
        }

        internal void pBuscarReglas(string codigo)
        {
            try
            {
                handler.CursorWait();

                if(!string.IsNullOrEmpty(codigo.Trim()))
                    this.Model.ListaReglas.Add(handler.DAO.pObtDatosRegla(codigo.Trim(), this.Model.BdSeleccionada));

                handler.CursorNormal();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        internal string pObtUpdate(List<SelectListItem> listaPrimarias, string Tabla, string ColRegla, string idRegla)
        {
            StringBuilder update = new StringBuilder();

            string sql;
            string primariasSql = "";
            string primariasUp = "";

            try
            {
                handler.CursorWait();

                foreach(SelectListItem item in listaPrimarias)
                {
                    primariasSql = primariasSql + item.Value + ",";
                }

                primariasSql = primariasSql.Substring(0, primariasSql.Length - 1);

                sql = $"select {primariasSql} from {Tabla} where {ColRegla} = {idRegla}";

                List<SelectListItem> valores = handler.DAO.pObtDatosTablaRegla(sql,Model.BdSeleccionada);

                foreach(SelectListItem item in valores)
                {
                    primariasUp = "";

                    foreach (SelectListItem itemCol in item.ListaHijos)
                    {

                        if (itemCol.Value.Any(char.IsLetter))
                            primariasUp = primariasUp + itemCol.Text + " = '" + itemCol.Value + "' AND ";
                        else
                            primariasUp = primariasUp + itemCol.Text + " = " + itemCol.Value + " AND ";

                    }

                    primariasUp = primariasUp.Substring(0, primariasUp.Length - 5);

                    sql = $"UPDATE {Tabla} {Environment.NewLine} SET {ColRegla} = IdConfExpre {Environment.NewLine} WHERE {primariasUp}; {Environment.NewLine}";

                    update.AppendLine(sql);
                }

                handler.CursorNormal();
            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }

            return update.ToString();
        }
    }
}

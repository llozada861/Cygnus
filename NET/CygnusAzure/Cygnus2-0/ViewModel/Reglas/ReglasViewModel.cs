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
            int contador = 1;
            OracleClob clValor;
            string nombreArchivo = "";

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
                            clValor = handler.DAO.pGeneraRegla(item.Value, this.Model.BdSeleccionada);
                            nombreArchivo = "ins_" + DateTime.Now.Day.ToString().PadLeft(2,'0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year + "_gr_config_expression_" + contador.ToString().PadLeft(2,'0') + ".sql";
                        }
                        else
                        {
                            clValor = handler.DAO.pRegeneraRegla(item.Value, this.Model.BdSeleccionada);
                            nombreArchivo = "up_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year + "_gr_config_expression_" + contador.ToString().PadLeft(2, '0') + ".sql";
                        }

                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = nombreArchivo;

                        contador++;

                        handler.CursorNormal();

                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            File.WriteAllText(saveFileDialog.FileName, clValor.Value, Encoding.Default);

                    }
                    catch (Exception ex)
                    {
                        handler.CursorNormal();
                        handler.MensajeError(ex.Message);
                        return;
                    }

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
    }
}

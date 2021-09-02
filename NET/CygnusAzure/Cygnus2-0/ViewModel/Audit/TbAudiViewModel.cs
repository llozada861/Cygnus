using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Audit;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Cygnus2_0.ViewModel.Audit
{
    public class TbAudiViewModel
    {
        private Handler handler;
        private MarkDAO dao;
        private readonly DelegateCommand _process;
        public ICommand Procesar => _process;

        public TbAudiViewModel(Handler handler)
        {
            this.handler = handler;
            this.Model = new TbAuditoriaModel();
            this.Model.Autor = Cygnus2_0.Properties.Cygnus.Default.UserName;
            this.Model.Login = Environment.UserName;
            _process = new DelegateCommand(OnProcess);
            this.dao = new MarkDAO(this.handler);
        }

        public TbAuditoriaModel Model { get; set; }

        public void OnProcess(object commandParameter)
        {
            if (string.IsNullOrEmpty(this.Model.Tabla))
            {
                handler.MensajeError("Ingrese el nombre de la tabla.");
                return;
            }

            if (string.IsNullOrEmpty(this.Model.Primaria))
            {
                handler.MensajeError("Ingrese la llave primaria de la tabla.");
                return;
            }

            if (string.IsNullOrEmpty(this.Model.Ticket))
            {
                handler.MensajeError("Ingrese la HU o la WO.");
                return;
            }

            if (string.IsNullOrEmpty(this.Model.Autor))
            {
                handler.MensajeError("Ingrese el autor.");
                return;
            }

            if (string.IsNullOrEmpty(this.Model.Login))
            {
                handler.MensajeError("Ingrese el login.");
                return;
            }

            try
            {
                handler.CursorWait();

                dao.pGeneraAuditoria(this.Model,out OracleClob tabla, out OracleClob trigger);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "craudit_" + Model.Tabla+ ".sql";
                handler.CursorNormal();

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    File.WriteAllText(saveFileDialog.FileName, tabla.Value, Encoding.Default);

                saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "trg_aud_" + Model.Tabla.ToLower() + ".sql";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    File.WriteAllText(saveFileDialog.FileName, trigger.Value, Encoding.Default);

            }
            catch (Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }
    }
}

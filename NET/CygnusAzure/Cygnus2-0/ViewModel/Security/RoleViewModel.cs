using Cygnus2_0.DAO;
using Cygnus2_0.General;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Security
{
    public class RoleViewModel: IViews
    {
        private Handler handler;
        private readonly DelegateCommand _process;
        private readonly DelegateCommand _clean;
        private readonly DelegateCommand _createUser;
        public ICommand Process => _process;
        public ICommand Clean => _clean;
        public ICommand CreateUser => _createUser;
        public RoleViewModel(Handler hand)
        {
            _process = new DelegateCommand(OnProcess);
            _clean = new DelegateCommand(OnClean);
            _createUser = new DelegateCommand(onCreateUser);

            handler = hand;
            Model = new RoleModel();

            handler.ListaRoles = new List<SelectListItem>();
            handler.ListaRoles.Add(new SelectListItem { Text = "Especialista", Value = "1" });
            handler.ListaRoles.Add(new SelectListItem { Text = "Usuario", Value = "0" });

            OnClean("");
        }

        public RoleModel Model { set; get; }

        public void OnClean(object commandParameter)
        {            
            try
            {
                pLimpiar();
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void OnConection(object commandParameter)
        {
        }
        public void onCreateUser(object commandParameter)
        {
            try
            {
                if(string.IsNullOrEmpty(this.Model.Usuario))
                {
                    handler.MensajeError("Debe ingresar el usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Password))
                {
                    handler.MensajeError("Debe ingresar la contraseña.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Rol.Text))
                {
                    handler.MensajeError("Debe ingresar el rol.");
                    return;
                }

                handler.CursorWait();
                pCreaUsuario();

                Thread.Sleep(5000);

                if (handler.ConexionOracle.ConexionOracleCompila.State == System.Data.ConnectionState.Open)
                {
                    handler.ConexionOracle.ConexionOracleCompila.Close();
                }

                string pass = this.Model.Usuario.Trim() + "-" + this.Model.Rol.Value;
                handler.CursorNormal();

                handler.MensajeOk("Proceso terminó.");
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        private void pCreaUsuario()
        {
            string credenciales;
            string sbNombreAplica;
            string sbAplica;
            StringBuilder sbAplicaBody = new StringBuilder();

            handler.pObtenerUsuarioCompilacion("FLEX");
            credenciales = handler.ConnView.Model.UsuarioCompila + "/" + handler.ConnView.Model.PassCompila + "@" + handler.ConnView.Model.BaseDatos;

            sbNombreAplica = this.Model.Usuario.Trim() + ".sql";
            sbAplica = Path.Combine(handler.PathTempAplica, sbNombreAplica);

            if (File.Exists(sbAplica))
            {
                File.Delete(sbAplica);
            }

            sbAplicaBody.Append(res.ScriptCreaUsuario);
            sbAplicaBody.Replace("[PAR_USUARIO_SQL]", this.Model.Usuario.Trim());
            sbAplicaBody.Replace("[PAR_PASS_SQL]", this.Model.Password);

            using (StreamWriter str = new StreamWriter(sbAplica))
            {
                str.Write(sbAplicaBody.ToString());
            }

            List<Archivo> archivos = new List<Archivo>{ new Archivo { FileName = sbNombreAplica, Ruta = handler.PathTempAplica, Tipo = Int32.Parse(res.TipoAplica) }};
            handler.DAO.pExecuteSqlplus(credenciales, archivos, handler.PathTempAplica, handler.ConnView.Model.UsuarioCompila);            
        }

        public void OnProcess(object commandParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Model.Usuario))
                {
                    handler.MensajeError("Debe ingresar el usuario.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Model.Rol.Text))
                {
                    handler.MensajeError("Debe ingresar el rol.");
                    return;
                }

                pGuardaRol();
                handler.MensajeOk("Rol asignado con éxito.");
            }
            catch (Exception ex)
            {
                handler.MensajeError(ex.Message);
            }
        }

        public void pGuardaRol()
        {
            string pass = this.Model.Usuario.Trim() + "-" + this.Model.Rol.Value;

            if (!string.IsNullOrEmpty(this.Model.Email))
            {
                SqliteDAO.pCreaConfiguracion(res.KeyEmail, this.Model.Email);
                handler.ConnView.Model.Correo = this.Model.Email;
            }
        }

        public void pLimpiar()
        {
            this.Model.Rol = new SelectListItem();
            this.Model.Usuario = "";
            this.Model.ListaRoles = null;
            this.Model.ListaRoles = handler.ListaRoles;
            this.Model.Email = "";
        }
    }
}

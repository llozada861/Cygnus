using Cygnus2_0.General;
using Cygnus2_0.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Model.Security
{
    public class EncryptModel
    {
        private Handler handler;
        private EncryptViewModel view;
        public EncryptModel(Handler hand, EncryptViewModel view)
        {
            handler = hand;
            this.view = view;
        }

        public void pGuardaPass()
        {
            string pass = view.Usuario.Text + "-" + view.Pass + "-" + view.BD;
            handler.DAO.pGuardaPass(view.Usuario.Text, EncriptaPass.Encriptar(pass));
        }
    }
}

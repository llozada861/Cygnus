using Cygnus2_0.General;
using Cygnus2_0.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.ViewModel.Home
{
    public class HomeViewModel : ViewModelBase, IViews
    {
        private Handler handler;
        private bool roleAdmin;
        private bool roleEspecialist;
        private bool roleUser;
        public HomeViewModel(Handler hand)
        {
            handler = hand;
        }
        public bool IsAdmin
        {
            get { return roleAdmin; }
            set { SetProperty(ref roleAdmin, handler.pObtRole(2)); }
        }
        public bool IsEspecialist
        {
            get { return roleEspecialist; }
            set { SetProperty(ref roleEspecialist, handler.pObtRole(1)); }
        }
        public bool IsUser
        {
            get { return roleUser; }
            set { SetProperty(ref roleUser, handler.pObtRole(0)); }
        }
        public void OnClean(object commandParameter)
        {
        }
        public void OnConection(object commandParameter)
        {
        }
        public void OnProcess(object commandParameter)
        {
        }
    }
}

using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.Model.Index;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.ViewModel.Index
{
    public class IndexViewModel: ViewModelBase
    {
        private Handler handler;
        private IndexModel indexModel;

        public IndexViewModel(Handler hand)
        {
            handler = hand;
            indexModel = new IndexModel(handler);
        }
    }
}

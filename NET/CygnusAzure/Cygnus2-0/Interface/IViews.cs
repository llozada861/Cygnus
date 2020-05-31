using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Interface
{
    public interface IViews
    {
        void OnProcess(object commandParameter);
        void OnClean(object commandParameter);
        void OnConection(object commandParameter);
    }
    
}

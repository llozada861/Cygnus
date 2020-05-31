using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.Security
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name, string email, int roles)
        {
            Name = name;
            Email = email;
            Role = roles;
        }

        public string Name { get; private set; }
        public string Email { get; private set; }
        public int Role { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}

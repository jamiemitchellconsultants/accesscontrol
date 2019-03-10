using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Helper
{
    public class AuthorityConfiguration
    {
        public string Authority { get; set; }
        public string AuthorityPort { get; set; }
        public string ApiName { get; set; }
        public string NameClaimType { get; set; }
        public string RoleClaimType { get; set; }
    }
}

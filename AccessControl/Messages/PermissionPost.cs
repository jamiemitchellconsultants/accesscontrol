using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    public class PermissionPost
    {
        public bool Deny { get; set; }
        public string ResourceId { get; set; }
        public string ActionId { get; set; }
    }
}

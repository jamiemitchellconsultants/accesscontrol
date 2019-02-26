using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    public class PermissionDTO
    {
        public bool Deny { get; set; }
        public string ResourceId { get; set; }
        public string ActionId { get; set; }
    }
}

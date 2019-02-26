using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlClient.Models
{
    public class UserPermissionCheck
    {
        public string SubjectId { get; set; }
        public string PermissionId { get; set; }
        public bool Deny { get; set; }
        public string ResourceActionId { get; set; }
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string LocalName { get; set; }
    }

}

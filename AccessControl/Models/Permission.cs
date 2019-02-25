using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Permission
    {
        public string PermissionId { get; set; }
        public string RoleId { get; set; }
        public string ResourceActionId { get; set; }
        public byte Deny { get; set; }

        public virtual Resourceaction ResourceAction { get; set; }
        public virtual Role Role { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Grouprole
    {
        public string GroupRoleId { get; set; }
        public string GroupId { get; set; }
        public string RoleId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Role Role { get; set; }
    }
}

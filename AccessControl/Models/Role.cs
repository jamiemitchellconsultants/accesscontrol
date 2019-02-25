using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Role
    {
        public Role()
        {
            Grouprole = new HashSet<Grouprole>();
            Permission = new HashSet<Permission>();
        }

        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<Grouprole> Grouprole { get; set; }
        public virtual ICollection<Permission> Permission { get; set; }
    }
}

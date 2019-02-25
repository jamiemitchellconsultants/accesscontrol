using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Group
    {
        public Group()
        {
            Grouprole = new HashSet<Grouprole>();
            Usergroup = new HashSet<Usergroup>();
        }

        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<Grouprole> Grouprole { get; set; }
        public virtual ICollection<Usergroup> Usergroup { get; set; }
    }
}

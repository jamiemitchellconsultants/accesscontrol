using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class User
    {
        public User()
        {
            Usergroup = new HashSet<Usergroup>();
        }

        public string UserId { get; set; }
        public string LocalName { get; set; }
        public string SubjectId { get; set; }

        public virtual ICollection<Usergroup> Usergroup { get; set; }
    }
}

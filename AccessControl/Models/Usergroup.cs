using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Usergroup
    {
        public string UserGroupId { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }

        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    }
}

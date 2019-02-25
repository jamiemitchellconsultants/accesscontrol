using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Resourceaction
    {
        public Resourceaction()
        {
            Permission = new HashSet<Permission>();
        }

        public string ResourceActionId { get; set; }
        public string ResourceId { get; set; }
        public string ActionId { get; set; }

        public virtual Action Action { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual ICollection<Permission> Permission { get; set; }
    }
}

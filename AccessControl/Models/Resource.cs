using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Resource
    {
        public Resource()
        {
            Resourceaction = new HashSet<Resourceaction>();
        }

        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ApplicationAreaId { get; set; }

        public virtual Applicationarea ApplicationArea { get; set; }
        public virtual ICollection<Resourceaction> Resourceaction { get; set; }
    }
}

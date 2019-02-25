using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Applicationarea
    {
        public Applicationarea()
        {
            Resource = new HashSet<Resource>();
        }

        public string ApplicationAreaId { get; set; }
        public string ApplicationAreaName { get; set; }

        public virtual ICollection<Resource> Resource { get; set; }
    }
}

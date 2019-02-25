using System;
using System.Collections.Generic;

namespace AccessControl.Models
{
    public partial class Action
    {
        public Action()
        {
            Resourceaction = new HashSet<Resourceaction>();
        }

        public string ActionId { get; set; }
        public string ActionName { get; set; }

        public virtual ICollection<Resourceaction> Resourceaction { get; set; }
    }
}

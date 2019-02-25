using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// An Action creation request
    /// </summary>
    public class ActionPost
    {
        /// <summary>
        /// The name of the action to be created
        /// </summary>
        /// <example>GET</example>
        public string ActionName { get; set; }
    }
}

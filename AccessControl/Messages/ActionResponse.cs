using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// An action in the access control system
    /// </summary>
    public class ActionResponse
    {
        /// <summary>
        /// Unique id of the action within access control
        /// </summary>
        /// <example>12345-213-12312-312</example>
        public string ActionId { get; set; }
        /// <summary>
        /// The unique name of an action within access control
        /// </summary>
        /// <example>GET</example>
        public string ActionName { get; set; }
    }
}

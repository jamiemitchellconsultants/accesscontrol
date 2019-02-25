using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// A role within the access control system
    /// </summary>
    public class RoleResponse
    {
        /// <summary>
        /// Unique id of a role within the access control system
        /// </summary>
        /// <example>432423-25234-1231-132</example>
        public string RoleId { get; set; }
        /// <summary>
        /// Name of a role within the access control system
        /// </summary>
        /// <example>publisher</example>
        public string RoleName { get; set; }
    }
}

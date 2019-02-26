using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// Request to create a role within the access control system
    /// </summary>
    public class RoleDTO
    {
        /// <summary>
        /// The name of the role to be created
        /// </summary>
        /// <example>publisher</example>
        public string RoleName { get; set; }
    }
}

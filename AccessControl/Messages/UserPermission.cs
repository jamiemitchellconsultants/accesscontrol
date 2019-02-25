using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// A permission within the access control system
    /// </summary>
    public class UserPermission
    {
        /// <summary>
        /// unique id of the permission
        /// </summary>
        /// <example>4234234-423423-432423-12323
        /// </example>
        public string PermissionId { get; set; }
        /// <summary>
        /// Id of the resource this permission relates to 
        /// </summary>
        /// <example>423423-42342-42342</example>
        public string ResourceId { get; set; }
        /// <summary>
        /// Id of the action this permission relates to 
        /// </summary>
        /// <example>423424-54353-234223-423</example>
        public string ActionId { get; set; }
        /// <summary>
        /// Name of the resource this permission relates to 
        /// </summary>
        /// <example>user</example>
        public string ResourceName { get; set; }
        /// <summary>
        /// Name of the action this resource relates to
        /// </summary>
        /// <example>GET</example>
        public string ActionName { get; set; }
        /// <summary>
        /// Indication if this is a Deny permission (true) or allow (false)
        /// </summary>
        /// <example>false</example>
        public bool Deny { get; set; }


    }
}

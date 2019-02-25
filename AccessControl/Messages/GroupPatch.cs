using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// For adding/removing users and roles to a group
    /// </summary>
    public class GroupPatch
    {
        /// <summary>
        /// Users in the group
        /// </summary>
        public  string[] AddUserId { get; set; }
        /// <summary>
        /// Roles in the group
        /// </summary>
        public string [] AddRoleId { get; set; }
        /// <summary>
        /// List of users to remove
        /// </summary>
        public string[] RemoveUserId { get; set; }
        /// <summary>
        /// List of roles to unlink
        /// </summary>
        public string[] RemoveRoleId { get; set; }
    }
}

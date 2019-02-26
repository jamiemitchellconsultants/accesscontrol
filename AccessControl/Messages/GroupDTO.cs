using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// A group creation request
    /// </summary>
    public class GroupDTO
    {
        /// <summary>
        /// The name of the group to be created
        /// </summary>
        /// <example>Auditor</example>
        public string GroupName { get; set; }
    }
}

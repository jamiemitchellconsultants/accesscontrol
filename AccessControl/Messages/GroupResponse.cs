using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// A user group within access control
    /// </summary>
    public class GroupResponse
    {
        /// <summary>
        /// The name of the user group
        /// </summary>
        /// <example>Audit</example>
        public string GroupName { get; set; }
        /// <summary>
        /// Unique id of the group within access control
        /// </summary>
        /// <example>12312412-1413-1231-123</example>
        public string GroupId { get; set; }
    }
}

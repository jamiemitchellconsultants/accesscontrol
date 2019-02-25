using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// The result of a permission check
    /// </summary>
    public class PermissionCheckResult
    {
        /// <summary>
        /// True if there is an allow permission, false if no permission or an explicit deny
        /// </summary>
        /// <example>true</example>
        public string Allow { get; set; }
    }
}

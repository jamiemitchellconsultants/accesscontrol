using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// Post to create a resource
    /// </summary>
    public class ResourcePost
    {
        /// <summary>
        /// Name of the resource to be created
        /// </summary>
        /// <example>user</example>
        public string ResourceName { get; set; }
        /// <summary>
        /// Application area of the resource
        /// </summary>
        public string ApplicationAreaId { get; set; }
    }
}

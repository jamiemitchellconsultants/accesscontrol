using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// A resource within the access control system
    /// </summary>
    public class ResourceResponse
    {
        /// <summary>
        /// Unique id of the resource within the access control system
        /// </summary>
        /// <example>12312-432-234523-234</example>
        public string ResourceId { get; set; }
        /// <summary>
        /// The name of a resource
        /// </summary>
        /// <example>user</example>
        public string ResourceName { get; set; }
        /// <summary>
        /// The reference to the application area the role belongs to 
        /// </summary>
        public string ApplicationAreaId { get; set; }
    }
}

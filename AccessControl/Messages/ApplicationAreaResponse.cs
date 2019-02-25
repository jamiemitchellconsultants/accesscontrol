using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// Response to a post or get for application area
    /// </summary>
    public class ApplicationAreaResponse
    {
        /// <summary>
        /// Unique id for an application area
        /// </summary>
        public  string ApplicationAreaId { get; set; }
        /// <summary>
        /// Name of an application area
        /// </summary>
        public string ApplicationAreaName { get; set; }
    }
}

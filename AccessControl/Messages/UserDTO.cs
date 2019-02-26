using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// Payload to create a new user
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Unique id from authentication system
        /// </summary>
        /// <example>123456-1234-1234-123456</example>
        public string SubjectId { get; set; }
        /// <summary>
        /// Name to display for the user
        /// </summary>
        /// <example>Fred Blogs</example>
        public string LocalName { get; set; }
    }
}

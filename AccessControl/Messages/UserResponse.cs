using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// Representation of a user stored in the access control system
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Internal id of the user entity
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Unique reference used by the authentication system
        /// </summary>
        public string SubjectId { get; set; }
        /// <summary>
        /// Display name for the user
        /// </summary>
        public string LocalName { get; set; }
    }
}

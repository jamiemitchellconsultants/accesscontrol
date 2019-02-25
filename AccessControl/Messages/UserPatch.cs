using Microsoft.AspNetCore.JsonPatch;

namespace AccessControl.Messages
{
    /// <summary>
    /// User elements that may be patched
    /// </summary>
    public class UserPatch
    {
        /// <summary>
        /// Unique id from authentication system
        /// </summary>
        /// <example>12312-312321-1231-231</example>
        public string SubjectId { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        /// <example>Fred Blogs</example>
        public string LocalName { get; set; }
    }

}

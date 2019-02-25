using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// A list of permissions associated with a user in the access control system
    /// </summary>
    public class PermissionResponse
    {
        public List< PermissionResponseDetail> Permission { get; set; }
    }

    public class PermissionResponseDetail :UserPermission      
    {
    }
}

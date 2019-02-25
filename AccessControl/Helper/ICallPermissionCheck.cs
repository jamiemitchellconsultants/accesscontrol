using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Models;

namespace AccessControl.Helper
{
    public interface ICallPermissionCheck
    {
        Task<bool> CheckPermission(string subjectId, string resource, string action);
        Task<IQueryable<UserPermissionCheck>> GetPermission(string subjectId);
    }
}

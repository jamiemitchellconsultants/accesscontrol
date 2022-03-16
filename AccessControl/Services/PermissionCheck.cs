using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Helper;
using AccessControl.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionCheck:ICallPermissionCheck
    {
        private AccessControlContext _context;
        private string _permissionSql = " select `user`.`LocalName` AS `LocalName`," +
                                        "`permission`.`PermissionId` AS `PermissionId`," +
                                        "`user`.`SubjectId` AS `SubjectId`," +
                                        "`permission`.`Deny` AS `Deny`," +
                                        "`resourceaction`.`ResourceActionId` AS `ResourceActionId`," +
                                        "`action`.`ActionId` AS `ActionId`," +
                                        "`action`.`ActionName` AS `ActionName`," +
                                        "`resource`.`ResourceId` AS `ResourceId`," +
                                        "`resource`.`ResourceName` AS `ResourceName` " +
                                        " from ((((((((`user` join `usergroup` on" +
                                        "   ((`user`.`UserID` = `usergroup`.`UserId`))) join `group` on" +
                                        "       ((`usergroup`.`GroupId` = `group`.`GroupId`))) join `grouprole` on" +
                                        "           ((`group`.`GroupId` = `grouprole`.`GroupId`))) join `role` on" +
                                        "               ((`grouprole`.`RoleId` = `role`.`RoleId`))) join `permission` on" +
                                        "                   ((`role`.`RoleId` = `permission`.`RoleId`))) join `resourceaction` on" +
                                        "                       ((`permission`.`ResourceActionId` = `resourceaction`.`ResourceActionId`))) join `resource` on" +
                                        "                           ((`resourceaction`.`ResourceId` = `resource`.`ResourceId`))) join `action` on" +
                                        "                               ((`resourceaction`.`ActionId` = `action`.`ActionId`)))";

        public PermissionCheck(AccessControlContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckPermission(string subjectId, string resource, string action)
        {
            var permissionList= _context.Query<UserPermissionCheck>().FromSql(
                $"{_permissionSql} where `user`.`subjectId` = @p0 and `resource`.'resourcename' = @p1 and  `action`.`actionname` = @p2"  ,subjectId,resource,action ).ToList();

            return (!permissionList.Exists(o => o.Deny)) && permissionList.Exists(o => !o.Deny);
        }


        public async Task<IQueryable<UserPermissionCheck>> GetPermission(string subjectId)
        {
            
            var list = _context..Query<UserPermissionCheck>().FromSql( $"{_permissionSql} where `user`.`subjectId` = @p0",subjectId);
            var r = list.ToList();                                   
            return list;
        }
    }
}

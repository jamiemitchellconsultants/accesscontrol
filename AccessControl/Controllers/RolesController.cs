using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AccessControl.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Swashbuckle.AspNetCore.Annotations;

namespace AccessControl.Controllers
{
    /// <summary>
    /// Manage the roles within access control
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AccessControlContext _context;

        /// <summary>
        /// Inject an entity framework db context
        /// </summary>
        /// <param name="context"></param>
        public RolesController(AccessControlContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the roles in the system
        /// </summary>
        /// <returns>List of roles</returns>
        /// <response code="200">List of roles found</response>
        [HttpGet]
        [SwaggerOperation(OperationId = "GetRoles")]
        [ProducesResponseType(typeof(RoleResponse[]),200)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles()
        {
            return Ok( await _context.Role.Select(o=>new RoleResponse{RoleId = o.RoleId,RoleName = o.RoleName}).ToListAsync());
        }


        // GET: api/Roles/5
        /// <summary>
        /// Gets the details of a role withing the access control system
        /// </summary>
        /// <param name="id">Id of the role to retrieve</param>
        /// <returns></returns>
        [HttpGet("{roleId}")]
        [ProducesResponseType(typeof(RoleResponse),200)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<Role>> GetRole(string roleId)
        {
            var role = await _context.Role.FindAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            return Ok( new RoleResponse{RoleId = role.RoleId, RoleName = role.RoleName});
        }

        // POST: api/Roles
        /// <summary>
        /// Adds a role to the system
        /// </summary>
        /// <param name="role">Details of the role to create</param>
        /// <returns>The created role</returns>
        /// <response code="200">The role created</response>
        [HttpPost]
        [ProducesResponseType(200,Type = typeof(RoleResponse))]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<RoleResponse>> CreateRole(RoleDTO role)
        {
            var dbRole= _context.Role.Add(new Role{RoleId = Guid.NewGuid().ToString(),RoleName = role.RoleName});
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RoleExists(dbRole.Entity.RoleId))
                {
                    return Conflict();
                }
                else
                {
                    _context.Entry(dbRole.Entity).State = EntityState.Unchanged;
                    throw;
                }
            }

            return CreatedAtAction("GetRoles", new { id = dbRole.Entity.RoleId }, new RoleResponse{RoleName = dbRole.Entity.RoleName, RoleId = dbRole.Entity.RoleId});
        }

        // DELETE: api/Roles/5
        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="roleId">THe role to delete</param>
        /// <returns></returns>
        /// <response code="200">Role deleted</response>
        /// <response code="400">Role not found</response>
        [HttpDelete("{roleId}")]
        [ProducesResponseType(typeof(RoleResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<Role>> DeleteRole(string roleId)
        {
            var role = await _context.Role.Include(o=>o.Permission).Include(o=>o.Grouprole).FirstOrDefaultAsync(o=>o.RoleId==roleId);
            
            if (role == null)
            {
                return NotFound();
            }

            if (role.Permission.Count!=0  || role.Grouprole.Count!=0) throw new DbUpdateException("Groups or permissions are linked",(Exception) null);

            try {
                _context.Role.Remove(role);
                await _context.SaveChangesAsync();
            } catch
            {
                _context.Entry(role).State = EntityState.Unchanged;
                throw;
            }

            return Ok( new RoleResponse{RoleId = role.RoleId, RoleName = role.RoleName});
        }
        /// <summary>
        /// Adds and allow/deny permission to a role
        /// </summary>
        /// <param name="roleId">the role to create a permission for</param>
        /// <param name="permission">details of the permission to be created</param>
        /// <returns></returns>
        /// <response code="200">Permission created</response>
        /// <response code="404">Role not found</response>
        [HttpPost]
        [Route("{roleId}/permission")]
        [ProducesResponseType(typeof(PermissionResponse),200)]
        [ProducesResponseType(404)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<PermissionResponseDetail>> CreatePermission(string roleId,
            [FromBody] PermissionDTO permission)
        {
            var resourceAction = await _context.Resourceaction.FirstOrDefaultAsync(o =>
                o.ActionId == permission.ActionId && o.ResourceId == permission.ResourceId);
            if (resourceAction == null) return NotFound();
            var dbPermission = _context.Add(new Permission
            {
                Deny = Convert.ToByte( permission.Deny),
                PermissionId = Guid.NewGuid().ToString(),
                ResourceActionId = resourceAction.ResourceActionId,
                RoleId = roleId
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                    _context.Entry(dbPermission.Entity).State = EntityState.Unchanged;
                    throw;
            }

            return Ok(new PermissionResponseDetail
            {
                PermissionId = dbPermission.Entity.PermissionId,
                ActionId = permission.ActionId,
                ActionName = dbPermission.Entity.ResourceAction.Action.ActionName,
                Deny = permission.Deny,
                ResourceId = permission.ResourceId,
                ResourceName = dbPermission.Entity.ResourceAction.Resource.ResourceName
            });
        }
        /// <summary>
        /// Deletes a permission 
        /// </summary>
        /// <param name="roleId">The role to delete the permission from</param>
        /// <param name="permissionId">The permission to delete</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{roleId}/permission/{permissionId}")]
        [ProducesResponseType(typeof(PermissionResponseDetail), 200)]
        [ProducesResponseType(404)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<PermissionResponseDetail>> DeletePermission(string roleId, string permissionId)
        {
            var permission = await _context.Permission.FindAsync(permissionId);
            if (permission == null) return NotFound();
            if (permission.RoleId != roleId) return NotFound();

            var permissionDetail = new PermissionResponseDetail
            {
                ResourceId = permission.ResourceAction.ResourceId,
                ActionId = permission.ResourceAction.ActionId,
                Deny = Convert.ToBoolean( permission.Deny),
                PermissionId = permissionId,
                ActionName = permission.ResourceAction.Action.ActionId,
                ResourceName = permission.ResourceAction.Resource.ResourceName
                
            };
            _context.Permission.Remove(permission);
            await _context.SaveChangesAsync();
            return Ok(permissionDetail);
        }


        private bool RoleExists(string id)
        {
            return _context.Role.Any(e => e.RoleId == id);
        }
    }
}

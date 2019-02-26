using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Helper;
using AccessControl.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using  AccessControl.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace AccessControl.Controllers
{
    /// <summary>
    /// Manage users within the access control system
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AccessControlContext _context;
        private readonly PermissionCheck _permission;


        /// <summary>
        /// inject a db context for entities and one for permissions view
        /// </summary>
        /// <param name="context">Database entities</param>
        /// <param name="permissionCheck">Permissions view</param>
        public UsersController(AccessControlContext context,PermissionCheck permissionCheck)
        {
            _context = context;
            _permission = permissionCheck;
        }

        // GET: api/Users
        /// <summary>
        /// Get all the users in access control
        /// </summary>
        /// <returns>A list of users</returns>
        /// <response code="200">Users in access control</response>
        [HttpGet]
        [ExternalPermission("user","GET")]
        [SwaggerOperation(OperationId = "GetUsers")]
        [ProducesResponseType(200,Type=typeof(UserResponse[]))]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _context.User.Select(o => new UserResponse
                {LocalName = o.LocalName, SubjectId = o.SubjectId, UserId = o.UserId}).ToArrayAsync());
        }

        /// <summary>
        /// Gets a specific user
        /// </summary>
        /// <param name="subjectId">Use subject id from authentication to find user</param>
        /// <returns>User details</returns>
        /// <response code="400">User not found</response>
        // GET: api/Users/5
        [HttpGet("{subjectId}")]
        [ProducesResponseType(200,Type = typeof(UserResponse))]
        [ProducesResponseType(404)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<User>> GetUser(string subjectId)
        {
            var user = await _context.User.FirstOrDefaultAsync(o=>o.SubjectId==subjectId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserResponse
            {
                UserId = user.UserId,LocalName = user.LocalName,SubjectId = user.SubjectId

            });
        }
        /// <summary>
        /// Gets permissions assigned to user via group/role
        /// </summary>
        /// <param name="subjectId">The user to get permsssions of</param>
        /// <returns>List of permissions</returns>
        /// <response code="400">User not found</response>
        [HttpGet]
        [Route("{subjectId}/permission")]
        [ProducesResponseType(200, Type = typeof(PermissionResponse))]
        [ProducesResponseType(404)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<PermissionResponse>> GetUserPermissions(string subjectId)
        {
            var permissionResponse = new PermissionResponse {Permission = new List<PermissionResponseDetail>()};

            var user = await _context.User.FirstOrDefaultAsync(o => o.SubjectId == subjectId);
            if (user == null) return NotFound();

            var data = await _permission.GetPermission(subjectId);
            var dataList = await data.ToListAsync();
            var dataresult = dataList.Where(o =>
                    !dataList.Exists(p => p.Deny && p.ActionId == o.ActionId && p.ResourceId == o.ResourceId))
                .GroupBy(x => x.PermissionId).Select(y => y.First()).ToList();
            var dataOk = dataresult.Select(r => new PermissionResponseDetail
            {
                ActionId = r.ActionId,
                ActionName = r.ActionName,
                PermissionId = r.PermissionId,
                ResourceId = r.ResourceId,
                ResourceName = r.ResourceName,
                Deny = r.Deny
            }).ToList();
            return Ok(new PermissionResponse {Permission = dataOk});
        }

        /// <summary>
        /// Checks to see if user has a specific permission
        /// </summary>
        /// <param name="subjectId">Unique id from authentication system</param>
        /// <param name="resource">The resource to be checked</param>
        /// <param name="action">The action to be checked</param>
        /// <response code="200">Permission Result</response>
        [HttpGet]
        [Route("{subjectId}/permission/{resource}/{action}")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(200,Type= typeof(PermissionCheckResult))]
        public async Task<ActionResult<PermissionCheckResult>> GetUserPermissionCheck(string subjectId, string resource,
            string action)
        {
            var permission = await _permission.CheckPermission(subjectId, resource, action);
            return Ok(new PermissionCheckResult { Allow = permission.ToString() });

        }
        // POST: api/Users
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">The details of the user to be created</param>
        /// <returns>Representation of the created user</returns>
        /// <response code="201">User Created</response>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UserResponse))]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType((int)StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> CreateUser(UserDTO user)
        {
            var dbUser = _context.User.Add(new User { UserId = Guid.NewGuid().ToString(), LocalName = user.LocalName, SubjectId = user.SubjectId});
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(dbUser.Entity.UserId))
                {
                    return Conflict();
                }
                else
                {
                    _context.Entry(dbUser.Entity).State = EntityState.Unchanged;

                    throw;
                }
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUsers", new { id = dbUser.Entity.UserId }, new UserResponse{LocalName = user.LocalName,SubjectId = user.SubjectId,UserId = dbUser.Entity.UserId});
        }

        /// <summary>
        /// Edits a user entity
        /// </summary>
        /// <param name="subjectId">Unique id from authentication system to identify user</param>
        /// <param name="userPatch">Patch actions to apply</param>
        /// <response code="404">Subject Id not found</response>
        /// <response code="200">Patch applied</response>
        [HttpPatch("{subjectId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<IActionResult> PatchUser(string subjectId, [FromBody] JsonPatchDocument<UserPatch> userPatch)
        {
            var dbUser = await _context.User.FirstOrDefaultAsync(o => o.SubjectId == subjectId);
            if (dbUser == null) return NotFound();
            var messageUser = new UserPatch {LocalName = dbUser.LocalName, SubjectId = dbUser.SubjectId};
            userPatch.ApplyTo(messageUser);
            dbUser.SubjectId = messageUser.SubjectId;
            dbUser.LocalName = messageUser.LocalName;
            await _context.SaveChangesAsync();
            return Ok();
        }
        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}

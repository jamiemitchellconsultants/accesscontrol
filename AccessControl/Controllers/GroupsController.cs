using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace AccessControl.Controllers
{
    /// <summary>
    /// Manage the user groups within access control
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly AccessControlContext _context;

        /// <summary>
        /// Pass db context to constructor
        /// </summary>
        /// <param name="context">Entity framework db context</param>
        public GroupsController(AccessControlContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of groups in the system
        /// </summary>
        /// <returns>List of groups</returns>
        // GET: api/Groups
        [HttpGet]
        [ProducesResponseType(typeof(GroupResponse[]), 200)]
        public async Task<ActionResult<IEnumerable<GroupResponse>>> GetGroup()
        {
            return Ok(await _context.Group.Select(o => new GroupResponse {GroupId = o.GroupId, GroupName = o.GroupName})
                .ToListAsync());
        }

        /// <summary>
        /// Gets a group defined within access control
        /// </summary>
        /// <param name="groupId">Id of the group required</param>
        /// <returns>THe group with the supplied groupId</returns>
        /// <response code="400">Group not found</response>
        // GET: api/Groups/5
        [HttpGet("{groupId}")]
        [ProducesResponseType(typeof(GroupResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<GroupResponse>> GetGroup(string groupId)
        {
            var dbGroup = await _context.Group.FindAsync(groupId);

            if (dbGroup == null)
            {
                return NotFound();
            }

            return Ok(new GroupResponse {GroupId = dbGroup.GroupId, GroupName = dbGroup.GroupName});
        }

        /// <summary>
        /// Gets the users in a group
        /// </summary>
        /// <param name="groupId">The groupId of the group to get users for</param>
        /// <returns>List of users in the specified group</returns>
        /// <response code="400">Group not found</response>
        /// <response code="200">Users</response>
        [HttpGet()]
        [Route("{groupId}/user")]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(UserResponse[]), 200)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetGroupUsers(string groupId)
        {
            var group = await _context.Group.FindAsync(groupId);
            return Ok(
                group.Usergroup.Select(o => new UserResponse
                    {LocalName = o.User.LocalName, SubjectId = o.User.SubjectId, UserId = o.UserId})
            );
        }

        /// <summary>
        /// Gets the roles in a group
        /// </summary>
        /// <param name="groupId">The id of the group to get roles from</param>
        /// <returns>List of roles linked to the specified group</returns>
        /// <response code="400">Group not found</response>
        /// <response code="200">List of roles</response>
        [HttpGet]
        [Route("{groupId}/role")]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(RoleResponse), 200)]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetGroupRoles(string groupId)
        {
            var group = await _context.Group.FindAsync(groupId);
            return Ok(group.Grouprole.Select(o => new RoleResponse {RoleName = o.Role.RoleName, RoleId = o.RoleId}));
        }

        // POST: api/Groups
        /// <summary>
        /// Adds a group to the access control system
        /// </summary>
        /// <param name="group">Detail of the group to be created</param>
        /// <returns>The created group</returns>
        /// <response code="201">Created group</response>
        [HttpPost]
        [ProducesResponseType(typeof(GroupResponse), 201)]
        public async Task<ActionResult<Group>> PostGroup(GroupPost group)
        {
            var dbGroup = await _context.Group.AddAsync(new Group
                {GroupId = Guid.NewGuid().ToString(), GroupName = @group.GroupName});
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GroupExists(dbGroup.Entity.GroupId))
                {
                    return Conflict();
                }
                else
                {
                    _context.Entry(dbGroup.Entity).State = EntityState.Unchanged;

                    throw;
                }
            }

            return CreatedAtAction("GetGroup", new {id = dbGroup.Entity.GroupId},
                new GroupResponse {GroupId = dbGroup.Entity.GroupId, GroupName = dbGroup.Entity.GroupName});
        }

        /// <summary>
        /// uses patch to add/remove users and roles for a group
        /// </summary>
        /// <param name="groupId">Group to apply the patch to </param>
        /// <param name="groupPatch">the patch document</param>
        /// <returns>200</returns>
        [HttpPatch("groupId")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PatchGroup(string groupId, [FromBody] JsonPatchDocument<GroupPatch> groupPatch)
        {
            var group = await _context.Group.Include(o => o.Grouprole).Include(o => o.Usergroup)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
            foreach (var groupPatchOperation in groupPatch.Operations.FindAll(o => o.op == "add"))
            {

                if (groupPatchOperation.path.ToLower().StartsWith("/adduserid"))
                {
                    var idsToAdd = new List<string>();
                    if (groupPatchOperation.path.ToLower().EndsWith("-"))
                    {
                        idsToAdd.Add(groupPatchOperation.value.ToString());
                    }
                    else
                    {
                        idsToAdd.AddRange(groupPatchOperation.value as string[]);
                    }

                    foreach (var idToAdd in idsToAdd)
                    {
                        var createdUserGroup = await _context.Usergroup.AddAsync(new Usergroup
                            {UserGroupId = Guid.NewGuid().ToString(), GroupId = group.GroupId, UserId = idToAdd});
                        group.Usergroup.Add(createdUserGroup.Entity);

                    }

                    //add a user to a group
                }
                else if (groupPatchOperation.path.ToLower().StartsWith("/addroleid"))
                {
                    //add a role
                    var idsToAdd = new List<string>();
                    if (groupPatchOperation.path.ToLower().EndsWith("-"))
                    {
                        idsToAdd.Add(groupPatchOperation.value.ToString());
                    }
                    else
                    {
                        idsToAdd.AddRange(groupPatchOperation.value as string[]);
                    }

                    foreach (var idToAdd in idsToAdd)
                    {
                        var createdRoleGroup = await _context.Grouprole.AddAsync(new Grouprole
                            {GroupRoleId = Guid.NewGuid().ToString(), GroupId = group.GroupId, RoleId = idToAdd});
                        group.Grouprole.Add(createdRoleGroup.Entity);

                    }
                }
                else if (groupPatchOperation.path.ToLower().StartsWith("/removeuserid"))
                {
                    var idsToAdd = new List<string>();
                    if (groupPatchOperation.path.ToLower().EndsWith("-"))
                    {
                        idsToAdd.Add(groupPatchOperation.value.ToString());
                    }
                    else
                    {
                        idsToAdd.AddRange(groupPatchOperation.value as string[]);
                    }

                    foreach (var idToAdd in idsToAdd)
                    {
                        var foundUser = await _context.Usergroup.FirstOrDefaultAsync(o => o.UserId == idToAdd);
                        if (foundUser != null)
                        {
                            _context.Usergroup.Remove(foundUser);

                        }
                    }

                }
                else if (groupPatchOperation.path.ToLower().StartsWith("/removeroleid"))
                {
                    var idsToAdd = new List<string>();
                    if (groupPatchOperation.path.ToLower().EndsWith("-"))
                    {
                        idsToAdd.Add(groupPatchOperation.value.ToString());
                    }
                    else
                    {
                        idsToAdd.AddRange(groupPatchOperation.value as string[]);
                    }

                    foreach (var idToAdd in idsToAdd)
                    {
                        var foundRole = await _context.Grouprole.FirstOrDefaultAsync(o => o.RoleId == idToAdd);
                        if (foundRole != null)
                        {
                            _context.Grouprole.Remove(foundRole);

                        }
                    }
                }

                await _context.SaveChangesAsync();

            }

            return Ok();
        }

        private bool GroupExists(string id)
        {
            return _context.Group.Any(e => e.GroupId == id);
        }
    }
}


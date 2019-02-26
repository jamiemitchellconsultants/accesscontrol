using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AccessControl.Helper;
using AccessControl.Messages;
using AccessControl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using Swashbuckle.AspNetCore.Annotations;

namespace AccessControl.Controllers
{
    /// <summary>
    /// External api to allow clients to check for user permissions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionCheckController : ControllerBase
    {
        private ICallPermissionCheck _context;
        private AccessControlContext _dbContext;
        /// <summary>
        /// Inject database context and permission checker
        /// </summary>
        /// <param name="permissionChecker"></param>
        /// <param name="context"></param>
        public PermissionCheckController(ICallPermissionCheck permissionChecker, AccessControlContext context)
        {
            _context = permissionChecker;
            _dbContext = context;
        }
        // GET: api/PermissionCheck
        /// <summary>
        /// Get a list of permission for the identity
        /// </summary>
        /// <param name="JWTIssuerId">Id of the registered JWT issuer</param>
        /// <returns>List of permissions</returns>
        [HttpGet("{JWTIssuerId}")]
        //[AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Messages.UserPermission>))]
        [SwaggerOperation(OperationId = "GetAllPermissions")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<IEnumerable<Messages.UserPermission>>> GetUserPermissions(string JWTIssuerId )
        {
            var issuer =await  _dbContext.Jwtissuer.FindAsync(JWTIssuerId);
            var subjectIdClaimName = issuer.SubjectClaimName;
            var subjectId = int.TryParse(subjectIdClaimName, out int claimNo) ? User.Claims.Skip(claimNo).FirstOrDefault()?.Value : User.Claims.FirstOrDefault(o => o.Type == subjectIdClaimName)?.Value;
            var data =await  _context.GetPermission(subjectId);
            var dataList= data.ToList();
            return Ok( dataList.Where(o => !dataList.Exists(p => p.Deny && p.ActionId == o.ActionId && p.ResourceId == o.ResourceId))
                .GroupBy( x => x.PermissionId).Select(y => y.First()).Select(r => new Messages.UserPermission
                {
                    ActionId = r.ActionId,
                    ActionName = r.ActionName,
                    PermissionId = r.PermissionId,
                    ResourceId = r.ResourceId,
                    ResourceName = r.ResourceName,
                    Deny = r.Deny
                }));
        }
        /// <summary>
        /// Determine if an identity has a permission
        /// </summary>
        /// <param name="resource">Resoruce to check. e.g. user</param>
        /// <param name="action">Action to check. e.g. GET</param>
        /// <param name="JWTIssuerId">Name of the claim of subjectId</param>
        /// <returns></returns>
        [AllowAnonymous]
        // GET: api/PermissionCheck/5
        [HttpGet("{resource}/{action}")]
        [ProducesResponseType(200,Type = typeof(PermissionCheckResult))]
        [SwaggerOperation(OperationId = "CheckPermission")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<PermissionCheckResult>> Get(string resource,string action,[FromQuery] string JWTIssuerId = "")
        {
            var issuer = await _dbContext.Jwtissuer.FindAsync(JWTIssuerId);
            var subjectIdClaimName = issuer.SubjectClaimName;

            var subjectId = int.TryParse(subjectIdClaimName, out int claimNo) ? User.Claims.Skip(claimNo).FirstOrDefault()?.Value : User.Claims.FirstOrDefault(o => o.Type == subjectIdClaimName)?.Value;
            var result = await _context.CheckPermission(subjectId, resource, action);
            return Ok( new PermissionCheckResult{ Allow = result.ToString()});
        }

    }
}

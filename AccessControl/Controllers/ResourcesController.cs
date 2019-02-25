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

namespace AccessControl.Controllers
{
    /// <summary>
    /// Manages named resources in access control
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly AccessControlContext _context;
        /// <summary>
        /// Constructor accepting an entity framework db context
        /// </summary>
        /// <param name="context"></param>
        public ResourcesController(AccessControlContext context)
        {
            _context = context;
        }

        // GET: api/Resources
        /// <summary>
        /// Gets a list of resources registered within access control
        /// </summary>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(200,Type=typeof(ResourceResponse[]))]
        public async Task<ActionResult<IEnumerable<ResourceResponse>>> GetResource()
        {
            return Ok( await _context.Resource.Select(o=>new ResourceResponse{ResourceId = o.ResourceId,ResourceName = o.ResourceName, ApplicationAreaId = o.ApplicationAreaId}).ToListAsync());
        }

        // GET: api/Resources/5
        /// <summary>
        /// Gets details of a single resource
        /// </summary>
        /// <param name="resourceId">the id of the resource to be returned</param>
        /// <returns></returns>
        [HttpGet("{resourceId}")]
        [ProducesResponseType(200, Type=typeof(ResourceResponse))]
        public async Task<ActionResult<ResourceResponse>> GetResource(string resourceId)
        {
            var resource = await _context.Resource.FindAsync(resourceId);

            if (resource == null)
            {
                return NotFound();
            }

            return  Ok(new ResourceResponse{ ResourceId = resource.ResourceId, ResourceName = resource.ResourceName, ApplicationAreaId = resource.ApplicationAreaId}) ;
        }

        // POST: api/Resources
        /// <summary>
        /// Create a new resource within access control
        /// </summary>
        /// <param name="resource">The resource to be created</param>
        /// <response code="201">Resource created</response>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ResourceResponse))]
        public async Task<ActionResult<Resource>> PostResource(ResourcePost resource)
        {
            var dbResource = _context.Resource.Add(new Resource {ResourceId  = Guid.NewGuid().ToString(), ResourceName = resource.ResourceName, ApplicationAreaId = resource.ApplicationAreaId});
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ResourceExists(dbResource.Entity.ResourceId))
                {
                    return Conflict();
                }
                else
                {
                    _context.Entry(dbResource.Entity).State = EntityState.Unchanged;

                    throw;
                }
            }
            
            return CreatedAtAction("GetResource", new { id = dbResource.Entity.ResourceId }, new ResourceResponse{ResourceId = dbResource.Entity.ResourceId,ResourceName = dbResource.Entity.ResourceName});
        }

        /// <summary>
        /// Updates a resource within the system to assign / remove actions
        /// </summary>
        /// <param name="resourceId">The id of the resource to edit</param>
        /// <param name="resurcePatch"></param>
        /// <response code="200">Patch successful</response>
        [HttpPatch("{resourceId}")]
        public async Task<IActionResult> PatchResource(string resourceId,
            [FromBody] JsonPatchDocument<ResourcePatch> resurcePatch)
        {
            var resource =await  _context.Resource.Include(o => o.Resourceaction)
                .FirstOrDefaultAsync(p => p.ResourceId == resourceId);
            foreach (var resurcePatchOperation in resurcePatch.Operations.FindAll(o=>o.op=="add" && o.path.ToLower()=="/actionid/-"))
            {
                _context.Resourceaction.Add(new Resourceaction
                    {ResourceActionId = Guid.NewGuid().ToString(), ActionId = resurcePatchOperation.value.ToString(), Resource = resource});
            }
            
            foreach (var operation in resurcePatch.Operations.FindAll(o=>o.op=="add"&&o.path.ToLower()== "/deleteresourceactionid/-"))
            {
                _context.Resourceaction.Remove(_context.Resourceaction.Find(operation.value.ToString()));
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Resources/5
        /// <summary>
        /// Delete a resource, unlink actions, remove permissions
        /// </summary>
        /// <param name="resourceId">Id of resource to delete</param>
        [HttpDelete("{resourceId}")]
        [ProducesResponseType(204,Type = typeof(ResourceResponse))]
        public async Task<ActionResult<ResourceResponse>> DeleteResource(string resourceId)
        {
            var resource = await _context.Resource.FindAsync(resourceId);
            if (resource == null)
            {
                return NotFound();
            }

            resource = await _context.Resource.Include(p => p.Resourceaction)
                .FirstOrDefaultAsync(o => o.ResourceId == resourceId);

            foreach (var resourceAction in resource.Resourceaction)
            {
                var resourceActionDelete =await _context.Resourceaction.Include(p => p.Permission)
                    .FirstOrDefaultAsync(o => o.ResourceActionId == resourceAction.ResourceActionId);
                foreach (var permission in resourceActionDelete.Permission)
                {
                    _context.Permission.Remove(permission);
                }

                _context.Resourceaction.Remove(resourceAction);
            }

            _context.Resource.Remove(resource);

            await _context.SaveChangesAsync();

            return Ok(  new ResourceResponse{ResourceName = resource.ResourceName,ResourceId = resource.ResourceId});
        }

       

        private bool ResourceExists(string id)
        {
            return _context.Resource.Any(e => e.ResourceId == id);
        }
    }
}

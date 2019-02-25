
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;


namespace AccessControl.Controllers
{
    /// <summary>
    /// Manage the ApplicationArea entities
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationAreasController : ControllerBase
    {
        private readonly AccessControlContext _context;
        /// <summary>
        /// Pass databse context
        /// </summary>
        /// <param name="context">Entity framework DbContext</param>
        public ApplicationAreasController(AccessControlContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all ApplicationAreas in access control
        /// </summary>
        /// <returns>A list of ApplicationArea entities</returns>
        // GET: api/ApplicationAreas
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationAreaResponse[]))]
        public async Task<ActionResult<IEnumerable<ApplicationAreaResponse>>> GetApplicationArea()
        {
            return Ok(await _context.Applicationarea.Select(o => new ApplicationAreaResponse { ApplicationAreaId = o.ApplicationAreaId, ApplicationAreaName = o.ApplicationAreaName }).ToListAsync());
        }

        // GET: api/ApplicationAreas/5
        /// <summary>
        /// Get a single ApplicationArea from access control
        /// </summary>
        /// <param name="id">The id of the ApplicationArea required</param>
        /// <returns>The details of the ApplicationArea</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ApplicationAreaResponse))]
        public async Task<ActionResult<ApplicationAreaResponse>> GetApplicationArea(string id)
        {
            var ApplicationArea = await _context.Applicationarea.FindAsync(id);

            if (ApplicationArea == null)
            {
                return NotFound();
            }

            return Ok(new ApplicationAreaResponse { ApplicationAreaId = ApplicationArea.ApplicationAreaId, ApplicationAreaName = ApplicationArea.ApplicationAreaName });
        }


        // POST: api/ApplicationAreas
        /// <summary>
        /// Create a new ApplicationArea within access control
        /// </summary>
        /// <param name="ApplicationArea">The ApplicationArea to be created</param>
        /// <returns>A created ApplicationArea entity</returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApplicationAreaResponse))]
        public async Task<ActionResult<ApplicationAreaResponse>> PostApplicationArea(ApplicationAreaPost ApplicationArea)
        {
            var dbApplicationArea =
            _context.Applicationarea.Add(new Models.Applicationarea { ApplicationAreaName = ApplicationArea.ApplicationAreaName, ApplicationAreaId = Guid.NewGuid().ToString() });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApplicationAreaExists(dbApplicationArea.Entity.ApplicationAreaId))
                {
                    return Conflict();
                }
                else
                {
                    _context.Entry(dbApplicationArea.Entity).State = EntityState.Unchanged;

                    throw;
                }
            }

            return CreatedAtAction("GetApplicationArea", new { id = dbApplicationArea.Entity.ApplicationAreaId }, new ApplicationAreaResponse { ApplicationAreaId = dbApplicationArea.Entity.ApplicationAreaId, ApplicationAreaName = dbApplicationArea.Entity.ApplicationAreaName });
        }

        private bool ApplicationAreaExists(string id)
        {
            return _context.Applicationarea.Any(e => e.ApplicationAreaId == id);
        }
    }
}

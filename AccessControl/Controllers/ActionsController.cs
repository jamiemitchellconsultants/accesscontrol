
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;
using Action = AccessControl.Models.Action;

namespace AccessControl.Controllers
{
    /// <summary>
    /// Manage the Action entities
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly AccessControlContext _context;
        /// <summary>
        /// Pass databse context
        /// </summary>
        /// <param name="context">Entity framework DbContext</param>
        public ActionsController(AccessControlContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all actions in access control
        /// </summary>
        /// <returns>A list of action entities</returns>
        // GET: api/Actions
        [HttpGet]
        [ProducesResponseType(200,Type=typeof(ActionResponse[]))]
        public async Task<ActionResult<IEnumerable<ActionResponse>>> GetAction()
        {
            return Ok( await _context.Action.Select(o=>new ActionResponse{ActionId = o.ActionId, ActionName = o.ActionName}).ToListAsync());
        }

        // GET: api/Actions/5
        /// <summary>
        /// Get a single action from access control
        /// </summary>
        /// <param name="id">The id of the action required</param>
        /// <returns>The details of the action</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200,Type=typeof(ActionResponse))]
        public async Task<ActionResult<ActionResponse>> GetAction(string id)
        {
            var action = await _context.Action.FindAsync(id);

            if (action == null)
            {
                return NotFound();
            }

            return Ok(new ActionResponse{ActionId = action.ActionId,ActionName = action.ActionName});
        }


        // POST: api/Actions
        /// <summary>
        /// Create a new action within access control
        /// </summary>
        /// <param name="action">The action to be created</param>
        /// <returns>A created action entity</returns>
        [HttpPost]
        [ProducesResponseType(200, Type=typeof(ActionResponse))]
        public async Task<ActionResult<ActionResponse>> PostAction(ActionPost action)
        {
            var dbAction = 
            _context.Action.Add(new Models.Action { ActionName = action.ActionName, ActionId = Guid.NewGuid().ToString() });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ActionExists(dbAction.Entity.ActionId))
                {
                    return Conflict();
                }
                else
                {
                    _context.Entry(dbAction.Entity).State = EntityState.Unchanged;

                    throw;
                }
            }

            return CreatedAtAction("GetAction", new { id = dbAction.Entity.ActionId }, new ActionResponse{ActionId =dbAction.Entity.ActionId,ActionName = dbAction.Entity.ActionName});
        }

        private bool ActionExists(string id)
        {
            return _context.Action.Any(e => e.ActionId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace AccessControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtissuersController : ControllerBase
    {
        private readonly AccessControlContext _context;

        public JwtissuersController(AccessControlContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Gets the list of JWT issuers
        /// </summary>
        /// <returns></returns>
        // GET: api/Jwtissuers
        [HttpGet]
        [SwaggerOperation(OperationId = "GetJWTIssuers")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        public async Task<ActionResult<IEnumerable<Jwtissuer>>> GetJwtissuers()
        {
            return await _context.Jwtissuer.ToListAsync();
        }

        // GET: api/Jwtissuers/5
        [HttpGet("{JWTIssuerId}")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(typeof(Jwtissuer),200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Jwtissuer>> GetJwtissuer(string JWTIssuerId)
        {
            var jwtissuer = await _context.Jwtissuer.FindAsync(JWTIssuerId);

            if (jwtissuer == null)
            {
                return NotFound();
            }


            return jwtissuer;
        }

        // PUT: api/Jwtissuers/5
        [HttpPut("{JWTIssuerId}")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Jwtissuer),200)]
        public async Task<IActionResult> UpdateJWTIssuer(string JWTIssuerId,[FromBody] Jwtissuer jwtissuer)
        {
            if (JWTIssuerId != jwtissuer.JwtissuerId)
            {
                return BadRequest();
            }

            _context.Entry(jwtissuer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JwtissuerExists(JWTIssuerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Jwtissuers
        [HttpPost]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Jwtissuer),200)]
        public async Task<ActionResult<Jwtissuer>> CreateJwtissuer([FromBody] Jwtissuer jwtissuer)
        {
            _context.Jwtissuer.Add(jwtissuer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (JwtissuerExists(jwtissuer.JwtissuerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetJwtissuers", new { id = jwtissuer.JwtissuerId }, jwtissuer);
        }

        // DELETE: api/Jwtissuers/5
        [HttpDelete("{JWTIssuerId}")]
        [ProducesErrorResponseType(typeof(ApiErrorResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Jwtissuer),StatusCodes.Status202Accepted)]
        public async Task<ActionResult<Jwtissuer>> DeleteJwtissuer(string JWTIssuerId)
        {
            var jwtissuer = await _context.Jwtissuer.FindAsync(JWTIssuerId);
            if (jwtissuer == null)
            {
                return NotFound();
            }

            _context.Jwtissuer.Remove(jwtissuer);
            await _context.SaveChangesAsync();

            return jwtissuer;
        }

        private bool JwtissuerExists(string id)
        {
            return _context.Jwtissuer.Any(e => e.JwtissuerId == id);
        }
    }
}

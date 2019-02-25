using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccessControl.Models;

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

        // GET: api/Jwtissuers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jwtissuer>>> GetJwtissuer()
        {
            return await _context.Jwtissuer.ToListAsync();
        }

        // GET: api/Jwtissuers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Jwtissuer>> GetJwtissuer(string id)
        {
            var jwtissuer = await _context.Jwtissuer.FindAsync(id);

            if (jwtissuer == null)
            {
                return NotFound();
            }


            return jwtissuer;
        }

        // PUT: api/Jwtissuers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJwtissuer(string id, Jwtissuer jwtissuer)
        {
            if (id != jwtissuer.JwtissuerId)
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
                if (!JwtissuerExists(id))
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
        public async Task<ActionResult<Jwtissuer>> PostJwtissuer(Jwtissuer jwtissuer)
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

            return CreatedAtAction("GetJwtissuer", new { id = jwtissuer.JwtissuerId }, jwtissuer);
        }

        // DELETE: api/Jwtissuers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Jwtissuer>> DeleteJwtissuer(string id)
        {
            var jwtissuer = await _context.Jwtissuer.FindAsync(id);
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

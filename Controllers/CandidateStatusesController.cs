using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crm.Data;
using crm.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "NormalPolicy")]
    public class CandidateStatusesController : ControllerBase
    {
        private readonly CrmContext _context;

        public CandidateStatusesController(CrmContext context)
        {
            _context = context;
        }

        // GET: api/CandidateStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateStatus>>> GetCandidateStatuses()
        {
            return await _context.CandidateStatuses.ToListAsync();
        }

        // GET: api/CandidateStatus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidateStatus([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candidateStatus = await _context.CandidateStatuses.FindAsync(id);

            if (candidateStatus == null)
            {
                return NotFound();
            }

            return Ok(candidateStatus);
        }

        // PUT: api/CandidateStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidateStatus([FromRoute] int id, [FromBody] CandidateStatus candidateStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != candidateStatus.CandidateStatusId)
            {
                return BadRequest();
            }

            _context.Entry(candidateStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateStatusExists(id))
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

        // POST: api/CandidateStatus
        [HttpPost]
        public async Task<IActionResult> PostCandidateStatus([FromBody] CandidateStatus candidateStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CandidateStatuses.Add(candidateStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCandidateStatus", new { id = candidateStatus.CandidateStatusId }, candidateStatus);
        }

        // DELETE: api/CandidateStatus/5
        [Authorize(Policy = "ElevatedPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidateStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candidateStatus = await _context.CandidateStatuses.FindAsync(id);
            if (candidateStatus == null)
            {
                return NotFound();
            }

            _context.CandidateStatuses.Remove(candidateStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CandidateStatusExists(int id)
        {
            return _context.CandidateStatuses.Any(e => e.CandidateStatusId == id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crm.Data;
using crm.Models;
using crm.BindingModels;
using crm.Services;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;
using crm.Authentication;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "NormalPolicy")]
    public class SendToInterviewController: ControllerBase
    {

        private readonly CrmContext _context;
        public SendToInterviewController(
            CrmContext context)
        {
            _context = context;
        }

        // GET: api/Candidates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSentCandidates()
        {
        var candidatesSent = await _context.CandidatesSentToInterview
            .Select(candidateSent => new
            {
                CandidateSentToIntrerviewId = candidateSent.CandidateSentToIntrerviewId,
                CandidateId = candidateSent.Candidate == null ? 0 : candidateSent.Candidate.CandidateId,
                CandidateName = candidateSent.Candidate == null ? "" : candidateSent.Candidate.Name,
                JobId = candidateSent.Job == null ? 0 : candidateSent.Job.JobId,
                Title = candidateSent.Job == null ? "" : candidateSent.Job.Title
            })
            .ToListAsync();
            return candidatesSent;
        }        


        // POST: api/Candidates
        [HttpPost]
        public async Task<IActionResult> PostCandidateSentToInterview([FromBody] CandidateSentToIntrerviewBindModel bindCandidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var job = await _context.Jobs.Include(j => j.Recruiter).FirstOrDefaultAsync(j => j.JobId == bindCandidate.JobId);
            if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == job.RecruiterId.ToString()) ||
                                       (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                return BadRequest();

            var candidateSent = new CandidateSentToIntrerview();
            if (bindCandidate.JobId > 0 && bindCandidate.CandidateId > 0)
            {
                var checkForExistingRecord = await _context.CandidatesSentToInterview.FirstOrDefaultAsync(cs => cs.CandidateId == bindCandidate.CandidateId && cs.JobId == bindCandidate.JobId);
                if (checkForExistingRecord == null)
                {
                    candidateSent.JobId = bindCandidate.JobId;
                    candidateSent.CandidateId = bindCandidate.CandidateId;
                }                
            }
            else
            {
                return BadRequest();
            }
                
            
            _context.CandidatesSentToInterview.Add(candidateSent);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Candidates/5
        [Authorize(Policy = "ElevatedPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candidateSent = await _context.CandidatesSentToInterview.FirstOrDefaultAsync(c => c.CandidateSentToIntrerviewId == id);
            if (candidateSent == null)
            {
                return NotFound();
            }               

            _context.CandidatesSentToInterview.Remove(candidateSent);
            await _context.SaveChangesAsync();


            return Ok();
        }   
    }
}

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

    public class CandidatesController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly ISavedDocumentHandler _savedDocumentHandler;
        public CandidatesController(
            CrmContext context,
            ISavedDocumentHandler savedDocumentHandler)
        {
            _context = context;
            _savedDocumentHandler = savedDocumentHandler;
        }

        // GET: api/Candidates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCandidates()
        {
            var dummy = User.Claims;
            var candidates = await _context.Candidates
            .Select(candidate => new
            {
                CandidateId = candidate.CandidateId,
                Name = candidate.Name,
                Email = candidate.Email,
                Phone = candidate.Phone,
                Documents = candidate.Documents,
                JobId = candidate.JobId,
                Job = new
                {
                    Id = candidate.Job == null ? 0 : candidate.Job.JobId,
                    Title = candidate.Job == null ? "" : candidate.Job.Title
                },
                RecruiterId = candidate.RecruiterId,
                Recruiter = new
                {
                    Id = candidate.Recruiter == null ? 0 : candidate.Recruiter.RecruiterId,
                    Name = candidate.Recruiter == null ? "" : candidate.Recruiter.Name,
                },
                CandidateStatus = candidate.CandidateStatus,
                AppliedForJobs = candidate.AppliedForJobs == null ? null : candidate.AppliedForJobs
                .Select(c => new
                {
                    CandidateSentToIntrerviewId = c.CandidateSentToIntrerviewId,
                    JobId = c.Job == null ? 0 : c.Job.JobId,
                    Title = c.Job == null ? "" : c.Job.Title
                }),
            })
            .ToListAsync();
            return candidates;
        }

        // GET: api/Candidates/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candidate = await _context.Candidates
            .Where(ca => ca.CandidateId == id)
            .Select(c => new
            {
                CandidateId = c.CandidateId,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Documents = c.Documents,
                JobId = c.JobId,
                Job = new
                {
                    Id = c.Job == null ? 0 : c.Job.JobId,
                    Title = c.Job == null ? "" : c.Job.Title
                },
                RecruiterId = c.RecruiterId,
                Recruiter = new
                {
                    Id = c.Recruiter == null ? 0 : c.Recruiter.RecruiterId,
                    Name = c.Recruiter == null ? "" : c.Recruiter.Name,
                },
                CandidateStatus = c.CandidateStatus,
                AppliedForJobs = c.AppliedForJobs == null ? null : c.AppliedForJobs
                .Select(can => new
                {
                    CandidateSentToIntrerviewId = can.CandidateSentToIntrerviewId,
                    JobId = can.Job == null ? 0 : can.Job.JobId,
                    Title = can.Job == null ? "" : can.Job.Title
                }),
            })
            .FirstOrDefaultAsync();

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(candidate);
        }

        // PUT: api/Candidates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidate([FromRoute] int id, [FromBody] BindCandidate bindCandidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bindCandidate.CandidateId)
            {
                return BadRequest();
            }

            if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == bindCandidate.RecruiterId.ToString()) ||
                                       (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                return BadRequest();


            var updatedCandidate = await _context.Candidates.FirstOrDefaultAsync(c => c.CandidateId == bindCandidate.CandidateId);
            updatedCandidate.Name = bindCandidate.Name;
            updatedCandidate.Email= bindCandidate.Email;
            updatedCandidate.Phone = bindCandidate.Phone;
            if (bindCandidate.JobId > 0)
            {
                var candidateSent = new CandidateSentToIntrerview() { JobId = bindCandidate.JobId, CandidateId = bindCandidate.CandidateId};
                _context.CandidatesSentToInterview.Add(candidateSent);                
            }                
            if (bindCandidate.RecruiterId > 0)
                updatedCandidate.Recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == bindCandidate.RecruiterId);
            if (bindCandidate.CandidateStatusId > 0)
                updatedCandidate.CandidateStatus = await _context.CandidateStatuses.FirstOrDefaultAsync(s => s.CandidateStatusId == bindCandidate.CandidateStatusId);

            _context.Entry(updatedCandidate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }catch(Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/Candidates
        [HttpPost]
        public async Task<IActionResult> PostCandidate([FromBody] BindCandidate bindCandidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bindCandidate.RecruiterId < 1)
                return BadRequest();

            var newCandidate = new Candidate()
            {
                Name = bindCandidate.Name,
                Email = bindCandidate.Email,
                Phone = bindCandidate.Phone
            };
            if (bindCandidate.RecruiterId > 0)
                newCandidate.Recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == bindCandidate.RecruiterId);
            if (bindCandidate.CandidateStatusId > 0)
                newCandidate.CandidateStatus = await _context.CandidateStatuses.FirstOrDefaultAsync(s => s.CandidateStatusId == bindCandidate.CandidateStatusId);
            _context.Candidates.Add(newCandidate);

            if (bindCandidate.JobId > 0)
            {
                var candiadetSent = new CandidateSentToIntrerview() {JobId = bindCandidate.JobId, CandidateId = newCandidate.CandidateId };
                _context.CandidatesSentToInterview.Add(candiadetSent);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCandidate", new { id = newCandidate.CandidateId }, newCandidate);
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

            var candidate = await _context.Candidates.Include(c => c.Documents).FirstOrDefaultAsync(c => c.CandidateId == id);
            if (candidate == null)
            {
                return NotFound();
            }

            if (candidate.Documents != null)
            {
                foreach (var doc in candidate.Documents)
                {
                    _savedDocumentHandler.DeleteDocument(doc.Name);
                }
            }

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(e => e.CandidateId == id);
        }

        [HttpPost("UploadDocument/{candidate_id}")]
        public async Task<IActionResult> UploadFile(int candidate_id, IFormFile file)
        {
            try
            {
                var candidate = await _context.Candidates.Include(c => c.Documents).FirstOrDefaultAsync(c => c.CandidateId == candidate_id);
                if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == candidate.RecruiterId.ToString()) ||
                    (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                    return BadRequest();

                if (candidate.Documents == null)
                    candidate.Documents = new List<FileRepresentationInDatabase>();               
               
                var document = await _savedDocumentHandler.SaveDocument(file);
                if (document == null)
                    return BadRequest();
                _context.Documents.Add(document);
                _context.SaveChanges();
                var doc = _context.Documents.FirstOrDefault(d => d.FileRepresentationInDatabaseId == document.FileRepresentationInDatabaseId);
                if (doc == null)
                    BadRequest();
                candidate.Documents.Add(doc);
                _context.Entry(candidate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
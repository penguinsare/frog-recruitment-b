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
    public class JobsController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly ISavedDocumentHandler _savedDocumentHandler;

        public JobsController(CrmContext context, ISavedDocumentHandler savedDocumentHandler)
        {
            _context = context;
            _savedDocumentHandler = savedDocumentHandler;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJobs()
        {            
            var jobs = await _context.Jobs
            .Select(job => new
            {
                JobId = job.JobId,
                Title = job.Title,
                BaseSalary = job.BaseSalary,
                Fee = job.Fee,
                StartDate = job.StartDate,
                Client = new
                {
                    Id = job.Client == null ? 0 : job.Client.ClientId,
                    CompanyName = job.Client == null ? "" : job.Client.CompanyName,
                    ContactPerson = job.Client == null ? "" : job.Client.ContactPerson
                },
                Recruiter = new
                {
                    Id = job.Recruiter == null ? 0 : job.Recruiter.RecruiterId,
                    Name = job.Recruiter == null ? "" : job.Recruiter.Name
                },
                Documents = job.Documents,
                Remarks = job.Remarks,
                CandidatesSent = job.CandidatesSent == null ? null : job.CandidatesSent
                .Select(c => new
                {
                    CandidateSentToIntrerviewId = c.CandidateSentToIntrerviewId,
                    CandidateId = c.Candidate == null ? 0 : c.Candidate.CandidateId,
                    CandidateName = c.Candidate == null ? "" : c.Candidate.Name,                    
                    CandidateEmail = c.Candidate == null ? "" : c.Candidate.Email
                }),
                JobStatus = job.JobStatus == null ? null : job.JobStatus,
            })
            .ToListAsync();

            return jobs;
        }

        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var returnJob = await _context.Jobs
            .Where(j => j.JobId == id)
            .Select(job => new
            {
                JobId = job.JobId,
                Title = job.Title,
                BaseSalary = job.BaseSalary,
                Fee = job.Fee,
                StartDate = job.StartDate,
                Client = new
                {
                    Id = job.Client == null ? 0 : job.Client.ClientId,
                    CompanyName = job.Client == null ? "" : job.Client.CompanyName,
                    ContactPerson = job.Client == null ? "" : job.Client.ContactPerson
                },
                Recruiter = new
                {
                    Id = job.Recruiter == null ? 0 : job.Recruiter.RecruiterId,
                    Name = job.Recruiter == null ? "" : job.Recruiter.Name
                },
                Documents = job.Documents,
                Remarks = job.Remarks,
                CandidatesSent = job.CandidatesSent == null ? null : job.CandidatesSent
                .Select(c => new
                {
                    CandidateSentToIntrerviewId = c.CandidateSentToIntrerviewId,
                    CandidateId = c.Candidate == null ? 0 : c.Candidate.CandidateId,
                    CandidateName = c.Candidate == null ? "" : c.Candidate.Name,
                    CandidateEmail = c.Candidate == null ? "" : c.Candidate.Email
                }),
                JobStatus = job.JobStatus == null ? null : job.JobStatus                    
            })
            .FirstOrDefaultAsync(j => j.JobId == id);

            if (returnJob == null)
            {
                return NotFound();
            }

            return Ok(returnJob);
        }

        // PUT: api/Jobs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob([FromRoute] int id, [FromBody] BindJob bindJob)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bindJob.JobId)
            {
                return BadRequest();
            }
            var updatedJob = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobId== bindJob.JobId);
            if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == updatedJob.RecruiterId.ToString()) ||
                                        (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                return BadRequest();

            updatedJob.Title = bindJob.Title;
            updatedJob.Remarks = bindJob.Remarks;
            updatedJob.BaseSalary = bindJob.BaseSalary;
            updatedJob.Fee = bindJob.Fee;
            updatedJob.StartDate = bindJob.StartDate;
            if (bindJob.ClientId > 0 & updatedJob.ClientId != bindJob.ClientId)
                updatedJob.Client = await _context.CrmClients.FirstOrDefaultAsync(cl => cl.ClientId == bindJob.ClientId);
            if (bindJob.JobStatusId > 0)
                updatedJob.JobStatus = await _context.JobStatuses.FirstOrDefaultAsync(js => js.JobStatusId == bindJob.JobStatusId);

            _context.Entry(updatedJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
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

        // POST: api/Jobs
        [HttpPost]
        public async Task<IActionResult> PostJob([FromBody] BindJob bindJob)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bindJob.RecruiterId < 1)
                return BadRequest();

            var newJob = new Job()
            {
                Title = bindJob.Title,
                Remarks = bindJob.Remarks
            };
            if (bindJob.ClientId > 0)
                newJob.Client = await _context.CrmClients.FirstOrDefaultAsync(cl => cl.ClientId == bindJob.ClientId);
            if (bindJob.RecruiterId > 0)
                newJob.Recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == bindJob.RecruiterId);

            newJob.CandidatesSent = new List<CandidateSentToIntrerview>();
            if (bindJob.CandidateId > 0)
                newJob.CandidatesSent
                    .Add(new CandidateSentToIntrerview() {
                        CandidateSentToIntrerviewId = bindJob.CandidateId
                    });
            if (bindJob.JobStatusId > 0)
            {
                newJob.JobStatus = await _context.JobStatuses.FirstOrDefaultAsync(js => js.JobStatusId == bindJob.JobStatusId);
            }

            _context.Jobs.Add(newJob);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJob", new { id = newJob.JobId }, newJob);
        }

        // DELETE: api/Jobs/5
        [Authorize(Policy = "ElevatedPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = await _context.Jobs.Include(j => j.Documents).FirstOrDefaultAsync(j => j.JobId == id);
            if (job == null)
            {
                return NotFound();
            }

            if (job.Documents != null)
            {
                foreach (var doc in job.Documents)
                {
                    _savedDocumentHandler.DeleteDocument(doc.Name);
                }
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
            //return Ok(job);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }

        [HttpPost("UploadDocument/{job_id}")]
        public async Task<IActionResult> UploadFile(int job_id, IFormFile file)
        {
            try
            {
                var job = await _context.Jobs.Include(c => c.Documents).FirstOrDefaultAsync(j => j.JobId == job_id);
                if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == job.RecruiterId.ToString()) ||
                                       (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                    return BadRequest();

                if (job.Documents == null)
                    job.Documents = new List<FileRepresentationInDatabase>();
                var document = await _savedDocumentHandler.SaveDocument(file);
                if (document == null)
                    return BadRequest();

                _context.Documents.Add(document);
                _context.SaveChanges();
                var doc = _context.Documents.FirstOrDefault(d => d.FileRepresentationInDatabaseId == document.FileRepresentationInDatabaseId);
                if (doc == null)
                    BadRequest();
                job.Documents.Add(doc);
                _context.Entry(job).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
        }
    }
}
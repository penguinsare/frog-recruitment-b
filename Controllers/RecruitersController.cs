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
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "NormalPolicy")]
    public class RecruitersController : ControllerBase
    {
        private readonly CrmContext _context;
        public RecruitersController(CrmContext context)
        {
            _context = context;
        }
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Recruiter>>> GetRecruiters()
        {
            return await _context.Recruiters
            .ToListAsync();
        }

        // GET api/recruiters/5
        [HttpGet("{id}")]
        
        public async Task<ActionResult<Recruiter>> Get(int id)
        {
            try
            {
                var recruiter = await _context.Recruiters.FirstOrDefaultAsync(j => j.RecruiterId == id);
                if (recruiter != null)
                {
                    return Ok(recruiter);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST api/recruiters
        [HttpPost]
        [Authorize(Policy = "ElevatedPolicy")]
        public async Task<ActionResult<Recruiter>> Post([FromBody] BindRecruiter bindRecruiter)
        {
            try
            {
                if (bindRecruiter != null)
                {
                    var newRecruiter = new Recruiter()
                    { 
                        Name = bindRecruiter.Name,
                        Email = bindRecruiter.Email,
                        Phone = bindRecruiter.Phone
                    };
                   
                    _context.Recruiters.Add(newRecruiter);
                    await _context.SaveChangesAsync();
                    var insertedRecruiter = _context.Recruiters.Where(j => j.RecruiterId == newRecruiter.RecruiterId);
                    if (insertedRecruiter != null)
                    {
                        return CreatedAtAction(nameof(newRecruiter), new { id = newRecruiter.RecruiterId }, newRecruiter);
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/recruiters/5
        [HttpPut("{id}")]
        [Authorize(Policy = "ElevatedPolicy")]
        public async Task<ActionResult> Put(int id, [FromBody] BindRecruiter bindRecruiter)
        {
            try
            {
                if (id != bindRecruiter.RecruiterId)
                {
                    return BadRequest();
                }

                var updatedRecruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == bindRecruiter.RecruiterId);
                updatedRecruiter.Name = bindRecruiter.Name;
                updatedRecruiter.Email = bindRecruiter.Email;
                updatedRecruiter.Phone = bindRecruiter.Phone;

                _context.Entry(updatedRecruiter).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE api/recruiters/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "ElevatedPolicy")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var recruiter = await _context.Recruiters.FirstOrDefaultAsync(j => j.RecruiterId == id);
                if (recruiter == null)
                {
                    return NotFound();
                }
                _context.Remove(recruiter);
                await _context.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
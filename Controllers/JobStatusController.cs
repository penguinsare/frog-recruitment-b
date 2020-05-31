using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crm.Data;
using crm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "NormalPolicy")]
    public class JobStatusController : ControllerBase
    {
        private CrmContext _context;

        public JobStatusController(CrmContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobStatus>>> Get()
        {
            try
            {
                return await _context.JobStatuses.ToListAsync();
            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
        }
    }
}
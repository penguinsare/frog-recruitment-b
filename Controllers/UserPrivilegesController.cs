using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crm.Data;
using crm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPrivilegesController : ControllerBase
    {
        private CrmContext _context;

        public UserPrivilegesController(CrmContext context)
        {
            _context = context;
        }

        // GET: api/UserPrivileges
        [HttpGet]
        public IEnumerable<UserPrivilege> Get()
        {
            return _context.UserPrivileges.ToList();
        }
    }
}

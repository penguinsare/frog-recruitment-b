using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using crm.BindingModels;
using crm.Data;
using crm.Models;
using crm.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;
using crm.Authentication;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "NormalPolicy")]
    public class ClientsController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly ISavedDocumentHandler _savedDocumentHandler;
        public ClientsController(
            CrmContext context,
            ISavedDocumentHandler savedDocumentHandler)
        {
            _context = context;
            _savedDocumentHandler = savedDocumentHandler;
        }
        // GET api/clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetClientss()
        {
            var clients = await _context.CrmClients
                .Select(client => new
                {
                    ClientId = client.ClientId,
                    ContactPerson = client.ContactPerson,
                    CompanyName = client.CompanyName,
                    Designation = client.Designation,
                    TelephoneOffice = client.TelephoneOffice,
                    TelephoneMobile = client.TelephoneMobile,
                    Email = client.Email,
                    Address = client.Address,
                    Remarks = client.Remarks,
                    Documents = client.Documents,
                    Recruiter = new
                    {
                        Id = client.Recruiter == null ? 0 : client.Recruiter.RecruiterId,
                        Name = client.Recruiter == null ? "" : client.Recruiter.Name
                    }
                })
                .ToListAsync();

            return clients;
        }

        // GET api/clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> Get(int id)
        {
            try
            {
                var returnClient = await _context.CrmClients
                .Where(cl => cl.ClientId == id)
                .Select(client => new
                {
                    ClientId = client.ClientId,
                    ContactPerson = client.ContactPerson,
                    CompanyName = client.CompanyName,
                    Designation = client.Designation,
                    TelephoneOffice = client.TelephoneOffice,
                    TelephoneMobile = client.TelephoneMobile,
                    Email = client.Email,
                    Address = client.Address,
                    Remarks = client.Remarks,
                    Documents = client.Documents,
                    Recruiter = new
                    {
                        Id = client.Recruiter == null ? 0 : client.Recruiter.RecruiterId,
                        Name = client.Recruiter == null ? "" : client.Recruiter.Name
                    }
                })
                .FirstOrDefaultAsync(cl => cl.ClientId == id);

                if (returnClient == null)
                {
                    return NotFound();
                }

                return Ok(returnClient);
                
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST api/clients
        
        [HttpPost]
        public async Task<ActionResult<Client>> Post([FromBody] BindClient bindClient)
        {
            try
            {
                if (bindClient != null)
                {
                    if (bindClient.RecruiterId < 1)
                        return BadRequest();

                    var newClient = new Client()
                    {
                        ContactPerson = bindClient.ContactPerson,
                        CompanyName = bindClient.CompanyName,
                        Email = bindClient.Email,
                        TelephoneOffice = bindClient.TelephoneOffice,
                        TelephoneMobile = bindClient.TelephoneMobile,
                        Designation = bindClient.Designation,
                        Remarks = bindClient.Remarks,
                        Address = bindClient.Address
                    };

                    if (bindClient.RecruiterId > 0)
                        newClient.Recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == bindClient.RecruiterId);

                    _context.CrmClients.Add(newClient);
                    await _context.SaveChangesAsync();

                    var insertedClient = _context.CrmClients.Where(j => j.ClientId == newClient.ClientId);
                    if (insertedClient != null)
                    {
                        return CreatedAtAction(nameof(newClient), new { id = newClient.ClientId }, newClient);
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/clients/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] BindClient bindClient)
        {
            try
            {
                if (id != bindClient.ClientId)
                {
                    return BadRequest();
                }

                if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == bindClient.RecruiterId.ToString()) ||
                                        (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                    return BadRequest();

                var updatedClient = await _context.CrmClients.FirstOrDefaultAsync(cl => cl.ClientId == bindClient.ClientId);
                updatedClient.ContactPerson = bindClient.ContactPerson;
                updatedClient.CompanyName = bindClient.CompanyName;
                updatedClient.Email = bindClient.Email;
                updatedClient.TelephoneOffice = bindClient.TelephoneOffice;
                updatedClient.TelephoneMobile = bindClient.TelephoneMobile;
                updatedClient.Designation = bindClient.Designation;
                updatedClient.Remarks = bindClient.Remarks;
                updatedClient.Address = bindClient.Address;
                
                _context.Entry(updatedClient).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE api/clients/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "ElevatedPolicy")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var client = await _context.CrmClients.Include(cl => cl.Documents).FirstOrDefaultAsync(cl => cl.ClientId == id);
                if (client == null)
                {
                    return NotFound();
                }

                if (client.Documents != null)
                {
                    foreach (var doc in client.Documents)
                    {
                        _savedDocumentHandler.DeleteDocument(doc.Name);
                    }
                }
                _context.Remove(client);
                await _context.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
        }

        [HttpPost("UploadDocument/{client_id}")]
        public async Task<IActionResult> UploadFile(int client_id, IFormFile file)
        {
            try
            {
                var client = _context.CrmClients.Include(cl => cl.Documents).FirstOrDefault(cl => cl.ClientId == client_id);

                if (!User.HasClaim(claim => (claim.Type == DefinedClaimTypes.RecruiterId && claim.Value == client.RecruiterId.ToString()) ||
                                       (claim.Type == DefinedClaimTypes.Access && claim.Value == DefinedClaimAccessValues.Elevated)))
                    return BadRequest();

                if (client.Documents == null)
                    client.Documents = new List<FileRepresentationInDatabase>();

                var document = await _savedDocumentHandler.SaveDocument(file);
                if (document == null)
                    return BadRequest();
                _context.Documents.Add(document);
                _context.SaveChanges();
                client.Documents.Add(document);
                _context.Entry(client).State = EntityState.Modified;
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
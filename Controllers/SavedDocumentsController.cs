using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crm.Data;
using crm.Services;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crm.Models;

namespace crm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "NormalPolicy")]
    public class SavedDocumentsController : ControllerBase
    {
        private CrmContext _context;
        private ISavedDocumentHandler _savedDocumentHandler;
        public SavedDocumentsController(
            CrmContext context, 
            ISavedDocumentHandler savedDocumentHandler)
        {
            _context = context;
            _savedDocumentHandler = savedDocumentHandler;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Download(int id)
        {
            
            try
            {
                var document = _context.Documents.FirstOrDefault(d => d.FileRepresentationInDatabaseId == id);

                var savedDocument = await _savedDocumentHandler
                  .RetrieveSavedDocument(document.Name);
                return File(
                    savedDocument.OpenReadStream(),
                    savedDocument.ContentType,
                    savedDocument.FileName);
            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
        }

        [Authorize(Policy = "ElevatedPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var document = await _context.Documents
                    .FirstOrDefaultAsync(d => d.FileRepresentationInDatabaseId == id);
                if (document == null)
                {
                    return NotFound();
                }
                _savedDocumentHandler.DeleteDocument(document.Name);
                _context.Remove(document);
                await _context.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                var debugEx = ex;
                return BadRequest();
            }
        }
    }
}
using crm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Services
{
    public interface ISavedDocumentHandler
    {
        Task<SavedDocument> RetrieveSavedDocument(string fullPath);
        Task<FileRepresentationInDatabase> SaveDocument(IFormFile file);
        void DeleteDocument(string pathToFile);
    }
}

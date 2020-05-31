using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace crm.Services
{
    public class SavedDocument : IFormFile
    {
        //private Stream _memoryStream;
        private byte[] _streamBuffer;
        public SavedDocument(
            byte[] streamBuffer,
            string contentType = "application/pdf",
            string fileName = "new_file.pdf"            
            )
        {
            ContentType = contentType;
            FileName = fileName;
            _streamBuffer = streamBuffer;
        }
        public string ContentType
        { get; set; }

        public string ContentDisposition => throw new NotImplementedException();

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public long Length => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string FileName { get; set; }

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Stream OpenReadStream()
        {
            return new MemoryStream(_streamBuffer);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using crm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace crm.Services
{
    public class SavedDocumentHandler : ISavedDocumentHandler
    {

        private string _connectionString;
        private string _containerReference = "documents";
        private IConfiguration _configuration;
        public SavedDocumentHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            //_connectionString = _configuration["Azure:StorageConnectionString"];
        }

        public async Task<SavedDocument> RetrieveSavedDocument(string fileName)
        {
            return null;
            //var container = await ConnectToAzureCloudStorageContainer();
            //if (fileName == null)
            //    throw new ArgumentNullException("fileName");
            //var memStream = new MemoryStream();
            //var blockBlob = container.GetBlockBlobReference(fileName);
            //if (blockBlob.Exists())
            //{
            //    blockBlob.DownloadToStream(memStream);
            //}

            //memStream.Position = 0;
            //var returnDocument = new SavedDocument(
            //    memStream.GetBuffer(),
            //    "application/pdf",
            //    fileName);
            //return returnDocument;
            
        }

        public async Task<FileRepresentationInDatabase> SaveDocument(IFormFile file)
        {

            //var container = await ConnectToAzureCloudStorageContainer();
            //string fileName = file.FileName;
            ////get Blob reference
            //CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileName);            
            //if (cloudBlockBlob.Exists())
            //{
            //    fileName = 
            //        Path.GetFileNameWithoutExtension(fileName) + 
            //        "_" +
            //        DateTime.Now.ToString("yyyyMMddHHmmssfff") +
            //        Path.GetExtension(fileName);
            //    cloudBlockBlob = container.GetBlockBlobReference(fileName);
            //}
            //using (var stream = new MemoryStream())
            //{
            //    file.CopyTo(stream); 
            //    stream.Position = 0;
            //    cloudBlockBlob.UploadFromStream(stream);
            //}

            //cloudBlockBlob = container.GetBlockBlobReference(fileName);
            //if (cloudBlockBlob.Exists())
            //{
            //    return new FileRepresentationInDatabase()
            //    {
            //        Name = fileName,
            //        Path = ""
            //    };
            //}
            //else
            //{
            //    return null;
            //}

            //var filePath = Path.GetTempFileName();
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "documents",
                Path.GetFileName(file.FileName));
            DirectoryInfo documents = new DirectoryInfo(Path.Combine(
                Directory.GetCurrentDirectory(), "documents"));
            if (!documents.Exists)
                documents.Create();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                {
                    await file.CopyToAsync(stream);
                    return new FileRepresentationInDatabase()
                    {
                        Name = Path.GetFileName(file.FileName),
                        Path = filePath
                    };
                }
            }
            catch (Exception ex)
            {
                // if file already exists
                try{
                    var newFileName =
                    "\\" +
                    Path.GetFileNameWithoutExtension(filePath) +
                    "_" +
                    DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                    Path.GetExtension(filePath);
                    var newFilePath = Path.Combine(
                        Path.GetDirectoryName(filePath) +
                        newFileName);
                    using (var stream = new FileStream(newFilePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return new FileRepresentationInDatabase()
                    {
                        Name = Path.GetFileName(file.FileName),
                        Path = filePath
                    };
                }
                catch (Exception ex2)
                {
                    throw;
                }
                
            }            
        }

        public async void DeleteDocument(string pathToFile)
        {
            //try
            //{
            //    var container = await ConnectToAzureCloudStorageContainer();
            //    container.GetBlockBlobReference(fileName).DeleteIfExists();
            //}
            //catch (Exception ex)
            //{
            //    var debugEx = ex;
            //}
        }

        //private async Task<CloudBlobContainer> ConnectToAzureCloudStorageContainer()
        //{
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
        //    //create a block blob CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    //create a container CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");
        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_containerReference);

        //    if (await cloudBlobContainer.CreateIfNotExistsAsync())
        //    {
        //        await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        //    }

        //    return cloudBlobContainer;
        //}
    }
}

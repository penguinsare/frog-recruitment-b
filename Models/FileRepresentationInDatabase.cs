using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class FileRepresentationInDatabase
    {
        public int FileRepresentationInDatabaseId { get;set;}
        //public int RecruiterId { get; set; }
        [JsonIgnore]
        public string Path { get; set; }
        //[JsonIgnore]
        //public string Container { get; set; }
        public string Name { get; set; }
        public Candidate Candidate { get; set; }
        //[JsonIgnore]
        //public Recruiter Recruiter { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class PutInternshipApplication
    {
        public Guid InternshipId { get; set; }
        public string Message { get; set; }
    }
}
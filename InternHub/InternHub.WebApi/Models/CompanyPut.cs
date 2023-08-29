using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class CompanyPut
    {
        public string Name { get; set; } = null;
        public string Website { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public string Address { get; set; } = null;
        public string Description { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public Guid? CountyId { get; set; } = null;
        public string Email { get; set; } = null;
    }
}
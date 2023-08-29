using InternHub.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class PostStudent
    {
        [Required]
        public Guid StudyAreaId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Guid CountyId { get; set; }
        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
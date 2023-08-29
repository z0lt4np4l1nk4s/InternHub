using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class InternshipShortView
    {
        public InternshipShortView(Internship internship)
        {
            Name = internship.Name;
            Description = internship.Description;
            Address = internship.Address;
            StartDate = internship.StartDate;
            EndDate = internship.EndDate;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
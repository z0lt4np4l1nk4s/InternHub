using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace InternHub.WebApi.Models
{
    public class InternshipView
    {
        public InternshipView() { }

        public InternshipView(Internship internship)
        {
            Id = internship.Id;
            StudyAreaId = internship.StudyAreaId;
            StudyArea = new StudyAreaView(internship.StudyArea);
            CompanyId = internship.CompanyId;
            Company = new CompanyView(internship.Company);
            Name = internship.Name;
            Description = internship.Description;
            Address = internship.Address;
            StartDate = internship.StartDate;
            EndDate = internship.EndDate;
            ApplicationsCount = internship.ApplicationsCount;
        }

        public Guid StudyAreaId { get; set; }
        public StudyAreaView StudyArea { get; set; }
        public string CompanyId { get; set; }
        public CompanyView Company { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ApplicationsCount { get; set; }
        public Guid Id { get; set; }
        public bool IsApplied { get; set; } = false;
    }
}
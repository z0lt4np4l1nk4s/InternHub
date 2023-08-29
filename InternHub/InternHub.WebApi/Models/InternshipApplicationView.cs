using InternHub.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class InternshipApplicationView
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Message { get; set; }
        public StudentView Student { get; set; }
        public StateView State { get; set; }
        public InternshipView Internship { get; set; }

        public InternshipApplicationView(InternshipApplication internshipApplication)
        {
            Id = internshipApplication.Id;
            Message = internshipApplication.Message;
            DateCreated = internshipApplication.DateCreated;
            DateUpdated = internshipApplication.DateUpdated;
            Student = new StudentView(internshipApplication.Student);
            State = new StateView(internshipApplication.State);
            Internship = new InternshipView(internshipApplication.Internship);
        }
    }
}

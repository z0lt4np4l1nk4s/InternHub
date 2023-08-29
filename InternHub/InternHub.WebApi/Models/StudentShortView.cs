using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class StudentShortView
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public StudyAreaView StudyArea { get; set; }

        public string Email{ get; set; }
        public StudentShortView(Student student)
        {
            Id = student.Id;
            FirstName = student.FirstName;
            LastName = student.LastName;
            Email = student.Email;
            StudyArea = new StudyAreaView(student.StudyArea);
        }
    }
}
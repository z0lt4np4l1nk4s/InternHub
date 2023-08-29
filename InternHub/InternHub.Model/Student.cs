using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class Student : User, IStudent
    {
        [NotMapped]
        public Guid StudyAreaId { get; set; }
        [NotMapped]
        public virtual StudyArea StudyArea { get; set; }

        public Student() { }

        public Student(string id, string firstName, string lastName, string email, string phoneNumber, string address, string description, DateTime dateCreated, DateTime dateUpdated, Guid countyId, bool isActive, Guid studyAreaId)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Description = description;
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
            CountyId = countyId;
            StudyAreaId = studyAreaId;
            IsActive = isActive;
        }

        public Student (string id, string firstName, string lastName, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}

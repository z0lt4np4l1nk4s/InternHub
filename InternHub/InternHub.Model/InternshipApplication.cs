using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class InternshipApplication : BaseModel, IInternshipApplication
    {
     
        public Guid StateId { get; set; }
        public State State { get; set; }
        public string StudentId { get; set; }
        public Student Student { get; set; }
        public Guid InternshipId { get; set; }
        public Internship Internship { get; set; }
        public string Message { get; set; } 

    }
}

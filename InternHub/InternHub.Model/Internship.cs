using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class Internship : BaseModel, IInternship
    {
        public Guid StudyAreaId { get; set; }
        public StudyArea StudyArea { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ApplicationsCount { get; set; }
    }
}

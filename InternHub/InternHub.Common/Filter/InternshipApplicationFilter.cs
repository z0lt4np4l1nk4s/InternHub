using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Common.Filter
{
    public class InternshipApplicationFilter
    {
        public List<Guid> States { get; set; } = null;
        public string InternshipName { get; set; } = null;
        public string CompanyName { get; set; }
        public string StudentId { get; set; } = null;
        public string CompanyId { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
    }
}

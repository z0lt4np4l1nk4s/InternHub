using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Common.Filter
{
    public class StudentFilter
    {
        public List<Guid> StudyAreas { get; set; } = null;
        public List<Guid> Counties { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public bool IsActive { get; set; } = true;
    }
}

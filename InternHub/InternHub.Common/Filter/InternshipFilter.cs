using System;
using System.Collections.Generic;

namespace InternHub.Common.Filter
{
    public class InternshipFilter
    {
        public List<Guid> Counties { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string Name { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public string CompanyId { get; set; } = null;
    }
}

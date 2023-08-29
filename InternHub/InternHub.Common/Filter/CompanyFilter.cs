using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Common.Filter
{
    public class CompanyFilter
    {
        public string Name { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public bool? IsAccepted { get; set; } = null;
    }
}

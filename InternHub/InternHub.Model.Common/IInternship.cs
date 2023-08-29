using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface IInternship : IBaseModel
    {
        Guid StudyAreaId { get; set; }
        string CompanyId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Address { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface IInternshipApplication : IBaseModel
    {
        Guid StateId { get; set; }
        string StudentId { get; set; }
        Guid InternshipId { get; set; }
    }
}

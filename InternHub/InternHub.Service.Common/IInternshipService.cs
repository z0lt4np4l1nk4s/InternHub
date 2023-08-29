using InternHub.Common.Filter;
using InternHub.Common;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface IInternshipService
    {
        Task<PagedList<Internship>> GetAsync(Sorting sorting, Paging paging, InternshipFilter filter);
        Task<Internship> GetAsync(Guid id);
        Task<bool> PostAsync(Internship internship, string userId);
        Task<bool> PutAsync(Internship internship, string userId);
        Task<bool> DeleteAsync(Internship internship, string userId);
        Task<Internship> GetInternshipAsync(Guid id);
        Task<bool> IsStudentRegisteredToInternshipAsync(string studentId, Guid internshipId);
    }
}

using InternHub.Common.Filter;
using InternHub.Common;
using InternHub.Model;
using System.Threading.Tasks;
using System;

namespace InternHub.Repository.Common
{
    public interface IInternshipRepository
    {
        Task<PagedList<Internship>> GetAsync(Sorting sorting, Paging paging, InternshipFilter filter);
        Task<Internship> GetAsync(Guid id);
        Task<bool> PostAsync(Internship internship);
        Task<bool> PutAsync(Internship internship);
        Task<bool> DeleteAsync(Internship internship);
        Task<Internship> GetInternshipAsync(Guid id);
        Task<bool> IsStudentRegisteredToInternshipAsync(string studentId, Guid internshipId);
    }
}

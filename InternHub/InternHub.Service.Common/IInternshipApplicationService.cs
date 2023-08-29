using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface IInternshipApplicationService
    {
        Task<PagedList<InternshipApplication>> GetAllInternshipApplicationsAsync(Paging paging, Sorting sorting, InternshipApplicationFilter internshipApplicationFilter);
        Task<PagedList<InternshipApplication>> GetUnacceptedAsync(Paging paging, Sorting sorting, InternshipApplicationFilter internshipApplicationFilter);
        Task<bool> PutAsync(InternshipApplication internshipApplication, string currentUserId);
        Task<InternshipApplication> GetInternshipApplicationByIdAsync(Guid id);
        Task<InternshipApplication> GetByInternshipAsync(Guid internshipId, string studentId);
        Task<bool> PostInternshipApplicationAsync(InternshipApplication internshipApplication, string currentUserId);
        Task<bool> DeleteAsync(InternshipApplication internshipApplication, string userId);
        Task<Guid?> GetIdAsync(string studentId, Guid internshipId);
      
    }
}

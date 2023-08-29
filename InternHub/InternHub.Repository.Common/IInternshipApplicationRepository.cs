using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository.Common
{
    public interface IInternshipApplicationRepository
    {
        Task<PagedList<InternshipApplication>> GetAllInternshipApplicationsAsync(Paging paging, Sorting sorting, InternshipApplicationFilter internshipApplicationFilter);
        Task<PagedList<InternshipApplication>> GetUnacceptedAsync(Paging paging, Sorting sorting, InternshipApplicationFilter internshipApplicationFilter);
        Task<bool> PutAsync(InternshipApplication internshipApplication);
        Task<InternshipApplication> GetInternshipApplicationByIdAsync(Guid id);
        Task<InternshipApplication> GetByInternshipAsync(Guid internshipId, string studentId);
        Task<bool> PostInternshipApplicationAsync(InternshipApplication internshipApplication);
        Task<bool> DeleteAsync(InternshipApplication internshipApplication);
        Task<Guid?> GetIdAsync(string studentId, Guid internshipId);
       
    }
}

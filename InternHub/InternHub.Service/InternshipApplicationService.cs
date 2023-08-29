using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Repository.Common;
using InternHub.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service
{
    public class InternshipApplicationService : IInternshipApplicationService
    {
        public IInternshipApplicationRepository _repo;

        public InternshipApplicationService(IInternshipApplicationRepository internshipApplicationRepository)
        {
            _repo = internshipApplicationRepository;
        }

        public async Task<bool> DeleteAsync(InternshipApplication internshipApplication, string userId)
        {
            internshipApplication.UpdatedByUserId = userId;
            internshipApplication.DateUpdated = DateTime.Now;
            return await _repo.DeleteAsync(internshipApplication);
        }

        public async Task<PagedList<InternshipApplication>> GetAllInternshipApplicationsAsync(Paging paging, Sorting sorting, InternshipApplicationFilter internshipApplicationFilter) => await _repo.GetAllInternshipApplicationsAsync(paging, sorting, internshipApplicationFilter);

        public async Task<Guid?> GetIdAsync(string studentId, Guid internshipId)
        {
            return await _repo.GetIdAsync(studentId, internshipId);
        }
        public async Task<PagedList<InternshipApplication>> GetUnacceptedAsync(Paging paging, Sorting sorting, InternshipApplicationFilter internshipApplicationFilter) => await _repo.GetUnacceptedAsync(paging, sorting, internshipApplicationFilter);

        public async Task<bool> PutAsync(InternshipApplication internshipApplication, string currentUserId)
        {
            internshipApplication.UpdatedByUserId = currentUserId;
            internshipApplication.DateUpdated = DateTime.Now;
            return await _repo.PutAsync(internshipApplication);
        }

        public async Task<InternshipApplication> GetInternshipApplicationByIdAsync(Guid id) => await _repo.GetInternshipApplicationByIdAsync(id);

        public async Task<InternshipApplication> GetByInternshipAsync(Guid internshipId, string studentId) => await _repo.GetByInternshipAsync(internshipId, studentId);

        public async Task<bool> PostInternshipApplicationAsync(InternshipApplication internshipApplication, string currentUserId)
        {
            if (currentUserId == null)
            {
                return false;

            }
            if (internshipApplication == null)
            {
                return false;

            }
            internshipApplication.DateCreated = DateTime.Now;
            internshipApplication.DateUpdated = DateTime.Now;
            internshipApplication.CreatedByUserId = currentUserId;
            internshipApplication.UpdatedByUserId = currentUserId;
            return await _repo.PostInternshipApplicationAsync(internshipApplication);
        }
    }
}

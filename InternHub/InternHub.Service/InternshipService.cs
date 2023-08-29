using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Repository.Common;
using InternHub.Service.Common;
using System;
using System.Threading.Tasks;

namespace InternHub.Service
{
    public class InternshipService : IInternshipService
    {
        private IInternshipRepository InternshipRepository { get; set; }

        public InternshipService(IInternshipRepository internshipRepository)
        {
            InternshipRepository = internshipRepository;
        }
        public async Task<bool> DeleteAsync(Internship internship, string userId)
        {
            internship.UpdatedByUserId = userId;
            internship.DateUpdated = DateTime.Now;
            return await InternshipRepository.DeleteAsync(internship);
        }

        public async Task<PagedList<Internship>> GetAsync(Sorting sorting, Paging paging, InternshipFilter filter)
        {
            return await InternshipRepository.GetAsync(sorting, paging, filter);
        }

        public async Task<Internship> GetAsync(Guid id)
        {
            return await InternshipRepository.GetAsync(id);
        }

        public async Task<Internship> GetInternshipAsync(Guid id)
        {
            return await InternshipRepository.GetInternshipAsync(id);
        }

        public async Task<bool> PostAsync(Internship internship, string userId)
        {
            internship.Id = Guid.NewGuid();
            internship.DateCreated = DateTime.Now;
            internship.DateUpdated = DateTime.Now;
            internship.CreatedByUserId = userId;
            internship.UpdatedByUserId = userId;

            return await InternshipRepository.PostAsync(internship); 
        }

        public async Task<bool> PutAsync(Internship internship, string userId)
        {
            internship.UpdatedByUserId = userId;
            internship.DateUpdated = DateTime.Now;
            return await InternshipRepository.PutAsync(internship);
        }

        public async Task<bool> IsStudentRegisteredToInternshipAsync(string studentId, Guid internshipId)
        {
            return await InternshipRepository.IsStudentRegisteredToInternshipAsync(studentId, internshipId);
        }
    }
}

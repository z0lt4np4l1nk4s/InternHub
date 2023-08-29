
using InternHub.Model;
using InternHub.Repository.Common;
using InternHub.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service
{
    public class StudyAreaService : IStudyAreaService
    {
        public IStudyAreaRepository Repo { get; set; }

        public StudyAreaService(IStudyAreaRepository repository)
        {
            Repo = repository;
        }

        public async Task<List<StudyArea>> GetAllAsync()
        {
            return await Repo.GetAllAsync();
        }
        public async Task<StudyArea> GetByIdAsync(Guid id)
        {
            return await Repo.GetByIdAsync(id);
        }

        public async Task<bool> AddAsync(StudyArea studyArea, string currentUserId)
        {
            studyArea.DateCreated = studyArea.DateUpdated = DateTime.UtcNow;
            studyArea.CreatedByUserId = studyArea.UpdatedByUserId = currentUserId;
            return await Repo.AddAsync(studyArea);
        }

        public async Task<bool> UpdateAsync(StudyArea studyArea, string currentUserId)
        {
            studyArea.DateUpdated = DateTime.UtcNow;
            studyArea.UpdatedByUserId = currentUserId;
            return await Repo.UpdateAsync(studyArea);
        }

        public async Task<bool> RemoveAsync(StudyArea studyArea, string currentUserId)
        {
            studyArea.UpdatedByUserId = currentUserId;
            studyArea.DateUpdated = DateTime.Now;
            studyArea.IsActive = false;
            return await Repo.RemoveAsync(studyArea);
        }
    }
}

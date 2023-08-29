using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface IStudyAreaService
    {
        Task<List<StudyArea>> GetAllAsync();
        Task<StudyArea> GetByIdAsync(Guid id);
        Task<bool> AddAsync(StudyArea studyArea, string currentUserId);
        Task<bool> UpdateAsync(StudyArea studyArea, string currentUserId);
        Task<bool> RemoveAsync(StudyArea studyArea, string currentUserId);
    }
}

using InternHub.Common;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository.Common
{
    public interface IStudyAreaRepository
    {
        Task<List<StudyArea>> GetAllAsync();
        Task<StudyArea> GetByIdAsync(Guid id);
        Task<bool> AddAsync(StudyArea studyArea);
        Task<bool> UpdateAsync(StudyArea studyArea);
        Task<bool> RemoveAsync(StudyArea studyArea);
    }
}

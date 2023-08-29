using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface ICountyService
    {
        Task<List<County>> GetAllAsync();
        Task<County> GetByIdAsync(Guid id);
        Task<bool> AddAsync(County county, string currentUserId);
        Task<bool> UpdateAsync(County county, string currentUserId);
        Task<bool> RemoveAsync(County county, string currentUserId);
    }
}

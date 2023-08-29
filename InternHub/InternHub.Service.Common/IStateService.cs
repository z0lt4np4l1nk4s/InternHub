using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface IStateService
    {
        Task<List<State>> GetAllAsync();
        Task<State> GetByIdAsync(Guid id);
        Task<State> GetByNameAsync(string name);
        Task<bool> AddAsync(State state, string currentUserId);
        Task<bool> UpdateAsync(State state, string currentUserId);
        Task<bool> RemoveAsync(State state, string currentUserId);
    }
}

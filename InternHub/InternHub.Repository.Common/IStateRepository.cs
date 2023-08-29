using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository.Common
{
    public interface IStateRepository
    {
        Task<List<State>> GetAllAsync();
        Task<State> GetByIdAsync(Guid id);
        Task<State> GetByNameAsync(string name);
        Task<bool> AddAsync(State state);
        Task<bool> UpdateAsync(State state);
        Task<bool> RemoveAsync(State state);
    }
}

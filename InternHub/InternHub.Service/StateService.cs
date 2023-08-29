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
    public class StateService : IStateService
    {
        public IStateRepository Repo { get; }

        public StateService(IStateRepository repository)
        {
            Repo = repository;
        }

        public async Task<List<State>> GetAllAsync()
        {
            return await Repo.GetAllAsync();
        }

        public async Task<State> GetByIdAsync(Guid id)
        {
            return await Repo.GetByIdAsync(id);
        }

        public async Task<State> GetByNameAsync(string name)
        {
            return await Repo.GetByNameAsync(name);
        }

        public async Task<bool> AddAsync(State state, string currentUserId)
        {
            state.DateCreated = state.DateUpdated = DateTime.UtcNow;
            state.CreatedByUserId = state.UpdatedByUserId = currentUserId;
            return await Repo.AddAsync(state);
        }

        public async Task<bool> UpdateAsync(State state, string currentUserId)
        {
            state.DateUpdated = DateTime.UtcNow;
            state.UpdatedByUserId = currentUserId;
            return await Repo.UpdateAsync(state);
        }

        public async Task<bool> RemoveAsync(State state, string currentUserId)
        {
            state.UpdatedByUserId = currentUserId;
            state.DateUpdated = DateTime.Now;
            state.IsActive = false;
            return await Repo.RemoveAsync(state);
        }
    }
}

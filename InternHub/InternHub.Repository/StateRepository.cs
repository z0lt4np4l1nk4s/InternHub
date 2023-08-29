using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    public class StateRepository : IStateRepository
    {
        public IConnectionString ConnectionString { get; set; }

        public StateRepository(IConnectionString connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<List<State>> GetAllAsync()
        {
            List<State> states = new List<State>();
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "SELECT * FROM public.\"State\" WHERE \"IsActive\" = true;";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.HasRows && await reader.ReadAsync())
                {
                    State state = ReadState(reader);
                    if (state != null) states.Add(state);
                }
            }

            return states;
        }

        public async Task<State> GetByIdAsync(Guid id)
        {
            State state = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "SELECT * FROM public.\"State\" where \"IsActive\" = true and \"Id\" = @id;";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    state = ReadState(reader);
                }
            }

            return state;
        }

        public async Task<State> GetByNameAsync(string name)
        {
            State state = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "SELECT * FROM public.\"State\" where \"IsActive\" = true and \"Name\" ilike @name;";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    state = ReadState(reader);
                }
            }

            return state;
        }

        public async Task<bool> AddAsync(State state)
        {
            int numberOfAffectedRows = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "INSERT into public.\"State\" values  (@id, @name, @dateCreated, @dateUpdated, @createdByUserId, @updatedByUserId, @isActive);";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", state.Id);
                command.Parameters.AddWithValue("@name", state.Name);
                command.Parameters.AddWithValue("@dateUpdated", state.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", state.UpdatedByUserId);
                command.Parameters.AddWithValue("@isActive", state.IsActive);
                command.Parameters.AddWithValue("@dateCreated", state.DateCreated);
                command.Parameters.AddWithValue("@createdByUserId", state.CreatedByUserId);

                await connection.OpenAsync();

                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }

            return numberOfAffectedRows != 0;
        }

        public async Task<bool> UpdateAsync(State state)
        {
            int numberOfAffectedRows = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "UPDATE public.\"State\" SET \"Name\" = @name, \"DateUpdated\" = @dateUpdated, \"UpdatedByUserId\" = @updatedByUserId, \"IsActive\" = @isActive where \"Id\" = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", state.Id);
                command.Parameters.AddWithValue("@name", state.Name);
                command.Parameters.AddWithValue("@dateUpdated", state.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", state.UpdatedByUserId);
                command.Parameters.AddWithValue("@isActive", state.IsActive);

                await connection.OpenAsync();

                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }

            return numberOfAffectedRows != 0;
        }

        public async Task<bool> RemoveAsync(State state)
        {
            int numberOfAffectedRows = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "UPDATE public.\"State\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated, \"UpdatedByUserId\" = @updatedByUserId where \"Id\" = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", state.Id);
                command.Parameters.AddWithValue("@dateUpdated", state.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", state.UpdatedByUserId);

                await connection.OpenAsync();

                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }

            return numberOfAffectedRows != 0;
        }

        private State ReadState(NpgsqlDataReader reader)
        {
            try
            {
                return new State
                {
                    Id = (Guid)reader["Id"],
                    Name = Convert.ToString(reader["Name"]),
                    CreatedByUserId = Convert.ToString(reader["CreatedByUserId"]),
                    UpdatedByUserId = Convert.ToString(reader["UpdatedByUserId"]),
                    DateCreated = Convert.ToDateTime(reader["DateCreated"]),
                    DateUpdated = Convert.ToDateTime(reader["DateUpdated"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };
            }
            catch { return null; }
        }
    }
}

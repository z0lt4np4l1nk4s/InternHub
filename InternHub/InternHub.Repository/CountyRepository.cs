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
    //if filter.isactive!=null;
    public class CountyRepository : ICountyRepository
    {
        public IConnectionString ConnectionString { get; set; }

        public CountyRepository(IConnectionString connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<List<County>> GetAllAsync()
        {
            List<County> counties = new List<County>();
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "SELECT * FROM public.\"County\" WHERE \"IsActive\" = true;";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while(reader.HasRows && await reader.ReadAsync())
                {
                    County county = ReadCounty(reader);
                    if (county != null) counties.Add(county);
                }
            }

            return counties;
        }

        public async Task<County> GetByIdAsync(Guid id)
        {
            County county = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "SELECT * FROM public.\"County\" where \"IsActive\" = true and \"Id\" = @id;";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    county = ReadCounty(reader);
                }
            }

            return county;
        }

        public async Task<bool> AddAsync(County county)
        {
            int numberOfAffectedRows = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "INSERT into public.\"County\" values  (@id, @name, @dateCreated, @dateUpdated, @createdByUserId, @updatedByUserId, @isActive);";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", county.Id);
                command.Parameters.AddWithValue("@name", county.Name);
                command.Parameters.AddWithValue("@dateUpdated", county.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", county.UpdatedByUserId);
                command.Parameters.AddWithValue("@isActive", county.IsActive);
                command.Parameters.AddWithValue("@dateCreated", county.DateCreated);
                command.Parameters.AddWithValue("@createdByUserId", county.CreatedByUserId);

                await connection.OpenAsync();

                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }

            return numberOfAffectedRows != 0;
        }

        public async Task<bool> UpdateAsync(County county)
        {
            int numberOfAffectedRows = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "UPDATE public.\"County\" SET \"Name\" = @name, \"DateUpdated\" = @dateUpdated, \"UpdatedByUserId\" = @updatedByUserId, \"IsActive\" = @isActive where \"Id\" = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", county.Id);
                command.Parameters.AddWithValue("@name", county.Name);
                command.Parameters.AddWithValue("@dateUpdated", county.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", county.UpdatedByUserId);
                command.Parameters.AddWithValue("@isActive", county.IsActive);

                await connection.OpenAsync();

                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }

            return numberOfAffectedRows != 0;
        }

        public async Task<bool> RemoveAsync(County county)
        {
            int numberOfAffectedRows = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                string query = "UPDATE public.\"County\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated, \"UpdatedByUserId\" = @updatedByUserId  WHERE \"Id\" = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", county.Id);
                command.Parameters.AddWithValue("@dateUpdated", county.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", county.UpdatedByUserId);

                await connection.OpenAsync();

                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }

            return numberOfAffectedRows != 0;
        }

        private County ReadCounty(NpgsqlDataReader reader)
        {
            try
            {
                return new County
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

using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    internal class StudyAreaRepository : IStudyAreaRepository
    {
        IConnectionString _connectionString;

        public StudyAreaRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<StudyArea>> GetAllAsync()
        {
            List<StudyArea> list = new List<StudyArea>();


            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                NpgsqlCommand command = new NpgsqlCommand();

                command.Connection = connection;

                string selectQuery = "SELECT * FROM \"StudyArea\" sa WHERE sa.\"IsActive\"=true ";

                command.CommandText = selectQuery;

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.HasRows && await reader.ReadAsync())
                {
                    StudyArea studyArea = ReadStudyArea(reader);
                    if (studyArea != null) list.Add(studyArea);
                }
            }
            return list;
        }

        public async Task<StudyArea> GetByIdAsync(Guid id)
        {
            StudyArea studyArea = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                //student, state i internship
                string query = "SELECT * FROM \"StudyArea\" WHERE \"Id\" = @id and \"IsActive\" = true";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                await connection.OpenAsync();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    studyArea = ReadStudyArea(reader);
                }
                return studyArea;

            }
        }


        public async Task<bool> AddAsync(StudyArea studyArea)
        {
            bool success = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                await connection.OpenAsync();

                string studyAreaQuery = "INSERT INTO \"StudyArea\" VALUES (@id,@name,@dateCreated,@dateUpdated,@createdByUserId,@updatedByUserId,@isActive);";

                NpgsqlCommand studyAreaCommand = new NpgsqlCommand(studyAreaQuery, connection);
                studyAreaCommand.Parameters.AddWithValue("@id", studyArea.Id);
                studyAreaCommand.Parameters.AddWithValue("@name", studyArea.Name);
                studyAreaCommand.Parameters.AddWithValue("@dateCreated", studyArea.DateCreated);
                studyAreaCommand.Parameters.AddWithValue("@dateUpdated", studyArea.DateUpdated);
                studyAreaCommand.Parameters.AddWithValue("@createdByUserId", studyArea.CreatedByUserId);
                studyAreaCommand.Parameters.AddWithValue("@updatedByUserId", studyArea.UpdatedByUserId);
                studyAreaCommand.Parameters.AddWithValue("@isActive", studyArea.IsActive);

                int applicationResult = await studyAreaCommand.ExecuteNonQueryAsync();

                success = applicationResult != 0;
            }
            return success;
        }



        public async Task<bool> UpdateAsync(StudyArea studyArea)
        {
            int numberOfAffectedRows = 0;
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                NpgsqlCommand command = new NpgsqlCommand();
                List<string> updatedValues = new List<string>();
                command.Connection = connection;
                command.Parameters.AddWithValue("@id", studyArea.Id);
                command.CommandText = "UPDATE \"StudyArea\" SET \"Name\" = @name WHERE \"Id\" = @id and \"IsActive\" = true";

                command.Parameters.AddWithValue("@name", studyArea.Name);

                await connection.OpenAsync();
                numberOfAffectedRows = await command.ExecuteNonQueryAsync();
            }
            return numberOfAffectedRows != 0;
        }

        public async Task<bool> RemoveAsync(StudyArea studyArea)
        {
            bool success = false;
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                string query = "UPDATE public.\"StudyArea\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated, \"UpdatedByUserId\" = @updatedByUserId  WHERE \"Id\" = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", studyArea.Id);
                command.Parameters.AddWithValue("@dateUpdated", studyArea.DateUpdated);
                command.Parameters.AddWithValue("@updatedByUserId", studyArea.UpdatedByUserId);

                await connection.OpenAsync();
                int studyAreaResult = await command.ExecuteNonQueryAsync();

                success = studyAreaResult != 0;

            }
            return success;
        }

        public StudyArea ReadStudyArea(NpgsqlDataReader reader)
        {
            try
            {
                return new StudyArea
                {
                    Id = (Guid)reader["Id"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    DateUpdated = (DateTime)reader["DateUpdated"],
                    CreatedByUserId = (string)reader["CreatedByUserId"],
                    UpdatedByUserId = (string)reader["UpdatedByUserId"],
                    IsActive = (bool)reader["IsActive"],
                    Name = (string)reader["Name"],
                };
            }
            catch { return null; }
        }
    }
}

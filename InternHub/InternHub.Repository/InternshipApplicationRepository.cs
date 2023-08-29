using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    public class InternshipApplicationRepository : IInternshipApplicationRepository
    {
        IConnectionString _connectionString;

        public InternshipApplicationRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> DeleteAsync(InternshipApplication internshipApplication)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                connection.Open();

  
                string query = $"UPDATE public.\"InternshipApplication\" SET \"IsActive\" = false, \"UpdatedByUserId\" = @updatedByUserId WHERE \"Id\" = @id";


                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@updatedByUserId", internshipApplication.UpdatedByUserId);
                command.Parameters.AddWithValue("@id", internshipApplication.Id);
                int rowsUpdated = await command.ExecuteNonQueryAsync();

                return rowsUpdated > 0;

            }
        }

        private string getQuery = @"
                     SELECT 
                    ia.""Id"",
                    ia.""DateCreated"",
                    ia.""DateUpdated"",
                    ia.""CreatedByUserId"",
                    ia.""UpdatedByUserId"",
                    ia.""IsActive"",
                    ia.""StateId"",
                    ia.""StudentId"",
                    ia.""InternshipId"",
                    u.""FirstName"" as ""StudentFirstName"",
                    u.""LastName"" as ""StudentLastName"",
                    u.""Email"" as ""StudentEmail"",
                    u.""PhoneNumber"" as ""StudentPhoneNumber"",
                    u.""Address"" as ""StudentAddress"",
                    u.""Description"" as ""StudentDescription"",
                    c.""Name"" AS ""StudentCounty"",
                    sas.""Name"" AS ""StudentStudyArea"",
                    sa.""Id"" as ""InternshipStudyAreaId"",
                    sa.""Name"" AS ""InternshipStudyArea"",
                    i.""Id"" as ""InternshipId"",
                    i.""CompanyId"",
                    comp.""Name"" AS ""CompanyName"",
                    comp.""Website"" as ""CompanyWebsite"",
                    i.""Name"" AS ""InternshipName"",
                    i.""Description"" AS ""InternshipDescription"",
                    i.""Address"" AS ""InternshipAddress"",
                    i.""StartDate"",
                    i.""EndDate"",
                    sta.""Name"" AS ""InternshipApplicationStateName""
                FROM 
                    ""InternshipApplication"" ia
                    INNER JOIN ""Student"" s ON ia.""StudentId"" = s.""Id""
                    INNER JOIN ""State"" sta ON ia.""StateId"" = sta.""Id""
                    INNER JOIN dbo.""AspNetUsers"" u ON u.""Id"" = s.""Id""
                    INNER JOIN ""County"" c ON c.""Id"" = u.""CountyId""
                    INNER JOIN ""Internship"" i ON i.""Id"" = ia.""InternshipId""
                    INNER JOIN ""Company"" comp ON i.""CompanyId"" = comp.""Id""
                    inner join ""StudyArea"" sa on i.""StudyAreaId"" =sa.""Id""
                    inner join ""StudyArea"" sas on s.""StudyAreaId""=sas.""Id""";

        public async Task<PagedList<InternshipApplication>> GetAllInternshipApplicationsAsync(Paging paging, Sorting sorting, InternshipApplicationFilter filter)
        {
            PagedList<InternshipApplication> pagedList = new PagedList<InternshipApplication>();
            //umjesto where 1=1 isactive=1
            if (paging == null) paging = new Paging();
            if (sorting == null) sorting = new Sorting();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.Parameters.AddWithValue("@pageSize", paging.PageSize);
                command.Parameters.AddWithValue("@skip", (paging.CurrentPage - 1) * paging.PageSize);

                string sortBy = (sorting.SortBy.ToLower() == "Id".ToLower() ? "s" : "ia") + ".\"" + sorting.SortBy + "\"";

                List<string> parameters = new List<string>();

                SetFilterParameters(parameters, command, filter);
                string selectQuery = getQuery +

                "WHERE ia.\"IsActive\" = true " +
                    (parameters.Count == 0 ? "" : "AND " + string.Join(" AND ", parameters)) + $" ORDER BY {sortBy} {(sorting.SortOrder.ToLower() == "asc" ? "ASC" : "DESC")} LIMIT @pageSize OFFSET @skip";

                string countQuery = "SELECT COUNT(*) FROM \r\n                    \"InternshipApplication\" ia\r\n                    INNER JOIN \"Student\" s ON ia.\"StudentId\" = s.\"Id\"\r\n                    INNER JOIN \"State\" sta ON ia.\"StateId\" = sta.\"Id\"\r\n                    INNER JOIN dbo.\"AspNetUsers\" u ON u.\"Id\" = s.\"Id\"\r\n                    INNER JOIN \"County\" c ON c.\"Id\" = u.\"CountyId\"\r\n                    INNER JOIN \"Internship\" i ON i.\"Id\" = ia.\"InternshipId\"\r\n                    INNER JOIN \"Company\" comp ON i.\"CompanyId\" = comp.\"Id\"\r\n                    inner join \"StudyArea\" sa on i.\"StudyAreaId\" =sa.\"Id\"\r\n                    inner join \"StudyArea\" sas on s.\"StudyAreaId\"=sas.\"Id\"  WHERE ia.\"IsActive\" = true " + (parameters.Count == 0 ? "" : "AND " + string.Join(" AND ", parameters));

                NpgsqlCommand countCommand = new NpgsqlCommand(countQuery, connection);

                foreach (NpgsqlParameter npgsqlParameter in command.Parameters) countCommand.Parameters.AddWithValue(npgsqlParameter.ParameterName, npgsqlParameter.Value);

                command.CommandText = selectQuery;

                await connection.OpenAsync();

                object countResult = await countCommand.ExecuteScalarAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (countResult == null) return pagedList;

                while (reader.HasRows && await reader.ReadAsync())
                {
                    InternshipApplication internshipApplication = ReadInternshipApplication(reader);
                    if (internshipApplication != null) pagedList.Data.Add(internshipApplication);
                }
                pagedList.CurrentPage = paging.CurrentPage;
                pagedList.PageSize = paging.PageSize;
                pagedList.DatabaseRecordsCount = Convert.ToInt32(countResult);
                pagedList.LastPage = Convert.ToInt32(pagedList.DatabaseRecordsCount / paging.PageSize) + (pagedList.DatabaseRecordsCount % paging.PageSize != 0 ? 1 : 0);
            }
            return pagedList;
        }

        public async Task<PagedList<InternshipApplication>> GetUnacceptedAsync(Paging paging, Sorting sorting, InternshipApplicationFilter filter)
        {
            PagedList<InternshipApplication> pagedList = new PagedList<InternshipApplication>();
            //umjesto where 1=1 isactive=1
            if (paging == null) paging = new Paging();
            if (sorting == null) sorting = new Sorting();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.Parameters.AddWithValue("@pageSize", paging.PageSize);
                command.Parameters.AddWithValue("@skip", (paging.CurrentPage - 1) * paging.PageSize);

                string sortBy = (sorting.SortBy.ToLower() == "Id".ToLower() ? "s" : "ia") + ".\"" + sorting.SortBy + "\"";

                List<string> parameters = new List<string>
                {
                    "sta.\"Name\" ilike '%Processing%'"
                };

                SetFilterParameters(parameters, command, filter);
                string selectQuery = getQuery +

                "WHERE ia.\"IsActive\" = true " +
                    (parameters.Count == 0 ? "" : "AND " + string.Join(" AND ", parameters)) + $" ORDER BY {sortBy} {(sorting.SortOrder.ToLower() == "asc" ? "ASC" : "DESC")} LIMIT @pageSize OFFSET @skip";

                string countQuery = "SELECT COUNT(*) FROM \r\n                    \"InternshipApplication\" ia\r\n                    INNER JOIN \"Student\" s ON ia.\"StudentId\" = s.\"Id\"\r\n                    INNER JOIN \"State\" sta ON ia.\"StateId\" = sta.\"Id\"\r\n                    INNER JOIN dbo.\"AspNetUsers\" u ON u.\"Id\" = s.\"Id\"\r\n                    INNER JOIN \"County\" c ON c.\"Id\" = u.\"CountyId\"\r\n                    INNER JOIN \"Internship\" i ON i.\"Id\" = ia.\"InternshipId\"\r\n                    INNER JOIN \"Company\" comp ON i.\"CompanyId\" = comp.\"Id\"\r\n                    inner join \"StudyArea\" sa on i.\"StudyAreaId\" =sa.\"Id\"\r\n                    inner join \"StudyArea\" sas on s.\"StudyAreaId\"=sas.\"Id\"  WHERE ia.\"IsActive\" = true " + (parameters.Count == 0 ? "" : "AND " + string.Join(" AND ", parameters));

                NpgsqlCommand countCommand = new NpgsqlCommand(countQuery, connection);

                foreach (NpgsqlParameter npgsqlParameter in command.Parameters) countCommand.Parameters.AddWithValue(npgsqlParameter.ParameterName, npgsqlParameter.Value);

                command.CommandText = selectQuery;

                await connection.OpenAsync();

                object countResult = await countCommand.ExecuteScalarAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (countResult == null) return pagedList;

                while (reader.HasRows && await reader.ReadAsync())
                {
                    InternshipApplication internshipApplication = ReadInternshipApplication(reader);
                    if (internshipApplication != null) pagedList.Data.Add(internshipApplication);
                }
                pagedList.CurrentPage = paging.CurrentPage;
                pagedList.PageSize = paging.PageSize;
                pagedList.DatabaseRecordsCount = Convert.ToInt32(countResult);
                pagedList.LastPage = Convert.ToInt32(pagedList.DatabaseRecordsCount / paging.PageSize) + (pagedList.DatabaseRecordsCount % paging.PageSize != 0 ? 1 : 0);
            }
            return pagedList;

        }

        public async Task<Guid?> GetIdAsync(string studentId, Guid internshipId)
        {
            using(NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                connection.Open();

                string query = "select \"Id\" " +
                    "from public.\"InternshipApplication\" ia " +
                    "where \"StudentId\" = @studentId and \"InternshipId\" = @internshipId  and \"IsActive\" = true";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@internshipId", internshipId);

                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    return (Guid)result;
                }
                return null;

                
            }
        }

        public async Task<InternshipApplication> GetInternshipApplicationByIdAsync(Guid id)
        {

            InternshipApplication internshipApplication = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                //student, state i internship
                string query = getQuery + "WHERE ia.\"Id\" = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                await connection.OpenAsync();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    internshipApplication = ReadInternshipApplication(reader);
                }
                return internshipApplication;

            }
        }

        public async Task<InternshipApplication> GetByInternshipAsync(Guid internshipId, string studentId)
        {

            InternshipApplication internshipApplication = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                string query = getQuery + "WHERE ia.\"InternshipId\" = @internshipId and \"StudentId\" = @studentId";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@internshipId", internshipId);
                command.Parameters.AddWithValue("@studentId", studentId);
                await connection.OpenAsync();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    internshipApplication = ReadInternshipApplication(reader);
                }
                return internshipApplication;

            }

        }

        public async Task<bool> PostInternshipApplicationAsync(InternshipApplication internshipApplication)
        {

            bool success = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                //message
                connection.Open();

 

                string applicationQuery = "INSERT INTO \"InternshipApplication\" VALUES (@id,@stateId,@studentId,@internshipId,@message,@dateCreated,@DateUpdated,@CreatedByUserId,@UpdatedByUserId,@IsActive);";

                NpgsqlCommand applicationCommand = new NpgsqlCommand(applicationQuery, connection);
                applicationCommand.Parameters.AddWithValue("@id", internshipApplication.Id);
                applicationCommand.Parameters.AddWithValue("@dateCreated", internshipApplication.DateCreated);
                applicationCommand.Parameters.AddWithValue("@dateUpdated", internshipApplication.DateUpdated);
                applicationCommand.Parameters.AddWithValue("@createdByUserId", internshipApplication.CreatedByUserId);
                applicationCommand.Parameters.AddWithValue("@updatedByUserId", internshipApplication.UpdatedByUserId);
                applicationCommand.Parameters.AddWithValue("@isActive", internshipApplication.IsActive);
                applicationCommand.Parameters.AddWithValue("@stateId", internshipApplication.StateId);
                applicationCommand.Parameters.AddWithValue("@message", internshipApplication.Message);
                applicationCommand.Parameters.AddWithValue("@studentId", internshipApplication.StudentId);
                applicationCommand.Parameters.AddWithValue("@internshipId", internshipApplication.InternshipId);

                int applicationResult = await applicationCommand.ExecuteNonQueryAsync();

                success = applicationResult != 0;
            }
            return success;
        }

        public async Task<bool> PutAsync(InternshipApplication internshipApplication)
        {
            bool success = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                connection.Open();

                string applicationQuery = "UPDATE \"InternshipApplication\" set \"StateId\" = @stateId, \"UpdatedByUserId\" = @updatedByUserId, \"DateUpdated\" = @dateUpdated WHERE \"Id\" = @id;";

                NpgsqlCommand applicationCommand = new NpgsqlCommand(applicationQuery, connection);
                applicationCommand.Parameters.AddWithValue("@id", internshipApplication.Id);
                applicationCommand.Parameters.AddWithValue("@dateUpdated", internshipApplication.DateUpdated);
                applicationCommand.Parameters.AddWithValue("@updatedByUserId", internshipApplication.UpdatedByUserId);
                applicationCommand.Parameters.AddWithValue("@stateId", internshipApplication.StateId);

                int applicationResult = await applicationCommand.ExecuteNonQueryAsync();

                success = applicationResult != 0;
            }
            return success;
        }



        public InternshipApplication ReadInternshipApplication(NpgsqlDataReader reader)
        {
            try
            {
                return new InternshipApplication
                {
                    Id = (Guid)reader["Id"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    DateUpdated = (DateTime)reader["DateUpdated"],
                    CreatedByUserId = (string)reader["CreatedByUserId"],
                    UpdatedByUserId = (string)reader["UpdatedByUserId"],
                    IsActive = (bool)reader["IsActive"],
                    StateId = (Guid)reader["StateId"],
                    StudentId = (string)reader["StudentId"],
                    InternshipId = (Guid)reader["InternshipId"],
                    Student = new Student()
                    {
                        Id = (string)reader["StudentId"],
                        FirstName = (string)reader["StudentFirstName"],
                        LastName = (string)reader["StudentLastName"],
                        Email = (string)reader["StudentEmail"],
                        PhoneNumber = (string)reader["StudentPhoneNumber"],
                        Address = (string)reader["StudentAddress"],
                        Description = (string)reader["StudentDescription"],
                        County = new County { Name = (string)reader["StudentCounty"] },
                        StudyArea = new StudyArea { Name = (string)reader["StudentStudyArea"] },
                    },
                    Internship = new Internship()
                    {
                        Id = (Guid)reader["InternshipId"],
                        StudyAreaId = (Guid)reader["InternshipStudyAreaId"],
                        StudyArea=new StudyArea { Name = (string)reader["InternshipStudyArea"] },
                        CompanyId = (string)reader["CompanyId"],
                        Company = new Company { Name = (string)reader["CompanyName"], Website = (string)reader["CompanyWebsite"] },
                        Name = (string)reader["InternshipName"],
                        Description = (string)reader["InternshipDescription"],
                        Address = (string)reader["InternshipAddress"],
                        StartDate = (DateTime)reader["StartDate"],
                        EndDate = (DateTime)reader["EndDate"]
                    },
                    State = new State()
                    {
                        Id = (Guid)reader["StateId"],
                        Name = (string)reader["InternshipApplicationStateName"],
                    },
                    

                };
            }
            catch { return null; }
        }

        private void SetFilterParameters(List<string> parameters, NpgsqlCommand command, InternshipApplicationFilter filter)
        {
            if (filter != null)
            {
                if (filter.States != null && filter.States.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();

                    command.Parameters.AddWithValue("@stateId", filter.States);
                    for (int i = 0; i < filter.States.Count; i++)
                    {
                        string parameterName = $"@stateId{i}";
                        builder.Append(parameterName);

                        if (i < filter.States.Count - 1)
                        {
                            builder.Append(", ");
                        }
                        command.Parameters.AddWithValue(parameterName, filter.States[i]);
                    }
                    parameters.Add("sta.\"Id\" in (" + builder.ToString() + ")");



                }
                if (filter.CompanyId != null)
                {
                    parameters.Add("i.\"CompanyId\" = @companyId");
                    command.Parameters.AddWithValue("@companyId", filter.CompanyId);
                }
                if (filter.InternshipName != null)
                {
                    parameters.Add("i.\"Name\" ILIKE @internshipName");
                    command.Parameters.AddWithValue("@internshipName", "%" + filter.InternshipName + "%");
                }
                if (filter.StudentId != null)
                {
                    parameters.Add("s.\"Id\" = @userId");
                    command.Parameters.AddWithValue("@userId", filter.StudentId);
                }
                if (filter.CompanyName != null)
                {
                    parameters.Add("comp.\"Name\" ILIKE @companyName");
                    command.Parameters.AddWithValue("@companyName", "%" + filter.CompanyName + "%");
                }
                if (filter.FirstName != null)
                {
                    parameters.Add("u.\"FirstName\" ILIKE @firstName");
                    command.Parameters.AddWithValue("@firstName", "%" + filter.FirstName + "%");
                }
                if (filter.LastName != null)
                {
                    parameters.Add("u.\"LastName\" ILIKE @lastName");
                    command.Parameters.AddWithValue("@lastName", "%" + filter.LastName + "%");
                }
            }
        }
    }
}

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
    public class StudentRepository : IStudentRepository
    {
        IConnectionString _connectionString;

        public StudentRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Student>> GetByInternship(Guid internshipId)
        {
            List<Student> students = new List<Student>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                NpgsqlCommand command = new NpgsqlCommand("select * from \"Student\" s inner join dbo.\"AspNetUsers\" anu on s.\"Id\" = anu.\"Id\"  inner join \"InternshipApplication\" ia on s.\"Id\" = ia.\"StudentId\" inner join \"State\" sta on sta.\"Id\" = ia.\"StateId\" where ia.\"IsActive\" = true and anu.\"IsActive\" = true and ia.\"InternshipId\" = @internshipId and sta.\"Name\" ilike '%Accepted%'", connection);
                command.Parameters.AddWithValue("@internshipId", internshipId);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.HasRows && await reader.ReadAsync())
                {
                    Student student = ReadStudentAsAdmin(reader);
                    students.Add(student);
                }
            }
            return students;
        }

        public async Task<PagedList<Student>> GetStudentViewAsAdminAsync(Sorting sorting, Paging paging, StudentFilter filter)
        {
            PagedList<Student> pagedStudents = new PagedList<Student>()
            {
                CurrentPage = paging.CurrentPage,
                PageSize = paging.PageSize,
            };

            StringBuilder selectQueryBuilder = new StringBuilder();
            StringBuilder countQueryBuilder = new StringBuilder();
            StringBuilder filterQueryBuilder = new StringBuilder();

            List<Student> students = new List<Student>();

            int totalCount;


            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name.Replace("\"", "")))
            {
                await connection.OpenAsync();

                string selectQuery = "SELECT * FROM public.\"Student\" s inner join dbo.\"AspNetUsers\" u on u.\"Id\" = s.\"Id\" ";
                string countQuery = "SELECT COUNT(*) FROM public.\"Student\" s inner join dbo.\"AspNetUsers\" u on u.\"Id\" = s.\"Id\" ";
                string initialFilterCondition = " WHERE u.\"IsActive\" = true ";
                //string initialFilterCondition = " WHERE 1=1 ";

                selectQueryBuilder.Append(selectQuery);
                selectQueryBuilder.Append(initialFilterCondition);

                countQueryBuilder.Append(countQuery);
                countQueryBuilder.Append(initialFilterCondition);

                //if(filter.IsActive != null)
                //{
                //    filterQueryBuilder.Append($" AND u.\"IsActive\" = " + filter.IsActive.Value);
                //}

                if (string.IsNullOrEmpty(filter.FirstName) == false)
                {
                    filterQueryBuilder.Append($" AND u.\"FirstName\" ILIKE '{filter.FirstName}%'");
                }

                if (string.IsNullOrEmpty(filter.LastName) == false)
                {
                    filterQueryBuilder.Append($" AND u.\"LastName\" ILIKE '{filter.LastName}%'");
                }


                if (filter.Counties.Count != 0)
                {
                    filterQueryBuilder.Append("AND u.\"CountyId\" IN (");

                    for (int i = 0; i < filter.Counties.Count; i++)
                    {
                        filterQueryBuilder.Append($"'{filter.Counties[i]}'");

                        if (i != filter.Counties.Count - 1)
                        {
                            filterQueryBuilder.Append(",");
                        }
                    }

                    filterQueryBuilder.Append(") ");
                }

                if (filter.StudyAreas.Count != 0)
                {
                    filterQueryBuilder.Append("AND s.\"StudyAreaId\" IN (");

                    for (int counter = 0; counter < filter.StudyAreas.Count; counter++)
                    {
                        filterQueryBuilder.Append($"'{filter.StudyAreas[counter]}'");

                        if (counter != filter.StudyAreas.Count - 1)
                        {
                            filterQueryBuilder.Append(",");
                        }

                    }
                    filterQueryBuilder.Append(") ");

                }

                string sortingQuery = $" ORDER BY u.\"{sorting.SortBy}\" {sorting.SortOrder} ";
                string pagingQuery = $" LIMIT {paging.PageSize} OFFSET {(paging.CurrentPage - 1) * paging.PageSize}";

                selectQueryBuilder.Append(filterQueryBuilder.ToString());
                selectQueryBuilder.Append(sortingQuery);
                selectQueryBuilder.Append(pagingQuery);


                string sql = selectQueryBuilder.ToString();

                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                NpgsqlDataReader dr = await cmd.ExecuteReaderAsync();

                if (dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        Student student = ReadStudentAsAdmin(dr);
                        students.Add(student);
                    }
                }
                dr.Close();
                countQueryBuilder.Append(filterQueryBuilder.ToString());
                string s = countQueryBuilder.ToString();
                NpgsqlCommand countCommand = new NpgsqlCommand(countQueryBuilder.ToString(), connection);
                var countScalar = await countCommand.ExecuteScalarAsync();
                totalCount = Convert.ToInt32(countScalar);

                connection.Close();
            }

            pagedStudents.Data = students;
            pagedStudents.DatabaseRecordsCount = totalCount;
            pagedStudents.LastPage = Convert.ToInt32(pagedStudents.DatabaseRecordsCount / paging.PageSize) + (pagedStudents.DatabaseRecordsCount % paging.PageSize != 0 ? 1 : 0);

            return pagedStudents;

        }
        private Student ReadStudentAsAdmin(NpgsqlDataReader dr)
        {
            string id = dr.GetString(dr.GetOrdinal("Id"));
            string firstName = dr.GetString(dr.GetOrdinal("FirstName"));
            string lastName = dr.GetString(dr.GetOrdinal("LastName"));
            string email = dr.GetString(dr.GetOrdinal("Email"));


            return new Student(id, firstName, lastName, email);
        }

        public async Task<PagedList<Student>> GetStudentsAsync(Sorting sorting, Paging paging, StudentFilter filter)
        {
            if (sorting == null) sorting = new Sorting();
            if (paging == null) paging = new Paging();
            if (filter == null) filter = new StudentFilter();

            PagedList<Student> pagedStudents = new PagedList<Student>()
            {
                CurrentPage = paging.CurrentPage,
                PageSize = paging.PageSize,
            };

            StringBuilder selectQueryBuilder = new StringBuilder();
            StringBuilder countQueryBuilder = new StringBuilder();
            StringBuilder filterQueryBuilder = new StringBuilder();

            List<Student> students = new List<Student>();

            int totalCount;


            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name.Replace("\"", "")))
            {
                await connection.OpenAsync();

                string selectQuery = "SELECT s.*, u.*, c.\"Name\" as \"CountyName\", sa.\"Name\" as \"StudyAreaName\" FROM public.\"Student\" s inner join dbo.\"AspNetUsers\" u on u.\"Id\" = s.\"Id\" inner join public.\"County\" c on c.\"Id\" = u.\"CountyId\" inner join public.\"StudyArea\" sa on sa.\"Id\" = s.\"StudyAreaId\" ";
                string countQuery = "SELECT COUNT(*) FROM public.\"Student\" s inner join dbo.\"AspNetUsers\" u on u.\"Id\" = s.\"Id\" inner join public.\"County\" c on c.\"Id\" = u.\"CountyId\" inner join public.\"StudyArea\" sa on sa.\"Id\" = s.\"StudyAreaId\" ";
                //string initialFilterCondition = " WHERE u.\"IsActive\" = true ";
                string initialFilterCondition = " WHERE 1=1 ";

                selectQueryBuilder.Append(selectQuery);
                selectQueryBuilder.Append(initialFilterCondition);

                countQueryBuilder.Append(countQuery);
                countQueryBuilder.Append(initialFilterCondition);

                filterQueryBuilder.Append($" AND u.\"IsActive\" = " + filter.IsActive);


                if (string.IsNullOrEmpty(filter.FirstName) == false)
                {
                    filterQueryBuilder.Append($" AND u.\"FirstName\" ILIKE '{filter.FirstName}%'");
                }

                if (string.IsNullOrEmpty(filter.LastName) == false)
                {
                    filterQueryBuilder.Append($" AND u.\"LastName\" ILIKE '{filter.LastName}%'");
                }


                if (filter.Counties != null && filter.Counties.Count != 0)
                {
                    filterQueryBuilder.Append(" AND u.\"CountyId\" IN (");

                    for (int i = 0; i < filter.Counties.Count; i++)
                    {
                        filterQueryBuilder.Append($"'{filter.Counties[i]}'");

                        if (i != filter.Counties.Count - 1)
                        {
                            filterQueryBuilder.Append(",");
                        }
                    }

                    filterQueryBuilder.Append(") ");
                }

                if (filter.StudyAreas != null && filter.StudyAreas.Count != 0)
                {
                    filterQueryBuilder.Append(" AND s.\"StudyAreaId\" IN (");

                    for (int counter = 0; counter < filter.StudyAreas.Count; counter++)
                    {
                        filterQueryBuilder.Append($"'{filter.StudyAreas[counter]}'");

                        if (counter != filter.StudyAreas.Count - 1)
                        {
                            filterQueryBuilder.Append(",");
                        }

                    }
                    filterQueryBuilder.Append(") ");

                }



                string sortingQuery = $" ORDER BY u.\"{sorting.SortBy}\" {sorting.SortOrder} ";
                string pagingQuery = $" LIMIT {paging.PageSize} OFFSET {(paging.CurrentPage - 1) * paging.PageSize}";

                selectQueryBuilder.Append(filterQueryBuilder.ToString());
                selectQueryBuilder.Append(sortingQuery);
                selectQueryBuilder.Append(pagingQuery);


                string sql = selectQueryBuilder.ToString();

                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                NpgsqlDataReader dr = await cmd.ExecuteReaderAsync();

                if (dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        Student student = ReadStudent(dr);
                        if (student != null) students.Add(student);
                    }
                }
                dr.Close();
                countQueryBuilder.Append(filterQueryBuilder.ToString());
                string s = countQueryBuilder.ToString();
                NpgsqlCommand countCommand = new NpgsqlCommand(countQueryBuilder.ToString(), connection);
                var countScalar = await countCommand.ExecuteScalarAsync();
                totalCount = Convert.ToInt32(countScalar);

                connection.Close();
            }

            pagedStudents.Data = students;
            pagedStudents.DatabaseRecordsCount = totalCount;
            pagedStudents.LastPage = Convert.ToInt32(pagedStudents.DatabaseRecordsCount / paging.PageSize) + (pagedStudents.DatabaseRecordsCount % paging.PageSize != 0 ? 1 : 0);

            return pagedStudents;
        }


        private Student ReadStudent(NpgsqlDataReader dr)
        {
            string id = dr.GetString(dr.GetOrdinal("Id"));
            string firstName = dr.GetString(dr.GetOrdinal("FirstName"));
            string lastName = dr.GetString(dr.GetOrdinal("LastName"));
            string email = dr.GetString(dr.GetOrdinal("Email"));
            string phoneNumber = dr.GetString(dr.GetOrdinal("PhoneNumber"));
            string address = dr.GetString(dr.GetOrdinal("Address"));
            string description = dr.GetString(dr.GetOrdinal("Description"));
            DateTime dateCreated = dr.GetDateTime(dr.GetOrdinal("DateCreated"));
            DateTime dateUpdated = dr.GetDateTime(dr.GetOrdinal("DateUpdated"));
            Guid countyId = dr.GetGuid(dr.GetOrdinal("CountyId"));
            bool isActive = dr.GetBoolean(dr.GetOrdinal("IsActive"));
            Guid studyAreaId = dr.GetGuid(dr.GetOrdinal("StudyAreaId"));

            Student student = new Student(id, firstName, lastName, email, phoneNumber, address, description, dateCreated, dateUpdated, countyId, isActive, studyAreaId);
            student.County = new County
            {
                Id = countyId,
                Name = (string)dr["CountyName"]
            };
            student.StudyArea = new StudyArea()
            {
                Id = studyAreaId,
                Name = (string)dr["StudyAreaName"]
            };
            return student;
        }

        public async Task<Student> GetStudentByIdAsync(string id)
        {
            Student student = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                string query = @"
                SELECT s.*, u.*, c.""Name"" as ""CountyName"", sa.""Name"" as ""StudyAreaName""
                FROM public.""Student"" s
                INNER JOIN public.""StudyArea"" sa ON s.""StudyAreaId"" = sa.""Id"" inner join dbo.""AspNetUsers"" u on s.""Id"" = u.""Id""
                INNER JOIN public.""County"" c on u.""CountyId"" = c.""Id""
                WHERE s.""Id"" = @Id";


                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    student = ReadStudent(reader);
                }
            }
            return student;
        }

        public async Task<int> PostAsync(Student student)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertUserQuery = "INSERT INTO dbo.\"AspNetUsers\" (\"Id\", \"FirstName\", \"LastName\", \"Address\", \"Description\", \"CountyId\", \"IsActive\", \"DateCreated\", \"DateUpdated\", \"Email\", \"EmailConfirmed\", \"PasswordHash\", \"SecurityStamp\", \"PhoneNumber\", \"PhoneNumberConfirmed\", \"TwoFactorEnabled\", \"LockoutEndDateUtc\", \"LockoutEnabled\", \"AccessFailedCount\", \"UserName\", \"Discriminator\") VALUES (@id, @firstname, @lastname, @address, @description, @countyId, true, @dateCreated, @dateUpdated, @email, false, @password, 'security_stamp', @phoneNumber, false, false, null, false, 0, @username, 'User')";
                        string insertStudentQuery = "INSERT INTO public.\"Student\" (\"Id\", \"StudyAreaId\") VALUES (@id, @studyAreaId)";


                        NpgsqlCommand studentInsertCommand = new NpgsqlCommand(insertStudentQuery, connection, transaction);
                        NpgsqlCommand userInsertCommand = new NpgsqlCommand(insertUserQuery, connection, transaction);

                        userInsertCommand.Parameters.AddWithValue("@id", student.Id);
                        userInsertCommand.Parameters.AddWithValue("@firstname", student.FirstName);
                        userInsertCommand.Parameters.AddWithValue("@lastname", student.LastName);
                        userInsertCommand.Parameters.AddWithValue("@address", student.Address);
                        userInsertCommand.Parameters.AddWithValue("@description", student.Description);
                        userInsertCommand.Parameters.AddWithValue("@countyId", student.CountyId);
                        userInsertCommand.Parameters.AddWithValue("@dateCreated", student.DateCreated);
                        userInsertCommand.Parameters.AddWithValue("@dateUpdated", student.DateUpdated);
                        userInsertCommand.Parameters.AddWithValue("@email", student.Email);
                        userInsertCommand.Parameters.AddWithValue("@phoneNumber", student.PhoneNumber);
                        userInsertCommand.Parameters.AddWithValue("@username", student.Email);
                        userInsertCommand.Parameters.AddWithValue("@isActive", student.IsActive = true);
                        userInsertCommand.Parameters.AddWithValue("@password", student.Password);

                        studentInsertCommand.Parameters.AddWithValue("@id", student.Id);
                        studentInsertCommand.Parameters.AddWithValue("@studyAreaId", student.StudyAreaId);

                        string userRoleQuery = "INSERT INTO dbo.\"AspNetUserRoles\" VALUES (@userId, @roleId)";
                        NpgsqlCommand userRoleCommand = new NpgsqlCommand(userRoleQuery, connection);
                        userRoleCommand.Parameters.AddWithValue("@userId", student.Id);
                        userRoleCommand.Parameters.AddWithValue("@roleId", student.RoleId);

                        int numberOfUserRowsAffected = await userInsertCommand.ExecuteNonQueryAsync();
                        int numberOfStudentRowsAffected = await studentInsertCommand.ExecuteNonQueryAsync();
                        int userRoleRowsAffected = await userRoleCommand.ExecuteNonQueryAsync();

                        int result = numberOfUserRowsAffected * numberOfStudentRowsAffected * userRoleRowsAffected;

                        if (result > 0)
                            await transaction.CommitAsync();
                        else
                            await transaction.RollbackAsync();

                        return result;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return 0;
                    }
                }
            }
        }

        public async Task<int> DeleteAsync(Student student)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                await connection.OpenAsync();


                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateUserQuery = "UPDATE dbo.\"AspNetUsers\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated WHERE \"Id\" = @id";


                        NpgsqlCommand updateUserCommand = new NpgsqlCommand(updateUserQuery, connection, transaction);

                        updateUserCommand.Parameters.AddWithValue("@id", student.Id);
                        updateUserCommand.Parameters.AddWithValue("@dateUpdated", student.DateUpdated);

                        string internshipApplicationQuery = $"UPDATE public.\"InternshipApplication\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated WHERE \"StudentId\" = @studentId;";
                        NpgsqlCommand internshipApplicationCommand = new NpgsqlCommand(internshipApplicationQuery, connection, transaction);
                        internshipApplicationCommand.Parameters.AddWithValue("@studentId", student.Id);
                        internshipApplicationCommand.Parameters.AddWithValue("@dateUpdated", student.DateUpdated);

                        int userResult = await updateUserCommand.ExecuteNonQueryAsync();
                        int internshipApplicationResult = await internshipApplicationCommand.ExecuteNonQueryAsync();

                        int result = userResult * internshipApplicationResult;

                        if (result > 0)
                            await transaction.CommitAsync();
                        else
                            await transaction.RollbackAsync();

                        return result;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return 0;
                    }
                }

            }
        }

        public async Task<int> PutAsync(Student student)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString.Name))
            {
                await connection.OpenAsync();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateStudentQuery = "UPDATE public.\"Student\" SET \"StudyAreaId\" = @studyAreaId WHERE \"Id\" = @id ";

                        string updateUserQuery = "UPDATE dbo.\"AspNetUsers\" SET \"FirstName\" = @firstName, \"LastName\" = @lastName, \"Address\" = @address, \"Description\" = @description, \"PhoneNumber\" = @phoneNumber, \"Email\" = @email, \"DateUpdated\" = @dateUpdated, \"CountyId\" = @countyId WHERE \"Id\" = @id";

                        NpgsqlCommand updateStudentCommand = new NpgsqlCommand(updateStudentQuery, connection);
                        NpgsqlCommand updateUserCommand = new NpgsqlCommand(updateUserQuery, connection);

                        updateStudentCommand.Parameters.AddWithValue("@id", student.Id);
                        updateStudentCommand.Parameters.AddWithValue("@studyAreaId", student.StudyAreaId);

                        updateUserCommand.Parameters.AddWithValue("@id", student.Id);
                        updateUserCommand.Parameters.AddWithValue("@firstName", student.FirstName);
                        updateUserCommand.Parameters.AddWithValue("@lastName", student.LastName);
                        updateUserCommand.Parameters.AddWithValue("@address", student.Address);
                        updateUserCommand.Parameters.AddWithValue("@description", student.Description);
                        updateUserCommand.Parameters.AddWithValue("@phoneNumber", student.PhoneNumber);
                        updateUserCommand.Parameters.AddWithValue("@email", student.Email);
                        updateUserCommand.Parameters.AddWithValue("@dateUpdated", student.DateUpdated);
                        updateUserCommand.Parameters.AddWithValue("@countyId", student.CountyId);

                        int numberOfUserRowsAffected = await updateStudentCommand.ExecuteNonQueryAsync();
                        int numberOfStudentRowsAffected = await updateUserCommand.ExecuteNonQueryAsync();

                        if (numberOfUserRowsAffected * numberOfStudentRowsAffected > 0)
                            await transaction.CommitAsync();
                        else
                            await transaction.RollbackAsync();

                        return numberOfStudentRowsAffected * numberOfUserRowsAffected;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return 0;
                    }
                }
            }
        }
    }

}




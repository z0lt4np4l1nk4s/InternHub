using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    public class InternshipRepository : IInternshipRepository
    {

        private readonly IConnectionString connectionString;
        public InternshipRepository(IConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<bool> DeleteAsync(Internship internship)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    string companyQuery = $"UPDATE public.\"Internship\" SET \"IsActive\" = false, \"UpdatedByUserId\" = @updatedByUserId WHERE \"Id\" = @id";


                    NpgsqlCommand command = new NpgsqlCommand(companyQuery, connection);
                    command.Parameters.AddWithValue("@updatedByUserId", internship.UpdatedByUserId);
                    command.Parameters.AddWithValue("@id", internship.Id);
                    int rowsUpdated = await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                    return rowsUpdated > 0;
                }
            }
        }

        public async Task<PagedList<Internship>> GetAsync(Sorting sorting, Paging paging, InternshipFilter filter)
        {
            if (sorting == null) sorting = new Sorting();
            if (paging == null) paging = new Paging();
            if (filter == null) filter = new InternshipFilter();

            List<Internship> internships = new List<Internship>();
            PagedList<Internship> pagedList = new PagedList<Internship>();

            string selectQuery = "SELECT i.\"Id\" AS \"InternshipId\"," +
                    " i.\"StudyAreaId\"," +
                    " sa.\"Name\" AS \"StudyAreaName\"," +
                    " i.\"CompanyId\"," +
                    " cv.\"Name\" AS \"CompanyViewName\"," +
                    " cv.\"Website\" AS \"CompanyWebsite\"," +
                    " u.\"Address\" AS \"CompanyAddress\"," +
                    " cv.\"Id\" AS \"CompanyId\"," +
                    " i.\"Name\"," +
                    " i.\"Description\"," +
                    " i.\"Address\"," +
                    " i.\"StartDate\"," +
                    " i.\"EndDate\"," +
                    " COUNT(ia.\"Id\") AS \"ApplicationsCount\" " +
                    " FROM public.\"Internship\" i" +
                    " INNER JOIN public.\"StudyArea\" sa ON i.\"StudyAreaId\" = sa.\"Id\"" +
                    " INNER JOIN public.\"Company\" cv ON i.\"CompanyId\" = cv.\"Id\"" +
                    " INNER JOIN dbo.\"AspNetUsers\" u ON cv.\"Id\" = u.\"Id\"" +
                    " INNER JOIN public.\"County\" c ON c.\"Id\" = u.\"CountyId\"" +
                    " LEFT JOIN public.\"InternshipApplication\" ia ON i.\"Id\" = ia.\"InternshipId\" AND ia.\"IsActive\" = true";


            string countQuery = "SELECT COUNT(DISTINCT i.\"Id\")" +
                    " FROM public.\"Internship\" i" +
                    " INNER JOIN public.\"StudyArea\" sa ON i.\"StudyAreaId\" = sa.\"Id\"" +
                    " INNER JOIN public.\"Company\" cv ON i.\"CompanyId\" = cv.\"Id\"" +
                    " INNER JOIN dbo.\"AspNetUsers\" u ON cv.\"Id\" = u.\"Id\"" +
                    " INNER JOIN public.\"County\" c ON c.\"Id\" = u.\"CountyId\" " +
                    " LEFT JOIN public.\"InternshipApplication\" ia ON i.\"Id\" = ia.\"InternshipId\" AND ia.\"IsActive\" = true";


            string groupByQuery = " GROUP BY i.\"Id\"," +
                                    " i.\"StudyAreaId\"," +
                                    " sa.\"Name\"," +
                                    " i.\"CompanyId\"," +
                                    " cv.\"Name\"," +
                                    " cv.\"Name\"," +
                                    " cv.\"Website\"," +
                                    " u.\"Address\"," +
                                    " cv.\"Id\"," +
                                    " i.\"Name\"," +
                                    " i.\"Description\"," +
                                    " i.\"Address\"," +
                                    " i.\"StartDate\"," +
                                    " i.\"EndDate\" ";

            StringBuilder selectQueryBuilder = new StringBuilder();
            StringBuilder countQueryBuilder = new StringBuilder();
            StringBuilder filterQueryBuilder = new StringBuilder();

            string sortBy;
            switch (sorting.SortBy.ToLower())
            {
                case "counties": sortBy = "c.\"" + sorting.SortBy + "\""; break;
                default: sortBy = "i.\"" + sorting.SortBy + "\""; break;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                connection.Open();

                string filterQuery = " WHERE 1=1 ";
                string sortingQuery = $" ORDER BY {sortBy} {sorting.SortOrder} ";
                string pagingQuery = $" LIMIT {paging.PageSize} OFFSET {(paging.CurrentPage - 1) * paging.PageSize}";

                NpgsqlCommand selectCommand = new NpgsqlCommand("", connection);
                NpgsqlCommand countCommand = new NpgsqlCommand("", connection);

                filterQueryBuilder.Append(filterQuery);
                filterQueryBuilder.Append(" AND i.\"IsActive\" = @isActive");
                selectCommand.Parameters.AddWithValue("@isActive", filter.IsActive);
                countCommand.Parameters.AddWithValue("@isActive", filter.IsActive);
                if(filter.CompanyId != null)
                {
                    filterQueryBuilder.Append(" AND i.\"CompanyId\" = @companyId ");
                    selectCommand.Parameters.AddWithValue("@companyId", filter.CompanyId);
                    countCommand.Parameters.AddWithValue("@companyId", filter.CompanyId);
                }
                if (filter.Name != null)
                {
                    filterQueryBuilder.Append(" AND i.\"Name\" LIKE '%' || @name || '%' ");
                    selectCommand.Parameters.AddWithValue("@name", filter.Name);
                    countCommand.Parameters.AddWithValue("@name", filter.Name);
                }
                if (filter.StartDate != null)
                {
                    filterQueryBuilder.Append(" AND i.\"StartDate\" >= @startDate ");
                    selectCommand.Parameters.AddWithValue("@startDate", filter.StartDate);
                    countCommand.Parameters.AddWithValue("@startDate", filter.StartDate);
                }
                if (filter.EndDate != null)
                {
                    filterQueryBuilder.Append(" AND i.\"EndDate\" <= @endDate ");
                    selectCommand.Parameters.AddWithValue("@endDate", filter.EndDate);
                    countCommand.Parameters.AddWithValue("@endDate", filter.EndDate);
                }
                if (filter.Counties != null && filter.Counties.Count > 0)
                {
                    filterQueryBuilder.Append(" AND u.\"CountyId\" IN (");

                    for (int i = 0; i < filter.Counties.Count; i++)
                    {
                        string parameterName = $"@countyId{i}";
                        filterQueryBuilder.Append(parameterName);

                        if (i < filter.Counties.Count - 1)
                        {
                            filterQueryBuilder.Append(", ");
                        }

                        selectCommand.Parameters.AddWithValue(parameterName, filter.Counties[i]);
                        countCommand.Parameters.AddWithValue(parameterName, filter.Counties[i]);

                    }

                    filterQueryBuilder.Append(")");
                }


                selectQueryBuilder.Append(selectQuery);
                selectQueryBuilder.Append(filterQueryBuilder.ToString());
                selectQueryBuilder.Append(groupByQuery);
                selectQueryBuilder.Append(sortingQuery);
                selectQueryBuilder.Append(pagingQuery);



                selectCommand.CommandText = selectQueryBuilder.ToString();

                NpgsqlDataReader selectReader = await selectCommand.ExecuteReaderAsync();
                while (selectReader.HasRows && selectReader.Read())
                {
                    Internship internship = ReadInternship(selectReader);
                    if (internship != null)
                    {
                        internship.ApplicationsCount = (long)selectReader["ApplicationsCount"];
                        internships.Add(internship);
                    };
                }
                selectReader.Close();


                countQueryBuilder.Append(countQuery);
                countQueryBuilder.Append(filterQueryBuilder.ToString());

                countCommand.CommandText = countQueryBuilder.ToString();
                var scalarCount = await countCommand.ExecuteScalarAsync();

                int totalDatabaseRecords = Int32.Parse(scalarCount.ToString());

                pagedList.Data = internships;
                pagedList.CurrentPage = paging.CurrentPage;
                pagedList.PageSize = paging.PageSize;
                pagedList.DatabaseRecordsCount = totalDatabaseRecords;
                pagedList.LastPage = Convert.ToInt32(totalDatabaseRecords / paging.PageSize) + (totalDatabaseRecords % paging.PageSize != 0 ? 1 : 0);
            }
            return pagedList;
        }

        public async Task<Internship> GetAsync(Guid id)
        {
            return await GetInternshipAsync(id);
        }

        public async Task<Internship> GetInternshipAsync(Guid id)
        {
            Internship internship = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
                {
                    connection.Open();
                    string query = "SELECT i.\"Id\" AS \"InternshipId\"," +
                                " i.\"StudyAreaId\", " +
                                " sa.\"Name\" AS \"StudyAreaName\", " +
                                " i.\"CompanyId\"," +
                                " cv.\"Name\" AS \"CompanyViewName\"," +
                                " cv.\"Website\" AS \"CompanyWebsite\"," +
                                " u.\"Address\" AS \"CompanyAddress\"," +
                                " cv.\"Id\" AS \"CompanyId\"," +
                                " i.\"Name\"," +
                                " i.\"Description\", " +
                                " i.\"Address\"," +
                                " i.\"StartDate\"," +
                                " i.\"EndDate\"" +
                                " FROM public.\"Internship\" i" +
                                " INNER JOIN public.\"StudyArea\" sa ON i.\"StudyAreaId\" = sa.\"Id\"" +
                                " INNER JOIN public.\"Company\" cv ON i.\"CompanyId\" = cv.\"Id\"" +
                                " INNER JOIN dbo.\"AspNetUsers\" u ON cv.\"Id\" = u.\"Id\"" +
                                " INNER JOIN public.\"County\" c ON c.\"Id\" = u.\"CountyId\"" +
                                " LEFT JOIN public.\"InternshipApplication\" ia ON i.\"Id\" = ia.\"InternshipId\" AND ia.\"IsActive\" = true" +
                                " where i.\"Id\" = @id";
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);

                    command.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    reader.Read();
                    internship = ReadInternship(reader);
                    reader.Close();
                }

                return internship;
            }
            catch (Exception) { return null; }
        }

        public async Task<bool> PostAsync(Internship internship)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                connection.Open();

                string insertQuery = "INSERT INTO public.\"Internship\" " +
                     "(\"Id\", \"StudyAreaId\", \"CompanyId\", \"Name\", \"Description\", \"Address\", \"StartDate\", \"EndDate\", \"CreatedByUserId\", \"UpdatedByUserId\", \"DateCreated\", \"DateUpdated\", \"IsActive\") " +
                     "VALUES (@Id, @StudyAreaId, @CompanyId, @Name, @Description, @Address, @StartDate, @EndDate, @CreatedByUserId, @UpdatedByUserId, @DateCreated, @DateUpdated, @IsActive)";

                NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Id", internship.Id);
                command.Parameters.AddWithValue("@StudyAreaId", internship.StudyAreaId);
                command.Parameters.AddWithValue("@CompanyId", internship.CompanyId);
                command.Parameters.AddWithValue("@Name", internship.Name);
                command.Parameters.AddWithValue("@Description", internship.Description);
                command.Parameters.AddWithValue("@Address", internship.Address);
                command.Parameters.AddWithValue("@StartDate", internship.StartDate);
                command.Parameters.AddWithValue("@EndDate", internship.EndDate);
                command.Parameters.AddWithValue("@CreatedByUserId", internship.CreatedByUserId);
                command.Parameters.AddWithValue("@UpdatedByUserId", internship.UpdatedByUserId);
                command.Parameters.AddWithValue("@DateCreated", internship.DateCreated);
                command.Parameters.AddWithValue("@DateUpdated", internship.DateUpdated);
                command.Parameters.AddWithValue("@IsActive", true);

                int rowsUpdated = await command.ExecuteNonQueryAsync();
                return rowsUpdated > 0;
            }

        }

        public async Task<bool> PutAsync(Internship internship)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                connection.Open();

                string updateQuery = "UPDATE \"Internship\" SET \"StudyAreaId\" = @studyAreaId, \"CompanyId\" = @companyId, \"Name\" = @name, \"Description\" = @description, \"Address\" = @address, \"StartDate\" = @startDate, \"EndDate\" = @endDate WHERE \"Id\" = @id";

                NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@studyAreaId", internship.StudyAreaId);
                command.Parameters.AddWithValue("@companyId", internship.CompanyId);
                command.Parameters.AddWithValue("@name", internship.Name);
                command.Parameters.AddWithValue("@description", internship.Description);
                command.Parameters.AddWithValue("@address", internship.Address);
                command.Parameters.AddWithValue("@startDate", internship.StartDate);
                command.Parameters.AddWithValue("@endDate", internship.EndDate);
                command.Parameters.AddWithValue("@id", internship.Id);

                int rowsUpdated = await command.ExecuteNonQueryAsync();
                return rowsUpdated > 0;
            }
        }


        public async Task<bool> IsStudentRegisteredToInternshipAsync(string studentId, Guid internshipId)
        {
            using (NpgsqlConnection connection= new NpgsqlConnection(connectionString.Name))
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand("select COUNT(*) " +
                    "from public.\"InternshipApplication\" ia " +
                    "inner join public.\"Student\" s on ia.\"StudentId\" = s.\"Id\" " +
                    "inner join public.\"Internship\" i on ia.\"InternshipId\" = i.\"Id\" and i.\"IsActive\" = true " +
                    "inner join dbo.\"AspNetUsers\" anu on anu.\"Id\" = s.\"Id\" and anu.\"IsActive\" = true " +
                    "where ia.\"StudentId\" = @studentId and ia.\"InternshipId\" = @internshipId and ia.\"IsActive\" = true", connection);

                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("internshipId", internshipId);


                int count = Convert.ToInt32(await command.ExecuteScalarAsync());



                return count > 0;
            }
        }


        private Internship ReadInternship(NpgsqlDataReader reader)
        {
            try
            {
                return new Internship
                {
                    Id = (Guid)reader["InternshipId"],
                    StudyAreaId = (Guid)reader["StudyAreaId"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    Address = (string)reader["Address"],
                    StartDate = (DateTime)reader["StartDate"],
                    CompanyId = (string)reader["CompanyId"],
                    EndDate = (DateTime)reader["EndDate"],
                    StudyArea = new StudyArea
                    {
                        Name = (string)reader["StudyAreaName"],
                        Id = (Guid)reader["StudyAreaId"]
                    },

                    Company = new Company
                    {
                        Name = (string)reader["CompanyViewName"],
                        Id = (string)reader["CompanyId"],
                        Website = (string)reader["CompanyWebsite"],
                        Address = (string)reader["CompanyAddress"]
                    },
                };
            }
            catch { return null; }
        }
    }
}

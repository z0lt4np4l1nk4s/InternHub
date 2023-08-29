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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IConnectionString connectionString;
        public CompanyRepository(IConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> DeleteAsync(Company company)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                await connection.OpenAsync();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string userQuery = $"UPDATE dbo.\"AspNetUsers\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated WHERE \"Id\" = @id";
                        NpgsqlCommand userCommand = new NpgsqlCommand(userQuery, connection, transaction);
                        userCommand.Parameters.AddWithValue("@id", company.Id);
                        userCommand.Parameters.AddWithValue("@dateUpdated", company.DateUpdated);

                        string internshipQuery = $"UPDATE public.\"Internship\" SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated WHERE \"CompanyId\" = @companyId";
                        NpgsqlCommand internshipCommand = new NpgsqlCommand(internshipQuery, connection, transaction);
                        internshipCommand.Parameters.AddWithValue("@companyId", company.Id);
                        internshipCommand.Parameters.AddWithValue("@dateUpdated", company.DateUpdated);

                        string internshipApplicationQuery = $"UPDATE public.\"InternshipApplication\" ia SET \"IsActive\" = false, \"DateUpdated\" = @dateUpdated WHERE ia.\"InternshipId\" IN (SELECT i.\"Id\" FROM public.\"Internship\" i WHERE i.\"CompanyId\" = @companyId);";
                        NpgsqlCommand internshipApplicationCommand = new NpgsqlCommand(internshipApplicationQuery, connection, transaction);
                        internshipApplicationCommand.Parameters.AddWithValue("@companyId", company.Id);
                        internshipApplicationCommand.Parameters.AddWithValue("@dateUpdated", company.DateUpdated);

                        int userResult = await userCommand.ExecuteNonQueryAsync();
                        int internshipResult = await internshipCommand.ExecuteNonQueryAsync();
                        int internshipApplicationResult = await internshipApplicationCommand.ExecuteNonQueryAsync();

                        int result = userResult * internshipResult * internshipApplicationResult;

                        if (result > 0)
                            await transaction.CommitAsync();
                        else
                            await transaction.RollbackAsync();

                        return result > 0;
                    }
                    catch 
                    { 
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }

        public async Task<PagedList<Company>> GetAsync(Sorting sorting, Paging paging, CompanyFilter filter)
        {
            List<Company> companies = new List<Company>();
            int recordsCount = 0;

            if (sorting == null) sorting = new Sorting();
            if (paging == null) paging = new Paging();
            if (filter == null) filter = new CompanyFilter();

            StringBuilder selectQueryBuilder = new StringBuilder();
            StringBuilder countQueryBuilder = new StringBuilder();
            StringBuilder filterQueryBuilder = new StringBuilder();

            string sortBy;

            switch (sorting.SortBy.ToLower())
            {
                case "name": sortBy = "c.\"" + sorting.SortBy + "\""; break;
                case "website": sortBy = "c.\"" + sorting.SortBy + "\""; break;
                case "isaccepted": sortBy = "c.\"" + sorting.SortBy + "\""; break;
                default: sortBy = "u.\"" + sorting.SortBy + "\""; break;
            }


            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                await connection.OpenAsync();

                string selectQuery = "SELECT u.*, c.*, co.\"Name\" as \"CountyName\" FROM public.\"Company\" c INNER JOIN dbo.\"AspNetUsers\" u ON c.\"Id\" = u.\"Id\" inner join \"County\" co on u.\"CountyId\" = co.\"Id\" ";
                string countQuery = "SELECT COUNT(*) FROM public.\"Company\" c INNER JOIN dbo.\"AspNetUsers\" u ON c.\"Id\" = u.\"Id\" ";

                string filterQuery = " WHERE 1=1 ";
                string sortingQuery = $" ORDER BY {sortBy} {sorting.SortOrder} ";
                string pagingQuery = $" LIMIT {paging.PageSize} OFFSET {(paging.CurrentPage - 1) * paging.PageSize}";

                NpgsqlCommand selectCommand = new NpgsqlCommand();

                filterQueryBuilder.Append(filterQuery);
                filterQueryBuilder.Append(" AND u.\"IsActive\" = @isActive");
                selectCommand.Parameters.AddWithValue("@isActive", filter.IsActive);
                if (filter.IsAccepted != null)
                {
                    filterQueryBuilder.Append(" AND c.\"IsAccepted\" = @isAccepted");
                    selectCommand.Parameters.AddWithValue("@isAccepted", filter.IsAccepted.Value);
                }
                if (filter.Name != null)
                {
                    filterQueryBuilder.Append(" AND c.\"Name\" ilike @name");
                    selectCommand.Parameters.AddWithValue("@name", "%" + filter.Name + "%");
                }

                selectQueryBuilder.Append(selectQuery);
                selectQueryBuilder.Append(filterQueryBuilder.ToString());
                selectQueryBuilder.Append(sortingQuery);
                selectQueryBuilder.Append(pagingQuery);

                selectCommand.CommandText = selectQueryBuilder.ToString();
                selectCommand.Connection = connection;
                NpgsqlDataReader selectReader = await selectCommand.ExecuteReaderAsync();
                while (selectReader.HasRows && selectReader.Read())
                {
                    Company company = ReadCompany(selectReader);
                    if (company != null) companies.Add(company);
                }
                selectReader.Close();

                countQueryBuilder.Append(countQuery);
                countQueryBuilder.Append(filterQueryBuilder.ToString());

                NpgsqlCommand countCommand = new NpgsqlCommand(countQueryBuilder.ToString(), connection);
                foreach (NpgsqlParameter item in selectCommand.Parameters) countCommand.Parameters.AddWithValue(item.ParameterName, item.Value);
                var countScalar = await countCommand.ExecuteScalarAsync();
                recordsCount = Convert.ToInt32(countScalar);

            }
            PagedList<Company> pagedList = new PagedList<Company>
            {
                Data = companies,
                CurrentPage = paging.CurrentPage,
                DatabaseRecordsCount = recordsCount,
                LastPage = Convert.ToInt32(recordsCount / paging.PageSize) + (recordsCount % paging.PageSize != 0 ? 1 : 0),
                PageSize = paging.PageSize
            };

            return pagedList;
        }

        public async Task<bool> PostAsync(Company company)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertUserQuery = "INSERT INTO dbo.\"AspNetUsers\" (\"Id\", \"FirstName\", \"LastName\", \"Address\", \"Description\", \"CountyId\", \"IsActive\", \"DateCreated\", \"DateUpdated\", \"Email\", \"EmailConfirmed\", \"PasswordHash\", \"SecurityStamp\", \"PhoneNumber\", \"PhoneNumberConfirmed\", \"TwoFactorEnabled\", \"LockoutEndDateUtc\", \"LockoutEnabled\", \"AccessFailedCount\", \"UserName\", \"Discriminator\") VALUES (@id, @firstname, @lastname, @address, @description, @countyId, true, @dateCreated, @dateUpdated, @email, false, @password, 'security_stamp', @phoneNumber, false, false, null, false, 0, @username, 'User');";
                        string insertCompanyQuery = "INSERT INTO public.\"Company\" (\"Id\", \"Name\", \"Website\", \"IsAccepted\") VALUES(@id, @name, @website, false);";

                        NpgsqlCommand userCommand = new NpgsqlCommand(insertUserQuery, connection);
                        NpgsqlCommand companyCommand = new NpgsqlCommand(insertCompanyQuery, connection);

                        NpgsqlCommand companyInsertCommand = new NpgsqlCommand(insertCompanyQuery, connection, transaction);
                        NpgsqlCommand userInsertCommand = new NpgsqlCommand(insertUserQuery, connection, transaction);

                        userInsertCommand.Parameters.AddWithValue("@id", company.Id);
                        userInsertCommand.Parameters.AddWithValue("@firstname", company.FirstName);
                        userInsertCommand.Parameters.AddWithValue("@lastname", company.LastName);
                        userInsertCommand.Parameters.AddWithValue("@address", company.Address);
                        userInsertCommand.Parameters.AddWithValue("@description", company.Description);
                        userInsertCommand.Parameters.AddWithValue("@countyId", company.CountyId);
                        userInsertCommand.Parameters.AddWithValue("@dateCreated", company.DateCreated);
                        userInsertCommand.Parameters.AddWithValue("@dateUpdated", company.DateUpdated);
                        userInsertCommand.Parameters.AddWithValue("@phoneNumber", company.PhoneNumber);
                        userInsertCommand.Parameters.AddWithValue("@email", company.Email);
                        userInsertCommand.Parameters.AddWithValue("@username", company.Email);
                        userInsertCommand.Parameters.AddWithValue("@password", company.Password);


                        companyInsertCommand.Parameters.AddWithValue("@id", company.Id);
                        companyInsertCommand.Parameters.AddWithValue("@name", company.Name);
                        companyInsertCommand.Parameters.AddWithValue("@website", company.Website);

                        string userRoleQuery = "INSERT INTO dbo.\"AspNetUserRoles\" VALUES (@userId, @roleId)";
                        NpgsqlCommand userRoleCommand = new NpgsqlCommand(userRoleQuery, connection);
                        userRoleCommand.Parameters.AddWithValue("@userId", company.Id);
                        userRoleCommand.Parameters.AddWithValue("@roleId", company.RoleId);

                        int userRowsAffectedCount = await userInsertCommand.ExecuteNonQueryAsync();
                        int companyRowsAffectedCount = await companyInsertCommand.ExecuteNonQueryAsync();
                        int userRoleRowsAffectedCount = await userRoleCommand.ExecuteNonQueryAsync();

                        int result = userRowsAffectedCount * companyRowsAffectedCount * userRoleRowsAffectedCount;

                        if (result > 0)
                            await transaction.CommitAsync();
                        else
                            await transaction.RollbackAsync();

                        return result > 0;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }

        public async Task<bool> PutAsync(Company company)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateCompanySql = "UPDATE \"Company\" SET \"Website\" = @website, \"Name\" = @name WHERE \"Id\" = @id";

                        string updateUserSql = "UPDATE dbo.\"AspNetUsers\" SET \"FirstName\" = @firstname, \"LastName\" = @lastname, \"Address\" = @address, \"Description\" = @description, \"Email\" = @email, \"PhoneNumber\" = @phoneNumber, \"CountyId\" = @countyId WHERE \"Id\" = @id";

                        using (NpgsqlCommand updateCompanyCommand = new NpgsqlCommand(updateCompanySql, connection, transaction))
                        using (NpgsqlCommand updateUserCommand = new NpgsqlCommand(updateUserSql, connection, transaction))
                        {
                            updateCompanyCommand.Parameters.AddWithValue("@website", company.Website);
                            updateCompanyCommand.Parameters.AddWithValue("@name", company.Name);
                            updateCompanyCommand.Parameters.AddWithValue("@id", company.Id);

                            updateUserCommand.Parameters.AddWithValue("@firstName", company.FirstName);
                            updateUserCommand.Parameters.AddWithValue("@lastName", company.LastName);
                            updateUserCommand.Parameters.AddWithValue("@id", company.Id);
                            updateUserCommand.Parameters.AddWithValue("@description", company.Description);
                            updateUserCommand.Parameters.AddWithValue("@phoneNumber", company.PhoneNumber);
                            updateUserCommand.Parameters.AddWithValue("@address", company.Address);
                            updateUserCommand.Parameters.AddWithValue("@email", company.Email);
                            updateUserCommand.Parameters.AddWithValue("@countyId", company.CountyId);

                            int companyRowsAffectedCount = await updateCompanyCommand.ExecuteNonQueryAsync();
                            int userRowsAffectedCount = await updateUserCommand.ExecuteNonQueryAsync();

                            if (companyRowsAffectedCount * userRowsAffectedCount > 0)
                                await transaction.CommitAsync();
                            else
                                await transaction.RollbackAsync();

                            return companyRowsAffectedCount * userRowsAffectedCount > 0;
                        }
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }

        public async Task<Company> GetAsync(string id)
        {
            Company company = new Company();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
                {
                    await connection.OpenAsync();

                    NpgsqlCommand command = new NpgsqlCommand("SELECT u.*, c.*, co.\"Name\" as \"CountyName\" FROM public.\"Company\" c INNER JOIN dbo.\"AspNetUsers\" u ON c.\"Id\" = u.\"Id\" INNER JOIN public.\"County\" co ON co.\"Id\" = u.\"CountyId\" WHERE c.\"Id\" = @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows || !await reader.ReadAsync()) return null;

                    company = ReadCompany(reader);

                    reader.Close();
                }
                return company;
            }
            catch { return null; }
        }

        public async Task<bool> AcceptAsync(Company company)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString.Name))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        NpgsqlCommand companyCommand = new NpgsqlCommand("UPDATE public.\"Company\" SET \"IsAccepted\" = true WHERE \"Id\" = @id", connection, transaction);
                        companyCommand.Parameters.AddWithValue("@id", company.Id);

                        NpgsqlCommand userCommand = new NpgsqlCommand("UPDATE dbo.\"AspNetUsers\" SET \"DateUpdated\" = @dateUpdated WHERE \"Id\" = @id", connection, transaction);
                        userCommand.Parameters.AddWithValue("@id", company.Id);
                        userCommand.Parameters.AddWithValue("@dateUpdated", company.DateUpdated);

                        NpgsqlCommand userRoleRemoveCommand = new NpgsqlCommand("DELETE from dbo.\"AspNetUserRoles\" r where r.\"UserId\" = @userId", connection, transaction);
                        userRoleRemoveCommand.Parameters.AddWithValue("@userId", company.Id);

                        NpgsqlCommand userRoleAddCommand = new NpgsqlCommand("INSERT INTO dbo.\"AspNetUserRoles\" values (@userId, @roleId)", connection, transaction);
                        userRoleAddCommand.Parameters.AddWithValue("@userId", company.Id);
                        userRoleAddCommand.Parameters.AddWithValue("@roleId", company.RoleId);

                        int companyResult = await companyCommand.ExecuteNonQueryAsync();
                        int userResult = await userCommand.ExecuteNonQueryAsync();
                        int userRoleRemoveResult = await userRoleRemoveCommand.ExecuteNonQueryAsync();
                        int userRoleAddResult = await userRoleAddCommand.ExecuteNonQueryAsync();

                        int result = companyResult * userResult * userRoleRemoveResult * userRoleAddResult;

                        if (result > 0) await transaction.CommitAsync();
                        else await transaction.RollbackAsync();

                        return result > 0;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                    }
                }
                return false;
            }
        }

        private Company ReadCompany(NpgsqlDataReader reader)
        {
            try
            {
                return new Company()
                {
                    Address = Convert.ToString(reader["Address"]),
                    CountyId = (Guid)reader["CountyId"],
                    County = new County()
                    {
                        Id = (Guid)reader["CountyId"],
                        Name = Convert.ToString(reader["CountyName"]),
                    },
                    DateCreated = Convert.ToDateTime(reader["DateCreated"]),
                    DateUpdated = Convert.ToDateTime(reader["DateUpdated"]),
                    Description = Convert.ToString(reader["Description"]),
                    Email = Convert.ToString(reader["Email"]),
                    FirstName = Convert.ToString(reader["FirstName"]),
                    LastName = Convert.ToString(reader["LastName"]),
                    Id = Convert.ToString(reader["Id"]),
                    IsAccepted = Convert.ToBoolean(reader["IsAccepted"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    Name = Convert.ToString(reader["Name"]),
                    PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                    Website = Convert.ToString(reader["Website"])
                };
            }
            catch { return null; }
        }
    }
}

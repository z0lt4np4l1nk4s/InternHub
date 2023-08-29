using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        public IConnectionString ConnectionString { get; set; }

        public NotificationRepository(IConnectionString connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<bool> AddAsync(Notification notification)
        {
            return await AddRangeAsync(new List<Notification> { notification });
        }

        public async Task<bool> AddRangeAsync(List<Notification> notifications)
        {
            int numberOfAffectedRows = 1;
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                await connection.OpenAsync();

                NpgsqlTransaction transaction = connection.BeginTransaction();

                foreach(Notification notification in notifications)
                {
                    string query = "INSERT INTO public.\"Notification\" VALUES (@id, @title, @body, @receiverUserId, @dateCreated, @dateUpdated, @createdByUserId, @updatedByUserId, @isActive)";
                    NpgsqlCommand command = new NpgsqlCommand(query, connection, transaction);
                    command.Parameters.AddWithValue("@id", notification.Id);
                    command.Parameters.AddWithValue("@title", notification.Title);
                    command.Parameters.AddWithValue("@body", notification.Body);
                    command.Parameters.AddWithValue("@receiverUserId", notification.ReceiverUserId);
                    command.Parameters.AddWithValue("@dateCreated", notification.DateCreated);
                    command.Parameters.AddWithValue("@dateUpdated", notification.DateUpdated);
                    command.Parameters.AddWithValue("@createdByUserId", notification.CreatedByUserId);
                    command.Parameters.AddWithValue("@updatedByUserId", notification.UpdatedByUserId);
                    command.Parameters.AddWithValue("@isActive", notification.IsActive);
                    numberOfAffectedRows *= await command.ExecuteNonQueryAsync();

                    if (numberOfAffectedRows <= 0) return false;
                }

                await transaction.CommitAsync();
            }
            return numberOfAffectedRows != 0;
        }

        public async Task<PagedList<Notification>> GetAllAsync(Sorting sorting, Paging paging, NotificationFilter filter)
        {
            if (sorting == null) sorting = new Sorting();
            if (paging == null) paging = new Paging();

            PagedList<Notification> notifications = new PagedList<Notification>()
            {
                CurrentPage = paging.CurrentPage,
                PageSize = paging.PageSize,
            };

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString.Name))
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.Parameters.AddWithValue("@pageSize", paging.PageSize);
                command.Parameters.AddWithValue("@skip", (paging.CurrentPage - 1) * paging.PageSize);

                string sortBy = "n.\"" + sorting.SortBy + "\"";

                StringBuilder builder = new StringBuilder();

                string selectQuery = "SELECT * FROM public.\"Notification\" n INNER JOIN dbo.\"AspNetUsers\" u ON n.\"ReceiverUserId\" = u.\"Id\"" +
                    " WHERE n.\"IsActive\" = true " + builder.ToString() +
                    $" ORDER BY {sortBy} {(sorting.SortOrder.ToLower() == "asc" ? "ASC" : "DESC")} LIMIT @pageSize OFFSET @skip;";

                string countQuery = "SELECT COUNT(*) FROM \"Notification\" WHERE \"IsActive\" = true " + builder.ToString();
                NpgsqlCommand countCommand = new NpgsqlCommand(countQuery, connection);

                foreach (NpgsqlParameter npgsqlParameter in command.Parameters) countCommand.Parameters.AddWithValue(npgsqlParameter.ParameterName, npgsqlParameter.Value);

                command.CommandText = selectQuery;

                await connection.OpenAsync();

                object countResult = await countCommand.ExecuteScalarAsync();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (countResult == null || !reader.HasRows) return null;

                while (reader.HasRows && await reader.ReadAsync())
                {
                    Notification notification = ReadNotification(reader);
                    
                    if(notification != null) notifications.Data.Add(notification);
                }

                notifications.DatabaseRecordsCount = Convert.ToInt32(countResult);
                notifications.LastPage = Convert.ToInt32(notifications.DatabaseRecordsCount / paging.PageSize) + (notifications.DatabaseRecordsCount % paging.PageSize != 0 ? 1 : 0);
            }

            return notifications;
        }

        private Notification ReadNotification(NpgsqlDataReader reader)
        {
            try
            {
                return new Notification()
                {
                    Id = (Guid)reader["Id"],
                    Title = Convert.ToString(reader["Title"]),
                    Body = Convert.ToString(reader["Body"]),
                    CreatedByUserId = Convert.ToString(reader["CreatedByUserId"]),
                    UpdatedByUserId = Convert.ToString(reader["UpdatedByUserId"]),
                    ReceiverUserId = Convert.ToString(reader["ReceiverUserId"]),
                    DateCreated = Convert.ToDateTime(reader["DateCreated"]),
                    DateUpdated = Convert.ToDateTime(reader["DateUpdated"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    ReceiverUser = new User
                    {
                        FirstName = Convert.ToString(reader["FirstName"]),
                        LastName = Convert.ToString(reader["LastName"]),
                        Email = Convert.ToString(reader["Email"]),
                        Address = Convert.ToString(reader["Address"]),
                        Description = Convert.ToString(reader["Description"]),
                        Id = Convert.ToString(reader["ReceiverUserId"]),
                        PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                    }
                };
            }
            catch { return null; }
        }
    }
}

using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface INotificationService
    {
        Task<bool> AddAsync(string subject, string body, User user);
        Task<bool> AddRangeAsync(string subject, string body, List<User> users);
        Task<PagedList<Notification>> GetAllAsync(Sorting sorting, Paging paging, NotificationFilter filter);
    }
}

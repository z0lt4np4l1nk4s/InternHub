using InternHub.Common.Filter;
using InternHub.Common;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository.Common
{
    public interface INotificationRepository
    {
        Task<bool> AddAsync(Notification notification);
        Task<bool> AddRangeAsync(List<Notification> notifications);
        Task<PagedList<Notification>> GetAllAsync(Sorting sorting, Paging paging, NotificationFilter filter);
    }
}

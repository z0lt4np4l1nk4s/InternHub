using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface INotification : IBaseModel
    {
        string ReceiverUserId { get; set; }
        string Title { get; set; }
        string Body { get; set; }
    }
}

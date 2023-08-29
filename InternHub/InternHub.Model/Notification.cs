using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class Notification : BaseModel, INotification
    {
        public string ReceiverUserId { get; set; }
        public User ReceiverUser { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}

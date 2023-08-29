using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class NotificationView
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public NotificationView() { }

        public NotificationView(Notification notification) 
        {
            Title = notification.Title;
            Body = notification.Body;
        }
    }
}
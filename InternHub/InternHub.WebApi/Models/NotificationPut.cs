using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class NotificationPut
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public List<string> UserIds { get; set; }
    }
}
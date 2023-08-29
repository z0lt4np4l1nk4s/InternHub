using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Generic;
using InternHub.Service.Common;
using InternHub.Model;
using InternHub.Repository.Common;
using System.Threading.Tasks;
using System;
using InternHub.Common;
using InternHub.Common.Filter;

namespace InternHub.Service
{
    public class NotificationService : INotificationService
    {
        public INotificationRepository NotificationRepository { get; set; }

        public NotificationService(INotificationRepository notificationRepository)
        {
            NotificationRepository = notificationRepository;
        }

        public async Task<bool> AddAsync(string subject, string body, User user)
        {
            return await AddRangeAsync(subject, body, new List<User> { user });
        }

        public async Task<bool> AddRangeAsync(string subject, string body, List<User> users)
        {
            if (users.Count == 0) return false;
            try
            {
                List<Notification> notifications = new List<Notification>();

                var email = new MimeMessage();

                email.From.Add(new MailboxAddress("InternHub Info", "zoltan@timeframe.link"));
                foreach (User user in users)
                {
                    email.To.Add(new MailboxAddress(user.GetFullName(), user.Email));
                    notifications.Add(new Notification()
                    {
                        CreatedByUserId = "0c2ba6ff-9145-43cf-ac48-ccf7effea536",
                        UpdatedByUserId = "0c2ba6ff-9145-43cf-ac48-ccf7effea536",
                        Id = Guid.NewGuid(),
                        Title = subject,
                        Body = body,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        IsActive = true,
                        ReceiverUserId = user.Id
                    });
                }

                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = body
                };
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("mail.timeframe.link", 465, true);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate("email", "password");

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                return await NotificationRepository.AddRangeAsync(notifications);
            }
            catch { return false; }
        }

        public async Task<PagedList<Notification>> GetAllAsync(Sorting sorting, Paging paging, NotificationFilter filter)
        {
            return await NotificationRepository.GetAllAsync(sorting, paging, filter);
        }
    }
}

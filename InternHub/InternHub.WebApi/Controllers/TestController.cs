using InternHub.Model.Common;
using InternHub.Service;
using InternHub.Service.Common;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InternHub.WebApi.Controllers
{
    public class TestController : ApiController
    {
        public IStateService State { get; }
        public INotificationService NotificationService { get; }
        public RoleManager RoleManager { get; }

        public TestController(IStateService state, INotificationService notificationService, RoleManager roleManager)
        {
            State = state;
            NotificationService = notificationService;
            RoleManager = roleManager;
        }

        public HttpResponseMessage Post()
        {
            //NotificationService.Send();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Authorize]
        public HttpResponseMessage Get()
        {
            var d = RoleManager.FindByName("Admin");
            return Request.CreateResponse(HttpStatusCode.OK, "test");
        }
    }
}

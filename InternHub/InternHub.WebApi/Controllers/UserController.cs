using InternHub.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace InternHub.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        public UserManager UserManager { get; }

        public UserController(UserManager userManager)
        {
            UserManager = userManager;
        }

        [HttpGet, Route("GetRole")]
        public async Task<HttpResponseMessage> GetRole()
        {
            IList<string> roles = await UserManager.GetRolesAsync(User.Identity.GetUserId());
            if (roles.Count > 0) return Request.CreateResponse(HttpStatusCode.OK, roles[0]);
            return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        }
    }
}

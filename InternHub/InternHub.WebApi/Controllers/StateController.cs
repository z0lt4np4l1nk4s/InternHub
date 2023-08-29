using InternHub.Model;
using InternHub.Service.Common;
using InternHub.WebApi.Models;
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
    public class StateController : ApiController
    {
        public IStateService StateService { get; set; }

        public StateController(IStateService stateService)
        {
            StateService = stateService;
        }

        // GET: api/State
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetAsync()
        {
            try
            {
                List<State> states = await StateService.GetAllAsync();
                return Request.CreateResponse(HttpStatusCode.OK, states.Select(x => new StateView(x)).OrderBy(x => x.Name));
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // GET: api/State/5
        [Authorize]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                State state = await StateService.GetByIdAsync(id);
                if (state == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK, new StateView(state));
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // POST: api/State
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] StatePut state)
        {
            try
            {
                if (state == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                State newState = new State
                {
                    Id = Guid.NewGuid(),
                    Name = state.Name
                };
                bool result = await StateService.AddAsync(newState, User.Identity.GetUserId());
                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // PUT: api/State/5
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] StatePut state)
        {
            try
            {
                if (id == null || state == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                State oldState = await StateService.GetByIdAsync(id);

                if (state.Name != null) oldState.Name = state.Name;

                bool result = await StateService.UpdateAsync(oldState, User.Identity.GetUserId());
                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // DELETE: api/State/5
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                State state = await StateService.GetByIdAsync(id);
                if (state == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                bool result = await StateService.RemoveAsync(state, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }
    }
}

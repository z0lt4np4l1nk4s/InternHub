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
    public class CountyController : ApiController
    {
        public ICountyService CountyService { get; }

        public CountyController(ICountyService countyService)
        {
            CountyService = countyService;
        }

        // GET: api/County
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetAsync()
        {
            try
            {
                List<County> counties = await CountyService.GetAllAsync();
                return Request.CreateResponse(HttpStatusCode.OK, counties.Select(x => new CountyView(x)).OrderBy(x => x.Name));
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // GET: api/County/5
        [Authorize]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                County county = await CountyService.GetByIdAsync(id);

                if (county == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK, new CountyView(county));
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // POST: api/County
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] CountyPut county)
        {
            try
            {
                if(county == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                County newCounty = new County
                {
                    Id = Guid.NewGuid(),
                    Name = county.Name
                };

                bool result = await CountyService.AddAsync(newCounty, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // PUT: api/County/5
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] CountyPut county)
        {
            try
            {
                if(id == null || county == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                County oldCounty = await CountyService.GetByIdAsync(id);
                
                if (county.Name != null) oldCounty.Name = county.Name;

                bool result = await CountyService.UpdateAsync(oldCounty, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // DELETE: api/County/5
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                if(id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                County county = await CountyService.GetByIdAsync(id);

                if (county == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                bool result = await CountyService.RemoveAsync(county, User.Identity.GetUserId());

                if(!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }
    }
}


using InternHub.Model;
using InternHub.Model.Common;
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
    public class StudyAreaController : ApiController
    {
        public IStudyAreaService StudyAreaService { get; }

        public StudyAreaController(IStudyAreaService studyAreaService)
        {
            StudyAreaService = studyAreaService;
        }

        // GET: api/StudyArea
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetAllAsync()
        {
            try
            {
                List<StudyArea> studyAreas = await StudyAreaService.GetAllAsync();
                if (studyAreas == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                return Request.CreateResponse(HttpStatusCode.OK, studyAreas.Select(x => new StudyAreaView(x)).OrderBy(x => x.Name));
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.InternalServerError, ex); }
        }

        // GET: api/StudyArea/5
        [Authorize]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                StudyArea studyArea = await StudyAreaService.GetByIdAsync(id);

                if (studyArea == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK, new StudyAreaView(studyArea));
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.InternalServerError, ex); }
        }

        // POST: api/StudyArea
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] StudyAreaPut studyArea)
        {
            try
            {
                if (studyArea == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                StudyArea newStudyArea = new StudyArea
                {
                    Id = Guid.NewGuid(),
                    Name = studyArea.Name
                };

                bool result = await StudyAreaService.AddAsync(newStudyArea, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // PUT: api/StudyArea/5
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] StudyAreaPut studyArea)
        {
            try
            {
                if (id == null || studyArea == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                StudyArea oldStudyArea = await StudyAreaService.GetByIdAsync(id);

                if (studyArea.Name != null) oldStudyArea.Name = studyArea.Name;

                bool result = await StudyAreaService.UpdateAsync(oldStudyArea, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }

        // DELETE: api/StudyArea/5
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                StudyArea studyArea = await StudyAreaService.GetByIdAsync(id);

                if (studyArea == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                bool result = await StudyAreaService.RemoveAsync(studyArea, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError, "Code crash"); }
        }
    }
}

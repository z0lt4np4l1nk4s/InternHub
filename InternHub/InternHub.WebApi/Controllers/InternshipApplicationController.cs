using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Service;
using InternHub.Service.Common;
using InternHub.WebApi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace InternHub.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/InternshipApplication")]
    public class InternshipApplicationController : ApiController
    {
        private IInternshipApplicationService InternshipApplicationService { get; }
        private IStateService StateService { get; }
        private INotificationService NotificationService { get; }

        public InternshipApplicationController(IInternshipApplicationService internshipApplicationService, IStateService stateService, INotificationService notificationService)
        {
            InternshipApplicationService = internshipApplicationService;
            StateService = stateService;
            NotificationService = notificationService;
        }

        public async Task<HttpResponseMessage> GetAllAsync([FromUri] Sorting sorting = null, [FromUri] Paging paging = null, [FromUri] InternshipApplicationFilter filter = null)
        {
            try
            {
                PagedList<InternshipApplication> internshipApplications = await InternshipApplicationService.GetAllInternshipApplicationsAsync(paging, sorting, filter);
                if (internshipApplications == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                PagedList<InternshipApplicationView> pagedApplications = new PagedList<InternshipApplicationView>
                {
                    CurrentPage = internshipApplications.CurrentPage,
                    DatabaseRecordsCount = internshipApplications.DatabaseRecordsCount,
                    LastPage = internshipApplications.LastPage,
                    PageSize = internshipApplications.PageSize,
                    Data = internshipApplications.Data.Select(x => new InternshipApplicationView(x)).ToList()
                };
                return Request.CreateResponse(HttpStatusCode.OK, pagedApplications);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request.", ex);
            }
        }

        [HttpGet, Route("GetUnaccepted")]
        public async Task<HttpResponseMessage> GetUnacceptedAsync([FromUri] Sorting sorting = null, [FromUri] Paging paging = null, [FromUri] InternshipApplicationFilter filter = null)
        {
            try
            {
                PagedList<InternshipApplication> pagedList = await InternshipApplicationService.GetUnacceptedAsync(paging, sorting, filter);

                if (pagedList == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                PagedList<InternshipApplicationStudentsView> pagedListView = new PagedList<InternshipApplicationStudentsView>
                {
                    CurrentPage = pagedList.CurrentPage,
                    PageSize = pagedList.PageSize,
                    DatabaseRecordsCount = pagedList.DatabaseRecordsCount,
                    Data = pagedList.Data.Select(intershipApplication => new InternshipApplicationStudentsView(intershipApplication)).ToList(),
                    LastPage = pagedList.LastPage
                };

                return Request.CreateResponse(HttpStatusCode.OK, pagedListView);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request.", ex);
            }
        }

        public async Task<HttpResponseMessage> GetByIdAsync(Guid id)
        {
            try
            {

                if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                InternshipApplication internshipApplication = await InternshipApplicationService.GetInternshipApplicationByIdAsync(id);

                if (internshipApplication == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, new InternshipApplicationView(internshipApplication));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Authorize(Roles = "Student")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] PutInternshipApplication internshipApplication)
        {
            if (internshipApplication == null) { return Request.CreateResponse(HttpStatusCode.BadRequest); }
            string currentUserId = User.Identity.GetUserId();
            try
            {
                State state = await StateService.GetByNameAsync("Processing");

                if (state == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                InternshipApplication newInternshipApplication = new InternshipApplication()
                {
                    Id = Guid.NewGuid(),
                    InternshipId = internshipApplication.InternshipId,
                    StudentId = currentUserId,
                    StateId = state.Id,
                    Message = internshipApplication.Message,
                };

                bool success = await InternshipApplicationService.PostInternshipApplicationAsync(newInternshipApplication, currentUserId);

                if (!success) return Request.CreateResponse(HttpStatusCode.BadRequest);

                //Company existingCompany = null;
                //await NotificationService.AddAsync("Account rejected", "Dear " + existingCompany.GetFullName() + "!\n\nYour registration for the company " + existingCompany.Name + " on the platform InternHub has been rejected. If you think it's a mistake fell free to contact us!" + " \n\nYour InternHub team", existingCompany);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            InternshipApplication existingInternshipApplication = await InternshipApplicationService.GetInternshipApplicationByIdAsync(id);
            string currentUserId = User.Identity?.GetUserId();

            if (existingInternshipApplication == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any internship application with that id!"); }

            if (await InternshipApplicationService.DeleteAsync(existingInternshipApplication, currentUserId) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Couldn't delete internship application!");
            }
            return Request.CreateResponse(HttpStatusCode.OK, $"Internship application with id:{id} was deleted!");
        }

        [Authorize(Roles = "Student")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid internshipId, string studentId)
        {
            InternshipApplication existingInternshipApplication = await InternshipApplicationService.GetByInternshipAsync(internshipId, studentId);
            string currentUserId = User.Identity?.GetUserId();

            if (existingInternshipApplication == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any internship application with that id!"); }

            if (await InternshipApplicationService.DeleteAsync(existingInternshipApplication, currentUserId) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Couldn't delete internship application!");
            }
            return Request.CreateResponse(HttpStatusCode.OK, $"Internship application with id:{existingInternshipApplication.Id} was deleted!");
        }

        [HttpGet]
        [Route("GetId")]
        public async Task<HttpResponseMessage> GetIdAsync(string studentId, Guid internshipId)
        {
            Guid? result = await InternshipApplicationService.GetIdAsync(studentId, internshipId);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        [HttpPost, Authorize(Roles = "Company"), Route("Accept")]
        public async Task<HttpResponseMessage> AcceptAsync(Guid id, bool isAccepted)
        {
            try
            {
                State state = await StateService.GetByNameAsync(isAccepted ? "Accepted" : "Declined");
                if (state == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                InternshipApplication internshipApplication = await InternshipApplicationService.GetInternshipApplicationByIdAsync(id);
                if (internshipApplication == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                internshipApplication.StateId = state.Id;
                bool result = await InternshipApplicationService.PutAsync(internshipApplication, User.Identity.GetUserId());

                if (!result) return Request.CreateResponse(HttpStatusCode.BadRequest);

                string title = "Internship Application Update";

                if (isAccepted)
                {
                    await NotificationService.AddAsync(title, "Dear " + internshipApplication.Student.GetFullName() + "!\n\nYour application for the " + internshipApplication.Internship.Name + " internship on the platform InternHub is accepted. The company will soon update you about further steps!" + " \n\nYour InternHub team", internshipApplication.Student);
                }
                else
                {
                    await NotificationService.AddAsync(title, "Dear " + internshipApplication.Student.GetFullName() + "!\n\nYour application for the " + internshipApplication.Internship.Name + " internship on the platform InternHub is declined. Don't let that discourage you, another opportunity will come soon!" + " \n\nYour InternHub team", internshipApplication.Student);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError); }
        }
    }
}

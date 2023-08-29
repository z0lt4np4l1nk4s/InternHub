using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using InternHub.Service;
using InternHub.Service.Common;
using InternHub.WebApi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace InternHub.WebApi.Controllers
{
    [RoutePrefix("api/Company")]
    public class CompanyController : ApiController
    {
        private ICompanyService CompanyService { get; }
        private INotificationService NotificationService { get; }
        private RoleManager RoleManager { get; }

        public CompanyController(ICompanyService companyService, RoleManager roleManager, INotificationService notificationService)
        {
            CompanyService = companyService;
            RoleManager = roleManager;
            NotificationService = notificationService;
        }

        [Authorize]
        public async Task<HttpResponseMessage> GetAsync([FromUri] Paging paging = null, [FromUri] Sorting sorting = null, [FromUri] CompanyFilter filter = null)
        {
            PagedList<Company> pagedList = await CompanyService.GetAsync(sorting, paging, filter);

            PagedList<CompanyView> pagedListView = new PagedList<CompanyView>
            {
                CurrentPage = pagedList.CurrentPage,
                PageSize = pagedList.PageSize,
                DatabaseRecordsCount = pagedList.DatabaseRecordsCount,
                Data = pagedList.Data.Select(company => new CompanyView(company)).ToList(),
                LastPage = pagedList.LastPage
            };

            return Request.CreateResponse(HttpStatusCode.OK, pagedListView);
        }

        // GET api/<controller>/5
        [Authorize]
        public async Task<HttpResponseMessage> GetAsync(string id)
        {
            Company existingCompany = await CompanyService.GetAsync(id);

            if (existingCompany == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any company with that id!"); }

            CompanyDetailsView companyDetailsView = new CompanyDetailsView(existingCompany);

            return Request.CreateResponse(HttpStatusCode.OK, companyDetailsView);
        }

        // POST api/<controller>
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostAsync([FromBody] CompanyPost company)
        {
            if (company == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

            PasswordHasher passwordHasher = new PasswordHasher();

            Company newCompany = new Company
            {
                Name = company.Name,
                Website = company.Website,
                FirstName = company.FirstName,
                LastName = company.LastName,
                Address = company.Address,
                Description = company.Description,
                PhoneNumber = company.PhoneNumber,
                CountyId = company.CountyId,
                Email = company.Email,
                Password = passwordHasher.HashPassword(company.Password)
            };

            Role role = await RoleManager.FindByNameAsync("User");
            if (role == null) return Request.CreateResponse(HttpStatusCode.InternalServerError);

            newCompany.RoleId = role.Id;

            if (await CompanyService.PostAsync(newCompany) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request");
            }

            await NotificationService.AddAsync("Account created", "Dear " + newCompany.GetFullName() + "!\n\nYour account on the platform InternHub has been created. If you have any problems, feel free to contact us!" + " \n\nYour InternHub team", newCompany);

            return Request.CreateResponse(HttpStatusCode.OK, newCompany);
        }

        // PUT api/<controller>/5
        [Authorize(Roles = "Company,Admin")]
        public async Task<HttpResponseMessage> PutAsync(string id, [FromBody] CompanyPut updatedCompany)
        {
            Company existingCompany = await CompanyService.GetAsync(id);

            if (existingCompany == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any company with that id!"); }

            if (updatedCompany.Website != null) existingCompany.Website = updatedCompany.Website;
            if (updatedCompany.Name != null) existingCompany.Name = updatedCompany.Name;
            if (updatedCompany.FirstName != null) existingCompany.FirstName = updatedCompany.FirstName;
            if (updatedCompany.LastName != null) existingCompany.LastName = updatedCompany.LastName;
            if (updatedCompany.Address != null) existingCompany.Address = updatedCompany.Address;
            if (updatedCompany.Description != null) existingCompany.Description = updatedCompany.Description;
            if (updatedCompany.PhoneNumber != null) existingCompany.PhoneNumber = updatedCompany.PhoneNumber;
            if (updatedCompany.Email != null) existingCompany.Email = updatedCompany.Email;
            if (updatedCompany.CountyId != null) existingCompany.CountyId = updatedCompany.CountyId.Value;

            if (await CompanyService.PutAsync(existingCompany) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Couldn't update it!");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated");
        }

        // DELETE api/<controller>/5
        [Authorize(Roles = "Admin,Company")]
        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            Company existingCompany = await CompanyService.GetAsync(id);

            if (existingCompany == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any company with that id!"); }

            if (await CompanyService.DeleteAsync(existingCompany) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Couldn't delete company!");
            }

            await NotificationService.AddAsync("Account deleted", "Dear " + existingCompany.GetFullName() + "!\n\nYour account on the platform InternHub has been deleted. If you think it's a mistake fell free to contact us!" + " \n\nYour InternHub team", existingCompany);
            return Request.CreateResponse(HttpStatusCode.OK, $"Company with id: {id} was deleted!");
        }


        [HttpPut, Route("Approve")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> ApproveAsync(string id, bool isAccepted)
        {
            try
            {
                Company existingCompany = await CompanyService.GetAsync(id);

                if (existingCompany.IsAccepted) return Request.CreateResponse(HttpStatusCode.OK);

                if (existingCompany == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any company with that id!"); }


                if (isAccepted)
                {
                    Role role = await RoleManager.FindByNameAsync("Company");
                    if (role == null) throw new Exception();
                    existingCompany.RoleId = role.Id;
                    if (await CompanyService.AcceptAsync(existingCompany) == false)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Couldn't approve company!");
                    }

                    await NotificationService.AddAsync("Account accepted", "Dear " + existingCompany.GetFullName() + "!\n\nYour registration with the company " + existingCompany.Name + " on the platform InternHub has been accepted. From now on you can create your internships without interruption!" + " \n\nYour InternHub team", existingCompany);

                    return Request.CreateResponse(HttpStatusCode.OK, "Company accepted!");
                }
                else
                {
                    if (await CompanyService.DeleteAsync(existingCompany) == false)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Couldn't delete company!");
                    }
                    await NotificationService.AddAsync("Account rejected", "Dear " + existingCompany.GetFullName() + "!\n\nYour registration with the company " + existingCompany.Name + " on the platform InternHub has been rejected. If you think it's a mistake fell free to contact us!" + " \n\nYour InternHub team", existingCompany);
                    return Request.CreateResponse(HttpStatusCode.OK, $"Company with id: {id} was deleted!");
                }
            }
            catch { return Request.CreateResponse(HttpStatusCode.InternalServerError); }
        }
    }
}
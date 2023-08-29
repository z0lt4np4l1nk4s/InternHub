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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace InternHub.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Student")]
    public class StudentController : ApiController
    {
        private IStudentService StudentService { get; }
        private RoleManager RoleManager { get; }
        private INotificationService NotificationService { get; }

        public StudentController(IStudentService studentService, RoleManager roleManager, INotificationService notificationService)
        {
            StudentService = studentService;
            RoleManager = roleManager;
            NotificationService = notificationService;
        }

        // GET: Student
        [HttpGet]
        public async Task<HttpResponseMessage> GetAsync([FromUri] Sorting sorting = null, [FromUri] Paging paging = null, [FromUri] StudentFilter filter = null)
        {
            try
            {
                List<StudentView> students = new List<StudentView>();

                PagedList<Student> mappedStudents = await StudentService.GetAllAsync(sorting, paging, filter);

                mappedStudents.Data.ForEach(student =>
                {
                    StudentView studentView = new StudentView(student);
                    students.Add(studentView);
                });

                PagedList<StudentView> pagedStudents = new PagedList<StudentView>()
                {
                    CurrentPage = mappedStudents.CurrentPage,
                    DatabaseRecordsCount = mappedStudents.DatabaseRecordsCount,
                    LastPage = mappedStudents.LastPage,
                    PageSize = mappedStudents.PageSize,
                    Data = students
                };

                return Request.CreateResponse(HttpStatusCode.OK, pagedStudents);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error", ex);
            }
        }

        [HttpGet, Route("GetByInternship")]
        public async Task<HttpResponseMessage> GetByInternshipAsync(Guid internshipId)
        {
            try
            {
                List<Student> students = await StudentService.GetByInternship(internshipId);

                if (students == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                return Request.CreateResponse(HttpStatusCode.OK, students.Select(x => new StudentView(x)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error", ex);
            }
        }

        [HttpGet]
        [Route("admin")]
        public async Task<HttpResponseMessage> GetStudentViewAsAdmin([FromUri] Sorting sorting = null, [FromUri] Paging paging = null, [FromUri] StudentFilter filter = null)
        {
            try
            {
                List<StudentShortView> students = new List<StudentShortView>();

                PagedList<Student> mappedStudents = await StudentService.GetAllAsync(sorting, paging, filter);

                mappedStudents.Data.ForEach(student =>
                {
                    StudentShortView studentShortView = new StudentShortView(student);
                    students.Add(studentShortView);
                });

                PagedList<StudentShortView> pagedStudents = new PagedList<StudentShortView>()
                {
                    CurrentPage = mappedStudents.CurrentPage,
                    DatabaseRecordsCount = mappedStudents.DatabaseRecordsCount,
                    LastPage = mappedStudents.LastPage,
                    PageSize = mappedStudents.PageSize,
                    Data = students
                };

                return Request.CreateResponse(HttpStatusCode.OK, pagedStudents);
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "GRESKA", ex);
            }

        }

        [HttpGet]
        [Authorize]
        public async Task<HttpResponseMessage> GetByIdAsync(string id)
        {
            if (id == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
            Student student = await StudentService.GetStudentByIdAsync(id);

            if (student == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There isn't any student with that id!");
            }

            StudentView studentView = new StudentView(student);

            return Request.CreateResponse(HttpStatusCode.OK, studentView);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostAsync([FromBody] PostStudent student)
        {
            if (student == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            PasswordHasher passwordHasher = new PasswordHasher();

            Guid generatedId = Guid.NewGuid();

            Student mappedStudent = new Student()
            {
                Id = generatedId.ToString(),
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                Description = student.Description,
                CountyId = student.CountyId,
                StudyAreaId = student.StudyAreaId,
                Password = passwordHasher.HashPassword(student.Password)
            };

            Role role = await RoleManager.FindByNameAsync("Student");
            if (role == null) return Request.CreateResponse(HttpStatusCode.InternalServerError);

            mappedStudent.RoleId = role.Id;

            int rowsAffected = await StudentService.PostAsync(mappedStudent);

            if (rowsAffected <= 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Couldn't create new student");

            await NotificationService.AddAsync("Account created", "Dear " + mappedStudent.GetFullName() + "!\n\nYour account on the platform InternHub has been created. If you have any problems, feel free to contact us!" + " \n\nYour InternHub team", mappedStudent);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {

            Student existingStudent = await StudentService.GetStudentByIdAsync(id);


            if (existingStudent == null) { return Request.CreateResponse(HttpStatusCode.NotFound, "There isn't any student with that id! "); }

            if (await StudentService.DeleteAsync(existingStudent) <= 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to delete student!");
            }

            await NotificationService.AddAsync("Account deleted", "Dear " + existingStudent.GetFullName() + "!\n\nYour account on the platform InternHub has been deleted. If you think it's a mistake fell free to contact us!" + " \n\nYour InternHub team", existingStudent);

            return Request.CreateResponse(HttpStatusCode.OK, $"Student with id : {id} is deleted! ");

        }

        [HttpPut]
        public async Task<HttpResponseMessage> PutAsync([FromUri] string id, [FromBody] StudentPut student)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || student == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

                Student existingStudent = await StudentService.GetStudentByIdAsync(id);

                if (existingStudent == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Student with id: {id} is not found");
                }


                if (!string.IsNullOrEmpty(student.FirstName))
                {
                    existingStudent.FirstName = student.FirstName;

                }
                if (!string.IsNullOrEmpty(student.LastName))
                {
                    existingStudent.LastName = student.LastName;

                }
                if (!string.IsNullOrEmpty(student.Email))
                {
                    existingStudent.Email = student.Email;

                }
                if (!string.IsNullOrEmpty(student.PhoneNumber))
                {
                    existingStudent.PhoneNumber = student.PhoneNumber;

                }
                if (!string.IsNullOrEmpty(student.Address))
                {
                    existingStudent.Address = student.Address;

                }
                if (!string.IsNullOrEmpty(student.Description))
                {
                    existingStudent.Description = student.Description;

                }
                if (student.StudyAreaId != null)
                {
                    existingStudent.StudyAreaId = student.StudyAreaId.Value;
                }
                if (student.CountyId != null)
                {
                    existingStudent.CountyId = student.CountyId.Value;
                }

                int rowsAffected = await StudentService.PutAsync(existingStudent);

                if (rowsAffected <= 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Couldn't update student");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

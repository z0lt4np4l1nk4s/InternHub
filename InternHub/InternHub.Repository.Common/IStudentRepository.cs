using InternHub.Common.Filter;
using InternHub.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternHub.Model;

namespace InternHub.Repository.Common
{
    public interface IStudentRepository
    {
        Task<PagedList<Student>> GetStudentsAsync(Sorting sorting, Paging paging, StudentFilter filter);
        Task<PagedList<Student>> GetStudentViewAsAdminAsync(Sorting sorting, Paging paging, StudentFilter filter);
        Task<List<Student>> GetByInternship(Guid internshipId);
        Task<Student> GetStudentByIdAsync(string id);
        Task<int> PostAsync(Student student);
        Task<int> DeleteAsync(Student student);
        Task<int> PutAsync(Student student);
    }
}

using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternHub;
namespace InternHub.Service.Common
{
    public interface IStudentService
    {
        Task<PagedList<Student>> GetAllAsync(Sorting sorting, Paging paging, StudentFilter filter);
        Task<List<Student>> GetByInternship(Guid internshipId);
        Task<Student> GetStudentByIdAsync(string id);
        Task<int> PostAsync(Student student);
        Task<int> DeleteAsync(Student student);
        Task<int> PutAsync(Student student);
        
    }
}

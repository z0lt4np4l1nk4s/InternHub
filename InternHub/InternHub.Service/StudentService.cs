using InternHub.Common;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternHub.Service.Common;
using InternHub.Repository;
using InternHub.Common.Filter;
using InternHub.Repository.Common;

namespace InternHub.Service
{
    public class StudentService : IStudentService
    {
        IStudentRepository _studentRepository;
        
        public StudentService(IStudentRepository repository) 
        {
            _studentRepository = repository;
      
        }
        public async Task<PagedList<Student>> GetAllAsync(Sorting sorting, Paging paging, StudentFilter filter)
        {
            return await _studentRepository.GetStudentsAsync(sorting, paging, filter);
        }

        public async Task<List<Student>> GetByInternship(Guid internshipId)
        {
            return await _studentRepository.GetByInternship(internshipId);
        }

        public async Task<Student> GetStudentByIdAsync(string id)
        {
            return await _studentRepository.GetStudentByIdAsync(id);
        }

        public async Task<int> PostAsync(Student student)
        {
            student.DateCreated = DateTime.Now;
            student.DateUpdated = DateTime.Now;
            return await _studentRepository.PostAsync(student);
            
        }
        public async Task<int> DeleteAsync(Student student)
        {
            student.DateUpdated = DateTime.Now;
            return await _studentRepository.DeleteAsync(student);
        }

        public async Task<int> PutAsync(Student student)
        {
            student.DateUpdated = DateTime.Now;
            return await _studentRepository.PutAsync(student);
        }
    }
}

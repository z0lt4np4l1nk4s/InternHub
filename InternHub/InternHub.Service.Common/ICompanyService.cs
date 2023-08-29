using InternHub.Common;
using InternHub.Common.Filter;
using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Service.Common
{
    public interface ICompanyService
    {
        Task<PagedList<Company>> GetAsync(Sorting sorting, Paging paging, CompanyFilter filter);
        Task<Company> GetAsync(string id);
        Task<bool> PostAsync(Company company);
        Task<bool> PutAsync(Company company);
        Task<bool> DeleteAsync(Company company);
        Task<bool> AcceptAsync(Company company);
    }
}

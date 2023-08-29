using InternHub.Common;
using InternHub.Model;
using InternHub.Repository.Common;
using InternHub.Service.Common;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InternHub.Common.Filter;

namespace InternHub.Service
{
    public class CompanyService : ICompanyService
    {
        private ICompanyRepository CompanyRepository { get; set; }
        public CompanyService(ICompanyRepository companyRepository) {
            CompanyRepository = companyRepository;
        }
        public async Task<bool> DeleteAsync(Company company)
        {
            company.DateUpdated = DateTime.Now;
            return await CompanyRepository.DeleteAsync(company);
        }

        public async Task<PagedList<Company>> GetAsync(Sorting sorting, Paging paging, CompanyFilter filter)
        {
            return await CompanyRepository.GetAsync(sorting, paging, filter);
        }

        public async Task<Company> GetAsync(string id)
        {
            return await CompanyRepository.GetAsync(id);
        }

        public async Task<bool> PostAsync(Company company)
        {
            company.DateUpdated = company.DateCreated = DateTime.Now;
            company.Id = Guid.NewGuid().ToString();
            return await CompanyRepository.PostAsync(company);
        }

        public async Task<bool> PutAsync(Company company)
        {
            company.DateUpdated = DateTime.Now;
            return await CompanyRepository.PutAsync(company);
        }

        public async Task<bool> AcceptAsync(Company company)
        {
            return await CompanyRepository.AcceptAsync(company);
        }

        
    }
}

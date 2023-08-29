using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class CompanyDetailsView : UserView
    {
        public CompanyDetailsView(Company company) : base(company)
        {
            Name = company.Name;
            Website = company.Website;
        }

        public CompanyDetailsView() { }
        public string Name { get; set; }
        public string Website { get; set; }
    }
}
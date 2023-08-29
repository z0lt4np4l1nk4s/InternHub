using InternHub.Model;


namespace InternHub.WebApi.Models
{
    public class CompanyView
    {
        public CompanyView(Company company)
        {
            Name = company.Name;
            Website = company.Website;
            Address = company.Address;
            Id = company.Id;
            FirstName = company.FirstName;
            LastName = company.LastName;
        }
        public CompanyView() { }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Id { get; set; }
        
    }
}
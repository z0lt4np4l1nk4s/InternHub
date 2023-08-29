using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class CountyView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public CountyView() { }
        public CountyView(County county)
        {
            Id = county.Id;
            Name = county.Name;
        }
    }
}
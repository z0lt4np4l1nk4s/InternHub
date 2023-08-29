using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class Company : User, ICompany
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public bool IsAccepted { get; set; }
    }
}

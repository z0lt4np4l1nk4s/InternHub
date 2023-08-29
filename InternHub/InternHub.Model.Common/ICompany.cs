using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface ICompany : IUser
    {
        string Name { get; set; }
        string Website { get; set; }
        bool IsAccepted { get; set; }
    }
}

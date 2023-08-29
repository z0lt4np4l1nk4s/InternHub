using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface IBaseModel
    {
        Guid Id { get; set; }
        DateTime DateCreated { get; set; }
        string CreatedByUserId { get; set; }
        string UpdatedByUserId { get; set; }
        bool IsActive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface IState : IBaseModel
    {
        string Name { get; set; }
    }
}

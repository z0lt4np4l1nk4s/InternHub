using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model.Common
{
    public interface IRole
    {
        string Id { get; set; }
        string Name { get; set; }
    }
}

using InternHub.Model.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternHub.Model;
using InternHub.Service.Common;

namespace InternHub.Service
{
    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(IRoleStore<Role, string> roleStore) : base(roleStore) { }
    }
}

using InternHub.Model.Common;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class Role : IdentityRole, IRole
    {
        public Role() : base() { }
        public Role(string roleName) : base(roleName) { }
    }
}

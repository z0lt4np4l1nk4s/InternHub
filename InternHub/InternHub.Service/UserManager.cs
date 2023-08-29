using InternHub.Model.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternHub.Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using InternHub.Service.Common;

namespace InternHub.Service
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store) : base(store) { }
    }
}

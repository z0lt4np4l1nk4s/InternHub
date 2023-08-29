using InternHub.Model.Common;
using InternHub.Model.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class User : IdentityUser, Common.IUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CountyId { get; set; }
        [NotMapped]
        public County County { get; set; }
        [NotMapped]
        public string Password { get; set; } = "";
        [NotMapped]
        public string RoleId { get; set; } = "";

        public string GetFullName() => FirstName + " " + LastName;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}

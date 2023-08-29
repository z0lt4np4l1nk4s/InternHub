namespace InternHub.Model.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<InternHub.Model.Identity.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(InternHub.Model.Identity.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            // Create the role if it doesn't exist

            Role adminRole = new Role { Id = "0eea48f7-a781-428d-997c-97734b9a1321", Name = "Admin" };

            if (!context.Roles.Any(x => x.Name == "Admin"))
            {
                context.Roles.AddOrUpdate(adminRole);
                context.Roles.AddOrUpdate(new Role { Id = "cfabc5fe-99c2-4449-9f3b-b6c27694e22c", Name = "Student" });
                context.Roles.AddOrUpdate(new Role { Id = "3c622019-b413-4363-ba44-9fc67c3d35a4", Name = "Company" });
                context.Roles.AddOrUpdate(new Role { Id = "b4f33ce1-1fcf-4be6-8bed-fc9c77a59594", Name = "User" });
            }

            if (!context.Users.Any(u => u.UserName == "admin@mono.com"))
            {
                User user = new User
                {
                    UserName = "admin@mono.com",
                    Email = "admin@mono.com",
                    Address = "IT Park 1",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Description = "I am the admin",
                    SecurityStamp = "",
                    PhoneNumber = "",
                    IsActive = true,
                    CountyId = Guid.Parse("517bf28e-8064-4731-b9b9-099eb8d39f2f"),
                    PasswordHash = new PasswordHasher().HashPassword("123456")
                };
                user.Id = "0c2ba6ff-9145-43cf-ac48-ccf7effea536";
                var userStore = new UserStore<User>(context);
                context.Users.Add(user);

                // Assign the user to the role
                var userRole = new IdentityUserRole
                {
                    UserId = user.Id,
                    RoleId = adminRole.Id
                };
                context.Set<IdentityUserRole>().Add(userRole);
            }
        }
    }
}

using InternHub.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base(Environment.GetEnvironmentVariable("ConnectionString"), throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Ignore(x => x.Password).Ignore(x => x.RoleId);
            modelBuilder.Entity<Student>().Ignore(x => x.StudyArea).Ignore(x => x.StudyAreaId);
            modelBuilder.Entity<Company>().Ignore(x => x.Website).Ignore(x => x.IsAccepted).Ignore(x => x.Name);
            modelBuilder.Ignore<StudyArea>();
        }
    }
}

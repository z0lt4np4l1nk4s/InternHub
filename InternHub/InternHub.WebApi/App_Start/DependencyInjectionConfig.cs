using Autofac;
using Autofac.Integration.WebApi;
using InternHub.Model;
using InternHub.Model.Common;
using InternHub.Model.Identity;
using InternHub.Repository;
using InternHub.Service;
using InternHub.Service.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace InternHub.WebApi.App_Start
{
    public class DependencyInjectionConfig
    {
        public static IContainer Container { get; private set; }

        public static void Register()
        {
            var builder = new ContainerBuilder();


            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new RepositoryModule());
            builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();

            builder.RegisterType<UserStore<User>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<RoleStore<Role>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register(c => new IdentityFactoryOptions<UserManager>()
            {
                DataProtectionProvider = new DpapiDataProtectionProvider("InternHub")
            }).AsSelf().InstancePerLifetimeScope();


            // Register the ApplicationUserManager
            builder.Register(c =>
            {
                var manager = new UserManager(new UserStore<User>(c.Resolve<ApplicationDbContext>()));
                // Configure validation logic for usernames
                manager.UserValidator = new UserValidator<User>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };
                // Configure validation logic for passwords
                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    //RequireNonLetterOrDigit = true,
                    //RequireDigit = true,
                    //RequireLowercase = true,
                    //RequireUppercase = true,
                };
                var dataProtectionProvider = c.Resolve<IdentityFactoryOptions<UserManager>>().DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;
            }).InstancePerLifetimeScope();

            // Register the ApplicationRoleManager
            builder.Register(c =>
            {
                var applicationRoleManager = new RoleManager(new RoleStore<Role>(c.Resolve<ApplicationDbContext>()));
                return applicationRoleManager;
            }).InstancePerLifetimeScope();

            builder.Register(x => new ConnectionString { Name = Environment.GetEnvironmentVariable("ConnectionString") }).As<IConnectionString>();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            // Set the dependency resolver to be Autofac.
            Container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
        }
    }
}
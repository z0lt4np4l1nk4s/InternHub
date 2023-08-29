using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Repository
{
    public class RepositoryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var k = builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.Name.EndsWith("Repository"))
                .As(t => t.GetInterfaces().FirstOrDefault(x => x.Name == "I" + t.Name));
        }
    }
}

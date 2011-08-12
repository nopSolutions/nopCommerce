using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.Twitter.Core;

namespace Nop.Plugin.ExternalAuth.Twitter
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<TwitterProviderAuthorizer>().As<IOAuthProviderTwitterAuthorizer>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

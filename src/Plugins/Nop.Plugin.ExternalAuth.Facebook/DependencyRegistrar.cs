using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.Facebook.Core;

namespace Nop.Plugin.ExternalAuth.Facebook
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<FacebookProviderAuthorizer>().As<IOAuthProviderFacebookAuthorizer>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

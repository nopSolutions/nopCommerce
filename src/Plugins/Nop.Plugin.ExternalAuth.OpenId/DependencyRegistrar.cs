using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.OpenId.Core;

namespace Nop.Plugin.ExternalAuth.OpenId
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<OpenIdProviderAuthorizer>().As<IOpenIdProviderAuthorizer>().InstancePerHttpRequest();
            builder.RegisterType<OpenIdRelyingPartyService>().As<IOpenIdRelyingPartyService>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

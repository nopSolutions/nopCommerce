using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Weixin;

namespace Senparc.Weixin.MP.CommonService.Infrastructure
{
    /// <summary>
    /// Represents a plugin dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WConfigService>().As<IWConfigService>().InstancePerLifetimeScope();
            builder.RegisterType<WLocationService>().As<IWLocationService>().InstancePerLifetimeScope();
            builder.RegisterType<WMessageBindService>().As<IWMessageBindService>().InstancePerLifetimeScope();
            builder.RegisterType<WMessageService>().As<IWMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<WQrCodeImageService>().As<IWQrCodeImageService>().InstancePerLifetimeScope();
            builder.RegisterType<WQrCodeLimitService>().As<IWQrCodeLimitService>().InstancePerLifetimeScope();
            builder.RegisterType<WQrCodeLimitUserService>().As<IWQrCodeLimitUserService>().InstancePerLifetimeScope();
            builder.RegisterType<WUserService>().As<IWUserService>().InstancePerLifetimeScope();
            builder.RegisterType<WUserTagService>().As<IWUserTagService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
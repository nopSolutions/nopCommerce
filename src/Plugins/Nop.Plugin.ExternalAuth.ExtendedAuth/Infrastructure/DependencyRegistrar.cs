using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.ExtendedAuth.Service;
using Nop.Services.Authentication.External;
using Telegram.Bot;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1000; }
        }

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ExternalAuthenticationService_Override>().As<IExternalAuthenticationService>().InstancePerLifetimeScope();
            builder
                .Register(ctx => new TelegramBotClient(config.TelegramBotSecret))
                .As<ITelegramBotClient>()
                .SingleInstance();
        }
    }
}

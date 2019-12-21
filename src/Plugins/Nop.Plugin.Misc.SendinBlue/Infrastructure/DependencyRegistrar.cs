using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.SendinBlue.Services;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.SendinBlue.Infrastructure
{
    /// <summary>
    /// Represents a plugin dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, NopConfig config)
        {
            //register custom services
            services.AddScoped<SendinBlueManager>();
            services.AddScoped<SendinBlueMarketingAutomationManager>();

            //override services
            services.AddScoped<IWorkflowMessageService, SendinBlueMessageService>();
            services.AddScoped<IEmailSender, SendinBlueEmailSender>();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}
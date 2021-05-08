using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService_Override>();
            services.AddSingleton<WeekdayProductRotation>();

            services.AddSingleton<ITelegramBotClient, TelegramBotClient>(ctx => 
                new TelegramBotClient(appSettings.ExtendedAuthSettings.TelegramBotSecret));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using NopBrasil.Plugin.Payments.PagSeguro.Services;

namespace NopBrasil.Plugin.Payments.PagSeguro.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 100;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //client to request pag seguro services
            services.AddHttpClient<PagSeguroHttpClient>().WithProxy();
        }
    }
}

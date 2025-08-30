using System.Collections.Generic;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.Momo.Infrastructure;

/// <summary>
/// Represents object for the configuring scripts of the plugin
/// </summary>
public class ScriptRegistrar : INopStartup
{
    public int Order => 101;

    public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder application)
    {
        //no need to configure
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //add custom resources
        services.AddScriptResource("momo.payment.js", "/Plugins/Payments.Momo/Scripts/momo.payment.js", true);
    }
}

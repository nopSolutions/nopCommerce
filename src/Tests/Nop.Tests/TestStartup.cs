using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Tests.Nop.Web.Tests.Public.Factories;

namespace Nop.Tests;

public class TestStartup : ITestNopStartup
{
    public int Order => 1;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ProductModelFactoryTests.ProductModelFactoryForTest>();
    }
}

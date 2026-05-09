using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Tests;

public interface ITestNopStartup : INopStartup
{
}

public abstract class TestNopStartup : ITestNopStartup
{
    public virtual int Order => 500;

    public void Configure(IApplicationBuilder application)
    {
    }

    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}

public abstract class TestNopStartup<T> : TestNopStartup where T : INopStartup, new()
{
    private T _implementation = new();

    public override int Order => _implementation.Order;

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => _implementation.ConfigureServices(services, configuration);
}

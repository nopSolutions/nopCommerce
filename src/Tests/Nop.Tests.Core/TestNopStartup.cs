using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Tests;

/// <summary>
/// Represents object for the configuring test services on test application startup
/// </summary>
public interface ITestNopStartup : INopStartup
{
}

public abstract class TestNopStartup : ITestNopStartup
{
    public virtual int Order => 500;
    public virtual bool CallConfigure => false;

    public virtual void Configure(IApplicationBuilder application)
    {
        if (application != null && CallConfigure)
            Configure(application);
    }

    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}

public abstract class TestNopStartup<T> : TestNopStartup where T : INopStartup, new()
{
    private T _target = new();

    public override int Order => _target.Order;

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => _target.ConfigureServices(services, configuration);
}

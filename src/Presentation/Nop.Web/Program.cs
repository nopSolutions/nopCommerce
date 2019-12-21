using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Nop.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                // Add a new service provider configuration
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = false;
                    options.ValidateOnBuild = false;
                })
                .Build();

            await host.RunAsync();
        }
    }
}
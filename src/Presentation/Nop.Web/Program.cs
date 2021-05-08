using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nop.Services.Configuration;

namespace Nop.Web
{
    public class Program
    {
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task Main(string[] args)
        {
            //initialize the host
            using var host = Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(options =>
                {
                    //we don't validate the scopes, since at the app start and the initial configuration we need 
                    //to resolve some services (registered as "scoped") through the root container
                    options.ValidateScopes = false;
                    options.ValidateOnBuild = true;
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .ConfigureAppConfiguration(config =>
                    {
                        config
                            .AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true)
                            .AddEnvironmentVariables();
                    })
                    .UseStartup<Startup>())
                .Build();

            //start the program, a task will be completed when the host starts
            await host.StartAsync();

            //a task will be completed when shutdown is triggered
            await host.WaitForShutdownAsync();
        }
    }
}
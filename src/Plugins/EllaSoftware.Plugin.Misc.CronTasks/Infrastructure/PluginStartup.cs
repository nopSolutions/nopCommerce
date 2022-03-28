using System;
using System.Threading.Tasks;
using EllaSoftware.Plugin.Misc.CronTasks.Models;
using EllaSoftware.Plugin.Misc.CronTasks.Services;
using EllaSoftware.Plugin.Misc.CronTasks.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Quartz;

namespace EllaSoftware.Plugin.Misc.CronTasks.Infrastructure
{
    public class PluginStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IValidator<CronTaskModel>, CronTaskValidator>();
        }

        public void Configure(IApplicationBuilder application)
        {
            var scheduler = application.ApplicationServices.GetRequiredService<IScheduler>();
            var appLifetime = application.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var cronTaskService = application.ApplicationServices.GetRequiredService<ICronTaskService>();
            var logger = application.ApplicationServices.GetRequiredService<ILogger>();

            appLifetime.ApplicationStarted.Register(async () => await StartScheduler(scheduler, cronTaskService, logger));
            appLifetime.ApplicationStopped.Register(async () => await scheduler.Shutdown());
        }

        private async Task StartScheduler(IScheduler scheduler,
            ICronTaskService cronTaskService,
            ILogger logger)
        {
            await scheduler.Start();

            //add jobs
            foreach (var cronTask in await cronTaskService.GetCronTasksAsync())
            {
                try
                {
                    await cronTaskService.RescheduleQuartzJobAsync(cronTask.Key, cronTask.Value);
                }
                catch(Exception ex)
                {
                    await logger.ErrorAsync($"Error to run a cron task (ID={cronTask.Key}). {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 101;
    }
}

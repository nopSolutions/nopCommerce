using Autofac;
using EllaSoftware.Plugin.Misc.CronTasks.Services;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EllaSoftware.Plugin.Misc.CronTasks.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(
               IServiceCollection services,
               ITypeFinder typeFinder,
               AppSettings appSettings
        ) {
            services.AddScoped<ICronTaskService, CronTaskService>();

            // quartz
            services.AddSingleton<StdSchedulerFactory>();
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            services.AddSingleton<IScheduler>(scheduler);
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

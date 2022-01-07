using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Services.ScheduleTasks;


namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks
{
    public class TestTaskScheduler: TaskScheduler
    {
        public TestTaskScheduler(AppSettings appSettings, IHttpClientFactory httpClientFactory, IScheduleTaskService scheduleTaskService, IServiceScopeFactory serviceScopeFactory, IStoreContext storeContext) : base(appSettings, httpClientFactory, scheduleTaskService, serviceScopeFactory, storeContext)
        {
        }

        public bool IsInit => _taskThreads.Any();

        public bool IsRun => _taskThreads.All(p => p.IsStarted && !p.IsDisposed);
    }
}

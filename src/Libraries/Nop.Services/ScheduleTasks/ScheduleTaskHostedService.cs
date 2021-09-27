using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Nop.Services.ScheduleTasks
{
    public class ScheduleTaskHostedService : IHostedService
    {
        private readonly ITaskScheduler _taskScheduler;

        public ScheduleTaskHostedService(ITaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _taskScheduler.InitializeAsync();
            _taskScheduler.StartScheduler();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _taskScheduler.StopScheduler();

            return Task.CompletedTask;
        }
    }
}
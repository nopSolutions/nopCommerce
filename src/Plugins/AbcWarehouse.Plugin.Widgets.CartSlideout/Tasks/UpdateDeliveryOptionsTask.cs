using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks
{
    public class UpdateDeliveryOptionsTask : IScheduleTask
    {
        private readonly ILogger _logger;

        public UpdateDeliveryOptionsTask(
            ILogger logger)
        {
            _logger = logger;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _logger.InformationAsync("Ran UpdateDeliveryOptionsTask");
        }
    }
}

using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.EnhancedLogging.Tasks
{
    class EnhancedClearLogTask : IScheduleTask
    {
        private readonly INopDataProvider _nopDataProvider;
        private readonly EnhancedLoggingSettings _settings;

        public EnhancedClearLogTask(
            INopDataProvider nopDataProvider,
            EnhancedLoggingSettings settings)
        {
            _nopDataProvider = nopDataProvider;
            _settings = settings;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            var command = $@"
                DELETE FROM {_nopDataProvider.GetTable<Log>().TableName}
                WHERE CreatedOnUtc < GETDATE() - {_settings.DaysToKeepLogs}
            ";
            await _nopDataProvider.ExecuteNonQueryAsync(command);
        }
    }
}

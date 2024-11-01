using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using Nop.Services.Catalog;
using Nop.Services.Tasks;
using Nop.Data;
using Nop.Services.Logging;
using Microsoft.Data.SqlClient;

namespace Nop.Plugin.Misc.AbcCore.Tasks
{
    class UpdatePdpVideosTask : IScheduleTask
    {
        private readonly CoreSettings _settings;
        private readonly ILogger _logger;

        public UpdatePdpVideosTask(
            CoreSettings settings,
            ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _logger.InformationAsync("Updating PDP videos.");

            using (SqlConnection connection = new SqlConnection(_settings.StagingDbConnectionString))
            {
                var queryString = "SELECT * FROM rws_product_video";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            await _logger.InformationAsync("PDP videos updated.");
        }
    }
}

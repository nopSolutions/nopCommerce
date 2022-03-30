using Nop.Plugin.Misc.AbcCore;
using Nop.Services.Tasks;
using System.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Services;

namespace Nop.Plugin.Misc.AbcCore
{
    class ArchiveTask : IScheduleTask
    {
        private readonly ArchiveService _archiveService;
        private readonly CoreSettings _coreSettings;

        public ArchiveTask(
            ArchiveService archiveService,
            CoreSettings coreSettings
        )
        {
            _archiveService = archiveService;
            _coreSettings = coreSettings;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            using (IDbConnection backendConn = _coreSettings.GetBackendDbConnection())
            {
                await _archiveService.ArchiveProductContentAsync(backendConn);
            }
            
        }
    }
}

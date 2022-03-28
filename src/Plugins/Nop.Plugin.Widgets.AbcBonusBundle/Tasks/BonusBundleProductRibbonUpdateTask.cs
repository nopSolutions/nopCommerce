using Nop.Data;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Tasks
{
    public class BonusBundleProductRibbonUpdateTask : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly INopDataProvider _dbContext;

        public BonusBundleProductRibbonUpdateTask(ILogger logger,
            INopDataProvider dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync()
        {
            const string ProductRibbonName = "BONUS BUNDLE TEMPLATE";

            var conditionIdCommand = @"
	        SELECT TOP 1 ec.ConditionId FROM SS_PR_ProductRibbon pr
	        JOIN SS_C_EntityCondition ec ON pr.Id = ec.EntityId
	        WHERE pr.Name = 'BONUS BUNDLE TEMPLATE'
	        AND ec.EntityType = 30
            ";
            var conditionId = (await _dbContext.QueryAsync<int?>(conditionIdCommand)).FirstOrDefault();
            if (conditionId == null)
            {
                await _logger.WarningAsync($"Did not find condition ID needed for Bonus Bundle Product Ribbons sync, make sure '{ProductRibbonName}' product ribbon exists.");
                return;
            }

            var syncCommand = $@"
            DELETE FROM SS_C_ProductOverride
	        WHERE ConditionId = {conditionId}

	        INSERT INTO SS_C_ProductOverride
	        SELECT {conditionId} AS ConditionId, p.Id AS ProductId, 0 FROM ProductAbcBundles pab
	        JOIN Product p ON p.Sku = pab.Sku COLLATE SQL_Latin1_General_CP1_CI_AS
            ";
            await _dbContext.ExecuteNonQueryAsync(syncCommand);
        }
    }
}

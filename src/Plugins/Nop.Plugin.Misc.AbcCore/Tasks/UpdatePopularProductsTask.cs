using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using Nop.Services.Catalog;
using Nop.Services.Tasks;
using Nop.Data;

namespace Nop.Plugin.Misc.AbcCore.Tasks
{
    class UpdatePopularProductsTask : IScheduleTask
    {
        private readonly INopDataProvider _nopDataProvider;

        public UpdatePopularProductsTask(INopDataProvider nopDataProvider)
        {
            _nopDataProvider = nopDataProvider;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _nopDataProvider.ExecuteNonQueryAsync(@"
                IF OBJECT_ID('tempdb..#productSalesCount') IS NOT NULL
                DROP TABLE #productSalesCount;

                CREATE TABLE #productSalesCount
                (
                    ProductId INT PRIMARY KEY,
                    SalesCount INT
                );

                INSERT INTO #productSalesCount
                SELECT p.Id, count(*) as SalesCount
                FROM OrderItem oi
                JOIN Product p ON oi.ProductId = p.Id
                GROUP BY p.Id;

                UPDATE pcm
                SET pcm.DisplayOrder = psc.SalesCount * -1
                FROM Product_Category_Mapping pcm
                INNER JOIN #productSalesCount psc ON psc.ProductId = pcm.ProductId;

                IF OBJECT_ID('tempdb..#productSalesCount') IS NOT NULL
                DROP TABLE #productSalesCount;
            ");
        }
    }
}

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

            await _nopDataProvider.ExecuteNonQueryAsync(@"
                -- promotes Hawthorne-specific items
                UPDATE
                    pcm
                SET
                    -- using 1000 as an arbitrary offset
                    pcm.DisplayOrder = pcm.DisplayOrder - 1000
                FROM
                    Product_Category_Mapping AS pcm
                    JOIN Product AS p ON p.Id = pcm.ProductId
                    JOIN Product_Manufacturer_Mapping AS pmm ON pmm.ProductId = p.Id
                WHERE ManufacturerId IN (
                    SELECT Id FROM Manufacturer
                    -- Hawthorne-specific brands
                    WHERE Name IN ('WOLF', 'SUBZERO', 'COVE')
                )
            ");
        }
    }
}

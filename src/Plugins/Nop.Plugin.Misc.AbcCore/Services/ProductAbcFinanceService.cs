using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using System.Threading.Tasks;
using System.Linq;
using LinqToDB.Data;

// Since the table doesn't have an ID value, I need to directly get data from
// a query and cannot use IRepository
namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class ProductAbcFinanceService : IProductAbcFinanceService
    {
        private readonly INopDataProvider _nopDataProvider;

        public ProductAbcFinanceService(
            INopDataProvider nopDataProvider
        )
        {
            _nopDataProvider = nopDataProvider;
        }

        public async Task<ProductAbcFinance> GetProductAbcFinanceByAbcItemNumberAsync(string abcItemNumber)
        {
            var sql = @"SELECT
                           AbcItemNumber,
                           Sku,
                           Description,
                           Months,
                           TransPromo,
                           Fix_Pay as IsMonthlyPricing,
                           Min_Pay as IsDeferredPricing,
                           BegDate as StartDate,
                           EndDate
                        FROM ProductAbcFinance
                        WHERE AbcItemNumber = @abcItemNumber";
            var pafs = await _nopDataProvider.QueryAsync<ProductAbcFinance>(
                sql,
                new DataParameter[]
                {
                    new DataParameter("abcItemNumber", abcItemNumber)
                }
            );
            return pafs.FirstOrDefault();
        }
    }
}

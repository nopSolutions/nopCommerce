using Nop.Plugin.Misc.AbcCore.Domain;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IProductAbcFinanceService
    {
        Task<ProductAbcFinance> GetProductAbcFinanceByAbcItemNumberAsync(string abcItemNumber);
    }
}

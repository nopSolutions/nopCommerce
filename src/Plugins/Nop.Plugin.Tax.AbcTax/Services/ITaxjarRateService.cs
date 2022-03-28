using System.Threading.Tasks;
using Address = Nop.Core.Domain.Common.Address;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public partial interface ITaxjarRateService
    {
        Task<decimal> GetTaxJarRateAsync(Address address);
    }
}
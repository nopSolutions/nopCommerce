using System.Threading.Tasks;
using Address = Nop.Core.Domain.Common.Address;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public partial interface ITaxjarRateService
    {
        Task<decimal> GetTaxJarRateAsync(TaxRateRequest taxRateRequest);
    }
}
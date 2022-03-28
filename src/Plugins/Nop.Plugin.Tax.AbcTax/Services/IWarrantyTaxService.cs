using System.Threading.Tasks;
using Address = Nop.Core.Domain.Common.Address;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public partial interface IWarrantyTaxService
    {
        Task<(decimal taxRate, decimal sciSubTotalInclTax)> CalculateWarrantyTaxAsync(
            ShoppingCartItem sci,
            Customer customer,
            decimal sciSubTotalExclTax
        );
        Task<(decimal taxRate, decimal sciSubTotalInclTax, decimal sciUnitPriceInclTax, decimal warrantyUnitPriceExclTax, decimal warrantyUnitPriceInclTax)> CalculateWarrantyTaxAsync(
            ShoppingCartItem sci,
            Customer customer,
            decimal sciSubTotalExclTax,
            decimal sciUnitPriceExclTax
        );
    }
}
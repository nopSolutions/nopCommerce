using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Tax;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Services.Common;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public partial class WarrantyTaxService : IWarrantyTaxService
    {
        private readonly IAttributeUtilities _attributeUtilities;
        private readonly IImportUtilities _importUtilities;

        private readonly IAbcTaxService _abcTaxService;
        private readonly IAddressService _addressService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly ITaxService _taxService;
        private readonly ITaxCategoryService _taxCategoryService;

        public WarrantyTaxService(
            IAttributeUtilities attributeUtilities,
            IImportUtilities importUtilities,
            IAbcTaxService abcTaxService,
            IAddressService addressService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            ITaxService taxService,
            ITaxCategoryService taxCategoryService
        )
        {
            _attributeUtilities = attributeUtilities;
            _importUtilities = importUtilities;
            _abcTaxService = abcTaxService;
            _addressService = addressService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _taxService = taxService;
            _taxCategoryService = taxCategoryService;
        }

        public async Task<(decimal taxRate, decimal sciSubTotalInclTax)> CalculateWarrantyTaxAsync(
            ShoppingCartItem sci,
            Customer customer,
            decimal sciSubTotalExclTax
        ) {
            var result = await CalculateWarrantyTaxAsync(
                sci,
                customer,
                sciSubTotalExclTax,
                sciSubTotalExclTax / sci.Quantity
            );
            return (result.taxRate, result.sciSubTotalInclTax);
        }

        public async Task<(decimal taxRate, decimal sciSubTotalInclTax, decimal sciUnitPriceInclTax, decimal warrantyUnitPriceExclTax, decimal warrantyUnitPriceInclTax)> CalculateWarrantyTaxAsync(
            ShoppingCartItem sci,
            Customer customer,
            decimal sciSubTotalExclTax,
            decimal sciUnitPriceExclTax
        ) {
            var product = await _productService.GetProductByIdAsync(sci.ProductId);
            var sciSubTotalInclTax = await _taxService.GetProductPriceAsync(product, sciSubTotalExclTax, true, customer);
            var sciUnitPriceInclTax = await _taxService.GetProductPriceAsync(product, sciUnitPriceExclTax, true, customer);

            var warrantyUnitPriceExclTax = decimal.Zero;
            (decimal price, decimal taxRate) warrantyUnitPriceInclTax;
            warrantyUnitPriceInclTax.price = decimal.Zero;
            warrantyUnitPriceInclTax.taxRate = decimal.Zero;

            // warranty item handling
            ProductAttributeMapping warrantyPam = await _attributeUtilities.GetWarrantyAttributeMappingAsync(sci.AttributesXml);
            if (warrantyPam != null)
            {
                warrantyUnitPriceExclTax =
                    (await _productAttributeParser.ParseProductAttributeValuesAsync(sci.AttributesXml))
                    .Where(pav => pav.ProductAttributeMappingId == warrantyPam.Id)
                    .Select(pav => pav.PriceAdjustment)
                    .FirstOrDefault();

                // get warranty "product" - this is so the warranties have a tax category
                Product warrProduct = _importUtilities.GetExistingProductBySku("WARRPLACE_SKU");

                var isCustomerInTaxableState = await IsCustomerInTaxableStateAsync(customer);

                if (warrProduct == null)
                {
                    // taxed warranty price
                    warrantyUnitPriceInclTax = await _taxService.GetProductPriceAsync(product, warrantyUnitPriceExclTax, false, customer);
                }
                else
                {
                    warrantyUnitPriceInclTax = await _taxService.GetProductPriceAsync(warrProduct, warrantyUnitPriceExclTax, isCustomerInTaxableState, customer);
                }

                var productUnitPriceInclTax
                    = await _taxService.GetProductPriceAsync(product, sciUnitPriceExclTax - warrantyUnitPriceExclTax, true, customer);

                sciUnitPriceInclTax.price = productUnitPriceInclTax.price + warrantyUnitPriceInclTax.price;
                sciSubTotalInclTax.price = sciUnitPriceInclTax.price * sci.Quantity;
            }

            return (warrantyUnitPriceInclTax.taxRate, sciSubTotalInclTax.price, sciUnitPriceInclTax.price, warrantyUnitPriceExclTax, warrantyUnitPriceInclTax.price);
        }

        private async Task<bool> IsCustomerInTaxableStateAsync(Customer customer)
        {
            var taxCategory = (await _taxCategoryService.GetAllTaxCategoriesAsync()).FirstOrDefault(x => x.Name == "Warranties");
            var shippingAddress = customer.ShippingAddressId.HasValue ?
                await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value) :
                null;
            if (shippingAddress == null) return false;

            return await _abcTaxService.GetAbcTaxRateAsync(
                // for now this should be fine, since all rates apply to all stores
                0,
                taxCategory?.Id ?? 0,
                shippingAddress
            ) != null;
        }
    }
}
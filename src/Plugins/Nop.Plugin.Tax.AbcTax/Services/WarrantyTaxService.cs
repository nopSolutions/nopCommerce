using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Tax;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Services.Common;
using Nop.Plugin.Misc.AbcCore.Delivery;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public partial class WarrantyTaxService : IWarrantyTaxService
    {
        private readonly IAttributeUtilities _attributeUtilities;
        private readonly IImportUtilities _importUtilities;

        private readonly IAbcTaxService _abcTaxService;
        private readonly IAddressService _addressService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly ITaxService _taxService;
        private readonly ITaxCategoryService _taxCategoryService;

        public WarrantyTaxService(
            IAttributeUtilities attributeUtilities,
            IImportUtilities importUtilities,
            IAbcTaxService abcTaxService,
            IAddressService addressService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
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
            _productAttributeService = productAttributeService;
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

            // adjust the pricing based on whether delivery options exist
            var isCustomerInTaxableState = await IsCustomerInTaxableStateAsync(customer);
            if (!isCustomerInTaxableState)
            {
                var pams = await _productAttributeParser.ParseProductAttributeMappingsAsync(sci.AttributesXml);
                foreach (var pam in pams)
                {
                    var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                    var paName = pa.Name;
                    var pasToRemove = new string[] {
                        AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName,
                        AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName,
                        AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName,
                        AbcDeliveryConsts.WarrantyProductAttributeName
                    };
                    if (pasToRemove.Contains(paName))
                    {
                        var pavs = await _productAttributeParser.ParseProductAttributeValuesAsync(sci.AttributesXml, pam.Id);
                        foreach (var pav in pavs)
                        {
                            sciSubTotalInclTax.price -= (pav.PriceAdjustment * (sciUnitPriceInclTax.taxRate / 100)) * sci.Quantity;
                            sciUnitPriceInclTax.price -= (pav.PriceAdjustment * (sciUnitPriceInclTax.taxRate / 100));
                        }
                    }
                }
            }

            return (0, sciSubTotalInclTax.price, sciUnitPriceInclTax.price, 0, 0);
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
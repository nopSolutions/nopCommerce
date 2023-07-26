using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Services.Catalog;
using Nop.Services.Tax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcFrontend.Services
{
    public class WarrantyService : IWarrantyService
    {
        public readonly string WARRANTY_PLACEHOLDER_SKU = "WARRPLACE_SKU";

        private readonly IRepository<WarrantySku> _warrantySkuRepository;

        public readonly IAttributeUtilities _attributeUtilities;
        private readonly ITaxCategoryService _taxCategoryService;
        public readonly IProductAttributeParser _productAttributeParser;
        public readonly IImportUtilities _importUtilities;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;

        public WarrantyService(
            IRepository<WarrantySku> warrantySkuRepository,
            IAttributeUtilities attributeUtilities,
            ITaxCategoryService taxCategoryService,
            IProductAttributeParser productAttributeParser,
            IImportUtilities importUtilities,
            IProductService productService,
            IProductAttributeService productAttributeService
        )
        {
            _warrantySkuRepository = warrantySkuRepository;
            _attributeUtilities = attributeUtilities;
            _taxCategoryService = taxCategoryService;
            _productAttributeParser = productAttributeParser;
            _importUtilities = importUtilities;
            _productService = productService;
            _productAttributeService = productAttributeService;
        }

        public string GetWarrantySkuByName(string name)
        {
            return _warrantySkuRepository.Table
                .Where(ws => ws.Name == name)
                .Select(ws => ws.Sku).FirstOrDefault();
        }
    }
}

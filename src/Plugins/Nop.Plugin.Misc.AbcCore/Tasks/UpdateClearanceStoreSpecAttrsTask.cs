// Calls clearance stock backend to determine which clearance items are
// associated to which stores for PLP filter
using Nop.Plugin.Misc.AbcCore;
using Nop.Services.Tasks;
using System.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Services;
using System.Data.Odbc;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Core;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Tasks
{
    class UpdateClearanceStoreSpecAttrsTask : IScheduleTask
    {
        private readonly IAbcCategoryService _categoryService;
        private readonly IBackendStockService _backendStockService;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        public UpdateClearanceStoreSpecAttrsTask(
            IAbcCategoryService categoryService,
            IBackendStockService backendStockService,
            ISpecificationAttributeService specificationAttributeService)
        {
            _categoryService = categoryService;
            _backendStockService = backendStockService;
            _specificationAttributeService = specificationAttributeService;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            var clearanceCategory = await _categoryService.GetCategoryByNameAsync("Clearance");
            if (clearanceCategory == null)
            {
                throw new NopException("Cannot find the 'Clearance' category.");
            }

            var storeSpecAttrName = "Shop by Store";
            var storeSpecAttr = (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                                                                         .FirstOrDefault(sa => sa.Name == storeSpecAttrName);
            if (storeSpecAttr == null)
            {
                throw new NopException($"Cannot find the '{storeSpecAttrName}' specification attribute with options.");
            }

            var storeSpecAttrOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(storeSpecAttr.Id);
            var storeSpecAttrOptionIds = storeSpecAttrOptions.Select(pso => pso.Id);

            var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(clearanceCategory.Id);
            foreach (var pc in productCategories)
            {
                var productId = pc.ProductId;
                var productSpecificationAttributes = (await _specificationAttributeService.GetProductSpecificationAttributesAsync(productId))
                                                                                          .Where(psa => storeSpecAttrOptionIds.Contains(psa.SpecificationAttributeOptionId));
                var currentStoreNames = await productSpecificationAttributes.SelectAwait(async psa => (await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId)).Name)
                                                                            .ToListAsync();

                var backendStockResponse = await _backendStockService.GetApiStockAsync(productId);
                if (backendStockResponse == null) { continue; }
                var availableStores = backendStockResponse.ProductStocks
                                                          .Where(ps => ps.Available)
                                                          .Select(ps => ps.Shop.Name);
                
                var toInsert = availableStores.Except(currentStoreNames);
                foreach (var store in toInsert)
                {
                    var specAttrOption = storeSpecAttrOptions.First(sao => sao.Name == store);
                    var psa = new ProductSpecificationAttribute()
                    {
                        ProductId = productId,
                        AttributeType = SpecificationAttributeType.Option,
                        SpecificationAttributeOptionId = specAttrOption.Id,
                        AllowFiltering = true
                    };
                    await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psa);
                }

                var toDelete = currentStoreNames.Except(availableStores);
                foreach (var store in toDelete)
                {
                    var psa = await productSpecificationAttributes.FirstOrDefaultAwaitAsync(async psa => (await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId)).Name == store);
                    await _specificationAttributeService.DeleteProductSpecificationAttributeAsync(psa);
                }
            }
        }
    }
}

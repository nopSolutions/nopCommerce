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

            var storeSpecAttr = (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                                                                         .FirstOrDefault(sa => sa.Name == "Store");
            if (storeSpecAttr == null)
            {
                throw new NopException("Cannot find the 'Store' specification attribute with options.");
            }

            var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(clearanceCategory.Id);

            foreach (var pc in productCategories)
            {
                var productId = pc.ProductId;
                var backendStockResponse = await _backendStockService.GetApiStockAsync(productId);
                if (backendStockResponse == null) { continue; }

                var availableStores = backendStockResponse.ProductStocks.Where(ps => ps.Available);
                foreach (var store in availableStores)
                {
                    var a = 1;
                }
            }
        }
    }
}

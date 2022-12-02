// This task adds ABc Item Number to product tags for Mickey Shorr and the InstantSearch
// plugin.

using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using Nop.Services.Catalog;
using Nop.Services.Tasks;


namespace Nop.Plugin.Misc.AbcCore
{
    class UpdateProductTagsTask : IScheduleTask
    {
        private readonly IAbcProductService _abcProductService;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IProductTagService _productTagService;

        public UpdateProductTagsTask(
            IAbcProductService abcProductService,
            IProductAbcDescriptionService productAbcDescriptionService,
            IProductTagService productTagService)
        {
            _abcProductService = abcProductService;
            _productAbcDescriptionService = productAbcDescriptionService;
            _productTagService = productTagService;
        }
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            var products = await _abcProductService.GetAllPublishedProductsAsync();

            foreach (var product in products)
            {
                var productAbcDescription = await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(product.Id);
                if (productAbcDescription != null)
                {
                    await _productTagService.UpdateProductTagsAsync(product, new string[] { productAbcDescription.AbcItemNumber });
                }
            }
        }
    }
}

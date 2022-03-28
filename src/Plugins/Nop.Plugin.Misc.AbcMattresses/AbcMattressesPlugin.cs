using Nop.Services.Plugins;
using Nop.Services.Common;
using Nop.Services.Tasks;
using Nop.Plugin.Misc.AbcMattresses.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nop.Services.Localization;
using Nop.Core;

namespace Nop.Plugin.Misc.AbcMattresses
{
    public class AbcMattressesPlugin : BasePlugin, IMiscPlugin
    {
        private readonly string TaskType =
            $"{typeof(UpdateMattressesTask).Namespace}.{typeof(UpdateMattressesTask).Name}, " +
            $"{typeof(UpdateMattressesTask).Assembly.GetName().Name}";

        private readonly IAbcMattressModelService _abcMattressModelService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

        public AbcMattressesPlugin(
            IAbcMattressModelService abcMattressModelService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IScheduleTaskService scheduleTaskService,
            INopDataProvider nopDataProvider,
            ILocalizationService localizationService,
            IWebHelper webHelper
        )
        {
            _abcMattressModelService = abcMattressModelService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _scheduleTaskService = scheduleTaskService;
            _nopDataProvider = nopDataProvider;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        public override async System.Threading.Tasks.Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await AddProductAttributesAsync();
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await RemoveTasksAsync();
            await AddTaskAsync();

            await AddProductAttributesAsync();

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await RemoveTasksAsync();
            await RemoveProductAttributesAsync();

            var productIdsToDelete = _abcMattressModelService.GetAllAbcMattressModels()
                                                           .Where(m => m.ProductId != null)
                                                           .Select(m => m.ProductId.Value)
                                                           .ToArray();
            var productsToDelete = (await _productService.GetProductsByIdsAsync(productIdsToDelete))
                                        .Where(p => !p.Deleted)
                                        .ToList();

            foreach (var product in productsToDelete)
            {
                // create a random string for the Sku to satisfy the index
                product.Sku = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 24)
                                .Select(s => s[new Random().Next(s.Length)]).ToArray());
                await _productService.UpdateProductAsync(product);
            }
            await _productService.DeleteProductsAsync(productsToDelete);

            await base.UninstallAsync();
        }
        private async System.Threading.Tasks.Task AddTaskAsync()
        {
            ScheduleTask task = new ScheduleTask();
            task.Name = $"Update Mattresses";
            task.Seconds = 14400;
            task.Type = TaskType;
            task.Enabled = false;
            task.StopOnError = false;

            await _scheduleTaskService.InsertTaskAsync(task);
        }

        private async System.Threading.Tasks.Task RemoveTasksAsync()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }
        }

        private async System.Threading.Tasks.Task RemoveProductAttributesAsync()
        {
            var productAttributes = (await _productAttributeService.GetAllProductAttributesAsync())
                                                            .Where(pa => pa.Name == AbcMattressesConsts.MattressSizeName ||
                                                                         AbcMattressesConsts.IsBase(pa.Name) ||
                                                                         AbcMattressesConsts.IsMattressProtector(pa.Name) ||
                                                                         AbcMattressesConsts.IsFrame(pa.Name) ||
                                                                         pa.Name == AbcMattressesConsts.FreeGiftName)
                                                            .ToList();

            await _productAttributeService.DeleteProductAttributesAsync(productAttributes);
        }

        private async System.Threading.Tasks.Task AddProductAttributesAsync()
        {
            ProductAttribute[] productAttributes =
            {
                new ProductAttribute() { Name = AbcMattressesConsts.MattressSizeName },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameTwin },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameTwinXL },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameFull },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameQueen },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameQueenFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameKing },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameKingFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameCaliforniaKing },
                new ProductAttribute() { Name = AbcMattressesConsts.BaseNameCaliforniaKingFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorTwin },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorTwinXL },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorFull },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorQueen },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorQueenFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorKing },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorKingFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorCaliforniaKing },
                new ProductAttribute() { Name = AbcMattressesConsts.MattressProtectorCaliforniaKingFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameTwin },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameTwinXL },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameFull },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameQueen },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameQueenFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameKing },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameKingFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameCaliforniaKing },
                new ProductAttribute() { Name = AbcMattressesConsts.FrameCaliforniaKingFlexhead },
                new ProductAttribute() { Name = AbcMattressesConsts.FreeGiftName }
            };

            foreach (var productAttribute in productAttributes)
            {
                var existingProductAttribute = (await _productAttributeService.GetAllProductAttributesAsync())
                                                                       .Where(pa => pa.Name == productAttribute.Name)
                                                                       .FirstOrDefault();

                if (existingProductAttribute == null)
                {
                    await _productAttributeService.InsertProductAttributeAsync(productAttribute);
                }
            }
        }
    }
}
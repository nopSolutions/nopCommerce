using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Services.Tasks;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks
{
    public partial class UpdateDeliveryOptionsTask : IScheduleTask
    {
        private async System.Threading.Tasks.Task AddHaulAwayAsync(
            int productId,
            AbcDeliveryMap map,
            int deliveryOptionsPamId,
            int? deliveryPavId,
            int? deliveryInstallPavId,
            decimal deliveryPriceAdjustment,
            decimal deliveryInstallPriceAdjustment)
        {
            if (deliveryPavId.HasValue)
            {
                await AddHaulAwayAttributeAsync(
                    productId,
                    _haulAwayDeliveryProductAttribute.Id,
                    deliveryOptionsPamId,
                    deliveryPavId.Value,
                    map.DeliveryHaulway,
                    deliveryPriceAdjustment);
            }

            if (deliveryInstallPavId.HasValue)
            {
                await AddHaulAwayAttributeAsync(
                    productId,
                    _haulAwayDeliveryInstallProductAttribute.Id,
                    deliveryOptionsPamId,
                    deliveryInstallPavId.Value,
                    map.DeliveryHaulwayInstall,
                    deliveryInstallPriceAdjustment);
            }
        }

        private async System.Threading.Tasks.Task<ProductAttributeMapping> AddHaulAwayAttributeMappingAsync(
            int productId,
            int productAttributeId,
            int deliveryOptionsPamId,
            int? pavId)
        {
            if (pavId == null)
            {
                return null;
            }

            var pam = (await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId))
                                                    .SingleOrDefault(pam => pam.ProductAttributeId == productAttributeId);
            if (pam == null)
            {
                pam = new ProductAttributeMapping()
                {
                    ProductId = productId,
                    ProductAttributeId = productAttributeId,
                    AttributeControlType = AttributeControlType.Checkboxes,
                    TextPrompt = "Haul Away",
                    ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{deliveryOptionsPamId}\"><ProductAttributeValue><Value>{pavId}</Value></ProductAttributeValue></ProductAttribute></Attributes>",
                };
                await _abcProductAttributeService.InsertProductAttributeMappingAsync(pam);
            }

            return pam;
        }

        private async System.Threading.Tasks.Task AddHaulAwayAttributeAsync(
            int productId,
            int productAttributeId,
            int deliveryOptionsPamId,
            int deliveryOptionsPavId,
            int abcDeliveryMapItemNumber,
            decimal priceAdjustment)
        {
            var pam = await AddHaulAwayAttributeMappingAsync(
                productId,
                productAttributeId,
                deliveryOptionsPamId,
                deliveryOptionsPavId);
            if (pam == null)
            {
                return;
            }

            var pav = (await _abcProductAttributeService.GetProductAttributeValuesAsync(pam.Id)).FirstOrDefault();
            _abcDeliveryService.AddValue(
                pam.Id,
                pav,
                abcDeliveryMapItemNumber,
                "Remove Old Appliance ({0})",
                0,
                false,
                priceAdjustment);
        }
    }
}

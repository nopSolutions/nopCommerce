using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Orders;
using System;
using Nop.Web.Models.Catalog;
using Nop.Plugin.Misc.AbcCore.Delivery;

namespace Nop.Plugin.Misc.AbcFrontend.Extensions
{
    public static class ProductAttributeModelExtensions
    {
        // This makes sure the attribute doesn't show on PDP
        // before loading all combinations
        public static bool IsHiddenPreload(this ProductDetailsModel.ProductAttributeModel model)
        {
            return model.TextPrompt == "Box Spring or Adjustable Base" ||
                   model.TextPrompt == "Mattress Protector" ||
                   model.TextPrompt == "Frame" ||
                   model.Name == AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName ||
                   model.Name == AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName ||
                   model.Name == AbcDeliveryConsts.DeliveryAccessoriesProductAttributeName ||
                   model.Name == AbcDeliveryConsts.DeliveryInstallAccessoriesProductAttributeName ||
                   model.Name == AbcDeliveryConsts.PickupAccessoriesProductAttributeName;
        }
    }
}

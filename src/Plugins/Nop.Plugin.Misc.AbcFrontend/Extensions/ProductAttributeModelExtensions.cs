using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Orders;
using System;
using Nop.Web.Models.Catalog;

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
                   model.Name == "Delivery/Pickup Options" ||
                   model.Name == "Haul Away (Delivery)" ||
                   model.Name == "Haul Away (Delivery/Install)";
        }
    }
}

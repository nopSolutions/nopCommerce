using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace AO.Services.Domain
{
    public partial class AOProductExtensionData : BaseEntity
    {
        public int ProductId { get; set; }
        
        public int StatusId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.ProductExtension.ForceOutOfGoogleShopping")]
        public bool ForceOutOfGoogleShopping { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.ProductExtension.ForceOutOfPriceRunner")]
        public bool ForceOutOfPriceRunner { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.ProductExtension.InventoryCountLastDone")]
        public DateTime InventoryCountLastDone { get; set; }
    }
}

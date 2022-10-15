using Nop.Web.Framework.Models;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Models
{
    public record PickStoreModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public Shop SelectedShop { get; set; }
        public string PickupInStoreText { get; set; }
        public string GoogleMapsAPIKey { get; set; }
        public bool IsPickupOnlyMode { get; set; }
    }
}

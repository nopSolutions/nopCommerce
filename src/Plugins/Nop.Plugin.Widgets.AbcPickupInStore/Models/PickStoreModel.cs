using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Models
{
    public record PickStoreModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public Shop SelectedShop { get; set; }
        public string PickupInStoreText { get; set; }
        public string GoogleMapsAPIKey { get; set; }
    }
}

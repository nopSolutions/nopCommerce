using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public interface IAbcDeliveryService
    {
        Task<AbcDeliveryItem> GetAbcDeliveryItemByItemNumberAsync(int itemNumber);

        Task<IList<AbcDeliveryMap>> GetAbcDeliveryMapsAsync();

        // Changed to synchronous to prevent collision issues
        ProductAttributeValue AddValue(
            int pamId,
            ProductAttributeValue pav,
            int itemNumber,
            string displayName,
            int displayOrder,
            bool isPreSelected,
            decimal priceAdjustment = 0);
    }
}
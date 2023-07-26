using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public interface IAbcDeliveryService
    {
        Task<AbcDeliveryItem> GetAbcDeliveryItemByItemNumberAsync(string itemNumber);

        Task<AbcDeliveryItem> GetAbcDeliveryItemByDescriptionAsync(string description);

        Task<AbcDeliveryItem> GetAbcDeliveryItemByIdAsync(int id);

        Task<IList<AbcDeliveryMap>> GetAbcDeliveryMapsAsync();

        Task<IList<AbcDeliveryAccessory>> GetAbcDeliveryAccessoriesByCategoryId(int categoryId);
    }
}
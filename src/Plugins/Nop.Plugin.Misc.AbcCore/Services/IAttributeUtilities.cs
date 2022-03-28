using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Models;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IAttributeUtilities
    {
        Task<string> InsertHomeDeliveryAttributeAsync(Product product, string attributes);
        Task<string> InsertPickupAttributeAsync(Product product, StockResponse stockResponse, string attributes, Shop currentCsm = null);
        Task<ProductAttributeMapping> GetPickupAttributeMappingAsync(string attributesXml);
        Task<ProductAttributeMapping> GetHomeDeliveryAttributeMappingAsync(string attributesXml);
        Task<ProductAttributeMapping> GetWarrantyAttributeMappingAsync(string attributesXml);
        Task<string> RemovePickupAttributesAsync(string attributes);
        Task<ProductAttributeMapping> GetAttributeMappingByNameAsync(string attributesXml, string name);
    }
}
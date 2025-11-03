using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.ProductWarehouseIventories;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductWarehouseInventoryDtoMappings
    {
        public static ProductWarehouseInventoryDto ToDto(this ProductWarehouseInventory mapping)
        {
            return mapping.MapTo<ProductWarehouseInventory, ProductWarehouseInventoryDto>();
        }
    }
}

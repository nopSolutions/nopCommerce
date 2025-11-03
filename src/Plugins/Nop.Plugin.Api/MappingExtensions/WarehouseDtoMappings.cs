using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.Warehouses;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class WarehouseDtoMappings
    {
        public static WarehouseDto ToDto(this Warehouse warehouse)
        {
            return warehouse.MapTo<Warehouse, WarehouseDto>();
        }
    }
}

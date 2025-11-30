using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Api.Factories
{
    public class WarehouseFactory : IFactory<Warehouse>
    {
        public Task<Warehouse> InitializeAsync()
        {
            var warehouse = new Warehouse();
            
            return Task.FromResult(warehouse);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping;
public interface IShippingAdminService
{
    Task InsertShippingMethodAsync(ShippingMethod method);
    Task UpdateShippingMethodAsync(ShippingMethod method);
    Task DeleteShippingMethodAsync(ShippingMethod method);
    Task<IList<ShippingMethod>> GetAllShippingMethodsAsync();

    Task InsertWarehouseAsync(Warehouse warehouse);
    Task<IList<Warehouse>> GetAllWarehousesAsync();
    // aur bhi jo admin CRUD methods hain...
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Nop.Core.Domain.Shipping;
//using Nop.Data;

//namespace Nop.Services.Shipping;
//public class ShippingAdminService : IShippingAdminService
//{
//    private readonly IRepository<ShippingMethod> _shippingMethodRepository;
//    private readonly IRepository<Warehouse> _warehouseRepository;

//    public ShippingAdminService(IRepository<ShippingMethod> shippingMethodRepository,
//                                IRepository<Warehouse> warehouseRepository)
//    {
//        _shippingMethodRepository = shippingMethodRepository;
//        _warehouseRepository = warehouseRepository;
//    }

//    public async Task InsertShippingMethodAsync(ShippingMethod method)
//    {
//        await _shippingMethodRepository.InsertAsync(method);
//    }

//    //public async Task<IList<ShippingMethod>> GetAllShippingMethodsAsync()
//    //{
//    //    return await _shippingMethodRepository.GetAllAsync();
//    //}

//    public async Task InsertWarehouseAsync(Warehouse warehouse)
//    {
//        await _warehouseRepository.InsertAsync(warehouse);
//    }

//    // Baaki methods similar
//}

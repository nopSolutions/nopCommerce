using Nop.Core;
using Nop.Plugin.Shipping.ByWeight.Domain;

namespace Nop.Plugin.Shipping.ByWeight.Services
{
    public partial interface IShippingByWeightService
    {
        void DeleteShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord);

        IPagedList<ShippingByWeightRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);

        ShippingByWeightRecord FindRecord(int shippingMethodId,
            int storeId, int warehouseId, 
            int countryId, int stateProvinceId, string zip, decimal weight);

        ShippingByWeightRecord GetById(int shippingByWeightRecordId);

        void InsertShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord);

        void UpdateShippingByWeightRecord(ShippingByWeightRecord shippingByWeightRecord);
    }
}

using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface ICustomerShopService
    {
        CustomerShopMapping GetCurrentCustomerShopMapping(int customerId);
        void InsertOrUpdateCustomerShop(int customerId, int shopId);
    }
}
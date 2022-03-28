using Nop.Plugin.Misc.AbcCore.Domain;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface ICustomShopService : IShopService
    {
        Shop GetShopByAbcBranchId(string branchId);

        Shop GetShopByName(string name);

        ShopAbc GetShopAbcByShopId(int shopId);
    }
}
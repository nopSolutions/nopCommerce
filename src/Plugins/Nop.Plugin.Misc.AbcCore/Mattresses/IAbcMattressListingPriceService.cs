using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressListingPriceService
    {
        Task<decimal?> GetListingPriceForMattressProductAsync(int productId);
    }
}

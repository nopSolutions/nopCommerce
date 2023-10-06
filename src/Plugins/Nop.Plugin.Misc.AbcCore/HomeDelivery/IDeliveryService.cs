using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public interface IDeliveryService
    {
        Task<bool> CheckZipcodeAsync(string zip);
    }
}

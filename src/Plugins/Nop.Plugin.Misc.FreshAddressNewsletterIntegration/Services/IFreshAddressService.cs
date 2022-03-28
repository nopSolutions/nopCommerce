using Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Models;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Services
{
    public interface IFreshAddressService
    {
        Task<FreshAddressResponse> ValidateEmailAsync(string email);
    }
}

using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Services.Payments;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface ICardCheckService
    {
        Task<(string AuthNo, string RefNo, string StatusCode, string ResponseMessage)> CheckCardAsync(
            ProcessPaymentRequest paymentRequest,
            Address billingAddress,
            string domain,
            string ip
        );
    }
}
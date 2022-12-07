using Iyzipay.Model;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Iyzico.Services
{
    public interface IPaymentIyzicoService
    {
        Task<Buyer> GetBuyer(int customerId);
        Task<Address> GetAddress(Core.Domain.Common.Address billingAddress);
    }
}
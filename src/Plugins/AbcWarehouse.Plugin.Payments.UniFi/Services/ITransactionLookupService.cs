using System.Threading.Tasks;
using AbcWarehouse.Plugin.Payments.UniFi.Models;

namespace AbcWarehouse.Plugin.Payments.UniFi.Services
{
    public interface ITransactionLookupService
    {
        Task<TransactionLookupResponse> TransactionLookupAsync(string transactionToken);
    }
}
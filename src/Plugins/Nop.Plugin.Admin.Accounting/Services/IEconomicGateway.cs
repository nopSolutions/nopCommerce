using AO.Services.Domain;
using Nop.Plugin.Admin.Accounting.Models.EconomicModels;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting.Services
{
    public interface IEconomicGateway
    {
        Task<bool> HasBeenBookedAsync(AOInvoice invoice);

        Task<int> BookInvoiceAsync(InvoiceBookModel invoiceModel);

        Task<bool> CreateCustomerAsync(CustomerInfo customerInfo);

        Task<bool> CustomerExistAsync(int customerId);
    }
}
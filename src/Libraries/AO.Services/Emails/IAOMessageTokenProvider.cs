using AO.Services.Domain;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Emails
{
    public interface IAOMessageTokenProvider
    {
        Task<string> ProductListToHtmlTableAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems, int languageId);
    }
}
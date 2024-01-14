using AO.Services.Domain;
using Nop.Core.Domain.Orders;
using Nop.Services.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Emails
{
    public interface IMessageService : IWorkflowMessageService
    {
        Task<IList<int>> SendAdminEmail(string email, string subject, string body);

        Task QueueAdminEmailAsync(string emailAddress, string subject, string body);

        Task QueueInvoiceEmailAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems);

        Task QueueCreditNoteEmailAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems);

        Task QueueInvoiceEmailAlternativeAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems, string alternativeEmailAddress);

        Task QueueCreditNoteEmailAlternativeAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems, string alternativeEmailAddress);
    }
}

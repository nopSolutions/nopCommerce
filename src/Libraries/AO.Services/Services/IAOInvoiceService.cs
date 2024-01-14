using AO.Services.Domain;
using AO.Services.Models;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IAOInvoiceService
    {
        Task<AOInvoice> CreateInvoiceAsync(List<OrderItem> orderItems, Order order, bool creditNote, bool invoiceShipping, DateTime invoiceDate, DateTime paymentDate, bool isPaid, bool isManual, bool reStock = false);

        Task<AOInvoice> CreateManualInvoiceAsync(string orderItemText, decimal total, string currencyCode, decimal taxRate, int customerId, DateTime invoiceDate, DateTime paymentDate);

        Task<AOInvoice> GetInvoiceByIdAsync(int Id);

        Task<List<AOInvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId);

        Task<List<AOInvoiceItem>> GetInvoiceItemsByInvoiceItemIdAsync(int orderItemId, int invoiceId, bool credited = false);

        Task<List<AOInvoice>> GetInvoiceByOrderIdAsync(int orderId);

        Task<int> GetInvoiceItemCountOnInvoiceAsync(int invoiceItemId, int invoiceId);

        Task<AOInvoiceItem> GetInvoiceItemByOrderItemIdAsync(int orderItemId, int invoiceId);

        Task<AOInvoiceListModel> GetTopInvoicesAsync(string searchphrase, int topCount);

        Task<InvoiceModel> GetInvoiceModelByIdAsync(int invoiceNumber);

        Task<List<AOInvoiceItem>> GetInvoiceItemsByOrderIdAsync(int orderId);

        Task<List<AOInvoice>> GetInvoicesNotBookedAsync();

        Task UpdateInvoiceAsync(AOInvoice invoice);
    }
}
using AO.Services.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using System;
using System.Collections.Generic;

namespace AO.Services.Models
{
    public class InvoiceModel
    {
        public int InvoiceNumber { get; set; }

        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal InvoiceTax { get; set; }

        public decimal InvoiceDiscount { get; set; }

        public decimal InvoiceTotal { get; set; }

        public decimal InvoiceRefundedAmount { get; set; }

        public decimal InvoiceShipping { get; set; }

        public decimal CurrencyRate { get; set; }

        public string TrackingNumber { get; set; }

        public string CustomerCurrencyCode { get; set; }

        public string PaymentMethod { get; set; }

        public string CustomerPresentationName { get; set; }

        public Customer InvoiceCustomer { get; set; }

        public Address InvoiceAddress { get; set; }

        public Country InvoiceCountry { get; set; }

        public IList<OrderItemModel> InvoiceItems { get; set; }

        public bool InvoiceIsPaid { get; set; }

        public bool InvoiceIsManual { get; set; }

        public DateTime? BookedDate { get; set; }

        public int? EconomicInvoiceNumber { get; set; }
    }
}
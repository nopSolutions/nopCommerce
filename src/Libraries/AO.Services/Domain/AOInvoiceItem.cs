using Nop.Core;

namespace AO.Services.Domain
{
    public class AOInvoiceItem : BaseEntity
    {
        public int InvoiceId { get; set; }

        public int OrderItemId { get; set; }

        public bool Credited { get; set; }

        public int Quantity { get; set; }

        public string EAN { get; set; }
    }
}
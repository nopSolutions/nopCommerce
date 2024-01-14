namespace AO.Services.Models
{
    public class OrderItemModel
    {
        public int InvoiceId { get; set; }

        public int OrderItemId { get; set; }

        public bool Credited { get; set; }

        public int Quantity { get; set; }

        public string EAN { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string VariantInfo { get; set; }
    }
}

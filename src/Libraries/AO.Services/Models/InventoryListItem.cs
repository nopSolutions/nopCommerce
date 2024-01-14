namespace AO.Services.Models
{
    public class InventoryListItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal CostPrice { get; set; }

        public int StockQuantity { get; set; }

        public decimal TotalCostPrice { get; set; }

        public string PositionName { get; set; }

        public string ShelfName { get; set; }

        public string RackName { get; set; }
    }
}
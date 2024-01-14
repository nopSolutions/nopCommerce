using Nop.Core.Domain.Orders;
using System.Collections.Generic;

namespace AO.Services.Services.Models
{
    public class ManageStockResultModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string EAN { get; set; }

        public int StockQuantity { get; set; }

        public string ColorString { get; set; }

        public string SizeString { get; set; }

        public string StockPosition { get; set; }

        public IList<Order> SoldOrdersWithEAN { get; set; }
    }
}
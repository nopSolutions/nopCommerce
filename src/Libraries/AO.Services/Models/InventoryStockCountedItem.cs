using System;

namespace AO.Services.Models
{
    public class InventoryStockCountedItem
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }       
        public string Published { get; set; }
        public string AOProductStatus { get; set; }     
        public DateTime StatusCountDoneTime { get; set; }
    }
}

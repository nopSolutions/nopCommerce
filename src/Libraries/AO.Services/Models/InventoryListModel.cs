using System;
using System.Collections.Generic;

namespace AO.Services.Models
{
    public class InventoryListModel
    {
        public IList<InventoryListItem> InventoryListItems { get; set; }

        public string ListTitle { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalCostPrice { get; set; }

        public DateTime InventoryListDatetime { get; set; }

        public string WarningMessage { get; set; }
    }
}

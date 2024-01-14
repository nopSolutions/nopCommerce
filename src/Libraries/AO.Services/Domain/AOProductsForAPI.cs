using Nop.Core;
using System;

namespace AO.Services.Domain
{
    public class AOProductForAPI : BaseEntity
    {
        public int ProductId
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public string ProductSKU
        {
            get;
            set;
        }

        public decimal CostPrice
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public decimal OldPrice
        {
            get;
            set;
        }

        public string ShippingCost
        {
            get;
            set;
        }

        public string ProductUrl
        {
            get;
            set;
        }

        public string ManufacturerSKU
        {
            get;
            set;
        }

        public string ManufacturerName
        {
            get;
            set;
        }

        public string EAN
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ImageURL
        {
            get;
            set;
        }

        public int InventoryCount
        {
            get;
            set;
        }

        public bool AllowOutOfStockOrders
        {
            get;
            set;
        }

        public string DeliveryTime
        {
            get;
            set;
        }

        public string Size
        {
            get; set;
        }

        public string Color
        {
            get; set;
        }

        public int LanguageId
        {
            get;
            set;
        }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    // https://developers.google.com/analytics/devguides/collection/ga4/reference/events?client_type=gtag#purchase_item
    public class GA4OrderItem
    {
        public string Sku { get; init; }
        public string Name { get; init; }
        public string Brand { get; init; }
        public string Category { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
    }
}

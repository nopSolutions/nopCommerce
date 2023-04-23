using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    // https://developers.google.com/analytics/devguides/collection/ga4/reference/events?client_type=gtag#purchase
    public class PurchaseModel
    {
        public string OrderId { get; init; }

        public decimal Value { get; init; }

        public decimal Tax { get; init; }

        public decimal Shipping { get; init; }

        public IList<GA4OrderItem> OrderItems { get; init; }
    }
}

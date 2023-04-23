using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    // https://developers.google.com/analytics/devguides/collection/ga4/ecommerce?client_type=gtag#initiate_the_checkout_process
    public class BeginCheckoutModel
    {
        public decimal Value { get; init; }
        public IList<GA4OrderItem> Items { get; init; }
    }
}

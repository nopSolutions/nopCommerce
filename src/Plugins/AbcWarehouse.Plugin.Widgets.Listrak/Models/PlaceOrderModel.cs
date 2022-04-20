using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.Listrak.Models
{
    // https://help.listrak.com/en/articles/4820523-javascript-website-integration-installation#h_d52fe12e2d
    // look under Significant Events -> Placing an Order -> Variables
    public class PlaceOrderModel
    {
        public string CustomerEmail { get; init; }

        public string CustomerFirstName { get; init; }

        public string CustomerLastName { get; init; }

        public int OrderNumber { get; init; }

        public decimal ItemTotal { get; init; }

        public decimal ShippingTotal { get; init; }

        public decimal TaxTotal { get; init; }

        public decimal OrderTotal { get; init; }

        public IList<OrderItem> OrderItems { get; init; }
    }
}

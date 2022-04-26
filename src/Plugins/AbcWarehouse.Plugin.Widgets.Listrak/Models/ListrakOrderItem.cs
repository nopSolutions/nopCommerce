using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.Listrak.Models
{
    public class ListrakOrderItem
    {
        public string Sku { get; init; }

        public int Quantity { get; init; }

        public decimal Price { get; init; }
    }
}

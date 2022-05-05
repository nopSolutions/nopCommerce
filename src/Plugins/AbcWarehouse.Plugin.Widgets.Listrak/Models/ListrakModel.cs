using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.Listrak.Models
{
    public class ListrakModel
    {
        public IList<ListrakOrderItem> CartUpdateOrderItems { get; set; }

        public PlaceOrderModel PlaceOrderModel { get; set; }

        public string ProductBrowseSku { get; set; }

        public string MerchantId { get; set; }
    }
}

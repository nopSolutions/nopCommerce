using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Models
{
    public class UniFiModel
    {
        public string PartnerId { get; set; }

        public string FlowType { get; set; }

        public string Tags { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class ShipmentListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Orders.Shipments.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public bool DisplayPdfPackagingSlip { get; set; }
    }
}
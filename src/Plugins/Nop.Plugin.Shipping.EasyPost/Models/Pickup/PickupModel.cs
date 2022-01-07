using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.EasyPost.Models.Pickup
{
    /// <summary>
    /// Represents a pickup model
    /// </summary>
    public record PickupModel : BaseNopModel
    {
        #region Ctor

        public PickupModel()
        {
            PickupAddress = new AddressModel();
            AvailablePickupRates = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public string PickupId { get; set; }

        public int? ShipmentId { get; set; }

        public int? BatchId { get; set; }

        public bool Created { get; set; }

        public bool Purchased { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Pickup.Status")]
        public string Status { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Pickup.Instructions")]
        public string Instructions { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Pickup.MinDate")]
        public DateTime MinDate { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Pickup.MaxDate")]
        public DateTime MaxDate { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Pickup.Rate")]
        public string PickupRateId { get; set; }
        public IList<SelectListItem> AvailablePickupRates { get; set; }

        public AddressModel PickupAddress { get; set; }

        #endregion
    }
}
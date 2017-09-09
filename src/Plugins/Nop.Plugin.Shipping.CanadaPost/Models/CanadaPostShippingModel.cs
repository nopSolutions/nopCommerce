using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Shipping.CanadaPost.Models
{
    public class CanadaPostShippingModel : BaseNopModel
    {
        public CanadaPostShippingModel()
        {
            SelectedServicesCodes = new List<string>();
            AvailableServices = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.CustomerNumber")]
        public string CustomerNumber { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.ContractId")]
        public string ContractId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.Api")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.Services")]
        public IList<SelectListItem> AvailableServices { get; set; }
        public IList<string> SelectedServicesCodes { get; set; }
    }
}
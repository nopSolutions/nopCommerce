using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.EasyPost.Models.Configuration
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            CarrierAccounts = new List<string>();
            AvailableCarrierAccounts = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.ApiKey")]
        [DataType(DataType.Password)]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.TestApiKey")]
        [DataType(DataType.Password)]
        public string TestApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.UseAllAvailableCarriers")]
        public bool UseAllAvailableCarriers { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.CarrierAccounts")]
        public IList<SelectListItem> AvailableCarrierAccounts { get; set; }
        public IList<string> CarrierAccounts { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.AddressVerification")]
        public bool AddressVerification { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Configuration.Fields.StrictAddressVerification")]
        public bool StrictAddressVerification { get; set; }

        #endregion
    }
}
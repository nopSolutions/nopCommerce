using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Square.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            Locations = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.ApplicationId")]
        public string ApplicationId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.SandboxApplicationId")]
        public string SandboxApplicationId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.ApplicationSecret")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string ApplicationSecret { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.AccessToken")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string AccessToken { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.SandboxAccessToken")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string SandboxAccessToken { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.Use3ds")]
        public bool Use3ds { get; set; }
        public bool Use3ds_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.TransactionMode")]
        public int TransactionModeId { get; set; }
        public bool TransactionModeId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.Location")]
        public string LocationId { get; set; }
        public bool LocationId_OverrideForStore { get; set; }
        public IList<SelectListItem> Locations { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Square.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        #endregion
    }
}
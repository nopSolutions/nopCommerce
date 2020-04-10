using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            PaymentTypes = new List<SelectListItem>();
            ButtonsWidgetZones = new List<int>();
            AvailableButtonsWidgetZones = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.SecretKey")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.PaymentType")]
        public int PaymentTypeId { get; set; }
        public bool PaymentTypeId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentTypes { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.ButtonsWidgetZones")]
        public IList<int> ButtonsWidgetZones { get; set; }
        public bool ButtonsWidgetZones_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableButtonsWidgetZones { get; set; }

        #endregion
    }
}
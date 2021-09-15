using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Payments
{
    /// <summary>
    /// Represents a payment method model
    /// </summary>
    public partial record PaymentMethodModel : BaseNopModel, IPluginModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SystemName")]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Configure")]
        public string ConfigurationUrl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.Logo")]
        public string LogoUrl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportCapture")]
        public bool SupportCapture { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportPartiallyRefund")]
        public bool SupportPartiallyRefund { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportRefund")]
        public bool SupportRefund { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportVoid")]
        public bool SupportVoid { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.RecurringPaymentType")]
        public string RecurringPaymentType { get; set; }

        #endregion
    }
}
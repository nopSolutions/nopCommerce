using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.PeachPayments.Domains;
using Nop.Web.Areas.Admin.Models.Directory;

namespace Nop.Plugin.Payments.PeachPayments
{
    public class PeachPaymentsSettings:ISettings
    {
        public string WebhookUrl { get; set; }
        public bool UseSandbox { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public string Email { get; set; }
        public string MerchantGuid { get; set; }
        public string SignUpUrl { get; set; }
        public bool SetCredentialsManually { get; set; }

        public PaymentType PaymentType { get; set; }
        public bool DisplayButtonsOnShoppingCart { get; set; }

        public bool DisplayButtonsOnProductDetails { get; set; }
        public bool DisplayLogoInHeaderLinks { get; set; }
        public string LogoInHeaderLinks { get; set; }
        public bool DisplayLogoInFooter { get; set; }
        public bool DisplayPayLaterMessages { get; set; }
        public string LogoInFooter { get; set; }

        public string StyleLayout { get; set; }
        public string StyleColor { get; set; }
        public string StyleShape { get; set; }

        public string StyleLabel { get; set; }
        public int? RequestTimeout { get; set; }
        public decimal MinDiscountAmount { get; set; }

        public string CheckoutChannel { get; set; }
        public string SecretToken { get; set; }
        public string CheckoutChannelSandbox { get; set; }
        public string SecretTokenSandbox { get; set; }
        public string Callbackurl { get; set; }
        public string MerchantIdPrefix { get; set; }
        public string PeachPaymentsCheckoutDisplayText { get; set; }
        public string SortOrder { get; set; }
        public SandboxMode SandboxMode { get; set; }
        public int SandBoxModeId { get; set; }
        public int CurrencyId { get; set; }
        public string DisabledFunding { get; set; }
        public string EnabledFunding { get; set; }

    }
}

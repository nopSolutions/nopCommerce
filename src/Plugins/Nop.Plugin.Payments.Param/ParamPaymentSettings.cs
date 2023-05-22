using Nop.Core.Configuration;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Param
{
    public class IParamPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }
        public string ClientCode { get; set; }
        public string ClientUsername { get; set; }
        public string ClientPassword { get; set; }
        public string Guid { get; set; }
        public string TestUrl { get; set; }
        public string ProductUrl { get; set; }
        public bool Installment { get; set; }
        public int? InstallmentInt { get; set; }
        public ProcessPaymentRequest ProcessPaymentRequest { get; set; }

    }
}

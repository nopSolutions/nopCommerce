using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Payments.UniFi.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string TransactionToken { get; init; }
        public string TokenId { get; init; }
        public string PartnerId { get; init; }
        public string ClientTransactionId { get; init; }
        public string Address1 { get; init; }
        public string Address2 { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string Zip { get; init; }
        public string TransactionAmount { get; init; }
    }
}

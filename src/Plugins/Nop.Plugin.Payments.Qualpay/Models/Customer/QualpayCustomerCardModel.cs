using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Qualpay.Models.Customer
{
    /// <summary>
    /// Represents Qualpay customer card model
    /// </summary>
    public class QualpayCustomerCardModel : BaseNopModel
    {
        public string Id { get; set; }

        public string CardId { get; set; }

        public string MaskedNumber { get; set; }

        public string ExpirationDate { get; set; }

        public string CardType { get; set; }
    }
}
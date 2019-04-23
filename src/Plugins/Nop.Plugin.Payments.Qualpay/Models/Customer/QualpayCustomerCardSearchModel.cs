using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Qualpay.Models.Customer
{
    /// <summary>
    /// Represents Qualpay customer card search model
    /// </summary>
    public class QualpayCustomerCardSearchModel : BaseSearchModel
    {
        public int CustomerId { get; set; }
    }
}
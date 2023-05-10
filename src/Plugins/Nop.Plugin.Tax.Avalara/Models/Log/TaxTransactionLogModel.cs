using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.Log
{
    /// <summary>
    /// Represents a tax transaction log model
    /// </summary>
    public record TaxTransactionLogModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.StatusCode")]
        public int StatusCode { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.Url")]
        public string Url { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.RequestMessage")]
        public string RequestMessage { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.ResponseMessage")]
        public string ResponseMessage { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.Customer")]
        public int? CustomerId { get; set; }
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        #endregion
    }
}
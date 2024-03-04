using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    public class GA4Model
    {
        public string GoogleTag { get; set; }
        public bool IsDebugMode { get; set; }

        public GA4OrderItem ViewItemModel { get; set; }
        public GA4GeneralModel BeginCheckoutModel { get; set; }
        public PurchaseModel PurchaseModel { get; set; }
        public AddToCartModel AddToCartModel { get; set; }
        public GA4GeneralModel ViewCartModel { get; set; }
        public GA4GeneralModel AddShippingInfoModel { get; set; }
    }
}

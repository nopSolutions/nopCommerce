using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    public class GA4Model
    {
        public string GoogleTag { get; set; }
        public bool IsDebugMode { get; set; }

        public GA4OrderItem ViewItemModel { get; set; }
        public BeginCheckoutModel BeginCheckoutModel { get; set; }
        public PurchaseModel PurchaseModel { get; set; }
    }
}

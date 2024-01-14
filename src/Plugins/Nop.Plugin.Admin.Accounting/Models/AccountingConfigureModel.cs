using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Admin.Accounting.Models
{
    public class AccountingConfigureModel
    {
        [NopResourceDisplayName("Nop.Plugin.Admin.Accounting.ErrorMessage")]
        public string ErrorMessage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.Accounting.ReconciliationListDaysBack")]
        public int ReconciliationListDaysBack { get; set; }

        public string PhysicalShop_ProductNumber { get; set; }

        public string WebshopDK_ProductNumber { get; set; }

        public string WebshopSE_ProductNumber { get; set; }

        public string WebshopOther_ProductNumber { get; set; }

        public string WebshopOtherWithoutTax_ProductNumber { get; set; }        
    }
}

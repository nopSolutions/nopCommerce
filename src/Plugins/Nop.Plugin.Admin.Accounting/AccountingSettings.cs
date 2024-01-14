using Nop.Core.Configuration;

namespace Nop.Plugin.Admin.Accounting
{
    public class AccountingSettings : ISettings
    {
        /// <summary>
        /// How many days should the list go back in time
        /// </summary>
        public int ReconciliationListDaysBack { get; set; }

        public string PhysicalShop_ProductNumber { get; set; }

        public string WebshopDK_ProductNumber { get; set; }

        public string WebshopSE_ProductNumber { get; set; }

        public string WebshopOther_ProductNumber { get; set; }

        public string WebshopOtherWithoutTax_ProductNumber { get; set; }        
    }
}

using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class OnlineCustomerModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.CustomerInfo")]
        public string CustomerInfo { get; set; }

        [NopResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [NopResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.Location")]
        public string Location { get; set; }

        [NopResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }
        
        [NopResourceDisplayName("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }
    }
}
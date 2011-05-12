using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer
{
    public class CustomerOrderListModel : BaseNopModel
    {
        public CustomerOrderListModel()
        {
            Orders = new List<OrderDetailsModel>();
        }

        public IList<OrderDetailsModel> Orders { get; set; }
        public CustomerNavigationModel NavigationModel { get; set; }

        #region Nested classes
        public class OrderDetailsModel : BaseNopEntityModel
        {
            public string OrderTotal { get; set; }
            public bool IsReturnRequestAllowed { get; set; }
            public string OrderStatus { get; set; }
            public string CreatedOn { get; set; }
        }
        #endregion
    }
}
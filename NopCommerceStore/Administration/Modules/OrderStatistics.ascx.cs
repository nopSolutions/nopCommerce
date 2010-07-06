//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class OrderStatisticsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            //pending
            var orders_os_pending = OrderManager.GetOrderReport(OrderStatusEnum.Pending, null, null);
            lblTotalIncomplete.Text = orders_os_pending.Count.ToString();
            lblTotalIncompleteValue.Text = PriceHelper.FormatPrice(orders_os_pending.Total, true, false);

            //not paid
            var orders_ps_pending = OrderManager.GetOrderReport(null, PaymentStatusEnum.Pending, null);
            lblTotalUnpaid.Text = orders_ps_pending.Count.ToString();
            lblTotalUnpaidValue.Text = PriceHelper.FormatPrice(orders_ps_pending.Total, true, false);

            //not shipped
            var orders_ss_pending = OrderManager.GetOrderReport(null, null, ShippingStatusEnum.NotYetShipped);
            lblTotalUnshipped.Text = orders_ss_pending.Count.ToString();
            lblTotalUnshippedValue.Text = PriceHelper.FormatPrice(orders_ss_pending.Total, true, false);
        }
    }
}


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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;
namespace NopSolutions.NopCommerce.Web
{
    public partial class MoneybookersReturnPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            //comment this line to process return
            Response.Redirect(CommonHelper.GetStoreLocation());

            if (!Page.IsPostBack)
            {
                string pay_to_email = HttpContext.Current.Request.Form["pay_to_email"];
                string pay_from_email = HttpContext.Current.Request.Form["pay_from_email"];
                string merchant_id = HttpContext.Current.Request.Form["merchant_id"];
                string mb_transaction_id = HttpContext.Current.Request.Form["mb_transaction_id"];
                string mb_amount = HttpContext.Current.Request.Form["mb_amount"];
                string mb_currency = HttpContext.Current.Request.Form["mb_currency"];
                string status = HttpContext.Current.Request.Form["status"];
                string md5sig = HttpContext.Current.Request.Form["md5sig"];
                string amount = HttpContext.Current.Request.Form["amount"];
                string currency = HttpContext.Current.Request.Form["currency"];

                if (status == "2")
                {
                    Order order = OrderManager.GetOrderById(Convert.ToInt32(mb_transaction_id));
                    if (OrderManager.CanMarkOrderAsPaid(order))
                    {
                        OrderManager.MarkOrderAsPaid(order.OrderId);
                    }
                    Response.Redirect("~/checkoutcompleted.aspx");
                }
                else
                    Response.Redirect(CommonHelper.GetStoreLocation());

            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }
    }
}
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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerReturnRequestsControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            BindData();
        }

        protected void rptrRequests_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var returnRequest = (ReturnRequest)e.Item.DataItem;

                var lRequestTitle = (Literal)e.Item.FindControl("lRequestTitle");
                var lItem = (Literal)e.Item.FindControl("lItem");
                var lReason = (Literal)e.Item.FindControl("lReason");
                var lAction = (Literal)e.Item.FindControl("lAction");
                var lDate = (Literal)e.Item.FindControl("lDate");
                var phComments = (PlaceHolder)e.Item.FindControl("phComments");
                var lComments = (Literal)e.Item.FindControl("lComments");

                lRequestTitle.Text = string.Format(GetLocaleResourceString("CustomerReturnRequests.RequestTitle"), returnRequest.ReturnRequestId, OrderManager.GetReturnRequestStatusName(returnRequest.ReturnStatus));
                string prodLink = string.Format("<a href=\"{0}\">{1}</a>", SEOHelper.GetProductUrl(returnRequest.OrderProductVariant.ProductVariant.Product), returnRequest.OrderProductVariant.ProductVariant.LocalizedFullProductName);
                lItem.Text = string.Format(GetLocaleResourceString("CustomerReturnRequests.Item"), returnRequest.Quantity, prodLink);
                lReason.Text = string.Format(GetLocaleResourceString("CustomerReturnRequests.Reason"), Server.HtmlEncode(returnRequest.ReasonForReturn));
                lAction.Text = string.Format(GetLocaleResourceString("CustomerReturnRequests.Action"), Server.HtmlEncode(returnRequest.RequestedAction));
                lDate.Text = string.Format(GetLocaleResourceString("CustomerReturnRequests.Date"), DateTimeHelper.ConvertToUserTime(returnRequest.CreatedOn, DateTimeKind.Utc));
                if (!string.IsNullOrEmpty(returnRequest.CustomerComments))
                {
                    phComments.Visible = true;
                    lComments.Text = OrderManager.FormatReturnRequestCommentsText(returnRequest.CustomerComments);
                }
                else
                {
                    phComments.Visible = false;
                }
            }
        }

        private void BindData()
        {
            if (SettingManager.GetSettingValueBoolean("ReturnRequests.Enable"))
            {
                var returnRequests = OrderManager.SearchReturnRequests(NopContext.Current.User.CustomerId, 0, null);
                if (returnRequests.Count > 0)
                {
                    rptrRequests.DataSource = returnRequests;
                    rptrRequests.DataBind();
                }
                else
                {
                    this.Visible = false;
                }
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}
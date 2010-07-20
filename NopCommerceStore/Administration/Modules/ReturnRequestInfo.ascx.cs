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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ReturnRequestInfoControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlStatus.Items.Clear();
            string[] statuses = Enum.GetNames(typeof(ReturnStatusEnum));
            foreach (string status in statuses)
            {
                int enumValue = (int)Enum.Parse(typeof(ReturnStatusEnum), status, true);
                ListItem ddlItem = new ListItem(OrderManager.GetReturnRequestStatusName((ReturnStatusEnum)enumValue), enumValue.ToString());
                ddlStatus.Items.Add(ddlItem);
            }
        }

        private void BindData()
        {
            ReturnRequest rr = OrderManager.GetReturnRequestById(this.ReturnRequestId);
            if (rr != null)
            {
                this.lblReturnRequestId.Text = rr.ReturnRequestId.ToString();
                this.lblName.Text = string.Format("{0} x {1}", rr.Quantity, Server.HtmlEncode(rr.OrderProductVariant.ProductVariant.FullProductName));
                this.lblOrder.Text = string.Format("<a href=\"OrderDetails.aspx?OrderID={0}\">{1}</a>", rr.OrderProductVariant.OrderId, string.Format(GetLocaleResourceString("Admin.ReturnRequestInfo.Order.View"),rr.OrderProductVariant.OrderId));
                this.lblCustomer.Text = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", rr.CustomerId, string.Format(GetLocaleResourceString("Admin.ReturnRequestInfo.Customer.View"), Server.HtmlEncode(rr.Customer.Email)));
                CommonHelper.SelectListItem(this.ddlStatus, rr.ReturnStatusId);
                this.txtReasonForReturn.Text = rr.ReasonForReturn;
                this.txtRequestedAction.Text = rr.RequestedAction;
                this.txtCustomerComments.Text = rr.CustomerComments;
                this.txtStaffNotes.Text = rr.StaffNotes;

                this.lblDate.Text = DateTimeHelper.ConvertToUserTime(rr.CreatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                Response.Redirect("ReturnRequests.aspx");
            }
        }

        public ReturnRequest SaveInfo()
        {
            ReturnRequest rr = OrderManager.GetReturnRequestById(this.ReturnRequestId);

            if (rr != null)
            {
                rr = OrderManager.UpdateReturnRequest(rr.ReturnRequestId,
                    rr.OrderProductVariantId, rr.Quantity,
                    rr.CustomerId, txtReasonForReturn.Text,
                    txtRequestedAction.Text, txtCustomerComments.Text,
                    txtStaffNotes.Text, (ReturnStatusEnum)int.Parse(this.ddlStatus.SelectedItem.Value),
                    rr.CreatedOn, rr.UpdatedOn);
            }
            else
            {
                Response.Redirect("ReturnRequests.aspx");
            }

            return rr;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public int ReturnRequestId
        {
            get
            {
                return CommonHelper.QueryStringInt("ReturnRequestId");
            }
        }
    }
}
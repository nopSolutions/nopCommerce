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
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ReturnRequestsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        protected void BindGrid()
        {
            var returnRequests = OrderManager.SearchReturnRequests(0, 0, null);
            gvReturnRequests.DataSource = returnRequests;
            gvReturnRequests.DataBind();
        }

        protected void gvReturnRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReturnRequests.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        public string GetProductVariantUrl(OrderProductVariant opv)
        {
            string result = string.Empty;
            ProductVariant productVariant = opv.ProductVariant;
            if (productVariant != null)
                result = "ProductVariantDetails.aspx?ProductVariantID=" + productVariant.ProductVariantId.ToString();
            else
                result = "Not available. Product variant ID=" + opv.ProductVariantId.ToString();
            return result;
        }

        public string GetProductVariantName(OrderProductVariant opv)
        {
            ProductVariant productVariant = opv.ProductVariant;
            if (productVariant != null)
                return productVariant.FullProductName;
            return "Not available. ID=" + opv.ProductVariantId.ToString();
        }

        public string GetAttributeDescription(OrderProductVariant opv)
        {
            string result = opv.AttributeDescription;
            if (!String.IsNullOrEmpty(result))
                result = "<br />" + result;
            return result;
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.ReturnRequests.CustomerColumn.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }

        protected string GetOrderInfo(int orderId)
        {
            string orderInfo = string.Empty;
            Order order = OrderManager.GetOrderById(orderId);
            if (order != null)
            {
                orderInfo = string.Format("<a href=\"OrderDetails.aspx?OrderID={0}\">{1}</a>", order.OrderId, GetLocaleResourceString("Admin.ReturnRequests.OrderColumn.View"));
            }
            return orderInfo;
        }
        
        protected void gvReturnRequests_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var opv = (ReturnRequest)e.Row.DataItem;
                DropDownList ddlQuantity = e.Row.FindControl("ddlQuantity") as DropDownList;
                if (ddlQuantity != null)
                {
                    
                }
            }
        }
        
    }
}
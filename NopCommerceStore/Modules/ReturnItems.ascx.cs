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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ReturnItemsControl : BaseNopUserControl
    {
        #region Fields
        private Order order = null;
        #endregion

        #region Utilities

        protected void BindData()
        {
            lTitle.Text = string.Format(GetLocaleResourceString("ReturnItems.Title"), order.OrderId);
            phInput.Visible = true;
            phResult.Visible = false;

            //purchased products
            var orderProductVariants = order.OrderProductVariants;
            gvOrderProductVariants.DataSource = orderProductVariants;
            gvOrderProductVariants.DataBind();
        }
        
        protected void FillDropDowns()
        {
            this.ddlReturnReason.Items.Clear();
            string returnReasons = SettingManager.GetSettingValue("ReturnRequests.ReturnReasons");
            foreach (var returnReason in returnReasons.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ListItem item2 = new ListItem(returnReason.Trim(), returnReason.Trim());
                this.ddlReturnReason.Items.Add(item2);
            }

            this.ddlReturnAction.Items.Clear();
            string returnActions = SettingManager.GetSettingValue("ReturnRequests.ReturnActions");
            foreach (var returnAction in returnActions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ListItem item2 = new ListItem(returnAction.Trim(), returnAction.Trim());
                this.ddlReturnAction.Items.Add(item2);
            }
        }

        #endregion

        #region Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }
            order = OrderManager.GetOrderById(this.OrderId);
            if (order == null || order.Deleted || NopContext.Current.User.CustomerId != order.CustomerId)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if (!OrderManager.IsReturnRequestAllowed(order))
            {
                Response.Redirect(CommonHelper.GetStoreLocation());
            }

            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (GridViewRow row in gvOrderProductVariants.Rows)
            {
                HiddenField hfOpvId = row.FindControl("hfOpvId") as HiddenField;
                DropDownList ddlQuantity = row.FindControl("ddlQuantity") as DropDownList;
                if (hfOpvId != null && ddlQuantity != null)
                {
                    int quantity = Convert.ToInt32(ddlQuantity.SelectedValue);
                    if (quantity > 0)
                    {
                        int opvId = Convert.ToInt32(hfOpvId.Value);
                        var opv = OrderManager.GetOrderProductVariantById(opvId);
                        if (opv != null)
                        {
                            if (opv.Order.CustomerId == NopContext.Current.User.CustomerId)
                            {
                                DateTime dtNow = DateTime.UtcNow;
                                var rr = OrderManager.InsertReturnRequest(opv.OrderProductVariantId,
                                    quantity, NopContext.Current.User.CustomerId,
                                    ddlReturnReason.SelectedValue, ddlReturnAction.SelectedValue,
                                    txtComments.Text, string.Empty, ReturnStatusEnum.Pending,
                                    dtNow, dtNow);
                                count++; 
                            }
                        }
                    }
                }

            }

            phInput.Visible = false;
            phResult.Visible = true;
            if (count > 0)
            {
                lResults.Text = GetLocaleResourceString("ReturnItems.Submitted");
            }
            else
            {
                lResults.Text = GetLocaleResourceString("ReturnItems.NoItems");
            }
        }

        protected void gvOrderProductVariants_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var opv = (OrderProductVariant)e.Row.DataItem;
                DropDownList ddlQuantity = e.Row.FindControl("ddlQuantity") as DropDownList;
                if (ddlQuantity != null)
                {
                    ddlQuantity.Items.Clear();
                    for (int i = 0; i <= opv.Quantity; i++)
                    {
                        ListItem item = new ListItem(i.ToString(), i.ToString());
                        ddlQuantity.Items.Add(item);
                    }
                }
            }
        }
 
        #endregion

        #region Methods
        public string GetProductVariantName(int productVariantId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant != null)
                return productVariant.LocalizedFullProductName;
            return "Not available. ID=" + productVariantId.ToString();
        }
        
        public string GetAttributeDescription(OrderProductVariant opv)
        {
            string result = opv.AttributeDescription;
            if (!String.IsNullOrEmpty(result))
                result = "<br />" + result;
            return result;
        }

        public string GetProductUrl(int productVariantId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant != null)
                return SEOHelper.GetProductUrl(productVariant.ProductId);
            return string.Empty;
        }

        public string GetProductVariantUnitPrice(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    result = PriceHelper.FormatPrice(orderProductVariant.UnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, false);
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    result = PriceHelper.FormatPrice(orderProductVariant.UnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, true);
                    break;
            }

            return result;
        }

        #endregion

        #region Properties

        public int OrderId
        {
            get
            {
                return CommonHelper.QueryStringInt("OrderId");
            }
        }

        #endregion
    }
}
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
    public partial class CustomerDownloadableProductsControl : BaseNopUserControl
    {
        #region Utilities

        protected void BindData()
        {
            var items = OrderManager.GetAllOrderProductVariants(null, NopContext.Current.User.CustomerId, null, null, null, null, null, true);
            if (items.Count > 0)
            {
                pnlProducts.Visible = true;
                pnlMessage.Visible = false;

                gvOrderProductVariants.DataSource = items;
                gvOrderProductVariants.DataBind();
            }
            else
            {
                pnlProducts.Visible = false;
                pnlMessage.Visible = true;
            }
        }

        #endregion

        #region Handlers

        protected override void OnInit(EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            this.BindData();

            base.OnInit(e);
        }

        #endregion

        #region Methods

        public string GetOrderUrl(OrderProductVariant opv)
        {
            string result = string.Empty;
            Order order = opv.Order;
            if (order!=null)
            {
                result = string.Format("<a class=\"link\" href=\"{0}OrderDetails.aspx?OrderID={1}\" >{1}</a>", CommonHelper.GetStoreLocation(), order.OrderId);
            }
            return result.ToLowerInvariant();
        }

        public string GetOrderDate(OrderProductVariant opv)
        {
            string result = string.Empty;
            Order order = opv.Order;
            if (order != null)
            {
                result = DateTimeHelper.ConvertToUserTime(order.CreatedOn, DateTimeKind.Utc).ToString("d");
            }
            return result;
        }
        
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

        public string GetDownloadUrl(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            if (OrderManager.IsDownloadAllowed(orderProductVariant))
            {
                result = string.Format("<a class=\"link\" href=\"{0}\" >{1}</a>", DownloadManager.GetDownloadUrl(orderProductVariant), GetLocaleResourceString("Account.DownloadableProducts.Download"));
            }
            else
            {
                result = GetLocaleResourceString("Account.DownloadableProducts.ProductsGrid.Download.na");
            }
            return result;
        }

        public string GetLicenseDownloadUrl(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            if (OrderManager.IsLicenseDownloadAllowed(orderProductVariant))
            {
                result = string.Format("<a class=\"link\" href=\"{0}\" >{1}</a>", DownloadManager.GetLicenseDownloadUrl(orderProductVariant), GetLocaleResourceString("Account.DownloadableProducts.DownloadLicense"));
            }
            return result;
        }

        #endregion
    }
}
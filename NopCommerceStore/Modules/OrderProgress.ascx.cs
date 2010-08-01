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
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class OrderProgressControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetStatus();
                SetLinks();
            }
        }

        private void SetStatus()
        {
            hlCart.CssClass = GetStatusCss("Cart");
            hlAddress.CssClass = GetStatusCss("Address");
            hlShipping.CssClass = GetStatusCss("Shipping");
            hlPayment.CssClass = GetStatusCss("Payment");
            hlConfirm.CssClass = GetStatusCss("Confirm");
            hlComplete.CssClass = GetStatusCss("Complete");
        }

        private void SetLinks()
        {
            switch (this.OrderProgressStep.ToLowerInvariant())
            {
                case "cart":
                    {
                        hlCart.NavigateUrl = SEOHelper.GetShoppingCartUrl();
                        hlAddress.NavigateUrl = string.Empty;
                        hlShipping.NavigateUrl = string.Empty;
                        hlPayment.NavigateUrl = string.Empty;
                        hlConfirm.NavigateUrl = string.Empty;
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
                case "address":
                    {
                        hlCart.NavigateUrl = SEOHelper.GetShoppingCartUrl();
                        hlAddress.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingaddress.aspx";
                        hlShipping.NavigateUrl = string.Empty;
                        hlPayment.NavigateUrl = string.Empty;
                        hlConfirm.NavigateUrl = string.Empty;
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
                case "shipping":
                    {
                        hlCart.NavigateUrl = SEOHelper.GetShoppingCartUrl();
                        hlAddress.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingaddress.aspx";
                        hlShipping.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingmethod.aspx";
                        hlPayment.NavigateUrl = string.Empty;
                        hlConfirm.NavigateUrl = string.Empty;
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
                case "payment":
                    {
                        hlCart.NavigateUrl = SEOHelper.GetShoppingCartUrl();
                        hlAddress.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingaddress.aspx";
                        hlShipping.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingmethod.aspx";
                        hlPayment.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutpaymentmethod.aspx";
                        hlConfirm.NavigateUrl = string.Empty;
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
                case "confirm":
                    {
                        hlCart.NavigateUrl = SEOHelper.GetShoppingCartUrl();
                        hlAddress.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingaddress.aspx";
                        hlShipping.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutshippingmethod.aspx";
                        hlPayment.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutpaymentmethod.aspx";
                        hlConfirm.NavigateUrl = CommonHelper.GetStoreLocation() + "checkoutconfirm.aspx";
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
                case "complete":
                    {
                        hlCart.NavigateUrl = string.Empty;
                        hlAddress.NavigateUrl = string.Empty;
                        hlShipping.NavigateUrl = string.Empty;
                        hlPayment.NavigateUrl = string.Empty;
                        hlConfirm.NavigateUrl = string.Empty;
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
                default:
                    {
                        hlCart.NavigateUrl = string.Empty;
                        hlAddress.NavigateUrl = string.Empty;
                        hlShipping.NavigateUrl = string.Empty;
                        hlPayment.NavigateUrl = string.Empty;
                        hlConfirm.NavigateUrl = string.Empty;
                        hlComplete.NavigateUrl = string.Empty;
                    }
                    break;
            }
        }

        public string GetStatusCss(string step)
        {
            if (this.OrderProgressStep.ToLowerInvariant() == step.ToLowerInvariant())
                return "active-step";
            else
                return "inactive-step";
        }

        public string OrderProgressStep
        {
            get
            {
                object obj2 = this.ViewState["OrderProgressStep"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["OrderProgressStep"] = value;
            }
        }
    }
}
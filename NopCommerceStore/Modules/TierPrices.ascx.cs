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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class TierPriceControl : BaseNopUserControl
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
            var productVariant = ProductManager.GetProductVariantById(this.ProductVariantId);
            
            if (productVariant != null)
            {
                var tierPrices = productVariant.TierPrices;
                if (tierPrices.Count > 0)
                {
                    lvTierPrices.DataSource = tierPrices;
                    lvTierPrices.DataBind();
                }
                else
                    this.Visible = false; 
            }
            else
                this.Visible = false;
        }

        protected void lvTierPrices_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var currentItem = (ListViewDataItem)e.Item;
                var tierPrice = currentItem.DataItem as TierPrice;
                var productVariant = tierPrice.ProductVariant;

                var lblQuantity = (Label)currentItem.FindControl("lblQuantity");
                var lblPrice = (Label)currentItem.FindControl("lblPrice");

                decimal taxRate = decimal.Zero;
                decimal priceBase = TaxManager.GetPrice(productVariant, tierPrice.Price, out taxRate);
                decimal price = CurrencyManager.ConvertCurrency(priceBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);

                string priceStr = PriceHelper.FormatPrice(price, false, false);
                lblQuantity.Text = string.Format(GetLocaleResourceString("Products.TierPricesQuantityFormat"), tierPrice.Quantity);
                lblPrice.Text = priceStr;
            }
        }
            
        public int ProductVariantId
        {
            get
            {
                object obj2 = this.ViewState["ProductVariantId"];
                if (obj2 != null)
                    return (int)obj2;
                else
                    return 0;
            }
            set
            {
                this.ViewState["ProductVariantId"] = value;
            }
        }
    }
}
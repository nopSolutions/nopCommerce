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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class RecentlyViewedProductsBoxControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        private void BindData()
        {
            int number = ProductManager.RecentlyViewedProductsNumber;
            var products = ProductManager.GetRecentlyViewedProducts(number);
            if (ProductManager.RecentlyViewedProductsEnabled && products.Count > 0)
            {
                lvRecentlyViewedProducts.DataSource = products;
                lvRecentlyViewedProducts.DataBind();
            }
            else
            {
                this.Visible = false;
            }
        }


        protected void lvRecentlyViewedProducts_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = e.Item as ListViewDataItem;
                if (dataItem != null)
                {
                    var product = dataItem.DataItem as Product;
                    if (product != null)
                    {
                        var hlProduct = dataItem.FindControl("hlProduct") as HyperLink;
                        hlProduct.NavigateUrl = SEOHelper.GetProductUrl(product);
                    }
                }
            }
        }

    }
}
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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CrossSellProductsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                pnlData.Visible = true;
                pnlMessage.Visible = false;

                var existingCrossSellProductCollection = product.CrossSellProducts;
                List<CrossSellProductHelperClass> crossSellProducts = GetCrossSellProducts(existingCrossSellProductCollection);
                gvCrossSellProducts.Columns[1].Visible = SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages");
                gvCrossSellProducts.DataSource = crossSellProducts;
                gvCrossSellProducts.DataBind();
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
            }
        }

        private List<CrossSellProductHelperClass> GetCrossSellProducts(List<CrossSellProduct> existingCrossSellProductCollection)
        {
            List<CrossSellProductHelperClass> result = new List<CrossSellProductHelperClass>();
            foreach (CrossSellProduct crossSellProduct in existingCrossSellProductCollection)
            {
                Product product = crossSellProduct.Product2;
                if (product != null)
                {
                    CrossSellProductHelperClass rphc = new CrossSellProductHelperClass();
                    rphc.CrossSellProductId = crossSellProduct.CrossSellProductId;
                    rphc.ProductId2 = product.ProductId;
                    rphc.ProductInfo2 = product.Name;
                    if (SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages"))
                    {
                        rphc.ProductImage = GetProductImageUrl(product);
                    }
                    rphc.IsMapped = true;
                    result.Add(rphc);
                }
            }

            return result;
        }

        public string GetProductImageUrl(Product product)
        {
            var picture = product.DefaultPicture;
            if (picture != null)
            {
                return PictureManager.GetPictureUrl(picture, SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80));
            }
            else
            {
                return PictureManager.GetDefaultPictureUrl(SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80));
            }
        }

        [Serializable]
        private class CrossSellProductHelperClass
        {
            public int CrossSellProductId { get; set; }
            public int ProductId2 { get; set; }
            public string ProductInfo2 { get; set; }
            public string ProductImage { get; set; }
            public bool IsMapped { get; set; }
        }

        public void SaveInfo()
        {
            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                foreach (GridViewRow row in gvCrossSellProducts.Rows)
                {
                    CheckBox cbProductInfo2 = row.FindControl("cbProductInfo2") as CheckBox;
                    HiddenField hfProductId2 = row.FindControl("hfProductId2") as HiddenField;
                    HiddenField hfCrossSellProductId = row.FindControl("hfCrossSellProductId") as HiddenField;
                    int crossSellProductId = int.Parse(hfCrossSellProductId.Value);
                    int productId2 = int.Parse(hfProductId2.Value);
                    
                    if (crossSellProductId > 0 && !cbProductInfo2.Checked)
                        ProductManager.DeleteCrossSellProduct(crossSellProductId);
                    if (crossSellProductId > 0 && cbProductInfo2.Checked)
                        ProductManager.UpdateCrossSellProduct(crossSellProductId, product.ProductId, productId2);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAddNew.OnClientClick = string.Format("javascript:OpenWindow('CrossSellProductAdd.aspx?pid={0}&BtnId={1}', 800, 600, true); return false;", this.ProductId, btnRefresh.ClientID);
            
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }
    }
}
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
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CategoryProductControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Category category = CategoryManager.GetCategoryById(this.CategoryId);

            if (category != null)
            {
                var existingProductCategoryCollection = category.ProductCategories;
                List<ProductCategoryMappingHelperClass> productCategoryMappings = GetProductCategoryMappings(existingProductCategoryCollection);
                gvProductCategoryMappings.Columns[1].Visible = SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages");
                gvProductCategoryMappings.DataSource = productCategoryMappings;
                gvProductCategoryMappings.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAddNew.OnClientClick = string.Format("javascript:OpenWindow('CategoryProductAdd.aspx?cid={0}&BtnId={1}', 800, 600, true); return false;", this.CategoryId, btnRefresh.ClientID);
            
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public void SaveInfo()
        {
            Category category = CategoryManager.GetCategoryById(this.CategoryId);

            if (category != null)
            {
                foreach (GridViewRow row in gvProductCategoryMappings.Rows)
                {
                    CheckBox cbProductInfo = row.FindControl("cbProductInfo") as CheckBox;
                    HiddenField hfProductId = row.FindControl("hfProductId") as HiddenField;
                    HiddenField hfProductCategoryId = row.FindControl("hfProductCategoryId") as HiddenField;
                    CheckBox cbFeatured = row.FindControl("cbFeatured") as CheckBox;
                    NumericTextBox txtRowDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;

                    int productId = int.Parse(hfProductId.Value);
                    int productCategoryId = int.Parse(hfProductCategoryId.Value);
                    bool featured = cbFeatured.Checked;
                    int displayOrder = txtRowDisplayOrder.Value;

                    if (productCategoryId > 0 && !cbProductInfo.Checked)
                        CategoryManager.DeleteProductCategory(productCategoryId);
                    if (productCategoryId > 0 && cbProductInfo.Checked)
                        CategoryManager.UpdateProductCategory(productCategoryId, productId, category.CategoryId, featured, displayOrder);
                }
            }
        }

        protected void gvProductCategoryMappings_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductCategoryMappings.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        private List<ProductCategoryMappingHelperClass> GetProductCategoryMappings(List<ProductCategory> existingProductCategoryCollection)
        {
            List<ProductCategoryMappingHelperClass> result = new List<ProductCategoryMappingHelperClass>();
            foreach (ProductCategory pc in existingProductCategoryCollection)
            {
                Product product = pc.Product;
                if (product != null)
                {
                    ProductCategoryMappingHelperClass pcmhc = new ProductCategoryMappingHelperClass();
                    pcmhc.ProductCategoryId = pc.ProductCategoryId;
                    pcmhc.ProductId = pc.ProductId;
                    if (SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages"))
                    {
                        pcmhc.ProductImage = GetProductImageUrl(product);
                    }
                    pcmhc.ProductInfo = product.Name;
                    pcmhc.IsMapped = true;
                    pcmhc.IsFeatured = pc.IsFeaturedProduct;
                    pcmhc.DisplayOrder = pc.DisplayOrder;
                    result.Add(pcmhc);
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
        private class ProductCategoryMappingHelperClass
        {
            public int ProductCategoryId { get; set; }
            public int ProductId { get; set; }
            public string ProductImage { get; set; }
            public string ProductInfo { get; set; }
            public bool IsMapped { get; set; }
            public bool IsFeatured { get; set; }
            public int DisplayOrder { get; set; }
        }

        public int CategoryId
        {
            get
            {
                return CommonHelper.QueryStringInt("CategoryId");
            }
        }
    }
}
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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class RelatedProductsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Product product = IoC.Resolve<IProductService>().GetProductById(this.ProductId);
            if (product != null)
            {
                pnlData.Visible = true;
                pnlMessage.Visible = false;

                var existingRelatedProductCollection = product.RelatedProducts;
                List<RelatedProductHelperClass> relatedProducts = GetRelatedProducts(existingRelatedProductCollection);
                gvRelatedProducts.Columns[1].Visible = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowAdminProductImages");
                gvRelatedProducts.DataSource = relatedProducts;
                gvRelatedProducts.DataBind();
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
            }
        }

        private List<RelatedProductHelperClass> GetRelatedProducts(List<RelatedProduct> existingRelatedProductCollection)
        {
            List<RelatedProductHelperClass> result = new List<RelatedProductHelperClass>();
            foreach (RelatedProduct relatedProduct in existingRelatedProductCollection)
            {
                Product product = relatedProduct.Product2;
                if (product != null)
                {
                    RelatedProductHelperClass rphc = new RelatedProductHelperClass();
                    rphc.RelatedProductId = relatedProduct.RelatedProductId;
                    rphc.ProductId2 = product.ProductId;
                    rphc.ProductInfo2 = product.Name;
                    if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowAdminProductImages"))
                    {
                        rphc.ProductImage = GetProductImageUrl(product);
                    }
                    rphc.IsMapped = true;
                    rphc.DisplayOrder = relatedProduct.DisplayOrder;
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
                return IoC.Resolve<IPictureService>().GetPictureUrl(picture, IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80));
            }
            else
            {
                return IoC.Resolve<IPictureService>().GetDefaultPictureUrl(IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80));
            }
        }

        [Serializable]
        private class RelatedProductHelperClass
        {
            public int RelatedProductId { get; set; }
            public int ProductId2 { get; set; }
            public string ProductInfo2 { get; set; }
            public string ProductImage { get; set; }
            public bool IsMapped { get; set; }
            public int DisplayOrder { get; set; }
        }

        public void SaveInfo()
        {
            Product product = IoC.Resolve<IProductService>().GetProductById(this.ProductId);
            if (product != null)
            {
                foreach (GridViewRow row in gvRelatedProducts.Rows)
                {
                    CheckBox cbProductInfo2 = row.FindControl("cbProductInfo2") as CheckBox;
                    HiddenField hfProductId2 = row.FindControl("hfProductId2") as HiddenField;
                    HiddenField hfRelatedProductId = row.FindControl("hfRelatedProductId") as HiddenField;
                    NumericTextBox txtRowDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;
                    int relatedProductId = int.Parse(hfRelatedProductId.Value);
                    int productId2 = int.Parse(hfProductId2.Value);
                    int displayOrder = txtRowDisplayOrder.Value;

                    if (relatedProductId > 0 && !cbProductInfo2.Checked)
                        IoC.Resolve<IProductService>().DeleteRelatedProduct(relatedProductId);
                    if (relatedProductId > 0 && cbProductInfo2.Checked)
                    {
                        var rp = IoC.Resolve<IProductService>().GetRelatedProductById(relatedProductId);
                        if (rp!=null)
                        {
                            rp.ProductId1 = product.ProductId;
                            rp.ProductId2 = productId2;
                            rp.DisplayOrder = displayOrder;
                            IoC.Resolve<IProductService>().UpdateRelatedProduct(rp);
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAddNew.OnClientClick = string.Format("javascript:OpenWindow('RelatedProductAdd.aspx?pid={0}&BtnId={1}', 800, 600, true); return false;", this.ProductId, btnRefresh.ClientID);
            
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
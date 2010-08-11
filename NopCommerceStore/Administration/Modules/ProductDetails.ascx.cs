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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductDetailsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SelectTab(this.ProductTabs, this.TabId);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SimpleTextBox txtName = ctrlProductInfoEdit.FindControl("txtName") as SimpleTextBox;
            if(txtName != null)
            {
                txtProductCopyName.Text = "Copy of " + txtName.Text;
            }

            PreviewButton.OnClientClick = string.Format("javascript:OpenWindow('{0}', 800, 600, true); return false;", SEOHelper.GetProductUrl(this.ProductId));

            base.OnPreRender(e);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Product product = null;
                    //uncomment this line to support transactions
                    //using (var scope = new System.Transactions.TransactionScope())
                    {
                        product = ctrlProductInfoEdit.SaveInfo();
                        ctrlProductSEO.SaveInfo();
                        ctrlProductVariants.SaveInfo();
                        ctrlProductCategory.SaveInfo();
                        ctrlProductManufacturer.SaveInfo();
                        ctrlRelatedProducts.SaveInfo();
                        ctrlCrossSellProducts.SaveInfo();
                        ctrlProductPictures.SaveInfo();
                        ctrlProductSpecifications.SaveInfo();

                        CustomerActivityManager.InsertActivity(
                            "EditProduct",
                            GetLocaleResourceString("ActivityLog.EditProduct"),
                            product.Name);

                        //uncomment this line to support transactions
                        //scope.Complete();
                    }

                    if (product != null)
                        Response.Redirect(string.Format("ProductDetails.aspx?ProductID={0}&TabID={1}", product.ProductId, this.GetActiveTabId(this.ProductTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                Product product = ProductManager.GetProductById(this.ProductId);
                if (product != null)
                {
                    ProductManager.MarkProductAsDeleted(this.ProductId);

                    CustomerActivityManager.InsertActivity(
                        "DeleteProduct",
                        GetLocaleResourceString("ActivityLog.DeleteProduct"),
                        product.Name);

                }
                Response.Redirect("Products.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void BtnDuplicate_OnClick(object sender, EventArgs e)
        {
            try
            {
                Product productCopy = ProductManager.DuplicateProduct(ProductId, txtProductCopyName.Text, cbIsProductCopyPublished.Checked, cbCopyImages.Checked);
                if(productCopy != null)
                {
                    CustomerActivityManager.InsertActivity(
                        "AddNewProduct",
                        GetLocaleResourceString("ActivityLog.AddNewProduct"),
                        productCopy.Name);

                    Response.Redirect(String.Format("ProductDetails.aspx?ProductID={0}", productCopy.ProductId));
                }
            }
            catch(Exception ex)
            {
                ProcessException(ex);
            }
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }

        protected string TabId
        {
            get
            {
                return CommonHelper.QueryString("TabId");
            }
        }
    }
}
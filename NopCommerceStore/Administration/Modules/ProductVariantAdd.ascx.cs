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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.Common.Utils;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductVariantAddControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Product product = ProductManager.GetProductById(this.ProductId);
            if (product == null)
                Response.Redirect("Products.aspx");
            lblProductName.Text = Server.HtmlEncode(product.Name);
            hlProductURL.NavigateUrl = CommonHelper.GetStoreAdminLocation() + "ProductDetails.aspx?ProductID=" + product.ProductId.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    ProductVariant productVariant = ctrlProductVariantInfo.SaveInfo();
                    ctrlProductVariantDiscounts.SaveInfo(productVariant.ProductVariantId);

                    CustomerActivityManager.InsertActivity(
                        "AddNewProductVariant",
                        GetLocaleResourceString("ActivityLog.AddNewProductVariant"),
                        productVariant.FullProductName);

                    Response.Redirect("ProductVariantDetails.aspx?ProductVariantID=" + productVariant.ProductVariantId);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
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
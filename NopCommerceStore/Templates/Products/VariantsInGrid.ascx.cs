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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using AjaxControlToolkit;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Modules;
using NopSolutions.NopCommerce.BusinessLogic.Products;

namespace NopSolutions.NopCommerce.Web.Templates.Products
{
    public partial class VariantsInGrid : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                ctrlProductRating.Visible = product.AllowCustomerRatings;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            pnlProductReviews.Visible = ctrlProductReviews.Visible;
            pnlProductSpecs.Visible = ctrlProductSpecs.Visible;
            pnlProductTags.Visible = ctrlProductTags.Visible;
            ProductsTabs.Visible = pnlProductReviews.Visible ||
                pnlProductSpecs.Visible ||
                pnlProductTags.Visible;

            //little hack here
            if (pnlProductTags.Visible)
                ProductsTabs.ActiveTab = pnlProductTags;
            if (pnlProductSpecs.Visible)
                ProductsTabs.ActiveTab = pnlProductSpecs;
            if (pnlProductReviews.Visible)
                ProductsTabs.ActiveTab = pnlProductReviews;
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
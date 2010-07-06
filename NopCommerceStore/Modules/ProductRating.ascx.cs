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
using AjaxControlToolkit;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductRatingControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            var product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                decimal currentRating = 0;
                if (product.TotalRatingVotes != 0)
                    currentRating = (decimal)product.RatingSum / (decimal)product.TotalRatingVotes;

                productRating.CurrentRating = (int)Math.Round(currentRating);

                lblProductRatingResult.Text = string.Format(GetLocaleResourceString("Products.CurrentRating"), currentRating.ToString("N"), product.TotalRatingVotes);

            }
            else
                this.Visible = false;
        }


        protected void productRating_Changed(object sender, RatingEventArgs e)
        {
            if (NopContext.Current.User == null || NopContext.Current.User.IsGuest)
            {
                lblProductRatingResult.Text = GetLocaleResourceString("Products.OnlyRegisteredUsersCanRating");
                e.CallbackResult = GetLocaleResourceString("Products.OnlyRegisteredUsersCanRating");
            }
            else
            {
                ProductManager.SetProductRating(this.ProductId, int.Parse(e.Value));
                lblProductRatingResult.Text = GetLocaleResourceString("Products.RatingWillBeUpdatedVerySoon");
                e.CallbackResult = "Update done. Value = " + e.Value;
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
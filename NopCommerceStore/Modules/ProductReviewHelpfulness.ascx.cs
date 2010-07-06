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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductReviewHelpfulnessControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            var productReview = ProductManager.GetProductReviewById(this.ProductReviewId);
            if (productReview != null)
            {
                lblHelpfulYesTotal.Text = productReview.HelpfulYesTotal.ToString();
                lblHelpfulNoTotal.Text = productReview.HelpfulNoTotal.ToString();
            }
            else
                this.Visible = false;
        }

        private void SetHelpful(bool WasHelpful)
        {
            var productReview = ProductManager.GetProductReviewById(this.ProductReviewId);
            if (productReview != null)
            {
                if (NopContext.Current.User == null && CustomerManager.AllowAnonymousUsersToReviewProduct)
                    CustomerManager.CreateAnonymousUser();

                if (NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !CustomerManager.AllowAnonymousUsersToReviewProduct))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }
                
                ProductManager.SetProductRatingHelpfulness(productReview.ProductReviewId, WasHelpful);
                BindData();
            }
            else
                Response.Redirect(CommonHelper.GetStoreLocation());
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SetHelpful(true);
            }
        }

        protected void btnNo_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SetHelpful(false);
            }
        }
        
        public int ProductReviewId
        {
            get
            {
                object obj2 = this.ViewState["ProductReviewId"];
                if (obj2 != null)
                    return (int)obj2;
                else
                    return 0;
            }
            set
            {
                this.ViewState["ProductReviewId"] = value;
            }
        }
    }
}
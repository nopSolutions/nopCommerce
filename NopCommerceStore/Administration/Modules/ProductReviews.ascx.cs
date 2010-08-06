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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductReviewsControl : BaseNopAdministrationUserControl
    {
        protected void btnUpdateProductReview_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "UpdateItem")
            {
                int productReviewId = Convert.ToInt32(e.CommandArgument);
                ProductReview productReview = ProductManager.GetProductReviewById(productReviewId);
                if (productReview != null)
                {
                    ProductManager.UpdateProductReview(productReview.ProductReviewId,
                        productReview.ProductId, productReview.CustomerId, productReview.IPAddress, productReview.Title,
                        productReview.ReviewText, productReview.Rating, productReview.HelpfulYesTotal, productReview.HelpfulNoTotal, !productReview.IsApproved, productReview.CreatedOn);
                }
                BindData();
            }
        }

        protected void btnEditProductReview_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "EditItem")
            {
                int productReviewId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("ProductReviewDetails.aspx?ProductReviewID=" + productReviewId.ToString());
            }
        }

        protected void btnDeleteProductReview_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                int productReviewId = Convert.ToInt32(e.CommandArgument);
                ProductManager.DeleteProductReview(productReviewId);
                BindData();
            }
        }

        protected void gvProductReviews_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvProductReviews.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected string GetCustomerInfo(int customerId)
        {
            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer != null)
            {
                string customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                return customerInfo;
            }
            else
                return string.Empty;
        }

        protected void BindData()
        {
            List<ProductReview> productReviews = null;
            if (this.ProductId > 0)
                productReviews = ProductManager.GetProductReviewByProductId(ProductId);
            else
                productReviews = ProductManager.GetAllProductReviews();

            gvProductReviews.DataSource = productReviews;
            gvProductReviews.DataBind();
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
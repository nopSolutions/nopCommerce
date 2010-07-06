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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductReviewDetailsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ProductReview productReview = ProductManager.GetProductReviewById(this.ProductReviewId);
            if (productReview != null)
            {
                this.txtTitle.Text = productReview.Title;
                this.lblProduct.Text = GetProductInfo(productReview.ProductId);
                this.lblCustomer.Text = GetCustomerInfo(productReview.CustomerId);
                this.lblIPAddress.Text = productReview.IPAddress;
                //this.txtReviewText.Value = productReview.ReviewText;
                this.txtReviewText.Text = productReview.ReviewText;
                this.productRating.CurrentRating = productReview.Rating;
                this.cbIsApproved.Checked = productReview.IsApproved;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(productReview.CreatedOn, DateTimeKind.Utc).ToString();
            }
            else
                Response.Redirect("ProductReviews.aspx");
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

        protected string GetProductInfo(int productId)
        {
            Product product = ProductManager.GetProductById(productId);
            if (product != null)
            {
                string productInfo = string.Format("<a href=\"ProductDetails.aspx?ProductID={0}\">{1}</a>", product.ProductId, Server.HtmlEncode(product.Name));
                return productInfo;
            }
            else
                return string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    ProductReview productReview = ProductManager.GetProductReviewById(this.ProductReviewId);
                    if (productReview != null)
                    {
                        string title = txtTitle.Text.Trim();
                        string reviewText = txtReviewText.Text.Trim();
                        productReview = ProductManager.UpdateProductReview(productReview.ProductReviewId, productReview.ProductId,
                            productReview.CustomerId, productReview.IPAddress, title, reviewText,
                            productReview.Rating, productReview.HelpfulYesTotal, productReview.HelpfulNoTotal,
                            cbIsApproved.Checked, productReview.CreatedOn);
                        Response.Redirect("ProductReviewDetails.aspx?ProductReviewID=" + productReview.ProductReviewId.ToString());
                    }
                    else
                        Response.Redirect("ProductReviews.aspx");
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
                ProductManager.DeleteProductReview(this.ProductReviewId);
                Response.Redirect("ProductReviews.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int ProductReviewId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductReviewId");
            }
        }
    }
}
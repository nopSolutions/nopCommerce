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

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductPicturesControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                pnlData.Visible = true;
                pnlMessage.Visible = false;

                var productPictures = product.ProductPictures;
                if (productPictures.Count > 0)
                {
                    gvwImages.Visible = true;
                    gvwImages.DataSource = productPictures;
                    gvwImages.DataBind();
                }
                else
                    gvwImages.Visible = false;
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public void SaveInfo()
        {
        }

        protected void btnUploadProductPicture_Click(object sender, EventArgs e)
        {
            try
            {
                var product = ProductManager.GetProductById(this.ProductId);
                if(product != null)
                {
                    if(fuProductPicture1.HasFile)
                    {
                        Picture picture = PictureManager.InsertPicture(fuProductPicture1.FileBytes, fuProductPicture1.PostedFile.ContentType, true);
                        if(picture != null)
                        {
                            ProductPicture productPicture = ProductManager.InsertProductPicture(product.ProductId, picture.PictureId, txtProductPictureDisplayOrder1.Value);
                        }
                    }
                    if(fuProductPicture2.HasFile)
                    {
                        Picture picture = PictureManager.InsertPicture(fuProductPicture2.FileBytes, fuProductPicture2.PostedFile.ContentType, true);
                        if(picture != null)
                        {
                            ProductPicture productPicture = ProductManager.InsertProductPicture(product.ProductId, picture.PictureId, txtProductPictureDisplayOrder2.Value);
                        }
                    }
                    if(fuProductPicture3.HasFile)
                    {
                        Picture picture = PictureManager.InsertPicture(fuProductPicture3.FileBytes, fuProductPicture3.PostedFile.ContentType, true);
                        if(picture != null)
                        {
                            ProductPicture productPicture = ProductManager.InsertProductPicture(product.ProductId, picture.PictureId, txtProductPictureDisplayOrder3.Value);
                        }
                    }
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void gvwImages_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateProductPicture")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvwImages.Rows[index];
                NumericTextBox txtProductPictureDisplayOrder = row.FindControl("txtProductPictureDisplayOrder") as NumericTextBox;
                HiddenField hfProductPictureId = row.FindControl("hfProductPictureId") as HiddenField;

                int displayOrder = txtProductPictureDisplayOrder.Value;
                int productPictureId = int.Parse(hfProductPictureId.Value);
                ProductPicture productPicture = ProductManager.GetProductPictureById(productPictureId);

                if (productPicture != null)
                    ProductManager.UpdateProductPicture(productPicture.ProductPictureId,
                       productPicture.ProductId, productPicture.PictureId, displayOrder);

                BindData();
            }
        }

        protected void gvwImages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ProductPicture productPicture = (ProductPicture)e.Row.DataItem;
                Image iProductPicture = e.Row.FindControl("iProductPicture") as Image;
                if (iProductPicture != null)
                    iProductPicture.ImageUrl = PictureManager.GetPictureUrl(productPicture.PictureId);

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();
            }
        }

        protected void gvwImages_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int productPictureId = (int)gvwImages.DataKeys[e.RowIndex]["ProductPictureId"];
            ProductPicture productPicture = ProductManager.GetProductPictureById(productPictureId);
            if (productPicture != null)
            {
                ProductManager.DeleteProductPicture(productPicture.ProductPictureId);
                PictureManager.DeletePicture(productPicture.PictureId);
                BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            this.btnMoreUploads.Attributes["onclick"] = "showUploadPanels(); return false;";
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
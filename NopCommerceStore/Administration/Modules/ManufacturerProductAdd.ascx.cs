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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ManufacturerProductAddControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
            }
        }

        protected void FillDropDowns()
        {
            ParentCategory.EmptyItemText = GetLocaleResourceString("Admin.Common.All");
            ParentCategory.BindData();

            this.ddlManufacturer.Items.Clear();
            ListItem itemEmptyManufacturer = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlManufacturer.Items.Add(itemEmptyManufacturer);
            var manufacturers = ManufacturerManager.GetAllManufacturers();
            foreach (Manufacturer manufacturer in manufacturers)
            {
                ListItem item2 = new ListItem(manufacturer.Name, manufacturer.ManufacturerId.ToString());
                this.ddlManufacturer.Items.Add(item2);
            }
        }

        protected List<Product> GetProducts()
        {
            string productName = txtProductName.Text;
            int categoryId = ParentCategory.SelectedCategoryId;
            int manufacturerId = int.Parse(this.ddlManufacturer.SelectedItem.Value);

            int totalRecords = 0;
            var products = ProductManager.GetAllProducts(categoryId,
                manufacturerId, 0, null,
                null, null, productName, false, 1000, 0, null, out totalRecords);
            return products;
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

        protected void BindGrid()
        {
            var products = GetProducts();
            if (products.Count > 0)
            {
                this.gvProducts.Visible = true;
                this.btnSave.Visible = true;
                this.lblNoProductsFound.Visible = false;
                this.gvProducts.Columns[2].Visible = SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages");

                this.gvProducts.DataSource = products;
                this.gvProducts.DataBind();
            }
            else
            {
                this.gvProducts.Visible = false;
                this.btnSave.Visible = false;
                this.lblNoProductsFound.Visible = true;
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Manufacturer manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);
            if (manufacturer != null)
            {
                var existingProductManufacturers = manufacturer.ProductManufacturers;

                foreach (GridViewRow row in gvProducts.Rows)
                {
                    try
                    {
                        CheckBox cbProductInfo = row.FindControl("cbProductInfo") as CheckBox;
                        HiddenField hfProductId = row.FindControl("hfProductId") as HiddenField;
                        NumericTextBox txtRowDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;
                        int productId = int.Parse(hfProductId.Value);
                        int displayOrder = txtRowDisplayOrder.Value;
                        if (cbProductInfo.Checked)
                        {
                            if (existingProductManufacturers.FindProductManufacturer(productId, this.ManufacturerId) == null)
                            {
                                ManufacturerManager.InsertProductManufacturer(productId, this.ManufacturerId, false, displayOrder);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        ProcessException(exc);
                    }
                }
            }

            this.Page.ClientScript.RegisterStartupScript(typeof(ManufacturerProductAddControl), "closerefresh", "<script language=javascript>try {window.opener.document.forms[0]." + this.BtnId + ".click();}catch (e){} window.close();</script>");
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            base.OnPreRender(e);
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        private string BtnId
        {
            get
            {
                return CommonHelper.QueryString("BtnId");
            }
        }

        public int ManufacturerId
        {
            get
            {
                return CommonHelper.QueryStringInt("mid");
            }
        }
    }
}
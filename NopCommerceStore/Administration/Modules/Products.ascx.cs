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
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                if (SettingManager.GetSettingValueBoolean("Admin.LoadAllProducts"))
                {
                    BindGrid();
                }
            }

            //buttons
            btnPDFExport.Visible = SettingManager.GetSettingValueBoolean("Features.SupportPDF");
            btnExportXLS.Visible = SettingManager.GetSettingValueBoolean("Features.SupportExcel");
            btnImportXLS.Visible = SettingManager.GetSettingValueBoolean("Features.SupportExcel");
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvProducts.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            base.OnPreRender(e);
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
                manufacturerId, 0, null, null, null, productName, 
                false, int.MaxValue, 0, null, 
                ProductSortingEnum.Position, out totalRecords);
            return products;
        }

        protected void BindGrid()
        {
            var products = GetProducts();
            if (products.Count > 0)
            {
                this.gvProducts.Visible = true;
                this.lblNoProductsFound.Visible = false;
                this.gvProducts.Columns[1].Visible = SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages");
                this.gvProducts.DataSource = products;
                this.gvProducts.DataBind();
            }
            else
            {
                this.gvProducts.Visible = false;
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

        protected void BtnPDFExport_OnClick(object sender, EventArgs e)
        {
            if(Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("products_{0}_{1}.pdf", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                    string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);

                    PDFHelper.PrintProductsToPdf(GetProducts(), filePath);

                    CommonHelper.WriteResponsePdf(filePath, fileName);
                }
                catch(Exception ex)
                {
                    ProcessException(ex);
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
                try
                {
                    foreach (GridViewRow row in gvProducts.Rows)
                    {
                        var cbProduct = row.FindControl("cbProduct") as CheckBox;
                        var hfProductId = row.FindControl("hfProductId") as HiddenField;

                        bool isChecked = cbProduct.Checked;
                        int productId = int.Parse(hfProductId.Value);
                        if (isChecked)
                        {
                            ProductManager.MarkProductAsDeleted(productId);
                        }
                    }

                    BindGrid();
                }
                catch(Exception ex)
                {
                    ProcessException(ex);
                }
        }

        protected void btnExportXML_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = String.Format("products_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                    
                    var products = GetProducts();
                    string xml = ExportManager.ExportProductsToXml(products);
                    CommonHelper.WriteResponseXml(xml, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
        
        protected void btnExportXLS_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("products_{0}_{1}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                    string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);
                    var products = GetProducts();

                    ExportManager.ExportProductsToXls(filePath, products);
                    CommonHelper.WriteResponseXls(filePath, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnImportXLS_Click(object sender, EventArgs e)
        {
            if (fuXlsFile.PostedFile != null && !String.IsNullOrEmpty(fuXlsFile.FileName))
            {
                try
                {
                    byte[] fileBytes = fuXlsFile.FileBytes;
                    string extension = "xls";
                    if (fuXlsFile.FileName.EndsWith("xlsx"))
                        extension = "xlsx";

                    string fileName = string.Format("products_{0}_{1}.{2}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4), extension);
                    string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);

                    File.WriteAllBytes(filePath, fileBytes);
                    ImportManager.ImportProductsFromXls(filePath);

                    BindGrid();
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }
            }
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

        protected void btnGoDirectlyToSKU_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string sku = txtSKU.Text.Trim();
                    var pv = ProductManager.GetProductVariantBySKU(sku);
                    if (pv != null)
                    {
                        string url = string.Format("{0}ProductVariantDetails.aspx?ProductVariantId={1}", CommonHelper.GetStoreAdminLocation(), pv.ProductVariantId);
                        Response.Redirect(url);
                    }
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}
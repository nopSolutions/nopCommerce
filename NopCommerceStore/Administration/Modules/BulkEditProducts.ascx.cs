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
    public partial class BulkEditProductsControl : BaseNopAdministrationUserControl
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
        }

        protected void gvProductVariants_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvProductVariants.PageIndex = e.NewPageIndex;
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


        protected List<ProductVariant> GetProductVariants()
        {
            string productName = txtProductName.Text;
            int categoryId = ParentCategory.SelectedCategoryId;
            int manufacturerId = int.Parse(this.ddlManufacturer.SelectedItem.Value);

            int totalRecords = 0;
            var productVariants = ProductManager.GetAllProductVariants(categoryId, 
                manufacturerId, productName, int.MaxValue, 0, out totalRecords);
            return productVariants;
        }

        protected void BindGrid()
        {
            var productVariants = GetProductVariants();
            if (productVariants.Count > 0)
            {
                this.gvProductVariants.Visible = true;
                this.lblNoProductsFound.Visible = false;
                this.gvProductVariants.DataSource = productVariants;
                this.gvProductVariants.DataBind();
            }
            else
            {
                this.gvProductVariants.Visible = false;
                this.lblNoProductsFound.Visible = true;
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gvProductVariants.Rows)
                {
                    var cbProductVariant = row.FindControl("cbProductVariant") as CheckBox;
                    var hfProductVariantId = row.FindControl("hfProductVariantId") as HiddenField;

                    var txtPrice = row.FindControl("txtPrice") as DecimalTextBox;
                    var txtOldPrice = row.FindControl("txtOldPrice") as DecimalTextBox;
                    var cbPublished = row.FindControl("cbPublished") as CheckBox;

                    bool isChecked = cbProductVariant.Checked;
                    int productVariantId = int.Parse(hfProductVariantId.Value);
                    if (isChecked)
                    {
                        int pvId = int.Parse(hfProductVariantId.Value);
                        decimal price = txtPrice.Value;
                        decimal oldPrice = txtOldPrice.Value;
                        bool published = cbPublished.Checked;
                        var productVariant = ProductManager.GetProductVariantById(pvId);
                        if (productVariant != null)
                        {
                            productVariant = ProductManager.UpdateProductVariant(productVariant.ProductVariantId,
                                productVariant.ProductId, productVariant.Name,
                                productVariant.SKU, productVariant.Description,
                                productVariant.AdminComment, productVariant.ManufacturerPartNumber,
                                productVariant.IsGiftCard, productVariant.GiftCardType, 
                                productVariant.IsDownload,
                                productVariant.DownloadId, productVariant.UnlimitedDownloads,
                                productVariant.MaxNumberOfDownloads,
                                productVariant.DownloadExpirationDays, 
                                (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                                productVariant.HasSampleDownload, productVariant.SampleDownloadId,
                                productVariant.HasUserAgreement, productVariant.UserAgreementText,
                                productVariant.IsRecurring, productVariant.CycleLength,
                                productVariant.CyclePeriod, productVariant.TotalCycles,
                                productVariant.IsShipEnabled, productVariant.IsFreeShipping, productVariant.AdditionalShippingCharge,
                                productVariant.IsTaxExempt, productVariant.TaxCategoryId,
                                productVariant.ManageInventory, productVariant.StockQuantity,
                                productVariant.DisplayStockAvailability, productVariant.DisplayStockQuantity,
                                productVariant.MinStockQuantity, productVariant.LowStockActivity,
                                productVariant.NotifyAdminForQuantityBelow, productVariant.Backorders,
                                productVariant.OrderMinimumQuantity, productVariant.OrderMaximumQuantity,
                                productVariant.WarehouseId, productVariant.DisableBuyButton,
                                productVariant.CallForPrice, price, oldPrice,
                                productVariant.ProductCost, productVariant.CustomerEntersPrice,
                                productVariant.MinimumCustomerEnteredPrice, productVariant.MaximumCustomerEnteredPrice,
                                productVariant.Weight, productVariant.Length, productVariant.Width, 
                                productVariant.Height, productVariant.PictureId,
                                productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                                published, productVariant.Deleted, productVariant.DisplayOrder, 
                                productVariant.CreatedOn, productVariant.UpdatedOn);
                        }
                    }
                }

                //BindGrid();
                ShowMessage(GetLocaleResourceString("Admin.BulkEditProducts.SuccessfullyUpdated"));
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }
    }
}
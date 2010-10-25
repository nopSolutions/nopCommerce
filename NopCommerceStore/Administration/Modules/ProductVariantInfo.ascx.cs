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
using FredCK.FCKeditorV2;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductVariantInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ProductVariant productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(this.ProductVariantId);

            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }

            if (productVariant != null)
            {
                this.pnlProductVariantId.Visible = true;
                this.lblProductVariantId.Text = productVariant.ProductVariantId.ToString();


                this.txtName.Text = productVariant.Name;
                this.txtSKU.Text = productVariant.SKU;
                this.txtDescription.Value = productVariant.Description;
                this.txtAdminComment.Text = productVariant.AdminComment;
                this.txtManufacturerPartNumber.Text = productVariant.ManufacturerPartNumber;

                //gift card
                this.cbIsGiftCard.Checked = productVariant.IsGiftCard;
                CommonHelper.SelectListItem(this.ddlGiftCardType, productVariant.GiftCardType);

                //downloable products
                this.cbIsDownload.Checked = productVariant.IsDownload;
                Download productVariantDownload = productVariant.Download;
                if (productVariantDownload != null)
                {
                    this.cbUseDownloadURL.Checked = productVariantDownload.UseDownloadUrl;
                    this.txtDownloadURL.Text = productVariantDownload.DownloadUrl;

                    if (productVariantDownload.DownloadBinary != null)
                    {
                        btnRemoveProductVariantDownload.Visible = true;
                        hlProductVariantDownload.Visible = true;
                        string adminDownloadUrl = IoCFactory.Resolve<IDownloadManager>().GetAdminDownloadUrl(productVariantDownload);
                        hlProductVariantDownload.NavigateUrl = adminDownloadUrl;
                    }
                    else
                    {
                        btnRemoveProductVariantDownload.Visible = false;
                        hlProductVariantDownload.Visible = false;
                    }
                }
                else
                {
                    btnRemoveProductVariantDownload.Visible = false;
                    hlProductVariantDownload.Visible = false;
                }

                this.cbUnlimitedDownloads.Checked = productVariant.UnlimitedDownloads;
                this.txtMaxNumberOfDownloads.Value = productVariant.MaxNumberOfDownloads;
                if (productVariant.DownloadExpirationDays.HasValue)
                    this.txtDownloadExpirationDays.Text = productVariant.DownloadExpirationDays.Value.ToString();
                CommonHelper.SelectListItem(this.ddlDownloadActivationType, productVariant.DownloadActivationType);
                this.cbHasUserAgreement.Checked = productVariant.HasUserAgreement;
                this.txtUserAgreementText.Value = productVariant.UserAgreementText;
                

                this.cbHasSampleDownload.Checked = productVariant.HasSampleDownload;
                Download productVariantSampleDownload = productVariant.SampleDownload;
                if (productVariantSampleDownload != null)
                {
                    this.cbUseSampleDownloadURL.Checked = productVariantSampleDownload.UseDownloadUrl;
                    this.txtSampleDownloadURL.Text = productVariantSampleDownload.DownloadUrl;

                    if (productVariantSampleDownload.DownloadBinary != null)
                    {
                        btnRemoveProductVariantSampleDownload.Visible = true;
                        hlProductVariantSampleDownload.Visible = true;
                        string adminSampleDownloadUrl = IoCFactory.Resolve<IDownloadManager>().GetAdminDownloadUrl(productVariantSampleDownload);
                        hlProductVariantSampleDownload.NavigateUrl = adminSampleDownloadUrl;
                    }
                    else
                    {
                        btnRemoveProductVariantSampleDownload.Visible = false;
                        hlProductVariantSampleDownload.Visible = false;
                    }
                }
                else
                {
                    btnRemoveProductVariantSampleDownload.Visible = false;
                    hlProductVariantSampleDownload.Visible = false;
                }

                //recurring
                this.cbIsRecurring.Checked = productVariant.IsRecurring;
                this.txtCycleLength.Value = productVariant.CycleLength;
                CommonHelper.SelectListItem(this.ddlCyclePeriod, productVariant.CyclePeriod);
                this.txtTotalCycles.Value = productVariant.TotalCycles;

                //shipping
                this.cbIsShipEnabled.Checked = productVariant.IsShipEnabled;
                this.cbIsFreeShipping.Checked = productVariant.IsFreeShipping;
                this.txtAdditionalShippingCharge.Value = productVariant.AdditionalShippingCharge;

                //tax
                this.cbIsTaxExempt.Checked = productVariant.IsTaxExempt;
                CommonHelper.SelectListItem(this.ddlTaxCategory, productVariant.TaxCategoryId);

                //stock management
                CommonHelper.SelectListItem(this.ddlManageStock, productVariant.ManageInventory);
                this.txtStockQuantity.Value = productVariant.StockQuantity;
                this.cbDisplayStockAvailability.Checked = productVariant.DisplayStockAvailability;
                this.cbDisplayStockQuantity.Checked = productVariant.DisplayStockQuantity;
                this.txtMinStockQuantity.Value = productVariant.MinStockQuantity;
                CommonHelper.SelectListItem(this.ddlLowStockActivity, productVariant.LowStockActivityId);
                this.txtNotifyForQuantityBelow.Value = productVariant.NotifyAdminForQuantityBelow;
                CommonHelper.SelectListItem(this.ddlBackorders, productVariant.Backorders);
                this.txtOrderMinimumQuantity.Value = productVariant.OrderMinimumQuantity;
                this.txtOrderMaximumQuantity.Value = productVariant.OrderMaximumQuantity;

                //other settings
                CommonHelper.SelectListItem(this.ddlWarehouse, productVariant.WarehouseId);
                this.cbDisableBuyButton.Checked = productVariant.DisableBuyButton;
                this.cbCallForPrice.Checked = productVariant.CallForPrice;                
                this.txtPrice.Value = productVariant.Price;
                this.txtOldPrice.Value = productVariant.OldPrice;
                this.txtProductCost.Value = productVariant.ProductCost;
                this.cbCustomerEntersPrice.Checked = productVariant.CustomerEntersPrice;
                this.txtMinimumCustomerEnteredPrice.Value = (int)productVariant.MinimumCustomerEnteredPrice;
                this.txtMaximumCustomerEnteredPrice.Value = (int)productVariant.MaximumCustomerEnteredPrice;

                this.txtWeight.Value = productVariant.Weight;
                this.txtLength.Value = productVariant.Length;
                this.txtWidth.Value = productVariant.Width;
                this.txtHeight.Value = productVariant.Height;

                //picture
                Picture productVariantPicture = productVariant.Picture;
                btnRemoveProductVariantImage.Visible = productVariantPicture != null;
                string pictureUrl = IoCFactory.Resolve<IPictureManager>().GetPictureUrl(productVariantPicture, 100);
                this.iProductVariantPicture.Visible = true;
                this.iProductVariantPicture.ImageUrl = pictureUrl;

                //available dates
                if (productVariant.AvailableStartDateTime.HasValue)
                {
                    this.ctrlAvailableStartDateTimePicker.SelectedDate = productVariant.AvailableStartDateTime.Value;
                }
                if (productVariant.AvailableEndDateTime.HasValue)
                {
                    this.ctrlAvailableEndDateTimePicker.SelectedDate = productVariant.AvailableEndDateTime.Value;
                }

                this.cbPublished.Checked = productVariant.Published;
                this.txtDisplayOrder.Value = productVariant.DisplayOrder;
            }
            else
            {
                this.pnlProductVariantId.Visible = false;

                this.btnRemoveProductVariantImage.Visible = false;
                this.iProductVariantPicture.Visible = false;

                btnRemoveProductVariantDownload.Visible = false;
                hlProductVariantDownload.Visible = false;

                btnRemoveProductVariantSampleDownload.Visible = false;
                hlProductVariantSampleDownload.Visible = false;
            }
        }

        private void FillDropDowns()
        {
            CommonHelper.FillDropDownWithEnum(this.ddlGiftCardType, typeof(GiftCardTypeEnum));

            CommonHelper.FillDropDownWithEnum(this.ddlDownloadActivationType, typeof(DownloadActivationTypeEnum));

            CommonHelper.FillDropDownWithEnum(this.ddlCyclePeriod, typeof(RecurringProductCyclePeriodEnum));

            this.ddlTaxCategory.Items.Clear();
            ListItem itemTaxCategory = new ListItem("---", "0");
            this.ddlTaxCategory.Items.Add(itemTaxCategory);
            var taxCategoryCollection = IoCFactory.Resolve<ITaxCategoryManager>().GetAllTaxCategories();
            foreach (TaxCategory taxCategory in taxCategoryCollection)
            {
                ListItem item2 = new ListItem(taxCategory.Name, taxCategory.TaxCategoryId.ToString());
                this.ddlTaxCategory.Items.Add(item2);
            }

            this.ddlWarehouse.Items.Clear();
            ListItem itemWarehouse = new ListItem("---", "0");
            this.ddlWarehouse.Items.Add(itemWarehouse);
            var warehouseCollection = IoCFactory.Resolve<IWarehouseManager>().GetAllWarehouses();
            foreach (Warehouse warehouse in warehouseCollection)
            {
                ListItem item2 = new ListItem(warehouse.Name, warehouse.WarehouseId.ToString());
                this.ddlWarehouse.Items.Add(item2);
            }

            CommonHelper.FillDropDownWithEnum(this.ddlLowStockActivity, typeof(LowStockActivityEnum));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();


            this.cbIsGiftCard.Attributes.Add("onclick", "toggleGiftCard();");

            this.cbCustomerEntersPrice.Attributes.Add("onclick", "toggleCustomerEntersPrice();");

            this.cbIsDownload.Attributes.Add("onclick", "toggleDownloadableProduct();");
            this.cbUseDownloadURL.Attributes.Add("onclick", "toggleDownloadableProduct();");
            this.cbUnlimitedDownloads.Attributes.Add("onclick", "toggleDownloadableProduct();");
            this.cbHasSampleDownload.Attributes.Add("onclick", "toggleDownloadableProduct();");
            this.cbUseSampleDownloadURL.Attributes.Add("onclick", "toggleDownloadableProduct();");
            this.cbHasUserAgreement.Attributes.Add("onclick", "toggleDownloadableProduct();");

            this.cbIsRecurring.Attributes.Add("onclick", "toggleRecurring();");

            this.cbIsShipEnabled.Attributes.Add("onclick", "toggleShipping();");

            this.ddlManageStock.Attributes.Add("onchange", "toggleManageStock();");

            this.cbDisplayStockAvailability.Attributes.Add("onclick", "toggleManageStock();");

            base.OnPreRender(e);
        }

        public ProductVariant SaveInfo()
        {
            DateTime nowDT = DateTime.UtcNow;

            string name = txtName.Text.Trim();
            string sku = txtSKU.Text.Trim();
            string description = txtDescription.Value;
            string adminComment = txtAdminComment.Text.Trim();
            string manufacturerPartNumber = txtManufacturerPartNumber.Text.Trim();
            bool isGiftCard = cbIsGiftCard.Checked;
            int giftCardType = int.Parse(this.ddlGiftCardType.SelectedItem.Value);
            bool isDownload = cbIsDownload.Checked;
            bool unlimitedDownloads = cbUnlimitedDownloads.Checked;
            int maxNumberOfDownloads = txtMaxNumberOfDownloads.Value;
            int? downloadExpirationDays = null;
            if (!String.IsNullOrEmpty(txtDownloadExpirationDays.Text.Trim()))
                downloadExpirationDays = int.Parse(txtDownloadExpirationDays.Text.Trim());
            DownloadActivationTypeEnum downloadActivationType = (DownloadActivationTypeEnum)Enum.ToObject(typeof(DownloadActivationTypeEnum), int.Parse(this.ddlDownloadActivationType.SelectedItem.Value));
            bool hasUserAgreement = cbHasUserAgreement.Checked;
            string userAgreementText = txtUserAgreementText.Value;
            bool hasSampleDownload = cbHasSampleDownload.Checked;

            bool isRecurring = cbIsRecurring.Checked;
            int cycleLength = txtCycleLength.Value;
            RecurringProductCyclePeriodEnum cyclePeriod = (RecurringProductCyclePeriodEnum)Enum.ToObject(typeof(RecurringProductCyclePeriodEnum), int.Parse(this.ddlCyclePeriod.SelectedItem.Value));
            int totalCycles = txtTotalCycles.Value;

            bool isShipEnabled = cbIsShipEnabled.Checked;
            bool isFreeShipping = cbIsFreeShipping.Checked;
            decimal additionalShippingCharge = txtAdditionalShippingCharge.Value;
            bool isTaxExempt = cbIsTaxExempt.Checked;
            int taxCategoryId = int.Parse(this.ddlTaxCategory.SelectedItem.Value);
            int manageStock = Convert.ToInt32(ddlManageStock.SelectedValue);
            int stockQuantity = txtStockQuantity.Value;
            bool displayStockAvailability = cbDisplayStockAvailability.Checked;
            bool displayStockQuantity = cbDisplayStockQuantity.Checked;
            int minStockQuantity = txtMinStockQuantity.Value;
            LowStockActivityEnum lowStockActivity = (LowStockActivityEnum)Enum.ToObject(typeof(LowStockActivityEnum), int.Parse(this.ddlLowStockActivity.SelectedItem.Value));
            int notifyForQuantityBelow = txtNotifyForQuantityBelow.Value;
            int backorders = int.Parse(this.ddlBackorders.SelectedItem.Value);
            int orderMinimumQuantity = txtOrderMinimumQuantity.Value;
            int orderMaximumQuantity = txtOrderMaximumQuantity.Value;
            int warehouseId = int.Parse(this.ddlWarehouse.SelectedItem.Value);
            bool disableBuyButton = cbDisableBuyButton.Checked;
            bool callForPrice = cbCallForPrice.Checked;
            decimal price = txtPrice.Value;
            decimal oldPrice = txtOldPrice.Value;
            decimal productCost = txtProductCost.Value;
            bool customerEntersPrice = cbCustomerEntersPrice.Checked;
            decimal minimumCustomerEnteredPrice = txtMinimumCustomerEnteredPrice.Value;
            decimal maximumCustomerEnteredPrice = txtMaximumCustomerEnteredPrice.Value;
            decimal weight = txtWeight.Value;
            decimal length = txtLength.Value;
            decimal width = txtWidth.Value;
            decimal height = txtHeight.Value;
            DateTime? availableStartDateTime = ctrlAvailableStartDateTimePicker.SelectedDate;
            DateTime? availableEndDateTime = ctrlAvailableEndDateTimePicker.SelectedDate;
            if (availableStartDateTime.HasValue)
            {
                availableStartDateTime = DateTime.SpecifyKind(availableStartDateTime.Value, DateTimeKind.Utc);
            }
            if (availableEndDateTime.HasValue)
            {
                availableEndDateTime = DateTime.SpecifyKind(availableEndDateTime.Value, DateTimeKind.Utc);
            }
            bool published = cbPublished.Checked;
            int displayOrder = txtDisplayOrder.Value;

            ProductVariant productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(ProductVariantId);
            if (productVariant != null)
            {
                #region Update
                Picture productVariantPicture = productVariant.Picture;
                HttpPostedFile productVariantPictureFile = fuProductVariantPicture.PostedFile;
                if ((productVariantPictureFile != null) && (!String.IsNullOrEmpty(productVariantPictureFile.FileName)))
                {
                    byte[] productVariantPictureBinary = IoCFactory.Resolve<IPictureManager>().GetPictureBits(productVariantPictureFile.InputStream, productVariantPictureFile.ContentLength);
                    if (productVariantPicture != null)
                        productVariantPicture = IoCFactory.Resolve<IPictureManager>().UpdatePicture(productVariantPicture.PictureId, productVariantPictureBinary, productVariantPictureFile.ContentType, true);
                    else
                        productVariantPicture = IoCFactory.Resolve<IPictureManager>().InsertPicture(productVariantPictureBinary, productVariantPictureFile.ContentType, true);
                }
                int productVariantPictureId = 0;
                if (productVariantPicture != null)
                    productVariantPictureId = productVariantPicture.PictureId;

                int productVariantDownloadId = 0;
                if (isDownload)
                {
                    Download productVariantDownload = productVariant.Download;
                    bool useDownloadURL = cbUseDownloadURL.Checked;
                    string downloadURL = txtDownloadURL.Text.Trim();
                    byte[] productVariantDownloadBinary = null;
                    string downloadContentType = string.Empty;
                    string downloadFilename = string.Empty;
                    string downloadExtension = string.Empty;
                    if (productVariantDownload != null)
                    {
                        productVariantDownloadBinary = productVariantDownload.DownloadBinary;
                        downloadContentType = productVariantDownload.ContentType;
                        downloadFilename = productVariantDownload.Filename;
                        downloadExtension = productVariantDownload.Extension;
                    }

                    HttpPostedFile productVariantDownloadFile = fuProductVariantDownload.PostedFile;
                    if ((productVariantDownloadFile != null) && (!String.IsNullOrEmpty(productVariantDownloadFile.FileName)))
                    {
                        productVariantDownloadBinary = IoCFactory.Resolve<IDownloadManager>().GetDownloadBits(productVariantDownloadFile.InputStream, productVariantDownloadFile.ContentLength);
                        downloadContentType = productVariantDownloadFile.ContentType;
                        downloadFilename = Path.GetFileNameWithoutExtension(productVariantDownloadFile.FileName);
                        downloadExtension = Path.GetExtension(productVariantDownloadFile.FileName);
                    }

                    if (productVariantDownload != null)
                    {
                        productVariantDownload.UseDownloadUrl = useDownloadURL;
                        productVariantDownload.DownloadUrl = downloadURL;
                        productVariantDownload.DownloadBinary = productVariantDownloadBinary;
                        productVariantDownload.ContentType = downloadContentType;
                        productVariantDownload.Filename = downloadFilename;
                        productVariantDownload.Extension = downloadExtension;
                        productVariantDownload.IsNew = true;
                        IoCFactory.Resolve<IDownloadManager>().UpdateDownload(productVariantDownload);
                    }
                    else
                    {
                        productVariantDownload = new Download()
                        {
                            UseDownloadUrl = useDownloadURL,
                            DownloadUrl = downloadURL,
                            DownloadBinary = productVariantDownloadBinary,
                            ContentType = downloadContentType,
                            Filename = downloadFilename,
                            Extension = downloadExtension,
                            IsNew = true
                        };
                        IoCFactory.Resolve<IDownloadManager>().InsertDownload(productVariantDownload);
                    }

                    productVariantDownloadId = productVariantDownload.DownloadId;
                }

                int productVariantSampleDownloadId = 0;
                if (hasSampleDownload)
                {
                    Download productVariantSampleDownload = productVariant.SampleDownload;
                    bool useSampleDownloadURL = cbUseSampleDownloadURL.Checked;
                    string sampleDownloadURL = txtSampleDownloadURL.Text.Trim();
                    byte[] productVariantSampleDownloadBinary = null;
                    string sampleDownloadContentType = string.Empty;
                    string sampleDownloadFilename = string.Empty;
                    string sampleDownloadExtension = string.Empty;
                    if (productVariantSampleDownload != null)
                    {
                        productVariantSampleDownloadBinary = productVariantSampleDownload.DownloadBinary;
                        sampleDownloadContentType = productVariantSampleDownload.ContentType;
                        sampleDownloadFilename = productVariantSampleDownload.Filename;
                        sampleDownloadExtension = productVariantSampleDownload.Extension;
                    }

                    HttpPostedFile productVariantSampleDownloadFile = fuProductVariantSampleDownload.PostedFile;
                    if ((productVariantSampleDownloadFile != null) && (!String.IsNullOrEmpty(productVariantSampleDownloadFile.FileName)))
                    {
                        productVariantSampleDownloadBinary = IoCFactory.Resolve<IDownloadManager>().GetDownloadBits(productVariantSampleDownloadFile.InputStream, productVariantSampleDownloadFile.ContentLength);
                        sampleDownloadContentType = productVariantSampleDownloadFile.ContentType;
                        sampleDownloadFilename = Path.GetFileNameWithoutExtension(productVariantSampleDownloadFile.FileName);
                        sampleDownloadExtension = Path.GetExtension(productVariantSampleDownloadFile.FileName);
                    }

                    if (productVariantSampleDownload != null)
                    {
                        productVariantSampleDownload.UseDownloadUrl = useSampleDownloadURL;
                        productVariantSampleDownload.DownloadUrl = sampleDownloadURL;
                        productVariantSampleDownload.DownloadBinary = productVariantSampleDownloadBinary;
                        productVariantSampleDownload.ContentType = sampleDownloadContentType;
                        productVariantSampleDownload.Filename = sampleDownloadFilename;
                        productVariantSampleDownload.Extension = sampleDownloadExtension;
                        productVariantSampleDownload.IsNew = true;
                        IoCFactory.Resolve<IDownloadManager>().UpdateDownload(productVariantSampleDownload);
                    }
                    else
                    {
                        productVariantSampleDownload = new Download()
                        {
                            UseDownloadUrl = useSampleDownloadURL,
                            DownloadUrl = sampleDownloadURL,
                            DownloadBinary = productVariantSampleDownloadBinary,
                            ContentType = sampleDownloadContentType,
                            Filename = sampleDownloadFilename,
                            Extension = sampleDownloadExtension,
                            IsNew = true
                        };
                        IoCFactory.Resolve<IDownloadManager>().InsertDownload(productVariantSampleDownload);
                    }

                    productVariantSampleDownloadId = productVariantSampleDownload.DownloadId;
                }


                productVariant.Name = name;
                productVariant.SKU = sku;
                productVariant.Description = description;
                productVariant.AdminComment = adminComment;
                productVariant.ManufacturerPartNumber = manufacturerPartNumber;
                productVariant.IsGiftCard = isGiftCard;
                productVariant.GiftCardType = giftCardType;
                productVariant.IsDownload = isDownload;
                productVariant.DownloadId = productVariantDownloadId;
                productVariant.UnlimitedDownloads = unlimitedDownloads;
                productVariant.MaxNumberOfDownloads = maxNumberOfDownloads;
                productVariant.DownloadExpirationDays = downloadExpirationDays;
                productVariant.DownloadActivationType = (int)downloadActivationType;
                productVariant.HasSampleDownload = hasSampleDownload;
                productVariant.SampleDownloadId = productVariantSampleDownloadId;
                productVariant.HasUserAgreement = hasUserAgreement;
                productVariant.UserAgreementText = userAgreementText;
                productVariant.IsRecurring = isRecurring;
                productVariant.CycleLength = cycleLength;
                productVariant.CyclePeriod = (int)cyclePeriod;
                productVariant.TotalCycles = totalCycles;
                productVariant.IsShipEnabled = isShipEnabled;
                productVariant.IsFreeShipping = isFreeShipping;
                productVariant.AdditionalShippingCharge = additionalShippingCharge;
                productVariant.IsTaxExempt = isTaxExempt;
                productVariant.TaxCategoryId = taxCategoryId;
                productVariant.ManageInventory = manageStock;
                productVariant.StockQuantity = stockQuantity;
                productVariant.DisplayStockAvailability = displayStockAvailability;
                productVariant.DisplayStockQuantity = displayStockQuantity;
                productVariant.MinStockQuantity = minStockQuantity;
                productVariant.LowStockActivityId = (int)lowStockActivity;
                productVariant.NotifyAdminForQuantityBelow = notifyForQuantityBelow;
                productVariant.Backorders = backorders;
                productVariant.OrderMinimumQuantity = orderMinimumQuantity;
                productVariant.OrderMaximumQuantity = orderMaximumQuantity;
                productVariant.WarehouseId = warehouseId;
                productVariant.DisableBuyButton = disableBuyButton;
                productVariant.CallForPrice = callForPrice;
                productVariant.Price = price;
                productVariant.OldPrice = oldPrice;
                productVariant.ProductCost = productCost;
                productVariant.CustomerEntersPrice = customerEntersPrice;
                productVariant.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
                productVariant.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
                productVariant.Weight = weight;
                productVariant.Length = length;
                productVariant.Width = width;
                productVariant.Height = height;
                productVariant.PictureId = productVariantPictureId;
                productVariant.AvailableStartDateTime = availableStartDateTime;
                productVariant.AvailableEndDateTime = availableEndDateTime;
                productVariant.Published = published;
                productVariant.DisplayOrder = displayOrder;
                productVariant.UpdatedOn = nowDT;

                IoCFactory.Resolve<IProductManager>().UpdateProductVariant(productVariant);

                #endregion
            }
            else
            {
                #region Insert
                Product product = IoCFactory.Resolve<IProductManager>().GetProductById(this.ProductId);
                if (product != null)
                {
                    Picture productVariantPicture = null;
                    HttpPostedFile productVariantPictureFile = fuProductVariantPicture.PostedFile;
                    if ((productVariantPictureFile != null) && (!String.IsNullOrEmpty(productVariantPictureFile.FileName)))
                    {
                        byte[] productVariantPictureBinary = IoCFactory.Resolve<IPictureManager>().GetPictureBits(productVariantPictureFile.InputStream, productVariantPictureFile.ContentLength);
                        productVariantPicture = IoCFactory.Resolve<IPictureManager>().InsertPicture(productVariantPictureBinary, productVariantPictureFile.ContentType, true);
                    }
                    int productVariantPictureId = 0;
                    if (productVariantPicture != null)
                        productVariantPictureId = productVariantPicture.PictureId;

                    int productVariantDownloadId = 0;
                    if (isDownload)
                    {
                        bool useDownloadURL = cbUseDownloadURL.Checked;
                        string downloadURL = txtDownloadURL.Text.Trim();
                        byte[] productVariantDownloadBinary = null;
                        string downloadContentType = string.Empty;
                        string downloadFilename = string.Empty;
                        string downloadExtension = string.Empty;

                        HttpPostedFile productVariantDownloadFile = fuProductVariantDownload.PostedFile;
                        if ((productVariantDownloadFile != null) && (!String.IsNullOrEmpty(productVariantDownloadFile.FileName)))
                        {
                            productVariantDownloadBinary = IoCFactory.Resolve<IDownloadManager>().GetDownloadBits(productVariantDownloadFile.InputStream, productVariantDownloadFile.ContentLength);
                            downloadContentType = productVariantDownloadFile.ContentType;
                            downloadFilename = Path.GetFileNameWithoutExtension(productVariantDownloadFile.FileName);
                            downloadExtension = Path.GetExtension(productVariantDownloadFile.FileName);
                        }


                        var productVariantDownload = new Download()
                        {
                            UseDownloadUrl = useDownloadURL,
                            DownloadUrl = downloadURL,
                            DownloadBinary = productVariantDownloadBinary,
                            ContentType = downloadContentType,
                            Filename = downloadFilename,
                            Extension = downloadExtension,
                            IsNew = true
                        };
                        IoCFactory.Resolve<IDownloadManager>().InsertDownload(productVariantDownload);
                        productVariantDownloadId = productVariantDownload.DownloadId;
                    }

                    int productVariantSampleDownloadId = 0;
                    if (hasSampleDownload)
                    {
                        bool useSampleDownloadURL = cbUseSampleDownloadURL.Checked;
                        string sampleDownloadURL = txtSampleDownloadURL.Text.Trim();
                        byte[] productVariantSampleDownloadBinary = null;
                        string sampleDownloadContentType = string.Empty;
                        string sampleDownloadFilename = string.Empty;
                        string sampleDownloadExtension = string.Empty;

                        HttpPostedFile productVariantSampleDownloadFile = fuProductVariantSampleDownload.PostedFile;
                        if ((productVariantSampleDownloadFile != null) && (!String.IsNullOrEmpty(productVariantSampleDownloadFile.FileName)))
                        {
                            productVariantSampleDownloadBinary = IoCFactory.Resolve<IDownloadManager>().GetDownloadBits(productVariantSampleDownloadFile.InputStream, productVariantSampleDownloadFile.ContentLength);
                            sampleDownloadContentType = productVariantSampleDownloadFile.ContentType;
                            sampleDownloadFilename = Path.GetFileNameWithoutExtension(productVariantSampleDownloadFile.FileName);
                            sampleDownloadExtension = Path.GetExtension(productVariantSampleDownloadFile.FileName);
                        }

                        var productVariantSampleDownload = new Download()
                        {
                            UseDownloadUrl = useSampleDownloadURL,
                            DownloadUrl = sampleDownloadURL,
                            DownloadBinary = productVariantSampleDownloadBinary,
                            ContentType = sampleDownloadContentType,
                            Filename = sampleDownloadFilename,
                            Extension = sampleDownloadExtension,
                            IsNew = true
                        };
                        IoCFactory.Resolve<IDownloadManager>().InsertDownload(productVariantSampleDownload);
                        productVariantSampleDownloadId = productVariantSampleDownload.DownloadId;
                    }

                    productVariant = new ProductVariant()
                    {
                        ProductId = product.ProductId,
                        Name = name,
                        SKU = sku,
                        Description = description,
                        AdminComment = adminComment,
                        ManufacturerPartNumber = manufacturerPartNumber,
                        IsGiftCard = isGiftCard,
                        GiftCardType = giftCardType,
                        IsDownload = isDownload,
                        DownloadId = productVariantDownloadId,
                        UnlimitedDownloads = unlimitedDownloads,
                        MaxNumberOfDownloads = maxNumberOfDownloads,
                        DownloadExpirationDays = downloadExpirationDays,
                        DownloadActivationType = (int)downloadActivationType,
                        HasSampleDownload = hasSampleDownload,
                        SampleDownloadId = productVariantSampleDownloadId,
                        HasUserAgreement = hasUserAgreement,
                        UserAgreementText = userAgreementText,
                        IsRecurring = isRecurring,
                        CycleLength = cycleLength,
                        CyclePeriod = (int)cyclePeriod,
                        TotalCycles = totalCycles,
                        IsShipEnabled = isShipEnabled,
                        IsFreeShipping = isFreeShipping,
                        AdditionalShippingCharge = additionalShippingCharge,
                        IsTaxExempt = isTaxExempt,
                        TaxCategoryId = taxCategoryId,
                        ManageInventory = manageStock,
                        StockQuantity = stockQuantity,
                        DisplayStockAvailability = displayStockAvailability,
                        DisplayStockQuantity = displayStockQuantity,
                        MinStockQuantity = minStockQuantity,
                        LowStockActivityId = (int)lowStockActivity,
                        NotifyAdminForQuantityBelow = notifyForQuantityBelow,
                        Backorders = backorders,
                        OrderMinimumQuantity = orderMinimumQuantity,
                        OrderMaximumQuantity = orderMaximumQuantity,
                        WarehouseId = warehouseId,
                        DisableBuyButton = disableBuyButton,
                        CallForPrice = callForPrice,
                        Price = price,
                        OldPrice = oldPrice,
                        ProductCost = productCost,
                        CustomerEntersPrice = customerEntersPrice,
                        MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice,
                        MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice,
                        Weight = weight,
                        Length = length,
                        Width = width,
                        Height = height,
                        PictureId = productVariantPictureId,
                        AvailableStartDateTime = availableStartDateTime,
                        AvailableEndDateTime = availableEndDateTime,
                        Published = published,
                        Deleted = false,
                        DisplayOrder = displayOrder,
                        CreatedOn = nowDT,
                        UpdatedOn = nowDT
                    };

                    IoCFactory.Resolve<IProductManager>().InsertProductVariant(productVariant);
                }
                else
                {
                    Response.Redirect("Products.aspx");
                }
                #endregion
            }

            SaveLocalizableContent(productVariant);

            return productVariant;
        }

        protected void SaveLocalizableContent(ProductVariant productVariant)
        {
            if (productVariant == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                    var txtLocalizedDescription = (FCKeditor)item.FindControl("txtLocalizedDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string description = txtLocalizedDescription.Value;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description));

                    var content = IoCFactory.Resolve<IProductManager>().GetProductVariantLocalizedByProductVariantIdAndLanguageId(productVariant.ProductVariantId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = new ProductVariantLocalized()
                            {
                                ProductVariantId = productVariant.ProductVariantId,
                                LanguageId = languageId,
                                Name = name,
                                Description = description
                            };
                            IoCFactory.Resolve<IProductManager>().InsertProductVariantLocalized(content);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content.LanguageId = languageId;
                            content.Name = name;
                            content.Description = description;
                            IoCFactory.Resolve<IProductManager>().UpdateProductVariantLocalized(content);
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedName = (TextBox)e.Item.FindControl("txtLocalizedName");
                var txtLocalizedDescription = (FCKeditor)e.Item.FindControl("txtLocalizedDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = IoCFactory.Resolve<IProductManager>().GetProductVariantLocalizedByProductVariantIdAndLanguageId(this.ProductVariantId, languageId);

                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedDescription.Value = content.Description;
                }
            }
        }

        protected void btnRemoveProductVariantImage_Click(object sender, EventArgs e)
        {
            try
            {
                ProductVariant productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(this.ProductVariantId);
                if (productVariant != null)
                {
                    IoCFactory.Resolve<IPictureManager>().DeletePicture(productVariant.PictureId);
                    IoCFactory.Resolve<IProductManager>().RemoveProductVariantPicture(productVariant.ProductVariantId);
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnRemoveProductVariantDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ProductVariant productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(this.ProductVariantId);
                if (productVariant != null)
                {
                    Download download = productVariant.Download;
                    if (download != null)
                    {
                        download.DownloadBinary = null;
                        download.ContentType = string.Empty;
                        download.Filename = string.Empty;
                        download.Extension = string.Empty;
                        download.IsNew = true;
                        IoCFactory.Resolve<IDownloadManager>().UpdateDownload(download);
                    }
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnRemoveProductVariantSampleDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ProductVariant productVariant = IoCFactory.Resolve<IProductManager>().GetProductVariantById(this.ProductVariantId);
                if (productVariant != null)
                {
                    Download download = productVariant.SampleDownload;
                    if (download != null)
                    {
                        download.DownloadBinary = null;
                        download.ContentType = string.Empty;
                        download.Filename = string.Empty;
                        download.Extension = string.Empty;
                        download.IsNew = true;
                        IoCFactory.Resolve<IDownloadManager>().UpdateDownload(download);
                    }
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }

        public int ProductVariantId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductVariantId");
            }
        }
    }
}
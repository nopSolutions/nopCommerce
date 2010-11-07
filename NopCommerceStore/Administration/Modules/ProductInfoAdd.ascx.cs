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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using FredCK.FCKeditorV2;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductInfoAddControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }
        }

        private void FillDropDowns()
        {
            CommonHelper.FillDropDownWithEnum(this.ddlGiftCardType, typeof(GiftCardTypeEnum));

            CommonHelper.FillDropDownWithEnum(this.ddlDownloadActivationType, typeof(DownloadActivationTypeEnum));

            CommonHelper.FillDropDownWithEnum(this.ddlCyclePeriod, typeof(RecurringProductCyclePeriodEnum));
            
            //template
            this.ddlTemplate.Items.Clear();
            var productTemplateCollection = IoCFactory.Resolve<ITemplateService>().GetAllProductTemplates();
            foreach (ProductTemplate productTemplate in productTemplateCollection)
            {
                ListItem item2 = new ListItem(productTemplate.Name, productTemplate.ProductTemplateId.ToString());
                this.ddlTemplate.Items.Add(item2);
            }

            //tax categories
            this.ddlTaxCategory.Items.Clear();
            ListItem itemTaxCategory = new ListItem("---", "0");
            this.ddlTaxCategory.Items.Add(itemTaxCategory);
            var taxCategoryCollection = IoCFactory.Resolve<ITaxCategoryService>().GetAllTaxCategories();
            foreach (TaxCategory taxCategory in taxCategoryCollection)
            {
                ListItem item2 = new ListItem(taxCategory.Name, taxCategory.TaxCategoryId.ToString());
                this.ddlTaxCategory.Items.Add(item2);
            }

            //warehouses
            this.ddlWarehouse.Items.Clear();
            ListItem itemWarehouse = new ListItem("---", "0");
            this.ddlWarehouse.Items.Add(itemWarehouse);
            var warehouseCollection = IoCFactory.Resolve<IWarehouseService>().GetAllWarehouses();
            foreach (Warehouse warehouse in warehouseCollection)
            {
                ListItem item2 = new ListItem(warehouse.Name, warehouse.WarehouseId.ToString());
                this.ddlWarehouse.Items.Add(item2);
            }


            //Low stock activity
            this.ddlLowStockActivity.Items.Clear();
            LowStockActivityEnum[] lowStockActivities = (LowStockActivityEnum[])Enum.GetValues(typeof(LowStockActivityEnum));
            foreach (LowStockActivityEnum lsa in lowStockActivities)
            {
                ListItem item2 = new ListItem(lsa.GetLowStockActivityName(), ((int)lsa).ToString());
                ddlLowStockActivity.Items.Add(item2);
            }
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

        private string[] ParseProductTags(string productTags)
        {
            List<string> result = new List<string>();
            string[] values = productTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string val1 in values)
            {
                if (!String.IsNullOrEmpty(val1.Trim()))
                {
                    result.Add(val1);
                }
            }
            return result.ToArray();
        }

        public Product SaveInfo()
        {
            DateTime nowDT = DateTime.UtcNow;

            string name = txtName.Text.Trim();
            string shortDescription = txtShortDescription.Text.Trim();
            string fullDescription = txtFullDescription.Value.Trim();
            string adminComment = txtAdminComment.Text.Trim();
            int templateId = int.Parse(this.ddlTemplate.SelectedItem.Value);
            bool showOnHomePage = cbShowOnHomePage.Checked;
            bool allowCustomerReviews = cbAllowCustomerReviews.Checked;
            bool allowCustomerRatings = cbAllowCustomerRatings.Checked;
            bool published = cbPublished.Checked;
            string sku = txtSKU.Text.Trim();
            string manufacturerPartNumber = txtManufacturerPartNumber.Text.Trim();
            bool isGiftCard = cbIsGiftCard.Checked;
            int giftCardType = int.Parse(this.ddlGiftCardType.SelectedItem.Value);
            bool isDownload = cbIsDownload.Checked;
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
                    productVariantDownloadBinary = productVariantDownloadFile.GetDownloadBits();
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
                IoCFactory.Resolve<IDownloadService>().InsertDownload(productVariantDownload);
                productVariantDownloadId = productVariantDownload.DownloadId;
            }

            bool unlimitedDownloads = cbUnlimitedDownloads.Checked;
            int maxNumberOfDownloads = txtMaxNumberOfDownloads.Value;
            int? downloadExpirationDays = null;
            if (!String.IsNullOrEmpty(txtDownloadExpirationDays.Text.Trim()))
                downloadExpirationDays = int.Parse(txtDownloadExpirationDays.Text.Trim());
            DownloadActivationTypeEnum downloadActivationType = (DownloadActivationTypeEnum)Enum.ToObject(typeof(DownloadActivationTypeEnum), int.Parse(this.ddlDownloadActivationType.SelectedItem.Value));
            bool hasUserAgreement = cbHasUserAgreement.Checked;
            string userAgreementText = txtUserAgreementText.Value;

            bool hasSampleDownload = cbHasSampleDownload.Checked;
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
                    productVariantSampleDownloadBinary = productVariantSampleDownloadFile.GetDownloadBits();
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
                IoCFactory.Resolve<IDownloadService>().InsertDownload(productVariantSampleDownload);
                productVariantSampleDownloadId = productVariantSampleDownload.DownloadId;
            }

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

            //product
            var product = new Product()
            {
                Name = name,
                ShortDescription = shortDescription,
                FullDescription = fullDescription,
                AdminComment = adminComment,
                TemplateId = templateId,
                ShowOnHomePage = showOnHomePage,
                AllowCustomerReviews = allowCustomerReviews,
                AllowCustomerRatings = allowCustomerRatings,
                Published = published,
                CreatedOn = nowDT,
                UpdatedOn = nowDT
            };

            IoCFactory.Resolve<IProductService>().InsertProduct(product);


            //product variant
            var productVariant = new ProductVariant()
            {
                ProductId = product.ProductId,
                SKU = sku,
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
                AvailableStartDateTime = availableStartDateTime,
                AvailableEndDateTime = availableEndDateTime,
                Published = published,
                Deleted = false,
                DisplayOrder = 1,
                CreatedOn = nowDT,
                UpdatedOn = nowDT
            };

            IoCFactory.Resolve<IProductService>().InsertProductVariant(productVariant);

            SaveLocalizableContent(product);

            //product tags
            var productTags1 = IoCFactory.Resolve<IProductService>().GetAllProductTags(product.ProductId, string.Empty);
            foreach (var productTag in productTags1)
            {
                IoCFactory.Resolve<IProductService>().RemoveProductTagMapping(product.ProductId, productTag.ProductTagId);
            }
            string[] productTagNames = ParseProductTags(txtProductTags.Text);
            foreach (string productTagName in productTagNames)
            {
                ProductTag productTag = null;
                var productTags2 = IoCFactory.Resolve<IProductService>().GetAllProductTags(0,
                    productTagName);
                if (productTags2.Count == 0)
                {
                    productTag = new ProductTag()
                    {
                        Name = productTagName,
                        ProductCount = 0
                    };
                    IoCFactory.Resolve<IProductService>().InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTags2[0];
                }
                IoCFactory.Resolve<IProductService>().AddProductTagMapping(product.ProductId, productTag.ProductTagId);
            }

            return product;
        }

        protected void SaveLocalizableContent(Product product)
        {
            if (product == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                    var txtLocalizedShortDescription = (TextBox)item.FindControl("txtLocalizedShortDescription");
                    var txtLocalizedFullDescription = (FCKeditor)item.FindControl("txtLocalizedFullDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string shortDescription = txtLocalizedShortDescription.Text;
                    string fullDescription = txtLocalizedFullDescription.Value;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) &&
                        string.IsNullOrEmpty(shortDescription) &&
                        string.IsNullOrEmpty(fullDescription));

                    var content = IoCFactory.Resolve<IProductService>().GetProductLocalizedByProductIdAndLanguageId(product.ProductId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = new ProductLocalized()
                            {
                                ProductId = product.ProductId,
                                LanguageId = languageId,
                                Name = name,
                                ShortDescription = shortDescription,
                                FullDescription = fullDescription
                            };
                            IoCFactory.Resolve<IProductService>().InsertProductLocalized(content);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content.LanguageId = languageId;
                            content.Name = name;
                            content.ShortDescription = shortDescription;
                            content.FullDescription = fullDescription;
                            IoCFactory.Resolve<IProductService>().UpdateProductLocalized(content);
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
                var txtLocalizedShortDescription = (TextBox)e.Item.FindControl("txtLocalizedShortDescription");
                var txtLocalizedFullDescription = (FCKeditor)e.Item.FindControl("txtLocalizedFullDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = IoCFactory.Resolve<IProductService>().GetProductLocalizedByProductIdAndLanguageId(this.ProductId, languageId);
                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedShortDescription.Text = content.ShortDescription;
                    txtLocalizedFullDescription.Value = content.FullDescription;
                }
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
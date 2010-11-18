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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class GlobalSettingsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SelectTab(this.CommonSettingsTabs, this.TabId);
                FillDropDowns();
                BindData();
            }
        }

        private void BindData()
        {
            txtStoreName.Text = this.SettingManager.StoreName;
            txtStoreURL.Text = this.SettingManager.StoreUrl;
            cbStoreClosed.Checked = this.SettingManager.GetSettingValueBoolean("Common.StoreClosed");
            cbStoreClosedForAdmins.Checked = this.SettingManager.GetSettingValueBoolean("Common.StoreClosed.AllowAdminAccess");
            cbAnonymousCheckoutAllowed.Checked = this.CustomerService.AnonymousCheckoutAllowed;
            cbUseOnePageCheckout.Checked = this.SettingManager.GetSettingValueBoolean("Checkout.UseOnePageCheckout");
            cbCheckoutTermsOfService.Checked = this.SettingManager.GetSettingValueBoolean("Checkout.TermsOfServiceEnabled");

            cbStoreNameInTitle.Checked = this.SettingManager.GetSettingValueBoolean("SEO.IncludeStoreNameInTitle");
            txtDefaulSEOTitle.Text = this.SettingManager.GetSettingValue("SEO.DefaultTitle");
            txtDefaulSEODescription.Text = this.SettingManager.GetSettingValue("SEO.DefaultMetaDescription");
            txtDefaulSEOKeywords.Text = this.SettingManager.GetSettingValue("SEO.DefaultMetaKeywords");
            cbConvertNonWesternChars.Checked = this.SettingManager.GetSettingValueBoolean("SEONames.ConvertNonWesternChars");
            cbAllowCustomerSelectTheme.Checked = this.SettingManager.GetSettingValueBoolean("Display.AllowCustomerSelectTheme");
            
            if (File.Exists(HttpContext.Current.Request.PhysicalApplicationPath + "favicon.ico"))
            {
                imgFavicon.ImageUrl = CommonHelper.GetStoreLocation() + "favicon.ico";
                imgFavicon.Visible = true;
                btnFaviconRemove.Visible = true;
            }
            else
            {
                imgFavicon.Visible = false;
                btnFaviconRemove.Visible = false;
            }
            cbShowWelcomeMessage.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowWelcomeMessageOnMainPage");
            cbShowNewsHeaderRssURL.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowNewsHeaderRssURL");
            cbShowBlogHeaderRssURL.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowBlogHeaderRssURL");
            cbEnableUrlRewriting.Checked = SEOHelper.EnableUrlRewriting;
            txtProductUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.Product.UrlRewriteFormat");
            cbProductCanonicalUrl.Checked = this.SettingManager.GetSettingValueBoolean("SEO.CanonicalURLs.Products.Enabled");
            txtCategoryUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.Category.UrlRewriteFormat");
            cbCategoryCanonicalUrl.Checked = this.SettingManager.GetSettingValueBoolean("SEO.CanonicalURLs.Category.Enabled");
            txtManufacturerUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.Manufacturer.UrlRewriteFormat");
            cbManufacturerCanonicalUrl.Checked = this.SettingManager.GetSettingValueBoolean("SEO.CanonicalURLs.Manufacturer.Enabled");
            txtProductTagUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.ProductTags.UrlRewriteFormat");
            txtNewsUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.News.UrlRewriteFormat");
            txtBlogUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.Blog.UrlRewriteFormat");
            txtTopicUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.Topic.UrlRewriteFormat");
            txtForumUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.Forum.UrlRewriteFormat");
            txtForumGroupUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.ForumGroup.UrlRewriteFormat");
            txtForumTopicUrlRewriteFormat.Text = this.SettingManager.GetSettingValue("SEO.ForumTopic.UrlRewriteFormat");


            lStoreImagesInDBStorage.Text = this.PictureService.StoreInDB ? GetLocaleResourceString("Admin.GlobalSettings.Media.StoreImagesInDB.DB") : GetLocaleResourceString("Admin.GlobalSettings.Media.StoreImagesInDB.FS");
            txtMaxImageSize.Value = this.SettingManager.GetSettingValueInteger("Media.MaximumImageSize");
            txtProductThumbSize.Value = this.SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize");
            txtProductDetailSize.Value = this.SettingManager.GetSettingValueInteger("Media.Product.DetailImageSize");
            txtProductVariantSize.Value = this.SettingManager.GetSettingValueInteger("Media.Product.VariantImageSize");
            txtCategoryThumbSize.Value = this.SettingManager.GetSettingValueInteger("Media.Category.ThumbnailImageSize");
            txtManufacturerThumbSize.Value = this.SettingManager.GetSettingValueInteger("Media.Manufacturer.ThumbnailImageSize");
            cbShowCartImages.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowProductImagesOnShoppingCart");
            cbShowWishListImages.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowProductImagesOnWishList");
            txtShoppingCartThumbSize.Value = this.SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize");
            cbShowAdminProductImages.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages");

            txtEncryptionPrivateKey.Text = this.SettingManager.GetSettingValue("Security.EncryptionPrivateKey");
            cbEnableLoginCaptchaImage.Checked = this.SettingManager.GetSettingValueBoolean("Common.LoginCaptchaImageEnabled");
            cbEnableRegisterCaptchaImage.Checked = this.SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled");
            
            CommonHelper.SelectListItem(this.ddlCustomerNameFormat, (int)this.CustomerService.CustomerNameFormatting);
            cbShowCustomersLocation.Checked = this.CustomerService.ShowCustomersLocation;
            cbShowCustomersJoinDate.Checked = this.CustomerService.ShowCustomersJoinDate;
            cbAllowPM.Checked = this.ForumService.AllowPrivateMessages;
            cbNotifyAboutPrivateMessages.Checked = this.ForumService.NotifyAboutPrivateMessages;
            cbAllowViewingProfiles.Checked = this.CustomerService.AllowViewingProfiles;
            cbCustomersAllowedToUploadAvatars.Checked = this.CustomerService.AllowCustomersToUploadAvatars;
            cbDefaultAvatarEnabled.Checked = this.CustomerService.DefaultAvatarEnabled;
            lblCurrentTimeZone.Text = DateTimeHelper.CurrentTimeZone.DisplayName;
            TimeZoneInfo defaultStoreTimeZone = DateTimeHelper.DefaultStoreTimeZone;
            if (defaultStoreTimeZone != null)
                CommonHelper.SelectListItem(this.ddlDefaultStoreTimeZone, defaultStoreTimeZone.Id);
            cbAllowCustomersToSetTimeZone.Checked = DateTimeHelper.AllowCustomersToSetTimeZone;


            cbUsernamesEnabled.Checked = this.CustomerService.UsernamesEnabled;
            cbAllowCustomersToChangeUsernames.Checked = this.CustomerService.AllowCustomersToChangeUsernames;
            cbNotifyAboutNewCustomerRegistration.Checked = this.CustomerService.NotifyNewCustomerRegistration;
            CommonHelper.SelectListItem(this.ddlRegistrationMethod, (int)this.CustomerService.CustomerRegistrationType);
            cbAllowNavigationOnlyRegisteredCustomers.Checked = this.CustomerService.AllowNavigationOnlyRegisteredCustomers;
            cbHideNewsletterBox.Checked = this.SettingManager.GetSettingValueBoolean("Display.HideNewsletterBox");

            cbShowCategoryProductNumber.Checked = this.SettingManager.GetSettingValueBoolean("Display.Products.ShowCategoryProductNumber");
            cbHidePricesForNonRegistered.Checked = this.SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered");
            txtMinOrderSubtotalAmount.Value = this.OrderService.MinOrderSubtotalAmount;
            txtMinOrderTotalAmount.Value = this.OrderService.MinOrderTotalAmount;
            cbShowDiscountCouponBox.Checked = this.SettingManager.GetSettingValueBoolean("Display.Checkout.DiscountCouponBox");
            cbShowGiftCardBox.Checked = this.SettingManager.GetSettingValueBoolean("Display.Checkout.GiftCardBox");
            cbShowSKU.Checked = this.SettingManager.GetSettingValueBoolean("Display.Products.ShowSKU");
            cbShowManufacturerPartNumber.Checked = this.SettingManager.GetSettingValueBoolean("Display.Products.ShowManufacturerPartNumber");
            cbDisplayCartAfterAddingProduct.Checked = this.SettingManager.GetSettingValueBoolean("Display.Products.DisplayCartAfterAddingProduct");
            cbEnableDynamicPriceUpdate.Checked = this.SettingManager.GetSettingValueBoolean("ProductAttribute.EnableDynamicPriceUpdate");
            cbAllowProductSorting.Checked = this.SettingManager.GetSettingValueBoolean("Common.AllowProductSorting");
            cbShowShareButton.Checked = this.ProductService.ShowShareButton;
            cbDownloadableProductsTab.Checked = this.SettingManager.GetSettingValueBoolean("Display.DownloadableProductsTab");
            cbUseImagesForLanguageSelection.Checked = this.SettingManager.GetSettingValueBoolean("Common.UseImagesForLanguageSelection", false);
            cbEnableCompareProducts.Checked = this.ProductService.CompareProductsEnabled;
            cbEnableWishlist.Checked = this.SettingManager.GetSettingValueBoolean("Common.EnableWishlist");
            cbEmailWishlist.Checked = this.SettingManager.GetSettingValueBoolean("Common.EmailWishlist");
            cbIsReOrderAllowed.Checked = this.OrderService.IsReOrderAllowed;
            cbEnableEmailAFriend.Checked = this.SettingManager.GetSettingValueBoolean("Common.EnableEmailAFirend");
            cbShowMiniShoppingCart.Checked = this.SettingManager.GetSettingValueBoolean("Common.ShowMiniShoppingCart");
            cbRecentlyViewedProductsEnabled.Checked = this.ProductService.RecentlyViewedProductsEnabled;
            txtRecentlyViewedProductsNumber.Value = this.ProductService.RecentlyViewedProductsNumber;
            cbRecentlyAddedProductsEnabled.Checked = this.ProductService.RecentlyAddedProductsEnabled;
            txtRecentlyAddedProductsNumber.Value = this.ProductService.RecentlyAddedProductsNumber;
            cbNotifyAboutNewProductReviews.Checked = this.ProductService.NotifyAboutNewProductReviews;
            cbProductReviewsMustBeApproved.Checked = this.CustomerService.ProductReviewsMustBeApproved;
            cbAllowAnonymousUsersToReviewProduct.Checked = this.CustomerService.AllowAnonymousUsersToReviewProduct;
            cbAllowAnonymousUsersToEmailAFriend.Checked = this.CustomerService.AllowAnonymousUsersToEmailAFriend;
            cbAllowAnonymousUsersToVotePolls.Checked = this.SettingManager.GetSettingValueBoolean("Common.AllowAnonymousUsersToVotePolls");

            cbAllowAnonymousUsersToSetProductRatings.Checked = this.CustomerService.AllowAnonymousUsersToSetProductRatings;
            cbShowBestsellersOnHomePage.Checked = this.SettingManager.GetSettingValueBoolean("Display.ShowBestsellersOnMainPage");
            txtShowBestsellersOnHomePageNumber.Value = this.SettingManager.GetSettingValueInteger("Display.ShowBestsellersOnMainPageNumber");
            cbProductsAlsoPurchased.Checked = this.ProductService.ProductsAlsoPurchasedEnabled;
            txtProductsAlsoPurchasedNumber.Value = this.ProductService.ProductsAlsoPurchasedNumber;
            txtCrossSellsNumber.Value = this.ProductService.CrossSellsNumber;
            txtSearchPageProductsPerPage.Value = this.ProductService.SearchPageProductsPerPage;
            txtMaxShoppingCartItems.Value = this.SettingManager.GetSettingValueInteger("Common.MaximumShoppingCartItems");
            txtMaxWishlistItems.Value = this.SettingManager.GetSettingValueInteger("Common.MaximumWishlistItems");

            cbLiveChatEnabled.Checked = this.SettingManager.GetSettingValueBoolean("LiveChat.Enabled", false);
            txtLiveChatBtnCode.Text = this.SettingManager.GetSettingValue("LiveChat.BtnCode");
            txtLiveChatMonCode.Text = this.SettingManager.GetSettingValue("LiveChat.MonCode");

            //Google Adsense
            cbGoogleAdsenseEnabled.Checked = this.SettingManager.GetSettingValueBoolean("GoogleAdsense.Enabled", false);
            txtGoogleAdsenseCode.Text = this.SettingManager.GetSettingValue("GoogleAdsense.Code");

            //Google Analytics
            cbGoogleAnalyticsEnabled.Checked = this.SettingManager.GetSettingValueBoolean("Analytics.GoogleEnabled", false);
            txtGoogleAnalyticsId.Text = this.SettingManager.GetSettingValue("Analytics.GoogleID");
            txtGoogleAnalyticsJS.Text = this.SettingManager.GetSettingValue("Analytics.GoogleJS");
            CommonHelper.SelectListItem(this.ddlGoogleAnalyticsPlacement, this.SettingManager.GetSettingValue("Analytics.GooglePlacement", "Body"));

            txtAllowedIPList.Text = this.SettingManager.GetSettingValue("Security.AdminAreaAllowedIP");

            if(File.Exists(PDFHelper.LogoFilePath))
            {
                imgPdfLogoPreview.ImageUrl = "~/images/pdflogo.img";
                btnPdfLogoRemove.Visible = true;
            }
            else
            {
                imgPdfLogoPreview.Visible = false;
            }

            //reward point
            cbRewardPointsEnabled.Checked = this.OrderService.RewardPointsEnabled;
            txtRewardPointsRate.Value = this.OrderService.RewardPointsExchangeRate;
            txtRewardPointsForRegistration.Value = this.OrderService.RewardPointsForRegistration;
            txtRewardPointsForPurchases_Amount.Value = this.OrderService.RewardPointsForPurchases_Amount;
            txtRewardPointsForPurchases_Points.Value = this.OrderService.RewardPointsForPurchases_Points;
            CommonHelper.SelectListItem(ddlRewardPointsAwardedOrderStatus, ((int)this.OrderService.RewardPointsForPurchases_Awarded).ToString());
            CommonHelper.SelectListItem(ddlRewardPointsCanceledOrderStatus, ((int)this.OrderService.RewardPointsForPurchases_Canceled).ToString());

            //gift cards
            if (this.OrderService.GiftCards_Activated.HasValue)
                CommonHelper.SelectListItem(ddlGiftCardsActivationOrderStatus, ((int)this.OrderService.GiftCards_Activated).ToString());
            else
                CommonHelper.SelectListItem(ddlGiftCardsActivationOrderStatus, 0);
            if (this.OrderService.GiftCards_Deactivated.HasValue)
                CommonHelper.SelectListItem(ddlGiftCardsDeactivationOrderStatus, ((int)this.OrderService.GiftCards_Deactivated).ToString());
            else
                CommonHelper.SelectListItem(ddlGiftCardsDeactivationOrderStatus, 0);

            //form fields
            cbffGenderEnabled.Checked = this.CustomerService.FormFieldGenderEnabled;
            cbffDateOfBirthEnabled.Checked = this.CustomerService.FormFieldDateOfBirthEnabled;
            cbffCompanyEnabled.Checked = this.CustomerService.FormFieldCompanyEnabled;
            cbffCompanyRequired.Checked = this.CustomerService.FormFieldCompanyRequired;
            cbffStreetAddressEnabled.Checked = this.CustomerService.FormFieldStreetAddressEnabled;
            cbffStreetAddressRequired.Checked = this.CustomerService.FormFieldStreetAddressRequired;
            cbffStreetAddress2Enabled.Checked = this.CustomerService.FormFieldStreetAddress2Enabled;
            cbffStreetAddress2Required.Checked = this.CustomerService.FormFieldStreetAddress2Required;
            cbffPostCodeEnabled.Checked = this.CustomerService.FormFieldPostCodeEnabled;
            cbffPostCodeRequired.Checked = this.CustomerService.FormFieldPostCodeRequired;
            cbffCityEnabled.Checked = this.CustomerService.FormFieldCityEnabled;
            cbffCityRequired.Checked = this.CustomerService.FormFieldCityRequired;
            cbffCountryEnabled.Checked = this.CustomerService.FormFieldCountryEnabled;
            cbffStateEnabled.Checked = this.CustomerService.FormFieldStateEnabled;
            cbffPhoneEnabled.Checked = this.CustomerService.FormFieldPhoneEnabled;
            cbffPhoneRequired.Checked = this.CustomerService.FormFieldPhoneRequired;
            cbffFaxEnabled.Checked = this.CustomerService.FormFieldFaxEnabled;
            cbffFaxRequired.Checked = this.CustomerService.FormFieldFaxRequired;
            cbffNewsletterBoxEnabled.Checked = this.CustomerService.FormFieldNewsletterEnabled;
            cbffTimeZoneEnabled.Checked = this.CustomerService.FormFieldTimeZoneEnabled;

            //return requests (RMA)
            cbReturnRequestsEnabled.Checked = this.SettingManager.GetSettingValueBoolean("ReturnRequests.Enable");
            txtReturnReasons.Text = this.SettingManager.GetSettingValue("ReturnRequests.ReturnReasons");
            txtReturnActions.Text = this.SettingManager.GetSettingValue("ReturnRequests.ReturnActions");

            cbDisplayPageExecutionTime.Checked = this.SettingManager.GetSettingValueBoolean("Display.PageExecutionTimeInfoEnabled");
        }

        private void FillDropDowns()
        {
            OrderStatusEnum[] orderStatuses = (OrderStatusEnum[])Enum.GetValues(typeof(OrderStatusEnum));            
            

            //reward points
            this.ddlRewardPointsAwardedOrderStatus.Items.Clear();
            foreach (OrderStatusEnum orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(orderStatus.GetOrderStatusName(), ((int)orderStatus).ToString());
                this.ddlRewardPointsAwardedOrderStatus.Items.Add(item2);
            }
            this.ddlRewardPointsCanceledOrderStatus.Items.Clear();
            foreach (OrderStatusEnum orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(orderStatus.GetOrderStatusName(), ((int)orderStatus).ToString());
                this.ddlRewardPointsCanceledOrderStatus.Items.Add(item2);
            }

            //gift cards
            this.ddlGiftCardsActivationOrderStatus.Items.Clear();
            ListItem gcaosEmpty = new ListItem("---", "0");
            this.ddlGiftCardsActivationOrderStatus.Items.Add(gcaosEmpty);
            foreach (OrderStatusEnum orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(orderStatus.GetOrderStatusName(), ((int)orderStatus).ToString());
                this.ddlGiftCardsActivationOrderStatus.Items.Add(item2);
            }
            this.ddlGiftCardsDeactivationOrderStatus.Items.Clear();
            ListItem gcdosEmpty = new ListItem("---", "0");
            this.ddlGiftCardsDeactivationOrderStatus.Items.Add(gcdosEmpty);
            foreach (OrderStatusEnum orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(orderStatus.GetOrderStatusName(), ((int)orderStatus).ToString());
                this.ddlGiftCardsDeactivationOrderStatus.Items.Add(item2);
            }

            //timezones
            this.ddlDefaultStoreTimeZone.Items.Clear();
            var timeZones = DateTimeHelper.GetSystemTimeZones();
            foreach (TimeZoneInfo timeZone in timeZones)
            {
                string timeZoneName = timeZone.DisplayName;
                ListItem ddlDefaultStoreTimeZoneItem2 = new ListItem(timeZoneName, timeZone.Id);
                this.ddlDefaultStoreTimeZone.Items.Add(ddlDefaultStoreTimeZoneItem2);
            }

            //etc
            CommonHelper.FillDropDownWithEnum(this.ddlCustomerNameFormat, typeof(CustomerNameFormatEnum));
            CommonHelper.FillDropDownWithEnum(this.ddlRegistrationMethod, typeof(CustomerRegistrationTypeEnum));
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            
            this.cbStoreClosed.Attributes.Add("onclick", "toggleStoreClosed();");
            this.cbEnableWishlist.Attributes.Add("onclick", "toggleWishlist();");            
            this.cbEnableUrlRewriting.Attributes.Add("onclick", "toggleUrlRewriting();");
            this.cbCustomersAllowedToUploadAvatars.Attributes.Add("onclick", "toggleCustomersAllowedToUploadAvatars();");
            this.cbAllowPM.Attributes.Add("onclick", "togglePM();");
            this.cbProductsAlsoPurchased.Attributes.Add("onclick", "toggleProductsAlsoPurchased();");
            this.cbRecentlyViewedProductsEnabled.Attributes.Add("onclick", "toggleRecentlyViewedProducts();");
            this.cbRecentlyAddedProductsEnabled.Attributes.Add("onclick", "toggleRecentlyAddedProducts();");
            this.cbShowBestsellersOnHomePage.Attributes.Add("onclick", "toggleShowBestsellersOnHomePage();");
            this.cbLiveChatEnabled.Attributes.Add("onclick", "toggleLiveChat();");
            this.cbGoogleAdsenseEnabled.Attributes.Add("onclick", "toggleGoogleAdsense();");
            this.cbGoogleAnalyticsEnabled.Attributes.Add("onclick", "toggleGoogleAnalytics();");
            this.cbUsernamesEnabled.Attributes.Add("onclick", "toggleUsernames();");
            
            this.btnStoreImagesInDBToggle.Attributes.Add("onclick", "this.disabled = true;" + Page.ClientScript.GetPostBackEventReference(this.btnStoreImagesInDBToggle, ""));
            
            base.OnPreRender(e);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    // Check IP list
                    string ipList = txtAllowedIPList.Text.Trim();
                    if (!String.IsNullOrEmpty(ipList))
                    {
                        foreach (string s in ipList.Split(new char[1] { ',' }))
                        {
                            if (!this.BlacklistService.IsValidIp(s.Trim()))
                            {
                                throw new NopException("IP list is not valid.");
                            }
                        }
                    }

                    this.SettingManager.StoreName = txtStoreName.Text;
                    this.SettingManager.StoreUrl = txtStoreURL.Text;
                    this.SettingManager.SetParam("Common.StoreClosed", cbStoreClosed.Checked.ToString());
                    this.SettingManager.SetParam("Common.StoreClosed.AllowAdminAccess", cbStoreClosedForAdmins.Checked.ToString());
                    this.CustomerService.AnonymousCheckoutAllowed = cbAnonymousCheckoutAllowed.Checked;
                    this.SettingManager.SetParam("Checkout.UseOnePageCheckout", cbUseOnePageCheckout.Checked.ToString());
                    this.SettingManager.SetParam("Checkout.TermsOfServiceEnabled", cbCheckoutTermsOfService.Checked.ToString());

                    this.SettingManager.SetParam("SEO.IncludeStoreNameInTitle", cbStoreNameInTitle.Checked.ToString());
                    this.SettingManager.SetParam("SEO.DefaultTitle", txtDefaulSEOTitle.Text);
                    this.SettingManager.SetParam("SEO.DefaultMetaDescription", txtDefaulSEODescription.Text);
                    this.SettingManager.SetParam("SEO.DefaultMetaKeywords", txtDefaulSEOKeywords.Text);
                    this.SettingManager.SetParam("SEONames.ConvertNonWesternChars", cbConvertNonWesternChars.Checked.ToString());

                    this.SettingManager.SetParam("Display.PublicStoreTheme", ctrlThemeSelector.SelectedTheme);
                    this.SettingManager.SetParam("Display.AllowCustomerSelectTheme", cbAllowCustomerSelectTheme.Checked.ToString());
                    
                    if (fileFavicon.HasFile)
                    {
                        HttpPostedFile postedFile = fileFavicon.PostedFile;
                        if (!postedFile.ContentType.Equals("image/x-icon") &&
                            !postedFile.ContentType.Equals("image/icon"))
                        {
                            throw new NopException("Image format not recognized, allowed formats are: .ico");
                        }
                        postedFile.SaveAs(HttpContext.Current.Request.PhysicalApplicationPath + "favicon.ico");
                    }
                    this.SettingManager.SetParam("Display.ShowWelcomeMessageOnMainPage", cbShowWelcomeMessage.Checked.ToString());
                    this.SettingManager.SetParam("Display.ShowNewsHeaderRssURL", cbShowNewsHeaderRssURL.Checked.ToString());
                    this.SettingManager.SetParam("Display.ShowBlogHeaderRssURL", cbShowBlogHeaderRssURL.Checked.ToString());
                    SEOHelper.EnableUrlRewriting = cbEnableUrlRewriting.Checked;
                    this.SettingManager.SetParam("SEO.Product.UrlRewriteFormat", txtProductUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.CanonicalURLs.Products.Enabled", cbProductCanonicalUrl.Checked.ToString());
                    this.SettingManager.SetParam("SEO.Category.UrlRewriteFormat", txtCategoryUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.CanonicalURLs.Category.Enabled", cbCategoryCanonicalUrl.Checked.ToString());
                    this.SettingManager.SetParam("SEO.Manufacturer.UrlRewriteFormat", txtManufacturerUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.CanonicalURLs.Manufacturer.Enabled", cbManufacturerCanonicalUrl.Checked.ToString());
                    this.SettingManager.SetParam("SEO.ProductTags.UrlRewriteFormat", txtProductTagUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.News.UrlRewriteFormat", txtNewsUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.Blog.UrlRewriteFormat", txtBlogUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.Topic.UrlRewriteFormat", txtTopicUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.Forum.UrlRewriteFormat", txtForumUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.ForumGroup.UrlRewriteFormat", txtForumGroupUrlRewriteFormat.Text);
                    this.SettingManager.SetParam("SEO.ForumTopic.UrlRewriteFormat", txtForumTopicUrlRewriteFormat.Text);


                    this.SettingManager.SetParam("Media.MaximumImageSize", txtMaxImageSize.Value.ToString());
                    this.SettingManager.SetParam("Media.Product.ThumbnailImageSize", txtProductThumbSize.Value.ToString());
                    this.SettingManager.SetParam("Media.Product.DetailImageSize", txtProductDetailSize.Value.ToString());
                    this.SettingManager.SetParam("Media.Product.VariantImageSize", txtProductVariantSize.Value.ToString());
                    this.SettingManager.SetParam("Media.Category.ThumbnailImageSize", txtCategoryThumbSize.Value.ToString());
                    this.SettingManager.SetParam("Media.Manufacturer.ThumbnailImageSize", txtManufacturerThumbSize.Value.ToString());
                    this.SettingManager.SetParam("Display.ShowProductImagesOnShoppingCart", cbShowCartImages.Checked.ToString());
                    this.SettingManager.SetParam("Display.ShowProductImagesOnWishList", cbShowWishListImages.Checked.ToString());
                    this.SettingManager.SetParam("Media.ShoppingCart.ThumbnailImageSize", txtShoppingCartThumbSize.Value.ToString());
                    this.SettingManager.SetParam("Display.ShowAdminProductImages", cbShowAdminProductImages.Checked.ToString());

                    this.SettingManager.SetParam("Security.AdminAreaAllowedIP", ipList);
                    this.SettingManager.SetParam("Common.LoginCaptchaImageEnabled", cbEnableLoginCaptchaImage.Checked.ToString());
                    this.SettingManager.SetParam("Common.RegisterCaptchaImageEnabled", cbEnableRegisterCaptchaImage.Checked.ToString());


                    this.CustomerService.CustomerNameFormatting = (CustomerNameFormatEnum)Enum.ToObject(typeof(CustomerNameFormatEnum), int.Parse(this.ddlCustomerNameFormat.SelectedItem.Value));
                    this.CustomerService.ShowCustomersLocation = cbShowCustomersLocation.Checked;
                    this.CustomerService.ShowCustomersJoinDate = cbShowCustomersJoinDate.Checked;
                    this.ForumService.AllowPrivateMessages = cbAllowPM.Checked;
                    this.ForumService.NotifyAboutPrivateMessages = cbNotifyAboutPrivateMessages.Checked;
                    this.CustomerService.AllowViewingProfiles = cbAllowViewingProfiles.Checked;
                    this.CustomerService.AllowCustomersToUploadAvatars = cbCustomersAllowedToUploadAvatars.Checked;
                    this.CustomerService.DefaultAvatarEnabled = cbDefaultAvatarEnabled.Checked;
                    string defaultStoreTimeZoneId = ddlDefaultStoreTimeZone.SelectedItem.Value;
                    DateTimeHelper.DefaultStoreTimeZone = DateTimeHelper.FindTimeZoneById(defaultStoreTimeZoneId);
                    DateTimeHelper.AllowCustomersToSetTimeZone = cbAllowCustomersToSetTimeZone.Checked;


                    this.CustomerService.UsernamesEnabled = cbUsernamesEnabled.Checked;
                    this.CustomerService.AllowCustomersToChangeUsernames = cbAllowCustomersToChangeUsernames.Checked;
                    this.CustomerService.NotifyNewCustomerRegistration = cbNotifyAboutNewCustomerRegistration.Checked;
                    this.CustomerService.CustomerRegistrationType = (CustomerRegistrationTypeEnum)Enum.ToObject(typeof(CustomerRegistrationTypeEnum), int.Parse(this.ddlRegistrationMethod.SelectedItem.Value));
                    this.CustomerService.AllowNavigationOnlyRegisteredCustomers = cbAllowNavigationOnlyRegisteredCustomers.Checked;
                    this.SettingManager.SetParam("Display.HideNewsletterBox", cbHideNewsletterBox.Checked.ToString());

                    this.SettingManager.SetParam("Display.Products.ShowCategoryProductNumber", cbShowCategoryProductNumber.Checked.ToString());
                    this.SettingManager.SetParam("Common.HidePricesForNonRegistered", cbHidePricesForNonRegistered.Checked.ToString());
                    this.OrderService.MinOrderSubtotalAmount = txtMinOrderSubtotalAmount.Value;
                    this.OrderService.MinOrderTotalAmount = txtMinOrderTotalAmount.Value;
                    this.SettingManager.SetParam("Display.Checkout.DiscountCouponBox", cbShowDiscountCouponBox.Checked.ToString());
                    this.SettingManager.SetParam("Display.Checkout.GiftCardBox", cbShowGiftCardBox.Checked.ToString());
                    this.SettingManager.SetParam("Display.Products.ShowSKU", cbShowSKU.Checked.ToString());
                    this.SettingManager.SetParam("Display.Products.ShowManufacturerPartNumber", cbShowManufacturerPartNumber.Checked.ToString());
                    this.SettingManager.SetParam("Display.Products.DisplayCartAfterAddingProduct", cbDisplayCartAfterAddingProduct.Checked.ToString());
                    this.SettingManager.SetParam("ProductAttribute.EnableDynamicPriceUpdate", cbEnableDynamicPriceUpdate.Checked.ToString());
                    this.SettingManager.SetParam("Common.AllowProductSorting", cbAllowProductSorting.Checked.ToString());
                    this.ProductService.ShowShareButton = cbShowShareButton.Checked;
                    this.SettingManager.SetParam("Display.DownloadableProductsTab", cbDownloadableProductsTab.Checked.ToString());
                    this.SettingManager.SetParam("Common.UseImagesForLanguageSelection", cbUseImagesForLanguageSelection.Checked.ToString());
                    this.ProductService.CompareProductsEnabled = cbEnableCompareProducts.Checked;
                    this.SettingManager.SetParam("Common.EnableWishlist", cbEnableWishlist.Checked.ToString());
                    this.SettingManager.SetParam("Common.EmailWishlist", cbEmailWishlist.Checked.ToString());
                    this.OrderService.IsReOrderAllowed = cbIsReOrderAllowed.Checked;
                    this.SettingManager.SetParam("Common.EnableEmailAFirend", cbEnableEmailAFriend.Checked.ToString());
                    this.SettingManager.SetParam("Common.ShowMiniShoppingCart", cbShowMiniShoppingCart.Checked.ToString());
                    this.ProductService.RecentlyViewedProductsEnabled = cbRecentlyViewedProductsEnabled.Checked;
                    this.ProductService.RecentlyViewedProductsNumber = txtRecentlyViewedProductsNumber.Value;
                    this.ProductService.RecentlyAddedProductsEnabled = cbRecentlyAddedProductsEnabled.Checked;
                    this.ProductService.RecentlyAddedProductsNumber = txtRecentlyAddedProductsNumber.Value;
                    this.ProductService.NotifyAboutNewProductReviews = cbNotifyAboutNewProductReviews.Checked;
                    this.CustomerService.ProductReviewsMustBeApproved = cbProductReviewsMustBeApproved.Checked;
                    this.CustomerService.AllowAnonymousUsersToReviewProduct = cbAllowAnonymousUsersToReviewProduct.Checked;
                    this.CustomerService.AllowAnonymousUsersToEmailAFriend = cbAllowAnonymousUsersToEmailAFriend.Checked;
                    this.SettingManager.SetParam("Common.AllowAnonymousUsersToVotePolls", cbAllowAnonymousUsersToVotePolls.Checked.ToString());

                    this.CustomerService.AllowAnonymousUsersToSetProductRatings = cbAllowAnonymousUsersToSetProductRatings.Checked;
                    this.SettingManager.SetParam("Display.ShowBestsellersOnMainPage", cbShowBestsellersOnHomePage.Checked.ToString());
                    this.SettingManager.SetParam("Display.ShowBestsellersOnMainPageNumber", txtShowBestsellersOnHomePageNumber.Value.ToString());
                    this.ProductService.ProductsAlsoPurchasedEnabled = cbProductsAlsoPurchased.Checked;
                    this.ProductService.ProductsAlsoPurchasedNumber = txtProductsAlsoPurchasedNumber.Value;
                    this.ProductService.CrossSellsNumber = txtCrossSellsNumber.Value;
                    this.ProductService.SearchPageProductsPerPage = txtSearchPageProductsPerPage.Value;
                    this.SettingManager.SetParam("Common.MaximumShoppingCartItems", txtMaxShoppingCartItems.Value.ToString());
                    this.SettingManager.SetParam("Common.MaximumWishlistItems", txtMaxWishlistItems.Value.ToString());
                    
                    this.SettingManager.SetParam("LiveChat.Enabled", cbLiveChatEnabled.Checked.ToString());
                    this.SettingManager.SetParam("LiveChat.BtnCode", txtLiveChatBtnCode.Text);
                    this.SettingManager.SetParam("LiveChat.MonCode", txtLiveChatMonCode.Text);

                    //Google Adsense
                    this.SettingManager.SetParam("GoogleAdsense.Enabled", cbGoogleAdsenseEnabled.Checked.ToString());
                    this.SettingManager.SetParam("GoogleAdsense.Code", txtGoogleAdsenseCode.Text);

                    //Google Analytics
                    this.SettingManager.SetParam("Analytics.GoogleEnabled", cbGoogleAnalyticsEnabled.Checked.ToString());
                    this.SettingManager.SetParam("Analytics.GoogleID", txtGoogleAnalyticsId.Text);
                    this.SettingManager.SetParam("Analytics.GoogleJS", txtGoogleAnalyticsJS.Text);
                    this.SettingManager.SetParam("Analytics.GooglePlacement", this.ddlGoogleAnalyticsPlacement.SelectedItem.Value);


                    if (uplPdfLogo.HasFile)
                    {
                        HttpPostedFile postedFile = uplPdfLogo.PostedFile;
                        if (!postedFile.ContentType.Equals("image/jpeg") && !postedFile.ContentType.Equals("image/gif") && !postedFile.ContentType.Equals("image/png"))
                        {
                            throw new NopException("Image format not recognized, allowed formats are: .png, .jpg, .jpeg, .gif");
                        }
                        postedFile.SaveAs(PDFHelper.LogoFilePath);
                    }

                    //reward point
                    this.OrderService.RewardPointsEnabled = cbRewardPointsEnabled.Checked;
                    this.OrderService.RewardPointsExchangeRate = txtRewardPointsRate.Value;
                    this.OrderService.RewardPointsForRegistration = txtRewardPointsForRegistration.Value;
                    this.OrderService.RewardPointsForPurchases_Amount = txtRewardPointsForPurchases_Amount.Value;
                    this.OrderService.RewardPointsForPurchases_Points = txtRewardPointsForPurchases_Points.Value;
                    OrderStatusEnum rppa = (OrderStatusEnum)int.Parse(ddlRewardPointsAwardedOrderStatus.SelectedItem.Value);
                    if (rppa != OrderStatusEnum.Pending)
                    {
                        this.OrderService.RewardPointsForPurchases_Awarded = rppa;
                    }
                    else
                    {
                        //ensure that order status is not pending
                        throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                    }
                    OrderStatusEnum rppc = (OrderStatusEnum)int.Parse(ddlRewardPointsCanceledOrderStatus.SelectedItem.Value);
                    if (rppc != OrderStatusEnum.Pending)
                    {
                        this.OrderService.RewardPointsForPurchases_Canceled = rppc;
                    }
                    else
                    {
                        //ensure that order status is not pending
                        throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                    }

                    //gift cards
                    int gcaos = int.Parse(ddlGiftCardsActivationOrderStatus.SelectedItem.Value);
                    if (gcaos > 0)
                    {
                        if ((OrderStatusEnum)gcaos != OrderStatusEnum.Pending)
                        {
                            this.OrderService.GiftCards_Activated = (OrderStatusEnum)gcaos;
                        }
                        else
                        {
                            //ensure that order status is not pending
                            throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                        }
                    }
                    else
                    {
                        this.OrderService.GiftCards_Activated = null;
                    }
                    int gcdos = int.Parse(ddlGiftCardsDeactivationOrderStatus.SelectedItem.Value);
                    if (gcdos > 0)
                    {
                        if ((OrderStatusEnum)gcdos != OrderStatusEnum.Pending)
                        {
                            this.OrderService.GiftCards_Deactivated = (OrderStatusEnum)gcdos;
                        }
                        else
                        {
                            //ensure that order status is not pending
                            throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                        }
                    }
                    else
                    {
                        this.OrderService.GiftCards_Deactivated = null;
                    }

                    //form fields
                    this.CustomerService.FormFieldGenderEnabled = cbffGenderEnabled.Checked;
                    this.CustomerService.FormFieldDateOfBirthEnabled = cbffDateOfBirthEnabled.Checked;
                    this.CustomerService.FormFieldCompanyEnabled = cbffCompanyEnabled.Checked;
                    this.CustomerService.FormFieldCompanyRequired = cbffCompanyRequired.Checked;
                    this.CustomerService.FormFieldStreetAddressEnabled = cbffStreetAddressEnabled.Checked;
                    this.CustomerService.FormFieldStreetAddressRequired = cbffStreetAddressRequired.Checked;
                    this.CustomerService.FormFieldStreetAddress2Enabled = cbffStreetAddress2Enabled.Checked;
                    this.CustomerService.FormFieldStreetAddress2Required = cbffStreetAddress2Required.Checked;
                    this.CustomerService.FormFieldPostCodeEnabled = cbffPostCodeEnabled.Checked;
                    this.CustomerService.FormFieldPostCodeRequired = cbffPostCodeRequired.Checked;
                    this.CustomerService.FormFieldCityEnabled = cbffCityEnabled.Checked;
                    this.CustomerService.FormFieldCityRequired = cbffCityRequired.Checked;
                    this.CustomerService.FormFieldCountryEnabled = cbffCountryEnabled.Checked;
                    this.CustomerService.FormFieldStateEnabled = cbffStateEnabled.Checked;
                    this.CustomerService.FormFieldPhoneEnabled = cbffPhoneEnabled.Checked;
                    this.CustomerService.FormFieldPhoneRequired = cbffPhoneRequired.Checked;
                    this.CustomerService.FormFieldFaxEnabled = cbffFaxEnabled.Checked;
                    this.CustomerService.FormFieldFaxRequired = cbffFaxRequired.Checked;
                    this.CustomerService.FormFieldNewsletterEnabled = cbffNewsletterBoxEnabled.Checked;
                    this.CustomerService.FormFieldTimeZoneEnabled = cbffTimeZoneEnabled.Checked;


                    this.SettingManager.SetParam("Display.PageExecutionTimeInfoEnabled", cbDisplayPageExecutionTime.Checked.ToString());

                    //return requests (RMA)
                    this.SettingManager.SetParam("ReturnRequests.Enable", cbReturnRequestsEnabled.Checked.ToString());
                    this.SettingManager.SetParam("ReturnRequests.ReturnReasons", txtReturnReasons.Text);
                    this.SettingManager.SetParam("ReturnRequests.ReturnActions", txtReturnActions.Text);

                    this.CustomerActivityService.InsertActivity(
                        "EditGlobalSettings",
                        GetLocaleResourceString("ActivityLog.EditGlobalSettings"));

                    Response.Redirect(string.Format("GlobalSettings.aspx?TabID={0}", this.GetActiveTabId(this.CommonSettingsTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnStoreImagesInDBToggle_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.PictureService.StoreInDB = !this.PictureService.StoreInDB;

                Response.Redirect(string.Format("GlobalSettings.aspx?TabID={0}", this.GetActiveTabId(this.CommonSettingsTabs)));
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void BtnPdfLogoRemove_OnClick(object sender, EventArgs e)
        {
            try
            {
                File.Delete(PDFHelper.LogoFilePath);
                imgPdfLogoPreview.Visible = false;
                btnPdfLogoRemove.Visible = false;
            }
            catch(Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void btnFaviconRemove_OnClick(object sender, EventArgs e)
        {
            try
            {
                File.Delete(HttpContext.Current.Request.PhysicalApplicationPath + "favicon.ico");
                imgFavicon.Visible = false;
                btnFaviconRemove.Visible = false;
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }
        
        protected void btnChangeEncryptionPrivateKey_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    SecurityHelper.ChangeEncryptionPrivateKey(txtEncryptionPrivateKey.Text);
                    lblChangeEncryptionPrivateKeyResult.Text = GetLocaleResourceString("Admin.GlobalSettings.Security.ChangeKeySuccess");
                }
                catch (Exception exc)
                {
                    lblChangeEncryptionPrivateKeyResult.Text = exc.Message;
                }
            }
        }

        protected string TabId
        {
            get
            {
                return CommonHelper.QueryString("TabId");
            }
        }
    }
}

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
            txtStoreName.Text = IoC.Resolve<ISettingManager>().StoreName;
            txtStoreURL.Text = IoC.Resolve<ISettingManager>().StoreUrl;
            cbStoreClosed.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.StoreClosed");
            cbStoreClosedForAdmins.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.StoreClosed.AllowAdminAccess");
            cbAnonymousCheckoutAllowed.Checked = IoC.Resolve<ICustomerService>().AnonymousCheckoutAllowed;
            cbUseOnePageCheckout.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Checkout.UseOnePageCheckout");
            cbCheckoutTermsOfService.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Checkout.TermsOfServiceEnabled");

            cbStoreNameInTitle.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.IncludeStoreNameInTitle");
            txtDefaulSEOTitle.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.DefaultTitle");
            txtDefaulSEODescription.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.DefaultMetaDescription");
            txtDefaulSEOKeywords.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.DefaultMetaKeywords");
            cbConvertNonWesternChars.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEONames.ConvertNonWesternChars");
            cbAllowCustomerSelectTheme.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.AllowCustomerSelectTheme");
            
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
            cbShowWelcomeMessage.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowWelcomeMessageOnMainPage");
            cbShowNewsHeaderRssURL.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowNewsHeaderRssURL");
            cbShowBlogHeaderRssURL.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowBlogHeaderRssURL");
            cbEnableUrlRewriting.Checked = SEOHelper.EnableUrlRewriting;
            txtProductUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Product.UrlRewriteFormat");
            cbProductCanonicalUrl.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.CanonicalURLs.Products.Enabled");
            txtCategoryUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Category.UrlRewriteFormat");
            cbCategoryCanonicalUrl.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.CanonicalURLs.Category.Enabled");
            txtManufacturerUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Manufacturer.UrlRewriteFormat");
            cbManufacturerCanonicalUrl.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.CanonicalURLs.Manufacturer.Enabled");
            txtProductTagUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.ProductTags.UrlRewriteFormat");
            txtNewsUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.News.UrlRewriteFormat");
            txtBlogUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Blog.UrlRewriteFormat");
            txtTopicUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Topic.UrlRewriteFormat");
            txtForumUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Forum.UrlRewriteFormat");
            txtForumGroupUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.ForumGroup.UrlRewriteFormat");
            txtForumTopicUrlRewriteFormat.Text = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.ForumTopic.UrlRewriteFormat");


            lStoreImagesInDBStorage.Text = IoC.Resolve<IPictureService>().StoreInDB ? GetLocaleResourceString("Admin.GlobalSettings.Media.StoreImagesInDB.DB") : GetLocaleResourceString("Admin.GlobalSettings.Media.StoreImagesInDB.FS");
            txtMaxImageSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.MaximumImageSize");
            txtProductThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.Product.ThumbnailImageSize");
            txtProductDetailSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.Product.DetailImageSize");
            txtProductVariantSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.Product.VariantImageSize");
            txtCategoryThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.Category.ThumbnailImageSize");
            txtManufacturerThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.Manufacturer.ThumbnailImageSize");
            cbShowCartImages.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowProductImagesOnShoppingCart");
            cbShowWishListImages.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowProductImagesOnWishList");
            txtShoppingCartThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize");
            cbShowAdminProductImages.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowAdminProductImages");

            txtEncryptionPrivateKey.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Security.EncryptionPrivateKey");
            cbEnableLoginCaptchaImage.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.LoginCaptchaImageEnabled");
            cbEnableRegisterCaptchaImage.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled");
            
            CommonHelper.SelectListItem(this.ddlCustomerNameFormat, (int)IoC.Resolve<ICustomerService>().CustomerNameFormatting);
            cbShowCustomersLocation.Checked = IoC.Resolve<ICustomerService>().ShowCustomersLocation;
            cbShowCustomersJoinDate.Checked = IoC.Resolve<ICustomerService>().ShowCustomersJoinDate;
            cbAllowPM.Checked = IoC.Resolve<IForumService>().AllowPrivateMessages;
            cbNotifyAboutPrivateMessages.Checked = IoC.Resolve<IForumService>().NotifyAboutPrivateMessages;
            cbAllowViewingProfiles.Checked = IoC.Resolve<ICustomerService>().AllowViewingProfiles;
            cbCustomersAllowedToUploadAvatars.Checked = IoC.Resolve<ICustomerService>().AllowCustomersToUploadAvatars;
            cbDefaultAvatarEnabled.Checked = IoC.Resolve<ICustomerService>().DefaultAvatarEnabled;
            lblCurrentTimeZone.Text = DateTimeHelper.CurrentTimeZone.DisplayName;
            TimeZoneInfo defaultStoreTimeZone = DateTimeHelper.DefaultStoreTimeZone;
            if (defaultStoreTimeZone != null)
                CommonHelper.SelectListItem(this.ddlDefaultStoreTimeZone, defaultStoreTimeZone.Id);
            cbAllowCustomersToSetTimeZone.Checked = DateTimeHelper.AllowCustomersToSetTimeZone;


            cbUsernamesEnabled.Checked = IoC.Resolve<ICustomerService>().UsernamesEnabled;
            cbAllowCustomersToChangeUsernames.Checked = IoC.Resolve<ICustomerService>().AllowCustomersToChangeUsernames;
            cbNotifyAboutNewCustomerRegistration.Checked = IoC.Resolve<ICustomerService>().NotifyNewCustomerRegistration;
            CommonHelper.SelectListItem(this.ddlRegistrationMethod, (int)IoC.Resolve<ICustomerService>().CustomerRegistrationType);
            cbAllowNavigationOnlyRegisteredCustomers.Checked = IoC.Resolve<ICustomerService>().AllowNavigationOnlyRegisteredCustomers;
            cbHideNewsletterBox.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.HideNewsletterBox");

            cbShowCategoryProductNumber.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowCategoryProductNumber");
            cbHidePricesForNonRegistered.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.HidePricesForNonRegistered");
            txtMinOrderSubtotalAmount.Value = IoC.Resolve<IOrderService>().MinOrderSubtotalAmount;
            txtMinOrderTotalAmount.Value = IoC.Resolve<IOrderService>().MinOrderTotalAmount;
            cbShowDiscountCouponBox.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Checkout.DiscountCouponBox");
            cbShowGiftCardBox.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Checkout.GiftCardBox");
            cbShowSKU.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowSKU");
            cbShowManufacturerPartNumber.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowManufacturerPartNumber");
            cbDisplayCartAfterAddingProduct.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.DisplayCartAfterAddingProduct");
            cbEnableDynamicPriceUpdate.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ProductAttribute.EnableDynamicPriceUpdate");
            cbAllowProductSorting.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.AllowProductSorting");
            cbShowShareButton.Checked = IoC.Resolve<IProductService>().ShowShareButton;
            cbDownloadableProductsTab.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.DownloadableProductsTab");
            cbUseImagesForLanguageSelection.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.UseImagesForLanguageSelection", false);
            cbEnableCompareProducts.Checked = IoC.Resolve<IProductService>().CompareProductsEnabled;
            cbEnableWishlist.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.EnableWishlist");
            cbEmailWishlist.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.EmailWishlist");
            cbIsReOrderAllowed.Checked = IoC.Resolve<IOrderService>().IsReOrderAllowed;
            cbEnableEmailAFriend.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.EnableEmailAFirend");
            cbShowMiniShoppingCart.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.ShowMiniShoppingCart");
            cbRecentlyViewedProductsEnabled.Checked = IoC.Resolve<IProductService>().RecentlyViewedProductsEnabled;
            txtRecentlyViewedProductsNumber.Value = IoC.Resolve<IProductService>().RecentlyViewedProductsNumber;
            cbRecentlyAddedProductsEnabled.Checked = IoC.Resolve<IProductService>().RecentlyAddedProductsEnabled;
            txtRecentlyAddedProductsNumber.Value = IoC.Resolve<IProductService>().RecentlyAddedProductsNumber;
            cbNotifyAboutNewProductReviews.Checked = IoC.Resolve<IProductService>().NotifyAboutNewProductReviews;
            cbProductReviewsMustBeApproved.Checked = IoC.Resolve<ICustomerService>().ProductReviewsMustBeApproved;
            cbAllowAnonymousUsersToReviewProduct.Checked = IoC.Resolve<ICustomerService>().AllowAnonymousUsersToReviewProduct;
            cbAllowAnonymousUsersToEmailAFriend.Checked = IoC.Resolve<ICustomerService>().AllowAnonymousUsersToEmailAFriend;
            cbAllowAnonymousUsersToVotePolls.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.AllowAnonymousUsersToVotePolls");

            cbAllowAnonymousUsersToSetProductRatings.Checked = IoC.Resolve<ICustomerService>().AllowAnonymousUsersToSetProductRatings;
            cbShowBestsellersOnHomePage.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowBestsellersOnMainPage");
            txtShowBestsellersOnHomePageNumber.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.ShowBestsellersOnMainPageNumber");
            cbProductsAlsoPurchased.Checked = IoC.Resolve<IProductService>().ProductsAlsoPurchasedEnabled;
            txtProductsAlsoPurchasedNumber.Value = IoC.Resolve<IProductService>().ProductsAlsoPurchasedNumber;
            txtCrossSellsNumber.Value = IoC.Resolve<IProductService>().CrossSellsNumber;
            txtSearchPageProductsPerPage.Value = IoC.Resolve<IProductService>().SearchPageProductsPerPage;
            txtMaxShoppingCartItems.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Common.MaximumShoppingCartItems");
            txtMaxWishlistItems.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Common.MaximumWishlistItems");

            cbLiveChatEnabled.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("LiveChat.Enabled", false);
            txtLiveChatBtnCode.Text = IoC.Resolve<ISettingManager>().GetSettingValue("LiveChat.BtnCode");
            txtLiveChatMonCode.Text = IoC.Resolve<ISettingManager>().GetSettingValue("LiveChat.MonCode");

            //Google Adsense
            cbGoogleAdsenseEnabled.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("GoogleAdsense.Enabled", false);
            txtGoogleAdsenseCode.Text = IoC.Resolve<ISettingManager>().GetSettingValue("GoogleAdsense.Code");

            //Google Analytics
            cbGoogleAnalyticsEnabled.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Analytics.GoogleEnabled", false);
            txtGoogleAnalyticsId.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Analytics.GoogleID");
            txtGoogleAnalyticsJS.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Analytics.GoogleJS");
            CommonHelper.SelectListItem(this.ddlGoogleAnalyticsPlacement, IoC.Resolve<ISettingManager>().GetSettingValue("Analytics.GooglePlacement", "Body"));

            txtAllowedIPList.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Security.AdminAreaAllowedIP");

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
            cbRewardPointsEnabled.Checked = IoC.Resolve<IOrderService>().RewardPointsEnabled;
            txtRewardPointsRate.Value = IoC.Resolve<IOrderService>().RewardPointsExchangeRate;
            txtRewardPointsForRegistration.Value = IoC.Resolve<IOrderService>().RewardPointsForRegistration;
            txtRewardPointsForPurchases_Amount.Value = IoC.Resolve<IOrderService>().RewardPointsForPurchases_Amount;
            txtRewardPointsForPurchases_Points.Value = IoC.Resolve<IOrderService>().RewardPointsForPurchases_Points;
            CommonHelper.SelectListItem(ddlRewardPointsAwardedOrderStatus, ((int)IoC.Resolve<IOrderService>().RewardPointsForPurchases_Awarded).ToString());
            CommonHelper.SelectListItem(ddlRewardPointsCanceledOrderStatus, ((int)IoC.Resolve<IOrderService>().RewardPointsForPurchases_Canceled).ToString());

            //gift cards
            if (IoC.Resolve<IOrderService>().GiftCards_Activated.HasValue)
                CommonHelper.SelectListItem(ddlGiftCardsActivationOrderStatus, ((int)IoC.Resolve<IOrderService>().GiftCards_Activated).ToString());
            else
                CommonHelper.SelectListItem(ddlGiftCardsActivationOrderStatus, 0);
            if (IoC.Resolve<IOrderService>().GiftCards_Deactivated.HasValue)
                CommonHelper.SelectListItem(ddlGiftCardsDeactivationOrderStatus, ((int)IoC.Resolve<IOrderService>().GiftCards_Deactivated).ToString());
            else
                CommonHelper.SelectListItem(ddlGiftCardsDeactivationOrderStatus, 0);

            //form fields
            cbffGenderEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldGenderEnabled;
            cbffDateOfBirthEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldDateOfBirthEnabled;
            cbffCompanyEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled;
            cbffCompanyRequired.Checked = IoC.Resolve<ICustomerService>().FormFieldCompanyRequired;
            cbffStreetAddressEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled;
            cbffStreetAddressRequired.Checked = IoC.Resolve<ICustomerService>().FormFieldStreetAddressRequired;
            cbffStreetAddress2Enabled.Checked = IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled;
            cbffStreetAddress2Required.Checked = IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Required;
            cbffPostCodeEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled;
            cbffPostCodeRequired.Checked = IoC.Resolve<ICustomerService>().FormFieldPostCodeRequired;
            cbffCityEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldCityEnabled;
            cbffCityRequired.Checked = IoC.Resolve<ICustomerService>().FormFieldCityRequired;
            cbffCountryEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldCountryEnabled;
            cbffStateEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldStateEnabled;
            cbffPhoneEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled;
            cbffPhoneRequired.Checked = IoC.Resolve<ICustomerService>().FormFieldPhoneRequired;
            cbffFaxEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldFaxEnabled;
            cbffFaxRequired.Checked = IoC.Resolve<ICustomerService>().FormFieldFaxRequired;
            cbffNewsletterBoxEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldNewsletterEnabled;
            cbffTimeZoneEnabled.Checked = IoC.Resolve<ICustomerService>().FormFieldTimeZoneEnabled;

            //return requests (RMA)
            cbReturnRequestsEnabled.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ReturnRequests.Enable");
            txtReturnReasons.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ReturnRequests.ReturnReasons");
            txtReturnActions.Text = IoC.Resolve<ISettingManager>().GetSettingValue("ReturnRequests.ReturnActions");

            cbDisplayPageExecutionTime.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.PageExecutionTimeInfoEnabled");
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
                            if (!IoC.Resolve<IBlacklistService>().IsValidIp(s.Trim()))
                            {
                                throw new NopException("IP list is not valid.");
                            }
                        }
                    }

                    IoC.Resolve<ISettingManager>().StoreName = txtStoreName.Text;
                    IoC.Resolve<ISettingManager>().StoreUrl = txtStoreURL.Text;
                    IoC.Resolve<ISettingManager>().SetParam("Common.StoreClosed", cbStoreClosed.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.StoreClosed.AllowAdminAccess", cbStoreClosedForAdmins.Checked.ToString());
                    IoC.Resolve<ICustomerService>().AnonymousCheckoutAllowed = cbAnonymousCheckoutAllowed.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Checkout.UseOnePageCheckout", cbUseOnePageCheckout.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Checkout.TermsOfServiceEnabled", cbCheckoutTermsOfService.Checked.ToString());

                    IoC.Resolve<ISettingManager>().SetParam("SEO.IncludeStoreNameInTitle", cbStoreNameInTitle.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("SEO.DefaultTitle", txtDefaulSEOTitle.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.DefaultMetaDescription", txtDefaulSEODescription.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.DefaultMetaKeywords", txtDefaulSEOKeywords.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEONames.ConvertNonWesternChars", cbConvertNonWesternChars.Checked.ToString());

                    IoC.Resolve<ISettingManager>().SetParam("Display.PublicStoreTheme", ctrlThemeSelector.SelectedTheme);
                    IoC.Resolve<ISettingManager>().SetParam("Display.AllowCustomerSelectTheme", cbAllowCustomerSelectTheme.Checked.ToString());
                    
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
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowWelcomeMessageOnMainPage", cbShowWelcomeMessage.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowNewsHeaderRssURL", cbShowNewsHeaderRssURL.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowBlogHeaderRssURL", cbShowBlogHeaderRssURL.Checked.ToString());
                    SEOHelper.EnableUrlRewriting = cbEnableUrlRewriting.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("SEO.Product.UrlRewriteFormat", txtProductUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.CanonicalURLs.Products.Enabled", cbProductCanonicalUrl.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("SEO.Category.UrlRewriteFormat", txtCategoryUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.CanonicalURLs.Category.Enabled", cbCategoryCanonicalUrl.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("SEO.Manufacturer.UrlRewriteFormat", txtManufacturerUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.CanonicalURLs.Manufacturer.Enabled", cbManufacturerCanonicalUrl.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("SEO.ProductTags.UrlRewriteFormat", txtProductTagUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.News.UrlRewriteFormat", txtNewsUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.Blog.UrlRewriteFormat", txtBlogUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.Topic.UrlRewriteFormat", txtTopicUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.Forum.UrlRewriteFormat", txtForumUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.ForumGroup.UrlRewriteFormat", txtForumGroupUrlRewriteFormat.Text);
                    IoC.Resolve<ISettingManager>().SetParam("SEO.ForumTopic.UrlRewriteFormat", txtForumTopicUrlRewriteFormat.Text);


                    IoC.Resolve<ISettingManager>().SetParam("Media.MaximumImageSize", txtMaxImageSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Media.Product.ThumbnailImageSize", txtProductThumbSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Media.Product.DetailImageSize", txtProductDetailSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Media.Product.VariantImageSize", txtProductVariantSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Media.Category.ThumbnailImageSize", txtCategoryThumbSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Media.Manufacturer.ThumbnailImageSize", txtManufacturerThumbSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowProductImagesOnShoppingCart", cbShowCartImages.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowProductImagesOnWishList", cbShowWishListImages.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Media.ShoppingCart.ThumbnailImageSize", txtShoppingCartThumbSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowAdminProductImages", cbShowAdminProductImages.Checked.ToString());

                    IoC.Resolve<ISettingManager>().SetParam("Security.AdminAreaAllowedIP", ipList);
                    IoC.Resolve<ISettingManager>().SetParam("Common.LoginCaptchaImageEnabled", cbEnableLoginCaptchaImage.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.RegisterCaptchaImageEnabled", cbEnableRegisterCaptchaImage.Checked.ToString());


                    IoC.Resolve<ICustomerService>().CustomerNameFormatting = (CustomerNameFormatEnum)Enum.ToObject(typeof(CustomerNameFormatEnum), int.Parse(this.ddlCustomerNameFormat.SelectedItem.Value));
                    IoC.Resolve<ICustomerService>().ShowCustomersLocation = cbShowCustomersLocation.Checked;
                    IoC.Resolve<ICustomerService>().ShowCustomersJoinDate = cbShowCustomersJoinDate.Checked;
                    IoC.Resolve<IForumService>().AllowPrivateMessages = cbAllowPM.Checked;
                    IoC.Resolve<IForumService>().NotifyAboutPrivateMessages = cbNotifyAboutPrivateMessages.Checked;
                    IoC.Resolve<ICustomerService>().AllowViewingProfiles = cbAllowViewingProfiles.Checked;
                    IoC.Resolve<ICustomerService>().AllowCustomersToUploadAvatars = cbCustomersAllowedToUploadAvatars.Checked;
                    IoC.Resolve<ICustomerService>().DefaultAvatarEnabled = cbDefaultAvatarEnabled.Checked;
                    string defaultStoreTimeZoneId = ddlDefaultStoreTimeZone.SelectedItem.Value;
                    DateTimeHelper.DefaultStoreTimeZone = DateTimeHelper.FindTimeZoneById(defaultStoreTimeZoneId);
                    DateTimeHelper.AllowCustomersToSetTimeZone = cbAllowCustomersToSetTimeZone.Checked;


                    IoC.Resolve<ICustomerService>().UsernamesEnabled = cbUsernamesEnabled.Checked;
                    IoC.Resolve<ICustomerService>().AllowCustomersToChangeUsernames = cbAllowCustomersToChangeUsernames.Checked;
                    IoC.Resolve<ICustomerService>().NotifyNewCustomerRegistration = cbNotifyAboutNewCustomerRegistration.Checked;
                    IoC.Resolve<ICustomerService>().CustomerRegistrationType = (CustomerRegistrationTypeEnum)Enum.ToObject(typeof(CustomerRegistrationTypeEnum), int.Parse(this.ddlRegistrationMethod.SelectedItem.Value));
                    IoC.Resolve<ICustomerService>().AllowNavigationOnlyRegisteredCustomers = cbAllowNavigationOnlyRegisteredCustomers.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Display.HideNewsletterBox", cbHideNewsletterBox.Checked.ToString());

                    IoC.Resolve<ISettingManager>().SetParam("Display.Products.ShowCategoryProductNumber", cbShowCategoryProductNumber.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.HidePricesForNonRegistered", cbHidePricesForNonRegistered.Checked.ToString());
                    IoC.Resolve<IOrderService>().MinOrderSubtotalAmount = txtMinOrderSubtotalAmount.Value;
                    IoC.Resolve<IOrderService>().MinOrderTotalAmount = txtMinOrderTotalAmount.Value;
                    IoC.Resolve<ISettingManager>().SetParam("Display.Checkout.DiscountCouponBox", cbShowDiscountCouponBox.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.Checkout.GiftCardBox", cbShowGiftCardBox.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.Products.ShowSKU", cbShowSKU.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.Products.ShowManufacturerPartNumber", cbShowManufacturerPartNumber.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.Products.DisplayCartAfterAddingProduct", cbDisplayCartAfterAddingProduct.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("ProductAttribute.EnableDynamicPriceUpdate", cbEnableDynamicPriceUpdate.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.AllowProductSorting", cbAllowProductSorting.Checked.ToString());
                    IoC.Resolve<IProductService>().ShowShareButton = cbShowShareButton.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Display.DownloadableProductsTab", cbDownloadableProductsTab.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.UseImagesForLanguageSelection", cbUseImagesForLanguageSelection.Checked.ToString());
                    IoC.Resolve<IProductService>().CompareProductsEnabled = cbEnableCompareProducts.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Common.EnableWishlist", cbEnableWishlist.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.EmailWishlist", cbEmailWishlist.Checked.ToString());
                    IoC.Resolve<IOrderService>().IsReOrderAllowed = cbIsReOrderAllowed.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Common.EnableEmailAFirend", cbEnableEmailAFriend.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.ShowMiniShoppingCart", cbShowMiniShoppingCart.Checked.ToString());
                    IoC.Resolve<IProductService>().RecentlyViewedProductsEnabled = cbRecentlyViewedProductsEnabled.Checked;
                    IoC.Resolve<IProductService>().RecentlyViewedProductsNumber = txtRecentlyViewedProductsNumber.Value;
                    IoC.Resolve<IProductService>().RecentlyAddedProductsEnabled = cbRecentlyAddedProductsEnabled.Checked;
                    IoC.Resolve<IProductService>().RecentlyAddedProductsNumber = txtRecentlyAddedProductsNumber.Value;
                    IoC.Resolve<IProductService>().NotifyAboutNewProductReviews = cbNotifyAboutNewProductReviews.Checked;
                    IoC.Resolve<ICustomerService>().ProductReviewsMustBeApproved = cbProductReviewsMustBeApproved.Checked;
                    IoC.Resolve<ICustomerService>().AllowAnonymousUsersToReviewProduct = cbAllowAnonymousUsersToReviewProduct.Checked;
                    IoC.Resolve<ICustomerService>().AllowAnonymousUsersToEmailAFriend = cbAllowAnonymousUsersToEmailAFriend.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Common.AllowAnonymousUsersToVotePolls", cbAllowAnonymousUsersToVotePolls.Checked.ToString());

                    IoC.Resolve<ICustomerService>().AllowAnonymousUsersToSetProductRatings = cbAllowAnonymousUsersToSetProductRatings.Checked;
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowBestsellersOnMainPage", cbShowBestsellersOnHomePage.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Display.ShowBestsellersOnMainPageNumber", txtShowBestsellersOnHomePageNumber.Value.ToString());
                    IoC.Resolve<IProductService>().ProductsAlsoPurchasedEnabled = cbProductsAlsoPurchased.Checked;
                    IoC.Resolve<IProductService>().ProductsAlsoPurchasedNumber = txtProductsAlsoPurchasedNumber.Value;
                    IoC.Resolve<IProductService>().CrossSellsNumber = txtCrossSellsNumber.Value;
                    IoC.Resolve<IProductService>().SearchPageProductsPerPage = txtSearchPageProductsPerPage.Value;
                    IoC.Resolve<ISettingManager>().SetParam("Common.MaximumShoppingCartItems", txtMaxShoppingCartItems.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Common.MaximumWishlistItems", txtMaxWishlistItems.Value.ToString());
                    
                    IoC.Resolve<ISettingManager>().SetParam("LiveChat.Enabled", cbLiveChatEnabled.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("LiveChat.BtnCode", txtLiveChatBtnCode.Text);
                    IoC.Resolve<ISettingManager>().SetParam("LiveChat.MonCode", txtLiveChatMonCode.Text);

                    //Google Adsense
                    IoC.Resolve<ISettingManager>().SetParam("GoogleAdsense.Enabled", cbGoogleAdsenseEnabled.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("GoogleAdsense.Code", txtGoogleAdsenseCode.Text);

                    //Google Analytics
                    IoC.Resolve<ISettingManager>().SetParam("Analytics.GoogleEnabled", cbGoogleAnalyticsEnabled.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Analytics.GoogleID", txtGoogleAnalyticsId.Text);
                    IoC.Resolve<ISettingManager>().SetParam("Analytics.GoogleJS", txtGoogleAnalyticsJS.Text);
                    IoC.Resolve<ISettingManager>().SetParam("Analytics.GooglePlacement", this.ddlGoogleAnalyticsPlacement.SelectedItem.Value);


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
                    IoC.Resolve<IOrderService>().RewardPointsEnabled = cbRewardPointsEnabled.Checked;
                    IoC.Resolve<IOrderService>().RewardPointsExchangeRate = txtRewardPointsRate.Value;
                    IoC.Resolve<IOrderService>().RewardPointsForRegistration = txtRewardPointsForRegistration.Value;
                    IoC.Resolve<IOrderService>().RewardPointsForPurchases_Amount = txtRewardPointsForPurchases_Amount.Value;
                    IoC.Resolve<IOrderService>().RewardPointsForPurchases_Points = txtRewardPointsForPurchases_Points.Value;
                    OrderStatusEnum rppa = (OrderStatusEnum)int.Parse(ddlRewardPointsAwardedOrderStatus.SelectedItem.Value);
                    if (rppa != OrderStatusEnum.Pending)
                    {
                        IoC.Resolve<IOrderService>().RewardPointsForPurchases_Awarded = rppa;
                    }
                    else
                    {
                        //ensure that order status is not pending
                        throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                    }
                    OrderStatusEnum rppc = (OrderStatusEnum)int.Parse(ddlRewardPointsCanceledOrderStatus.SelectedItem.Value);
                    if (rppc != OrderStatusEnum.Pending)
                    {
                        IoC.Resolve<IOrderService>().RewardPointsForPurchases_Canceled = rppc;
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
                            IoC.Resolve<IOrderService>().GiftCards_Activated = (OrderStatusEnum)gcaos;
                        }
                        else
                        {
                            //ensure that order status is not pending
                            throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                        }
                    }
                    else
                    {
                        IoC.Resolve<IOrderService>().GiftCards_Activated = null;
                    }
                    int gcdos = int.Parse(ddlGiftCardsDeactivationOrderStatus.SelectedItem.Value);
                    if (gcdos > 0)
                    {
                        if ((OrderStatusEnum)gcdos != OrderStatusEnum.Pending)
                        {
                            IoC.Resolve<IOrderService>().GiftCards_Deactivated = (OrderStatusEnum)gcdos;
                        }
                        else
                        {
                            //ensure that order status is not pending
                            throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                        }
                    }
                    else
                    {
                        IoC.Resolve<IOrderService>().GiftCards_Deactivated = null;
                    }

                    //form fields
                    IoC.Resolve<ICustomerService>().FormFieldGenderEnabled = cbffGenderEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldDateOfBirthEnabled = cbffDateOfBirthEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled = cbffCompanyEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldCompanyRequired = cbffCompanyRequired.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled = cbffStreetAddressEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldStreetAddressRequired = cbffStreetAddressRequired.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled = cbffStreetAddress2Enabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Required = cbffStreetAddress2Required.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled = cbffPostCodeEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldPostCodeRequired = cbffPostCodeRequired.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldCityEnabled = cbffCityEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldCityRequired = cbffCityRequired.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldCountryEnabled = cbffCountryEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldStateEnabled = cbffStateEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled = cbffPhoneEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldPhoneRequired = cbffPhoneRequired.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldFaxEnabled = cbffFaxEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldFaxRequired = cbffFaxRequired.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldNewsletterEnabled = cbffNewsletterBoxEnabled.Checked;
                    IoC.Resolve<ICustomerService>().FormFieldTimeZoneEnabled = cbffTimeZoneEnabled.Checked;


                    IoC.Resolve<ISettingManager>().SetParam("Display.PageExecutionTimeInfoEnabled", cbDisplayPageExecutionTime.Checked.ToString());

                    //return requests (RMA)
                    IoC.Resolve<ISettingManager>().SetParam("ReturnRequests.Enable", cbReturnRequestsEnabled.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("ReturnRequests.ReturnReasons", txtReturnReasons.Text);
                    IoC.Resolve<ISettingManager>().SetParam("ReturnRequests.ReturnActions", txtReturnActions.Text);

                    IoC.Resolve<ICustomerActivityService>().InsertActivity(
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
                IoC.Resolve<IPictureService>().StoreInDB = !IoC.Resolve<IPictureService>().StoreInDB;

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

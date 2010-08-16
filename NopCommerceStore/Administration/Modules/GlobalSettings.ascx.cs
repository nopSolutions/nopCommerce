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
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common;
using System.IO;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

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
            txtStoreName.Text = SettingManager.StoreName;
            txtStoreURL.Text = SettingManager.StoreUrl;
            cbStoreClosed.Checked = SettingManager.GetSettingValueBoolean("Common.StoreClosed");
            cbAnonymousCheckoutAllowed.Checked = CustomerManager.AnonymousCheckoutAllowed;
            cbUseOnePageCheckout.Checked = SettingManager.GetSettingValueBoolean("Checkout.UseOnePageCheckout");
            cbCheckoutTermsOfService.Checked = SettingManager.GetSettingValueBoolean("Checkout.TermsOfServiceEnabled");

            cbStoreNameInTitle.Checked = SettingManager.GetSettingValueBoolean("SEO.IncludeStoreNameInTitle");
            txtDefaulSEOTitle.Text = SettingManager.GetSettingValue("SEO.DefaultTitle");
            txtDefaulSEODescription.Text = SettingManager.GetSettingValue("SEO.DefaultMetaDescription");
            txtDefaulSEOKeywords.Text = SettingManager.GetSettingValue("SEO.DefaultMetaKeywords");
            cbConvertNonWesternChars.Checked = SettingManager.GetSettingValueBoolean("SEONames.ConvertNonWesternChars");
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
            cbShowWelcomeMessage.Checked = SettingManager.GetSettingValueBoolean("Display.ShowWelcomeMessageOnMainPage");
            cbShowNewsHeaderRssURL.Checked = SettingManager.GetSettingValueBoolean("Display.ShowNewsHeaderRssURL");
            cbShowBlogHeaderRssURL.Checked = SettingManager.GetSettingValueBoolean("Display.ShowBlogHeaderRssURL");
            cbEnableUrlRewriting.Checked = SEOHelper.EnableUrlRewriting;
            txtProductUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.Product.UrlRewriteFormat");
            txtCategoryUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.Category.UrlRewriteFormat");
            txtManufacturerUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.Manufacturer.UrlRewriteFormat");
            txtNewsUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.News.UrlRewriteFormat");
            txtBlogUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.Blog.UrlRewriteFormat");
            txtTopicUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.Topic.UrlRewriteFormat");
            txtForumUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.Forum.UrlRewriteFormat");
            txtForumGroupUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.ForumGroup.UrlRewriteFormat");
            txtForumTopicUrlRewriteFormat.Text = SettingManager.GetSettingValue("SEO.ForumTopic.UrlRewriteFormat");


            txtMaxImageSize.Value = SettingManager.GetSettingValueInteger("Media.MaximumImageSize");
            txtProductThumbSize.Value = SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize");
            txtProductDetailSize.Value = SettingManager.GetSettingValueInteger("Media.Product.DetailImageSize");
            txtProductVariantSize.Value = SettingManager.GetSettingValueInteger("Media.Product.VariantImageSize");
            txtCategoryThumbSize.Value = SettingManager.GetSettingValueInteger("Media.Category.ThumbnailImageSize");
            txtManufacturerThumbSize.Value = SettingManager.GetSettingValueInteger("Media.Manufacturer.ThumbnailImageSize");
            cbShowCartImages.Checked = SettingManager.GetSettingValueBoolean("Display.ShowProductImagesOnShoppingCart");
            cbShowWishListImages.Checked = SettingManager.GetSettingValueBoolean("Display.ShowProductImagesOnWishList");
            txtShoppingCartThumbSize.Value = SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize");
            cbShowAdminProductImages.Checked = SettingManager.GetSettingValueBoolean("Display.ShowAdminProductImages");

            txtEncryptionPrivateKey.Text = SettingManager.GetSettingValue("Security.EncryptionPrivateKey");
            cbEnableLoginCaptchaImage.Checked = SettingManager.GetSettingValueBoolean("Common.LoginCaptchaImageEnabled");
            cbEnableRegisterCaptchaImage.Checked = SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled");
            
            CommonHelper.SelectListItem(this.ddlCustomerNameFormat, (int)CustomerManager.CustomerNameFormatting);
            cbShowCustomersLocation.Checked = CustomerManager.ShowCustomersLocation;
            cbShowCustomersJoinDate.Checked = CustomerManager.ShowCustomersJoinDate;
            cbAllowPM.Checked = ForumManager.AllowPrivateMessages;
            cbAllowViewingProfiles.Checked = CustomerManager.AllowViewingProfiles;
            cbCustomersAllowedToUploadAvatars.Checked = CustomerManager.AllowCustomersToUploadAvatars;
            cbDefaultAvatarEnabled.Checked = CustomerManager.DefaultAvatarEnabled;
            lblCurrentTimeZone.Text = DateTimeHelper.CurrentTimeZone.DisplayName;
            TimeZoneInfo defaultStoreTimeZone = DateTimeHelper.DefaultStoreTimeZone;
            if (defaultStoreTimeZone != null)
                CommonHelper.SelectListItem(this.ddlDefaultStoreTimeZone, defaultStoreTimeZone.Id);
            cbAllowCustomersToSetTimeZone.Checked = DateTimeHelper.AllowCustomersToSetTimeZone;


            cbUsernamesEnabled.Checked = CustomerManager.UsernamesEnabled;
            CommonHelper.SelectListItem(this.ddlRegistrationMethod, (int)CustomerManager.CustomerRegistrationType);
            cbAllowNavigationOnlyRegisteredCustomers.Checked = CustomerManager.AllowNavigationOnlyRegisteredCustomers;
            cbHideNewsletterBox.Checked = SettingManager.GetSettingValueBoolean("Display.HideNewsletterBox");
            cbHidePricesForNonRegistered.Checked = SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered");
            txtMinOrderAmount.Value = OrderManager.MinOrderAmount;
            cbShowDiscountCouponBox.Checked = SettingManager.GetSettingValueBoolean("Display.Checkout.DiscountCouponBox");
            cbShowGiftCardBox.Checked = SettingManager.GetSettingValueBoolean("Display.Checkout.GiftCardBox");
            cbShowSKU.Checked = SettingManager.GetSettingValueBoolean("Display.Products.ShowSKU");
            cbDisplayCartAfterAddingProduct.Checked = SettingManager.GetSettingValueBoolean("Display.Products.DisplayCartAfterAddingProduct");
            cbEnableDynamicPriceUpdate.Checked = SettingManager.GetSettingValueBoolean("ProductAttribute.EnableDynamicPriceUpdate");
            cbAllowProductSorting.Checked = SettingManager.GetSettingValueBoolean("Common.AllowProductSorting");
            cbShowShareButton.Checked = ProductManager.ShowShareButton;
            cbDownloadableProductsTab.Checked = SettingManager.GetSettingValueBoolean("Display.DownloadableProductsTab");
            cbUseImagesForLanguageSelection.Checked = SettingManager.GetSettingValueBoolean("Common.UseImagesForLanguageSelection", false);
            cbEnableCompareProducts.Checked = ProductManager.CompareProductsEnabled;
            cbEnableWishlist.Checked = SettingManager.GetSettingValueBoolean("Common.EnableWishlist");
            cbIsReOrderAllowed.Checked = OrderManager.IsReOrderAllowed;
            cbEnableEmailAFriend.Checked = SettingManager.GetSettingValueBoolean("Common.EnableEmailAFirend");
            cbShowMiniShoppingCart.Checked = SettingManager.GetSettingValueBoolean("Common.ShowMiniShoppingCart");
            cbRecentlyViewedProductsEnabled.Checked = ProductManager.RecentlyViewedProductsEnabled;
            txtRecentlyViewedProductsNumber.Value = ProductManager.RecentlyViewedProductsNumber;
            cbRecentlyAddedProductsEnabled.Checked = ProductManager.RecentlyAddedProductsEnabled;
            txtRecentlyAddedProductsNumber.Value = ProductManager.RecentlyAddedProductsNumber;
            cbNotifyAboutNewProductReviews.Checked = ProductManager.NotifyAboutNewProductReviews;
            cbProductReviewsMustBeApproved.Checked = CustomerManager.ProductReviewsMustBeApproved;
            cbAllowAnonymousUsersToReviewProduct.Checked = CustomerManager.AllowAnonymousUsersToReviewProduct;
            cbAllowAnonymousUsersToEmailAFriend.Checked = CustomerManager.AllowAnonymousUsersToEmailAFriend;
            cbAllowAnonymousUsersToSetProductRatings.Checked = CustomerManager.AllowAnonymousUsersToSetProductRatings;
            cbShowBestsellersOnHomePage.Checked = SettingManager.GetSettingValueBoolean("Display.ShowBestsellersOnMainPage");
            txtShowBestsellersOnHomePageNumber.Value = SettingManager.GetSettingValueInteger("Display.ShowBestsellersOnMainPageNumber");
            cbProductsAlsoPurchased.Checked = ProductManager.ProductsAlsoPurchasedEnabled;
            txtProductsAlsoPurchasedNumber.Value = ProductManager.ProductsAlsoPurchasedNumber;
            txtCrossSellsNumber.Value = ProductManager.CrossSellsNumber;
            cbLiveChatEnabled.Checked = SettingManager.GetSettingValueBoolean("LiveChat.Enabled", false);
            txtLiveChatBtnCode.Text = SettingManager.GetSettingValue("LiveChat.BtnCode");
            txtLiveChatMonCode.Text = SettingManager.GetSettingValue("LiveChat.MonCode");

            //Google Adsense
            cbGoogleAdsenseEnabled.Checked = SettingManager.GetSettingValueBoolean("GoogleAdsense.Enabled", false);
            txtGoogleAdsenseCode.Text = SettingManager.GetSettingValue("GoogleAdsense.Code");

            //Google Analytics
            cbGoogleAnalyticsEnabled.Checked = SettingManager.GetSettingValueBoolean("Analytics.GoogleEnabled", false);
            txtGoogleAnalyticsId.Text = SettingManager.GetSettingValue("Analytics.GoogleID");
            txtGoogleAnalyticsJS.Text = SettingManager.GetSettingValue("Analytics.GoogleJS");

            txtAllowedIPList.Text = SettingManager.GetSettingValue("Security.AdminAreaAllowedIP");

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
            cbRewardPointsEnabled.Checked = OrderManager.RewardPointsEnabled;
            txtRewardPointsRate.Value = OrderManager.RewardPointsExchangeRate;
            txtRewardPointsForRegistration.Value = OrderManager.RewardPointsForRegistration;
            txtRewardPointsForPurchases_Amount.Value = OrderManager.RewardPointsForPurchases_Amount;
            txtRewardPointsForPurchases_Points.Value = OrderManager.RewardPointsForPurchases_Points;
            CommonHelper.SelectListItem(ddlRewardPointsAwardedOrderStatus, ((int)OrderManager.RewardPointsForPurchases_Awarded).ToString());
            CommonHelper.SelectListItem(ddlRewardPointsCanceledOrderStatus, ((int)OrderManager.RewardPointsForPurchases_Canceled).ToString());

            //gift cards
            if (OrderManager.GiftCards_Activated.HasValue)
                CommonHelper.SelectListItem(ddlGiftCardsActivationOrderStatus, ((int)OrderManager.GiftCards_Activated).ToString());
            else
                CommonHelper.SelectListItem(ddlGiftCardsActivationOrderStatus, 0);
            if (OrderManager.GiftCards_Deactivated.HasValue)
                CommonHelper.SelectListItem(ddlGiftCardsDeactivationOrderStatus, ((int)OrderManager.GiftCards_Deactivated).ToString());
            else
                CommonHelper.SelectListItem(ddlGiftCardsDeactivationOrderStatus, 0);

            //form fields
            cbffGenderEnabled.Checked = CustomerManager.FormFieldGenderEnabled;
            cbffDateOfBirthEnabled.Checked = CustomerManager.FormFieldDateOfBirthEnabled;
            cbffCompanyEnabled.Checked = CustomerManager.FormFieldCompanyEnabled;
            cbffCompanyRequired.Checked = CustomerManager.FormFieldCompanyRequired;
            cbffStreetAddressEnabled.Checked = CustomerManager.FormFieldStreetAddressEnabled;
            cbffStreetAddressRequired.Checked = CustomerManager.FormFieldStreetAddressRequired;
            cbffStreetAddress2Enabled.Checked = CustomerManager.FormFieldStreetAddress2Enabled;
            cbffStreetAddress2Required.Checked = CustomerManager.FormFieldStreetAddress2Required;
            cbffPostCodeEnabled.Checked = CustomerManager.FormFieldPostCodeEnabled;
            cbffPostCodeRequired.Checked = CustomerManager.FormFieldPostCodeRequired;
            cbffCityEnabled.Checked = CustomerManager.FormFieldCityEnabled;
            cbffCityRequired.Checked = CustomerManager.FormFieldCityRequired;
            cbffCountryEnabled.Checked = CustomerManager.FormFieldCountryEnabled;
            cbffStateEnabled.Checked = CustomerManager.FormFieldStateEnabled;
            cbffPhoneEnabled.Checked = CustomerManager.FormFieldPhoneEnabled;
            cbffPhoneRequired.Checked = CustomerManager.FormFieldPhoneRequired;
            cbffFaxEnabled.Checked = CustomerManager.FormFieldFaxEnabled;
            cbffFaxRequired.Checked = CustomerManager.FormFieldFaxRequired;
            cbffNewsletterBoxEnabled.Checked = CustomerManager.FormFieldNewsletterEnabled;

            //return requests (RMA)
            cbReturnRequestsEnabled.Checked = SettingManager.GetSettingValueBoolean("ReturnRequests.Enable");
            txtReturnReasons.Text = SettingManager.GetSettingValue("ReturnRequests.ReturnReasons");
            txtReturnActions.Text = SettingManager.GetSettingValue("ReturnRequests.ReturnActions");
        }

        private void FillDropDowns()
        {
            //reward points
            this.ddlRewardPointsAwardedOrderStatus.Items.Clear();
            var orderStatuses1 = OrderManager.GetAllOrderStatuses();
            foreach (OrderStatus orderStatus in orderStatuses1)
            {
                ListItem item2 = new ListItem(OrderManager.GetOrderStatusName(orderStatus.OrderStatusId), orderStatus.OrderStatusId.ToString());
                this.ddlRewardPointsAwardedOrderStatus.Items.Add(item2);
            }
            this.ddlRewardPointsCanceledOrderStatus.Items.Clear();
            var orderStatuses2 = OrderManager.GetAllOrderStatuses();
            foreach (OrderStatus orderStatus in orderStatuses2)
            {
                ListItem item2 = new ListItem(OrderManager.GetOrderStatusName(orderStatus.OrderStatusId), orderStatus.OrderStatusId.ToString());
                this.ddlRewardPointsCanceledOrderStatus.Items.Add(item2);
            }

            //gift cards
            this.ddlGiftCardsActivationOrderStatus.Items.Clear();
            ListItem gcaosEmpty = new ListItem("---", "0");
            this.ddlGiftCardsActivationOrderStatus.Items.Add(gcaosEmpty);
            var orderStatuses3 = OrderManager.GetAllOrderStatuses();
            foreach (OrderStatus orderStatus in orderStatuses3)
            {
                ListItem item2 = new ListItem(OrderManager.GetOrderStatusName(orderStatus.OrderStatusId), orderStatus.OrderStatusId.ToString());
                this.ddlGiftCardsActivationOrderStatus.Items.Add(item2);
            }
            this.ddlGiftCardsDeactivationOrderStatus.Items.Clear();
            ListItem gcdosEmpty = new ListItem("---", "0");
            this.ddlGiftCardsDeactivationOrderStatus.Items.Add(gcdosEmpty);
            var orderStatuses4 = OrderManager.GetAllOrderStatuses();
            foreach (OrderStatus orderStatus in orderStatuses4)
            {
                ListItem item2 = new ListItem(OrderManager.GetOrderStatusName(orderStatus.OrderStatusId), orderStatus.OrderStatusId.ToString());
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

            this.cbEnableUrlRewriting.Attributes.Add("onclick", "toggleUrlRewriting();");
            this.cbCustomersAllowedToUploadAvatars.Attributes.Add("onclick", "toggleCustomersAllowedToUploadAvatars();");
            this.cbProductsAlsoPurchased.Attributes.Add("onclick", "toggleProductsAlsoPurchased();");
            this.cbRecentlyViewedProductsEnabled.Attributes.Add("onclick", "toggleRecentlyViewedProducts();");
            this.cbRecentlyAddedProductsEnabled.Attributes.Add("onclick", "toggleRecentlyAddedProducts();");
            this.cbShowBestsellersOnHomePage.Attributes.Add("onclick", "toggleShowBestsellersOnHomePage();");
            this.cbLiveChatEnabled.Attributes.Add("onclick", "toggleLiveChat();");
            this.cbGoogleAdsenseEnabled.Attributes.Add("onclick", "toggleGoogleAdsense();");
            this.cbGoogleAnalyticsEnabled.Attributes.Add("onclick", "toggleGoogleAnalytics();");

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
                            if (!IpBlacklistManager.IsValidIp(s.Trim()))
                            {
                                throw new NopException("IP list is not valid.");
                            }
                        }
                    }

                    SettingManager.StoreName = txtStoreName.Text;
                    SettingManager.StoreUrl = txtStoreURL.Text;
                    SettingManager.SetParam("Common.StoreClosed", cbStoreClosed.Checked.ToString());
                    CustomerManager.AnonymousCheckoutAllowed = cbAnonymousCheckoutAllowed.Checked;
                    SettingManager.SetParam("Checkout.UseOnePageCheckout", cbUseOnePageCheckout.Checked.ToString());
                    SettingManager.SetParam("Checkout.TermsOfServiceEnabled", cbCheckoutTermsOfService.Checked.ToString());

                    SettingManager.SetParam("SEO.IncludeStoreNameInTitle", cbStoreNameInTitle.Checked.ToString());
                    SettingManager.SetParam("SEO.DefaultTitle", txtDefaulSEOTitle.Text);
                    SettingManager.SetParam("SEO.DefaultMetaDescription", txtDefaulSEODescription.Text);
                    SettingManager.SetParam("SEO.DefaultMetaKeywords", txtDefaulSEOKeywords.Text);
                    SettingManager.SetParam("SEONames.ConvertNonWesternChars", cbConvertNonWesternChars.Checked.ToString());

                    SettingManager.SetParam("Display.PublicStoreTheme", ctrlThemeSelector.SelectedTheme);
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
                    SettingManager.SetParam("Display.ShowWelcomeMessageOnMainPage", cbShowWelcomeMessage.Checked.ToString());
                    SettingManager.SetParam("Display.ShowNewsHeaderRssURL", cbShowNewsHeaderRssURL.Checked.ToString());
                    SettingManager.SetParam("Display.ShowBlogHeaderRssURL", cbShowBlogHeaderRssURL.Checked.ToString());
                    SEOHelper.EnableUrlRewriting = cbEnableUrlRewriting.Checked;
                    SettingManager.SetParam("SEO.Product.UrlRewriteFormat", txtProductUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.Category.UrlRewriteFormat", txtCategoryUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.Manufacturer.UrlRewriteFormat", txtManufacturerUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.News.UrlRewriteFormat", txtNewsUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.Blog.UrlRewriteFormat", txtBlogUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.Topic.UrlRewriteFormat", txtTopicUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.Forum.UrlRewriteFormat", txtForumUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.ForumGroup.UrlRewriteFormat", txtForumGroupUrlRewriteFormat.Text);
                    SettingManager.SetParam("SEO.ForumTopic.UrlRewriteFormat", txtForumTopicUrlRewriteFormat.Text);


                    SettingManager.SetParam("Media.MaximumImageSize", txtMaxImageSize.Value.ToString());
                    SettingManager.SetParam("Media.Product.ThumbnailImageSize", txtProductThumbSize.Value.ToString());
                    SettingManager.SetParam("Media.Product.DetailImageSize", txtProductDetailSize.Value.ToString());
                    SettingManager.SetParam("Media.Product.VariantImageSize", txtProductVariantSize.Value.ToString());
                    SettingManager.SetParam("Media.Category.ThumbnailImageSize", txtCategoryThumbSize.Value.ToString());
                    SettingManager.SetParam("Media.Manufacturer.ThumbnailImageSize", txtManufacturerThumbSize.Value.ToString());
                    SettingManager.SetParam("Display.ShowProductImagesOnShoppingCart", cbShowCartImages.Checked.ToString());
                    SettingManager.SetParam("Display.ShowProductImagesOnWishList", cbShowWishListImages.Checked.ToString());
                    SettingManager.SetParam("Media.ShoppingCart.ThumbnailImageSize", txtShoppingCartThumbSize.Value.ToString());
                    SettingManager.SetParam("Display.ShowAdminProductImages", cbShowAdminProductImages.Checked.ToString());

                    SettingManager.SetParam("Security.AdminAreaAllowedIP", ipList);

                    SettingManager.SetParam("Common.LoginCaptchaImageEnabled", cbEnableLoginCaptchaImage.Checked.ToString());
                    SettingManager.SetParam("Common.RegisterCaptchaImageEnabled", cbEnableRegisterCaptchaImage.Checked.ToString());


                    CustomerManager.CustomerNameFormatting = (CustomerNameFormatEnum)Enum.ToObject(typeof(CustomerNameFormatEnum), int.Parse(this.ddlCustomerNameFormat.SelectedItem.Value));
                    CustomerManager.ShowCustomersLocation = cbShowCustomersLocation.Checked;
                    CustomerManager.ShowCustomersJoinDate = cbShowCustomersJoinDate.Checked;
                    ForumManager.AllowPrivateMessages = cbAllowPM.Checked;
                    CustomerManager.AllowViewingProfiles = cbAllowViewingProfiles.Checked;
                    CustomerManager.AllowCustomersToUploadAvatars = cbCustomersAllowedToUploadAvatars.Checked;
                    CustomerManager.DefaultAvatarEnabled = cbDefaultAvatarEnabled.Checked;
                    string defaultStoreTimeZoneId = ddlDefaultStoreTimeZone.SelectedItem.Value;
                    DateTimeHelper.DefaultStoreTimeZone = DateTimeHelper.FindTimeZoneById(defaultStoreTimeZoneId);
                    DateTimeHelper.AllowCustomersToSetTimeZone = cbAllowCustomersToSetTimeZone.Checked;


                    CustomerManager.UsernamesEnabled = cbUsernamesEnabled.Checked;
                    CustomerManager.CustomerRegistrationType = (CustomerRegistrationTypeEnum)Enum.ToObject(typeof(CustomerRegistrationTypeEnum), int.Parse(this.ddlRegistrationMethod.SelectedItem.Value));
                    CustomerManager.AllowNavigationOnlyRegisteredCustomers = cbAllowNavigationOnlyRegisteredCustomers.Checked;
                    SettingManager.SetParam("Display.HideNewsletterBox", cbHideNewsletterBox.Checked.ToString());
                    SettingManager.SetParam("Common.HidePricesForNonRegistered", cbHidePricesForNonRegistered.Checked.ToString());
                    OrderManager.MinOrderAmount = txtMinOrderAmount.Value;
                    SettingManager.SetParam("Display.Checkout.DiscountCouponBox", cbShowDiscountCouponBox.Checked.ToString());
                    SettingManager.SetParam("Display.Checkout.GiftCardBox", cbShowGiftCardBox.Checked.ToString());
                    SettingManager.SetParam("Display.Products.ShowSKU", cbShowSKU.Checked.ToString());
                    SettingManager.SetParam("Display.Products.DisplayCartAfterAddingProduct", cbDisplayCartAfterAddingProduct.Checked.ToString());
                    SettingManager.SetParam("ProductAttribute.EnableDynamicPriceUpdate", cbEnableDynamicPriceUpdate.Checked.ToString());
                    SettingManager.SetParam("Common.AllowProductSorting", cbAllowProductSorting.Checked.ToString());
                    ProductManager.ShowShareButton = cbShowShareButton.Checked;
                    SettingManager.SetParam("Display.DownloadableProductsTab", cbDownloadableProductsTab.Checked.ToString());
                    SettingManager.SetParam("Common.UseImagesForLanguageSelection", cbUseImagesForLanguageSelection.Checked.ToString());
                    ProductManager.CompareProductsEnabled = cbEnableCompareProducts.Checked;
                    SettingManager.SetParam("Common.EnableWishlist", cbEnableWishlist.Checked.ToString());
                    OrderManager.IsReOrderAllowed = cbIsReOrderAllowed.Checked;
                    SettingManager.SetParam("Common.EnableEmailAFirend", cbEnableEmailAFriend.Checked.ToString());
                    SettingManager.SetParam("Common.ShowMiniShoppingCart", cbShowMiniShoppingCart.Checked.ToString());
                    ProductManager.RecentlyViewedProductsEnabled = cbRecentlyViewedProductsEnabled.Checked;
                    ProductManager.RecentlyViewedProductsNumber = txtRecentlyViewedProductsNumber.Value;
                    ProductManager.RecentlyAddedProductsEnabled = cbRecentlyAddedProductsEnabled.Checked;
                    ProductManager.RecentlyAddedProductsNumber = txtRecentlyAddedProductsNumber.Value;
                    ProductManager.NotifyAboutNewProductReviews = cbNotifyAboutNewProductReviews.Checked;
                    CustomerManager.ProductReviewsMustBeApproved = cbProductReviewsMustBeApproved.Checked;
                    CustomerManager.AllowAnonymousUsersToReviewProduct = cbAllowAnonymousUsersToReviewProduct.Checked;
                    CustomerManager.AllowAnonymousUsersToEmailAFriend = cbAllowAnonymousUsersToEmailAFriend.Checked;
                    CustomerManager.AllowAnonymousUsersToSetProductRatings = cbAllowAnonymousUsersToSetProductRatings.Checked;
                    SettingManager.SetParam("Display.ShowBestsellersOnMainPage", cbShowBestsellersOnHomePage.Checked.ToString());
                    SettingManager.SetParam("Display.ShowBestsellersOnMainPageNumber", txtShowBestsellersOnHomePageNumber.Value.ToString());
                    ProductManager.ProductsAlsoPurchasedEnabled = cbProductsAlsoPurchased.Checked;
                    ProductManager.ProductsAlsoPurchasedNumber = txtProductsAlsoPurchasedNumber.Value;
                    ProductManager.CrossSellsNumber = txtCrossSellsNumber.Value;

                    SettingManager.SetParam("LiveChat.Enabled", cbLiveChatEnabled.Checked.ToString());
                    SettingManager.SetParam("LiveChat.BtnCode", txtLiveChatBtnCode.Text);
                    SettingManager.SetParam("LiveChat.MonCode", txtLiveChatMonCode.Text);

                    //Google Adsense
                    SettingManager.SetParam("GoogleAdsense.Enabled", cbGoogleAdsenseEnabled.Checked.ToString());
                    SettingManager.SetParam("GoogleAdsense.Code", txtGoogleAdsenseCode.Text);

                    //Google Analytics
                    SettingManager.SetParam("Analytics.GoogleEnabled", cbGoogleAnalyticsEnabled.Checked.ToString());
                    SettingManager.SetParam("Analytics.GoogleID", txtGoogleAnalyticsId.Text);
                    SettingManager.SetParam("Analytics.GoogleJS", txtGoogleAnalyticsJS.Text);

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
                    OrderManager.RewardPointsEnabled = cbRewardPointsEnabled.Checked;
                    OrderManager.RewardPointsExchangeRate = txtRewardPointsRate.Value;
                    OrderManager.RewardPointsForRegistration = txtRewardPointsForRegistration.Value;
                    OrderManager.RewardPointsForPurchases_Amount = txtRewardPointsForPurchases_Amount.Value;
                    OrderManager.RewardPointsForPurchases_Points = txtRewardPointsForPurchases_Points.Value;
                    OrderStatusEnum rppa = (OrderStatusEnum)int.Parse(ddlRewardPointsAwardedOrderStatus.SelectedItem.Value);
                    if (rppa != OrderStatusEnum.Pending)
                    {
                        OrderManager.RewardPointsForPurchases_Awarded = rppa;
                    }
                    else
                    {
                        //ensure that order status is not pending
                        throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                    }
                    OrderStatusEnum rppc = (OrderStatusEnum)int.Parse(ddlRewardPointsCanceledOrderStatus.SelectedItem.Value);
                    if (rppc != OrderStatusEnum.Pending)
                    {
                        OrderManager.RewardPointsForPurchases_Canceled = rppc;
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
                            OrderManager.GiftCards_Activated = (OrderStatusEnum)gcaos;
                        }
                        else
                        {
                            //ensure that order status is not pending
                            throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                        }
                    }
                    else
                    {
                        OrderManager.GiftCards_Activated = null;
                    }
                    int gcdos = int.Parse(ddlGiftCardsDeactivationOrderStatus.SelectedItem.Value);
                    if (gcdos > 0)
                    {
                        if ((OrderStatusEnum)gcdos != OrderStatusEnum.Pending)
                        {
                            OrderManager.GiftCards_Deactivated = (OrderStatusEnum)gcdos;
                        }
                        else
                        {
                            //ensure that order status is not pending
                            throw new NopException(GetLocaleResourceString("Admin.GlobalSettings.PendingOrderStatusNotAllowed"));
                        }
                    }
                    else
                    {
                        OrderManager.GiftCards_Deactivated = null;
                    }

                    //form fields
                    CustomerManager.FormFieldGenderEnabled = cbffGenderEnabled.Checked;
                    CustomerManager.FormFieldDateOfBirthEnabled = cbffDateOfBirthEnabled.Checked;
                    CustomerManager.FormFieldCompanyEnabled = cbffCompanyEnabled.Checked;
                    CustomerManager.FormFieldCompanyRequired = cbffCompanyRequired.Checked;
                    CustomerManager.FormFieldStreetAddressEnabled = cbffStreetAddressEnabled.Checked;
                    CustomerManager.FormFieldStreetAddressRequired = cbffStreetAddressRequired.Checked;
                    CustomerManager.FormFieldStreetAddress2Enabled = cbffStreetAddress2Enabled.Checked;
                    CustomerManager.FormFieldStreetAddress2Required = cbffStreetAddress2Required.Checked;
                    CustomerManager.FormFieldPostCodeEnabled = cbffPostCodeEnabled.Checked;
                    CustomerManager.FormFieldPostCodeRequired = cbffPostCodeRequired.Checked;
                    CustomerManager.FormFieldCityEnabled = cbffCityEnabled.Checked;
                    CustomerManager.FormFieldCityRequired = cbffCityRequired.Checked;
                    CustomerManager.FormFieldCountryEnabled = cbffCountryEnabled.Checked;
                    CustomerManager.FormFieldStateEnabled = cbffStateEnabled.Checked;
                    CustomerManager.FormFieldPhoneEnabled = cbffPhoneEnabled.Checked;
                    CustomerManager.FormFieldPhoneRequired = cbffPhoneRequired.Checked;
                    CustomerManager.FormFieldFaxEnabled = cbffFaxEnabled.Checked;
                    CustomerManager.FormFieldFaxRequired = cbffFaxRequired.Checked;
                    CustomerManager.FormFieldNewsletterEnabled = cbffNewsletterBoxEnabled.Checked;

                    //return requests (RMA)
                    SettingManager.SetParam("ReturnRequests.Enable", cbReturnRequestsEnabled.Checked.ToString());
                    SettingManager.SetParam("ReturnRequests.ReturnReasons", txtReturnReasons.Text);
                    SettingManager.SetParam("ReturnRequests.ReturnActions", txtReturnActions.Text);

                    CustomerActivityManager.InsertActivity(
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
                ShowError(ex.Message);
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
                ShowError(ex.Message);
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

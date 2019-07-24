--upgrade scripts from nopCommerce 2.30 to nopCommerce 2.40

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreDiscounts.Hint">
    <Value>Check to ignore discounts (sitewide). It can significantly improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreFeaturedProducts.Hint">
    <Value>Check to ignore featured products (sitewide). It can significantly improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Messages.Order.Products(s).Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.MarkAsPrimaryWeight">
    <Value>Mark as primary weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.MarkAsPrimaryDimension">
    <Value>Mark as primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Description">
    <Value>NOTE: if you change your primary weight, then do not forget to update the appropriate ratios of the units</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Description">
    <Value>NOTE: if you change your primary dimension, then do not forget to update the appropriate ratios of the units</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.EmailAccounts.Fields.MarkAsDefaultEmail">
    <Value>Mark as default email account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Tax.Providers.Fields.MarkAsPrimaryProvider">
    <Value>Mark as primary provider</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Check">
    <Value>Check</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ProductThumbPictureSizeOnProductDetailsPage">
    <Value>Product thumbnail image size (product page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ProductThumbPictureSizeOnProductDetailsPage.Hint">
    <Value>The default size (pixels) for product thumbnail images displayed on product details page when you have more than one product image.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ProductThumbPictureSize">
    <Value>Product thumbnail image size (catalog)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ProductThumbPictureSize.Hint">
    <Value>The default size (pixels) for product thumbnail images displayed on category or manufacturer pages.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.MobileDevicesSupported">
    <Value>Mobile devices supported</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.MobileDevicesSupported.Hint">
    <Value>Check to enable mobile devices support.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.SignaturesEnabled.Hint">
    <Value>Add an opportunity for customers to specify signature. Signature will be displayed below each forum post.</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Topics.Count">
    <Value>{0} Topics</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Replies.Count">
    <Value>{0} Replies</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Config">
    <Value>Config</Value>
  </LocaleResource>
  <LocaleResource Name="Languages">
    <Value>Languages</Value>
  </LocaleResource>
  <LocaleResource Name="Currencies">
    <Value>Currencies</Value>
  </LocaleResource>
  <LocaleResource Name="Tax.SelectType">
    <Value>Tax display type</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Home">
    <Value>Home</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops">
    <Value>Desktop store theme</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops.Hint">
    <Value>The public store theme for desktops. You can download themes from the extensions page at www.nopcommerce.com.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForMobileDevices">
    <Value>Mobile store theme</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForMobileDevices.Hint">
    <Value>The public store theme for mobile devices. You can download themes from the extensions page at www.nopcommerce.com.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ForumSubscriptions.Description">
    <Value>You will receive an e-mail when a new forum topic/post is created.</Value>
  </LocaleResource>
  <LocaleResource Name="ForumSubscriptions.DeleteSelected">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ForumSubscriptions.InfoColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ForumSubscriptions.NoSubscriptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.ForumSubscriptions.DeleteSelected">
    <Value>Delete Selected</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ForumSubscriptions.InfoColumn">
    <Value>Forum/Topic</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ForumSubscriptions.NoSubscriptions">
    <Value>You are not currently subscribed to any forums</Value>
  </LocaleResource>
  <LocaleResource Name="ShippingReturns">
    <Value><![CDATA[Shipping &amp; Returns]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Orders.ID">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Orders.Order">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment">
    <Value>Page title SEO adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment.Hint">
    <Value>Select a page title SEO adjustment. For example, generated page title could be (PAGE NAME | YOURSTORE.COM) instead of (YOURSTORE.COM | PAGE NAME).</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Common.PageTitleSeoAdjustment.PagenameAfterStorename">
    <Value>Page name comes after store name</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Common.PageTitleSeoAdjustment.StorenameAfterPagename">
    <Value>Store name comes after page name</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.PaymentMethod">
    <Value>Payment method: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AssociatedExternalAuth.Hint">
    <Value>A list of external authentication identifiers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage">
    <Value>Show on registration page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage.Hint">
    <Value>Check to show CAPTCHA on registration page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage">
    <Value>Show on contact us page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage.Hint">
    <Value>Check to show CAPTCHA on contact us page.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductHasBeenAddedToTheCart">
    <Value>The product has been added to the cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingProduct">
    <Value>Display cart after adding product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingProduct.Hint">
    <Value>If checked, a customer will be taken to the Shopping Cart page immediately after adding a product to their cart. If unchecked, a customer will stay on the same page that they are adding the product to the cart from.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit">
    <Value>Calculate per weight unit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit.Hint">
    <Value>If you check this option, then rates are multiplied per weight unit (lb, kg, etc). This option is used for the fixed rates (without percents).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.FailedToSave">
    <Value>Failed to save requirement</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.BillingCountry.Fields.SelectCountry">
    <Value>Select country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.ShippingCountry.Fields.SelectCountry">
    <Value>Select country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExchangeRate.EcbExchange.SetCurrencyToEURO">
    <Value>You can use ECB (European central bank) exchange rate provider only when exchange rate currency code is set to EURO</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.Login">
    <Value>Login using Facebook account</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.OpenId.Login">
    <Value>Login using OpenID account</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.OpenId.YourAccount">
    <Value>Please click your account provider</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.OpenId.Manually">
    <Value>Enter manually your OpenID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.OpenId.SignIn">
    <Value>Sign In</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Twitter.Login">
    <Value>Login using Twitter account</Value>
  </LocaleResource>
  <LocaleResource Name="Plugin.Misc.MailChimp.ManualSync.Hint">
    <Value>Manually synchronize nopCommerce newsletter subscribers with MailChimp database</Value>
  </LocaleResource>
  <LocaleResource Name="Plugin.Misc.MailChimp.QueueAll.Hint">
    <Value>Queue existing newsletter subscribers (run only once)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugin.Misc.MailChimp.AutoSyncRestart">
    <Value>If sync task period has been changed, please restart the application</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Misc.WebServices.Description1">
    <Value>Actually configuration is not required. Just some notes:</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Misc.WebServices.Description2">
    <Value>Ensure that permissions are properly configured on Access Control List page (disabled by default)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Misc.WebServices.Description3">
    <Value>To access service use {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Misc.WebServices.Description4">
    <Value>For mex endpoint use {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Notes">
    <Value>If you''re using this gateway, ensure that your primary store currency is supported by Authorize.NET.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.UseSandbox">
    <Value>Use Sandbox</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.UseSandbox.Hint">
    <Value>Check to enable Sandbox (testing environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.TransactModeValues">
    <Value>Transaction mode</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.TransactModeValues.Hint">
    <Value>Choose transaction mode</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.TransactionKey">
    <Value>Transaction key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.TransactionKey.Hint">
    <Value>Specify transaction key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.LoginId">
    <Value>Login ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.LoginId.Hint">
    <Value>Specify login identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.AdditionalFee">
    <Value>Additional fee</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.AdditionalFee.Hint">
    <Value>Enter additional fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.UseSandbox">
    <Value>Use Sandbox</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.UseSandbox.Hint">
    <Value>Check to enable Sandbox (testing environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId">
    <Value>Google Vendor ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId.Hint">
    <Value>Specify Google Vendor ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey">
    <Value>Google Merchant Key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey.Hint">
    <Value>Specify Google Merchant Key.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback">
    <Value>Authenticate callback</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback.Hint">
    <Value>Check to ensure that Google handler callback is authenticated.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.AdditionalFee">
    <Value>Additional fee</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.AdditionalFee.Hint">
    <Value>Enter additional fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.TransactMode">
    <Value>After checkout mark payment as</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.TransactMode.Hint">
    <Value>Specify transaction mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.UseSandbox">
    <Value>Use Sandbox</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint">
    <Value>Check to enable Sandbox (testing environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.TransactMode">
    <Value>Transaction mode</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint">
    <Value>Specify transaction mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountName">
    <Value>API Account Name</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountName.Hint">
    <Value>Specify API account name.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword">
    <Value>API Account Password</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword.Hint">
    <Value>Specify API account password.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.Signature">
    <Value>Signature</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.Signature.Hint">
    <Value>Specify signature.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.AdditionalFee">
    <Value>Additional fee</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.AdditionalFee.Hint">
    <Value>Enter additional fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.RedirectionTip">
    <Value>You will be redirected to PayPal site to complete the order.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.UseSandbox">
    <Value>Use Sandbox</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.UseSandbox.Hint">
    <Value>Check to enable Sandbox (testing environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.BusinessEmail">
    <Value>Business Email</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.BusinessEmail.Hint">
    <Value>Specify your PayPal business email.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.PDTToken">
    <Value>PDT Identity Token</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.PDTToken.Hint">
    <Value>Specify PDT identity token</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal">
    <Value>PDT. Validate order total</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal.Hint">
    <Value>Check if PDT handler should validate order totals.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AdditionalFee">
    <Value>Additional fee</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AdditionalFee.Hint">
    <Value>Enter additional fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals">
    <Value>Pass product names and order totals to PayPal</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals.Hint">
    <Value>Check if product names and order totals should be passed to PayPal.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.EnableIpn">
    <Value>Enable IPN (Instant Payment Notification)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.EnableIpn.Hint">
    <Value>Check if IPN is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.IpnUrl">
    <Value>IPN Handler</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.IpnUrl.Hint">
    <Value>Specify IPN Handler.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.EnableIpn.Hint2">
    <Value>Leave blank to use the default IPN handler URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.GatewayUrl">
    <Value>Gateway URL</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.GatewayUrl.Hint">
    <Value>Specify gateway URL</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge">
    <Value>Additional handling charge.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge.Hint">
    <Value>Enter additional handling fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode">
    <Value>Shipped from zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode.Hint">
    <Value>Specify origin zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.AddRecord">
    <Value>Add record</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.AddRecord.Hint">
    <Value>Adding a new record</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Url">
    <Value>Canada Post URL</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Url.Hint">
    <Value>Specify Canada Post URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Port">
    <Value>Canada Post Port</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Port.Hint">
    <Value>Specify Canada Post port.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.CustomerId">
    <Value>Canada Post Customer ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.CustomerId.Hint">
    <Value>Specify Canada Post customer identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Url">
    <Value>URL</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Url.Hint">
    <Value>Specify FedEx URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Key">
    <Value>Key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Key.Hint">
    <Value>Specify FedEx key.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Password">
    <Value>Password</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Password.Hint">
    <Value>Specify FedEx password.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.AccountNumber">
    <Value>Account number</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.AccountNumber.Hint">
    <Value>Specify FedEx account number.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.MeterNumber">
    <Value>Meter number</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.MeterNumber.Hint">
    <Value>Specify FedEx meter number.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.UseResidentialRates">
    <Value>Use residential rates</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.UseResidentialRates.Hint">
    <Value>Check to use residential rates.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.ApplyDiscounts">
    <Value>Use discounted rates</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.ApplyDiscounts.Hint">
    <Value>Check to use discounted rates (instead of list rates).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.AdditionalHandlingCharge">
    <Value>Additional handling charge</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.AdditionalHandlingCharge.Hint">
    <Value>Enter additional handling fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.CarrierServices">
    <Value>Carrier Services Offered</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.CarrierServices.Hint">
    <Value>Select the services you want to offer to customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Street">
    <Value>Shipping origin. Street</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.Street.Hint">
    <Value>Specify origin street.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.City">
    <Value>Shipping origin. City</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.City.Hint">
    <Value>Specify origin city.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.StateOrProvinceCode">
    <Value>Shipping origin. State code (2 characters)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.StateOrProvinceCode.Hint">
    <Value>Specify origin state code (2 characters).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PostalCode">
    <Value>Shipping origin. Zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PostalCode.Hint">
    <Value>Specify origin zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.CountryCode">
    <Value>Shipping origin. Country code</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.CountryCode.Hint">
    <Value>Specify origin country code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedRateShipping.Fields.ShippingMethodName">
    <Value>Shipping method</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedRateShipping.Fields.Rate">
    <Value>Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Url">
    <Value>URL</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Url.Hint">
    <Value>Specify UPS URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.AccessKey">
    <Value>Access Key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.AccessKey.Hint">
    <Value>Specify UPS access key.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Username">
    <Value>Username</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Username.Hint">
    <Value>Specify UPS username.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Password">
    <Value>Password</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Password.Hint">
    <Value>Specify UPS password.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge">
    <Value>Additional handling charge</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint">
    <Value>Enter additional handling fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.InsurePackage">
    <Value>Insure package</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.InsurePackage.Hint">
    <Value>Check to ensure packages.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.CustomerClassification">
    <Value>UPS Customer Classification</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.CustomerClassification.Hint">
    <Value>Choose customer classification.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PickupType">
    <Value>UPS Pickup Type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PickupType.Hint">
    <Value>Choose UPS pickup type.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PackagingType">
    <Value>UPS Packaging Type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.PackagingType.Hint">
    <Value>Choose UPS packaging type.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.DefaultShippedFromCountry">
    <Value>Shipped from country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.DefaultShippedFromCountry.Hint">
    <Value>Specify origin country.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.DefaultShippedFromZipPostalCode">
    <Value>Shipped from zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.DefaultShippedFromZipPostalCode.Hint">
    <Value>Specify origin zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.AvailableCarrierServices">
    <Value>Carrier Services</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.AvailableCarrierServices.Hint">
    <Value>Select the services you want to offer to customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.Url">
    <Value>URL</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.Url.Hint">
    <Value>Specify USPS URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.Username">
    <Value>Username</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.Username.Hint">
    <Value>Specify USPS username.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.Password">
    <Value>Password</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.Password.Hint">
    <Value>Specify USPS password.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge">
    <Value>Additional handling charge</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge.Hint">
    <Value>Enter additional handling fee to charge your customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.ZipPostalCodeFrom">
    <Value>Shipped from zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.ZipPostalCodeFrom.Hint">
    <Value>Specify origin zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic">
    <Value>Domestic Carrier Services</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic.Hint">
    <Value>Select the services you want to offer to customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational">
    <Value>International Carrier Services</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational.Hint">
    <Value>Select the services you want to offer to customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Clickatell.SendTest">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Clickatell.SendTest.Hint">
    <Value>Send test message</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Verizon.SendTest">
    <Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Verizon.SendTest.Hint">
    <Value>Send test message</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.AddRecord">
    <Value>Add tax rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.AddRecord.Hint">
    <Value>Adding a new tax rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedRate.Fields.TaxCategoryName">
    <Value>Tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedRate.Fields.Rate">
    <Value>Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.UserId">
    <Value>StrikeIron User ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.UserId.Hint">
    <Value>Specify StrikeIron user identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.Password">
    <Value>StrikeIron Password</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.Password.Hint">
    <Value>Specify StrikeIron password.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingUsa.Button">
    <Value>Test (USA)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingUsa.Title">
    <Value>Test Online Tax Service (USA)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingUsa.Zip">
    <Value>Zip Code</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingUsa.Zip.Hint">
    <Value>Specify zip code for testing.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingCanada.Button">
    <Value>Test (Canada)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingCanada.Title">
    <Value>Test Online Tax Service (Canada)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingCanada.ProvinceCode">
    <Value>Two Letter Province Code</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.StrikeIron.TestingCanada.ProvinceCode.Hint">
    <Value>Specify postal code for testing.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.CannotLoadProduct">
    <Value>Product (Id={0}) cannot be loaded</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ProductDeleted">
    <Value>Product is deleted</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ProductUnpublished">
    <Value>Product is not published</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.BuyingDisabled">
    <Value>Buying is disabled for this product</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.WishlistDisabled">
    <Value>Wishlist is disabled for this product</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.RecipientEmailError">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.RecipientNameError">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.SenderEmailError">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCartWarning.SenderNameError">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.RecipientEmailError">
    <Value>Enter valid recipient email</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.RecipientNameError">
    <Value>Enter valid recipient name</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.SenderEmailError">
    <Value>Enter valid sender email</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.SenderNameError">
    <Value>Enter valid sender name</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.CannotLoadProductVariant">
    <Value>Product variant (Id={0}) cannot be loaded</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.NotAvailable">
    <Value>Product is not available</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax">
    <Value>Calculate ''X'' including tax</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax.Hint">
    <Value>Check to calculate ''X'' value including tax; otherwise excluding tax.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PassDimensions">
    <Value>Pass dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PassDimensions.Hint">
    <Value>Check if you want to pass package dimensions when requesting rates.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileHeadHtmlTag">
    <Value>Mobile version. Head HTML tag</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileAfterBodyStartHtmlTag">
    <Value><![CDATA[Mobile version. After <body> start HTML tag]]></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileHeaderLinks">
    <Value>Mobile version. Header links</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileBeforeContent">
    <Value>Mobile version. Before content</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileAfterContent">
    <Value>Mobile version. After content</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileFooter">
    <Value>Mobile version. Footer</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileBeforeBodyEndHtmlTag">
    <Value><![CDATA[Mobile version. Before <body> end HTML tag]]></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeBodyEndHtmlTag">
    <Value><![CDATA[Before <body> end HTML tag]]></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.UpdateCartItem">
    <Value>Update qty</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.LastName.Required">
    <Value>Last name is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Guest">
    <Value>Guest</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.Fields.Email.Required">
    <Value>Email is required.</Value>
  </LocaleResource>  
  <LocaleResource Name="BackInStockSubscriptions.Tooltip">
    <Value>You''ll receive a onetime e-mail when this product is available for ordering again. We will not send you any other e-mails or add you to our newsletter; you will only be e-mailed about this product!</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.ProductQuantity">
    <Value>Qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name.Hint">
    <Value>The name of the checkout attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.Name.Hint">
    <Value>The name of the checkout value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.Fields.Name.Hint">
    <Value>The name of the product attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.Fields.Description.Hint">
    <Value>The description of the product attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name.Hint">
    <Value>The name of the specification attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Fields.DisplayOrder.Hint">
    <Value>The display order of the specification attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name.Hint">
    <Value>The name of the option.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.DisplayOrder.Hint">
    <Value>The display order of the option.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.Description.Hint">
    <Value>The description of the category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.CategoryTemplate.Hint">
    <Value>Choose a category template. This template defines how this category (and its products) will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.Description.Hint">
    <Value>The description of the manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.ManufacturerTemplate.Hint">
    <Value>Choose a manufacturer template. This template defines how this manufacturer (and its products) will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShortDescription.Hint">
    <Value>The short description of the product. This is the text that displays in product lists i.e. category / manufacturer pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.FullDescription.Hint">
    <Value>The full description of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTemplate.Hint">
    <Value>Choose a product template. This template defines how this product (and its variants) will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTags.Hint">
    <Value>Product tags are keywords that this product can also be identified by. Enter a comma separated list of the tags to be associated with this product. The more products associated with a particular tag, the larger it will show on the tag cloud.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AutomaticallyAddRequiredProductVariants.Hint">
    <Value>Check to automatically add these product variants to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableEndDateTime.Hint">
    <Value>The end of the product''s availability in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AvailableStartDateTime.Hint">
    <Value>The start of the product''s availability in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Description.Hint">
    <Value>The description of the product variant.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.HasUserAgreement.Hint">
    <Value>Check if the product has a user agreement.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OrderMaximumQuantity.Hint">
    <Value>Set the maximum quantity allowed in a customer''s shopping cart e.g. set to 5 to only allow customers to purchase 5 of this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.OrderMinimumQuantity.Hint">
    <Value>Set the minimum quantity allowed in a customer''s shopping cart e.g. set to 3 to only allow customers to purchase 3 or more of this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType.Hint">
    <Value>The Activity Log Type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.UniqueSeoCode.Required">
    <Value>Please provide a unique SEO code.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.RecentlyAddedProductsEnabled">
    <Value>''Recently added products'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersLocation">
    <Value>Show customers'' location</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersLocation.Hint">
    <Value>A value indicating whether customers'' location is shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersJoinDate">
    <Value>Show customers'' join date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ShowCustomersJoinDate.Hint">
    <Value>A value indicating whether to show customers'' join date.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AllowViewingProfiles">
    <Value>Allow viewing of customer profiles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AllowViewingProfiles.Hint">
    <Value>A value indicating whether the viewing of customer profiles is allowed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UserRegistrationType.Hint">
    <Value>Determines customer registration method. Standard - mode where visitors can register and no approval is required. Email Validation - mode where user must respond to validation email that is sent to them before they are activated. Admin Approval - mode where visitors can register but admin approval is required. Disabled - mode where registration is disabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses.Hint">
    <Value>IP addresses allowed to access the Back End. Leave this field empty if you do not want to restrict access to the Back End. Use comma to separate them (e.g. 127.0.0.10,232.18.204.16)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.NumberOfDaysReturnRequestAvailable.Hint">
    <Value>Set a certain number of days that the Return Request Link will be available in the customer area. For example, if the store owner allows returns within 30 days of purchase, they would set this to 30. Logged in customers, viewing orders in ''My Account'', would then not see Return Request buttons for orders completed more than thirty days ago.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DisplayTaxRates.Hint">
    <Value>A value indicating whether each tax rate should be displayed on a separate line (shopping cart page).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Forums.ForumGroup.Fields.Description.Hint">
    <Value>The description of the forum group. This is the description that the customer will see.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Forums.Forum.Fields.Description.Hint">
    <Value>The description of the forum. This is the description that the customer will see.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.SystemKeyword.Hint">
    <Value>The system keyword for this poll.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.AuthorizationTransactionID.Hint">
    <Value>Authorization transaction identifier received from your payment gateway.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CaptureTransactionID.Hint">
    <Value>Capture transaction identifier received from your payment gateway.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.SubscriptionTransactionID.Hint">
    <Value>Subscription transaction identifier received from your payment gateway.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.Download">
    <Value>Downloadable product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.Name.Hint">
    <Value>The name of the discount.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.RequiresCouponCode.Hint">
    <Value>If checked, a customer must supply a valid coupon code for the discount to be applied.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.Updated">
    <Value>The payment has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart.Hint">
    <Value>A value indicating whether we need to find customers without shopping carts/wishlists. If unchecked, then all customers will be found.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UTCTime">
    <Value>Greenwich Mean Time (GMT/UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UTCTime.Hint">
    <Value>Greenwich Mean Time (GMT/UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Mobile.ViewMobileVersion">
    <Value>View mobile version</Value>
  </LocaleResource>
  <LocaleResource Name="Mobile.ViewFullSite">
    <Value>View Full Site</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.VatNumber.Status">
    <Value>Status</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.EmailIsNotProvided">
    <Value>Email is not provided</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.PasswordIsNotProvided">
    <Value>Password is not provided</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.UsernameIsNotProvided">
    <Value>Username is not provided</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.EmailAlreadyExists">
    <Value>The specified email already exists</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.UsernameAlreadyExists">
    <Value>The specified username already exists</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Errors.EmailIsNotProvided">
    <Value>Email is not entered</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Errors.PasswordIsNotProvided">
    <Value>Password is not entered</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Errors.EmailNotFound">
    <Value>The specified email could not be found</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Errors.OldPasswordDoesntMatch">
    <Value>Old password doesn''t match</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailUsernameErrors.NewEmailIsNotValid">
    <Value>New email is not valid</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailUsernameErrors.EmailTooLong">
    <Value>E-mail address is too long</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailUsernameErrors.EmailAlreadyExists">
    <Value>The e-mail address is already in use</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailUsernameErrors.UsernameTooLong">
    <Value>Username is too long</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailUsernameErrors.UsernameAlreadyExists">
    <Value>The username is already in use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Plugins">
    <Value>Plugins</Value>
  </LocaleResource>
</Language>
'

CREATE TABLE #LocaleStringResourceTmp
	(
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)

INSERT INTO #LocaleStringResourceTmp (ResourceName, ResourceValue)
SELECT	nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

--do it for each existing language
DECLARE @ExistingLanguageID int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [ID]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ResourceName nvarchar(200)
	DECLARE @ResourceValue nvarchar(MAX)
	DECLARE cur_localeresource CURSOR FOR
	SELECT ResourceName, ResourceValue
	FROM #LocaleStringResourceTmp
	OPEN cur_localeresource
	FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [LocaleStringResource]
			(
				[LanguageID],
				[ResourceName],
				[ResourceValue]
			)
			VALUES
			(
				@ExistingLanguageID,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource


	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO



--Update stored procedure according to new special price properties
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryId			int = 0,
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	DECLARE @SearchKeywords bit
	SET @SearchKeywords = 1
	IF (@Keywords IS NULL OR @Keywords = N'')
		SET @SearchKeywords = 0

	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',');
	
	DECLARE @SpecAttributesCount int	
	SELECT @SpecAttributesCount = COUNT(1) FROM #FilteredSpecs

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM Product p with (NOLOCK) 
	LEFT OUTER JOIN Product_Category_Mapping pcm with (NOLOCK) ON p.Id=pcm.ProductId
	LEFT OUTER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.Id=pmm.ProductId
	LEFT OUTER JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON p.Id=pptm.Product_Id
	LEFT OUTER JOIN ProductVariant pv with (NOLOCK) ON p.Id = pv.ProductId
	--searching of the localized values
	--comment the line below if you don't use it. It'll improve the performance
	LEFT OUTER JOIN LocalizedProperty lp with (NOLOCK) ON p.Id = lp.EntityId AND lp.LanguageId = @LanguageId AND lp.LocaleKeyGroup = N'Product'
	WHERE 
		(
		   (
				@CategoryId IS NULL OR @CategoryId=0
				OR (pcm.CategoryId=@CategoryId AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerId IS NULL OR @ManufacturerId=0
				OR (pmm.ManufacturerId=@ManufacturerId AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagId IS NULL OR @ProductTagId=0
				OR pptm.ProductTag_Id=@ProductTagId
			)
		AND	(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Deleted = 0
			)
		AND (
				--min price
				(@PriceMin IS NULL OR @PriceMin=0)
				OR 
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.SpecialPrice >= @PriceMin)
				)
				OR 
				(
					--regular price (price isn't specified or date range isn't valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.Price >= @PriceMin)
				)
			)
		AND (
				--max price
				(@PriceMax IS NULL OR @PriceMax=2147483644) -- max value
				OR 
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.SpecialPrice <= @PriceMax)
				)
				OR 
				(
					--regular price (price isn't specified or date range isn't valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.Price <= @PriceMax)
				)
			)
		AND	(
				@SearchKeywords = 0 or 
				(
					-- search standard content
					patindex(@Keywords, p.name) > 0
					or patindex(@Keywords, pv.name) > 0
					or patindex(@Keywords, pv.sku) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pv.Description) > 0)					
					--searching of the localized values
					--comment the lines below if you don't use it. It'll improve the performance
					or (lp.LocaleKey = N'Name' and patindex(@Keywords, lp.LocaleValue) > 0)
					or (@SearchDescriptions = 1 and lp.LocaleKey = N'ShortDescription' and patindex(@Keywords, lp.LocaleValue) > 0)
					or (@SearchDescriptions = 1 and lp.LocaleKey = N'FullDescription' and patindex(@Keywords, lp.LocaleValue) > 0)
				)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTimeUtc, '1/1/1900') and isnull(pv.AvailableEndDateTimeUtc, '1/1/2999'))
			)
		AND
			(
				--filter by specs
				@SpecAttributesCount = 0
				OR
				(
					NOT EXISTS(
						SELECT 1 
						FROM #FilteredSpecs [fs]
						WHERE [fs].SpecificationAttributeOptionId NOT IN (
							SELECT psam.SpecificationAttributeOptionId
							FROM dbo.Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryId IS NOT NULL AND @CategoryId > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerId IS NOT NULL AND @ManufacturerId > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 5
		--THEN dbo.[nop_getnotnullnotempty](pl.[Name],p.[Name]) END ASC,
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOnUtc END DESC

	DROP TABLE #FilteredSpecs

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	SET ROWCOUNT @RowsToReturn
	
	DROP TABLE #DisplayOrderTmp

	--return products (returned properties should be synchronized with 'Product' entity)
	SELECT  
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		IndexId
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
END
GO


--updated AddThis.com sharing code setting
UPDATE [Setting]
SET [Value]= N'<!-- AddThis Button BEGIN -->
<div class="addthis_toolbox addthis_default_style ">
<a class="addthis_button_preferred_1"></a>
<a class="addthis_button_preferred_2"></a>
<a class="addthis_button_preferred_3"></a>
<a class="addthis_button_preferred_4"></a>
<a class="addthis_button_compact"></a>
<a class="addthis_counter addthis_bubble_style"></a>
</div>
<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions"></script>
<!-- AddThis Button END -->'
WHERE [name] = N'catalogsettings.pagesharecode'
GO

--deleted obsolete settings
DELETE [Setting]
WHERE [name] = N'catalogsettings.hidepricesfornonregistered'
GO

DELETE [Setting]
WHERE [name] = N'shoppingcartsettings.wishlistenabled'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.productthumbpicturesizeonproductdetailspage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'mediasettings.productthumbpicturesizeonproductdetailspage', N'70')
END
GO

--mobile devices support
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.mobiledevicessupported')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'storeinformationsettings.mobiledevicessupported', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.defaultstorethemeformobiledevices')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'storeinformationsettings.defaultstorethemeformobiledevices', N'Mobile')
END
GO

UPDATE [CustomerAttribute]
SET [Key] = N'WorkingDesktopThemeName'
WHERE [Key] = N'WorkingThemeName'
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.defaultstorethemefordesktops')
BEGIN
	UPDATE [Setting]
	SET [Name] = N'storeinformationsettings.defaultstorethemefordesktops'
	WHERE [Name] = N'storeinformationsettings.defaultstoretheme'
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.emulatemobiledevice')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'storeinformationsettings.emulatemobiledevice', N'false')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'paymentsettings.bypasspaymentmethodselectionifonlyone')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'paymentsettings.bypasspaymentmethodselectionifonlyone', N'true')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.defaultviewmode')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.defaultviewmode', N'grid')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.defaultproductratingvalue')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.defaultproductratingvalue', N'5')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.pagetitleseoadjustment')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'seosettings.pagetitleseoadjustment', N'0')
END
GO


--missed 'Upload Pictures' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'UploadPictures')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Upload Pictures', N'UploadPictures', N'Configuration')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admin role be default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO

--new CAPTCHA settings
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.showonregistrationpage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'captchasettings.showonregistrationpage', N'true')
END
GO

--new CAPTCHA settings
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.showoncontactuspage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'captchasettings.showoncontactuspage', N'false')
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.displaycartafteraddingproduct')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.displaycartafteraddingproduct', N'true')
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.includefeaturedproductsinnormallists')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.includefeaturedproductsinnormallists', N'false')
END
GO



--new 'Allow Customer Impersonation' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'AllowCustomerImpersonation')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Allow Customer Impersonation', N'AllowCustomerImpersonation', N'Customers')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admin role be default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.freeshippingoverxincludingtax')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shippingsettings.freeshippingoverxincludingtax', N'false')
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fedexsettings.passdimensions')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'fedexsettings.passdimensions', N'false')
END
GO

--'Keep alive' schedule task
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ScheduleTask]
		WHERE [Name] = N'Keep alive')
BEGIN
	INSERT [dbo].[ScheduleTask] ([Name], [Seconds], [Type], [Enabled], [StopOnError])
	VALUES (N'Keep alive', 300, N'Nop.Services.Common.KeepAliveTask, Nop.Services', 1, 0)
END
GO

--clear serialized shipping options
DELETE FROM CustomerAttribute
WHERE [Key] like N'LastShippingOption'
GO
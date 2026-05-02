--upgrade scripts from nopCommerce 4.20 to 4.30
--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification">
    <Value>HTML minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Hint">
    <Value>
      Specification attributes are product features i.e, screen size, number of USB-ports, visible on product details page. Specification attributes can be used for filtering products on the category details page. Unlike product attributes, specification attributes are used for information purposes only.
      You can add attribute for your product using existing list of attributes, or if you need to create a new one go to Catalog &gt; Attributes &gt; Specification attributes.
    </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxCategory.Hint">
    <Value>Select default tax category for products. It''ll be pre-selected on the "Add new product" page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.TaxCategory.Hint">
    <Value>The tax classification for this attribute (used to calculate tax). You can manage tax categories by selecting Configuration &gt; Tax &gt; Tax Categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute.Hint">
    <Value>Choose a product specification attribute. You can manage specification attributes from Catalog &gt; Attributes &gt; Product Specifications.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderTotalDiscount.Hint">
    <Value>The total discount applied to this order. Manage your discounts from Promotions &gt; Discounts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PaymentMethod.Hint">
    <Value>The payment method used for this transaction. You can manage Payment Methods from Configuration &gt; Payment &gt; Payment Methods.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippingMethod.Hint">
    <Value>The customers chosen shipping method for this order. You can manage shipping methods from Configuration &gt; Shipping &gt; Shipping Methods.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Tax.Hint">
    <Value>Total tax applied to this order. Manage your tax settings from Configuration &gt; Tax.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue">
    <Value>Error while save attribute combinations. Attribute value not specified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum">
    <Value>Show on forum</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum.Hint">
    <Value>Check to show CAPTCHA on forum, when editing and creating a topic or post.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanDeliver">
    <Value>Delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanDeliver.Hint">
    <Value>Check to apply current date to delivery.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanShip">
    <Value>Shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanShip.Hint">
    <Value>Check to apply current date to shipment.</Value>
  </LocaleResource>
  <LocaleResource Name="ShippingMethod.NotAvailableMethodsError">
    <Value>Your order cannot be completed at this time as there is no shipping methods available for it. Please make necessary changes in your shipping address.</Value>
  </LocaleResource>
  <LocaleResource Name="ShippingMethod.SpecifyMethodError">
    <Value>Please specify shipping method.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.NotAvailableMethodsError">
    <Value>Your order cannot be completed at this time as there is no payment methods available for it.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.SpecifyMethodError">
    <Value>Please specify payment method.</Value>
  </LocaleResource>
  <LocaleResource Name="AjaxCart.Failure">
    <Value>Failed to add the product. Please refresh the page and try one more time.</Value>
  </LocaleResource>
  <LocaleResource Name="MainMenu.AjaxFailure">
    <Value>Failed to open menu. Please refresh the page and try one more time.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Token.Key">
    <Value>Verification token</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Use3ds">
    <Value>Use 3D-Secure</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Use3ds.Hint">
    <Value>Determine whether to use 3D-Secure feature. Used for Strong customer authentication (SCA). SCA is generally friction-free for the buyer, but a card-issuing bank may require additional authentication for some payments. In those cases, the buyer must verify their identiy with the bank using an additional secure dialog.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles.Hint">
    <Value>Choose one or several customer roles i.e. administrators, vendors, guests, who will be able to see this product in catalog. If you don''t need this option just leave this field empty. In order to use this functionality, you have to disable the following setting: Configuration &gt; Settings &gt; Catalog &gt; Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.CannotBeFound">
    <Value>The coupon code cannot be found</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Empty">
    <Value>The coupon code is empty</Value>
  </LocaleResource>
  <LocaleResource Name="PrivateMessages.Inbox.NoItems">
    <Value>No inbox messages</Value>
  </LocaleResource>
  <LocaleResource Name="PrivateMessages.Sent.NoItems">
    <Value>No sent messages</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude">
    <Value>Latitude</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.Hint">
    <Value>Specify a latitude (DD.dddddddd°).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange">
    <Value>Latitude should be in range -90 to 90</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision">
    <Value>Precision should be less then 8</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue">
    <Value>Latitude and Longitude should be specify together</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude">
    <Value>Longitude</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.Hint">
    <Value>Specify a longitude (DD.dddddddd°).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange">
    <Value>Longitude should be in range -180 to 180</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision">
    <Value>Precision should be less then 8</Value>
  </LocaleResource>
    <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue">
    <Value>Latitude and Longitude should be specify together</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.LoadNotDelivered">
    <Value>Load not delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.LoadNotDelivered.Hint">
    <Value>Load only undelivered shipments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Newsletter.Hint">
    <Value>Choose stores to subscribe to newsletter.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.RedirectMessage">
    <Value>Redirected</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Instructions">
    <Value>
        <![CDATA[
            <div style="margin: 0 0 10px;">
                <em><b>Warning: Square sandbox data has been changed. For more information visit our <a href="https://docs.nopcommerce.com/user-guide/configuring/settingup/payments/methods/square.html" target="_blank">documentation</a>.</em></b><br />
                <br />
                For plugin configuration, follow these steps:<br />
                <br />
                1. You will need a Square Merchant account. If you don''t already have one, you can sign up here: <a href="http://squ.re/nopcommerce" target="_blank">https://squareup.com/signup/</a><br />
                2. Sign in to ''Square Merchant Dashboard''. Go to ''Account & Settings'' &#8594; ''Locations'' tab and create new location.<br />
                <em>   Important: Your merchant account must have at least one location with enabled credit card processing. Please refer to the Square customer support if you have any questions about how to set this up.</em><br />
                3. Sign in to your ''Square Developer Dashboard'' at <a href="http://squ.re/nopcommerce1" target="_blank">https://connect.squareup.com/apps</a>; use the same login credentials as your merchant account.<br />
                4. Click on ''Create Your First Application'' and fill in the ''Application Name''. This name is for you to recognize the application in the developer portal and is not used by the plugin. Click ''Create Application'' at the bottom of the page.<br />
                5. Now you are on the details page of the previously created application. On the ''Credentials'' tab click on the ''Change Version'' button and choose ''2019-09-25''.<br />
                6. Make sure you uncheck ''Use sandbox'' below.<br />
                7. In the ''Square Developer Dashboard'' go to the details page of the your previously created application:
                    <ul>
                        <li>On the ''Credentials'' tab make sure the ''Application mode'' setting value is ''Production''</li>
                        <li>On the ''Credentials'' tab copy the ''Application ID'' and paste it into ''Application ID'' below</li>
                        <li>Go to ''OAuth'' tab. Click ''Show'' on the ''Application Secret'' field. Copy the ''Application Secret'' and paste it into ''Application Secret'' below</li>
                        <li>Copy this URL: <em>{0}</em>. On the ''OAuth'' tab paste this URL into ''Redirect URL''. Click ''Save''</li>
                    </ul>
                8. Click ''Save'' below to save the plugin configuration.<br />
                9. Click ''Obtain access token'' below; the Access token field should populate.<br />
                <em>Note: If for whatever reason you would like to disable an access to your accounts, simply ''Revoke access tokens'' below.</em><br />
                10. Choose the previously created location. ''Location'' is a required parameter for payment requests.<br />
                11. Fill in the remaining fields and click ''Save'' to complete the configuration.<br />
                <br />
                <em>Note: The payment form must be generated only on a webpage that uses HTTPS.</em><br />
            </div>
        ]]>
    </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIconsArchive.Hint">
    <Value>Upload archive with favicon and app icons for different operating systems and devices. You can see an example of the favicon and app icons archive in /icons/samples in the root of the site. Your favicon and app icons path is "/icons/icons_{0}"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Wishlist.EmailAFriend">
    <Value><![CDATA[This message template is used when a customer wants to share some product from the wishlist with a friend by sending an email. You can set up this option by ticking the checkbox Allow customers to email their wishlists in Configuration - Settings - Shopping cart settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FirstNameEnabled">
    <Value>''First name'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FirstNameEnabled.Hint">
    <Value>Set if ''First name'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FirstNameRequired">
    <Value>''First name'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FirstNameRequired.Hint">
    <Value>Check if ''First name'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.LastNameEnabled">
    <Value>''Last name'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.LastNameEnabled.Hint">
    <Value>Set if ''Last name'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.LastNameRequired">
    <Value>''Last name'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.LastNameRequired.Hint">
    <Value>Check if ''Last name'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Payment.Methods.DownloadMorePlugins">
    <Value><![CDATA[You can download more plugins in our <a href="https://www.nopcommerce.com/extensions?category=payment-modules&utm_source=admin-panel&utm_medium=payment-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.DownloadMorePlugins">
    <Value><![CDATA[You can download more nopCommerce plugins in our <a href="https://www.nopcommerce.com/marketplace?utm_source=admin-panel&utm_medium=plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Instructions">
    <Value><![CDATA[Here you can find third-party extensions and themes which are developed by our community and partners. They are also available in our <a href="https://www.nopcommerce.com/marketplace?utm_source=admin-panel&utm_medium=official-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.GetMore">
    <Value><![CDATA[You can get more themes in our <a href="https://www.nopcommerce.com/themes?utm_source=admin-panel&utm_medium=theme-settings&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Providers.DownloadMorePlugins">
    <Value><![CDATA[You can download more plugins in our <a href="https://www.nopcommerce.com/extensions?category=shipping-delivery&utm_source=admin-panel&utm_medium=shipping-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Tax.Providers.DownloadMorePlugins">
    <Value><![CDATA[You can download more plugins in our <a href="https://www.nopcommerce.com/extensions?category=taxes&utm_source=admin-panel&utm_medium=tax-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Microdata">
    <Value>Microdata tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Microdata.Hint">
    <Value>Check to generate Microdata tags on the product details page.</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Promotions.Discounts.Fields.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AdminComment.Hint">
    <Value>This comment is for internal use only, not visible for customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Localization.UploadLocalePattern">
    <Value>Set CLDR for current culture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Localization.Description">
    <Value><![CDATA[Sets the <a href="http://cldr.unicode.org/" target="_blank">CLDR</a> pattern for localization of client-side validation according to the current culture]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LocalePattern.SuccessUpload">
    <Value>Localization patterns for the current culture loaded successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Products.Qty.AriaLabel">
    <Value>Enter a quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchEnabled">
    <Value>Search enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchEnabled.Hint">
    <Value>Check to enabled the search box.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.County">
    <Value>County / region</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.County.Required">
    <Value>County / region is required.</Value>
  </LocaleResource>
   <LocaleResource Name="Address.Fields.County">
    <Value>County / region</Value>
  </LocaleResource>
  <LocaleResource Name="Address.Fields.County.Required">
    <Value>County / region is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County">
    <Value>County / region</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County.Hint">
    <Value>Enter county / region.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County.Required">
    <Value>County / region is required.</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled">
    <Value>''County / region'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled.Hint">
    <Value>Set if ''County / region'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired">
    <Value>''County / region'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired.Hint">
    <Value>Check if ''County / region'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyEnabled">
    <Value>''County / region'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyEnabled.Hint">
    <Value>Set if ''County / region'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyRequired">
    <Value>''County / region'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyRequired.Hint">
    <Value>Check if ''County / region'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County">
    <Value>County / region</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County.Hint">
    <Value>The county / region.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County.Required">
    <Value>County / region is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Address.County">
    <Value>County / region</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.County">
    <Value>County / region</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.County.Hint">
    <Value>Search by a specific county / region.</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Post.IsUseful">
    <Value>This post/answer is useful</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Post.IsNotUseful">
    <Value>This post/answer is not useful</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAlreadySentQueuedEmails">
    <Value>Delete already sent emails</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.TotalDeleted">
    <Value>{0} emails were deleted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.TopicName">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisplayPickupInStoreOnShippingMethodPage">
    <Value>Display "Pickup in store" on "Shipping method" page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisplayPickupInStoreOnShippingMethodPage.Hint">
    <Value>Display "Pickup in store" options on "Shipping method" page; otherwise display on the "Shipping address" page.</Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaType">
    <Value>Type of reCAPTCHA</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaType.Hint">
    <Value>Select a type of reCAPTCHA. Also check if ''reCAPTCHA public key'' and ''reCAPTCHA private key'' fields uses the same reCAPTCHA version.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaV3ScoreThreshold">
    <Value>reCAPTCHA v3 score threshold</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaV3ScoreThreshold.Hint">
    <Value>Select a reCAPTCHA v3 score threshold (1.0 is very likely a good interaction, 0.0 is very likely a bot). By default, you can use a threshold of 0.5.</Value>
  </LocaleResource>
    <LocaleResource Name="Enums.Nop.Core.Domain.Security.CaptchaType.CheckBoxReCaptchaV2">
    <Value>reCAPTCHA v2 ("I''m not a robot" Checkbox)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.CaptchaType.ReCaptchaV3">
    <Value>reCAPTCHA v3</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.FriendlyName">
    <Value>Plugin name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.FriendlyName.Hint">
    <Value>Search by a plugin name.</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Plugins.Author">
    <Value>Author</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Author.Hint">
    <Value>Search by author.</Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Vendors.List.SearchEmail">
    <Value>Vendor Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.List.SearchEmail.Hint">
    <Value>Search by a vendor Email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle">
    <Value>Title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle.Hint">
    <Value>Search by a blog post title.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.List.SearchTitle">
    <Value>Title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.List.SearchTitle.Hint">
    <Value>Search by a news items title.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.Warehouse.SearchName">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.Warehouse.SearchName.Hint">
    <Value>Search by warehouse name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchPublished">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchPublished.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchPublished.Hint">
    <Value>Search by a "Published" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchPublished.PublishedOnly">
    <Value>Published only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchPublished.UnpublishedOnly">
    <Value>Unpublished only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchPublished">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchPublished.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchPublished.Hint">
    <Value>Search by a "Published" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchPublished.PublishedOnly">
    <Value>Published only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchPublished.UnpublishedOnly">
    <Value>Unpublished only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.Fields.SearchTagName">
    <Value>Tag name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.Fields.SearchTagName.Hint">
    <Value>Search by a tag name.</Value>
  </LocaleResource>
  <LocaleResource Name="BackInStockSubscriptions.Notification.Subscribed">
    <Value>Subscribed</Value>
  </LocaleResource>
  <LocaleResource Name="BackInStockSubscriptions.Notification.Unsubscribed">
    <Value>Unsubscribed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ChooseShippingTitle">
    <Value>Shipping Method</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShipping.Country.Required">
    <Value>Country is required</Value>
  </LocaleResource>
  <LocaleResource Name="Products.EstimateShipping.EstimatedDeliveryPrefix">
    <Value>Estimated Delivery on</Value>
  </LocaleResource>
  <LocaleResource Name="Products.EstimateShipping.NoSelectedShippingOption">
    <Value>Please select the address you want to ship from</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.NoShippingOptions">
    <Value>No shipping options</Value>
  </LocaleResource>
  <LocaleResource Name="Products.EstimateShipping.PriceTitle">
    <Value>Shipping:</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.SelectShippingOption.Button">
    <Value>Apply</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ShippingOption.EstimatedDelivery">
    <Value>Estimated Delivery</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ShippingOption.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ShippingOption.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ShipToTitle">
    <Value>Ship to</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Products.EstimateShipping.ToAddress">
    <Value>to</Value>
  </LocaleResource>
  <LocaleResource Name="Products.EstimateShipping.ViaProvider">
    <Value>via</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ZipPostalCode">
    <Value>Zip / postal code</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShipping.ZipPostalCode.Required">
    <Value>Zip / postal code is required</Value>
  </LocaleResource>
  <LocaleResource Name="Products.QuantityShouldBePositive">
    <Value>Quantity should be positive</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays">
    <Value>Transit days</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays.Hint">
    <Value>The number of days of delivery of the goods.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.TransitDays">
    <Value>Transit days</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.TransitDays.Hint">
    <Value>The number of days of delivery of the goods to pickup point.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.ZipPostalCode.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.Country.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.ZipPostalCode">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.StateProvince">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.Country">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.ShippingOptionWithRate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.EstimateShipping.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingEnabled.Hint">
    <Value>Check to allow customers to estimate shipping on product and shopping cart pages.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIconsArchive">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIconsArchive.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIcons">
    <Value>Upload single icon or icons archive</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIcons.FileExtensions">
    <Value>Ico or zip file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIcons.Hint">
    <Value>Upload single icon or archive with favicon and app icons for different operating systems and devices. You can see an example of the favicon and app icons archive in /icons/samples in the root of the site. Your favicon and app icons path is "/icons/icons_{0}"</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.UploadNewIconsArchive">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.UploadNewIcons">
    <Value>Uploaded a new favicon and app icons for store (ID = ''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile">
    <Value>XML file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.BackToList">
    <Value>back to product tags list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.Updated">
    <Value>The product tag has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.Deleted">
    <Value>The product tag has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step4">
    <Value>Click on the ''Install'' link to choose the plugin for install.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step5">
    <Value>Click on the ''Restart application to apply changes'' button on the top panel to finish the installation process.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MarkAsNewEndDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MarkAsNewStartDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisplayStockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.AddToCart.NoAddedItems">
    <Value>No products selected to add to cart.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Tabs">
    <Value>Panels and product species</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Sales.Bestsellers.Total">
    <Value>Total</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.CommonInfo">
    <Value>Product info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Price">
    <Value>Prices</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.IsAdmin">
    <Value>Administrator cannot apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.Address.NotFound">
    <Value>Address can''t be loaded</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>  
  <LocaleResource Name=" Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Tax.Categories.Fields.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Category.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Manufacturer.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.DisplayOrder.Required">
    <Value>Please provide a display order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio.Required">
    <Value>Please provide a weight ratio.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Ratio.Required">
    <Value>Please provide a dimension ratio.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Clear">
    <Value>Clear activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.RequireOtherProductsAddedToTheCart">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.RequireOtherProductsAddedToCart">
    <Value>Require other products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name=" Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Tax.Categories.Fields.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Category.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Manufacturer.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.DisplayOrder.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio.Required">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Ratio.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Result.EmailValidation">
    <Value>Your registration has been successfully completed. You have just been sent an email containing activation instructions.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoice.NoOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.NoOrders">
    <Value>No orders selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.NoProducts">
    <Value>No products selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.NoCustomers">
    <Value>No customers selected</Value>
  </LocaleResource>
  <LocaleResource Name="Address.OtherNonUS">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Address.Other">
    <Value>Other</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.OtherNonUS">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Other">
    <Value>Other</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PluginNotEnabled">
    <Value>Uninstall and delete the plugin(s) that you don''t use</Value>
  </LocaleResource>
  <LocaleResource Name="BackInStockSubscriptions.Notification.Subscribed">
    <Value>You''ve successfully subscribed</Value>
  </LocaleResource>
  <LocaleResource Name="BackInStockSubscriptions.Notification.Unsubscribed">
    <Value>You''ve successfully unsubscribed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog">
    <Value>Activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.ActivityLogType">
    <Value>Activity log type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.ActivityLogType.Hint">
    <Value>The activity log type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogTypeColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.Comment">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.Comment">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CreatedOn">
    <Value>Created On</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnFrom">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CreatedOnFrom">
    <Value>Created from</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnFrom.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CreatedOnFrom.Hint">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnTo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CreatedOnTo">
    <Value>Created to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnTo.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CreatedOnTo.Hint">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.Customer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.Customer">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CustomerEmail">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CustomerEmail">
    <Value>Customer Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CustomerEmail.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.CustomerEmail.Hint">
    <Value>A customer Email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLog.Fields.IpAddress">
    <Value>IP address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLogType">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLogType">
    <Value>Activity Types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLogType.Fields.Enabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLogType.Fields.Enabled">
    <Value>Is Enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLogType.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLogType.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLogType.Updated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.ActivityLogType.Updated">
    <Value>The types have been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.CustomerRoles.Fields.CustomerRoleId.Required">
    <Value>Customer role is required</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.CustomerRoles.Fields.DiscountId.Required">
    <Value>Discount is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Phone.NotValid">
    <Value>Phone number is not valid</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneNumberValidationEnabled">
    <Value>Phone number validation is enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneNumberValidationEnabled.Hint">
    <Value>Check to enable phone number validation (when registering or changing on the "My Account" page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneNumberValidationRule">
    <Value>Phone number validation rule</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneNumberValidationRule.Hint">
    <Value>Set the validation rule for phone number. You can specify a list of allowed characters or a regular expression. If you use a regular expression check the "Use regex for phone number validation" setting.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneNumberValidationUseRegex">
    <Value>Use regex for phone number validation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneNumberValidationUseRegex.Hint">
    <Value>Check to use a regular expression for phone number validation (when registering or changing on the "My Account" page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerSettings.PhoneNumberRegexValidationRule.Error">
    <Value>The regular expression for phone number validation is incorrect</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.ShippingOption.IsNotFound">
    <Value>Selected shipping option is not found</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.Pickup.PriceFrom">
    <Value>From {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.AccountAlreadyAssigned">
    <Value>Account is already assigned</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DefaultLanguage.DefaultItemText">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.EmptyItemText">
    <Value>---</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RentalPriceLength.ShouldBeGreaterThanZero">
    <Value>Rental period length should be greater 0.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.IsActive.Hint">
    <Value>Search by a "IsActive" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.IsActive.ActiveOnly">
    <Value>Active only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.IsActive.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.IsActive.InactiveOnly">
    <Value>Inactive only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.Language">
    <Value>Language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.Language.Hint">
    <Value>Search by a "Language" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.Language.Standard">
    <Value>Standard</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.List.Name.Hint">
    <Value>A name to find.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Name.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.DimensionsType">
    <Value>Dimensions type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.DimensionsType.Hint">
    <Value>Choose dimensions type (inches or centimeters).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.WeightType">
    <Value>Weight type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.WeightType.Hint">
    <Value>Choose the weight type (pounds or kilograms).</Value>
  </LocaleResource>
  <LocaleResource Name="Shipping.EstimateShippingPopUp.Product.IsNotFound">
    <Value>Product is not found</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingCartPageEnabled">
    <Value>Estimate shipping enabled (cart page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingCartPageEnabled.Hint">
    <Value>Check to allow customers to estimate shipping on the shopping cart page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingProductPageEnabled">
    <Value>Estimate shipping enabled (product page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingProductPageEnabled.Hint">
    <Value>Check to allow customers to estimate shipping on the product details pages. Please note that all the shipping provider APIs will be called on the product details page. Also the final shipping rate in the cart may not be exactly equal to the sum of all the individual estimates.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingEnabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.EstimateShippingEnabled.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.Avalara.Fields.AccountId.Required">
    <Value>Account ID is required</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.Avalara.Fields.EnableLogging">
    <Value>Enable logging</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.Avalara.Fields.EnableLogging.Hint">
    <Value>Determine whether to enable logging of all requests to Avalara services.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.Avalara.Fields.LicenseKey.Required">
    <Value>Account license key is required</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.Avalara.Fields.TaxOriginAddressType.DefaultTaxAddress.Warning">
    <Value><![CDATA[Ensure that you have correctly filled in the ''Default tax address'' under <a href=\"{0}\" target=\"_blank\">Tax settings</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.Avalara.Fields.TaxOriginAddressType.ShippingOrigin.Warning">
    <Value><![CDATA[Ensure that you have correctly filled in the ''Shipping origin'' under <a href=\"{0}\" target=\"_blank\">Shipping settings</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.NivoSlider.AltText">
    <Value>Image alternate text</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.NivoSlider.AltText.Hint">
    <Value>Enter alternate text that will be added to image.</Value>
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
				[LanguageId],
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

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.showonforum')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.showonforum', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.recaptcharequesttimeout')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.recaptcharequesttimeout', 20, 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'squarepaymentsettings.use3ds')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'squarepaymentsettings.use3ds', 'false', 0)
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[StorePickupPoint]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[StorePickupPoint]') AND NAME = 'Latitude')
BEGIN
	ALTER TABLE [StorePickupPoint]
	ADD Latitude decimal(18, 8) NULL
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[StorePickupPoint]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[StorePickupPoint]') AND NAME = 'Longitude')
BEGIN
	ALTER TABLE [StorePickupPoint]
	ADD Longitude decimal(18, 8) NULL
END
GO

-- update the "DeleteGuests" stored procedure
ALTER PROCEDURE [DeleteGuests]
(
	@OnlyWithoutShoppingCart bit = 1,
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (CustomerId int)
		
	INSERT #tmp_guests (CustomerId)
	SELECT c.[Id] 
	FROM [Customer] c with (NOLOCK)
		LEFT JOIN [ShoppingCartItem] sci with (NOLOCK) ON sci.[CustomerId] = c.[Id]
		INNER JOIN (
			--guests only
			SELECT ccrm.[Customer_Id] 
			FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK)
				INNER JOIN [CustomerRole] cr with (NOLOCK) ON cr.[Id] = ccrm.[CustomerRole_Id]
			WHERE cr.[SystemName] = N'Guests'
		) g ON g.[Customer_Id] = c.[Id]
		LEFT JOIN [Order] o with (NOLOCK) ON o.[CustomerId] = c.[Id]
		LEFT JOIN [BlogComment] bc with (NOLOCK) ON bc.[CustomerId] = c.[Id]
		LEFT JOIN [NewsComment] nc with (NOLOCK) ON nc.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReview] pr with (NOLOCK) ON pr.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReviewHelpfulness] prh with (NOLOCK) ON prh.[CustomerId] = c.[Id]
		LEFT JOIN [PollVotingRecord] pvr with (NOLOCK) ON pvr.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Topic] ft with (NOLOCK) ON ft.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Post] fp with (NOLOCK) ON fp.[CustomerId] = c.[Id]
	WHERE 1 = 1
		--no orders
		AND (o.Id is null)
		--no blog comments
		AND (bc.Id is null)
		--no news comments
		AND (nc.Id is null)
		--no product reviews
		AND (pr.Id is null)
		--no product reviews helpfulness
		AND (prh.Id is null)
		--no poll voting
		AND (pvr.Id is null)
		--no forum topics
		AND (ft.Id is null)
		--no forum topics
		AND (fp.Id is null)
		--no system accounts
		AND (c.IsSystemAccount = 0)
		--created from
		AND ((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
		--created to
		AND ((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
		--shopping cart items
		AND ((@OnlyWithoutShoppingCart = 0) OR (sci.Id is null))
	
	--delete guests
	DELETE [Customer]
	WHERE [Id] IN (SELECT [CustomerId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [CustomerId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'Customer')
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.recaptchaapiurl')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.recaptchaapiurl', N'https://www.google.com/recaptcha/', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'seosettings.microdataenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'seosettings.microdataenabled', 'true', 0)
END
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'securitysettings.enablexsrfprotectionforadminarea')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'securitysettings.enablexsrfprotectionforadminarea'
END
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'securitysettings.enablexsrfprotectionforpublicstore')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'securitysettings.enablexsrfprotectionforpublicstore'
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='RedeemedRewardPointsEntryId')
BEGIN
	ALTER TABLE [Order] ADD	RedeemedRewardPointsEntryId int NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Discount] ADD 	AdminComment nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RewardPointsHistory]') and NAME='OrderId')
BEGIN
	ALTER TABLE [RewardPointsHistory] ADD OrderId int NULL
END
GO

--fluent migrator
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[MigrationVersionInfo]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [MigrationVersionInfo](
	[AppliedOn] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Version] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddAffiliateAddressFK', 637097594562551771)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogCommentBlogPostFK', 637097605404497785)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogCommentCustomerFK', 637097605404497786)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogCommentStoreFK', 637097605404497787)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogPostLanguageFK', 637097607595956342)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBackInStockSubscriptionProductFK', 637097608748261630)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddBackInStockSubscriptionCustomerFK', 637097608748261631)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddPredefinedProductAttributeValueProductAttributeFK', 637097611590754490)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductAttributeMappingProductFK', 637097615386806324)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductAttributeMappingProductAttributeFK', 637097615386806325)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductAttributeValueProductAttributeMappingFK', 637097616507544540)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductCategoryCategoryFK', 637097618625689396)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductCategoryProductFK', 637097618625689397)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductManufacturerManufacturerFK', 637097620539067594)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductManufacturerProductFK', 637097620539067595)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductPictureProductPictureFK', 637097627662625749)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductPictureProductProductFK', 637097627662625750)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddAddProductProductTagProductFK', 637097631880193450)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddAddProductProductTagProductTagFK', 637097631880193451)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewHelpfulnessProductReviewFK', 637097639558603530)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewProductFK', 637097639998948304)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewCustomerFK', 637097639998948305)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewStoreFK', 637097639998948306)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewReviewTypeMappingProductReviewFK', 637097643602513441)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewReviewTypeMappingReviewTypeFK', 637097643602513442)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductSpecificationAttributeSpecificationAttributeOptionFK', 637097645462261985)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductSpecificationAttributeProductFK', 637097645462261986)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductWarehouseInventoryProductFK', 637097650980051780)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductWarehouseInventoryWarehouseFK', 637097650980051781)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddSpecificationAttributeOptionSpecificationAttributeFK', 637097653366619708)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddStockQuantityHistoryProductFK', 637097656165419186)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddStockQuantityHistoryWarehouseFK', 637097656165419187)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddTierPriceProductFK', 637097657438051844)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddTierPriceCustomerRoleFK', 637097657438051845)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddAddressAttributeValueAddressAttributeValueFk', 637097693526459118)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddAddressCountryFK', 637097696240659480)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddAddressStateProvinceFK', 637097696240659481)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddCustomerAddressCustomerFK', 637097698595245358)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerAddressAddressFK', 637097698595245359)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerAttributeValueCustomerAttributeFK', 637097701504308129)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerCustomerRoleCustomerFK', 637097703237489896)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerCustomerRoleCustomerRoleFK', 637097703237489897)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerBillingAddressFK', 637097705651641381)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerShippingAddressFK', 637097705651641382)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerPasswordCustomerFK', 637097707461276491)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddExternalAuthenticationRecordCustomerFK', 637097708449096139)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddRewardPointsHistoryCustomerFK', 637097709252342366)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddRewardPointsHistoryOrderFK', 637097709252342367)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddStateProvinceCountryFK', 637097713433797964)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountCategoryDiscountFK', 637097771695936887)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountCategoryCategoryFK', 637097771695936888)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountManufacturerDiscountFK', 637097774149883528)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountManufacturerManufacturerFK', 637097774149883529)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountProductDiscountFK', 637097778951975256)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountProductProductFK', 637097778951975257)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountUsageHistoryDiscountFK', 637097780041180783)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountUsageHistoryOrderFK', 637097780041180784)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumForumGroupFK', 637097783627313370)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumPostForumTopicFK', 637097784463004325)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumPostCustomerFK', 637097784463004326)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumPostVoteForumPostFK', 637097787633262801)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumSubscriptionCustomerFK', 637097788387699848)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumTopicForumFK', 637097789101910240)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumTopicCustomerFK', 637097789101910241)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddPrivateMessageFromCustomerFK', 637097790373669695)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddPrivateMessageToCustomerFK', 637097790373669696)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddLocaleStringResourceLanguageFK', 637097792951964555)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddLocalizedPropertyLanguageFK', 637097793590515436)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddActivityLogActivityLogTypeFK', 637097794508380329)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddActivityLogCustomerFK', 637097794508380330)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddLogCustomerFK', 637097795893561926)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddPictureBinaryPictureFK', 637097796695631609)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddQueuedEmailEmailAccountFK', 637097797031655781)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsCommentNewsItemFK', 637097798362530772)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsCommentCustomerFK', 637097798362530773)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsCommentStoreFK', 637097798362530774)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsItemLanguageFK', 637097800094361423)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddCheckoutAttributeValueCheckoutAttributeFK', 637097801078553212)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddGiftCardOrderItemFK', 637097802922130581)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddGiftCardUsageHistoryGiftCardFK', 637097803156452475)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddGiftCardUsageHistoryOrderFK', 637097803156452476)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddOrderItemOrderFK', 637097804609436788)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddOrderItemProductFK', 637097804609436789)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderCustomerFK', 637097805896028942)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderBillingAddressFK', 637097805896028943)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderShippingAddressFK', 637097805896028944)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderPickupAddressFK', 637097805896028945)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderNoteOrderFK', 637097808997123308)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddRecurringPaymentHistoryRecurringPaymentFK', 637097810210887644)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddRecurringPaymentOrderFK', 637097811410960207)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddReturnRequestCustomerFK', 637097812291248082)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShoppingCartItemCustomerFK', 637097813093371767)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShoppingCartItemProductFK', 637097813093371768)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollAnswerPollFK', 637097815487520229)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollLanguageFK', 637097816025962851)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollVotingRecordPollAnswerFK', 637097817036693383)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollVotingRecordCustomerFK', 637097817036693384)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddAclRecordCustomerRoleFK', 637097818436073081)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPermissionRecordCustomerRoleCustomerRoleFK', 637097819107801301)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPermissionRecordCustomerRolePermissionRecordFK', 637097819107801302)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShipmentItemShipmentFK', 637097820921984734)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShipmentOrderFK', 637097821681126845)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShippingMethodCountryCountryFK', 637097822410528356)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShippingMethodCountryShippingMethodFK', 637097822410528357)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddStoreMappingStoreFK', 637097823639005655)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddVendorAttributeValueVendorAttributeFK', 637097824346411077)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddVendorNoteVendorFK', 637097824991868645)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddDiscountRequirementDiscountFK', 637118390520043560)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddDiscountRequirementDiscountRequirementFK', 637118390520043561)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderRewardPointsHistoryFK', 637121109617140897)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddProductAttributeCombinationProductFk', 637121110600830411)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddLocaleStringResourceIX', 637123449689037677)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddProductPriceDatesEtcIX', 637123449689037678)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddCountryDisplayOrderIX', 637123449689037679)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddLogCreatedOnUtcIX', 637123449689037680)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddCustomerEmailIX', 637123449689037681)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddCustomerUsernameIX', 637123449689037682)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerCustomerGuidIX', 637123449689037683)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerSystemNameIX', 637123449689037684)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerCreatedOnUtcIX', 637123449689037685)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddGenericAttributeEntityIdKeyGroupIX', 637123449689037686)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddQueuedEmailCreatedOnUtcIX', 637123449689037687)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddOrderCreatedOnUtcIX', 637123449689037688)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddLanguageDisplayOrderIX', 637123449689037689)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddNewsletterSubscriptionEmailStoreIdIX', 637123449689037690)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddShoppingCartItemShoppingCartTypeIdCustomerIdIX', 637123449689037691)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddRelatedProductProductId1IX', 637123449689037692)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX', 637123449689037693)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductProductAttributeMappingProductIdDisplayOrderIX', 637123449689037694)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddManufacturerDisplayOrderIX', 637123449689037695)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryDisplayOrderIX', 637123449689037696)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryParentCategoryIdIX', 637123449689037697)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsGroupDisplayOrderIX', 637123449689037698)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsForumDisplayOrderIX', 637123449689037699)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsSubscriptionForumIdIX', 637123449689037700)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsSubscriptionTopicIdIX', 637123449689037701)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductDeletedPublishedIX', 637123449689037702)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductPublishedIX', 637123449689037703)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductShowOnHomepageIX', 637123449689037704)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductParentGroupedProductIdIX', 637123449689037705)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductVisibleIndividuallyIX', 637123449689037706)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPCMProductCategoryIX', 637123449689037707)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCurrencyDisplayOrderIX', 637123449689037708)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductTagNameIX', 637123521091647925)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddActivityLogCreatedOnUtcIX', 637123521091647926)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddUrlRecordSlugIX', 637123521091647927)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddUrlRecordCustom1IX', 637123521091647928)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddAclRecordEntityIdEntityNameIX', 637123521091647929)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddStoreMappingEntityIdEntityNameIX', 637123521091647930)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryLimitedToStoresIX', 637123521091647931)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddManufacturerLimitedToStoresIX', 637123521091647932)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductLimitedToStoresIX', 637123521091647933)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategorSubjectToAclIX', 637123521091647934)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddManufacturerSubjectToAclIX', 637123521091647935)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductSubjectToAclIX', 637123521091647936)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductCategoryMappingIsFeaturedProductIX', 637123521091647937)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductManufacturerMappingIsFeaturedProductIX', 637123521091647938)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerCustomerRoleMappingCustomerIdIX', 637123521091647939)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductDeleteIdIX', 637123521091647940)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddGetLowStockProductsIX', 637123521091647941)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPMMProductManufacturerIX', 637123521091647942)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPCMProductIdExtendedIX', 637123537559280389)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPMMProductIdExtendedIX', 637123537559280390)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPSAMAllowFilteringIX', 637123537559280391)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX', 637123537559280392)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX', 637123537559280393)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductVisibleIndividuallyPublishedDeletedExtendedIX', 637123537559280394)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryDeletedExtendedIX', 637123537559280395)
END
GO

-- update the "ProductLoadAllPaged" stored procedure
ALTER PROCEDURE [ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchManufacturerPartNumber bit = 0, -- a value indicating whether to search by a specified "keyword" in manufacturer part number
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@OverridePublished	bit = null, --null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@FilterableSpecificationAttributeOptionIds nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	/* Products that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ProductId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@OriginalKeywords nvarchar(4000),
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	SET @OriginalKeywords = @Keywords
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, LEN(@Keywords) - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, LEN(@Keywords)) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--product name
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Name]) > 0 '

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			IF @UseFullTextSearch = 1
			BEGIN
				SET @sql = @sql + 'OR CONTAINS(p.[ShortDescription], @Keywords) '
				SET @sql = @sql + 'OR CONTAINS(p.[FullDescription], @Keywords) '
			END
			ELSE
			BEGIN
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[ShortDescription]) > 0 '
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[FullDescription]) > 0 '
			END
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + 'OR p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + 'OR p.[Sku] = @OriginalKeywords '
		END

		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND ( (lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

		IF @SearchDescriptions = 1
		BEGIN
			--localized product short description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

			--localized product full description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '
		END

		SET @sql = @sql + ' ) '

		IF @SearchProductTags = 1
		BEGIN
			--product tags (exact match)
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE pt.[Name] = @OriginalKeywords '

			--localized product tags
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name''
				AND lp.[LocaleValue] = @OriginalKeywords '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')
	DECLARE @FilteredCustomerRoleIdsCount int	
	SET @FilteredCustomerRoleIdsCount = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)
	
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

	SET @sql = '
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ProductId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN ('
		
		SET @sql = @sql + + CAST(@CategoryIds AS nvarchar(max))

		SET @sql = @sql + ')'

		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pcm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by manufacturer
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		AND pmm.ManufacturerId = ' + CAST(@ManufacturerId AS nvarchar(max))
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pmm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
	END
	
	--filter by warehouse
	IF @WarehouseId > 0
	BEGIN
		--we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		--but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET @sql = @sql + '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory [pwi]
					WHERE [pwi].WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ' AND [pwi].ProductId = p.Id))
			)'
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	--"Published" property
	IF (@OverridePublished is null)
	BEGIN
		--process according to "showHidden"
		IF @ShowHidden = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Published = 1'
		END
	END
	ELSE IF (@OverridePublished = 1)
	BEGIN
		--published only
		SET @sql = @sql + '
		AND p.Published = 1'
	END
	ELSE IF (@OverridePublished = 0)
	BEGIN
		--unpublished only
		SET @sql = @sql + '
		AND p.Published = 0'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(p.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')'
	END
	
	--show hidden and ACL
	IF  @ShowHidden = 0 and @FilteredCustomerRoleIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
    --prepare filterable specification attribute option identifier (if requested)
    IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
        DECLARE @sql_filterableSpecs nvarchar(max)
        SET @sql_filterableSpecs = '
	        INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
	        SELECT DISTINCT [psam].SpecificationAttributeOptionId
	        FROM [Product_SpecificationAttribute_Mapping] [psam] WITH (NOLOCK)
	            WHERE [psam].[AllowFiltering] = 1
	            AND [psam].[ProductId] IN (' + @sql + ')'

        EXEC sp_executesql @sql_filterableSpecs

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',') 

    CREATE TABLE #FilteredSpecsWithAttributes
	(
        SpecificationAttributeId int not null,
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecsWithAttributes (SpecificationAttributeId, SpecificationAttributeOptionId)
	SELECT sao.SpecificationAttributeId, fs.SpecificationAttributeOptionId
    FROM #FilteredSpecs fs INNER JOIN SpecificationAttributeOption sao ON sao.Id = fs.SpecificationAttributeOptionId
    ORDER BY sao.SpecificationAttributeId 

    DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecsWithAttributes)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
        DECLARE @SpecificationAttributeId int
        DECLARE @LastSpecificationAttributeId int
        SET @LastSpecificationAttributeId = 0
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT SpecificationAttributeId, SpecificationAttributeOptionId
		FROM #FilteredSpecsWithAttributes

		OPEN cur_SpecificationAttributeOption
        FOREACH:
            FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeId, @SpecificationAttributeOptionId
            IF (@LastSpecificationAttributeId <> 0 AND @SpecificationAttributeId <> @LastSpecificationAttributeId OR @@FETCH_STATUS <> 0) 
			    SET @sql = @sql + '
        AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId IN (SELECT SpecificationAttributeOptionId FROM #FilteredSpecsWithAttributes WHERE SpecificationAttributeId = ' + CAST(@LastSpecificationAttributeId AS nvarchar(max)) + '))'
            SET @LastSpecificationAttributeId = @SpecificationAttributeId
		IF @@FETCH_STATUS = 0 GOTO FOREACH
		CLOSE cur_SpecificationAttributeOption
		DEALLOCATE cur_SpecificationAttributeOption
	END

	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
		--manufacturer position (display order)
		IF @ManufacturerId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' pmm.DisplayOrder ASC'
		END
		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Name] ASC'
	END
	
	SET @sql = @sql + '
	ORDER BY' + @sql_orderby
	
    SET @sql = '
    INSERT INTO #DisplayOrderTmp ([ProductId])' + @sql

	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
    DROP TABLE #FilteredSpecsWithAttributes
	DROP TABLE #FilteredCustomerRoleIds
	DROP TABLE #KeywordProducts

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

-- update the "DeleteGuests" stored procedure
ALTER PROCEDURE [DeleteGuests]
(
	@OnlyWithoutShoppingCart bit = 1,
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (CustomerId int)
	CREATE TABLE #tmp_adresses (AddressId int)
		
	INSERT #tmp_guests (CustomerId)
	SELECT c.[Id] 
	FROM [Customer] c with (NOLOCK)
		LEFT JOIN [ShoppingCartItem] sci with (NOLOCK) ON sci.[CustomerId] = c.[Id]
		INNER JOIN (
			--guests only
			SELECT ccrm.[Customer_Id] 
			FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK)
				INNER JOIN [CustomerRole] cr with (NOLOCK) ON cr.[Id] = ccrm.[CustomerRole_Id]
			WHERE cr.[SystemName] = N'Guests'
		) g ON g.[Customer_Id] = c.[Id]
		LEFT JOIN [Order] o with (NOLOCK) ON o.[CustomerId] = c.[Id]
		LEFT JOIN [BlogComment] bc with (NOLOCK) ON bc.[CustomerId] = c.[Id]
		LEFT JOIN [NewsComment] nc with (NOLOCK) ON nc.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReview] pr with (NOLOCK) ON pr.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReviewHelpfulness] prh with (NOLOCK) ON prh.[CustomerId] = c.[Id]
		LEFT JOIN [PollVotingRecord] pvr with (NOLOCK) ON pvr.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Topic] ft with (NOLOCK) ON ft.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Post] fp with (NOLOCK) ON fp.[CustomerId] = c.[Id]
	WHERE 1 = 1
		--no orders
		AND (o.Id is null)
		--no blog comments
		AND (bc.Id is null)
		--no news comments
		AND (nc.Id is null)
		--no product reviews
		AND (pr.Id is null)
		--no product reviews helpfulness
		AND (prh.Id is null)
		--no poll voting
		AND (pvr.Id is null)
		--no forum topics
		AND (ft.Id is null)
		--no forum topics
		AND (fp.Id is null)
		--no system accounts
		AND (c.IsSystemAccount = 0)
		--created from
		AND ((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
		--created to
		AND ((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
		--shopping cart items
		AND ((@OnlyWithoutShoppingCart = 0) OR (sci.Id is null))

	INSERT #tmp_adresses (AddressId)
	SELECT [Address_Id] FROM [CustomerAddresses] WHERE [Customer_Id] IN (SELECT [CustomerId] FROM #tmp_guests)

	--delete guests
	DELETE [Customer]
	WHERE [Id] IN (SELECT [CustomerId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [CustomerId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'Customer')

	--delete addresses
	DELETE [Address]
	WHERE [Id] IN (SELECT [AddressId] FROM #tmp_adresses)
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
	DROP TABLE #tmp_adresses
END
GO

--new columns
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[GenericAttribute]') and NAME='CreatedOrUpdatedDateUTC')
BEGIN
	ALTER TABLE [GenericAttribute] ADD
	CreatedOrUpdatedDateUTC datetime NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.firstnameenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.firstnameenabled', 'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.firstnamerequired')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.firstnamerequired', 'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.lastnameenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.lastnameenabled', 'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.lastnamerequired')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.lastnamerequired', 'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'catalogsettings.productsearchenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.productsearchenabled', 'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.lastactivityminutes')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.lastactivityminutes', '15', 0)
END
GO

-- update the "ProductLoadAllPaged" stored procedure
ALTER PROCEDURE [ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchManufacturerPartNumber bit = 0, -- a value indicating whether to search by a specified "keyword" in manufacturer part number
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@OverridePublished	bit = null, --null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@FilterableSpecificationAttributeOptionIds nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN	
	/* Products that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ProductId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@OriginalKeywords nvarchar(4000),
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	SET @OriginalKeywords = @Keywords
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
                    DECLARE @len_keywords INT
					DECLARE @len_nvarchar INT
					SET @len_keywords = 0
					SET @len_nvarchar = DATALENGTH(CONVERT(NVARCHAR(MAX), 'a'))

					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

                        --LEN excludes trailing spaces. That is why we use DATALENGTH
						--see https://docs.microsoft.com/sql/t-sql/functions/len-transact-sql?view=sqlallproducts-allversions for more ditails
						SET @len_keywords = DATALENGTH(@Keywords) / @len_nvarchar

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, @len_keywords - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
                    SET @len_keywords = DATALENGTH(@Keywords) / @len_nvarchar
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, @len_keywords) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--product name
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Name]) > 0 '

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			IF @UseFullTextSearch = 1
			BEGIN
				SET @sql = @sql + 'OR CONTAINS(p.[ShortDescription], @Keywords) '
				SET @sql = @sql + 'OR CONTAINS(p.[FullDescription], @Keywords) '
			END
			ELSE
			BEGIN
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[ShortDescription]) > 0 '
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[FullDescription]) > 0 '
			END
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + 'OR p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + 'OR p.[Sku] = @OriginalKeywords '
		END

		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND ( (lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

		IF @SearchDescriptions = 1
		BEGIN
			--localized product short description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

			--localized product full description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '
		END

		SET @sql = @sql + ' ) '

		IF @SearchProductTags = 1
		BEGIN
			--product tags (exact match)
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE pt.[Name] = @OriginalKeywords '

			--localized product tags
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name''
				AND lp.[LocaleValue] = @OriginalKeywords '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')
	DECLARE @FilteredCustomerRoleIdsCount int	
	SET @FilteredCustomerRoleIdsCount = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)
	
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

	SET @sql = '
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ProductId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN ('
		
		SET @sql = @sql + + CAST(@CategoryIds AS nvarchar(max))

		SET @sql = @sql + ')'

		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pcm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by manufacturer
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		AND pmm.ManufacturerId = ' + CAST(@ManufacturerId AS nvarchar(max))
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pmm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
	END
	
	--filter by warehouse
	IF @WarehouseId > 0
	BEGIN
		--we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		--but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET @sql = @sql + '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory [pwi]
					WHERE [pwi].WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ' AND [pwi].ProductId = p.Id))
			)'
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	--"Published" property
	IF (@OverridePublished is null)
	BEGIN
		--process according to "showHidden"
		IF @ShowHidden = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Published = 1'
		END
	END
	ELSE IF (@OverridePublished = 1)
	BEGIN
		--published only
		SET @sql = @sql + '
		AND p.Published = 1'
	END
	ELSE IF (@OverridePublished = 0)
	BEGIN
		--unpublished only
		SET @sql = @sql + '
		AND p.Published = 0'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(p.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')'
	END
	
	--show hidden and ACL
	IF  @ShowHidden = 0 and @FilteredCustomerRoleIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
    --prepare filterable specification attribute option identifier (if requested)
    IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
        DECLARE @sql_filterableSpecs nvarchar(max)
        SET @sql_filterableSpecs = '
	        INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
	        SELECT DISTINCT [psam].SpecificationAttributeOptionId
	        FROM [Product_SpecificationAttribute_Mapping] [psam] WITH (NOLOCK)
	            WHERE [psam].[AllowFiltering] = 1
	            AND [psam].[ProductId] IN (' + @sql + ')'

        EXEC sp_executesql @sql_filterableSpecs

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',') 

    CREATE TABLE #FilteredSpecsWithAttributes
	(
        SpecificationAttributeId int not null,
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecsWithAttributes (SpecificationAttributeId, SpecificationAttributeOptionId)
	SELECT sao.SpecificationAttributeId, fs.SpecificationAttributeOptionId
    FROM #FilteredSpecs fs INNER JOIN SpecificationAttributeOption sao ON sao.Id = fs.SpecificationAttributeOptionId
    ORDER BY sao.SpecificationAttributeId 

    DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecsWithAttributes)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
        DECLARE @SpecificationAttributeId int
        DECLARE @LastSpecificationAttributeId int
        SET @LastSpecificationAttributeId = 0
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT SpecificationAttributeId, SpecificationAttributeOptionId
		FROM #FilteredSpecsWithAttributes

		OPEN cur_SpecificationAttributeOption
        FOREACH:
            FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeId, @SpecificationAttributeOptionId
            IF (@LastSpecificationAttributeId <> 0 AND @SpecificationAttributeId <> @LastSpecificationAttributeId OR @@FETCH_STATUS <> 0) 
			    SET @sql = @sql + '
        AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId IN (SELECT SpecificationAttributeOptionId FROM #FilteredSpecsWithAttributes WHERE SpecificationAttributeId = ' + CAST(@LastSpecificationAttributeId AS nvarchar(max)) + '))'
            SET @LastSpecificationAttributeId = @SpecificationAttributeId
		IF @@FETCH_STATUS = 0 GOTO FOREACH
		CLOSE cur_SpecificationAttributeOption
		DEALLOCATE cur_SpecificationAttributeOption
	END

	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
		--manufacturer position (display order)
		IF @ManufacturerId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' pmm.DisplayOrder ASC'
		END
		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Name] ASC'
	END
	
	SET @sql = @sql + '
	ORDER BY' + @sql_orderby
	
    SET @sql = '
    INSERT INTO #DisplayOrderTmp ([ProductId])' + @sql

	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
    DROP TABLE #FilteredSpecsWithAttributes
	DROP TABLE #FilteredCustomerRoleIds
	DROP TABLE #KeywordProducts

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO	

--update fluent migration versions
DELETE FROM [MigrationVersionInfo] WHERE [Description] in ('AddProductAttributeValueProductAttributeMappingFK', 'AddProductCategoryCategoryFK', 'AddProductCategoryProductFK', 'AddProductManufacturerManufacturerFK', 'AddProductManufacturerProductFK', 'AddProductPictureProductPictureFK', 'AddProductPictureProductProductFK', 'AddAddProductProductTagProductFK', 'AddAddProductProductTagProductTagFK', 'AddProductReviewHelpfulnessProductReviewFK', 'AddProductReviewProductFK', 'AddProductReviewCustomerFK', 'AddProductReviewStoreFK', 'AddProductReviewReviewTypeMappingProductReviewFK', 'AddProductReviewReviewTypeMappingReviewTypeFK', 'AddProductSpecificationAttributeSpecificationAttributeOptionFK', 'AddProductSpecificationAttributeProductFK', 'AddProductWarehouseInventoryProductFK', 'AddProductWarehouseInventoryWarehouseFK', 'AddSpecificationAttributeOptionSpecificationAttributeFK', 'AddStockQuantityHistoryProductFK', 'AddStockQuantityHistoryWarehouseFK', 'AddTierPriceProductFK', 'AddTierPriceCustomerRoleFK', 'AddAddressAttributeValueAddressAttributeValueFk', 'AddAddressCountryFK', 'AddAddressStateProvinceFK', 'AddCustomerAddressCustomerFK', 'AddCustomerAddressAddressFK', 'AddCustomerAttributeValueCustomerAttributeFK', 'AddCustomerCustomerRoleCustomerFK', 'AddCustomerCustomerRoleCustomerRoleFK', 'AddCustomerBillingAddressFK', 'AddCustomerShippingAddressFK', 'AddCustomerPasswordCustomerFK', 'AddExternalAuthenticationRecordCustomerFK', 'AddRewardPointsHistoryCustomerFK', 'AddRewardPointsHistoryOrderFK', 'AddStateProvinceCountryFK', 'AddDiscountCategoryDiscountFK', 'AddDiscountCategoryCategoryFK', 'AddDiscountManufacturerDiscountFK', 'AddDiscountManufacturerManufacturerFK', 'AddDiscountProductDiscountFK', 'AddDiscountProductProductFK', 'AddDiscountUsageHistoryDiscountFK', 'AddDiscountUsageHistoryOrderFK', 'AddForumForumGroupFK', 'AddForumPostForumTopicFK', 'AddForumPostCustomerFK', 'AddForumPostVoteForumPostFK', 'AddForumSubscriptionCustomerFK', 'AddForumTopicForumFK', 'AddForumTopicCustomerFK', 'AddPrivateMessageFromCustomerFK', 'AddPrivateMessageToCustomerFK', 'AddLocaleStringResourceLanguageFK', 'AddLocalizedPropertyLanguageFK', 'AddActivityLogActivityLogTypeFK', 'AddActivityLogCustomerFK', 'AddLogCustomerFK', 'AddPictureBinaryPictureFK', 'AddQueuedEmailEmailAccountFK', 'AddNewsCommentNewsItemFK', 'AddNewsCommentCustomerFK', 'AddNewsCommentStoreFK', 'AddNewsItemLanguageFK', 'AddCheckoutAttributeValueCheckoutAttributeFK', 'AddGiftCardOrderItemFK', 'AddGiftCardUsageHistoryGiftCardFK', 'AddGiftCardUsageHistoryOrderFK', 'AddOrderItemOrderFK', 'AddOrderItemProductFK', 'AddOrderCustomerFK', 'AddOrderBillingAddressFK', 'AddOrderShippingAddressFK', 'AddOrderPickupAddressFK', 'AddOrderNoteOrderFK', 'AddRecurringPaymentHistoryRecurringPaymentFK', 'AddRecurringPaymentOrderFK', 'AddReturnRequestCustomerFK', 'AddShoppingCartItemCustomerFK', 'AddShoppingCartItemProductFK', 'AddPollAnswerPollFK', 'AddPollLanguageFK', 'AddPollVotingRecordPollAnswerFK', 'AddPollVotingRecordCustomerFK', 'AddAclRecordCustomerRoleFK', 'AddPermissionRecordCustomerRoleCustomerRoleFK', 'AddPermissionRecordCustomerRolePermissionRecordFK', 'AddShipmentItemShipmentFK', 'AddShipmentOrderFK', 'AddShippingMethodCountryCountryFK', 'AddShippingMethodCountryShippingMethodFK', 'AddStoreMappingStoreFK', 'AddVendorAttributeValueVendorAttributeFK', 'AddVendorNoteVendorFK', 'AddAffiliateAddressFK', 'AddBlogCommentBlogPostFK', 'AddBlogCommentCustomerFK', 'AddBlogCommentStoreFK', 'AddBlogPostLanguageFK', 'AddBackInStockSubscriptionProductFK', 'AddBackInStockSubscriptionCustomerFK', 'AddPredefinedProductAttributeValueProductAttributeFK', 'AddProductAttributeMappingProductFK', 'AddProductAttributeMappingProductAttributeFK', 'AddDiscountRequirementDiscountFK', 'AddDiscountRequirementDiscountRequirementFK', 'AddOrderRewardPointsHistoryFK', 'AddProductAttributeCombinationProductFk', 'AddPCMProductIdExtendedIX', 'AddPMMProductIdExtendedIX', 'AddPSAMAllowFilteringIX', 'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX', 'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX', 'AddProductVisibleIndividuallyPublishedDeletedExtendedIX', 'AddCategoryDeletedExtendedIX', 'AddLocaleStringResourceIX', 'AddProductPriceDatesEtcIX', 'AddCountryDisplayOrderIX', 'AddLogCreatedOnUtcIX', 'AddCustomerEmailIX', 'AddCustomerUsernameIX', 'AddCustomerCustomerGuidIX', 'AddCustomerSystemNameIX', 'AddCustomerCreatedOnUtcIX', 'AddGenericAttributeEntityIdKeyGroupIX', 'AddQueuedEmailCreatedOnUtcIX', 'AddOrderCreatedOnUtcIX', 'AddLanguageDisplayOrderIX', 'AddNewsletterSubscriptionEmailStoreIdIX', 'AddShoppingCartItemShoppingCartTypeIdCustomerIdIX', 'AddRelatedProductProductId1IX', 'AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX', 'AddProductProductAttributeMappingProductIdDisplayOrderIX', 'AddManufacturerDisplayOrderIX', 'AddCategoryDisplayOrderIX', 'AddCategoryParentCategoryIdIX', 'AddForumsGroupDisplayOrderIX', 'AddForumsForumDisplayOrderIX', 'AddForumsSubscriptionForumIdIX', 'AddForumsSubscriptionTopicIdIX', 'AddProductDeletedPublishedIX', 'AddProductPublishedIX', 'AddProductShowOnHomepageIX', 'AddProductParentGroupedProductIdIX', 'AddProductVisibleIndividuallyIX', 'AddPCMProductCategoryIX', 'AddCurrencyDisplayOrderIX', 'AddProductTagNameIX', 'AddActivityLogCreatedOnUtcIX', 'AddUrlRecordSlugIX', 'AddUrlRecordCustom1IX', 'AddAclRecordEntityIdEntityNameIX', 'AddStoreMappingEntityIdEntityNameIX', 'AddCategoryLimitedToStoresIX', 'AddManufacturerLimitedToStoresIX', 'AddProductLimitedToStoresIX', 'AddCategorSubjectToAclIX', 'AddManufacturerSubjectToAclIX', 'AddProductSubjectToAclIX', 'AddProductCategoryMappingIsFeaturedProductIX', 'AddProductManufacturerMappingIsFeaturedProductIX', 'AddCustomerCustomerRoleMappingCustomerIdIX', 'AddProductDeleteIdIX', 'AddGetLowStockProductsIX', 'AddPMMProductManufacturerIX');

INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductAttributeValueProductAttributeMappingFK', 637097184507544540)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductCategoryCategoryFK', 637097186625689396)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductCategoryProductFK', 637097186625689397)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductManufacturerManufacturerFK', 637097188539067594)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductManufacturerProductFK', 637097188539067595)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductPictureProductPictureFK', 637097195662625749)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductPictureProductProductFK', 637097195662625750)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddAddProductProductTagProductFK', 637097199880193450)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddAddProductProductTagProductTagFK', 637097199880193451)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductReviewHelpfulnessProductReviewFK', 637097207558603530)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductReviewProductFK', 637097207998948304)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductReviewCustomerFK', 637097207998948305)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductReviewStoreFK', 637097207998948306)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductReviewReviewTypeMappingProductReviewFK', 637097211602513441)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductReviewReviewTypeMappingReviewTypeFK', 637097211602513442)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductSpecificationAttributeSpecificationAttributeOptionFK', 637097213462261985)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductSpecificationAttributeProductFK', 637097213462261986)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductWarehouseInventoryProductFK', 637097218980051780)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:28.0000000' AS DateTime2), N'AddProductWarehouseInventoryWarehouseFK', 637097218980051781)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddSpecificationAttributeOptionSpecificationAttributeFK', 637097221366619708)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddStockQuantityHistoryProductFK', 637097224165419186)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddStockQuantityHistoryWarehouseFK', 637097224165419187)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddTierPriceProductFK', 637097225438051844)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddTierPriceCustomerRoleFK', 637097225438051845)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddAddressAttributeValueAddressAttributeValueFk', 637097261526459118)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddAddressCountryFK', 637097264240659480)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddAddressStateProvinceFK', 637097264240659481)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerAddressCustomerFK', 637097266595245358)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerAddressAddressFK', 637097266595245359)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerAttributeValueCustomerAttributeFK', 637097269504308129)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerCustomerRoleCustomerFK', 637097271237489896)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerCustomerRoleCustomerRoleFK', 637097271237489897)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerBillingAddressFK', 637097273651641381)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerShippingAddressFK', 637097273651641382)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddCustomerPasswordCustomerFK', 637097275461276491)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddExternalAuthenticationRecordCustomerFK', 637097276449096139)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddRewardPointsHistoryCustomerFK', 637097277252342366)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddRewardPointsHistoryOrderFK', 637097277252342367)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddStateProvinceCountryFK', 637097281433797964)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountCategoryDiscountFK', 637097339695936887)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountCategoryCategoryFK', 637097339695936888)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountManufacturerDiscountFK', 637097342149883528)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountManufacturerManufacturerFK', 637097342149883529)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountProductDiscountFK', 637097346951975256)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountProductProductFK', 637097346951975257)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountUsageHistoryDiscountFK', 637097348041180783)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:29.0000000' AS DateTime2), N'AddDiscountUsageHistoryOrderFK', 637097348041180784)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumForumGroupFK', 637097351627313370)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumPostForumTopicFK', 637097352463004325)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumPostCustomerFK', 637097352463004326)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumPostVoteForumPostFK', 637097355633262801)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumSubscriptionCustomerFK', 637097356387699848)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumTopicForumFK', 637097357101910240)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddForumTopicCustomerFK', 637097357101910241)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddPrivateMessageFromCustomerFK', 637097358373669695)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddPrivateMessageToCustomerFK', 637097358373669696)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddLocaleStringResourceLanguageFK', 637097360951964555)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddLocalizedPropertyLanguageFK', 637097361590515436)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddActivityLogActivityLogTypeFK', 637097362508380329)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddActivityLogCustomerFK', 637097362508380330)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddLogCustomerFK', 637097363893561926)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddPictureBinaryPictureFK', 637097364695631609)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddQueuedEmailEmailAccountFK', 637097365031655781)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddNewsCommentNewsItemFK', 637097366362530772)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddNewsCommentCustomerFK', 637097366362530773)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddNewsCommentStoreFK', 637097366362530774)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddNewsItemLanguageFK', 637097368094361423)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddCheckoutAttributeValueCheckoutAttributeFK', 637097369078553212)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddGiftCardOrderItemFK', 637097370922130581)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddGiftCardUsageHistoryGiftCardFK', 637097371156452475)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddGiftCardUsageHistoryOrderFK', 637097371156452476)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderItemOrderFK', 637097372609436788)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderItemProductFK', 637097372609436789)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderCustomerFK', 637097373896028942)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderBillingAddressFK', 637097373896028943)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderShippingAddressFK', 637097373896028944)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderPickupAddressFK', 637097373896028945)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:30.0000000' AS DateTime2), N'AddOrderNoteOrderFK', 637097376997123308)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddRecurringPaymentHistoryRecurringPaymentFK', 637097378210887644)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddRecurringPaymentOrderFK', 637097379410960207)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddReturnRequestCustomerFK', 637097380291248082)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddShoppingCartItemCustomerFK', 637097381093371767)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddShoppingCartItemProductFK', 637097381093371768)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPollAnswerPollFK', 637097383487520229)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPollLanguageFK', 637097384025962851)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPollVotingRecordPollAnswerFK', 637097385036693383)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPollVotingRecordCustomerFK', 637097385036693384)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddAclRecordCustomerRoleFK', 637097386436073081)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPermissionRecordCustomerRoleCustomerRoleFK', 637097387107801301)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPermissionRecordCustomerRolePermissionRecordFK', 637097387107801302)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddShipmentItemShipmentFK', 637097388921984734)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddShipmentOrderFK', 637097389681126845)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddShippingMethodCountryCountryFK', 637097390410528356)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddShippingMethodCountryShippingMethodFK', 637097390410528357)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddStoreMappingStoreFK', 637097391639005655)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddVendorAttributeValueVendorAttributeFK', 637097392346411077)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddVendorNoteVendorFK', 637097392991868645)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddAffiliateAddressFK', 637097594562551771)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddBlogCommentBlogPostFK', 637097605404497785)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddBlogCommentCustomerFK', 637097605404497786)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddBlogCommentStoreFK', 637097605404497787)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddBlogPostLanguageFK', 637097607595956342)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddBackInStockSubscriptionProductFK', 637097608748261630)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddBackInStockSubscriptionCustomerFK', 637097608748261631)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddPredefinedProductAttributeValueProductAttributeFK', 637097611590754490)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddProductAttributeMappingProductFK', 637097615386806324)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddProductAttributeMappingProductAttributeFK', 637097615386806325)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddDiscountRequirementDiscountFK', 637117958520043560)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddDiscountRequirementDiscountRequirementFK', 637117958520043561)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:31.0000000' AS DateTime2), N'AddOrderRewardPointsHistoryFK', 637120677617140897)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductAttributeCombinationProductFk', 637120678600830411)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddPCMProductIdExtendedIX', 637123105559280389)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddPMMProductIdExtendedIX', 637123105559280390)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddPSAMAllowFilteringIX', 637123105559280391)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX', 637123105559280392)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX', 637123105559280393)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductVisibleIndividuallyPublishedDeletedExtendedIX', 637123105559280394)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCategoryDeletedExtendedIX', 637123105559280395)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddLocaleStringResourceIX', 637123449689037677)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductPriceDatesEtcIX', 637123449689037678)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCountryDisplayOrderIX', 637123449689037679)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddLogCreatedOnUtcIX', 637123449689037680)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCustomerEmailIX', 637123449689037681)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCustomerUsernameIX', 637123449689037682)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCustomerCustomerGuidIX', 637123449689037683)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCustomerSystemNameIX', 637123449689037684)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCustomerCreatedOnUtcIX', 637123449689037685)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddGenericAttributeEntityIdKeyGroupIX', 637123449689037686)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddQueuedEmailCreatedOnUtcIX', 637123449689037687)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddOrderCreatedOnUtcIX', 637123449689037688)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddLanguageDisplayOrderIX', 637123449689037689)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddNewsletterSubscriptionEmailStoreIdIX', 637123449689037690)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddShoppingCartItemShoppingCartTypeIdCustomerIdIX', 637123449689037691)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddRelatedProductProductId1IX', 637123449689037692)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX', 637123449689037693)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductProductAttributeMappingProductIdDisplayOrderIX', 637123449689037694)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddManufacturerDisplayOrderIX', 637123449689037695)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCategoryDisplayOrderIX', 637123449689037696)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCategoryParentCategoryIdIX', 637123449689037697)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddForumsGroupDisplayOrderIX', 637123449689037698)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddForumsForumDisplayOrderIX', 637123449689037699)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddForumsSubscriptionForumIdIX', 637123449689037700)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddForumsSubscriptionTopicIdIX', 637123449689037701)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductDeletedPublishedIX', 637123449689037702)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductPublishedIX', 637123449689037703)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductShowOnHomepageIX', 637123449689037704)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductParentGroupedProductIdIX', 637123449689037705)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductVisibleIndividuallyIX', 637123449689037706)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddPCMProductCategoryIX', 637123449689037707)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCurrencyDisplayOrderIX', 637123449689037708)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductTagNameIX', 637123521091647925)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddActivityLogCreatedOnUtcIX', 637123521091647926)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddUrlRecordSlugIX', 637123521091647927)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddUrlRecordCustom1IX', 637123521091647928)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddAclRecordEntityIdEntityNameIX', 637123521091647929)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddStoreMappingEntityIdEntityNameIX', 637123521091647930)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCategoryLimitedToStoresIX', 637123521091647931)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddManufacturerLimitedToStoresIX', 637123521091647932)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductLimitedToStoresIX', 637123521091647933)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCategorSubjectToAclIX', 637123521091647934)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddManufacturerSubjectToAclIX', 637123521091647935)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductSubjectToAclIX', 637123521091647936)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductCategoryMappingIsFeaturedProductIX', 637123521091647937)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductManufacturerMappingIsFeaturedProductIX', 637123521091647938)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddCustomerCustomerRoleMappingCustomerIdIX', 637123521091647939)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:32.0000000' AS DateTime2), N'AddProductDeleteIdIX', 637123521091647940)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:33.0000000' AS DateTime2), N'AddGetLowStockProductsIX', 637123521091647941)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2020-03-10T15:24:33.0000000' AS DateTime2), N'AddPMMProductManufacturerIX', 637123521091647942)
GO
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.captchatype')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.captchatype', 'CheckBoxReCaptchaV2', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.recaptchav3scorethreshold')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.recaptchav3scorethreshold', '0.5', 0)
END
GO

--update fluent migration versions
DELETE FROM [MigrationVersionInfo] WHERE [Description] in ('AddProductAttributeValueProductAttributeMappingFK', 'AddProductCategoryCategoryFK', 'AddProductCategoryProductFK', 'AddProductManufacturerManufacturerFK', 'AddProductManufacturerProductFK', 'AddProductPictureProductPictureFK', 'AddProductPictureProductProductFK', 'AddAddProductProductTagProductFK', 'AddAddProductProductTagProductTagFK', 'AddProductReviewHelpfulnessProductReviewFK', 'AddProductReviewProductFK', 'AddProductReviewCustomerFK', 'AddProductReviewStoreFK', 'AddProductReviewReviewTypeMappingProductReviewFK', 'AddProductReviewReviewTypeMappingReviewTypeFK', 'AddProductSpecificationAttributeSpecificationAttributeOptionFK', 'AddProductSpecificationAttributeProductFK', 'AddProductWarehouseInventoryProductFK', 'AddProductWarehouseInventoryWarehouseFK', 'AddSpecificationAttributeOptionSpecificationAttributeFK', 'AddStockQuantityHistoryProductFK', 'AddStockQuantityHistoryWarehouseFK', 'AddTierPriceProductFK', 'AddTierPriceCustomerRoleFK', 'AddAddressAttributeValueAddressAttributeValueFk', 'AddAddressCountryFK', 'AddAddressStateProvinceFK', 'AddCustomerAddressCustomerFK', 'AddCustomerAddressAddressFK', 'AddCustomerAttributeValueCustomerAttributeFK', 'AddCustomerCustomerRoleCustomerFK', 'AddCustomerCustomerRoleCustomerRoleFK', 'AddCustomerBillingAddressFK', 'AddCustomerShippingAddressFK', 'AddCustomerPasswordCustomerFK', 'AddExternalAuthenticationRecordCustomerFK', 'AddRewardPointsHistoryCustomerFK', 'AddRewardPointsHistoryOrderFK', 'AddStateProvinceCountryFK', 'AddDiscountCategoryDiscountFK', 'AddDiscountCategoryCategoryFK', 'AddDiscountManufacturerDiscountFK', 'AddDiscountManufacturerManufacturerFK', 'AddDiscountProductDiscountFK', 'AddDiscountProductProductFK', 'AddDiscountUsageHistoryDiscountFK', 'AddDiscountUsageHistoryOrderFK', 'AddForumForumGroupFK', 'AddForumPostForumTopicFK', 'AddForumPostCustomerFK', 'AddForumPostVoteForumPostFK', 'AddForumSubscriptionCustomerFK', 'AddForumTopicForumFK', 'AddForumTopicCustomerFK', 'AddPrivateMessageFromCustomerFK', 'AddPrivateMessageToCustomerFK', 'AddLocaleStringResourceLanguageFK', 'AddLocalizedPropertyLanguageFK', 'AddActivityLogActivityLogTypeFK', 'AddActivityLogCustomerFK', 'AddLogCustomerFK', 'AddPictureBinaryPictureFK', 'AddQueuedEmailEmailAccountFK', 'AddNewsCommentNewsItemFK', 'AddNewsCommentCustomerFK', 'AddNewsCommentStoreFK', 'AddNewsItemLanguageFK', 'AddCheckoutAttributeValueCheckoutAttributeFK', 'AddGiftCardOrderItemFK', 'AddGiftCardUsageHistoryGiftCardFK', 'AddGiftCardUsageHistoryOrderFK', 'AddOrderItemOrderFK', 'AddOrderItemProductFK', 'AddOrderCustomerFK', 'AddOrderBillingAddressFK', 'AddOrderShippingAddressFK', 'AddOrderPickupAddressFK', 'AddOrderNoteOrderFK', 'AddRecurringPaymentHistoryRecurringPaymentFK', 'AddRecurringPaymentOrderFK', 'AddReturnRequestCustomerFK', 'AddShoppingCartItemCustomerFK', 'AddShoppingCartItemProductFK', 'AddPollAnswerPollFK', 'AddPollLanguageFK', 'AddPollVotingRecordPollAnswerFK', 'AddPollVotingRecordCustomerFK', 'AddAclRecordCustomerRoleFK', 'AddPermissionRecordCustomerRoleCustomerRoleFK', 'AddPermissionRecordCustomerRolePermissionRecordFK', 'AddShipmentItemShipmentFK', 'AddShipmentOrderFK', 'AddShippingMethodCountryCountryFK', 'AddShippingMethodCountryShippingMethodFK', 'AddStoreMappingStoreFK', 'AddVendorAttributeValueVendorAttributeFK', 'AddVendorNoteVendorFK', 'AddAffiliateAddressFK', 'AddBlogCommentBlogPostFK', 'AddBlogCommentCustomerFK', 'AddBlogCommentStoreFK', 'AddBlogPostLanguageFK', 'AddBackInStockSubscriptionProductFK', 'AddBackInStockSubscriptionCustomerFK', 'AddPredefinedProductAttributeValueProductAttributeFK', 'AddProductAttributeMappingProductFK', 'AddProductAttributeMappingProductAttributeFK', 'AddDiscountRequirementDiscountFK', 'AddDiscountRequirementDiscountRequirementFK', 'AddOrderRewardPointsHistoryFK', 'AddProductAttributeCombinationProductFk', 'AddPCMProductIdExtendedIX', 'AddPMMProductIdExtendedIX', 'AddPSAMAllowFilteringIX', 'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX', 'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX', 'AddProductVisibleIndividuallyPublishedDeletedExtendedIX', 'AddCategoryDeletedExtendedIX', 'AddLocaleStringResourceIX', 'AddProductPriceDatesEtcIX', 'AddCountryDisplayOrderIX', 'AddLogCreatedOnUtcIX', 'AddCustomerEmailIX', 'AddCustomerUsernameIX', 'AddCustomerCustomerGuidIX', 'AddCustomerSystemNameIX', 'AddCustomerCreatedOnUtcIX', 'AddGenericAttributeEntityIdKeyGroupIX', 'AddQueuedEmailCreatedOnUtcIX', 'AddOrderCreatedOnUtcIX', 'AddLanguageDisplayOrderIX', 'AddNewsletterSubscriptionEmailStoreIdIX', 'AddShoppingCartItemShoppingCartTypeIdCustomerIdIX', 'AddRelatedProductProductId1IX', 'AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX', 'AddProductProductAttributeMappingProductIdDisplayOrderIX', 'AddManufacturerDisplayOrderIX', 'AddCategoryDisplayOrderIX', 'AddCategoryParentCategoryIdIX', 'AddForumsGroupDisplayOrderIX', 'AddForumsForumDisplayOrderIX', 'AddForumsSubscriptionForumIdIX', 'AddForumsSubscriptionTopicIdIX', 'AddProductDeletedPublishedIX', 'AddProductPublishedIX', 'AddProductShowOnHomepageIX', 'AddProductParentGroupedProductIdIX', 'AddProductVisibleIndividuallyIX', 'AddPCMProductCategoryIX', 'AddCurrencyDisplayOrderIX', 'AddProductTagNameIX', 'AddActivityLogCreatedOnUtcIX', 'AddUrlRecordSlugIX', 'AddUrlRecordCustom1IX', 'AddAclRecordEntityIdEntityNameIX', 'AddStoreMappingEntityIdEntityNameIX', 'AddCategoryLimitedToStoresIX', 'AddManufacturerLimitedToStoresIX', 'AddProductLimitedToStoresIX', 'AddCategorSubjectToAclIX', 'AddManufacturerSubjectToAclIX', 'AddProductSubjectToAclIX', 'AddProductCategoryMappingIsFeaturedProductIX', 'AddProductManufacturerMappingIsFeaturedProductIX', 'AddCustomerCustomerRoleMappingCustomerIdIX', 'AddProductDeleteIdIX', 'AddGetLowStockProductsIX', 'AddPMMProductManufacturerIX', 'Nop.Data base schema', 'Shipping.FixedByWeightByTotal base schema', 'Tax.Avalara base schema', 'Tax.FixedOrByCountryStateZip base schema', 'Pickup.PickupInStore base schema');

BEGIN TRANSACTION
GO
CREATE TABLE Tmp_MigrationVersionInfo
	(
	AppliedOn datetime NULL,
	Description nvarchar(1024) NULL,
	Version bigint NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Tmp_MigrationVersionInfo SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM MigrationVersionInfo)
	 EXEC('INSERT INTO Tmp_MigrationVersionInfo (AppliedOn, Description, Version)
		SELECT CONVERT(datetime, AppliedOn), CONVERT(nvarchar(1024), Description), Version FROM MigrationVersionInfo WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE MigrationVersionInfo
GO
EXECUTE sp_rename N'Tmp_MigrationVersionInfo', N'MigrationVersionInfo', 'OBJECT' 
GO
COMMIT

INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637160666562551771, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'Nop.Data base schema')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637163160551687541, CAST(N'2020-03-18T07:25:09.000' AS DateTime), N'Shipping.FixedByWeightByTotal base schema')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637163177576455442, CAST(N'2020-03-18T07:25:09.000' AS DateTime), N'Tax.Avalara base schema')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637163188436455432, CAST(N'2020-03-18T07:25:09.000' AS DateTime), N'Tax.FixedOrByCountryStateZip base schema')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637163190176455422, CAST(N'2020-03-18T07:25:09.000' AS DateTime), N'Pickup.PickupInStore base schema')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280389, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddPCMProductIdExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280390, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddPMMProductIdExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280391, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddPSAMAllowFilteringIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280392, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280393, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280394, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddProductVisibleIndividuallyPublishedDeletedExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196545559280395, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCategoryDeletedExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037677, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddLocaleStringResourceIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037678, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddProductPriceDatesEtcIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037679, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCountryDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037680, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddLogCreatedOnUtcIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037681, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCustomerEmailIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037682, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCustomerUsernameIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037683, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCustomerCustomerGuidIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037684, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCustomerSystemNameIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037685, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddCustomerCreatedOnUtcIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037686, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddGenericAttributeEntityIdKeyGroupIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037687, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddQueuedEmailCreatedOnUtcIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037688, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddOrderCreatedOnUtcIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037689, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddLanguageDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037690, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddNewsletterSubscriptionEmailStoreIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037691, CAST(N'2020-03-18T07:23:31.000' AS DateTime), N'AddShoppingCartItemShoppingCartTypeIdCustomerIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037692, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddRelatedProductProductId1IX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037693, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037694, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductProductAttributeMappingProductIdDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037695, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddManufacturerDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037696, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddCategoryDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037697, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddCategoryParentCategoryIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037698, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddForumsGroupDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037699, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddForumsForumDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037700, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddForumsSubscriptionForumIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037701, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddForumsSubscriptionTopicIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037702, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductDeletedPublishedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037703, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductPublishedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037704, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductShowOnHomepageIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037705, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductParentGroupedProductIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037706, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductVisibleIndividuallyIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037707, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddPCMProductCategoryIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196889689037708, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddCurrencyDisplayOrderIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647925, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductTagNameIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647926, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddActivityLogCreatedOnUtcIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647927, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddUrlRecordSlugIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647928, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddUrlRecordCustom1IX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647929, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddAclRecordEntityIdEntityNameIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647930, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddStoreMappingEntityIdEntityNameIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647931, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddCategoryLimitedToStoresIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647932, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddManufacturerLimitedToStoresIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647933, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductLimitedToStoresIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647934, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddCategorSubjectToAclIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647935, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddManufacturerSubjectToAclIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647936, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductSubjectToAclIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647937, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductCategoryMappingIsFeaturedProductIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647938, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductManufacturerMappingIsFeaturedProductIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647939, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddCustomerCustomerRoleMappingCustomerIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647940, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddProductDeleteIdIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647941, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddGetLowStockProductsIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196961091647942, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddPMMProductManufacturerIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637200411689037680, CAST(N'2020-03-18T07:23:32.000' AS DateTime), N'AddOrderRewardPointsHistoryFK')

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'securitysettings.forcesslforallpages'
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeightByTotalRecord]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ShippingByWeightByTotalRecord]') AND NAME = 'TransitDays')
BEGIN
	ALTER TABLE [ShippingByWeightByTotalRecord]
	ADD TransitDays int NULL
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[StorePickupPoint]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[StorePickupPoint]') AND NAME = 'TransitDays')
BEGIN
	ALTER TABLE [StorePickupPoint]
	ADD TransitDays int NULL
END
GO
GO

IF EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'UploadIconsArchive')
BEGIN
    UPDATE [ActivityLogType]
    SET [SystemKeyword] = N'UploadIcons', [Name] = N'Upload a favicon and app icons'
    WHERE [SystemKeyword] = N'UploadIconsArchive'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'UploadIcons')
BEGIN
   INSERT INTO [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
   VALUES (N'UploadIcons', N'Upload a favicon and app icons', 'true')
END
GO

--update setting
UPDATE [Setting]
SET [Value] = N'public,max-age=31536000'
WHERE [Name] = N'commonsettings.StaticFilesCacheControl'
GO

--delete indexe
IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_RewardPointsHistory_OrderId' and object_id=object_id(N'[RewardPointsHistory]'))
BEGIN
	DROP INDEX [IX_RewardPointsHistory_OrderId] ON [RewardPointsHistory]
END
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_RewardPointsHistory_OrderId_OrderId') AND parent_object_id = OBJECT_ID(N'RewardPointsHistory'))
	ALTER TABLE [RewardPointsHistory] DROP CONSTRAINT FK_RewardPointsHistory_OrderId_OrderId
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_RewardPointsHistory_Order_OrderId') AND parent_object_id = OBJECT_ID(N'RewardPointsHistory'))
	ALTER TABLE [RewardPointsHistory] DROP CONSTRAINT FK_RewardPointsHistory_Order_OrderId
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_RewardPointsHistory_OrderId_Order_Id') AND parent_object_id = OBJECT_ID(N'RewardPointsHistory'))
	ALTER TABLE [RewardPointsHistory] DROP CONSTRAINT FK_RewardPointsHistory_OrderId_Order_Id
GO

--delete column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RewardPointsHistory]') and NAME='OrderId')
BEGIN
	ALTER TABLE [RewardPointsHistory] DROP COLUMN OrderId
END
GO

--delete indexe
IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_MessageTemplate_EmailAccountId' and object_id=object_id(N'[MessageTemplate]'))
BEGIN
	DROP INDEX [IX_MessageTemplate_EmailAccountId] ON [MessageTemplate]
END
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_MessageTemplate_EmailAccountId_EmailAccount_Id') AND parent_object_id = OBJECT_ID(N'MessageTemplate'))
	ALTER TABLE [MessageTemplate] DROP CONSTRAINT FK_MessageTemplate_EmailAccountId_EmailAccount_Id
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_MessageTemplate_EmailAccount_EmailAccountId') AND parent_object_id = OBJECT_ID(N'MessageTemplate'))
	ALTER TABLE [MessageTemplate] DROP CONSTRAINT FK_MessageTemplate_EmailAccount_EmailAccountId
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_MessageTemplate_EmailAccountId_EmailAccountId') AND parent_object_id = OBJECT_ID(N'MessageTemplate'))
	ALTER TABLE [MessageTemplate] DROP CONSTRAINT FK_MessageTemplate_EmailAccountId_EmailAccountId
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.usestoredprocedureforloadingcategories')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'commonsettings.usestoredprocedureforloadingcategories'
END
GO

--drop the "CategoryLoadAllPaged" stored procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[CategoryLoadAllPaged]') AND OBJECTPROPERTY(OBJECT_ID, N'IsProcedure') = 1)
    DROP PROCEDURE [CategoryLoadAllPaged];
GO

--drop the "LanguagePackImport" stored procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[LanguagePackImport]') AND OBJECTPROPERTY(OBJECT_ID, N'IsProcedure') = 1)
    DROP PROCEDURE [LanguagePackImport];
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'cookiesettings.compareproductscookieexpires')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'cookiesettings.compareproductscookieexpires', N'240', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'cookiesettings.recentlyviewedproductscookieexpires')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'cookiesettings.recentlyviewedproductscookieexpires', N'240', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'cookiesettings.customercookieexpires')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'cookiesettings.customercookieexpires', N'8760', 0)
END
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_Topic_TopicTemplateId_TopicTemplate_Id') AND parent_object_id = OBJECT_ID(N'Topic'))
	ALTER TABLE [Topic] DROP CONSTRAINT FK_Topic_TopicTemplateId_TopicTemplate_Id
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'cachingsettings.shorttermcachetime')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'cachingsettings.shorttermcachetime', N'5', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'cachingsettings.defaultcachetime')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'cachingsettings.defaultcachetime', N'60', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'cachingsettings.bundledfilescachetime')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'cachingsettings.bundledfilescachetime', N'120', 0)
END
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.renderxuacompatible')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'commonsettings.renderxuacompatible'
END
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.xuacompatiblevalue')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'commonsettings.xuacompatiblevalue'
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.phonenumbervalidationenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.phonenumbervalidationenabled', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.phonenumbervalidationuseregex')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.phonenumbervalidationuseregex', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.phoneNumbervalidationrule')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.phonenumbervalidationrule', N'^[0-9]{1,14}?$', 0)
END
GO
 
--update fluent migration versions
DELETE FROM [MigrationVersionInfo] WHERE [Description] in ('AddPCMProductIdExtendedIX', 'AddPMMProductIdExtendedIX', 'AddPSAMAllowFilteringIX', 'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX', 'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX', 'AddProductVisibleIndividuallyPublishedDeletedExtendedIX', 'AddCategoryDeletedExtendedIX', 'Widgets.FacebookPixel base schema');

INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280389, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddPCMProductIdExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280390, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddPMMProductIdExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280391, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddPSAMAllowFilteringIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280392, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280393, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280394, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddProductVisibleIndividuallyPublishedDeletedExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637196977559280395, CAST(N'2020-04-30T13:53:20.000' AS DateTime), N'AddCategoryDeletedExtendedIX')
INSERT [MigrationVersionInfo] ([Version], [AppliedOn], [Description]) VALUES (637207344000000000, CAST(N'2020-04-30T13:54:21.000' AS DateTime), N'Widgets.FacebookPixel base schema')
GO

--delete FK
IF EXISTS (SELECT *  FROM sys.foreign_keys  WHERE object_id = OBJECT_ID(N'FK_StockQuantityHistory_WarehouseId_Warehouse_Id') AND parent_object_id = OBJECT_ID(N'StockQuantityHistory'))
	ALTER TABLE [StockQuantityHistory] DROP CONSTRAINT FK_StockQuantityHistory_WarehouseId_Warehouse_Id
GO

--update country
UPDATE [Country]
SET [ThreeLetterIsoCode] = 'ROU'
WHERE [TwoLetterIsoCode] = 'RO' AND [ThreeLetterIsoCode] = 'ROM'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RewardPointsHistory]') and NAME='UsedWithOrder')
BEGIN
	ALTER TABLE [RewardPointsHistory] ADD [UsedWithOrder] uniqueidentifier NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'upssettings.weighttype')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'upssettings.weighttype', N'LBS', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'upssettings.dimensionstype')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'upssettings.dimensionstype', N'IN', 0)
END
GO

--rename setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.estimateshippingenabled')
BEGIN
	UPDATE [Setting]
	SET [Name] = N'shippingsettings.estimateshippingcartpageenabled'
	WHERE [Name] = N'shippingsettings.estimateshippingenabled'
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.estimateshippingproductpageenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'shippingsettings.estimateshippingproductpageenabled', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'avalarataxsettings.enablelogging')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'avalarataxsettings.enablelogging', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.restarttimeout')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.restarttimeout', N'3000', 0)
END
GO

-- remove the Nop.Plugin.Widgets.FacebookPixel plugin migration if it table not exists
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[FacebookPixelConfiguration]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	DELETE FROM [MigrationVersionInfo] WHERE [Description] = 'Widgets.FacebookPixel base schema';
END
GO

-- remove the Nop.Plugin.Tax.FixedOrByCountryStateZip plugin migration if it table not exists
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[TaxRate]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	DELETE FROM [MigrationVersionInfo] WHERE [Description] = 'Tax.FixedOrByCountryStateZip base schema';
END
GO

-- remove the Nop.Plugin.Tax.Avalara plugin migration if it table not exists
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[TaxTransactionLog]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	DELETE FROM [MigrationVersionInfo] WHERE [Description] = 'Tax.Avalara base schema';
END
GO

-- remove the Nop.Plugin.Shipping.FixedByWeightByTotal plugin migration if it table not exists
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[ShippingByWeightByTotalRecord]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	DELETE FROM [MigrationVersionInfo] WHERE [Description] = 'Shipping.FixedByWeightByTotal base schema';
END
GO

-- remove the Nop.Plugin.Pickup.PickupInStore plugin migration if it table not exists
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[StorePickupPoint]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
	DELETE FROM [MigrationVersionInfo] WHERE [Description] = 'Pickup.PickupInStore base schema';
END
GO

--add new slug to "reservedurlrecordslugs" setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs')
BEGIN
	DECLARE @NewUrlRecord nvarchar(4000)
	SET @NewUrlRecord = N'products'
	
	DECLARE @reservedurlrecordslugs nvarchar(4000)
	SELECT @reservedurlrecordslugs = [Value] FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs'
	
	IF (PATINDEX('%[,]' + @NewUrlRecord + '[,]%', @reservedurlrecordslugs) = 0 or PATINDEX('%[,]' + @NewUrlRecord, @reservedurlrecordslugs) = 0)
	BEGIN
		UPDATE [Setting]
		SET [Value] = @reservedurlrecordslugs + ',' + @NewUrlRecord
		WHERE [name] = N'seosettings.reservedurlrecordslugs'
	END
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.requestdelay')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'shippingsettings.requestdelay', N'300', 0)
END
GO
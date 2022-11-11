--upgrade scripts from nopCommerce 4.10 to 4.20

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Title.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisableBillingAddressCheckoutStep.Hint">
    <Value>Check to disable "Billing address" step during checkout. Billing address will be pre-filled and saved using the default registration data (this option cannot be used with guest checkout enabled). Also ensure that appropriate address fields that cannot be pre-filled are not required (or disabled). If a customer doesn''t have a billing address, then the billing address step will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.RelativeDateTime.Past">
    <Value>{0} ago</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore">
    <Value>Ignore additional shipping charge for pick up in store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore.Hint">
    <Value>Check if you want ignore additional shipping charge for pick up in store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseResponseCompression">
    <Value>Use response compression</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseResponseCompression.Hint">
    <Value>Enable to compress response (gzip by default). You can disable it if you have an active IIS Dynamic Compression Module configured at the server level.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.Reserved">
    <Value>The entered text will be replaced by ''{0}'', since it is already used as a SEO-friendly name for another page or contains invalid characters</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Instructions">
    <Value>
      <![CDATA[
		<p>
			<b>If you''re using this gateway ensure that your primary store currency is supported by PayPal.</b>
			<br />
			<br />To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal. Follow these steps to configure your account for PDT:<br />
			<br />1. Log in to your PayPal account (click <a href="https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8" target="_blank">here</a> to create your account).
			<br />2. Click the Profile button.
			<br />3. Click the Profile and Settings button.
			<br />4. Select the My selling tools item on left panel.
			<br />5. Click Website Preferences Update in the Selling online section.
			<br />6. Under Auto Return for Website Payments, click the On radio button.
			<br />7. For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment ({0}).
			<br />8. Under Payment Data Transfer, click the On radio button and get your PDT identity token.
			<br />9. Click Save.
			<br />
		</p>
		]]>
    </Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.Instructions">
    <Value><![CDATA[<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href="https://developers.facebook.com/apps" target ="_blank" > Facebook for Developers</a> page and sign in. If you don''t already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App button</b> in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID button</b>.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Click <b>Get Started</b> on <b>Facebook Login</b>.</li><li>Click the <b>Settings</b> link in the menu at the left, you are presented with the <b>Client OAuth Settings</b> page with some defaults already set.</li><li>Enter "{0:s}signin-facebook" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Filtering.SpecificationFilter.Separator">
    <Value>or</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert">
    <Value>Information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Ok">
    <Value>Ok</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.States.Failed">
    <Value>Failed to retrieve states.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.FailedRetrieving">
    <Value>Failed to retrieve specification options.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Alert.FailedGetDiscountRequirements">
    <Value>Failed to load requirements info. Please refresh the page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Alert.HistoryAdd">
    <Value>Failed to add reward points.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Alert.FailedToSave">
    <Value>Failed to save requirement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.Fields.GiftCardCouponCode.Alert.FailedGenerate">
    <Value>Failed to generate code.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.CustomerStatistics.Alert.FailedLoad">
    <Value>Failed to load statistics.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.OrderStatistics.Alert.FailedLoad">
    <Value>Failed to load statistics.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.MarkAsPrimaryDimension.Alert.FailedToUpdate">
    <Value>Failed to update dimension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.MarkAsPrimaryWeight.Alert.FailedToUpdate">
    <Value>Failed to update weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Alert.Add">
    <Value>Failed to add order note.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Alert.AddNew">
    <Value>Upload picture first.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Alert.PictureAdd">
    <Value>Failed to add product picture.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedGenerate">
    <Value>Error while generating attribute combinations.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Download.SaveDownloadURL.Alert.FailedSave">
    <Value>Failed to save download object.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.AddTitle.Alert.FailedAddNote">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.FailedAdd">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Alert.Error">
    <Value>Failed to update currency.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.SelectOption">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.NoAttributeOptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.FailedToSave">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.SelectOption">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.NoAttributeOptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.FailedToSave">
    <Value>Failed to save discount requirements.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.Save.Error">
    <Value>Error while saving.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.Save.Ok">
    <Value>Successfully saved.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.Add.Error">
    <Value>Failed to add record.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Activated">
    <Value>Coupon code ({0}) is activated! The discount will be applied to your order.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Invalid">
    <Value>This coupon code ({0}) is invalid or no longer available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireDigit">
    <Value>Password must have at least one digit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireDigit.Hint">
    <Value>Specify that passwords must have at least one digit.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireLowercase">
    <Value>Password must have at least one lowercase</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireLowercase.Hint">
    <Value>Specify that password must have at least one lowercase.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireNonAlphanumeric">
    <Value>Password must have at least one non alphanumeric character</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireNonAlphanumeric.Hint">
    <Value>Specify that password must have at least one non alphanumeric character.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireUppercase">
    <Value>Password must have at least one uppercase</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireUppercase.Hint">
    <Value>Specify that passwords must have at least one uppercase.</Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.IsNotEmpty">
    <Value>Password is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireDigit">
    <Value><![CDATA[<li>must have at least one digit</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireLowercase">
    <Value><![CDATA[<li>must have at least one lowercase</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireNonAlphanumeric">
    <Value><![CDATA[<li>must have at least one special character (e.g. #?!@$%^&*-)</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireUppercase">
    <Value><![CDATA[<li>must have at least one uppercase</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.LengthValidation">
    <Value><![CDATA[<li>must have at least {0} characters</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.Rule">
    <Value><![CDATA[<p>Password must meet the following rules: </p><ul>{0}{1}{2}{3}{4}</ul>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Fields.NewPassword.LengthValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Fields.NewPassword.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Password.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Password.LengthValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.NewPassword.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.NewPassword.LengthValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="FormattedAttributes.PriceAdjustment">
    <Value> [{0}{1}{2}]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Impersonate.Inactive">
    <Value>This customer is inactive</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.CannotBeUsed">
    <Value>You cannot use this discount coupon because the validation conditions are not met</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductUseLimitedToStores">
    <Value>Export/Import products with "limited to stores"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductUseLimitedToStores.Hint">
    <Value>Check if products should be exported/imported with "limited to stores" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Import.StoresDontExist">
    <Value>Stores with the following names and/or IDs don''t exist: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Aria.SortAscending">
    <Value>: activate to sort column ascending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Aria.SortDescending">
    <Value>: activate to sort column descending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.EmptyTable">
    <Value>No data available in table</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Info">
    <Value>_START_-_END_ of _TOTAL_ items</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.InfoEmpty">
    <Value>No records</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.InfoFiltered">
    <Value>(filtered from _MAX_ total entries)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Thousands">
    <Value>,</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.lengthMenu">
    <Value>Show _MENU_ items</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.LoadingRecords">
    <Value>Loading...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Paginate.First">
    <Value>First</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Paginate.Last">
    <Value>Last</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Paginate.Next">
    <Value>Next</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Paginate.Previous">
    <Value>Previous</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Processing">
    <Value>Processing...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.Search">
    <Value>Search:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.ZeroRecords">
    <Value>No matching records found</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DT.LoaderIcon">
    <Value><![CDATA[<i class=''fa fa-refresh fa-spin''></i>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Location.Select">
    <Value>Select location</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.GroupTierPricesForDistinctShoppingCartItems">
    <Value>Group tier prices for distinct shopping cart items</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.GroupTierPricesForDistinctShoppingCartItems.Hint">
    <Value>Allows to offer special prices when customers buy bigger amounts of a particular product. For example, when a customer could have two shopping cart items for the same products (different product attributes).</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.Addresses.Invalid">
    <Value>You have {0} invalid address(es)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LowStockActivity.Hint">
    <Value>Action to be taken when your current stock quantity falls below (reaches) the ''Minimum stock quantity''. Activation of the action will occur only after an order is placed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Mappings">
    <Value>Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Mappings">
    <Value>Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Tax">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Import.DatabaseNotContainCategory">
    <Value>Database doesn''t contain the ''{0}'' category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.BlogPostId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.BlogPostId.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.NewsItemId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.NewsItemId.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.AssociatedProducts.TryToAddSelfGroupedProduct">
    <Value>You cannot add current group product to related ones. This group product was ignored while adding.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForgotPasswordPage">
    <Value>Show on forgot password page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForgotPasswordPage.Hint">
    <Value>Check to show CAPTCHA on forgot password page when restore password.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.OneColumnProductPage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.OneColumnProductPage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.CommonInfo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Mappings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Security">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.RequireOtherProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CreatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CreatedOn.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UpdatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UpdatedOn.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.CreatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.UpdatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ID.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Id">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Mappings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Prices">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.LinkedProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.AdvancedProductTypes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Security">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Product.Hint">
    <Value>Search by a specific product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.BillingCountry">
    <Value>Billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.BillingCountry.Hint">
    <Value>Filter by billing country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Store.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSizeOptions">
    <Value>Page size options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSizeOptions">
    <Value>Page size options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagPageSizeOptions">
    <Value>''Products by tag'' page size options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions">
    <Value>Search page. Page size options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSizeOptions">
    <Value>Page size options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.IncludeInSitemap">
    <Value>Include in sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.IncludeInSitemap.Hint">
    <Value>Check to include this blog post in the sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeBlogPosts">
    <Value>Sitemap includes blog posts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeBlogPosts.Hint">
    <Value>Check if you want to include blog posts in sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTopics">
    <Value>Sitemap includes topics</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTopics.Hint">
    <Value>Check if you want to include topics in sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.BlogPosts">
    <Value>Blog posts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews">
    <Value>Sitemap includes news items</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews.Hint">
    <Value>Check if you want to include news items in sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.News">
    <Value>News</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Sitemap.Instructions">
    <Value><![CDATA[<p>These settings do not apply to sitemap.xml, only for your site map. You can configure generation for sitemap.xml on <a href="{0}">all settings page</a></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GooglePlusLink">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GooglePlusLink.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Footer.FollowUs.GooglePlus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.LogUserProfileChanges">
    <Value>Log user profile changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Gdpr.LogUserProfileChanges.Hint">
    <Value>Check to log user profile changes (if this feature is enabled in your store).</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Gdpr.GdprRequestType.ProfileChanged">
    <Value>User changed profile</Value>
  </LocaleResource>
  <LocaleResource Name="ScheduleTasks.Error">
    <Value>The "{0}" scheduled task failed with the "{1}" error (Task type: "{2}". Store name: "{3}". Task run address: "{4}").</Value>
  </LocaleResource>
  <LocaleResource Name="ScheduleTasks.TimeoutError">
    <Value>A scheduled task canceled. Timeout expired.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.ShowOnHomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.ShowOnHomePage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShowOnHomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShowOnHomePage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ShowOnHomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.ShowOnHomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.ShowOnHomePage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="HomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="HomePage.Products">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.ShowOnHomepage">
    <Value>Show on home page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.ShowOnHomepage.Hint">
    <Value>Check if you want to show a category on home page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShowOnHomepage">
    <Value>Show on home page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShowOnHomepage.Hint">
    <Value>Check to display this product on your store''s home page. Recommended for your most popular products.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem">
    <Value>Display "Home page"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem.Hint">
    <Value>Check if "Home page" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ShowOnHomepage">
    <Value>Show on home page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.ShowOnHomepage">
    <Value>Show on home page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.ShowOnHomepage.Hint">
    <Value>Check if you want to show poll on the home page.</Value>
  </LocaleResource>
  <LocaleResource Name="Homepage">
    <Value>Home page</Value>
  </LocaleResource>
  <LocaleResource Name="Homepage.Products">
    <Value>Featured products</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Plugins.LoadPluginsMode.All">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Plugins.LoadPluginsMode.InstalledOnly">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Plugins.LoadPluginsMode.NotInstalledOnly">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Services.Plugins.LoadPluginsMode.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Services.Plugins.LoadPluginsMode.InstalledOnly">
    <Value>Installed</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Services.Plugins.LoadPluginsMode.NotInstalledOnly">
    <Value>Not installed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification">
    <Value>Html minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification.Hint">
    <Value>Allows you to minify HTML pages as well as compress them, thereby increasing the download speed. Please note that after applying this setting, you need to restart the application.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Minification">
    <Value>Minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ProxyConnection.Failed">
    <Value>Proxy connection failed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ProxyConnection.OK">
    <Value>Proxy connection is OK</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchIncludeSubCategories">
    <Value>Search subcategories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchIncludeSubCategories.Hint">
    <Value>Check to search in subcategories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page size options should not have duplicate items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page size options should have unique items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page size options should have unique items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Manage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Restrictions.Manage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Forums.Manage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Url">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.Url.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.UseSandbox">
    <Value>Use sandbox</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.UseSandbox.Hint">
    <Value>Check to use sandbox (testing environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled">
    <Value>Saturday Delivery enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled.Hint">
    <Value>Check to get rates for Saturday Delivery options.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Errors">
    <Value><![CDATA[The store has some error(s) or warning(s). Please find more information on the <a href="{0}">Warnings</a> page]]></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Login.Fields.UserName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Login.Fields.Username">
    <Value>Username</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickupInStore">
    <Value>"Pick Up in Store" enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickupInStore.Hint">
    <Value>A value indicating whether "Pick Up in Store" option is enabled during checkout. Please ensure that you have at least one active pickup point provider.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickupInStore">
    <Value>Ignore additional shipping charge for pick up in store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickupInStore.Hint">
    <Value>Check if you want ignore additional shipping charge for pick up in store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.ManageInventoryMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.ManageInventoryMethod.MultipleWarehouse">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.OldPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.Price">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.Published">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.SKU">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.Fields.StockQuantity">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.List.SearchCategory">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.List.SearchCategory.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.List.SearchManufacturer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.List.SearchManufacturer.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.List.SearchProductName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.BulkEdit.List.SearchProductName.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.BulkEdit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.All">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Display">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Empty">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.First">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.ItemsPerPage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Last">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.MorePages">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Next">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Of">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Page">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Previous">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Refresh">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.Fields.Note.Validation">
    <Value>Vendor note can not be empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Fields.Store.Hint">
    <Value>A store name in which this setting is applied.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Fields.Name.Hint">
    <Value>Enter the locale resource name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Fields.Value.Hint">
    <Value>Enter the locale resource value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name.Hint">
    <Value>Enter the dimension name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword.Hint">
    <Value>Enter the dimension system keyword.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Ratio.Hint">
    <Value>Specify the dimension ratio.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.DisplayOrder.Hint">
    <Value>The display order of this dimension. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Name.Hint">
    <Value>Enter the weight name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword.Hint">
    <Value>Enter the weight system keyword.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio.Hint">
    <Value>Specify the weight ratio.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder.Hint">
    <Value>The display order of this weight. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Answers.Fields.Name.Hint">
    <Value>Enter the poll answer name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder.Hint">
    <Value>The display order of this poll answer. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Fields.Name.Hint">
    <Value>Enter the setting name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Fields.Value.Hint">
    <Value>Enter the setting value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Tax.Categories.Fields.Name.Hint">
    <Value>Enter the tax category name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Tax.Categories.Fields.DisplayOrder.Hint">
    <Value>The display order of this tax category. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Category.Name.Hint">
    <Value>Enter the template name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Category.DisplayOrder.Hint">
    <Value>The display order of this template. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Category.ViewPath.Hint">
    <Value>Enter the template view path.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Manufacturer.Name.Hint">
    <Value>Enter the template name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Manufacturer.DisplayOrder.Hint">
    <Value>The display order of this template. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Manufacturer.ViewPath.Hint">
    <Value>Enter the template view path.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.Name.Hint">
    <Value>Enter the template name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.DisplayOrder.Hint">
    <Value>The display order of this template. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.ViewPath.Hint">
    <Value>Enter the template view path.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.IgnoredProductTypes.Hint">
    <Value>Specify ignored product types for this template.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.Name.Hint">
    <Value>Enter the template name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.DisplayOrder.Hint">
    <Value>The display order of this template. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.ViewPath.Hint">
    <Value>Enter the template view path.</Value>
  </LocaleResource>
  <LocaleResource Name="Reviews.Fields.Rating.Good">
    <Value>Good</Value>
  </LocaleResource>
  <LocaleResource Name="Reviews.Fields.Rating.NotBadNotExcellent">
    <Value>Not bad but also not excellent</Value>
  </LocaleResource>
  <LocaleResource Name="Reviews.Fields.Rating.NotGood">
    <Value>Not good</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.RedisEnabled">
    <Value>Redis enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UseRedisToStoreDataProtectionKeys">
    <Value>Use Redis to store Data Protection Keys</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UseRedisForCaching">
    <Value>Use Redis for Caching</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UseRedisToStorePluginsInfo">
    <Value>Use Redis to store Plugins Info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.AzureBlobStorageEnabled">
    <Value>Azure Blob Storage enabled</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.System.SystemInfo.RedisEnabled.Hint">
    <Value>Indicates whether Redis is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UseRedisToStoreDataProtectionKeys.Hint">
    <Value>Indicates whether Redis is used to store Data Protection Keys.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UseRedisForCaching.Hint">
    <Value>Indicates whether Redis is used for Caching.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UseRedisToStorePluginsInfo.Hint">
    <Value>Indicates whether Redis is used to store Plugins Info.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.AzureBlobStorageEnabled.Hint">
    <Value>Indicates whether Azure Blob Storage is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.CurrentStaticCacheManager">
	<Value>Static cache manager name</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.System.SystemInfo.CurrentStaticCacheManager.Hint">
	<Value>Indicating the current static cache manager name.</Value>
  </LocaleResource>
  <LocaleResource Name="Languages.Selector.Label">
    <Value>Languages selector</Value>
  </LocaleResource>
  <LocaleResource Name="Tax.Selector.Label">
    <Value>Tax selector</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Specs.AttributeName">
    <Value>Attribute name</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Specs.AttributeValue">
    <Value>Attribute value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.CustomerRolesManagingError">
    <Value>Not enough rights to manage customer roles.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductBoxes.Hint">
    <Value>Check to display tax and shipping information in product boxes (catalog pages). This option is required in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter.Hint">
    <Value>Check to display tax and shipping information in the footer. This option is required in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductDetailsPage.Hint">
    <Value>Check to display tax and shipping information on product details pages. This option is required in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage.Hint">
    <Value>Check to display tax and shipping information on the order details page. This option is required in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart.Hint">
    <Value>Check to display tax and shipping information on the shopping cart page. This option is required in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist.Hint">
    <Value>Check to display tax and shipping information on the wishlist page. This option is required in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Plugins.Errors.UninstallDependsOn">
    <Value>The following plugins depend on the "{0}" plugin and must be uninstalled beforehand: "{1}"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Plugins.Errors.InstallDependsOn">
    <Value>The "{0}" plugin depends on the following plugins which must be installed beforehand: "{1}"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.Description">
    <Value>Favicon and app icons are small pictures associated with a particular website or web page. They are displayed by the browser in the tab before the page title, and as a picture next to a bookmark, in tabs and in other interface elements. You can see an example of the favicon and app icons archive in /icons/samples in the root of the site.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.MissingFile">
    <Value>Could not find file {0}. This file is required. It contains the code for the page head element.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIconsArchive">
    <Value>Upload icons archive</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIconsArchive.Hint">
    <Value>Upload archive with favicon and app icons for different operating systems and devices. You can see an example of the favicon and app icons archive in /icons/samples in the root of the site. Your favicon and app icons path is "icons/icons_{0}"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.FaviconAndAppIcons.Uploaded">
    <Value>Favicon and app icons have been uploaded</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.FaviconAndAppIcons">
    <Value>Favicon and app icons</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.ApplyChanges">
    <Value>Restart application to apply changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.DiscardChanges">
    <Value>Discard changes</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Plugins.ApplyChanges.Progress">
    <Value>Applying changes on plugins...</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Configuration.Plugins.DiscardChanges.Progress">
    <Value>Discarding changes on plugins...</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Configuration.Plugins.ChangesApplyAfterReboot">
    <Value>Changes will be apply after restart application</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Plugins.Errors.NotDeleted">
    <Value>The plugin "{0}" not deleted</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Plugins.Errors.NotInstalled">
    <Value>The plugin "{0}" not installed</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Plugins.Errors.NotUninstalled">
    <Value>The plugin "{0}" not uninstalled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PluginRequiredAssembly">
    <Value>the ''{0}'' plugin required the ''{1}'' assembly</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.System.Warnings.AssemblyHasCollision">
    <Value>The ''{0}'' assembly has collision, application loaded the ''{1}'' assembly, but {2}</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Plugins.Deleted">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Installed">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Plugins.Uninstalled">
    <Value></Value>
  </LocaleResource>   
  <LocaleResource Name="ActivityLog.UploadNewIconsArchive">
    <Value>Uploaded a new favicon and app icons archive for store (ID = ''{0}'')</Value>
  </LocaleResource>  
</Language>'

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

UPDATE [Topic] 
SET [IncludeInFooterColumn1] = 0
WHERE [SystemName] = 'VendorTermsOfService'
GO

ALTER TABLE [Topic] ALTER COLUMN [Title] nvarchar(max) NULL
GO

-- #3236
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='IncludeInSitemap')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [IncludeInSitemap] bit NULL
END
GO

UPDATE [BlogPost]
SET [IncludeInSitemap] = 0
WHERE [IncludeInSitemap] IS NULL
GO

ALTER TABLE [BlogPost]
ALTER COLUMN [IncludeInSitemap] bit NOT NULL
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


		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
	

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[ShortDescription], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[ShortDescription]) > 0 '


			--product full description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[FullDescription], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[FullDescription]) > 0 '



			--localized product short description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
				

			--localized product full description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE p.[Sku] = @OriginalKeywords '
		END

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

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.ignoreadditionalshippingchargeforpickupinstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.ignoreadditionalshippingchargeforpickupinstore', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.usericheditorforcustomeremails')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'adminareasettings.usericheditorforcustomeremails', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'messagessettings.usepopupnotifications')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'messagessettings.usepopupnotifications', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequirelowercase')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequirelowercase', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequireuppercase')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequireuppercase', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequirenonalphanumeric')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequirenonalphanumeric', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequiredigit')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequiredigit', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'catalogsettings.exportimportproductuselimitedtostores')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.exportimportproductuselimitedtostores', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'catalogsettings.useajaxloadmenu')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.useajaxloadmenu', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludetopics')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapsettings.sitemapincludetopics', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludeblogposts')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapsettings.sitemapincludeblogposts', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludenews')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapsettings.sitemapincludenews', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlenabled', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludeblogposts')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludeblogposts', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludecategories')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludecategories', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludecustomurls')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludecustomurls', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludemanufacturers')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludemanufacturers', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludeproducts')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludeproducts', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludeproducttags')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludeproducttags', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludetopics')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludetopics', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapxmlincludenews')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'sitemapxmlsettings.sitemapxmlincludenews', N'True', 0)
END
GO

--update old settings (#3236)
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapenabled')
BEGIN
	UPDATE [Setting] 
	SET [Name] = 'sitemapsettings.sitemapenabled'
	WHERE [Name] = 'commonsettings.sitemapenabled'
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludecategories')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'sitemapsettings.sitemapincludecategories'
	WHERE [Name] = 'commonsettings.sitemapincludecategories'
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludemanufacturers')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'sitemapsettings.sitemapincludemanufacturers'
	WHERE [Name] = 'commonsettings.sitemapincludemanufacturers'
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludeproducts')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'sitemapsettings.sitemapincludeproducts'
	WHERE [Name] = 'commonsettings.sitemapincludeproducts'
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemapincludeproducttags')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'sitemapsettings.sitemapincludeproducttags'
	WHERE [Name] = 'commonsettings.sitemapincludeproducttags'
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapsettings.sitemappagesize')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'sitemapsettings.sitemappagesize'
	WHERE [Name] = 'commonsettings.sitemappagesize'
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'sitemapxmlsettings.sitemapcustomurls')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'sitemapxmlsettings.sitemapcustomurls'
	WHERE [Name] = 'commonsettings.sitemapcustomurls'
END
GO

--updating of indexes in the Picture table for reduced table size after upgrade nopCommerce from 4.00 to 4.10 version
ALTER INDEX ALL ON [Picture] REBUILD
GO

--new activity log type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [Name] = N'Upload a favicon and app icons archive')
BEGIN
	INSERT [ActivityLogType] ( [SystemKeyword], [Name], [Enabled]) VALUES ( N'UploadIconsArchive', N'Upload a favicon and app icons archive', 1)
END
GO

--new ground shipping description
UPDATE [ShippingMethod] 
SET [Description] = 'Shipping by land transport'
WHERE [Name] = 'Ground'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.onecolumnproductpage'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.createdon'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.updatedon'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.id'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'adminareasettings.usenestedsetting'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.minificationenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.minificationenabled', N'True', 0)
END
GO

--update setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.enablehtmlminification')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'commonsettings.enablehtmlminification'
	WHERE [Name] = 'commonsettings.minificationenabled'
END
GO

--update the "ProductTagCountLoadAll" stored procedure
ALTER PROCEDURE [ProductTagCountLoadAll]
(
	@StoreId int,
	@AllowedCustomerRoleIds	nvarchar(MAX) = null	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
)
AS
BEGIN
	SET NOCOUNT ON
		
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
	
	SELECT pt.Id as [ProductTagId], COUNT(p.Id) as [ProductCount]
	FROM ProductTag pt with (NOLOCK)
	LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON pt.[Id] = pptm.[ProductTag_Id]
	LEFT JOIN Product p with (NOLOCK) ON pptm.[Product_Id] = p.[Id]
	WHERE
		p.[Deleted] = 0
		AND p.Published = 1
		AND (@StoreId = 0 or (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = 'Product' and [sm].StoreId=@StoreId
			)))
		AND (@FilteredCustomerRoleIdsCount = 0 or (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = 'Product'
				))
			))
	GROUP BY pt.Id
	ORDER BY pt.Id
END
GO


--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'storeinformationsettings.googlepluslink'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'gdprsettings.loguserprofilechanges')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'gdprsettings.loguserprofilechanges', N'True', 0)
END
GO

--alter column
ALTER TABLE [Setting] ALTER COLUMN [Value] [nvarchar](max) NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'mediasettings.useabsoluteimagepath')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'mediasettings.useabsoluteimagepath', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.scheduletaskruntimeout')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.scheduletaskruntimeout', N'', 0)
END
GO

--rename columns
EXEC sp_RENAME 'Category.ShowOnHomePage' , 'ShowOnHomepage', 'COLUMN'
GO

EXEC sp_RENAME 'Poll.ShowOnHomePage' , 'ShowOnHomepage', 'COLUMN'
GO

EXEC sp_RENAME 'Product.ShowOnHomePage' , 'ShowOnHomepage', 'COLUMN'
GO

--update setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.enablejsbundling')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'commonsettings.enablejsbundling'
	WHERE [Name] = 'seosettings.enablejsbundling'
END
GO

--update setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'commonsettings.enablecssbundling')
BEGIN
	UPDATE [Setting]
	SET [Name] = 'commonsettings.enablecssbundling'
	WHERE [Name] = 'seosettings.enablecssbundling'
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'paymentsettings.regenerateorderguidinterval')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'paymentsettings.regenerateorderguidinterval', N'180', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.enabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.enabled', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.address')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.address', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.port')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.port', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.username')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.username', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.password')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.password', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.bypassonlocal')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.bypassonlocal', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'proxysettings.preauthenticate')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'proxysettings.preauthenticate', N'True', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'upssettings.usesandbox')
BEGIN
    IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'upssettings.url' AND [Value] LIKE '%wwwcie.ups.com%')
        INSERT [Setting] ([Name], [Value], [StoreId]) VALUES (N'upssettings.usesandbox', N'True', 0)
    ELSE
        INSERT [Setting] ([Name], [Value], [StoreId]) VALUES (N'upssettings.usesandbox', N'False', 0)    
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'upssettings.saturdaydeliveryenabled')
BEGIN
    IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'upssettings.carrierservicesoffered' AND [Value] LIKE '%[sa]%')
        INSERT [Setting] ([Name], [Value], [StoreId]) VALUES (N'upssettings.saturdaydeliveryenabled', N'True', 0)
    ELSE
        INSERT [Setting] ([Name], [Value], [StoreId]) VALUES (N'upssettings.saturdaydeliveryenabled', N'False', 0)
END
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'upssettings.url'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'squarepaymentsettings.refreshtoken')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'squarepaymentsettings.refreshtoken', N'00000000-0000-0000-0000-000000000000', 0)
END
GO

--rename column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Order]') and NAME='PickUpInStore')
BEGIN
    EXEC sp_RENAME '[Order].[PickUpInStore]', 'PickupInStore', 'COLUMN'
END
GO

--update setting
UPDATE [Setting]
SET [Value] = '7'
WHERE [Name] = 'adminareasettings.popupgridpagesize'
GO

--update setting
UPDATE [Setting]
SET [Value] = '7, 15, 20, 50, 100'
WHERE [Name] = 'adminareasettings.gridpagesizes'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Picture]') AND NAME = 'VirtualPath')
BEGIN
	ALTER TABLE [Picture] ADD
	VirtualPath nvarchar(MAX) NULL
END
GO

--update datetime fields to change type
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ActivityLog' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog]
	ALTER TABLE [ActivityLog] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
	CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] DESC)
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Address' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog]
	ALTER TABLE [Address] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
	CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] DESC)
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Address' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Address] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BackInStockSubscription' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [BackInStockSubscription] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BlogComment' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [BlogComment] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BlogPost' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [BlogPost] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Campaign' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Campaign] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Category' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Category] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Currency' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Currency] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_Customer_CreatedOnUtc] ON [Customer]
	ALTER TABLE [Customer] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
	CREATE NONCLUSTERED INDEX [IX_Customer_CreatedOnUtc] ON [Customer] ([CreatedOnUtc] DESC)
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomerPassword' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [CustomerPassword] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DiscountUsageHistory' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [DiscountUsageHistory] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Forum' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Forum] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Group' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Group] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Post' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Post] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_PostVote' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_PostVote] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_PrivateMessage' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_PrivateMessage] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Subscription' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Subscription] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Topic' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Topic] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GdprLog' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [GdprLog] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GiftCard' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [GiftCard] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GiftCardUsageHistory' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [GiftCardUsageHistory] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Log' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_Log_CreatedOnUtc] ON [Log]
	ALTER TABLE [Log] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
	CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] DESC)
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Manufacturer' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Manufacturer] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'News' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [News] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NewsComment' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [NewsComment] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NewsLetterSubscription' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [NewsLetterSubscription] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Order' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_Order_CreatedOnUtc] ON [Order]
	ALTER TABLE [Order] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
	CREATE NONCLUSTERED INDEX [IX_Order_CreatedOnUtc] ON [Order] ([CreatedOnUtc] DESC)
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OrderNote' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [OrderNote] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PollVotingRecord' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [PollVotingRecord] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Product] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ProductReview' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ProductReview] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QueuedEmail' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail]
	ALTER TABLE [QueuedEmail] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
	CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] DESC)
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RecurringPayment' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [RecurringPayment] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RecurringPaymentHistory' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [RecurringPaymentHistory] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ReturnRequest' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ReturnRequest] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RewardPointsHistory' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [RewardPointsHistory] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Shipment' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Shipment] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ShoppingCartItem' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ShoppingCartItem] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StockQuantityHistory' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [StockQuantityHistory] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'VendorNote' AND COLUMN_NAME = 'CreatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [VendorNote] ALTER column [CreatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BlogPost' AND COLUMN_NAME = 'StartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [BlogPost] ALTER column [StartDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BlogPost' AND COLUMN_NAME = 'EndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [BlogPost] ALTER column [EndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Campaign' AND COLUMN_NAME = 'DontSendBeforeDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Campaign] ALTER column [DontSendBeforeDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Category' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Category] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Currency' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Currency] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Forum' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Forum] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Group' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Group] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Post' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Post] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Topic' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Topic] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Manufacturer' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Manufacturer] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Product] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ReturnRequest' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ReturnRequest] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ShoppingCartItem' AND COLUMN_NAME = 'UpdatedOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ShoppingCartItem] ALTER column [UpdatedOnUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'CannotLoginUntilDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Customer] ALTER column [CannotLoginUntilDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'LastLoginDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Customer] ALTER column [LastLoginDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'LastActivityDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Customer] ALTER column [LastActivityDateUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Discount' AND COLUMN_NAME = 'StartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Discount] ALTER column [StartDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Discount' AND COLUMN_NAME = 'EndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Discount] ALTER column [EndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Forum' AND COLUMN_NAME = 'LastPostTime' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Forum] ALTER column [LastPostTime] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Forums_Topic' AND COLUMN_NAME = 'LastPostTime' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Forums_Topic] ALTER column [LastPostTime] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'News' AND COLUMN_NAME = 'StartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [News] ALTER column [StartDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'News' AND COLUMN_NAME = 'EndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [News] ALTER column [EndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Order' AND COLUMN_NAME = 'PaidDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Order] ALTER column [PaidDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OrderItem' AND COLUMN_NAME = 'RentalStartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [OrderItem] ALTER column [RentalStartDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OrderItem' AND COLUMN_NAME = 'RentalEndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [OrderItem] ALTER column [RentalEndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Poll' AND COLUMN_NAME = 'StartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Poll] ALTER column [StartDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Poll' AND COLUMN_NAME = 'EndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Poll] ALTER column [EndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'PreOrderAvailabilityStartDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Product] ALTER column [PreOrderAvailabilityStartDateTimeUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'MarkAsNewStartDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Product] ALTER column [MarkAsNewStartDateTimeUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'MarkAsNewEndDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Product] ALTER column [MarkAsNewEndDateTimeUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'AvailableStartDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_Product_PriceDatesEtc] ON [Product]
	DROP INDEX [IX_Product_VisibleIndividually_Published_Deleted_Extended] ON [Product]
	ALTER TABLE [Product] ALTER column [AvailableStartDateTimeUtc] [datetime2](7) NULL
	CREATE NONCLUSTERED INDEX [IX_Product_PriceDatesEtc] ON [Product]  ([Price] ASC, [AvailableStartDateTimeUtc] ASC, [AvailableEndDateTimeUtc] ASC, [Published] ASC, [Deleted] ASC)
	CREATE NONCLUSTERED INDEX [IX_Product_VisibleIndividually_Published_Deleted_Extended] ON [Product] ([VisibleIndividually],[Published],[Deleted]) INCLUDE ([Id],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc])
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'AvailableEndDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_Product_PriceDatesEtc] ON [Product]
	DROP INDEX [IX_Product_VisibleIndividually_Published_Deleted_Extended] ON [Product]
	ALTER TABLE [Product] ALTER column [AvailableEndDateTimeUtc] [datetime2](7) NULL
	CREATE NONCLUSTERED INDEX [IX_Product_PriceDatesEtc] ON [Product]  ([Price] ASC, [AvailableStartDateTimeUtc] ASC, [AvailableEndDateTimeUtc] ASC, [Published] ASC, [Deleted] ASC)
	CREATE NONCLUSTERED INDEX [IX_Product_VisibleIndividually_Published_Deleted_Extended] ON [Product] ([VisibleIndividually],[Published],[Deleted]) INCLUDE ([Id],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc])
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QueuedEmail' AND COLUMN_NAME = 'DontSendBeforeDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended] ON [QueuedEmail]
	ALTER TABLE [QueuedEmail] ALTER column [DontSendBeforeDateUtc] [datetime2](7) NULL
	CREATE NONCLUSTERED INDEX [IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended] ON [QueuedEmail] ([SentOnUtc], [DontSendBeforeDateUtc]) INCLUDE ([SentTries])
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QueuedEmail' AND COLUMN_NAME = 'SentOnUtc' and DATA_TYPE = 'datetime')
BEGIN
	DROP INDEX [IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended] ON [QueuedEmail]
	ALTER TABLE [QueuedEmail] ALTER column [SentOnUtc] [datetime2](7) NULL
	CREATE NONCLUSTERED INDEX [IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended] ON [QueuedEmail] ([SentOnUtc], [DontSendBeforeDateUtc]) INCLUDE ([SentTries])
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RecurringPayment' AND COLUMN_NAME = 'StartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [RecurringPayment] ALTER column [StartDateUtc] [datetime2](7) NOT NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RewardPointsHistory' AND COLUMN_NAME = 'EndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [RewardPointsHistory] ALTER column [EndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ScheduleTask' AND COLUMN_NAME = 'LastStartUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ScheduleTask] ALTER column [LastStartUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ScheduleTask' AND COLUMN_NAME = 'LastEndUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ScheduleTask] ALTER column [LastEndUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ScheduleTask' AND COLUMN_NAME = 'LastSuccessUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ScheduleTask] ALTER column [LastSuccessUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Shipment' AND COLUMN_NAME = 'ShippedDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Shipment] ALTER column [ShippedDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Shipment' AND COLUMN_NAME = 'DeliveryDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [Shipment] ALTER column [DeliveryDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ShoppingCartItem' AND COLUMN_NAME = 'RentalStartDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ShoppingCartItem] ALTER column [RentalStartDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ShoppingCartItem' AND COLUMN_NAME = 'RentalEndDateUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [ShoppingCartItem] ALTER column [RentalEndDateUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TierPrice' AND COLUMN_NAME = 'StartDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [TierPrice] ALTER column [StartDateTimeUtc] [datetime2](7) NULL
END
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TierPrice' AND COLUMN_NAME = 'EndDateTimeUtc' and DATA_TYPE = 'datetime')
BEGIN
	ALTER TABLE [TierPrice] ALTER column [EndDateTimeUtc] [datetime2](7) NULL
END
GO	

-- alter procedure
ALTER PROCEDURE [FullText_Enable]
AS
BEGIN
	--create catalog
	EXEC('
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		CREATE FULLTEXT CATALOG [nopCommerceFullTextCatalog] AS DEFAULT')

	DECLARE @SQL nvarchar(500);
	DECLARE @index_name nvarchar(1000)
	DECLARE @ParmDefinition nvarchar(500);

	SELECT @SQL = N'SELECT @index_name_out = i.name FROM sys.tables AS tbl INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id) WHERE (i.is_unique=1 and i.is_disabled=0) and (tbl.name=@table_name)'
	SELECT @ParmDefinition = N'@table_name varchar(100), @index_name_out nvarchar(1000) OUTPUT'

	EXEC sp_executesql @SQL, @ParmDefinition, @table_name = 'Product', @index_name_out=@index_name OUTPUT
	
	--create indexes
	DECLARE @create_index_text nvarchar(4000)
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		CREATE FULLTEXT INDEX ON [Product]([Name], [ShortDescription], [FullDescription])
		KEY INDEX [' + @index_name +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	EXEC sp_executesql @SQL, @ParmDefinition, @table_name = 'LocalizedProperty', @index_name_out=@index_name OUTPUT
	
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		CREATE FULLTEXT INDEX ON [LocalizedProperty]([LocaleValue])
		KEY INDEX [' + @index_name +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	EXEC sp_executesql @SQL, @ParmDefinition, @table_name = 'ProductTag', @index_name_out=@index_name OUTPUT

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		CREATE FULLTEXT INDEX ON [ProductTag]([Name])
		KEY INDEX [' + @index_name +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
END
GO

-- alter procedure
ALTER PROCEDURE [CategoryLoadAllPaged]
(
    @ShowHidden         BIT = 0,
    @Name               NVARCHAR(MAX) = NULL,
    @StoreId            INT = 0,
    @CustomerRoleIds	NVARCHAR(MAX) = NULL,
    @PageIndex			INT = 0,
	@PageSize			INT = 2147483644,
    @TotalRecords		INT = NULL OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

    --filter by customer role IDs (access control list)
	SET @CustomerRoleIds = ISNULL(@CustomerRoleIds, '')
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId INT NOT NULL
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data AS INT) FROM [nop_splitstring_to_table](@CustomerRoleIds, ',')
	DECLARE @FilteredCustomerRoleIdsCount INT = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)

    --ordered categories
    CREATE TABLE #OrderedCategoryIds
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[CategoryId] int NOT NULL
	)
    
    --get max length of DisplayOrder and Id columns (used for padding Order column)
    DECLARE @lengthId INT = (SELECT LEN(MAX(Id)) FROM [Category])
    DECLARE @lengthOrder INT = (SELECT LEN(MAX(DisplayOrder)) FROM [Category])

    --get category tree
    ;WITH [CategoryTree]
    AS (SELECT [Category].[Id] AS [Id], 
		(select RIGHT(REPLICATE('0', @lengthOrder)+ RTRIM(CAST([Category].[DisplayOrder] AS NVARCHAR(MAX))), @lengthOrder)) + '-' + (select RIGHT(REPLICATE('0', @lengthId)+ RTRIM(CAST([Category].[Id] AS NVARCHAR(MAX))), @lengthId))  AS [Order]
        FROM [Category] WHERE [Category].[ParentCategoryId] = 0
        UNION ALL
        SELECT [Category].[Id] AS [Id], 
		[CategoryTree].[Order] + '|' + (select RIGHT(REPLICATE('0', @lengthOrder)+ RTRIM(CAST([Category].[DisplayOrder] AS NVARCHAR(MAX))), @lengthOrder)) + '-' + (select RIGHT(REPLICATE('0', @lengthId)+ RTRIM(CAST([Category].[Id] AS NVARCHAR(MAX))), @lengthId))  AS [Order]
        FROM [Category]
        INNER JOIN [CategoryTree] ON [CategoryTree].[Id] = [Category].[ParentCategoryId])
    INSERT INTO #OrderedCategoryIds ([CategoryId])
    SELECT [Category].[Id]
    FROM [CategoryTree]
    RIGHT JOIN [Category] ON [CategoryTree].[Id] = [Category].[Id]

    --filter results
    WHERE [Category].[Deleted] = 0
    AND (@ShowHidden = 1 OR [Category].[Published] = 1)
    AND (@Name IS NULL OR @Name = '' OR [Category].[Name] LIKE ('%' + @Name + '%'))
    AND (@ShowHidden = 1 OR @FilteredCustomerRoleIdsCount  = 0 OR [Category].[SubjectToAcl] = 0
        OR EXISTS (SELECT 1 FROM #FilteredCustomerRoleIds [roles] WHERE [roles].[CustomerRoleId] IN
            (SELECT [acl].[CustomerRoleId] FROM [AclRecord] acl WITH (NOLOCK) WHERE [acl].[EntityId] = [Category].[Id] AND [acl].[EntityName] = 'Category')
        )
    )
    AND (@StoreId = 0 OR [Category].[LimitedToStores] = 0
        OR EXISTS (SELECT 1 FROM [StoreMapping] sm WITH (NOLOCK)
			WHERE [sm].[EntityId] = [Category].[Id] AND [sm].[EntityName] = 'Category' AND [sm].[StoreId] = @StoreId
		)
    )
    ORDER BY ISNULL([CategoryTree].[Order], 1)

    --total records
    SET @TotalRecords = @@ROWCOUNT

    --paging
    SELECT [Category].* FROM #OrderedCategoryIds AS [Result] INNER JOIN [Category] ON [Result].[CategoryId] = [Category].[Id]
    WHERE ([Result].[Id] > @PageSize * @PageIndex AND [Result].[Id] <= @PageSize * (@PageIndex + 1))
    ORDER BY [Result].[Id]

    DROP TABLE #FilteredCustomerRoleIds
    DROP TABLE #OrderedCategoryIds
END

-- delete unused functions
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[nop_getnotnullnotempty]') AND type = N'FN')
BEGIN
	DROP FUNCTION [nop_getnotnullnotempty]
END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[nop_getprimarykey_indexname]') AND type = N'FN')
BEGIN
	DROP FUNCTION [nop_getprimarykey_indexname]
END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[nop_padright]') AND type = N'FN')
BEGIN
	DROP FUNCTION [nop_padright]
END
GO

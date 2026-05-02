--upgrade scripts from nopCommerce 2.50 to nopCommerce 2.60

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.RecurringPayments.Fields.Customer">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.Fields.Customer.Hint">
    <Value>Customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToCategories">
    <Value>Assigned to categories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToCategories.Hint">
    <Value>A list of categories to which the discount is to be applied. You can assign this discount on a category details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToCategories.NoRecords">
    <Value>No categories selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProductVariants">
    <Value>Assigned to product variants</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProductVariants.Hint">
    <Value>A list of product variants to which the discount is to be applied. You can assign this discount on a product variant details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AppliedToProductVariants.NoRecords">
    <Value>No products selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.IncompatiblePlugin">
    <Value>''{0}'' plugin is incompatible with your nopCommerce version. Delete it or update to the latest version.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.StartDate.Hint">
    <Value>Set the news item start date or leave empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.EndDate.Hint">
    <Value>Set the news item end date or leave empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate.Hint">
    <Value>Set the blog post start date or leave empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate.Hint">
    <Value>Set the blog post end date or leave empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List">
    <Value>Shipments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StartDate.Hint">
    <Value>The start date (shipped on) for the search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.EndDate.Hint">
    <Value>The end date (shipped on) for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.OrderID">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.All">
    <Value>Print packaging slips (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.Selected">
    <Value>Print packaging slips (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Fields.Name.Hint">
    <Value>Enter shipping method name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Fields.DisplayOrder.Hint">
    <Value>The display order of this shipping method. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Fields.Description.Hint">
    <Value>Enter shipping method description.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.Install.Progress">
    <Value>Installing plugin...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.Uninstall.Progress">
    <Value>Uninstalling plugin...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Header.RestartApplication.Progress">
    <Value>Restarting the application...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Yes">
    <Value>Yes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchCategoryName">
    <Value>Category name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchCategoryName.Hint">
    <Value>A category name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchManufacturerName">
    <Value>Manufacturer name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchManufacturerName.Hint">
    <Value>A manufacturer name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.ShowEmails.ShowEmails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.ShowEmails.ShowUsernames">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.ShowEmails.ShowFullNames">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.CustomerNameFormat.ShowEmails">
    <Value>Show emails</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.CustomerNameFormat.ShowUsernames">
    <Value>Show usernames</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.CustomerNameFormat.ShowFullNames">
    <Value>Show full names</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DefaultPasswordFormat">
    <Value>Default password format</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DefaultPasswordFormat.Hint">
    <Value>Choose default password format. Please keep in mind that this setting will be applied only to the newly registered customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.PasswordFormat.Clear">
    <Value>Clear</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.PasswordFormat.Hashed">
    <Value>Hashed</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.PasswordFormat.Encrypted">
    <Value>Encrypted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.LoadedAssemblies">
    <Value>Loaded assemblies</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.LoadedAssemblies.Hint">
    <Value>A list of loaded assemblies</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Hide">
    <Value>Hide</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Show">
    <Value>Show</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DirectoryPermission.OK">
    <Value>All directory permissions are OK</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DirectoryPermission.Wrong">
    <Value>The ''{0}'' account is not granted with Modify permission on folder ''{1}''. Please configure these permissions.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.FilePermission.OK">
    <Value>All file permissions are OK</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.FilePermission.Wrong">
    <Value>The ''{0}'' account is not granted with Modify permission on file ''{1}''. Please configure these permissions.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.FilePermission.Wrong">
    <Value>The ''{0}'' account is not granted with Modify permission on file ''{1}''. Please configure these permissions.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage">
    <Value>Show on blog page (comments)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage.Hint">
    <Value>Check to show CAPTCHA on blog page when writing a comment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage">
    <Value>Show on news page (comments)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage.Hint">
    <Value>Check to show CAPTCHA on news page when writing a comment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnProductReviewPage">
    <Value>Show on product reviews page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnProductReviewPage.Hint">
    <Value>Check to show CAPTCHA on product reviews page when writing a review.</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.ShippedDate.NotYet">
    <Value>Not yet</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShippedDate.NotYet">
    <Value>Not yet</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShippedDate.Button">
    <Value>Set as shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products">
    <Value>Products in shipment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StartDate.Hint">
    <Value>The start date (shipment creation date) for the search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.EndDate.Hint">
    <Value>The end date (shipment creation date) for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreTierPrices">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreTierPrices>Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Common.Error">
    <Value>Error</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Notification">
    <Value>Notification</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Warning">
    <Value>Warning</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.TermsOfService">
    <Value>Terms of service</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.EditDetails">
    <Value>Edit plugin details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.FriendlyName.Hint">
    <Value>The friendly name of the plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.DisplayOrder.Hint">
    <Value>The display order of the plugin. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.FriendlyName.Required">
    <Value>Friendly name is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Reviews.Helpfulness.YourOwnReview">
    <Value>You cannot vote for your own review</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Delete.Selected">
    <Value>Delete (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Title">
    <Value>Your cookie settings</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Description">
    <Value>On 26 May 2011, the rules about cookies on websites changed. This site uses cookies. One or more of the cookies we use is essential for parts of this website to operate and has already been set. You may delete and block all cookies from this site, but parts of the site will not work. To find out more about cookies used on this website and how to delete cookies, see our privacy policy</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.OK">
    <Value>OK</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Cancel">
    <Value>Cancel</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.CannotBrowse">
    <Value>You cannot browse this site until you accept cookies</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Hide">
    <Value>Hide</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Show">
    <Value>Show</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning">
    <Value>Display EU cookie law warning</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning.Hint">
    <Value>Make the site EU cookie law compliant. When enabled, new customers will see an appropriate warning box.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.Fields.Name.Hint">
    <Value>The name of the product tag.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductTags.EditTagDetails">
    <Value>Edit product tag details</Value>
  </LocaleResource>
  <LocaleResource Name="ContactUs.EmailSubject">
    <Value>{0}. Contact us</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Description">
    <Value>To find text or a specific resource (by name), you can apply a filter via the funnel icon in the "Value" or "Resource name" column headers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Description">
    <Value>To find text or a specific setting (by name), you can apply a filter via the funnel icon in the "Value" or "Setting name" column headers.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Country.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.StateProvince.Hint">
    <Value>If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartDisplayProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartDisplayProducts.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartProductNumber">
    <Value>Mini-shopping cart product number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartProductNumber.Hint">
    <Value>Specify the maximum number of products which can be displayed in the mini-shopping cart block.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.ShowProductImagesInMiniShoppingCart">
    <Value>Show product images in mini-shopping cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.ShowProductImagesInMiniShoppingCart.Hint">
    <Value>Determines whether product images should be displayed in the mini-shopping cart block.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.MiniCartThumbPictureSize">
    <Value>Mini-shopping cart thumbnail image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.MiniCartThumbPictureSize.Hint">
    <Value>The default size (pixels) for product thumbnail images in the mini-shopping cart block.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.ViewCart">
    <Value>Go to cart</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.Quantity">
    <Value>Quantity</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Mini.UnitPrice">
    <Value>Unit price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnShoppingCart.Hint">
    <Value>Determines whether product images should be displayed in your store shopping cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnWishList.Hint">
    <Value>Determines whether product images should be displayed on customer wishlists.</Value>
  </LocaleResource>
  <LocaleResource Name="Polls.Voted">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.Tooltip">
    <Value>Please enter your email address below. You will receive a link to reset your password.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteEnabled">
    <Value>Search autocomplete enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteEnabled.Hint">
    <Value>Check to enabled autocomplete in the search box.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteNumberOfProducts">
    <Value>Number of ''autocomplete'' products to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteNumberOfProducts.Hint">
    <Value>Change number of visible results shown in autocomplete dropdown when searching.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.FileUploaded">
    <Value>Your file successfully uploaded!</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInCompareProducts">
    <Value>Include short description in compare products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInCompareProducts.Hint">
    <Value>Check to display product short description on the compare products page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInCompareProducts">
    <Value>Include full description in compare products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInCompareProducts.Hint">
    <Value>Check to display product full description on the compare products page.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Compare.ShortDescription">
    <Value>Short description</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Compare.FullDescription">
    <Value>Full description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowAnonymousUsersToEmailWishlist">
    <Value>Allow guests to email their wishlists</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowAnonymousUsersToEmailWishlist.Hint">
    <Value>Check to allow guests to email their wishlists to friends.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PageTitle">
    <Value>nopCommerce administration</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.PdfLogo.Hint">
    <Value>Image file that will be displayed in PDF order invoices. A small image is recommended.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowOutOfStockItemsToBeAddedToWishlist">
    <Value>Allow ''out of stock'' items to be added to wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowOutOfStockItemsToBeAddedToWishlist.Hint">
    <Value>Check to allow ''out of stock'' products to be added to wishlist.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CompanyRequired">
    <Value>''Company'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CompanyRequired.Hint">
    <Value>Check if ''Company'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StreetAddressRequired">
    <Value>''Street address'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StreetAddressRequired.Hint">
    <Value>Check if ''Street address'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StreetAddress2Required">
    <Value>''Street address 2'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StreetAddress2Required.Hint">
    <Value>Check if ''Street address 2'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ZipPostalCodeRequired">
    <Value>''Zip / postal code'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ZipPostalCodeRequired.Hint">
    <Value>Check if ''Zip / postal code'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CityRequired">
    <Value>''City'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CityRequired.Hint">
    <Value>Check if ''City'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneRequired">
    <Value>''Phone number'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PhoneRequired.Hint">
    <Value>Check if ''Phone number'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FaxRequired">
    <Value>''Fax number'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FaxRequired.Hint">
    <Value>Check if ''Fax number'' is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Company.Required">
    <Value>Company name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.StreetAddress.Required">
    <Value>Street address is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.StreetAddress2.Required">
    <Value>Street address 2 is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.ZipPostalCode.Required">
    <Value>Zip / postal code is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.City.Required">
    <Value>City is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Phone.Required">
    <Value>Phone is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Fax.Required">
    <Value>Fax is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Company.Required">
    <Value>Company name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.StreetAddress.Required">
    <Value>Street address is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.StreetAddress2.Required">
    <Value>Street address 2 is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.ZipPostalCode.Required">
    <Value>Zip / postal code is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.City.Required">
    <Value>City is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Phone.Required">
    <Value>Phone is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Fax.Required">
    <Value>Fax is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.FriendlyName">
    <Value>Friendly name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.SystemName">
    <Value>System name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Configure">
    <Value>Configure</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.WidgetZone">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.WidgetZone.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.PluginFriendlyName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.PluginFriendlyName.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.DisplayOrder.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AvailableWidgetPlugins">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AvailableWidgetPlugins.FriendlyName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AvailableWidgetPlugins.AddToZone">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AddNew">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets.WidgetZone">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets.PluginFriendlyName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Added">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Deleted">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Updated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.EditWidgetDetails">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeadHtmlTag">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterBodyStartHtmlTag">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeaderLinks">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeaderSelectors">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeaderMenu">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeContent">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeLeftSideColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterLeftSideColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeMainColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterMainColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeRightSideColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterRightSideColumn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterContent">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.Footer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeBodyEndHtmlTag">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileHeadHtmlTag">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileAfterBodyStartHtmlTag">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileHeaderLinks">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileBeforeContent">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileAfterContent">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileFooter">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.MobileBeforeBodyEndHtmlTag">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.ChooseZone">
    <Value>Widget zone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.ChooseZone.Hint">
    <Value>Choose widget zone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings">
    <Value>Full-Text settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Supported">
    <Value>Full-Text is supported by your database</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported">
    <Value>Full-Text is not supported by your database</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled">
    <Value>Full-Text support has been enabled successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled">
    <Value>Full-Text support has been disabled successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enable">
    <Value>Enable Full-Text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disable">
    <Value>Disable Full-Text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.CurrenlyEnabled">
    <Value>Full-Text is enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.CurrenlyDisabled">
    <Value>Full-Text is disabled</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Common.FulltextSearchMode.ExactMatch">
    <Value>Exact match (using CONTAINS with prefix_term)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Common.FulltextSearchMode.Or">
    <Value>Using CONTAINS and OR with prefix_term</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Common.FulltextSearchMode.And">
    <Value>Using CONTAINS and AND with prefix_term</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode">
    <Value>Search mode</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode.Hint">
    <Value>Choose Full-Text search mode</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.TotalWeight">
    <Value>Total weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.TotalWeight.Hint">
    <Value>Shipment total weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Weight">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Feeds">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Feeds.BackToList">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Feeds.Configure">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Feeds.Fields.FriendlyName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Feeds.Fields.SystemName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Clickatell.Fields.Enabled">
    <Value>Enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Clickatell.Fields.Enabled.Hint">
    <Value>Check to enable SMS provider</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Verizon.Fields.Enabled">
    <Value>Enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Sms.Verizon.Fields.Enabled.Hint">
    <Value>Check to enable SMS provider</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.YourAccountWillBeLinkedTo">
    <Value>Account Association: Your new user account will be linked to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed">
    <Value>Encryption key changed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort">
    <Value>Encryption private key must be 16 characters long</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame">
    <Value>The new encryption key is the same as the old one</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SSLSettings">
    <Value>SSL settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SSLSettings.Hint">
    <Value>SSL settings can also be changed in web.config file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.AllowGuestsToVote">
    <Value>Allow guests to vote</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.AllowGuestsToVote.Hint">
    <Value>Check to allow guests to vote.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.Fields.ID">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Upload">
    <Value>Upload a file</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.DropFiles">
    <Value>Drop files here to upload</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Cancel">
    <Value>Cancel</Value>
  </LocaleResource>
  <LocaleResource Name="Common.FileUploader.Failed">
    <Value>Failed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ManufacturersBlockItemsToDisplay">
    <Value>Number of manufacturers to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ManufacturersBlockItemsToDisplay.Hint">
    <Value>Enter the number of manufacturers to display in manufacturer navigation block.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Tags.Popular.ViewAll">
    <Value>View all</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.AllProductTags">
    <Value>All product tags</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Tags.All">
    <Value>All product tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderWeight">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ItemWeight">
    <Value>Item weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ItemDimensions">
    <Value>Item dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AllowedQuantities">
    <Value>Allowed quantities</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AllowedQuantities.Hint">
    <Value>Enter a comma separated list of quantities you want this product variant to be restricted to. Instead of a quantity textbox that allows them to enter any quantity, they will receive a dropdown list of the values you enter here.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.AllowedQuantities">
    <Value>Allowed quantities for this product: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAbandonedCarts">
    <Value>Deleting abandoned shopping carts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan">
    <Value>Created before</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan.Hint">
    <Value>Delete shopping cart items created before the specified date.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.DeleteAbandonedCarts.TotalDeleted">
    <Value>{0} items were deleted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductImagesInSearchAutoComplete">
    <Value>Show product images in autocomplete box</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductImagesInSearchAutoComplete.Hint">
    <Value>Determines whether product images should be displayed in the autocomplete search box.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ActivityLog.ActivityLog.Fields.CustomerEmail.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog">
    <Value>Activity Log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog.ActivityLogType">
    <Value>Activity Log Type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog.Comment">
    <Value>Comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog.CreatedOn">
    <Value>Created on</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.StateProvince.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Zip">
    <Value>Zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Zip.Hint">
    <Value>Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Order.PurchaseOrderNumber">
    <Value>Purchase Order Number</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.PurchaseOrderNumber">
    <Value>Purchase Order Number: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PlaceOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.PlaceOrder">
    <Value>Placed a new order (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.ViewCategory">
    <Value>Public store. Viewed a category details page (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.ViewManufacturer">
    <Value>Public store. Viewed a manufacturer details page (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.ViewProduct">
    <Value>Public store. Viewed a product details page (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="Payment.ExpireMonth.Required">
    <Value>Expire month is required</Value>
  </LocaleResource>
  <LocaleResource Name="Payment.ExpireYear.Required">
    <Value>Expire year is required</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Manufactures">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Manufacturers">
    <Value>Manufacturers</Value>
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




--Update stored procedure according to the new search parameters (return filterable specs)
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductLoadAllPaged]
GO
CREATE PROCEDURE [ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
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
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		SET @Keywords = isnull(@Keywords, '')
		SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'
		
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.sku) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
			
		IF @SearchDescriptions = 1 SET @sql = @sql + '
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.ShortDescription) > 0
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.FullDescription) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.Description) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''ShortDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''FullDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
		
		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(MAX)', @Keywords

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

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')	
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

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
	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	IF @ShowHidden = 0
	OR @PriceMin > 0
	OR @PriceMax > 0
	OR @OrderBy = 10 /* Price: Low to High */
	OR @OrderBy = 11 /* Price: High to Low */
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN ProductVariant pv with (NOLOCK)
			ON p.Id = pv.ProductId'
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
		AND pcm.CategoryId IN (SELECT CategoryId FROM #FilteredCategoryIds)'
		
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
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1
		AND pv.Published = 1
		AND pv.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(pv.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(pv.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 
			FROM
				#FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM Product_SpecificationAttribute_Mapping psam
					WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
				)
			)'
	END
	
	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' pv.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' pv.[Price] DESC'
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
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs

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

	--prepare filterable specification attribute option identifier (if requested)
	IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
		INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
		SELECT DISTINCT [psam].SpecificationAttributeOptionId
		FROM [Product_SpecificationAttribute_Mapping] [psam]
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(1000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[nop_splitstring_to_table]') AND [type] in (N'FN', N'IF', N'TF'))
DROP FUNCTION [nop_splitstring_to_table]
GO
CREATE FUNCTION [dbo].[nop_splitstring_to_table]
(
    @string NVARCHAR(MAX),
    @delimiter CHAR(1)
)
RETURNS @output TABLE(
    data NVARCHAR(MAX)
)
BEGIN
    DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string)

    WHILE @start < LEN(@string) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@string) + 1

        INSERT INTO @output (data) 
        VALUES(SUBSTRING(@string, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END
    RETURN
END
GO


--Add 'StartDateUtc' and 'EndDateUtc' columns to [News] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='StartDateUtc')
BEGIN
	ALTER TABLE [News]
	ADD [StartDateUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='EndDateUtc')
BEGIN
	ALTER TABLE [News]
	ADD [EndDateUtc] datetime NULL
END
GO
--Add 'StartDateUtc' and 'EndDateUtc' columns to [BlogPost] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='StartDateUtc')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [StartDateUtc] datetime NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='EndDateUtc')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [EndDateUtc] datetime NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.defaultpasswordformat')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.defaultpasswordformat', N'Hashed')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.displayjavascriptdisabledwarning')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'commonsettings.displayjavascriptdisabledwarning', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.showonblogcommentpage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'captchasettings.showonblogcommentpage', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.showonnewscommentpage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'captchasettings.showonnewscommentpage', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.showonproductreviewpage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'captchasettings.showonproductreviewpage', N'false')
END
GO

--Add 'CreatedOnUtc' column to [Shipment] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Shipment]') and NAME='CreatedOnUtc')
BEGIN
	ALTER TABLE [Shipment]
	ADD [CreatedOnUtc] datetime NULL

	exec('UPDATE [Shipment] SET [CreatedOnUtc] = [ShippedDateUtc]')
END
GO

ALTER TABLE [Shipment] ALTER COLUMN [CreatedOnUtc] datetime NOT NULL
GO

--Created shipments should not be immediately shipped
ALTER TABLE [Shipment] ALTER COLUMN [ShippedDateUtc] datetime NULL
GO


--Store comment count in [News] entity/table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='ApprovedCommentCount')
BEGIN
	ALTER TABLE [News]
	ADD [ApprovedCommentCount] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='NotApprovedCommentCount')
BEGIN
	ALTER TABLE [News]
	ADD [NotApprovedCommentCount] int NULL
END
GO

EXEC('
	DECLARE @NewsId int
	DECLARE cur_news CURSOR FOR
	SELECT [Id]
	FROM [News]
	ORDER BY [Id]
	OPEN cur_news
	FETCH NEXT FROM cur_news INTO @NewsId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @ApprovedCommentCount int
		DECLARE @NotApprovedCommentCount int
		SET @ApprovedCommentCount = null -- clear cache (variable scope)
		SET @NotApprovedCommentCount = null -- clear cache (variable scope)


		SELECT @ApprovedCommentCount = COUNT(1) FROM [NewsComment] as [nc]
		INNER JOIN [CustomerContent] AS [cc] ON [nc].[Id] = [cc].[Id]
		WHERE [nc].[NewsItemId] = @NewsId AND [cc].[IsApproved]=1

		SELECT @NotApprovedCommentCount = COUNT(1) FROM [NewsComment] as [nc]
		INNER JOIN [CustomerContent] AS [cc] ON [nc].[Id] = [cc].[Id]
		WHERE [nc].[NewsItemId] = @NewsId AND [cc].[IsApproved]=0
	
		UPDATE [News]
		SET [ApprovedCommentCount] = @ApprovedCommentCount,
		[NotApprovedCommentCount] = @NotApprovedCommentCount
		WHERE [Id] = @NewsId
	
		--fetch next identifier
		FETCH NEXT FROM cur_news INTO @NewsId
	END
	CLOSE cur_news
	DEALLOCATE cur_news
	')
GO


ALTER TABLE [News] ALTER COLUMN [ApprovedCommentCount] int NOT NULL
GO
ALTER TABLE [News] ALTER COLUMN [NotApprovedCommentCount] int NOT NULL
GO



--Store comment count in [BlogPost] entity/table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='ApprovedCommentCount')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [ApprovedCommentCount] int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='NotApprovedCommentCount')
BEGIN
	ALTER TABLE [BlogPost]
	ADD [NotApprovedCommentCount] int NULL
END
GO

EXEC('
	DECLARE @BlogPostId int
	DECLARE cur_blogpost CURSOR FOR
	SELECT [Id]
	FROM [BlogPost]
	ORDER BY [Id]
	OPEN cur_blogpost
	FETCH NEXT FROM cur_blogpost INTO @BlogPostId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @ApprovedCommentCount int
		DECLARE @NotApprovedCommentCount int
		SET @ApprovedCommentCount = null -- clear cache (variable scope)
		SET @NotApprovedCommentCount = null -- clear cache (variable scope)


		SELECT @ApprovedCommentCount = COUNT(1) FROM [BlogComment] as [nc]
		INNER JOIN [CustomerContent] AS [cc] ON [nc].[Id] = [cc].[Id]
		WHERE [nc].[BlogPostId] = @BlogPostId AND [cc].[IsApproved]=1

		SELECT @NotApprovedCommentCount = COUNT(1) FROM [BlogComment] as [nc]
		INNER JOIN [CustomerContent] AS [cc] ON [nc].[Id] = [cc].[Id]
		WHERE [nc].[BlogPostId] = @BlogPostId AND [cc].[IsApproved]=0
	
		UPDATE [BlogPost]
		SET [ApprovedCommentCount] = @ApprovedCommentCount,
		[NotApprovedCommentCount] = @NotApprovedCommentCount
		WHERE [Id] = @BlogPostId
	
		--fetch next identifier
		FETCH NEXT FROM cur_blogpost INTO @BlogPostId
	END
	CLOSE cur_blogpost
	DEALLOCATE cur_blogpost
	')
GO


ALTER TABLE [BlogPost] ALTER COLUMN [ApprovedCommentCount] int NOT NULL
GO
ALTER TABLE [BlogPost] ALTER COLUMN [NotApprovedCommentCount] int NOT NULL
GO


--Store a value indicating whether we have tier prices in [ProductVariant] entity/table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariant]') and NAME='HasTierPrices')
BEGIN
	ALTER TABLE [ProductVariant]
	ADD [HasTierPrices] bit NULL
END
GO

EXEC('
	DECLARE @ProductVariantId int
	DECLARE cur_productvariant CURSOR FOR
	SELECT [Id]
	FROM [ProductVariant]
	ORDER BY [Id]
	OPEN cur_productvariant
	FETCH NEXT FROM cur_productvariant INTO @ProductVariantId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @HasTierPrices bit
		SET @HasTierPrices = null -- clear cache (variable scope)


		IF (EXISTS (SELECT 1 FROM [TierPrice] as [tp] WHERE [tp].[ProductVariantId] = @ProductVariantId))
		BEGIN
		SET @HasTierPrices = 1
		END
		ELSE
		BEGIN
		SET @HasTierPrices = 0
		END

		UPDATE [ProductVariant]
		SET [HasTierPrices] = @HasTierPrices
		WHERE [Id] = @ProductVariantId
	
		--fetch next identifier
		FETCH NEXT FROM cur_productvariant INTO @ProductVariantId
	END
	CLOSE cur_productvariant
	DEALLOCATE cur_productvariant
	')
GO


ALTER TABLE [ProductVariant] ALTER COLUMN [HasTierPrices] bit NOT NULL
GO

DELETE FROM [Setting]
WHERE [Name] = N'catalogsettings.ignoretierprices'
GO





--Store a value indicating whether we have discounts applied in [ProductVariant] entity/table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariant]') and NAME='HasDiscountsApplied')
BEGIN
	ALTER TABLE [ProductVariant]
	ADD [HasDiscountsApplied] bit NULL
END
GO

EXEC('
	DECLARE @ProductVariantId int
	DECLARE cur_productvariant CURSOR FOR
	SELECT [Id]
	FROM [ProductVariant]
	ORDER BY [Id]
	OPEN cur_productvariant
	FETCH NEXT FROM cur_productvariant INTO @ProductVariantId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @HasDiscountsApplied bit
		SET @HasDiscountsApplied = null -- clear cache (variable scope)


		IF (EXISTS (SELECT 1 FROM [Discount_AppliedToProductVariants] as [datpv] WHERE [datpv].[ProductVariant_Id] = @ProductVariantId))
		BEGIN
		SET @HasDiscountsApplied = 1
		END
		ELSE
		BEGIN
		SET @HasDiscountsApplied = 0
		END

		UPDATE [ProductVariant]
		SET [HasDiscountsApplied] = @HasDiscountsApplied
		WHERE [Id] = @ProductVariantId
	
		--fetch next identifier
		FETCH NEXT FROM cur_productvariant INTO @ProductVariantId
	END
	CLOSE cur_productvariant
	DEALLOCATE cur_productvariant
	')
GO


ALTER TABLE [ProductVariant] ALTER COLUMN [HasDiscountsApplied] bit NOT NULL
GO


--Store a value indicating whether we have discounts applied in [ProductVariant] entity/table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariant]') and NAME='HasDiscountsApplied')
BEGIN
	ALTER TABLE [ProductVariant]
	ADD [HasDiscountsApplied] bit NULL
END
GO

EXEC('
	DECLARE @ProductVariantId int
	DECLARE cur_productvariant CURSOR FOR
	SELECT [Id]
	FROM [ProductVariant]
	ORDER BY [Id]
	OPEN cur_productvariant
	FETCH NEXT FROM cur_productvariant INTO @ProductVariantId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @HasDiscountsApplied bit
		SET @HasDiscountsApplied = null -- clear cache (variable scope)


		IF (EXISTS (SELECT 1 FROM [Discount_AppliedToProductVariants] as [datpv] WHERE [datpv].[ProductVariant_Id] = @ProductVariantId))
		BEGIN
		SET @HasDiscountsApplied = 1
		END
		ELSE
		BEGIN
		SET @HasDiscountsApplied = 0
		END

		UPDATE [ProductVariant]
		SET [HasDiscountsApplied] = @HasDiscountsApplied
		WHERE [Id] = @ProductVariantId
	
		--fetch next identifier
		FETCH NEXT FROM cur_productvariant INTO @ProductVariantId
	END
	CLOSE cur_productvariant
	DEALLOCATE cur_productvariant
	')
GO

--Store a value indicating whether we have discounts applied in [Category] entity/table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Category]') and NAME='HasDiscountsApplied')
BEGIN
	ALTER TABLE [Category]
	ADD [HasDiscountsApplied] bit NULL
END
GO

EXEC('
	DECLARE @CategoryId int
	DECLARE cur_category CURSOR FOR
	SELECT [Id]
	FROM [Category]
	ORDER BY [Id]
	OPEN cur_category
	FETCH NEXT FROM cur_category INTO @CategoryId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @HasDiscountsApplied bit
		SET @HasDiscountsApplied = null -- clear cache (variable scope)


		IF (EXISTS (SELECT 1 FROM [Discount_AppliedToCategories] as [datc] WHERE [datc].[Category_Id] = @CategoryId))
		BEGIN
		SET @HasDiscountsApplied = 1
		END
		ELSE
		BEGIN
		SET @HasDiscountsApplied = 0
		END

		UPDATE [Category]
		SET [HasDiscountsApplied] = @HasDiscountsApplied
		WHERE [Id] = @CategoryId
	
		--fetch next identifier
		FETCH NEXT FROM cur_category INTO @CategoryId
	END
	CLOSE cur_category
	DEALLOCATE cur_category
	')
GO
ALTER TABLE [Category] ALTER COLUMN [HasDiscountsApplied] bit NOT NULL
GO



IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.defaultimagequality')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'mediasettings.defaultimagequality', N'100')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.displayeucookielawwarning')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'storeinformationsettings.displayeucookielawwarning', N'false')
END
GO


--Tax By Country & State & Zip provider issue fix
UPDATE [TaxRate]
SET [Zip] = null
WHERE [Zip] = N'*'
GO

--new generic attribute implementation
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[GenericAttribute]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[GenericAttribute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[KeyGroup] nvarchar(400) NOT NULL,
	[Key] nvarchar(400) NOT NULL,
	[Value] nvarchar(MAX) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerAttribute]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--move customer attributes to the new generic attributes
	EXEC('
	DECLARE @CustomerAttributeId int
	DECLARE cur_customerattribute CURSOR FOR
	SELECT [Id]
	FROM [CustomerAttribute]
	ORDER BY [Id]
	OPEN cur_customerattribute
	FETCH NEXT FROM cur_customerattribute INTO @CustomerAttributeId
	WHILE @@FETCH_STATUS = 0
	BEGIN

		DECLARE @AttributeCustomerId int
		DECLARE @AttributeKey nvarchar(MAX)
		DECLARE @AttributeValue nvarchar(MAX)
		SET @AttributeCustomerId = null -- clear cache (variable scope)
		SET @AttributeKey = null -- clear cache (variable scope)
		SET @AttributeValue = null -- clear cache (variable scope)
		SELECT @AttributeCustomerId = [CustomerId],
		@AttributeKey = [Key],
		@AttributeValue = [Value]
		FROM [CustomerAttribute] WHERE [Id]=@CustomerAttributeId

		--insert new generic attribute
		INSERT INTO [GenericAttribute] ([EntityId], [KeyGroup], [Key], [Value])
		VALUES (@AttributeCustomerId, N''Customer'', @AttributeKey, @AttributeValue)	
			
		--fetch next identifier
		FETCH NEXT FROM cur_customerattribute INTO @CustomerAttributeId
	END
	CLOSE cur_customerattribute
	DEALLOCATE cur_customerattribute
	')

	DROP TABLE [CustomerAttribute]
END
GO


--more SQL indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_GenericAttribute_EntityId_and_KeyGroup' and object_id=object_id(N'[GenericAttribute]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] 
	(
		[EntityId] ASC,
		[KeyGroup] ASC
	)
END
GO

DELETE FROM [Setting]
WHERE [Name] = N'shoppingcartsettings.minishoppingcartdisplayproducts'
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.minishoppingcartproductnumber')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.minishoppingcartproductnumber', N'5')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.showproductimagesinminishoppingcart')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.showproductimagesinminishoppingcart', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.minicartthumbpicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'mediasettings.minicartthumbpicturesize', N'47')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsearchautocompleteenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.productsearchautocompleteenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsearchautocompletenumberofproducts')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.productsearchautocompletenumberofproducts', N'10')
END
GO


--new 'permission records
IF NOT EXISTS (
		SELECT 1
		FROM [PermissionRecord]
		WHERE [SystemName] = N'PublicStoreAllowNavigation')
BEGIN
	INSERT [PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Public store. Allow navigation', N'PublicStoreAllowNavigation', N'PublicStore')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to all roles
	DECLARE @CustomerRoleId int
	DECLARE cur_customerrole CURSOR FOR
	SELECT Id
	FROM [CustomerRole]
	OPEN cur_customerrole
	FETCH NEXT FROM cur_customerrole INTO @CustomerRoleId
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		INSERT [PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
		VALUES (@PermissionRecordId, @CustomerRoleId)
		
		FETCH NEXT FROM cur_customerrole INTO @CustomerRoleId
	END
	CLOSE cur_customerrole
	DEALLOCATE cur_customerrole
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.includeshortdescriptionincompareproducts')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.includeshortdescriptionincompareproducts', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.includefulldescriptionincompareproducts')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.includefulldescriptionincompareproducts', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.allowanonymoususerstoemailwishlist')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.allowanonymoususerstoemailwishlist', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.allowoutofstockitemstobeaddedtowishlist')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.allowoutofstockitemstobeaddedtowishlist', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.companyrequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.companyrequired', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.streetaddressrequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.streetaddressrequired', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.streetaddress2required')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.streetaddress2required', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.zippostalcoderequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.zippostalcoderequired', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.cityrequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.cityrequired', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.phonerequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.phonerequired', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.faxrequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.faxrequired', N'false')
END
GO

--new widget implementation
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Widget]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	DROP TABLE [Widget]
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'widgetsettings.activewidgetsystemnames')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'widgetsettings.activewidgetsystemnames', N'')
END
GO

--Full-text support
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.usefulltextsearch')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'commonsettings.usefulltextsearch', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.fulltextsearchmode')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'commonsettings.fulltextsearchmode', N'ExactMatch')
END
GO

IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductLoadAllPaged]
GO
CREATE PROCEDURE [ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0,
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
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
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
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

				--remove wrong chars (' ")
				SET @Keywords = REPLACE(@Keywords, '''', '')
				SET @Keywords = REPLACE(@Keywords, '"', '')
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


		--product variant name
		SET @sql = @sql + '
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(pv.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Name]) > 0 '


		--SKU
		SET @sql = @sql + '
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(pv.[Sku], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Sku]) > 0 '


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
	

		--product short description
		IF @SearchDescriptions = 1
		BEGIN
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


			--product variant description
			SET @sql = @sql + '
			UNION
			SELECT pv.ProductId
			FROM ProductVariant pv with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(pv.[Description], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Description]) > 0 '


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

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000)', @Keywords

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

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')	
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

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
	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	IF @ShowHidden = 0
	OR @PriceMin > 0
	OR @PriceMax > 0
	OR @OrderBy = 10 /* Price: Low to High */
	OR @OrderBy = 11 /* Price: High to Low */
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN ProductVariant pv with (NOLOCK)
			ON p.Id = pv.ProductId'
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
		AND pcm.CategoryId IN (SELECT CategoryId FROM #FilteredCategoryIds)'
		
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
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1
		AND pv.Published = 1
		AND pv.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(pv.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(pv.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 
			FROM
				#FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM Product_SpecificationAttribute_Mapping psam
					WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
				)
			)'
	END
	
	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' pv.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' pv.[Price] DESC'
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
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs

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

	--prepare filterable specification attribute option identifier (if requested)
	IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
		INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
		SELECT DISTINCT [psam].SpecificationAttributeOptionId
		FROM [Product_SpecificationAttribute_Mapping] [psam]
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(1000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO


IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[nop_getprimarykey_indexname]') AND [type] in (N'FN', N'IF', N'TF'))
DROP FUNCTION  [nop_getprimarykey_indexname]
GO
CREATE FUNCTION [dbo].[nop_getprimarykey_indexname]
(
    @table_name nvarchar(1000) = null
)
RETURNS nvarchar(1000)
AS
BEGIN
	DECLARE @index_name nvarchar(1000)

    SELECT @index_name = i.name
	FROM sys.tables AS tbl
	INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id)
	WHERE (i.is_unique=1 and i.is_disabled=0) and (tbl.name=@table_name)

    RETURN @index_name
END
GO

IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[FullText_IsSupported]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [FullText_IsSupported]
GO
CREATE PROCEDURE [FullText_IsSupported]
AS
BEGIN	
	EXEC('
	SELECT CASE SERVERPROPERTY(''IsFullTextInstalled'')
	WHEN 1 THEN 
		CASE DatabaseProperty (DB_NAME(DB_ID()), ''IsFulltextEnabled'')
		WHEN 1 THEN 1
		ELSE 0
		END
	ELSE 0
	END')
END
GO

IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[FullText_Enable]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [FullText_Enable]
GO
CREATE PROCEDURE [FullText_Enable]
AS
BEGIN
	--create catalog
	EXEC('
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		CREATE FULLTEXT CATALOG [nopCommerceFullTextCatalog] AS DEFAULT')
	
	--create indexes
	DECLARE @create_index_text nvarchar(4000)
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		CREATE FULLTEXT INDEX ON [Product]([Name], [ShortDescription], [FullDescription])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('Product') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
	
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductVariant]''))
		CREATE FULLTEXT INDEX ON [ProductVariant]([Name], [Description], [SKU])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('ProductVariant') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		CREATE FULLTEXT INDEX ON [LocalizedProperty]([LocaleValue])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('LocalizedProperty') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
END
GO

IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[FullText_Disable]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [FullText_Disable]
GO
CREATE PROCEDURE [FullText_Disable]
AS
BEGIN
	EXEC('
	--drop indexes
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		DROP FULLTEXT INDEX ON [Product]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductVariant]''))
		DROP FULLTEXT INDEX ON [ProductVariant]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		DROP FULLTEXT INDEX ON [LocalizedProperty]
	')

	--drop catalog
	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		DROP FULLTEXT CATALOG [nopCommerceFullTextCatalog]
	')
END
GO








--language pack import
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[LanguagePackImport]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [LanguagePackImport]
GO
CREATE PROCEDURE [LanguagePackImport]
(
	@LanguageId int,
	@XmlPackage xml
)
AS
BEGIN
	IF EXISTS(SELECT * FROM [Language] WHERE [Id] = @LanguageId)
	BEGIN
		CREATE TABLE #LocaleStringResourceTmp
			(
				[LanguageId] [int] NOT NULL,
				[ResourceName] [nvarchar](200) NOT NULL,
				[ResourceValue] [nvarchar](MAX) NOT NULL
			)

		INSERT INTO #LocaleStringResourceTmp (LanguageID, ResourceName, ResourceValue)
		SELECT	@LanguageId, nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/LocaleResource') AS R(nref)

		DECLARE @ResourceName nvarchar(200)
		DECLARE @ResourceValue nvarchar(MAX)
		DECLARE cur_localeresource CURSOR FOR
		SELECT LanguageID, ResourceName, ResourceValue
		FROM #LocaleStringResourceTmp
		OPEN cur_localeresource
		FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@LanguageId AND ResourceName=@ResourceName))
			BEGIN
				UPDATE [LocaleStringResource]
				SET [ResourceValue]=@ResourceValue
				WHERE LanguageID=@LanguageId AND ResourceName=@ResourceName
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
					@LanguageId,
					@ResourceName,
					@ResourceValue
				)
			END
			
			
			FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
			END
		CLOSE cur_localeresource
		DEALLOCATE cur_localeresource

		DROP TABLE #LocaleStringResourceTmp
	END
END
GO


--Add 'Weight' column to [Shipment] and [OrderProductVariant] tables
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Shipment]') and NAME='TotalWeight')
BEGIN
	ALTER TABLE [Shipment]
	ADD [TotalWeight] decimal(18, 4) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[OrderProductVariant]') and NAME='ItemWeight')
BEGIN
	ALTER TABLE [OrderProductVariant]
	ADD [ItemWeight] decimal(18, 4) NULL
END
GO

--delete obsolete permission
DELETE FROM [PermissionRecord]
WHERE [SystemName] = N'ManagePromotionFeeds'
GO
DELETE FROM [Setting]
WHERE [Name] = N'smssettings.activesmsprovidersystemnames'
GO
DELETE FROM [PermissionRecord]
WHERE [SystemName] = N'ManageSMSProviders'
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'clickatellsettings.enabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'clickatellsettings.enabled', N'false')
END
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'verizonsettings.enabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'verizonsettings.enabled', N'false')
END
GO


--Add 'AllowGuestsToVote' column to [Poll] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Poll]') and NAME='AllowGuestsToVote')
BEGIN
	ALTER TABLE [Poll]
	ADD [AllowGuestsToVote] bit NULL
END
GO

UPDATE [Poll]
SET [AllowGuestsToVote] = 0
WHERE [AllowGuestsToVote] IS NULL
GO

ALTER TABLE [Poll] ALTER COLUMN [AllowGuestsToVote] bit NOT NULL
GO

--'Recurring payment cancelled' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'RecurringPaymentCancelled.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId])
	VALUES (N'RecurringPaymentCancelled.StoreOwnerNotification', null, N'%Store.Name%. Recurring payment cancelled', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%.</p>', 1, 0)
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.manufacturersblockitemstodisplay')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.manufacturersblockitemstodisplay', N'5')
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='OrderWeight')
BEGIN
	ALTER TABLE [Order]
	DROP COLUMN [OrderWeight]
END
GO


--Add 'AllowedQuantities' column to [ProductVariant] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariant]') and NAME='AllowedQuantities')
BEGIN
	ALTER TABLE [ProductVariant]
	ADD [AllowedQuantities] nvarchar(1000) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.autocompletesearchthumbpicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'mediasettings.autocompletesearchthumbpicturesize', N'20')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showproductimagesinsearchautocomplete')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.showproductimagesinsearchautocomplete', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.passshippinginfo')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'frooglesettings.passshippinginfo', N'false')
END
GO

IF (EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1))
	AND (NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShippingByWeight]') and NAME='StateProvinceId'))
BEGIN
	EXEC ('ALTER TABLE [ShippingByWeight]
	ADD [StateProvinceId] int NULL')
END
GO

IF (EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1))
	AND (EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShippingByWeight]') and NAME='StateProvinceId'))
BEGIN
	EXEC ('UPDATE [ShippingByWeight]
	SET [StateProvinceId] = 0
	WHERE [StateProvinceId] IS NULL')
END
GO

IF (EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1))
	AND (EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShippingByWeight]') and NAME='StateProvinceId'))
BEGIN
	EXEC('ALTER TABLE [ShippingByWeight] ALTER COLUMN [StateProvinceId] int NOT NULL')
END
GO

IF (EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1))
	AND (NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShippingByWeight]') and NAME='Zip'))
BEGIN
	EXEC ('ALTER TABLE [ShippingByWeight]
	ADD [Zip] nvarchar(400) NULL')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.fileuploadallowedextensions')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.fileuploadallowedextensions', N'')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.ViewCategory')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.ViewCategory', N'Public store. View a category', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.ViewManufacturer')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.ViewManufacturer', N'Public store. View a manufacturer', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.ViewProduct')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.ViewProduct', N'Public store. View a product', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.PlaceOrder')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.PlaceOrder', N'Public store. Place an order', N'false')
END
GO
--upgrade scripts from nopCommerce 1.80 to nopCommerce 1.90

--new locale resources
declare @resources xml
set @resources='
<Language>
  <LocaleResource Name="Admin.OnlineCustomers.Registered">
    <Value>Members:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Registered.Tooltip">
    <Value>See how many registered customers you got on your site in this moment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Total">
    <Value>Total:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Total.Tooltip">
    <Value>See how many customers (total) you got on your site in this moment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OnlineCustomers.Total.Value">
    <Value>{0} (maximum: {1})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.SearchPageProductsPerPage">
    <Value>Search page. Products per page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.SearchPageProductsPerPage.Tooltip">
    <Value>Set the page size for products on ''Search'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.SearchPageProductsPerPage.RequiredErrorMessage">
    <Value>Number of ''Search page. Products per page'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.SearchPageProductsPerPage.RangeErrorMessage">
    <Value>The number of ''Search page. Products per page'' must be from 1 to 999999.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DisplayPageExecutionTime">
    <Value>Display page execution time:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.DisplayPageExecutionTime.Tooltip">
    <Value>Display page execution time at the bottom of all pages in public store (this option should be disabled in production environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowManufacturerPartNumber">
    <Value>Show manufacturer part number:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowManufacturerPartNumber.Tooltip">
    <Value>Check to show manufacturer part numbers in public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ManufacturerPartNumber">
    <Value>Part number:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowCustomersToChangeUsernames">
    <Value>Allow customers to change their usernames:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowCustomersToChangeUsernames.Tooltip">
    <Value>A value indicating whether customers are allowed to change their usernames.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.Username.Tooltip">
    <Value>The username of the customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Username">
    <Value>Username</Value>
  </LocaleResource>
  <LocaleResource Name="Account.UsernameRequired">
    <Value>Username is required</Value>
  </LocaleResource>
  <LocaleResource Name="VAT.EnteredWithoutCountryCode">
    <Value>NOTE: Enter VAT number without country code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.VatNumber.Tooltip">
    <Value>Enter company VAT number (NOTE: Enter VAT number without country code)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.ProductVariants">
    <Value>Product variants</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.ProductVariants.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.ProductVariants.Unnamed">
    <Value>Unnamed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.ProductVariants.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.ProductVariants.Published">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.ProductVariants.SKU">
    <Value>SKU</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Products.Image">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Profiles.NotifyAboutPrivateMessages">
    <Value>Notify about new private messages:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Profiles.NotifyAboutPrivateMessages.Tooltip">
    <Value>Notify customers when a new private message is sent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.NotifyAboutNewCustomerRegistration">
    <Value>Notify about new customer registration:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.NotifyAboutNewCustomerRegistration.Tooltip">
    <Value>Notify the store owner when a new customer is registered.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.Warnings.TitleDescription">
    <Value>System warnings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.Warnings.Title">
    <Value>Warnings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.Warnings.Description">
    <Value>A self-diagnostics page that runs through the store settings and confirms all of them are correct,</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.WarningsTitle">
    <Value>Warnings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.WarningsDescription">
    <Value>Warnings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ProductCanonicalURL">
    <Value>enable canonical URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.CategoryCanonicalURL">
    <Value>enable canonical URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ManufacturerCanonicalURL">
    <Value>enable canonical URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxShoppingCartItems">
    <Value>Maximum shopping cart items:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxShoppingCartItems.Tooltip">
    <Value>Maximum number of distinct products allowed in a shopping cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxShoppingCartItems.RequiredErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxShoppingCartItems.RangeErrorMessage">
    <Value>Enter a maximum number of distinct products allowed in a shopping cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxWishlistItems">
    <Value>Maximum wishlist items:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxWishlistItems.Tooltip">
    <Value>Maximum number of distinct products allowed in a wishlist.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxWishlistItems.RequiredErrorMessage">
    <Value>The value must be from 1 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.MaxWishlistItems.RangeErrorMessage">
    <Value>Enter a maximum number of distinct products allowed in a wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowCategoryProductNumber">
    <Value>Show the number of distinct products besides each category:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.ShowCategoryProductNumber.Tooltip">
    <Value>Check to show the number of products besides each category (category navigation block)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToVotePolls">
    <Value>Allow anonymous users to vote on polls:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.AllowAnonymousUsersToVotePolls.Tooltip">
    <Value>Check to allow anonymous users to vote on polls.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.StoreImagesInDB">
    <Value>Pictures are stored into... :</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.StoreImagesInDB.Tooltip">
    <Value>A value indicating whether pictures are stored in database or file system.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.StoreImagesInDB.DB">
    <Value>database</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.StoreImagesInDB.FS">
    <Value>file system</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Media.StoreImagesInDB.Change">
    <Value>Change</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SubtotalDiscountInclTax">
    <Value>Order subtotal discount (incl tax):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SubtotalDiscountInclTax.Tooltip">
    <Value>The subtotal discount of this order (including tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SubtotalDiscountExclTax">
    <Value>Order subtotal discount (excl tax):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.SubtotalDiscountExclTax.Tooltip">
    <Value>The subtotal discount of this order (excluding tax).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.SubtotalDiscount.InPrimaryCurrency">
    <Value>Subtotal discount in primary currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.OrderDetails.EditOrderTotals.SubtotalDiscount.InCustomerCurrency">
    <Value>Subtotal discount in customer currency - {0}:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.General.StoreClosedForAdmins">
    <Value>Allow an admin to view the closed store:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.General.StoreClosedForAdmins.Tooltip">
    <Value>Check to allow a user with admin access to view the store while it is set to closed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DownloadColumn">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DownloadButton.Text">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.AllowCustomerSelectTheme">
    <Value>Allow customers to select a theme:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.AllowCustomerSelectTheme.Tooltip">
    <Value>Check to allow customers to select a store theme.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.SelectStoreTheme">
    <Value>Select store theme:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.NotifyCustomerButton.Text">
    <Value>Notify customer about status change</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.NotifyCustomerButton.Tooltip">
    <Value>Click to notify the customer about status change of this return request (save return request first)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount.RequiredErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderAmount.RangeErrorMessage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.MinOrderAmount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderTotalAmount">
    <Value>Min order total amount:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderTotalAmount.Tooltip">
    <Value>Enter minimum order total amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderTotalAmount.RequiredErrorMessage">
    <Value>Minimum order total amount is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderTotalAmount.RangeErrorMessage">
    <Value>The minimum total order amount must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.MinOrderTotalAmount">
    <Value>Minimum order total amount is {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderSubtotalAmount">
    <Value>Min order sub-total amount:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderSubtotalAmount.Tooltip">
    <Value>Enter minimum order sub-total amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderSubtotalAmount.RequiredErrorMessage">
    <Value>Minimum order sub-total amount is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Other.MinOrderSubtotalAmount.RangeErrorMessage">
    <Value>The minimum sub-total order amount must be from 0 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.MinOrderSubtotalAmount">
    <Value>Minimum order sub-total amount is {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.AdvancedSearch">
    <Value>Advanced search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerInfo.PasswordChanged">
    <Value>Password successfully changed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.SelectLanguage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.Language.Tooltip">
    <Value>Filter results by language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.SearchButton.Text">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.ResourceName">
    <Value>Resource name:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.ResourceName.Tooltip">
    <Value>Filter results by resource name or part of resource name. Leave empty to load all resources.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.ResourceValue">
    <Value>Resource value:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResources.ResourceValue.Tooltip">
    <Value>Filter results by resource value or part of resource value. Leave empty to load all resources.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteOldExportedFilesButton.Text">
    <Value>Delete old exported files</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteOldExportedFilesButton.Tooltip">
    <Value>Delete old exported files in \files\ExportImport folder (PDF, Excel etc)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteOldExportedFiles.Success">
    <Value>{0} files were successfully deleted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Maintenance.DeleteOldExportedFiles.NoFilesToDelete">
    <Value>No files found to delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from -100000000 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAttributeValues.New.PriceAdjustment.RangeErrorMessage">
    <Value>The price must be from -100000000 to 100000000</Value>
  </LocaleResource>
  <LocaleResource Name="PollBlock.Title">
    <Value>Community Poll</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.OrderNotes">
    <Value>Order notes:</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.OrderNotes.CreatedOn">
    <Value>Created on</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.OrderNotes.Note">
    <Value>Note</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryAdd.ACL">
    <Value>Access control</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryDetails.ACL">
    <Value>Access control</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryACL.Note">
    <Value>NOTE: ACL rules applied to public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryACL.MarkRequired">
    <Value>Mark all customer roles you want to restrict access to this category</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.ProductTags">
    <Value>Products tagged with ''{0}''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.ProductThumbSize">
    <Value>Product thumbnail image size:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.ProductThumbSize.Tooltip">
    <Value>The default size (pixels) for product thumbnail images.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.ProductThumbSize.RequiredErrorMessage">
    <Value>Enter a product thumbnail image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.ProductThumbSize.RangeErrorMessage">
    <Value>The product thumbnail image size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.PriceGrabber.ProductThumbSize">
    <Value>Product thumbnail image size:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.PriceGrabber.ProductThumbSize.Tooltip">
    <Value>The default size (pixels) for product thumbnail images.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.PriceGrabber.ProductThumbSize.RequiredErrorMessage">
    <Value>Enter a product thumbnail image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.PriceGrabber.ProductThumbSize.RangeErrorMessage">
    <Value>The product thumbnail image size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Become.ProductThumbSize">
    <Value>Product thumbnail image size:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Become.ProductThumbSize.Tooltip">
    <Value>The default size (pixels) for product thumbnail images.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Become.ProductThumbSize.RequiredErrorMessage">
    <Value>Enter a product thumbnail image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Become.ProductThumbSize.RangeErrorMessage">
    <Value>The product thumbnail image size must be from 0 to 999999</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.EmailWishList">
    <Value>Allow customers to email their wishlists:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Products.EmailWishList.Tooltip">
    <Value>Check to allow customers to email their wishlists to friends. NOTE: This option is available only for registered customers</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.EmailButton">
    <Value>Email a friend</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.WishlistEmailAFriend">
    <Value>Email wishlist to a friend</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.EmailAFriend">
    <Value>Email wishlist to a friend</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.FriendEmail">
    <Value>Friend''s Email:</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.YourEmailAddress">
    <Value>Your Email address:</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.PersonalMessage">
    <Value>Personal Message:</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.EmailAFriendButton">
    <Value>Send email</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.Description">
    <Value>{0} products in the wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.OnlyRegisteredUsersCanEmailAFriend">
    <Value>Only registered customers can use email a friend feature</Value>
  </LocaleResource>
  <LocaleResource Name="EmailWishlist.YourMessageHasBeenSent">
    <Value>Your message has been sent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ImportEmails.All">
    <Value>Export all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsletterSubscribers.ImportEmails.Confirmed">
    <Value>Export only active (confirmed)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ProductTagUrl">
    <Value>Product tag url rewrite format:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ProductTagUrl.Tooltip">
    <Value>The format for product tag urls. Must have 3 arguments i.e. ''{0}producttag/{1}-{2}.aspx''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.SEODisplay.ProductTagUrl.ErrorMessage">
    <Value>You must enter a valid rewrite format string.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.RelativeDateTimeFormattingEnabled">
    <Value>Relative date and time formatting:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumsSettings.RelativeDateTimeFormattingEnabled.Tooltip">
    <Value>Click to enable relative date and time formatting (e.g. 2 hours ago, a month ago)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.HidePaymentInfoForZeroOrders">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodInfo.HidePaymentInfoForZeroOrders.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.DuplicateEmail">
    <Value>The e-mail address that you entered is already in use. Please enter a different e-mail address.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.DuplicateUserName">
    <Value>Please enter a different user name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.Title">
    <Value>System Information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.nopVersion">
    <Value>nopCommerce version:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.OperatingSystem">
    <Value>Operating system:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.ASPNETInfo">
    <Value>ASP.NET info:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.IsFullTrust">
    <Value>Is full trust level:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.ServerTimeZone">
    <Value>Server time zone:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.ServerLocalTime">
    <Value>Server local time:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemInformation.UTCTime">
    <Value>Greenwich mean time (GMT/UTC):</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.SystemInformation.TitleDescription">
    <Value>System information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.SystemInformation.Title">
    <Value>System information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SystemHome.SystemInformation.Description">
    <Value>View system and server information here</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.SystemInformationTitle">
    <Value>System Information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Sitemap.SystemInformationDescription">
    <Value>View System Information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Profiles.FormFields.TimeZoneEnabled">
    <Value>''Time Zone'' enabled:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.Profiles.FormFields.TimeZoneEnabled.Tooltip">
    <Value>Set if ''Time Zone'' is enabled (registration page). NOTE: Ensure that ''Allow customers to select time zone'' option is also enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.Currency">
    <Value>Currency:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Froogle.Currency.Tooltip">
    <Value>Select the default currency that will be used to generate the feed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.PriceGrabber.Currency">
    <Value>Currency:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.PriceGrabber.Currency.Tooltip">
    <Value>Select the default currency that will be used to generate the feed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Become.Currency">
    <Value>Currency:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PromotionProviders.Become.Currency.Tooltip">
    <Value>Select the default currency that will be used to generate the feed</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.Company">
    <Value>Company: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.Name">
    <Value>Name: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.Phone">
    <Value>Phone: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.Address">
    <Value>Address: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.Address2">
    <Value>Address 2: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="PdfPackagingSlip.ShippingMethod">
    <Value>Shipping method: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Placement">
    <Value>Placement:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Placement.Tooltip">
    <Value>Select the placement of Google Analytics script</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Placement.Head">
    <Value>Before the closing &lt;/head&gt; tag</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GlobalSettings.GoogleAnalytics.Placement.Body">
    <Value>Before the &lt;/body&gt; tag</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Logs.SearchButton.Text">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Logs.SearchButton.Tooltip">
    <Value>Search for log based on the criteria below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.CreatedOnFrom">
    <Value>Created from:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.CreatedOnFrom.Tooltip">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.CreatedOnTo">
    <Value>Created to:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.CreatedOnTo.Tooltip">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.Message">
    <Value>Message:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.Message.Tooltip">
    <Value>Search by message or part of entered message. Leave empty to load all records</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.LogType">
    <Value>Log type:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.LogType.Tooltip">
    <Value>Select a log type.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Log.AllLogTypes">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductVariantDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAttributeAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductAttributeDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SpecificationAttributeAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SpecificationAttributeDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CheckoutAttributeDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPaymentDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCardDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequestDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerAdd.AddButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerAdd.AddButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerAdd.SaveButton">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddressAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AddressDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRoleAdd.AddButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRoleAdd.AddButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRoleAdd.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRoleAdd.SaveButton.Tooltip">
    <Value>Save customer role</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRoleAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CustomerRoleDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateAdd.AddButton">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateAdd.AddButton.Tooltip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateAdd.SaveButton.Text">
    <Value>Save</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateAdd.SaveButton.Tooltip">
    <Value>Save affiliate</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.AffiliateDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CampaignAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CampaignDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.DiscountDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PricelistAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PricelistDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PollDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.NewsDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlogPostAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlogPostDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TopicDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumGroupAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumGroupDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ForumDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductTemplateAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ProductTemplateDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryTemplateAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CategoryTemplateDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerTemplateAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ManufacturerTemplateDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageTemplateDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResourceAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResourceDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.EmailAccountDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlacklistIPAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlacklistIPDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlacklistNetworkAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.BlacklistNetworkDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResourceAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LocaleStringResourceDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CreditCardTypeAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CreditCardTypeDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.PaymentMethodDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxProviderAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxProviderDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxCategoryAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.TaxCategoryDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingMethodDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethodAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShippingRateComputationMethodDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CountryAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CountryDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StateProvinceAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StateProvinceDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LanguageAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.LanguageDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrencyAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrencyDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.WarehouseAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.WarehouseDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureDimensionDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MeasureWeightDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SettingAdd.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SettingDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.MessageQueueDetails.SaveAndStayButton.Text">
    <Value>Save and Continue Edit</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountLimitation.0">
    <Value>Unlimited</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountLimitation.10">
    <Value>One Time Only</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountLimitation.15">
    <Value>N Times Only</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountLimitation.20">
    <Value>One Time Per Customer</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountLimitation.25">
    <Value>N Times Per Customer</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.1">
    <Value>None</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.2">
    <Value>Must be assigned to customer role</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.5">
    <Value>Customer must be registered</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.7">
    <Value>Has all of these product variants in the cart</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.8">
    <Value>Has one of these product variants in the cart</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.10">
    <Value>Had purchased all of these product variants</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.20">
    <Value>Had purchased one of these product variants</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.30">
    <Value>Had spent x.xx amount</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.60">
    <Value>Billing country is</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountRequirement.70">
    <Value>Shipping country is</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountType.1">
    <Value>Assigned to order total</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountType.2">
    <Value>Assigned to product variants (SKUs)</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountType.5">
    <Value>Assigned to categories</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountType.10">
    <Value>Assigned to shipping</Value>
  </LocaleResource>
  <LocaleResource Name="DiscountType.20">
    <Value>Assigned to order subtotal</Value>
  </LocaleResource>
  <LocaleResource Name="LowStockActivity.0">
    <Value>Nothing</Value>
  </LocaleResource>
  <LocaleResource Name="LowStockActivity.1">
    <Value>Disable buy button</Value>
  </LocaleResource>
  <LocaleResource Name="LowStockActivity.2">
    <Value>Unpublish</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ContinueShopping">
    <Value>Continue shopping</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.UpdateCart">
    <Value>Update shopping cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.nopCommerceNews.HideAdv">
    <Value>Hide advertisements</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.nopCommerceNews.DisplayAdv">
    <Value>Display advertisements</Value>
  </LocaleResource>
  <LocaleResource Name="Profile.PostedOn">
    <Value>Posted</Value>
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
SELECT LanguageID
FROM [Nop_Language]
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
		IF (EXISTS (SELECT 1 FROM Nop_LocaleStringResource WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [Nop_LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [Nop_LocaleStringResource]
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
			DELETE [Nop_LocaleStringResource]
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



IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[Nop_CustomerAction]
    WHERE [SystemKeyword] = N'ManageEmailSettings')
BEGIN
  INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
  VALUES (N'Manage Email Settings', N'ManageEmailSettings', N'',20)
END
GO 


UPDATE [dbo].[Nop_Currency]
SET [CustomFormatting]=N'€0.00'
WHERE [CurrencyCode]=N'EUR'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SearchPage.ProductsPerPage')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SearchPage.ProductsPerPage', N'10', N'')
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'Customer.NewPM')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'Customer.NewPM')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'Customer.NewPM' 

	IF (@MessageTemplateID > 0)
	BEGIN

	--do it for each existing language
	DECLARE @ExistingLanguageID int
	DECLARE cur_existinglanguage CURSOR FOR
	SELECT LanguageID
	FROM [Nop_Language]
	OPEN cur_existinglanguage
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--insert localized message template
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. You have received a new private message',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />
		You have received a new private message.
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewCustomer.Notification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewCustomer.Notification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewCustomer.Notification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		--do it for each existing language
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT LanguageID
		FROM [Nop_Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'New customer registration',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />A new customer registered with your store. Below are the customer''s details:
		<br />Full name: %Customer.FullName%
		<br />Email: %Customer.Email%
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PaypalStandard.ValidateOrderTotal')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PaypalStandard.ValidateOrderTotal', N'true', N'')
END
GO


--new discount type
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountType]
		WHERE [DiscountTypeID] = 20)
BEGIN
	INSERT [dbo].[Nop_DiscountType] ([DiscountTypeID], [Name])
	VALUES (20, N'Assigned to order subtotal')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountInclTax')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountInclTax] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountInclTax] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountInclTaxInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountInclTaxInCustomerCurrency] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountInclTaxInCustomerCurrency] DEFAULT ((0))
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountExclTax')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountExclTax] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountExclTax] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountExclTaxInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountExclTaxInCustomerCurrency] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountExclTaxInCustomerCurrency] DEFAULT ((0))
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.StoreClosed.AllowAdminAccess')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.StoreClosed.AllowAdminAccess', N'True', N'')
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 5)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (5, N'Must be registered')
END
GO

--return requets message templates
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewReturnRequest.StoreOwnerNotification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewReturnRequest.StoreOwnerNotification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewReturnRequest.StoreOwnerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		--do it for each existing language
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT LanguageID
		FROM [Nop_Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. New return request.',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />
		%Customer.FullName% (%Customer.Email%) has just submitted a new return request. Details are below:
		<br />
		Request ID: %ReturnRequest.ID%
		<br />
		Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%
		<br />
		Reason for return: %ReturnRequest.Reason%
		<br />
		Requested action: %ReturnRequest.RequestedAction%
		<br />
		Customer comments:
		<br />
		%ReturnRequest.CustomerComment%
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'ReturnRequestStatusChanged.CustomerNotification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'ReturnRequestStatusChanged.CustomerNotification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'ReturnRequestStatusChanged.CustomerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		--do it for each existing language
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT LanguageID
		FROM [Nop_Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. Return request status was changed.',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />
		Hello %Customer.FullName%,
		<br />
		Your return request #%ReturnRequest.ID% status has been changed.
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO

--rename 'Extension' to 'MimeType' (Nop_Picture)
IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Picture]') and NAME='Extension')
BEGIN
 ALTER TABLE [dbo].[Nop_Picture] ADD MimeType nvarchar(20) NOT NULL CONSTRAINT [DF_Nop_Picture_MimeType] DEFAULT ((''))
 EXEC('UPDATE [dbo].[Nop_Picture] SET MimeType=Extension')
 ALTER TABLE [dbo].[Nop_Picture] DROP COLUMN Extension
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PictureLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PictureLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_PictureLoadAllPaged]
(
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		PictureID int NOT NULL		 
	)
	INSERT INTO #PageIndex (PictureID)
	SELECT
		PictureID
	FROM [Nop_Picture]
	ORDER BY PictureID DESC

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn

	SELECT 
		[p].PictureId,
		[p].PictureBinary,
		[p].MimeType,
		[p].IsNew
	FROM [Nop_Picture] [p]
		INNER JOIN #PageIndex [pi]
		ON [p].PictureID = [pi].PictureID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound

	SET ROWCOUNT 0
	
	DROP TABLE #PageIndex

END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Products.ShowCategoryProductNumber.IncludeSubCategories')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Products.ShowCategoryProductNumber.IncludeSubCategories', N'True', N'')
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PDFInvoice.RenderOrderNotes')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PDFInvoice.RenderOrderNotes', N'False', N'')
END
GO



--ACL per object
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLPerObject]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_ACLPerObject](
	[ACLPerObjectId] int IDENTITY(1,1) NOT NULL,
	[ObjectId] int NOT NULL,
	[ObjectTypeId] int NOT NULL,
	[CustomerRoleId] int NOT NULL,
	[Deny] bit NOT NULL,
 CONSTRAINT [PK_ACLPerObject] PRIMARY KEY CLUSTERED 
(
	[ACLPerObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ACLPerObject_Nop_CustomerRole'
           AND parent_obj = Object_id('Nop_ACLPerObject')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ACLPerObject
DROP CONSTRAINT FK_Nop_ACLPerObject_Nop_CustomerRole
GO
ALTER TABLE [dbo].[Nop_ACLPerObject]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACLPerObject_Nop_CustomerRole] FOREIGN KEY([CustomerRoleId])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PromotionProvider.BecomeCom.ProductThumbnailImageSize')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PromotionProvider.BecomeCom.ProductThumbnailImageSize', N'125', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PromotionProvider.Froogle.ProductThumbnailImageSize')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PromotionProvider.Froogle.ProductThumbnailImageSize', N'125', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PromotionProvider.PriceGrabber.ProductThumbnailImageSize')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PromotionProvider.PriceGrabber.ProductThumbnailImageSize', N'125', N'')
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.EmailWishlist')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.EmailWishlist', N'True', N'')
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'Wishlist.EmailAFriend')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'Wishlist.EmailAFriend')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'Wishlist.EmailAFriend' 

	IF (@MessageTemplateID > 0)
	BEGIN

	--do it for each existing language
	DECLARE @ExistingLanguageID int
	DECLARE cur_existinglanguage CURSOR FOR
	SELECT LanguageID
	FROM [Nop_Language]
	OPEN cur_existinglanguage
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--insert localized message template
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. Wishlist',  N'<p><a href="%Store.URL%"> %Store.Name%</a> <br />
<br />
%Customer.Email% was shopping on %Store.Name% and wanted to share a wishlist with you. <br />
<br />
<br />
For more info click <a target="_blank" href="%Wishlist.URLForCustomer%">here</a> <br />
<br />
<br />
%EmailAFriend.PersonalMessage%<br />
<br />
%Store.Name%</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO

--new discount requirements
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 7)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (7, N'Has all of these product variants in the cart')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 8)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (8, N'Has one of these product variants in the cart')
END
GO

--update NewVATSubmitted.StoreOwnerNotification message template
IF EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewVATSubmitted.StoreOwnerNotification')
BEGIN
	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewVATSubmitted.StoreOwnerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN

	--do it for each existing language
	DECLARE @ExistingLanguageID int
	DECLARE cur_existinglanguage CURSOR FOR
	SELECT LanguageID
	FROM [Nop_Language]
	OPEN cur_existinglanguage
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--update localized message template
		UPDATE [Nop_MessageTemplateLocalized]
		SET [Body] = N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
<br />
%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:
<br />
VAT number: %Customer.VatNumber%
<br />
VAT number status: %Customer.VatNumberStatus%
<br />
Received name: %VatValidationResult.Name%
<br />
Received address: %VatValidationResult.Address%
</p>'
		WHERE [MessageTemplateID] = @MessageTemplateID and
			  [LanguageID] = @ExistingLanguageID

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO


--product tag URL rewrites
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SEO.ProductTags.UrlRewriteFormat')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SEO.ProductTags.UrlRewriteFormat', N'{0}producttag/{1}-{2}.aspx', N'')
END
GO

IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_PaymentMethod]') and NAME='HidePaymentInfoForZeroOrders')
BEGIN
	ALTER TABLE [dbo].[Nop_PaymentMethod] DROP CONSTRAINT [DF_Nop_PaymentMethod_HidePaymentInfoForZeroOrders]
	ALTER TABLE [dbo].[Nop_PaymentMethod] DROP COLUMN HidePaymentInfoForZeroOrders
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
GO

CREATE PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
(
	@ShowHidden bit = 0,
	@FilterByCountryID int = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	IF(@FilterByCountryID IS NOT NULL AND @FilterByCountryID != 0)
		BEGIN
			SELECT  
				pm.PaymentMethodId,
				pm.Name,
				pm.VisibleName,
				pm.Description,
				pm.ConfigureTemplatePath,
				pm.UserTemplatePath,
				pm.ClassName,
				pm.SystemKeyword,
				pm.IsActive,
				pm.DisplayOrder
		    FROM 
				[Nop_PaymentMethod] pm
		    WHERE 
                pm.PaymentMethodID NOT IN 
				(
				    SELECT 
						pmc.PaymentMethodID
				    FROM 
						[Nop_PaymentMethod_RestrictedCountries] pmc
				    WHERE 
						pmc.CountryID = @FilterByCountryID AND 
						pm.PaymentMethodID = pmc.PaymentMethodID
				)
				AND
				(IsActive = 1 or @ShowHidden = 1)
		   ORDER BY 
				pm.DisplayOrder
		END
	ELSE
		BEGIN
			SELECT 
				*
			FROM 
				[Nop_PaymentMethod]
			WHERE 
				(IsActive = 1 or @ShowHidden = 1)
			ORDER BY 
				DisplayOrder
		END
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@RelatedToProductID	int = 0,
	@Keywords			nvarchar(MAX),
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
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
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #DisplayOrderTmp ([ProductID])
	SELECT p.ProductID
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_RelatedProduct rp with (NOLOCK) ON p.ProductID=rp.ProductID2
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
		   (
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				@RelatedToProductID IS NULL OR @RelatedToProductID=0
				OR rp.ProductID1=@RelatedToProductID
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
				@PriceMin IS NULL OR @PriceMin=0
				OR pv.Price > @PriceMin	
			)
		AND (
				@PriceMax IS NULL OR @PriceMax=2147483644 -- max value
				OR pv.Price < @PriceMax
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
					-- search language content
					or patindex(@Keywords, pl.name) > 0
					or patindex(@Keywords, pvl.name) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, pl.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pl.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pvl.Description) > 0)
				)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @RelatedToProductID IS NOT NULL AND @RelatedToProductID > 0
		THEN rp.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOn END DESC

	DROP TABLE #FilteredSpecs

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	DROP TABLE #DisplayOrderTmp

	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p with (NOLOCK) on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogClearAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogClearAll]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionLoadNonEmpty]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerSessionLoadNonEmpty]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PrivateMessageLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PrivateMessageLoadAll]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SearchLogClear]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SearchLogClear]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_LogClear]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LogClear]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLoadAll]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PictureLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PictureLoadAllPaged]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductRatingCreate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductRatingCreate]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RewardPointsHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RewardPointsHistoryLoadAll]
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.PaymentManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.PaymentManager.CacheEnabled', N'true', N'')
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.CreditCardTypeManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.PaymentMethodManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.PaymentStatusManager.CacheEnabled'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.BlacklistManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.BlacklistManager.CacheEnabled', N'true', N'')
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.IpBlacklistManager.CacheEnabled'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.ShippingManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.ShippingManager.CacheEnabled', N'true', N'')
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.ShippingRateComputationMethodManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.ShippingStatusManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.ShippingMethodManager.CacheEnabled'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.CustomerActivityManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.CustomerActivityManager.CacheEnabled', N'true', N'')
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountLimitation'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountLimitation
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountLimitation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_DiscountLimitation]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountRequirement'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountRequirement
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountRequirement]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_DiscountRequirement]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountType'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_DiscountType]
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Log_Nop_LogType'
           AND parent_obj = Object_id('Nop_Log')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Log
DROP CONSTRAINT FK_Nop_Log_Nop_LogType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_LogType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_LogType]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ProductVariant_Nop_LowStockActivity'
           AND parent_obj = Object_id('Nop_ProductVariant')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ProductVariant
DROP CONSTRAINT FK_Nop_ProductVariant_Nop_LowStockActivity
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_LowStockActivity]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_LowStockActivity]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ShoppingCart_Nop_ShoppingCartType'
           AND parent_obj = Object_id('Nop_ShoppingCartItem')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ShoppingCartItem
DROP CONSTRAINT FK_Nop_ShoppingCart_Nop_ShoppingCartType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ShoppingCartType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_ShoppingCartType]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Order_Nop_ShippingStatus'
           AND parent_obj = Object_id('Nop_Order')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Order
DROP CONSTRAINT FK_Nop_Order_Nop_ShippingStatus
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ShippingStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_ShippingStatus]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Order_Nop_PaymentStatus'
           AND parent_obj = Object_id('Nop_Order')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Order
DROP CONSTRAINT FK_Nop_Order_Nop_PaymentStatus
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_PaymentStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_PaymentStatus]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Order_Nop_OrderStatus'
           AND parent_obj = Object_id('Nop_Order')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Order
DROP CONSTRAINT FK_Nop_Order_Nop_OrderStatus
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_OrderStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_OrderStatus]
END
GO

UPDATE dbo.Nop_Log
SET LogTypeId=20
WHERE LogTypeId=0
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionDeleteExpired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerSessionDeleteExpired]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryLoadAll]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_SubscriptionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_SubscriptionLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShippingMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShippingMethodLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductTag_Product_MappingDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductTag_Product_MappingDelete]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductTag_Product_MappingInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductTag_Product_MappingInsert]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductTagUpdateCounts]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductTagUpdateCounts]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentHistoryLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RecurringPaymentLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RecurringPaymentLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ShoppingCartItemDeleteExpired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ShoppingCartItemDeleteExpired]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SearchTermReport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SearchTermReport]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerReportByLanguage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerReportByLanguage]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderAverageReport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderAverageReport]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderIncompleteReport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderIncompleteReport]
GO 


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicLoadActive]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicLoadActive]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicLoadAll]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_ForumDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_ForumDelete]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerUpdateCounts]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerUpdateCounts]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_ForumUpdateCounts]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_ForumUpdateCounts]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicUpdateCounts]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicUpdateCounts]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostDelete]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostInsert]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostUpdate]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicDelete]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicInsert]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicInsert]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicUpdate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicUpdate]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_TaxRateLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_TaxRateLoadAll]
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductTagLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductTagLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_OrderProductVariantReport]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_OrderProductVariantReport]
GO
CREATE PROCEDURE [dbo].[Nop_OrderProductVariantReport]
(
	@StartTime datetime = NULL,
	@EndTime datetime = NULL,
	@OrderStatusID int,
	@PaymentStatusID int,
	@BillingCountryID int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT opv.ProductVariantId, isnull(sum(opv.PriceExclTax), 0) as PriceExclTax, isnull(sum(opv.Quantity), 0) as Quantity
	FROM Nop_OrderProductVariant opv 
	INNER JOIN [Nop_Order] o ON o.OrderId = opv.OrderID
	WHERE
		(@StartTime is NULL or @StartTime <= o.CreatedOn) and
		(@EndTime is NULL or @EndTime >= o.CreatedOn) and 
		(@OrderStatusID IS NULL or @OrderStatusID=0 or o.OrderStatusID = @OrderStatusID) and
		(@PaymentStatusID IS NULL or @PaymentStatusID=0 or o.PaymentStatusID = @PaymentStatusID) and
		(@BillingCountryID IS NULL or @BillingCountryID=0 or o.BillingCountryID = @BillingCountryID) and
		(o.Deleted=0)
	GROUP BY opv.ProductVariantId
	ORDER BY PriceExclTax desc
END
GO


--update current version
UPDATE [dbo].[Nop_Setting] 
SET [Value]='1.90'
WHERE [Name]='Common.CurrentVersion'
GO

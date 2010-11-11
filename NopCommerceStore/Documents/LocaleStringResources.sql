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
    <Value>Notify customers when new a private messages is sent.</Value>
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
    <Value>A value indicationing whether pictures are stored into database or filesystem.</Value>
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

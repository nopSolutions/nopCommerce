--upgrade scripts from nopCommerce 3.70 to 3.80

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.NotifyAboutPrivateMessages.Hint">
    <Value>Indicates whether a customer should be notified by email about new private messages.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing.Hint">
    <Value>Check to allow customers to edit items already placed in the cart or wishlist. It could be useful when your products have attributes or any other fields entered by a customer.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.AddToWishlist.Update">
    <Value>Update</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture.Hint">
    <Value>Choose a picture associated to this attribute value. This picture will replace the main product image when this product attribute value is clicked (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ImageSquaresPicture">
    <Value>Square picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ImageSquaresPicture.Hint">
    <Value>Upload a picture to be used with the image squares attribute control</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewTopic">
    <Value>Added a new topic (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditTopic">
    <Value>Edited a topic (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteTopic">
    <Value>Deleted a topic (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteOrder">
    <Value>Deleted an order (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditOrder">
    <Value>Edited an order (ID = {0}). See order notes for details</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products">
    <Value>Restricted products [and quantity range]</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products">
    <Value>Restricted products [and quantity range]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PageSize">
    <Value>Page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PageSize.Hint">
    <Value>Set the page size for history of reward points on ''My account'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SaveBeforeEdit">
    <Value>You need to save the language before you can make or change resources for this language.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources">
    <Value>String resources</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Localization">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Select">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Fields.LanguageName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.View">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore">
    <Value>Reviews per store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore.Hint">
    <Value>Check to display reviews written in the current store only (on a product details page).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page Size options should have unique items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page Size options should have unique items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page Size options should not have duplicate items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Productreviews.Fields.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.Fields.Store.Hint">
	<Value>A store name in which this review was written.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchStore">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchStore.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions">
    <Value>Sort options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion">
    <Value>reCAPTCHA version</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion.Hint">
    <Value>Select version of the reCAPTCHA.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.WrongCaptchaV2">
    <Value>The reCAPTCHA response is invalid or malformed. Please try again.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Web.Framework.Security.Captcha.ReCaptchaVersion.Version1">
    <Value>Version 1.0</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Web.Framework.Security.Captcha.ReCaptchaVersion.Version2">
    <Value>Version 2.0</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchProduct">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchProduct.Hint">
    <Value>Search by a specific product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Imported">
    <Value>Products have been imported successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.FlagImageFileName.Hint">
    <Value>The flag image file name. The image should be saved into \images\flags\ directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.DontSendBeforeDate">
    <Value>Planned date of sending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.DontSendBeforeDate.Hint">
    <Value>The specific send date and time.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.SendImmediately">
    <Value>Send immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.SendImmediately.Hint">
    <Value>Send message immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.DontSendBeforeDate">
    <Value>Planned date of sending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.DontSendBeforeDate.Hint">
    <Value>Enter a specific date and time to send the campaign. Leave empty to send it immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.DontSendBeforeDate">
    <Value>Planned date of sending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.DontSendBeforeDate.Hint">
    <Value>The specific send date and time.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.SendImmediately">
    <Value>Send immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.SendImmediately.Hint">
    <Value>Send message immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.ImportFromExcelTip">
    <Value>Imported categories are distinguished by ID. If the ID already exists, then its corresponding category will be updated. For new categories ID do not need to specify</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.ImportFromExcelTip">
    <Value>Imported manufacturers are distinguished by ID. If the ID already exists, then its corresponding manufacturer will be updated. For new manufacturers ID do not need to specify</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Imported">
    <Value>Categories have been imported successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Imported">
    <Value>Manufacturers have been imported successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.SearchSettingName">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.SearchSettingName.Hint">
    <Value>Search by a specific setting name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.SearchSettingValue">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.SearchSettingValue.Hint">
    <Value>Search by a specific setting value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceName">
    <Value>Resource name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceName.Hint">
    <Value>Search by a specific resource.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceValue">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceValue.Hint">
    <Value>Search by a specific resource value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend">
    <Value>Delay send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend.Hint">
    <Value>The delay before sending message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.SendImmediately">
    <Value>Send immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.SendImmediately.Hint">
    <Value>Send message immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.MessageDelayPeriod.Days">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.MessageDelayPeriod.Hours">
    <Value>Hours</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowSearchByVendor">
    <Value>Allow search by vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowSearchByVendor.Hint">
    <Value>Check to allow customers to search by vendor on advanced search page.</Value>
  </LocaleResource>
  <LocaleResource Name="Search.Vendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Published">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Published.Hint">
    <Value>Determines whether this topic is published (visible) in your store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ImportToExcel.ManyRecordsWarning">
    <Value>Import requires a lot of memory resources. That''s why it''s not recommended to import more than 500 - 1,000 records at once. If you have more records, it''s better to split them to multiple Excel files and import separately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderStatus">
    <Value>Order statuses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderStatus.Hint">
    <Value>Search by a specific order statuses e.g. Complete.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.PaymentStatus">
    <Value>Payment statuses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.PaymentStatus.Hint">
    <Value>Search by a specific payment statuses e.g. Paid.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.ShippingStatus">
    <Value>Shipping statuses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.ShippingStatus.Hint">
    <Value>Search by a specific shipping statuses e.g. Not yet shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition">
     <Value>Condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.Attributes">
     <Value>Attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.Attributes.Hint">
     <Value>Choose an attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.EnableCondition">
     <Value>Enable condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.EnableCondition.Hint">
     <Value>Check to specify a condition (depending on other attribute) when this attribute should be enabled (visible).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.NoAttributeExists">
     <Value>No attribute exists that could be used as condition.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Condition.SaveBeforeEdit">
     <Value>You need to save the checkout attribute before you can edit conditional attributes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.BackupCreated">
    <Value>The backup created</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.BackupDeleted">
    <Value>Backup file "{0}" deleted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.BackupNow">
    <Value>Backup now</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.DatabaseBackups">
    <Value>Database backups</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.DatabaseRestored">
    <Value>Database is restored</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.Delete">
    <Value>Delete</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.FileName">
    <Value>File Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.FileSize">
    <Value>File Size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.Restore">
    <Value>Restore</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Maintenance.BackupDatabase.Progress">
    <Value>Processing database backup...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.FlagImage">
    <Value>Flag image</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.List.SearchIpAddress">
     <Value>IP address</Value>
   </LocaleResource>
   <LocaleResource Name="Admin.Customers.Customers.List.SearchIpAddress.Hint">
     <Value>Search by IP address.</Value>
   </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.ColorSquaresRgb">
    <Value>RGB color</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.ColorSquaresRgb.Hint">
    <Value>Choose color to be used instead of an option text name (it''ll be displayed as "color square").</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.EnableColorSquaresRgb">
    <Value>Specify color</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.EnableColorSquaresRgb.Hint">
    <Value>Check to choose color to be used instead of an option text name (it''ll be displayed as "color square").</Value>
  </LocaleResource> 
  <LocaleResource Name="Account.Fields.ConfirmEmail">
    <Value>Confirm email</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.ConfirmEmail.Required">
    <Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Email.EnteredEmailsDoNotMatch">
    <Value>The email and confirmation email do not match.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.EnteringEmailTwice">
    <Value>Force entering email twice</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.EnteringEmailTwice.Hint">
    <Value>Force entering email twice during registration</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists">
    <Value>This attribute is already added to this product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Header.LoggedInAs">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.MaximumProductNumber">
    <Value>Maximum number of products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.MaximumProductNumber.Hint">
    <Value>Sets a maximum number of products per vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ExceededMaximumNumber">
    <Value>The maximum allowed number of products has been exceeded ({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions.List.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.NotReturnable">
    <Value>Not returnable</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.NotReturnable.Hint">
    <Value>Check if this product is not returnable. In this case a customer won''t be allowed to submit return request.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.Impersonation.Started">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.Impersonation.Finished">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.Impersonation.Started.StoreOwner">
    <Value>Started customer impersonation (Email: {0}, ID = {1})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.Impersonation.Finished.StoreOwner">
    <Value>Finished customer impersonation (Email: {0}, ID = {1})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.Impersonation.Started.Customer">
    <Value>Impersonated by store owner (Email: {0}, ID = {1})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.Impersonation.Finished.Customer">
    <Value>Impersonation by store owner was finished (Email: {0}, ID = {1})</Value>
  </LocaleResource>
  <LocaleResource Name="Common.ManagePage">
    <Value>Manage this page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog.IpAddress">
    <Value>IP address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog.IpAddress.Hint">
    <Value>The IP address for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.AllowPostVoting">
    <Value>Allow users to vote for posts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.AllowPostVoting.Hint">
    <Value>Set if you want to allow users to vote for posts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.MaxVotesPerDay">
    <Value>Maximum votes per day</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.MaxVotesPerDay.Hint">
    <Value>Maximum number of votes for user per day.</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Votes">
    <Value>Votes</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Votes.AlreadyVoted">
    <Value>You already voted for this post</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Votes.Login">
    <Value>You need to log in to vote for post</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Votes.MaxVotesReached">
    <Value>A maximum of {0} votes can be cast per user per day</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.Votes.OwnPost">
    <Value>You cannot vote for your own post</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ImportProductImagesUsingHash">
    <Value>Import product images using hash</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.ImportProductImagesUsingHash.Hint">
    <Value>Check to use fast HASHBYTES (hash sum) database function to compare pictures when importing products. Please note that this functionality is not supported by some database.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.System.SystemInfo.UTCTime">
    <Value>Coordinated Universal Time (UTC)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.UTCTime.Hint">
    <Value>Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.CurrentUserTime">
    <Value>Current user time</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SystemInfo.CurrentUserTime.Hint">
    <Value>Current user time (based on specified datetime and timezone settings).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.Earning.Hint1">
    <Value>Each</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.Earning.Hint2">
    <Value>spent will earn</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.Earning.Hint3">
    <Value>reward points</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.ExchangeRate.Hint2">
    <Value>1 reward point =</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.SwitchToTreeView">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Treeview">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Note1">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.Note2">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.AddNew.UpdateTotals">
    <Value>Do not to forget to update order totals after adding this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.List.SearchIncludeSubCategories">
    <Value>Search sub categories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Export">
    <Value>Export</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoices">
    <Value>Print PDF invoices</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoices.All">
    <Value>Print PDF invoices (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoices.Selected">
    <Value>Print PDF invoices (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoice.All">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.PdfInvoice.Selected">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlips">
    <Value>Print packaging slips</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlips.All">
    <Value>Print packaging slips (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlips.Selected">
    <Value>Print packaging slips (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.All">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.Selected">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.ApiKey">
    <Value>Australia Post API Key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.ApiKey.Hint">
    <Value>Specify Australia Post API Key.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.GatewayUrl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.GatewayUrl.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Import">
    <Value>Import</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.StoreStatistics">
    <Value></Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Menu.Search">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.AddTitle">
    <Value>Add (reduce) points</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue.Hint">
    <Value>Enter points to add. Negative values are also supported (reduce points).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.AddButton">
    <Value>Add (reduce) reward points</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon">
    <Value>General settings</Value>
  </LocaleResource>
  <LocaleResource Name="Permission.ManageExternalAuthenticationMethods">
    <Value>Admin area. Manage External Authentication</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods">
    <Value>External authentication</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.BackToList">
    <Value>back to extenal authentication method list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed">
    <Value>All plugins and themes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Description">
    <Value>Shipping methods used by offline shipping rate compuration methods (e.g. "Fixed Rate Shipping" or "Shipping by weight").</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.EmailAccounts">
    <Value>Email accounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails">
    <Value>Message queue</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.NewsLetterSubscriptions">
    <Value>Newsletter subscribers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes">
    <Value>Product attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes">
    <Value>Checkout attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes">
    <Value>Specification attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement">
    <Value>Content management</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartProductNumber">
    <Value>Number of products in mini-shopping cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.NumberOfOrders">
    <Value>Orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.NumberOfCustomers">
    <Value>Registered customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.NumberOfPendingReturnRequests">
    <Value>Pending return requests</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.NumberOfLowStockProducts">
    <Value>Low stock products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.MoreInfo">
    <Value>More info</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Dashboard.IncompleteOrders">
    <Value>Incomplete orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.OrderStatistics">
    <Value>Orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.OrderStatistics.Month">
    <Value>Month</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.OrderStatistics.Week">
    <Value>Week</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.OrderStatistics.Year">
    <Value>Year</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics">
    <Value>New customers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics.Month">
    <Value>Month</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics.Week">
    <Value>Week</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.CustomerStatistics.Year">
    <Value>Year</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.LatestOrders">
    <Value>Latest Orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.LatestOrders.ViewAll">
    <Value>View All Orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Dashboard.CommonStatistics">
    <Value>Common statistics</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.RequireRegistrationForDownloadableProducts">
    <Value>Require registration for downloadable products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.RequireRegistrationForDownloadableProducts.Hint">
    <Value>Require account creation to purchase downloadable products.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderGuid">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.OrderGuid.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerProductReviews">
    <Value>My product reviews</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerProductReviews.ApprovalStatus.Approved">
    <Value>Approved</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerProductReviews.ApprovalStatus.Pending">
    <Value>Pending</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerProductReviews.ProductReviewFor">
    <Value>Product review for</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsTabOnAccountPage">
    <Value>Show product reviews tab on ''My account'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsTabOnAccountPage.Hint">
    <Value>Check to show product reviews tab on '' My account'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewsPageSizeOnAccountPage">
    <Value>Product reviews page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewsPageSizeOnAccountPage.Hint">
    <Value>Set the page size for product reviews e.g. ''10'' reviews per page.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.CustomerProductReviews">
    <Value>My product reviews</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask">
    <Value>Return request number mask</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask.Hint">
    <Value>Return request number mask. For example, RMA-{ID}-{YYYY}-{MM}-{DD}.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask.Description.ID">
    <Value>{ID} - Return request identifier</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask.Description.YYYY">
    <Value>{YYYY} - year of return request creation date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask.Description.YY">
    <Value>{YY} - last two digits of year of return request creation date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask.Description.MM">
    <Value>{MM} - month of return request creation date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestNumberMask.Description.DD">
    <Value>{DD} - day of return request creation date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.CustomNumber">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.CustomNumber.Hint">
    <Value>Return request identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.ID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.ID.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories.Fields.Category">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories.Fields.IsFeaturedProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories.SaveBeforeEdit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Categories">
    <Value>Categories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Categories.Hint">
    <Value>Choose categories. You can manage product categories by selecting Catalog > Categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories.NoCategoriesAvailable">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Categories.NoCategoriesAvailable">
    <Value>No categories available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.AllowNotRegisteredUsersToLeaveComments">
    <Value>Allow guests to leave comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.AllowNotRegisteredUsersToLeaveComments.Hint">
    <Value>Check to allow guests to leave comments.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.AllowNotRegisteredUsersToLeaveComments">
    <Value>Allow guests to leave comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.AllowNotRegisteredUsersToLeaveComments.Hint">
    <Value>Check to allow guests to leave comments.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.General">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.Performance">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviews">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.Search">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.CompareProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.Sharing">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateTimeSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationAutoRegisterEnabled">
    <Value>External authentication. Auto register enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationAutoRegisterEnabled.Hint">
    <Value>Check to enable auto registration when using external authentication (e.g. using Facebokk or Twitter).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FullTextSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LocalizationSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.PdfSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SecuritySettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SEOSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.StoreInformationSettings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.ReloadList.Progress">
    <Value>Reloading plugin list...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultTitle">
    <Value>Default page title</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowVendorsToEditInfo">
    <Value>Allow vendors to edit info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowVendorsToEditInfo.Hint">
    <Value>Check to allow vendors to edit information about themselves (in public store). Please note that localizable properties (name, description) are not supported in case if you have multiple languages (only standard values can be edited in this case).</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo">
    <Value>Vendor info</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.VendorInfo">
    <Value>Vendor info</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Description">
    <Value>Description</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Picture.Remove">
    <Value>Remove picture</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Picture.ErrorMessage">
    <Value>You can add only picture file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.NotifyStoreOwnerAboutVendorInformationChange">
    <Value>Notify about vendor information changes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.NotifyStoreOwnerAboutVendorInformationChange.Hint">
    <Value>Check to notify a store owner about vendor information changes.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Name.Required">
    <Value>Vendor name is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.VendorInfo.Email.Required">
    <Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Description">
    <Value>Description</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Picture.ErrorMessage">
    <Value>You can add only picture file</Value>
  </LocaleResource>
  <LocaleResource Name="Filtering.SpecificationFilter.Separator">
    <Value>or </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manufacturers.Fields.Manufacturer">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manufacturers.Fields.IsFeaturedProduct">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manufacturers.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manufacturers.SaveBeforeEdit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Manufacturers">
    <Value>Manufacturers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Manufacturers.Hint">
    <Value>Choose the manufacturer. You can manage manufacturers by selecting Catalog > Manufacturers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ACL">
     <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AdditionalShippingCharge">
     <Value>Additional shipping charge</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AdminComment">
     <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AllowBackInStockSubscriptions">
     <Value>Allow back in stock subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AllowCustomerReviews">
     <Value>Allow customer reviews</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AllowedQuantities">
     <Value>Allowed quantities</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AllowAddingOnlyExistingAttributeCombinations">
     <Value>Allow only existing attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AvailableEndDate">
     <Value>Available end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AvailableForPreOrder">
     <Value>Available for pre-order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.AvailableStartDate">
     <Value>Available start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Backorders">
     <Value>Backorders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.CommonInfo">
     <Value>General information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.AdvancedProductTypes">
     <Value>Advanced product types</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Price">
     <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Shipping">
     <Value>Shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Security">
     <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Inventory">
     <Value>Inventory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Mappings">
     <Value>Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.LinkedProducts">
     <Value>Linked products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Button">
     <Value>Settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.CallForPrice">
     <Value>Call for price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.CreatedOn">
     <Value>Created on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.CrossSellsProducts">
     <Value>Cross-sells products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.CustomerEntersPrice">
     <Value>Customer enters price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DeliveryDate">
     <Value>Delivery date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Dimensions">
    <Value>Dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisableBuyButton">
     <Value>Disable buy button</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisableWishlistButton">
     <Value>Disable wishlist button</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Discounts">
     <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisplayOrder">
     <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisplayStockAvailability">
     <Value>Display availability</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DisplayStockQuantity">
     <Value>Display stock qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.DownloadableProduct">
     <Value>Downloadable product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.FreeShipping">
     <Value>Free shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.GTIN">
     <Value>GTIN (global trade item number)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Id">
     <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.IsGiftCard">
     <Value>Is gift card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.IsRental">
     <Value>Is rental</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.LowStockActivity">
     <Value>Low stock activity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ManufacturerPartNumber">
     <Value>Manufacturer part number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MarkAsNew">
     <Value>Mark as new</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MarkAsNewEndDate">
     <Value>Mark as new. End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MarkAsNewStartDate">
     <Value>Mark as new. Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MaximumCartQuantity">
     <Value>Maximum cart qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MinimumCartQuantity">
     <Value>Minimum cart qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.MinimumStockQuantity">
     <Value>Minimum stock qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ModalDescription">
     <Value>Check fields you want to see on the product details page in the "basic" mode.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ModalTitle">
     <Value>Settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.NotifyAdminForQuantityBelow">
     <Value>Notify for qty below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.NotReturnable">
     <Value>Not returnable</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.PAngV">
     <Value>PAngV (base price) enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ProductCost">
     <Value>Product cost</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ProductTags">
     <Value>Product tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ProductTemplate">
     <Value>Product template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ProductType">
     <Value>Product type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Published">
     <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.RecurringProduct">
     <Value>Recurring product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.RelatedProducts">
     <Value>Related products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.RequireOtherProductsAddedToTheCart">
     <Value>Require other products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ShipSeparately">
     <Value>Ship separately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ShowOnHomePage">
     <Value>Show on home page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecialPrice">
     <Value>Special price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecialPriceEndDate">
     <Value>Special price end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecialPriceStartDate">
     <Value>Special price start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Stores">
     <Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.TelecommunicationsBroadcastingElectronicServices">
     <Value>Telecommunications, broadcasting and electronic services</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.TierPrices">
     <Value>Tier prices</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Vendor">
     <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.VisibleIndividually">
     <Value>Visible individually</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.UpdatedOn">
     <Value>Updated on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.UseMultipleWarehouses">
     <Value>Use multiple warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Warehouse">
     <Value>Warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Weight">
    <Value>Weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Prices">
     <Value>Prices</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.CommonInfo">
     <Value>General information</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Inventory">
     <Value>Inventory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Shipping">
     <Value>Shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Mappings">
     <Value>Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Security">
     <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.RequireOtherProducts">
     <Value>Require other products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Categories.NoCategoriesAvailable">
     <Value>No categories available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manufacturers">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Manufacturers.NoManufacturersAvailable">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Manufacturers.NoManufacturersAvailable">
     <Value>No manufacturers available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles.Hint">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableStores">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AvailableStores.Hint">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Discounts">
     <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Discounts.Hint">
     <Value>Select discounts to apply to this product. You can manage discounts by selecting Discounts from the Promotions menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Discounts.NoDiscounts">
    <Value>No discounts available. Create at least one discount before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Discounts.NoDiscounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Discounts">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LimitedToStores">
    <Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles">
     <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles.Hint">
    <Value>Choose one or several customer roles i.e. administrators, vendors, guests, who will be able to see this product in catalog. If you don''t need this option just leave this field empty. In order to use this functionality you have to disable the following setting: Catalog settings > Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Acl">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Stores">
     <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.GiftCard">
     <Value>Gift card</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.DownloadableProduct">
     <Value>Downloadable product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.RecurringProduct">
     <Value>Recurring product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Rental">
     <Value>Rental</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.StockQuantity">
     <Value>Stock qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UseMultipleWarehouses">
     <Value>Multiple warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DisplayStockAvailability">
     <Value>Display availability</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MinStockQuantity">
    <Value>Minimum stock qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.NotifyAdminForQuantityBelow">
    <Value>Notify for qty below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OrderMinimumQuantity">
    <Value>Minimum cart qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OrderMaximumQuantity">
    <Value>Maximum cart qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManageInventoryMethod">
    <Value>Inventory method</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.OldPrice">
    <Value>Old price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Stores.NoStoresAvailable">
    <Value>No stores available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Tabs">
    <Value>Tabs and display options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Seo">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.PurchasedWithOrders">
    <Value>Purchased with orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequireOtherProducts">
    <Value>Require other products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductType.Hint">
   <Value>Product type can be simple or grouped. In most cases your product will have the Simple product type. You need to use Grouped product type when a new product consists of one or more existing products that will be displayed on one single product details page.</Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Products.Fields.VisibleIndividually.Hint">
    <Value>Check it if you want the product to be on catalog or search results. You can uncheck this box to hide associated products from catalog and make them accessible only from grouped product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShortDescription.Hint">
    <Value>Short description is the text that is displayed in product list i.e. сategory / manufacturer pages.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.FullDescription.Hint">
    <Value>Full description is the text that is displayed in product page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AdminComment.Hint">
    <Value>This comment is for internal use only, not visible for customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTags.Hint">
    <Value>Product tags are the keywords for product identification. The more products associated with a particular tag, the larger it will show on the tag cloud.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.MarkAsNew.Hint">
    <Value>Check to mark  the product as new. Use this option for promoting new products.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsGiftCard.Hint">
    <Value>Check if it is a gift card. After adding gift card products to the shopping cart and completing the purchases, you can then search and view the list of all the purchased gift cards by selecting Gift Cards from the Sales menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.GiftCardType.Hint">
    <Value>There are two gift card types: virtual and physical. WARNING: not recommended to change the gift card type from one to another in a "live" store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsDownload.Hint">
    <Value>Check if the product is downloadable. When customers purchase a downloadable product, they can download it direct from your store. The link will be visible after checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Download.Hint">
    <Value>You can download file using URL or uploading from the computer. If you want to download file using URL check the box Use download URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.HasUserAgreement.Hint">
    <Value>Select this checkbox if the customer has a user agreement (a customer must agree with this user agreement when trying to download the product).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.HasSampleDownload.Hint">
    <Value>You can download file using URL or uploading from the computer. If you want to download file using URL check the box Use download URL.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsRecurring.Hint">
    <Value>Check if it is a recurring product. For any product, you can define a recurring cycle to enable the system to automatically create orders that repeat when a customer purchases such products.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringCycleLength.Hint">
    <Value>Specify the cycle length. It is a time period recurring order can be repeated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringCyclePeriod.Hint">
    <Value>Specify the cycle period. It defines units time period can be measured in.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RecurringTotalCycles.Hint">
    <Value>Total cycles are number of times customer will receive the recurring product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Price.Hint">
    <Value>The price of the product. You can manage currency by selecting Configuration > Location > Currencies.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductCost.Hint">
    <Value>Product cost is a prime product cost. This field is only for internal use, not visible for customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPrice.Hint">
    <Value>Set a special price of the product. The new price will be valid between the start and end dates.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SelectedDiscountIds.Hint">
     <Value>Select discouts. You can manage discounts by selecting Discounts from the Promotions menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.TaxCategory.Hint">
    <Value>The tax classification for the product. You can manage tax categories by selecting Configuration > Tax > Tax Categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Hint">
    <Value>Tier pricing is a promotional tool that allows a store owner to price items differently for higher quantities.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManageInventoryMethod.Hint">
    <Value>Select inventory method. There are three methods: Don’t track inventory, Track inventory and Track inventory by attributes. You should use Track inventory by attributes when the product has different combinations of these attributes and then manage inventory for this combinations.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Warehouse.Hint">
    <Value>Choose the warehouse which will be used when calculating shipping rates. You can manage warehouses by selecting  Configuration > Shipping > Warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsShipEnabled.Hint">
    <Value>Check if the product can be shipped. You can manage shipping settings by selecting Configuration > Shipping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Weight.Hint">
    <Value>The product weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Length.Hint">
    <Value>The product length.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Width.Hint">
    <Value>The product width.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShipSeparately.Hint">
    <Value>Check if the product should be shipped separately from other products (in single box). Notice that if the order includes several items of this product, all of them will be shipped separately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Vendor.Hint">
    <Value>Choose the vendor. You can manage vendors by selecting Customers > Vendors.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequireOtherProducts.Hint">
    <Value>Check if the product requires adding other products to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.RelatedProducts.Hint">
    <Value>The Related Products option provides the opportunity to advertise products that are not part of the selected category, to your visitors. These products are displayed on the product details pages.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.CrossSells.Hint">
    <Value>The Cross-sell products option provides the opportunity to buy additional products that generally go with the selected product. They are displayed at the bottom of the checkout page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Hint">
    <Value>Product attributes are quantifiable or descriptive aspects of a product (such as, color). For example, if you were to create an attribute for color, with the values of blue, green, yellow, and so on, you may want to apply this attribute to shirts, which you sell in various colors (you can adjust a price or weight for any of existing attribute values). You can add attribute for your product using existing list of attributes, or if you need to create a new one go to Catalog > Attributes > Product attributes. Please notice that if you want to manage inventory by product attributes (e.g. 5 green shirts and 3 blue ones), then ensure that Inventory method is set to Track inventory by product attributes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Hint">
    <Value>Specification attributes are product features i.e, screen size, number of USB-ports, visible on product details page. Specification attributes can be used for filtering products on the category details page.  Unlike product attributes, specification attributes are used for information purposes only.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DeliveryDate.Hint">
    <Value>Choose a delivery date which will be displayed in the public store. You can manage delivery dates by selecting Configuration > Shipping > Delivery dates.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Height.Hint">
    <Value>The product height.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SubjectToAcl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SubjectToAcl.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Description2">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Description">
    <Value>Also note that some attribute control types that support custom user input (e.g. file upload, textboxes, date picker) are useless with attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.CustomerRole">
     <Value>Limited to customer role</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.CustomerRole.Hint">
     <Value>Choose a customer role which subscribers will get this email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.List.Stores">
     <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.List.Stores.Hint">
     <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.IsCumulative">
    <Value>Cumulative with other discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.IsCumulative.Hint">
    <Value>If checked, this discount can be used with other ones simultaneously. Please note that this feature works only for discounts with the same discount type. Right now discounts with distinct types are already cumulative.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ShipToSameAddress">
    <Value>Ship to the same address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ShipToSameAddress.Hint">
    <Value>Check to display "ship to the same address" option during checkout ("billing address" step). In this case case "shipping address" with appropriate options (e.g. pick up in store) will be skipped. Also note that all billing countries should support shipping ("Allow shipping" checkbox ticked).</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.ShipToSameAddress">
    <Value>Ship to the same address</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Url">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Url.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Port">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Port.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.CustomerId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.CustomerId.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Api">
    <Value>API key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.Api.Hint">
    <Value>Specify Canada Post API key.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.CustomerNumber">
    <Value>Customer number</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.CustomerNumber.Hint">
    <Value>Specify customer number.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.UseSandbox">
    <Value>Use Sandbox</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.UseSandbox.Hint">
    <Value>Check to enable Sandbox (testing environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard">
    <Value>Standard</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Manage">
    <Value>Manage shipping methods</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Restrictions.Manage">
    <Value>Shipping method restrictions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Providers">
    <Value>Shipping providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Providers.Title">
    <Value>Shipping rate computation methods (providers)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapEnabled">
    <Value>Sitemap enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapEnabled.Hint">
    <Value>Check to enable sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeCategories">
    <Value>Sitemap includes categories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeCategories.Hint">
    <Value>Check if you want to include categories in sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeManufacturers">
    <Value>Sitemap includes manufacturers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeManufacturers.Hint">
    <Value>Check if you want to include manufacturers in sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeProducts">
    <Value>Sitemap includes products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SitemapIncludeProducts.Hint">
    <Value>Check if you want to include products in sitemap.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CustomHeadTags">
    <Value><![CDATA[Custom <head> tag]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CustomHeadTags.Hint">
    <Value><![CDATA[Enter a custom <head> tag(s) here. For example, some custom <meta> tag. Or leave empty if ignore this setting.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AdminAccountShouldExists.Deactivate">
    <Value>You can''t deactivate the last administrator. At least one administrator account should exists.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AdminAccountShouldExists.DeleteRole">
    <Value>You can''t remove the Administrator role. At least one administrator account should exists.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator">
    <Value>You can''t delete the last administrator. At least one administrator account should exists.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditActivityLogTypes">
    <Value>Edited activity log types</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteActivityLog">
    <Value>Deleted activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning">
    <Value>Quantity has been changed while you were editing the product. Changes haven''t been saved. Please ensure that everything is correct and click "Save" button one more time.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.AdditionalInfo">
    <Value>Additional info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreStoreLimitations.Notification">
    <Value>In order to use this functionality you have to disable the following setting: Configuration > Catalog settings > Ignore "limit per store" rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreAcl.Notification">
    <Value>In order to use this functionality you have to disable the following setting: Configuration > Catalog settings > Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ImportFromExcel.ManyRecordsWarning">
    <Value>Import requires a lot of memory resources. That''s why it''s not recommended to import more than 500 - 1,000 records at once. If you have more records, it''s better to split them to multiple Excel files and import separately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Description">
    <Value>Note that some attribute control types that support custom user input (e.g. file upload, textboxes, date picker) are useless with attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Logo">
    <Value>Logo</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Logo.Hint">
    <Value>Upload your store logo. If not uploaded, then the default one will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.OnlyAdminCanChangePassword">
    <Value>You''re not allowed to change passwords of administrators. Only administrators can do it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.OnlyAdminCanDeleteAdmin">
    <Value>You''re not allowed to delete administrators. Only administrators can do it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.CustomerRoles">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.CustomerRoles.NoRoles">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.CustomerRoles.Hint">
    <Value>Choose customer roles of this user</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.CustomerRoles.NoRoles">
    <Value>No customer roles available. Create at least one customer role before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.SpecificationAttributes.Description">
    <Value>Specification attributes are product features i.e, screen size, number of USB-ports, visible on product details page. Specification attributes can be used for filtering products on the category details page. Unlike product attributes, specification attributes are used for information purposes only. You can add attributes to existing product on a product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.Description">
    <Value>Product attributes are quantifiable or descriptive aspects of a product (such as, color). For example, if you were to create an attribute for color, with the values of blue, green, yellow, and so on, you may want to apply this attribute to shirts, which you sell in various colors (you can adjust a price or weight for any of existing attribute values). You can add attributes to existing product on a product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Description">
    <Value>Checkout attributes are displayed on the shopping cart page and provide the opportunity to offer more services to customers, i.e. gift wrapping, before placing the order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore.Hint">
    <Value>A value indicating whether "Pick Up in Store" option is enabled during checkout. Please ensure that you have at least one active pickup point provider.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayPickupPointsOnMap">
    <Value>Display pickup points on the map</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayPickupPointsOnMap.Hint">
    <Value>Check to display pickup points on the map.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.GoogleMapsApiKey">
    <Value>Google maps API key</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.GoogleMapsApiKey.Hint">
    <Value>Specify Google maps API key.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.PickUpInStoreFee">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.PickUpInStoreFee.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders">
    <Value>Pickup point providers</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.BackToList">
    <Value>back to pickup point provider list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.Configure">
    <Value>Configure</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.Fields.FriendlyName">
    <Value>Friendly name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.Fields.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.Fields.Logo">
    <Value>Logo</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPointProviders.Fields.SystemName">
    <Value>System name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.PickupPoints">
    <Value>Pickup points</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PickupAddress">
    <Value>Pickup point address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PickupAddress.Hint">
    <Value>Pickup point address info.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PickupAddress.ViewOnGoogleMaps">
    <Value>View address on Google Maps</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStoreAndFee">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore.MethodName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickupPoints">
    <Value>Pickup</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickupPoints.Description">
    <Value>Pick up your items at the store</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickupPoints.Name">
    <Value>Pickup at {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickupPoints.NotAvailable">
    <Value>Pickup points could not be loaded</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickupPoints.SelectPickupPoint">
    <Value>Select pickup point</Value>
  </LocaleResource>
  <LocaleResource Name="Order.PickupAddress">
    <Value>Pickup point address</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipments.PickupAddress">
    <Value>Pickup point address</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.Pickup">
    <Value>Pickup point:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ActionConfirmation">
    <Value>Are you sure you want to perform this action?</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Header.Logout">
    <Value>Logout</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures">
    <Value>Measures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions">
    <Value>Dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.CantDeletePrimary">
    <Value>The primary dimension can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Description">
    <Value>NOTE: if you change your primary dimension, then do not forget to update the appropriate ratios of the units</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.IsPrimaryDimension">
    <Value>Is primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.MarkAsPrimaryDimension">
    <Value>Mark as primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.Ratio">
    <Value>Ratio to primary dimension</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword">
    <Value>System keyword</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword.Required">
    <Value>Please provide a system keyword.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights">
    <Value>Weights</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.CantDeletePrimary">
    <Value>The primary weight can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Description">
    <Value>NOTE: if you change your primary weight, then do not forget to update the appropriate ratios of the units</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.IsPrimaryWeight">
    <Value>Is primary weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.MarkAsPrimaryWeight">
    <Value>Mark as primary weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio">
    <Value>Ratio to primary weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword">
    <Value>System keyword</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword.Required">
    <Value>Please provide a system keyword.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.CantDeletePrimary">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.IsPrimaryDimension">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.MarkAsPrimaryDimension">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.Name.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.Ratio">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.SystemKeyword">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.Fields.SystemKeyword.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.CantDeletePrimary">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.IsPrimaryWeight">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.MarkAsPrimaryWeight">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.Name.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.Ratio">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.SystemKeyword">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.Fields.SystemKeyword.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Permission.ManageMeasures">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Info">
    <Value>Customer info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ActivityLog">
    <Value>Activity log</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Impersonate">
    <Value>Place order (impersonate)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Info">
    <Value>Product info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Info">
    <Value>Category info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Info">
    <Value>Manufacturer info</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.Discounts">
    <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.Discounts.Hint">
    <Value>Select discounts to apply to this category. You can manage discounts by selecting Discounts from the Promotions menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.Discounts.NoDiscounts">
    <Value>No discounts available. Create at least one discount before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Discounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Discounts.NoDiscounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.Discounts">
    <Value>Discounts</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.Discounts.Hint">
    <Value>Select discounts to apply to this manufacturer. You can manage discounts by selecting Discounts from the Promotions menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.Discounts.NoDiscounts">
    <Value>No discounts available. Create at least one discount before mapping.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Discounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Discounts.NoDiscounts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Stores.NoStoresAvailable">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ManufacturerIds">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.SubjectToAcl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.SubjectToAcl.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AclCustomerRoles">
    <Value>Limited to customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the category will be shown. Leave empty if you want this category to be visible to all users.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Acl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.SubjectToAcl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.SubjectToAcl.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AclCustomerRoles">
    <Value>Limited to customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the manufacturer will be shown. Leave empty if you want this manufacturer to be visible to all users.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Acl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SubjectToAcl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SubjectToAcl.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Acl">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the topic will be shown. Leave empty if you want this topic to be visible to all users.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.LimitedToStores.Hint">
    <Value>Option to limit this product to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty. In order to use this functionality you have to disable the following setting: Catalog settings > Ignore "limit per store" rules.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.LimitedToStores.Hint">
    <Value>Option to limit this blog post to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.LimitedToStores.Hint">
    <Value>Option to limit this category to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.LimitedToStores.Hint">
    <Value>Option to limit this manufacturer to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.LimitedToStores.Hint">
    <Value>Option to limit this country to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.LimitedToStores.Hint">
    <Value>Option to limit this currency to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Info">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.LimitedToStores.Hint">
    <Value>Option to limit this language to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Info">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores.Hint">
    <Value>Option to limit this template to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.LimitedToStores.Hint">
    <Value>Option to limit this news item to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.LimitedToStores.Hint">
    <Value>Option to limit this topic to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores.Hint">
    <Value>Option to limit this attribute to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.AvailableStores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.AvailableStores.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Stores">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.LimitedToStores.Hint">
    <Value>Option to limit this plugin to a certain store. If you have multiple stores, choose one or several from the list. If you don''t use this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreStoreLimitations.Notification">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreAcl.Notification">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ShippingOriginAddress.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxAddress.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxAddress">
    <Value>Default tax address (used for tax calculation)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ShippingOriginAddress">
    <Value>Shipping origin</Value>
  </LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubtotal">
		<Value>Order subtotal</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubtotal.Hint">
		<Value>The subtotal of this order.</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubtotalExclTax">
		<Value>excl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubtotalExclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubtotalInclTax">
		<Value>incl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubtotalInclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderShipping">
		<Value>Order shipping</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderShipping.Hint">
		<Value>The total shipping cost for this order.</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderShippingExclTax">
		<Value>excl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderShippingExclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderShippingInclTax">
		<Value>incl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderShippingInclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubTotalDiscount">
		<Value>Order subtotal discount</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubTotalDiscount.Hint">
		<Value>The subtotal discount of this order.</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubTotalDiscountExclTax">
		<Value>excl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubTotalDiscountExclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubTotalDiscountInclTax">
		<Value>incl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.OrderSubTotalDiscountInclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.PaymentMethodAdditionalFee">
		<Value>Payment method additional fee</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.PaymentMethodAdditionalFee.Hint">
		<Value>The payment method additional fee for this order.</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.PaymentMethodAdditionalFeeExclTax">
		<Value>excl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.PaymentMethodAdditionalFeeExclTax.Hint">
		<Value></Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.PaymentMethodAdditionalFeeInclTax">
		<Value>incl tax</Value>
	</LocaleResource>
	<LocaleResource Name="Admin.Orders.Fields.PaymentMethodAdditionalFeeInclTax.Hint">
		<Value></Value>
	</LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreStoreLimitations.Notification">
    <Value>In order to use this functionality you have to disable the following setting: Catalog settings > Ignore "limit per store" rules.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Performance.IgnoreAcl.Notification">
    <Value>In order to use this functionality you have to disable the following setting: Catalog settings > Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Description">
    <Value>The Return Request feature (RMA) enables customers to send products back to you. Here you can find all submitted return requests.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.Description">
    <Value>Recurring payments are used for automatic renewal of consumable merchandise or subscription services.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ACL.Description">
    <Value>Access control list is a list of permissions attached to customer roles. This list specifies the access rights of users to objects.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Description">
    <Value>Affiliate is an Internet-based marketing practice in which a business rewards one or more affiliates for each visitor or customer. It is a web-based pay-for-performance program designed to compensate affiliate partner for driving qualified leads or sales to a merchant web site.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.OneColumnProductPage">
    <Value>One column product page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.OneColumnProductPage">
    <Value>One column product page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.AutoUpdateOrderTotalsOnEditingOrder">
    <Value>Auto update order totals</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.AutoUpdateOrderTotalsOnEditingOrder.Hint">
    <Value>Check to automatically update order totals on editing an order in admin area. IMPORANT: currently this functionality is in BETA testing status.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductAttributes">
    <Value>Export/Import products with attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductAttributes.Hint">
    <Value>Check if products should be exported/imported with product attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ProductAttributes">
    <Value>Product attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecificationAttributes">
    <Value>Specification attributes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.TaxCategory.Hint">
    <Value>The tax classification for this attribute (used to calculate tax). You can manage tax categories by selecting Configuration : Tax : Tax Categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeTaxClass">
    <Value>Payment method additional fee tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeTaxClass.Hint">
    <Value>Select tax category used for payment method additional fee tax calculation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.ShippingTaxClass">
    <Value>Shipping tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.ShippingTaxClass.Hint">
    <Value>Select tax category used for shipping tax calculation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Mode.Basic">
    <Value>Basic</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Mode.Advanced">
    <Value>Advanced</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Manufacturers">
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



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductAttributeValue]') and NAME='ImageSquaresPictureId')
BEGIN
	ALTER TABLE [ProductAttributeValue]
	ADD [ImageSquaresPictureId] int NULL
END
GO

UPDATE [ProductAttributeValue]
SET [ImageSquaresPictureId] = 0
WHERE [ImageSquaresPictureId] IS NULL
GO

ALTER TABLE [ProductAttributeValue] ALTER COLUMN [ImageSquaresPictureId] int NOT NULL
GO

--new column
 IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReview]') and NAME='StoreId')
 BEGIN
 	ALTER TABLE [dbo].[ProductReview] ADD
 	[StoreId] int NULL
 END
 GO

 DECLARE @DefaultStoreId INT
 SET @DefaultStoreId = (SELECT TOP (1) Id FROM [dbo].[Store]);
 UPDATE [dbo].[ProductReview] SET StoreId = @DefaultStoreId WHERE StoreId IS NULL
 GO
 
 ALTER TABLE [dbo].[ProductReview] ALTER COLUMN [StoreId] INT NOT NULL
 GO
 
 IF EXISTS (SELECT 1 FROM   sys.objects WHERE  
 			name = 'ProductReview_Store'
 			AND parent_object_id = Object_id('ProductReview')
 			AND Objectproperty(object_id,N'IsForeignKey') = 1)
 ALTER TABLE dbo.ProductReview
 DROP CONSTRAINT ProductReview_Store
 GO
 
 ALTER TABLE [dbo].[ProductReview]  WITH CHECK ADD  CONSTRAINT [ProductReview_Store] FOREIGN KEY([StoreId])
 REFERENCES [dbo].[Store] ([Id])
 ON DELETE CASCADE
 GO
 
--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showproductreviewsperstore')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'catalogsettings.showproductreviewsperstore', N'False', 0)
 END
 GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.imagesquarepicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'mediasettings.imagesquarepicturesize', N'32', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.importproductimagesusinghash')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'mediasettings.importproductimagesusinghash', N'true', 0)
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewTopic')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewTopic', N'Add a new topic', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteTopic')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteTopic', N'Delete a topic', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditTopic')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditTopic', N'Edit a topic', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteOrder')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteOrder', N'Delete an order', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditOrder')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditOrder', N'Edit an order', N'true')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.pagesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.pagesize', N'10', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsortingenumdisabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'catalogsettings.productsortingenumdisabled',N'',0);
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsortingenumdisplayorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'catalogsettings.productsortingenumdisplayorder',N'',0);
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchaversion')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'captchasettings.recaptchaversion',N'1',0);
END
GO

--new or update setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchatheme')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'captchasettings.recaptchatheme',N'',0);
END
ELSE
BEGIN
	UPDATE [Setting] 
	SET [Value] = N'' 
	WHERE [Name] = N'captchasettings.recaptchatheme'
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchalanguage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'captchasettings.recaptchalanguage',N'',0);
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.maximumproductnumber')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'vendorsettings.maximumproductnumber',N'3000',0);
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='DontSendBeforeDateUtc')
BEGIN
	ALTER TABLE [QueuedEmail]
	ADD [DontSendBeforeDateUtc] DATETIME NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Campaign]') and NAME='DontSendBeforeDateUtc')
BEGIN
	ALTER TABLE [Campaign]
	ADD [DontSendBeforeDateUtc] DATETIME NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') and NAME='DelayBeforeSend')
BEGIN
	ALTER TABLE [MessageTemplate]
	ADD [DelayBeforeSend] INT NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') and NAME='DelayPeriodId')
BEGIN
	ALTER TABLE [MessageTemplate]
	ADD [DelayPeriodId] INT NULL
END
GO

UPDATE [MessageTemplate]
SET [DelayPeriodId] = 0
WHERE [DelayPeriodId] IS NULL
GO

ALTER TABLE [MessageTemplate] ALTER COLUMN [DelayPeriodId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowsearchbyvendor')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'vendorsettings.allowsearchbyvendor',N'False',0);
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='Published')
BEGIN
	ALTER TABLE [Topic]
	ADD [Published] bit NULL
END
GO

UPDATE [Topic]
SET [Published] = 1
WHERE [Published] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [Published] bit NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='ConditionAttributeXml')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [ConditionAttributeXml] nvarchar(MAX) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.deleteguesttaskolderthanminutes')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'commonsettings.deleteguesttaskolderthanminutes',N'1440',0);
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[SpecificationAttributeOption]') and NAME='ColorSquaresRgb')
BEGIN
	ALTER TABLE [SpecificationAttributeOption]
	ADD [ColorSquaresRgb] nvarchar(100) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.enteringemailtwice')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'customersettings.enteringemailtwice',N'False',0);
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'pdfsettings.fontfilename')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'pdfsettings.fontfilename', N'FreeSerif.ttf', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ActivityLog]') and NAME='IpAddress')
BEGIN
	ALTER TABLE [ActivityLog]
	ADD [IpAddress] nvarchar(200) NULL
END
GO

--a stored procedure update
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
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
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
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
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--show hidden and ACL
	IF @ShowHidden = 0
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
	
	--show hidden and filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT [SpecificationAttributeOptionId]
		FROM [#FilteredSpecs]
		OPEN cur_SpecificationAttributeOption
		FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeOptionId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId = ' + CAST(@SpecificationAttributeOptionId AS nvarchar(max)) + ')'
			--fetch next identifier
			FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeOptionId
		END
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
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
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

	--prepare filterable specification attribute option identifier (if requested)
	IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
		INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
		SELECT DISTINCT [psam].SpecificationAttributeOptionId
		FROM [Product_SpecificationAttribute_Mapping] [psam] with (NOLOCK)
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

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
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		CREATE FULLTEXT INDEX ON [LocalizedProperty]([LocaleValue])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('LocalizedProperty') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		CREATE FULLTEXT INDEX ON [ProductTag]([Name])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('ProductTag') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
END
GO

--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'taxsettings.logerrors')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'taxsettings.logerrors', N'True', 0)
 END
 GO



 --new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='NotReturnable')
BEGIN
	ALTER TABLE [Product]
	ADD [NotReturnable] bit NULL
END
GO

UPDATE [Product]
SET [NotReturnable] = 0
WHERE [NotReturnable] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [NotReturnable] bit NOT NULL
GO


--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'Impersonation.Started')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'Impersonation.Started', N'Customer impersonation session. Started', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'Impersonation.Finished')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'Impersonation.Finished', N'Customer impersonation session. Finished', N'true')
END
GO

 --new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Forums_PostVote]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[Forums_PostVote](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ForumPostId] [int] NOT NULL,
        [CustomerId] [int] NOT NULL,
		[IsUp] [bit] NOT NULL,
		[CreatedOnUtc] [datetime] NOT NULL
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'Forums_PostVote_Forums_Post' AND parent_object_id = Object_id('Forums_PostVote') AND Objectproperty(object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE dbo.Forums_PostVote
    DROP CONSTRAINT Forums_PostVote_Forums_Post
END
GO

ALTER TABLE [dbo].[Forums_PostVote] WITH CHECK ADD CONSTRAINT [Forums_PostVote_Forums_Post] FOREIGN KEY([ForumPostId])
REFERENCES [dbo].[Forums_Post] ([Id])
ON DELETE CASCADE
GO

 --new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Forums_Post]') and NAME='VoteCount')
BEGIN
	ALTER TABLE [Forums_Post]
	ADD [VoteCount] int NULL
END
GO

UPDATE [Forums_Post]
SET [VoteCount] = 0
WHERE [VoteCount] IS NULL
GO

ALTER TABLE [Forums_Post] ALTER COLUMN [VoteCount] int NOT NULL
GO

--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'forumsettings.allowpostvoting')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'forumsettings.allowpostvoting', N'True', 0)
 END
 GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'forumsettings.maxvotesperday')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'forumsettings.maxvotesperday', N'30', 0)
 END
 GO

IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductLoadAllPaged]
GO

CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
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
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
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
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	--show hidden and ACL
	IF @ShowHidden = 0
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
	
	--show hidden and filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT [SpecificationAttributeOptionId]
		FROM [#FilteredSpecs]
		OPEN cur_SpecificationAttributeOption
		FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeOptionId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId = ' + CAST(@SpecificationAttributeOptionId AS nvarchar(max)) + ')'
			--fetch next identifier
			FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeOptionId
		END
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
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
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

	--prepare filterable specification attribute option identifier (if requested)
	IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
		INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
		SELECT DISTINCT [psam].SpecificationAttributeOptionId
		FROM [Product_SpecificationAttribute_Mapping] [psam] with (NOLOCK)
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

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
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.popupgridpagesize')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'adminareasettings.popupgridpagesize', N'10', 0)
 END
 GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.requireregistrationfordownloadableproducts')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'customersettings.requireregistrationfordownloadableproducts', N'False', 0)
 END
 GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showproductreviewstabonaccountpage')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'catalogsettings.showproductreviewstabonaccountpage', N'True', 0)
 END
 GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productreviewspagesizeonaccountpage')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'catalogsettings.productreviewspagesizeonaccountpage', N'10', 0)
 END
 GO

--delete some settings
 DELETE FROM [Setting]
 WHERE [name] = N'catalogsettings.IgnoreDiscounts' and [StoreId] > 0
 GO
 
 DELETE FROM [Setting]
 WHERE [name] = N'catalogsettings.IgnoreFeaturedProducts' and [StoreId] > 0
 GO
 
 DELETE FROM [Setting]
 WHERE [name] = N'catalogsettings.IgnoreAcl' and [StoreId] > 0
 GO
 
 DELETE FROM [Setting]
 WHERE [name] = N'catalogsettings.IgnoreStoreLimitations' and [StoreId] > 0
 GO
 
 DELETE FROM [Setting]
 WHERE [name] = N'catalogsettings.CacheProductPrices' and [StoreId] > 0
 GO

  --a stored procedure update
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [ProductLoadAllPaged]
GO

CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
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
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
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
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(p.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(p.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(p.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(p.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
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

 --update message templates
 UPDATE [MessageTemplate] SET [Body] = REPLACE([Body], 'ReturnRequest.ID', 'ReturnRequest.CustomNumber')
 GO

  --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.returnrequestnumbermask')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'ordersettings.returnrequestnumbermask', N'{ID}', 0)
 END
 GO

 --new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ReturnRequest]') and NAME='CustomNumber')
BEGIN
	ALTER TABLE [ReturnRequest]
	ADD [CustomNumber] NVARCHAR(MAX) NULL
END
GO

 UPDATE [ReturnRequest] SET [CustomNumber] = CAST([Id] AS NVARCHAR(200)) WHERE [CustomNumber] IS NULL OR [CustomNumber] = N''
 GO

 ALTER TABLE [ReturnRequest] ALTER COLUMN [CustomNumber] NVARCHAR(MAX) NOT NULL
 GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowvendorstoeditinfo')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'vendorsettings.allowvendorstoeditinfo', N'False', 0)
 END
 GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.notifystoreowneraboutvendorinformationchange')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'vendorsettings.notifystoreowneraboutvendorinformationchange', N'True', 0)
 END
 GO

 -- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'VendorInformationChange.StoreOwnerNotification')
 BEGIN
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'VendorInformationChange.StoreOwnerNotification', NULL, N'%Store.Name%. Vendor information change.', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Vendor %Vendor.Name% (%Vendor.Email%) has just changed information about itself.</p>', 1, 0, 0, 0, 0)
 END
 GO

  --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.GeneratePdfInvoiceInCustomerLanguage')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'ordersettings.GeneratePdfInvoiceInCustomerLanguage', N'true', 0)
 END
 GO

 --new column
 IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Campaign]') and NAME='CustomerRoleId')
 BEGIN
 	ALTER TABLE [Campaign]
	ADD [CustomerRoleId] INT NULL
 END
 GO

 UPDATE [Campaign]
 SET [CustomerRoleId] = 0
 WHERE [CustomerRoleId] IS NULL
 GO

 ALTER TABLE [Campaign] ALTER COLUMN [CustomerRoleId] INT NOT NULL
 GO
 GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='IsCumulative')
BEGIN
	ALTER TABLE [Discount]
	ADD [IsCumulative] bit NULL
END
GO

UPDATE [Discount]
SET [IsCumulative] = 0
WHERE [IsCumulative] IS NULL
GO

ALTER TABLE [Discount] ALTER COLUMN [IsCumulative] bit NOT NULL
GO
 
--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.shiptosameaddress')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'shippingsettings.shiptosameaddress', N'False', 0)
END
GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.customheadtags')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'seosettings.customheadtags', N'', 0)
 END
 GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditActivityLogTypes')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditActivityLogTypes', N'Edit activity log types', N'true')
END
 GO
 
--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteActivityLog')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteActivityLog', N'Delete activity log', N'true')
END
GO

--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.logopictureid')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'storeinformationsettings.logopictureid', N'0', 0)
 END
 GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'shippingsettings.pickupinstorefee'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.displaypickuppointsonmap')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.displaypickuppointsonmap', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.googlemapsapikey')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.googlemapsapikey', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.activepickuppointprovidersystemnames')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.activepickuppointprovidersystemnames', N'', 0)
END
GO

 --new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='PickupAddressId')
BEGIN
	ALTER TABLE [Order]
	ADD [PickupAddressId] int NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'Order_PickupAddress' AND parent_object_id = Object_id('Order') AND Objectproperty(object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE [dbo].[Order]
    DROP CONSTRAINT [Order_PickupAddress]
END
GO

ALTER TABLE [dbo].[Order] WITH CHECK ADD CONSTRAINT [Order_PickupAddress] FOREIGN KEY([PickupAddressId])
REFERENCES [dbo].[Address] ([Id])
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.hideadvertisementsonadminarea')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'adminareasettings.hideadvertisementsonadminarea', N'false', 0)
END
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'commonsettings.hideadvertisementsonadminarea'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.deleteguesttaskolderthanminutes')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.deleteguesttaskolderthanminutes', N'1440', 0)
END
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'commonsettings.deleteguesttaskolderthanminutes'
GO

--delete permission
DELETE FROM [PermissionRecord]
WHERE [SystemName] = N'ManageMeasures'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.autoupdateordertotalsoneditingorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.autoupdateordertotalsoneditingorder', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'producteditorsettings.weight')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'producteditorsettings.weight', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'producteditorsettings.dimensions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'producteditorsettings.dimensions', N'true', 0)
END
GO

--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportproductattributes')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'catalogsettings.exportimportproductattributes', N'True', 0)
 END
 GO
 
 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'producteditorsettings.productattributes')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'producteditorsettings.productattributes', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'producteditorsettings.specificationattributes')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'producteditorsettings.specificationattributes', N'true', 0)
END
GO
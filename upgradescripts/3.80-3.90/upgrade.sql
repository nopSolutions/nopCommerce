--upgrade scripts from nopCommerce 3.80 to 3.90

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Account.CustomerProductReviews.NoRecords">
    <Value>You haven''t written any reviews yet</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.EnteringEmailTwice.Hint">
    <Value>Force entering email twice during registration.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.MaximumProductNumber.Hint">
    <Value>Sets a maximum number of products per vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Description">
    <Value>Shipping methods used by offline shipping rate computation methods (e.g. "Fixed Rate Shipping" or "Shipping by weight").</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Price.Hint">
    <Value>The price of the product. You can manage currency by selecting Configuration > Currencies.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.CustomerRoles.Hint">
    <Value>Choose customer roles of this user.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductAttributes.Hint">
    <Value>Check if products should be exported/imported with product attributes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ShipToSameAddress.Hint">
    <Value>Check to display "ship to the same address" option during checkout ("billing address" step). In this case "shipping address" with appropriate options (e.g. pick up in store) will be skipped. Also note that all billing countries should support shipping ("Allow shipping" checkbox ticked).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.24days">
    <Value>Task period should not exceed 24 days.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Payment.MethodRestrictions">
    <Value>Payment restrictions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Vendor.Hint">
    <Value>Choose a vendor associated with this product. This can be useful when running a multi-vendor store to keep track of goods associated with vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ImportTip">
    <Value>You can download a CSV file with a list of states for other countries on the following page:</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTags.Placeholder">
    <Value>Enter tags ...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.Parent.None">
    <Value>[None]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.HideShippingTotal">
    <Value>Hide shipping total if shipping is not required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.HideShippingTotal.Hint">
    <Value>Check if you want to hide ''Shipping total'' label when shipping is not required.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountName.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.Signature">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.Signature.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ClientId">
    <Value>Client ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ClientId.Hint">
    <Value>Specify client ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ClientSecret">
    <Value>Client secret</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.ClientSecret.Hint">
    <Value>Specify secret key.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.WebhookId">
    <Value>Webhook ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.WebhookId.Hint">
    <Value>Specify webhook ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.WebhookCreate">
    <Value>Get webhook ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.WebhookError">
    <Value>Webhook was not created (see details in the log)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.TaxCategories.None">
    <Value>[None]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxCategory">
    <Value>Default tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxCategory.Hint">
    <Value>Select default tax category for products.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewAddressAttribute">
    <Value>Added a new address attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewAddressAttributeValue">
    <Value>Added a new address attribute value (ID = {0})</Value>
  </LocaleResource>    
  <LocaleResource Name="ActivityLog.AddNewAffiliate">
    <Value>Added a new affiliate (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewBlogPost">
    <Value>Added a new blog post (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCampaign">
    <Value>Added a new campaign (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCountry">
    <Value>Added a new country (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCurrency">
    <Value>Added a new currency (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCustomerAttribute">
    <Value>Added a new customer attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewCustomerAttributeValue">
    <Value>Added a new customer attribute value (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewEmailAccount">
    <Value>Added a new email account (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewLanguage">
    <Value>Added a new language (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewMeasureDimension">
    <Value>Added a new measure dimension (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewMeasureWeight">
    <Value>Added a new measure weight (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewNews">
    <Value>Added a new news (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.InstallNewPlugin">
    <Value>Installed a new plugin (FriendlyName: ''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewStateProvince">
    <Value>Added a new state province (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewStore">
    <Value>Added a new store (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewVendor">
    <Value>Added a new vendor (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewWarehouse">
    <Value>Added a new warehouse (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteAddressAttribute">
    <Value>Deleted an address attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteAddressAttributeValue">
    <Value>Deleted an address attribute value (ID = {0})</Value>
  </LocaleResource>  
  <LocaleResource Name="ActivityLog.DeleteAffiliate">
    <Value>Deleted an affiliate (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteBlogPost">
    <Value>Deleted a blog post (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteBlogPostComment">
    <Value>Deleted a blog post comment (ID = {0})</Value>
  </LocaleResource>  
  <LocaleResource Name="ActivityLog.DeleteCampaign">
    <Value>Deleted a campaign (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCountry">
    <Value>Deleted a country (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCurrency">
    <Value>Deleted a currency (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCustomerAttribute">
    <Value>Deleted a customer attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteCustomerAttributeValue">
    <Value>Deleted a customer attribute value (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteEmailAccount">
    <Value>Deleted an email account (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteLanguage">
    <Value>Deleted a language (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteMeasureDimension">
    <Value>Deleted a measure dimension (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteMeasureWeight">
    <Value>Deleted a measure weight (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteMessageTemplate">
    <Value>Deleted a message template (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteNews">
    <Value>Deleted a news (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteNewsComment">
    <Value>Deleted a news comment (ID = {0})</Value>
  </LocaleResource>  
  <LocaleResource Name="ActivityLog.UninstallPlugin">
    <Value>Uninstalled a plugin (FriendlyName: ''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteProductReview">
    <Value>Deleted a product revie (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteStateProvince">
    <Value>Deleted a state or province (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteStore">
    <Value>Deleted a store (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteVendor">
    <Value>Deleted a vendor (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteWarehouse">
    <Value>Deleted a warehouse (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditAddressAttribute">
    <Value>Edited an address attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditAddressAttributeValue">
    <Value>Edited an address attribute value (ID = {0})</Value>
  </LocaleResource>  
  <LocaleResource Name="ActivityLog.EditAffiliate">
    <Value>Edited an affiliate (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditBlogPost">
    <Value>Edited a blog post (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCampaign">
    <Value>Edited a campaign (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCountry">
    <Value>Edited a country (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCurrency">
    <Value>Edited a currency (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCustomerAttribute">
    <Value>Edited a customer attribute (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditCustomerAttributeValue">
    <Value>Edited a customer attribute value (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditEmailAccount">
    <Value>Edited an email account (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditLanguage">
    <Value>Edited a language (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditMeasureDimension">
    <Value>Edited a measure dimension (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditMeasureWeight">
    <Value>Edited a measure weight (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditMessageTemplate">
    <Value>Edited a message template (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditNews">
    <Value>Edited a news (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditPlugin">
    <Value>Edited a plugin (FriendlyName: ''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditProductReview">
    <Value>Edited a product revie (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditStateProvince">
    <Value>Edited a state or province (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditStore">
    <Value>Edited a store (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditTask">
    <Value>Edited a task (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditVendor">
    <Value>Edited a vendor (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditWarehouse">
    <Value>Edited a warehouse (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewPossibleOnlyAfterPurchasing">
    <Value>Product review possible only after purchasing product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewPossibleOnlyAfterPurchasing.Hint">
    <Value>Check if product can be reviewed only by customer who have already ordered it.</Value>
  </LocaleResource>
  <LocaleResource Name="Reviews.ProductReviewPossibleOnlyAfterPurchasing">
    <Value>Product can be reviewed only after purchasing it</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExchangeRate.EcbExchange.SetCurrencyToEURO">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExchangeRate.EcbExchange.Error">
    <Value>You can use ECB (European central bank) exchange rate provider only when the primary exchange rate currency is supported by ECB</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.AttachedDownload">
    <Value>Attached static file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.AttachedDownload.Hint">
    <Value>The attached static file that will be sent in this email.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.ContractId">
    <Value>Contract ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Fields.ContractId.Hint">
    <Value>Specify contract identifier.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductReviewPossibleOnlyAfterPurchasing">
    <Value>Product review possible only after product purchasing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSize.Positive">
    <Value>Page size should have a positive value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSize.Positive">
    <Value>Page size should have a positive value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSize.Positive">
    <Value>Page size should have a positive value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.Placeholder">
    <Value>Enter tags ...</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductSku">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductSku.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowSkuOnCatalogPages">
    <Value>Show SKU on catalog pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowSkuOnCatalogPages.Hint">
    <Value>Check to show product SKU on catalog pages in public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowSkuOnProductDetailsPage">
    <Value>Show SKU on product details page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowSkuOnProductDetailsPage.Hint">
    <Value>Check to show product SKU on the product details page in public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CancelCC">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CancelOrderTotals">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.CustomerEntersQty">
    <Value>Customer enters quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.CustomerEntersQty.Hint">
    <Value>Allow customers enter the quantity of associated product.</Value>
  </LocaleResource>
  <LocaleResource Name="ProductAttributes.Quantity">
    <Value> - quantity {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductAttributes.PriceAdjustment">
    <Value>{0} [{1}{2}]</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductAttributes.PriceAdjustment.PerItem">
    <Value> per item</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductAttributes.PriceAdjustment.Quantity">
    <Value>Enter quantity:</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.EmailAccount">
    <Value>Email account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.EmailAccount.Hint">
    <Value>The email account that will be used to send this campaign.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.AclCustomerRoles">
    <Value>Limited to customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.AclCustomerRoles.Hint">
    <Value>Choose one or several customer roles i.e. administrators, vendors, guests, who will be able to use this plugin. If you don''t need this option just leave this field empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.ActivatePointsImmediately">
    <Value>Activate points immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.ActivatePointsImmediately.Hint">
    <Value>Activates bonus points immediately after their calculation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.ActivationDelay">
    <Value>Reward points activation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.ActivationDelay.Hint">
    <Value>Specify how many days (hours) must elapse before earned points become active. Points earned by purchase cannot be redeemed until activated. For example, you may set the days before the points become available to 7. In this case, the points earned will be available for spending only 7 days after the purchase.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.ActivatedLater">
    <Value>The points will be activated on {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.RewardPointsActivatingDelayPeriod.Days">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.RewardPointsActivatingDelayPeriod.Hours">
    <Value>Hours</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.ActivatedLater">
    <Value>The points will be activated on {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.PublishedCurrencyRequired">
    <Value>At least one published currency is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.GuestsAndRegisteredRolesError">
    <Value>The customer cannot be in both ''Guests'' and ''Registered'' customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AddCustomerToGuestsOrRegisteredRoleError">
    <Value>Add the customer to ''Guests'' or ''Registered'' customer role</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.ValidEmailRequiredRegisteredRole">
    <Value>Valid Email is required for customer to be in ''Registered'' role</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.NonAdminNotImpersonateAsAdminError">
    <Value>A non-admin user cannot impersonate as an administrator</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.PublishedLanguageRequired">
    <Value>At least one published language is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError">
    <Value>This order item has an associated gift card record. Please delete it first</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.OrderItemDeleted">
    <Value>Order item is deleted</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError">
    <Value>Captcha is enabled but the appropriate keys are not entered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Copied">
    <Value>The message template has been copied successfully</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Copied">
    <Value>The product has been copied successfully</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.System.Warnings.URL.Reserved">
    <Value>Entered page name already exists, so it will be replaced by ''{0}''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DeliveryDate.Hint">
    <Value>Choose a delivery date which will be displayed in the public store. You can manage delivery dates by selecting Configuration > Shipping > Dates and ranges.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductAvailabilityRange">
    <Value>Product availability range</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductAvailabilityRange.Hint">
    <Value>Choose the product availability range that indicates when the product is expected to be available when out of stock (e.g. Available in 10-14 days). You can manage availability ranges by selecting Configuration > Shipping > Dates and ranges.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductAvailabilityRange.None">
    <Value>None</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.ProductAvailabilityRange">
    <Value>Product availability range</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DatesAndRanges">
    <Value>Dates and ranges</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Hint">
    <Value>List of delivery dates which will be available on the product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges">
    <Value>Product availability ranges</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Added">
    <Value>The new product availability range has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.AddNew">
    <Value>Add a new product availability range</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.BackToList">
    <Value>back to product availability range list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Deleted">
    <Value>The product availability range has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.EditProductAvailabilityRangeDetails">
    <Value>Edit product availability range details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.DisplayOrder.Hint">
    <Value>The display order of this product availability range. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name.Hint">
    <Value>Enter product availability range name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Hint">
    <Value>List of availability range options which will be available in product details.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.ProductAvailabilityRanges.Updated">
    <Value>The product availability range has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Availability.AvailabilityRange">
    <Value>Available in {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Availability.BackorderingWithDate">
    <Value>Out of stock - on backorder and will be dispatched once in stock ({0}).</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.AvailabilityRange">
    <Value>Available in {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CheckMoneyOrder.PaymentMethodDescription">
    <Value>Pay by cheque or money order</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.PaymentMethodDescription">
    <Value>Pay by credit / debit card</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.PaymentMethodDescription">
    <Value>Pay by credit / debit card</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.PaymentMethodDescription">
    <Value>You will be redirected to PayPal site to complete the payment</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PurchaseOrder.PaymentMethodDescription">
    <Value>Pay by purchase order (PO) number</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AccountActivation.AlreadyActivated">
    <Value>Your account already has been activated</Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.PasswordAlreadyHasBeenChanged">
    <Value>Your password already has been changed. For changing it once more, you need to again recover the password.</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.ResultAlreadyDeactivated">
    <Value>Your subscription already has been deactivated.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedRateShipping.Fields.ShippingMethodName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedRateShipping.Fields.Rate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Store">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Store.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Warehouse">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Warehouse.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Country">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Country.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.StateProvince">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.StateProvince.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Zip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Zip.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.ShippingMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.ShippingMethod.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.From">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.From.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.To">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.To.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.LowerWeightLimit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.LowerWeightLimit.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.DataHtml">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.AddRecord">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Formula">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Formula.Value">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.ShippingByWeight">
    <Value>By Weight</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fixed">
    <Value>Fixed Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Rate">
    <Value>Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Store.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Warehouse.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Country.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.StateProvince.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Zip">
    <Value>Zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.Zip.Hint">
    <Value>Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod">
    <Value>Shipping method</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod.Hint">
    <Value>Choose shipping method</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.From">
    <Value>Order weight from</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.From.Hint">
    <Value>Order weight from.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.To">
    <Value>Order weight to</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.To.Hint">
    <Value>Order weight to.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost">
    <Value>Additional fixed cost</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost.Hint">
    <Value>Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don''t want an additional fixed cost to be applied.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit">
    <Value>Lower weight limit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit.Hint">
    <Value>Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal">
    <Value>Charge percentage (of subtotal)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal.Hint">
    <Value>Charge percentage (of subtotal).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit">
    <Value>Rate per weight unit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit.Hint">
    <Value>Rate per weight unit.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated">
    <Value>Limit shipping methods to configured ones</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated.Hint">
    <Value>If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they''ll be able to choose any existing shipping options even they''ve not configured here (zero shipping fee in this case).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Fields.DataHtml">
    <Value>Data</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.AddRecord">
    <Value>Add record</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Formula">
    <Value>Formula to calculate rates</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.FixedOrByWeight.Formula.Value">
    <Value>[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowVendorsToImportProducts">
    <Value>Allow vendors to import products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowVendorsToImportProducts.Hint">
    <Value>Check if vendors are allowed to import products.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.ItemYouSave">
    <Value>You save: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.MaximumDiscountedQty">
    <Value>Discounted qty: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPrice.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceEndDateTimeUtc">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceEndDateTimeUtc.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceStartDateTimeUtc">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SpecialPriceStartDateTimeUtc.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecialPrice">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecialPriceEndDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.SpecialPriceStartDate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.AddNew">
    <Value>Add new tier price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Edit">
    <Value>Edit tier price details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.CustomerRole.Hint">
    <Value>Select customer role for which the tier price will be available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.EndDateTimeUtc">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.EndDateTimeUtc.Hint">
    <Value>The end date of the tier price in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Price.Hint">
    <Value>Specify the price.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Quantity.Hint">
    <Value>Specify quantity for which this tier price will be available.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.StartDateTimeUtc">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.StartDateTimeUtc.Hint">
    <Value>The start date of the tier price in Coordinated Universal Time (UTC).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.TierPrices.Fields.Store.Hint">
    <Value>Option to limit this tier price to a certain store. If you have multiple stores, choose one from the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DeactivateGiftCardsAfterDeletingOrder">
    <Value>Deactivate gift cards after deleting of an order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DeactivateGiftCardsAfterDeletingOrder.Hint">
    <Value>Check to deactivate related gift cards when an order is deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CompleteOrderWhenDelivered">
    <Value>Complete order when delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CompleteOrderWhenDelivered.Hint">
    <Value>Check if an order status should be set to "Complete" only when its shipping status is "Delivered". Otherwise, "Shipped" status will be enough.</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnRequests.Title">
    <Value><![CDATA[Return item(s) from <a href="{0}">order #{1}</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditNewsComment">
    <Value>Edited a news comment (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.NewsCommentsMustBeApproved">
    <Value>News comments must be approved</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.NewsCommentsMustBeApproved.Hint">
    <Value>Check if news comments must be approved by administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.Fields.IsApproved">
    <Value>Is approved</Value>
  </LocaleResource>
  <LocaleResource Name="News.Comments.SeeAfterApproving">
    <Value>News comment is successfully added. You will see it after approving by a store administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditBlogComment">
    <Value>Edited a blog comment (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.BlogCommentsMustBeApproved">
    <Value>Blog comments must be approved</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.BlogCommentsMustBeApproved.Hint">
    <Value>Check if blog comments must be approved by administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.Fields.IsApproved">
    <Value>Is approved</Value>
  </LocaleResource>
  <LocaleResource Name="Blog.Comments.SeeAfterApproving">
    <Value>Blog comment is successfully added. You will see it after approving by a store administrator.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory">
    <Value>Stock quantity history</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Hint">
    <Value>Here you can see a history of the product stock quantity changes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Fields.Combination">
    <Value>Attribute combination</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Fields.CreatedOn">
    <Value>Created On</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Fields.Message">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Fields.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Fields.QuantityAdjustment">
    <Value>Quantity adjustment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.StockQuantityHistory.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.StockQuantityHistory">
    <Value>Stock quantity history</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.CancelOrder">
    <Value>The stock quantity has been increased by canceling the order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.Combination.Edit">
    <Value>The stock quantity of combination has been edited</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.CopyProduct">
    <Value>The stock quantity has been edited by copying the product #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.DeleteOrder">
    <Value>The stock quantity has been increased by deleting the order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.DeleteOrderItem">
    <Value>The stock quantity has been increased by deleting an order item from the order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.DeleteShipment">
    <Value>The stock quantity has been increased by deleting a shipment from the order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.Edit">
    <Value>The stock quantity has been edited</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.MultipleWarehouses">
    <Value>Multiple warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.EditOrder">
    <Value>The stock quantity has been changed by editing the order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.ImportProduct.Edit">
    <Value>The stock quantity has been changed by importing product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.ImportProduct.EditWarehouse">
    <Value>Products have been moved {0} {1} by importing product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.EditWarehouse">
    <Value>Products have been moved {0} {1}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.EditWarehouse.New">
    <Value>to the {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.EditWarehouse.Old">
    <Value>from the {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.PlaceOrder">
    <Value>The stock quantity has been reduced by placing the order #{0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.StockQuantityHistory.Messages.Ship">
    <Value>The stock quantity has been reduced when an order item of the order #{0} was shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Copy.Name.New">
    <Value>{0} - copy</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Copy.SKU.New">
    <Value>{0}-copy</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded.Pending">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled.Pending">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DeactivateGiftCardsAfterCancellingOrder">
    <Value>Deactivate gift cards after cancelling of an order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.DeactivateGiftCardsAfterCancellingOrder.Hint">
    <Value>Check to deactivate related gift cards when an order is cancelled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ActivateGiftCardsAfterCompletingOrder">
    <Value>Activate gift cards after completing of an order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ActivateGiftCardsAfterCompletingOrder.Hint">
    <Value>Check to activate related gift cards when an order is completed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.GiftCards_Deactivated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.GiftCards_Deactivated.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.GiftCards_Deactivated.Pending">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.GiftCards_Activated">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.GiftCards_Activated.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.GiftCards_Activated.Pending">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.ID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.ID.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.RestartApplication">
    <Value>Do not forget to restart the application once a task has been modified.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.Login">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.Or">
    <Value>- or -</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Choose">
    <Value>Choose an associated product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchStore.Hint">
    <Value>Load products only from a specific store (available in this store).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchVendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchVendor.Hint">
    <Value>Load products only by a specific vendor (owned by this vendor).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchCategory">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchCategory.Hint">
    <Value>Load products only from a specific category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchManufacturer">
    <Value>Manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.NeverSold.SearchManufacturer.Hint">
    <Value>Load products only from a specific manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.CartsSharedBetweenStores">
    <Value>Carts shared between stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.CartsSharedBetweenStores.Hint">
    <Value>Determines whether shopping carts (and wishlists) are shared between stores (in multi-store environment).</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailRevalidation">
    <Value>Email validation</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailRevalidation.AlreadyChanged">
    <Value>Your email already has been validated</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailRevalidation.Changed">
    <Value>Your email has been validated</Value>
  </LocaleResource>
  <LocaleResource Name="Account.EmailRevalidation">
    <Value>Email validation</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.EmailRevalidation">
    <Value>Email validation</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.EmailToRevalidate">
    <Value>New email</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.EmailToRevalidate.Note">
    <Value>(not validated yet)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Methods.Description">
    <Value>Shipping methods used by offline shipping providers. For example, "Manual (Fixed or By Weight)".</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Product.IgnoredProductTypes">
    <Value>Ignored product type IDs (advanced)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestsAllowFiles">
    <Value>Allow file uploads</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ReturnRequestsAllowFiles.Hint">
    <Value>Check if you want to allow customers to upload files when submitting return requests.</Value>
  </LocaleResource>
  <LocaleResource Name="ReturnRequests.UploadedFile">
    <Value>Upload (any additional document, scan, etc)</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerReturnRequests.UploadedFile">
    <Value>Uploaded file:</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerReturnRequests.UploadedFile.Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.UploadedFile">
    <Value>Uploaded file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.UploadedFile.Hint">
    <Value>File uploaded by customer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.UploadedFile.Download">
    <Value>Download</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.Fields.ReplyText">
    <Value>Reply text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.Fields.ReplyText.Hint">
    <Value>The reply text (by a store owner). If specified, then it''ll be visible to a customer. Leave empty to ignore this functionality.</Value>
  </LocaleResource>
  <LocaleResource Name="Reviews.Reply">
    <Value>A manager responded to this review</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.ShowBlogCommentsPerStore">
    <Value>Blog comments per store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.ShowBlogCommentsPerStore.Hint">
    <Value>Check to display blog comments written in the current store only.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.ShowNewsCommentsPerStore">
    <Value>News comments per store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.ShowNewsCommentsPerStore.Hint">
    <Value>Check to display news comments written in the current store only.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.Fields.StoreName">
    <Value>Store name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.Fields.StoreName">
    <Value>Store name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasAttributes">
    <Value>The associated product has attributes, keep in mind that customers can not select them in the product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasRequiredAttributes">
    <Value>The associated product has required product attributes, so customers won''t be able to choose this product attribute value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable">
    <Value>The associated product is downloadable, keep in mind that won''t be able to download it.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.GiftCard">
    <Value>The associated product is a gift card, keep in mind that customers can not specify its details in the product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Display">
    <Value>{0} - {1} of {2} items</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Empty">
    <Value>No items to display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.First">
    <Value>Go to the first page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.ItemsPerPage">
    <Value>items per page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Last">
    <Value>Go to the last page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.MorePages">
    <Value>More pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Next">
    <Value>Go to the next page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Of">
    <Value>of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Page">
    <Value>Page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Previous">
    <Value>Go to the previous page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Pager.Refresh">
    <Value>Refresh</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Address">
    <Value>Address (optional)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FailedPasswordAllowedAttempts">
    <Value>Maximum login failures</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FailedPasswordAllowedAttempts.Hint">
    <Value>Maximum login failures to lockout account. Set 0 to disable this feature.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FailedPasswordLockoutMinutes">
    <Value>Lockout time (login failures)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FailedPasswordLockoutMinutes.Hint">
    <Value>Enter number of minutes to lockout users (for login failures).</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Login.WrongCredentials.LockedOut">
    <Value>Customer is locked out</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.ImportFromExcelTip">
    <Value>Imported manufacturers are distinguished by ID. If the ID already exists, then its corresponding manufacturer will be updated. You should not specify ID (leave 0) for new manufacturers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.ImportFromExcelTip">
    <Value>Imported categories are distinguished by ID. If the ID already exists, then its corresponding category will be updated. You should not specify ID (leave 0) for new categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement">
    <Value>For conditional expressions use the token %if (your conditions ) ... endif%</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedRate.Fields.TaxCategoryName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedRate.Fields.Rate">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Store">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Store.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Country">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Country.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.StateProvince">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.StateProvince.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Zip">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Zip.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.TaxCategory">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.TaxCategory.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Percentage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.Fields.Percentage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.AddRecord">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.CountryStateZip.AddRecord.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fixed">
    <Value>Fixed rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip">
    <Value>By Country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName">
    <Value>Tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate">
    <Value>Rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint">
    <Value>The country.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint">
    <Value>If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip">
    <Value>Zip</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint">
    <Value>Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory">
    <Value>Tax category</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint">
    <Value>The tax category.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage">
    <Value>Percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint">
    <Value>The tax rate.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.AddRecord">
    <Value>Add tax rate</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle">
    <Value>New tax rate</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.SearchCustomNumber">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.SearchCustomNumber.Hint">
    <Value>Search by a specific return request identifier.</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.ReturnRequests.SearchEndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.SearchEndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.ReturnRequests.SearchReturnRequestStatus">
    <Value>Return status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.SearchReturnRequestStatus.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.SearchReturnRequestStatus.Hint">
    <Value>Search by a specific return request status e.g. Received.</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.ReturnRequests.SearchStartDate">
    <Value>Start date</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ReturnRequests.SearchStartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Hint">
    <Value>
      Product attributes are quantifiable or descriptive aspects of a product (such as, color). For example, if you were to create an attribute for color, with the values of blue, green, yellow, and so on, you may want to apply this attribute to shirts, which you sell in various colors (you can adjust a price or weight for any of existing attribute values).
      You can add attribute for your product using existing list of attributes, or if you need to create a new one go to Catalog > Attributes > Product attributes. Please notice that if you want to manage inventory by product attributes (e.g. 5 green shirts and 3 blue ones), then ensure that "Inventory method" is set to "Track inventory by product attributes".
    </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.ApproveSelected">
    <Value>Approve selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.ApproveSelected">
    <Value>Approve selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.DisapproveSelected">
    <Value>Disapprove selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.DisapproveSelected">
    <Value>Disapprove selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.RegisteredInStore">
    <Value>Registered in the store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.RegisteredInStore.Hint">
    <Value>Indicating in which store the customer is registered</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ConsiderAssociatedProductsDimensions">
    <Value>Consider associated products dimensions and weight</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.ConsiderAssociatedProductsDimensions.Hint">
    <Value>Check to consider associated products dimensions and weight on shipping, uncheck for example, if the main product already includes them.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.CreatedOnFrom">
    <Value>Created from</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.CreatedOnFrom.Hint">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.CreatedOnTo">
    <Value>Created to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.CreatedOnTo.Hint">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchText">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchText.Hint">
    <Value>Search in title and comment text.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.CreatedOnFrom">
    <Value>Created from</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.CreatedOnFrom.Hint">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.CreatedOnTo">
    <Value>Created to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.CreatedOnTo.Hint">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchText">
    <Value>Message</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchText.Hint">
    <Value>Search in comment text.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerOrders.RecurringOrders.RetryLastPayment">
    <Value>Retry last payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.RecurringPayments.History.LastPaymentFailed">
    <Value>Last payment failed</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchApproved">
    <Value>Approved</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchApproved.Hint">
    <Value>Search by a "Approved" property.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchApproved.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchApproved.ApprovedOnly">
    <Value>Approved only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchApproved.DisapprovedOnly">
    <Value>Disapproved only</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchApproved">
    <Value>Approved</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchApproved.Hint">
    <Value>Search by a "Approved" property.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchApproved.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchApproved.ApprovedOnly">
    <Value>Approved only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.SearchApproved.DisapprovedOnly">
    <Value>Disapproved only</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchApproved">
    <Value>Approved</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchApproved.Hint">
    <Value>Search by a "Approved" property.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchApproved.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly">
    <Value>Approved only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly">
    <Value>Disapproved only</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.RoundingWarning">
    <Value>It looks like you have "ShoppingCartSettings.RoundPricesDuringCalculation" setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as PayPal only rounds to two decimals.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.Order">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Promotions.Discounts.History.Order">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.GiftCards.History.Order">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.RecurringPayments.History.Order">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.Order">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.Order.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.OrderId">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Customers.Customers.Orders.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.History.OrderId">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.History.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.History.OrderId">
    <Value>Order ID</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.GiftCards.History.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Orders.Fields.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CustomOrderNumber.Hint">
    <Value>The unique number of this order.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.RecurringPayments.History.OrderId">
    <Value>Created order ID</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.RecurringPayments.History.CustomOrderNumber">
    <Value>Created order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.OrderId">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.OrderID">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CustomOrderNumber">
    <Value>Order #</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Orders.Shipments.CustomOrderNumber.Hint">
    <Value>The order associated to this shipment.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask">
    <Value>Order number mask</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask.Hint">
    <Value>Order number mask, for creating custom order number. For example, RE-{YYYY}-{MM}. Leave this field empty if you don''t want to use custom order numbers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask.Description.DD">
    <Value>{DD} - day of order creation date</Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask.Description.ID">
    <Value>{ID} - Order identifier</Value>
  </LocaleResource>     
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask.Description.MM">
    <Value>{MM} - month of order creation date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask.Description.YYYY">
    <Value>{YYYY} - year of order creation date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.CustomOrderNumberMask.Description.YY">
    <Value>{YY} - last two digits of year of order creation date</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.GiftCards.Fields.Order">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.Fields.Order.Hint">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.GiftCards.Fields.OrderId">
    <Value>Order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.Fields.OrderId.Hint">
    <Value>The gift card was purchased with this order.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.Fields.CustomOrderNumber">
    <Value>Order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.Fields.CustomOrderNumber.Hint">
    <Value>The gift card was purchased with this order.</Value>
  </LocaleResource>   
  <LocaleResource Name="ActivityLog.EditOrder">
    <Value>Edited an order (Order number = {0}). See order notes for details</Value>
  </LocaleResource> 
  <LocaleResource Name="Account.CustomerOrders.RecurringOrders.ViewInitialOrder">
    <Value>View order (Order number - {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ReturnRequests.Fields.CustomOrderNumber.Hint">
    <Value>The unique number of the order.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Affiliates.Orders.OrderId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Orders.ID">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Promotions.Discounts.History.OrderId">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.GiftCards.History.OrderId">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Orders.Fields.ID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ID.Hint">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.RecurringPayments.History.OrderId">
    <Value></Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.ReturnRequests.Fields.OrderId">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Orders.Shipments.OrderID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.OrderID.Hint">
    <Value></Value>
  </LocaleResource>    
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.PassPurchasedItems">
    <Value>Pass purchased items</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.PassPurchasedItems.Hint">
    <Value>Check to pass information about purchased items to PayPal.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UnduplicatedPasswordsNumber">
    <Value>Unduplicated passwords number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.UnduplicatedPasswordsNumber.Hint">
    <Value>Specify the number of customer passwords that mustn''t be the same as the previous one, enter 0 if the customer can use the same password time after time.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Errors.PasswordMatchesWithPrevious">
    <Value>You entered the password that is the same as one of the last passwords you used. Please create a new password.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.PasswordIsExpired">
    <Value>Your password has expired, please create a new one</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordLifetime">
    <Value>Password lifetime</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordLifetime.Hint">
    <Value>Specify number of days for password expiration. Don''t forget to check "EnablePasswordLifetime" property on customer role edit page for those roles, who will have to change passwords.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.EnablePasswordLifetime">
    <Value>Enable password lifetime</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.EnablePasswordLifetime.Hint">
    <Value>Check to force customers to change their passwords after a specified time.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.Id">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Id">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="ContactUs.EmailSubject">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.EmailSubject">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderStatus.CancelledNotification">
    <Value>This order is cancelled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Blog.BlogComment">
	<Value><![CDATA[This message template is used when a new blog comment to the certain blog post is created. The message is received by a store owner. You can set up this option by ticking the checkbox <strong>Notify about new blog comments</strong> in Configuration - Settings - Blog settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.BackInStock">
	<Value><![CDATA[This message template is used when the customer subscribed for the certain product which is unavailable at the moment to notify the customer about the product being back to stock again. You can set up this option by ticking the checkbox <strong>Allow back in stock subscriptions</strong> in Product info tab - Inventory section.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.EmailRevalidationMessage">
	<Value>This message template is used when a customer changes an email address in his account. The customer receives a message to confirm an email address used when changing email address.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.EmailValidationMessage">
	<Value>This message template is used the option Email notification from Registration method dropdown list in Configuration - Settings - Customer settings is selected. The customer receives a message to confirm an email address used when registering.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.NewOrderNote">
	<Value>This message template is used when the customer gets a notification about a new order being placed from this account.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.NewPM">
	<Value><![CDATA[This message template is used when the customer gets a notification about a new private message being received. You can set up this option by ticking the checkbox <strong>Show alert for PM</strong> in Configuration - Settings - Forum settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.PasswordRecovery">
	<Value>This message template is used when a customer forgets a password needed to log in the account. The customer receives a message with a link for entering a new password.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Customer.WelcomeMessage">
	<Value>This message template is used to welcome a new customer after registration.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Forums.NewForumPost">
	<Value>This message template is used when a new forum post in certain forum topic is created. The message is received by a store owner.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Forums.NewForumTopic">
	<Value>This message template is used when a new forum topic is created. The message is received by a store owner.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.GiftCard.Notification">
	<Value>This message template is used to send a notification to a customer about getting a gift card.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.NewCustomer.Notification">
	<Value><![CDATA[This message template is used when a new customer is registered. The message is received by a store owner. You can set up this option by ticking the checkbox <strong>Notify about new customer registration</strong> in Configuration - Settings - Customer settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.NewReturnRequest.CustomerNotification">
	<Value>This message template is used to notify a customer about a new return request submitted from his/her account.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.NewReturnRequest.StoreOwnerNotification">
	<Value>This message template is used when a new return request is created. The message is received by a store owner.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.News.NewsComment">
	<Value><![CDATA[This message template is used when a new comment to the certain news item is created. The message is received by a store owner. You can set up this option by ticking the checkbox <strong>Notify about new news comments</strong> in Configuration - Settings - News settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.NewsLetterSubscription.ActivationMessage">
	<Value>This message template is used when a customer subscribes to the newsletter to confirm the subscription.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.NewsLetterSubscription.DeactivationMessage">
	<Value><![CDATA[This message template is used when a customer unsubscribes to the newsletter to confirm the subscription deactivation. You can allow users to unsubscribe by ticking the checkbox <strong>Newsletter box. Allow to unsubscribe</strong> in Configuration - Settings - Customer settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.NewVATSubmitted.StoreOwnerNotification">
	<Value><![CDATA[This message template is used when a new VAT is submitted. The message is received by a store owner. You can set up this option by clicking <strong>Notify admin when a new VAT number is submitted</strong> in Configuration - Settings - Tax settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderCancelled.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain order was canceled. The order can ba canceled by a customer on the account page or by store owner in Customers - Customers in Orders tab or in Sales - Orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderCompleted.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain order was completed. The order gets the order status Complete when it''s paid and delivered, or it can be changed manually to Complete in Sales - Orders.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPaid.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain order was paid. The order gets the payment status Paid when the amount was charged, or it can be changed manually Sales - Orders by clicking Mark as paid button in Payment status.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPaid.StoreOwnerNotification">
	<Value>This message template is used to notify a store owner that the certain order was paid. The order gets the status Paid when the amount was charged, or it can be changed manually Sales - Orders by clicking Mark as paid button in Payment status.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPaid.VendorNotification">
	<Value>This message template is used to notify a vendor that the certain order was paid. The order gets the status Paid when the amount was charged.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPlaced.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain order was placed. Orders can be viewed by a customer on the account page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPlaced.StoreOwnerNotification">
	<Value>This message template is used to notify a store owner that the certain order was placed. Orders can be viewed by a store owner in Sales - Orders or in Customers menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPlaced.VendorNotification">
	<Value>This message template is used to notify a vendor that the certain order was placed. Orders can be viewed by a vendor in Sales - Orders. You can allow them to access this part of Admin Area in Configuration - Access control list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderRefunded.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain order was refunded. The customer can submit to refund the order when the payment status is paid on the account page, or it can be done by a store owner in Sales - Orders by clicking "Refund" button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderRefunded.StoreOwnerNotification">
	<Value>This message template is used to notify a store owner that the certain order was refunded. The customer can submit to refund the order when the payment status is paid on the account page, or it can be done by a store owner in Sales - Orders by clicking "Refund" button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Product.ProductReview">
	<Value><![CDATA[This message template is used to notify a store owner that a new product review is created. You can set up this option by ticking the checkbox <strong>Notify about new product reviews</strong> in Configuration - Settings - Catalog settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.QuantityBelow.AttributeCombination.StoreOwnerNotification">
	<Value>This message template is used to notify a store owner that the certain product attribute combination is getting low stock. You can set up the combination minimum quantity when creating or editing the product in Product attribute tab - Attributes combinations tab in Notify admin for quantity below field.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.QuantityBelow.StoreOwnerNotification">
	<Value><![CDATA[This message template is used to notify a store owner that the certain product is getting low stock. You can set up the minimum  product quantity when creating or editing the product in Inventory section, <strong>Minimum stock qty field</strong>.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.RecurringPaymentCancelled.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain recurring payment is canceled. Payment can be canceled by a customer in the account page or by a store owner in Sales - Recurring payments in History tab by clicking "Cancel recurring payment" button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.RecurringPaymentCancelled.StoreOwnerNotification">
	<Value>This message template is used to notify a store owner that the certain recurring payment is canceled. Payment can be canceled by a customer in the account page or by a store owner in Sales - Recurring payments in History tab by clicking "Cancel recurring payment" button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.RecurringPaymentFailed.CustomerNotification">
	<Value>This message template is used to notify a customer that the certain recurring payment is failed. For example, the amount can''t be charged from the provided credit card.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.ReturnRequestStatusChanged.CustomerNotification">
	<Value>This message template is used to notify a customer that the request status to the certain order is changed. You can set up this option in Sales - Return requests by clicking "Notify customer about status change" button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Service.ContactUs">
	<Value>This message template is used to notify a store owner about a message sent through the contact form.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Service.ContactVendor">
	<Value>This message template is used to notify a vendor about a message sent through the contact form.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Service.EmailAFriend">
	<Value>This message template is used to send "email a friend" message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.ShipmentDelivered.CustomerNotification">
	<Value><![CDATA[This message template is used to notify a customer that the shipping status of the certain order is set to delivered. You can set up this option in Configuration - Settings - Shipping settings by ticking the checkbox <strong>Display shipment events (customers)</strong>.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.ShipmentSent.CustomerNotification">
	<Value><![CDATA[This message template is used to notify a customer that the shipping status of the certain order is sent. You can set up this option in Configuration - Settings - Shipping settings by ticking the checkbox <strong>Display shipment events (customers)</strong>.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.VendorAccountApply.StoreOwnerNotification">
	<Value><![CDATA[This message template is used to notify a store owner that new user has applied for vendor account. You can allow customers to apply for vendor account in Configuration - Setting - Vendor settings by ticking the checkbox <strong>Allow customers to apply for vendor account</strong>.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.VendorInformationChange.StoreOwnerNotification">
	<Value><![CDATA[This message template is used to notify a store owner that the vendor changed some vendor''s details. You can allow vendors to change info about themselves in Configuration - Setting - Vendor settings by ticking the checkbox <strong>Allow vendors to edit info</strong>.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Wishlist.EmailAFriend">
	<Value>This message template is used when a customer wants to share some product from the wishlist with a friend by sending an email. You can set up this option by ticking the checkbox <strong>Allow customers to email their wishlists</strong> in Configuration - Settings - Shopping cart settings.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.TaxBasedOnPickupPointAddress">
    <Value>Tax based on pickup point address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.TaxBasedOnPickupPointAddress.Hint">
    <Value>A value indicating whether to use pickup point address (when pickup point is chosen) for tax calculation.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning">
    <Value>It seems that you use Redis server for caching, keep in mind that enabling this setting create a lot of traffic between the Redis server and the application because of the large number of locales.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ExportWithProducts">
    <Value>Export orders with products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.ExportWithProducts.Hint">
    <Value>Check if orders should be exported with products.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Description">
    <Value><![CDATA[Set up requirements to a created discount if you want to limit it to certain user categories depending on a customer role, the amount spent, etc. You can use single requirement type, or group several types and apply them simultaneously.<br>Requirement group is a useful feature for creating discount requirement templates. You can create a requirement group just once and then use it every time you want this limitation to be applied. You can include one requirement group into another one if needed.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.DiscountRequirementType.AddGroup">
    <Value>Add requirement group</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Hint">
    <Value>You can choose one of the following requirement types, or add a requirement group to use several requirement types simultaneously.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.GroupName">
    <Value>Group name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.GroupName.Hint">
    <Value>Specify name of the requirement group (e.g. "Permitted customer roles").</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.RequirementGroup">
    <Value>Add to group</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.RequirementGroup.Hint">
    <Value>Choose the group you want the requirement group you’re creating to be assigned to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.RequirementGroup.Title">
    <Value>Group</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Discounts.RequirementGroupInteractionType.And">
    <Value>AND</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Discounts.RequirementGroupInteractionType.Or">
    <Value>OR</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Remove">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Requirement.Title">
    <Value>Requirement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup">
    <Value>Default requirement group</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.InteractionTypeInGroup">
    <Value>Interaction type in this group is</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.GroupIsEmpty">
    <Value>The group is empty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.RemoveRequirement">
    <Value>Remove requirement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.RemoveGroup">
    <Value>Remove group</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.BlockTitle.BlogComments">
    <Value>Blog comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.AdditionalSections">
    <Value>Additional sections</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.CatalogPages">
    <Value>Catalog pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Compare">
    <Value>Compare products</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ExportImport">
    <Value>Export/Import</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Performance">
    <Value>Performance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductFields">
    <Value>Product fields</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductPage">
    <Value>Product page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductSorting">
    <Value>Product sorting</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductReviews">
    <Value>Product reviews</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Search">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Share">
    <Value>Share</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Tax">
    <Value>Tax</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Tags">
    <Value>Tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Account">
    <Value>Account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.DefaultFields">
    <Value>Default fields</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.ExternalAuthentication">
    <Value>External authentication</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Password">
    <Value>Password and security</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Profile">
    <Value>Profile</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.TimeZone">
    <Value>Time zone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.Feeds">
    <Value>Feeds</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.PageSizes">
    <Value>Page sizes</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.Permissions">
    <Value>Permissions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.FullText">
    <Value>Full-Text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Captcha">
    <Value>CAPTCHA</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Localization">
    <Value>Localization</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Pdf">
    <Value>Pdf</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Security">
    <Value>Security</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.SEO">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Sitemap">
    <Value>Sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.SocialMedia">
    <Value>Social media</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.BlockTitle.OtherPages">
    <Value>Other pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.BlockTitle.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.BlockTitle.NewsComments">
    <Value>News comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.BlockTitle.Checkout">
    <Value>Checkout</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.BlockTitle.GiftCards">
    <Value>Gift cards</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.BlockTitle.OrderTotals">
    <Value>Order totals</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.BlockTitle.PdfInvoice">
    <Value>Pdf invoice</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.BlockTitle.Checkout">
    <Value>Checkout</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.BlockTitle.Notifications">
    <Value>Notifications</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.BlockTitle.MiniShoppingCart">
    <Value>Mini shopping cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.BlockTitle.Wishlist">
    <Value>Wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.Payment">
    <Value>Payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.Shipping">
    <Value>Shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.TaxDispaying">
    <Value>Tax dispaying</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.BlockTitle.VAT">
    <Value>VAT</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.BlockTitle.Catalog">
    <Value>Catalog</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.IgnoreDiscounts.Warning">
    <Value>In order to use this functionality you have to disable the following setting: Configuration > Catalog settings > Ignore discounts (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.GoogleShopping.GeneralInstructions">
    <Value><![CDATA[<p><ul><li>At least two unique product identifiers are required. So each of your product shouldhave manufacturer (brand) and MPN (manufacturer part number) specified</li><li>Specify default tax values in your Google Merchant Center account settings</li><li>Specify default shipping values in your Google Merchant Center account settings</li><li>In order to get more info about required fields look at the following article <a href="http://www.google.com/support/merchants/bin/answer.py?answer=188494" target="_blank">http://www.google.com/support/merchants/bin/answer.py?answer=188494</a></li></ul></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.GoogleShopping.OverrideInstructions">
    <Value><![CDATA[<p>You can download the list of allowed Google product category attributes <a href="http://www.google.com/support/merchants/bin/answer.py?answer=160081" target="_blank">here</a></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Instructions">
    <Value><![CDATA[<p><b>If you''re using this gateway ensure that your primary store currency is supported by Paypal.</b><br /><br />To configure plugin follow these steps:<br />1. Log into your Developer PayPal account (click <a href="https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8" target="_blank">here</a> to create your account).<br />2. Click on My Apps & Credentials from the Dashboard.<br />3. Create new REST API app.<br />4. Copy your Client ID and Secret key below.<br />5. To be able to use recurring payments you need to set the webhook ID. You can get it manually in your PayPal account (enter the URL https://www.yourStore.com/Plugins/PaymentPayPalDirect/Webhook below REST API application credentials), or automatically by pressing "@T("Plugins.Payments.PayPalDirect.WebhookCreate")" button (not visible when running the site locally).<br /></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Instructions">
    <Value><![CDATA[<p><b>If you''re using this gateway ensure that your primary store currency is supported by Paypal.</b><br /><br />To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal. Follow these steps to configure your account for PDT:<br /><br />1. Log in to your PayPal account (click <a href="https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8" target="_blank">here</a> to create your account).<br />2. Click the Profile subtab.<br />3. Click Website Payment Preferences in the Seller Preferences column.<br />4. Under Auto Return for Website Payments, click the On radio button.<br />5. For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment (http://www.yourStore.com/Plugins/PaymentPayPalStandard/PDTHandler).<br />6. Under Payment Data Transfer, click the On radio button.<br />7. Click Save.<br />8. Click Website Payment Preferences in the Seller Preferences column.<br />9. Scroll down to the Payment Data Transfer section of the page to view your PDT identity token.<br /><br /><b>Two ways to be able to receive IPN messages (optional):</b><br /><br /><b>The first way is to check ''Enable IPN'' below.</b> It will include in the request the url of you IPN handler<br /><br /><b>The second way is to confugure your paypal account to activate this service</b>; follow these steps:<br />1. Log in to your Premier or Business account.<br />2. Click the Profile subtab.<br />3. Click Instant Payment Notification in the Selling Preferences column.<br />4. Click the ''Edit IPN Settings'' button to update your settings.<br />5. Select ''Receive IPN messages'' (Enabled) and enter the URL of your IPN handler (http://www.yourStore.com/Plugins/PaymentPayPalStandard/IPNHandler).<br />6. Click Save, and you should get a message that you have successfully activated IPN.</p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.CanadaPost.Instructions">
    <Value><![CDATA[<p>To configure plugin follow one of these steps:<br />1. If you are a Canada Post commercial customer, fill Customer number, Contract ID and API key below.<br />2. If you are a Solutions for Small Business customer, specify your Customer number and API key below.<br />3. If you are a non-contracted customer or you want to use the regular price of shipping paid by customers, fill the API key field only.<br /><br /><em>Note: Canada Post gateway returns shipping price in the CAD currency, ensure that you have correctly configured exchange rate from PrimaryStoreCurrency to CAD.</em></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.Instructions">
    <Value><![CDATA[<p>Google Analytics is a free website stats tool from Google. It keeps track of statisticsabout the visitors and ecommerce conversion on your website.<br /><br />Follow the next steps to enable Google Analytics integration:<br /><ul><li><a href="http://www.google.com/analytics/" target="_blank">Create a Google Analyticsaccount</a> and follow the wizard to add your website</li><li>Copy the Tracking ID into the ''ID'' box below</li><li>Click the ''Save'' button below and Google Analytics will be integrated into your store</li></ul><br />If you would like to switch between Google Analytics (used by default) and Universal Analytics, then please use the buttons below:</p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.Note">
    <Value><![CDATA[<p><em>Please note that {ECOMMERCE} line works only when you have "Disable order completed page" order setting unticked.</em></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Instructions">
    <Value><![CDATA[<p>Here you can find third-party extensions and themes which are developed by our community and partners.They are also available in our <a href="http://www.nopcommerce.com/marketplace.aspx?utm_source=admin-panel&utm_medium=official-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Captcha.Instructions">
    <Value><![CDATA[<p>A CAPTCHA is a program that can tell whether its user is a human or a computer.You''ve probably seen them — colorful images with distorted text at the bottom ofWeb registration forms. CAPTCHAs are used by many websites to prevent abuse from"bots," or automated programs usually written to generate spam. No computer programcan read distorted text as well as humans can, so bots cannot navigate sites protectedby CAPTCHAs. nopCommerce uses <a href="http://www.google.com/recaptcha" target="_blank">reCAPTCHA</a>.</p>]]></Value>
  </LocaleResource> 
 <LocaleResource Name="Admin.Configuration.Plugins.Description.DownloadMorePlugins">
    <Value><![CDATA[<p>You can download more nopCommerce plugins in our <a href="http://www.nopcommerce.com/marketplace.aspx?utm_source=admin-panel&utm_medium=plugins&utm_campaign=admin-panel" target="_blank">marketplace</a></p>]]></Value>
 </LocaleResource>   
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.ViewLink">
    <Value>Edit values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.ViewLink">
    <Value>Edit condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.ViewLink">
    <Value>Edit rules</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportCategories">
    <Value>{0} categories were imported</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportManufacturers">
    <Value>{0} manufacturers were imported</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportProducts">
    <Value>{0} products were imported</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportStates">
    <Value>{0} states and provinces were imported</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Wishlist.AddToWishlist">
    <Value>Add to wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Wishlist.AddToWishlist.Update">
    <Value>Update</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SystemName.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.RoundingType">
    <Value>Rounding type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.RoundingType.Hint">
    <Value>The rounding type.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding001">
    <Value>Default rounding</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding005Up">
    <Value>Rounding up with 0.05 intervals (0.06 round to 0.10)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding005Down">
    <Value>Rounding down with 0.05 intervals (0.06 round to 0.05)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding01Up">
    <Value>Rounding up with 0.10 intervals (1.05 round to 1.10)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding01Down">
    <Value>Rounding down with 0.10 intervals (1.05 round to 1.00)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding05">
    <Value>Rounding with 0.50 intervals</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding1">
    <Value>Rounding with 1.00 intervals (1.01-1.49 round to 1.00, 1.50-1.99 round to 2.00)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Directory.RoundingType.Rounding1Up">
    <Value>Rounding up with 1.00 intervals (1.01–1.99 round to 2.00)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled">
    <Value>Picture zoom</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled.Hint">
    <Value>Check to enable picture zoom on product details page.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem">
    <Value>Display "Blog"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem.Hint">
    <Value>Check if "Blog" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem">
    <Value>Display "Forums"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem.Hint">
    <Value>Check if "Forums" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem">
    <Value>Display "Contact us"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem.Hint">
    <Value>Check if "Contact us" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem">
    <Value>Display "My account"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem.Hint">
    <Value>Check if "My account" menu item should be displayed in the top menu.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem">
    <Value>Display "Home page"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem.Hint">
    <Value>Check if "Home page" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem">
    <Value>Display "New products"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem.Hint">
    <Value>Check if "New products" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem">
    <Value>Display "Search"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem.Hint">
    <Value>Check if "Search" menu item should be displayed in the top menu.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.TopMenuItems">
    <Value>Top menu items</Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Web.Framework.Validators.MaxDecimal">
    <Value>The value is out of range. Maximum value is {0}.99</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled.Hint">
    <Value>Check to enable free shipping for all orders over ''X''. Set the value to X below.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingProduct">
    <Value>Display cart after adding a product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingProduct.Hint">
    <Value>If checked, a customer will be taken to the shopping cart page immediately after adding a product to their cart. If unchecked, a customer will stay on the same page from which the product was added to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct">
    <Value>Display wishlist after adding a product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct.Hint">
    <Value>If checked, a customer will be taken to the wishlist page immediately after adding a product to their wishlist. If unchecked, a customer will stay on the same page from which the product was added to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnWishList.Hint">
    <Value>Determines whether product images should be displayed in customer wishlists.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.BodyOverview.Hint">
    <Value>Brief overview of this blog post. If specified, then it will be used instead of full body on the main blog page. HTML is supported.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend.Hint">
    <Value>A delay before sending the message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.ShowOnHomePage.Hint">
    <Value>Check if you want to show poll on the home page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre-selected for the customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre-selected for the customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre-selected for the address.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre-selected for the customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.IsPreSelected.Hint">
    <Value>Determines whether this attribute value is pre-selected for the customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Choose">
    <Value>Choose a product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.Body.Hint">
    <Value>Enter the message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CardType.Hint">
    <Value>The type of a card used in the transaction.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CustomerIP.Hint">
    <Value>Customer IP address from where an order has been placed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.CustomerRole.Hint">
    <Value>Choose a customer role to which this email will be sent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.SentTries.Required">
    <Value>Enter send attempts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.List.LoadNotSent.Hint">
    <Value>Only load emails into queue that have not been sent yet.</Value>
  </LocaleResource>
  <LocaleResource Name="Forum.TopicSubjectCannotBeEmpty">
    <Value>Topic subject can not be empty</Value>
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
DECLARE @ExistingLanguageId int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [Id]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageId
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
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageId=@ExistingLanguageId AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageId=@ExistingLanguageId AND ResourceName=@ResourceName
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
				@ExistingLanguageId,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageId=@ExistingLanguageId AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource

	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageId
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.hideshippingtotal')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.hideshippingtotal', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'taxsettings.defaulttaxcategoryid')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'taxsettings.defaulttaxcategoryid', N'0', 0)
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewAddressAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewAddressAttribute', N'Add a new address attribute', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewAffiliate')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewAffiliate', N'Add a new affiliate', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewBlogPost')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewBlogPost', N'Add a new blog post', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewCampaign')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewCampaign', N'Add a new campaign', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewCountry')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewCountry', N'Add a new country', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewCurrency')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewCurrency', N'Add a new currency', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewCustomerAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewCustomerAttribute', N'Add a new customer attribute', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewCustomerAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewCustomerAttributeValue', N'Add a new customer attribute value', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewEmailAccount')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewEmailAccount', N'Add a new email account', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewLanguage')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewLanguage', N'Add a new language', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewMeasureDimension')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewMeasureDimension', N'Add a new measure dimension', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewMeasureWeight')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewMeasureWeight', N'Add a new measure weight', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewNews')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewNews', N'Add a new news', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'InstallNewPlugin')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'InstallNewPlugin', N'Install a new plugin', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewStateProvince')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewStateProvince', N'Add a new state or province', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewStore')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewStore', N'Add a new store', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewVendor')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewVendor', N'Add a new vendor', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewWarehouse')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewWarehouse', N'Add a new warehouse', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteAddressAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteAddressAttribute', N'Delete an address attribute', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteAffiliate')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteAffiliate', N'Delete an affiliate', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteBlogPost')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteBlogPost', N'Delete a blog post', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteCampaign')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteCampaign', N'Delete a campaign', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteCountry')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteCountry', N'Delete a country', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteCurrency')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteCurrency', N'Delete a currency', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteCustomerAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteCustomerAttribute', N'Delete a customer attribute', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteCustomerAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteCustomerAttributeValue', N'Delete a customer attribute value', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteEmailAccount')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteEmailAccount', N'Delete an email account', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteLanguage')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteLanguage', N'Delete a language', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteMeasureDimension')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteMeasureDimension', N'Delete a measure dimension', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteMeasureWeight')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteMeasureWeight', N'Delete a measure weight', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteMessageTemplate')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteMessageTemplate', N'Delete a message template', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteNews')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteNews', N'Delete a news', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'UninstallPlugin')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'UninstallPlugin', N'Uninstall a plugin', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteProductReview')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteProductReview', N'Delete a product review', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteStateProvince')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteStateProvince', N'Delete a state or province', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteStore')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteStore', N'Delete a store', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteVendor')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteVendor', N'Delete a vendor', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteWarehouse')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteWarehouse', N'Delete a warehouse', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditAddressAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditAddressAttribute', N'Edit an address attribute', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditAffiliate')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditAffiliate', N'Edit an affiliate', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditBlogPost')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditBlogPost', N'Edit a blog post', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditCampaign')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditCampaign', N'Edit a campaign', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditCountry')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditCountry', N'Edit a country', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditCurrency')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditCurrency', N'Edit a currency', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditCustomerAttribute')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditCustomerAttribute', N'Edit a customer attribute', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditCustomerAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditCustomerAttributeValue', N'Edit a customer attribute value', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditEmailAccount')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditEmailAccount', N'Edit an email account', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditLanguage')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditLanguage', N'Edit a language', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditMeasureDimension')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditMeasureDimension', N'Edit a measure dimension', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditMeasureWeight')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditMeasureWeight', N'Edit a measure weight', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditMessageTemplate')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditMessageTemplate', N'Edit a message template', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditNews')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditNews', N'Edit a news', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditPlugin')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditPlugin', N'Edit a plugin', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditProductReview')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditProductReview', N'Edit a product review', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditStateProvince')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditStateProvince', N'Edit a state or province', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditStore')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditStore', N'Edit a store', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditTask')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditTask', N'Edit a task', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditVendor')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditVendor', N'Edit a vendor', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditWarehouse')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditWarehouse', N'Edit a warehouse', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteBlogPostComment')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteBlogPostComment', N'Delete a blog post comment', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteNewsComment')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteNewsComment', N'Delete a news comment', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewAddressAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewAddressAttributeValue', N'Add a new address attribute value', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditAddressAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditAddressAttributeValue', N'Edit an address attribute value', N'true')
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteAddressAttributeValue')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteAddressAttributeValue', N'Delete an address attribute value', N'true')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productreviewpossibleonlyafterpurchasing')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.productreviewpossibleonlyafterpurchasing', N'False', 0)
END
GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportusedropdownlistsforassociatedentities')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'catalogsettings.exportimportusedropdownlistsforassociatedentities', N'True', 0)
 END
 GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showskuoncatalogpages')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.showskuoncatalogpages', N'False', 0)
END
GO

--rename settings
UPDATE [Setting] 
SET [Name] = N'catalogsettings.showskuonproductdetailspage' 
WHERE [Name] = N'catalogsettings.showproductsku'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductAttributeValue]') and NAME='CustomerEntersQty')
BEGIN
	ALTER TABLE [ProductAttributeValue]
	ADD [CustomerEntersQty] bit NULL
END
GO

UPDATE [ProductAttributeValue]
SET [CustomerEntersQty] = 0
WHERE [CustomerEntersQty] IS NULL
GO

ALTER TABLE [ProductAttributeValue] ALTER COLUMN [CustomerEntersQty] bit NOT NULL
GO

--new or update setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.renderassociatedattributevaluequantity')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'shoppingcartsettings.renderassociatedattributevaluequantity', N'True', 0);
END
ELSE
BEGIN
	UPDATE [Setting] 
	SET [Value] = N'True' 
	WHERE [Name] = N'shoppingcartsettings.renderassociatedattributevaluequantity'
END
GO

--update column
ALTER TABLE [RewardPointsHistory] ALTER COLUMN [PointsBalance] int NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.activationdelay')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.activationdelay', N'0', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.activationdelayperiodid')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.activationdelayperiodid', N'0', 0)
END
GO


--new discount coupon code logic
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] = 'Customer' and [Key] = 'DiscountCouponCode'
GO

--new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductAvailabilityRange]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[ProductAvailabilityRange](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] nvarchar(400) NOT NULL,
		[DisplayOrder] int NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='ProductAvailabilityRangeId')
BEGIN
	ALTER TABLE [Product]
	ADD [ProductAvailabilityRangeId] int NULL
END
GO

UPDATE [Product]
SET [ProductAvailabilityRangeId] = 0
WHERE [ProductAvailabilityRangeId] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [ProductAvailabilityRangeId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'paymentsettings.showpaymentmethoddescriptions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'paymentsettings.showpaymentmethoddescriptions', N'True', 0)
END
GO


--ensure that dbo is added to existing stored procedures
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[FullText_IsSupported]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [FullText_IsSupported]
GO
CREATE PROCEDURE [dbo].[FullText_IsSupported]
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
CREATE PROCEDURE [dbo].[FullText_Enable]
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



IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[FullText_Disable]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [FullText_Disable]
GO
CREATE PROCEDURE [dbo].[FullText_Disable]
AS
BEGIN
	EXEC('
	--drop indexes
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		DROP FULLTEXT INDEX ON [Product]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		DROP FULLTEXT INDEX ON [LocalizedProperty]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		DROP FULLTEXT INDEX ON [ProductTag]
	')

	--drop catalog
	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		DROP FULLTEXT CATALOG [nopCommerceFullTextCatalog]
	')
END
GO




IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[LanguagePackImport]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [LanguagePackImport]
GO
CREATE PROCEDURE [dbo].[LanguagePackImport]
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

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fixedorbyweightsettings.shippingbyweightenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'fixedorbyweightsettings.shippingbyweightenabled', N'False', 0)
END
GO

IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.activeshippingratecomputationmethodsystemnames' AND [Value] LIKE N'%Shipping.ByWeight%')
BEGIN
    UPDATE [Setting] 
    SET [Value] = N'True' 
    WHERE [Name] = N'fixedorbyweightsettings.shippingbyweightenabled'
END
GO

--rename settings
UPDATE [Setting] 
SET [Name] = N'fixedorbyweightsettings.limitmethodstocreated' 
WHERE [Name] = N'shippingbyweightsettings.limitmethodstocreated'
GO

--rename settings
UPDATE [Setting] 
SET [Name] = N'shippingratecomputationmethod.fixedorbyweight.rate.shippingmethodid' + SUBSTRING(name, 62, len(name))
WHERE [Name] like N'shippingratecomputationmethod.fixedrate.rate.shippingmethodid%'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowvendorstoimportproducts')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.allowvendorstoimportproducts', N'True', 0)
END
GO

--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[TierPrice]') and NAME='StartDateTimeUtc')
BEGIN
	ALTER TABLE [TierPrice]
	ADD [StartDateTimeUtc] datetime NULL
END
GO

--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[TierPrice]') and NAME='EndDateTimeUtc')
BEGIN
	ALTER TABLE [TierPrice]
	ADD [EndDateTimeUtc] datetime NULL
END
GO

--add a tier prices instead of product special prices 
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='SpecialPrice')
BEGIN
	EXEC('
        INSERT INTO [dbo].[TierPrice]([ProductId], [StoreId], [CustomerRoleId], [Quantity], [Price], [StartDateTimeUtc], [EndDateTimeUtc])
        SELECT [Id], 0, NULL, 1, [SpecialPrice], [SpecialPriceStartDateTimeUtc], [SpecialPriceEndDateTimeUtc]
        FROM [dbo].[Product]
        WHERE [SpecialPrice] <> 0')
END
GO

UPDATE [Product]
SET [HasTierPrices] = 1
WHERE [Id] IN (SELECT [ProductId] FROM [dbo].[TierPrice])
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='SpecialPrice')
BEGIN
	ALTER TABLE [Product] DROP COLUMN [SpecialPrice]
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='SpecialPriceStartDateTimeUtc')
BEGIN
	ALTER TABLE [Product] DROP COLUMN [SpecialPriceStartDateTimeUtc]
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='SpecialPriceEndDateTimeUtc')
BEGIN
	ALTER TABLE [Product] DROP COLUMN [SpecialPriceEndDateTimeUtc]
END
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'producteditordettings.specialprice'
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'producteditordettings.specialpricestartdate'
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'producteditordettings.specialpriceenddate'
GO

  --a stored procedure update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
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
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.deactivategiftcardsafterdeletingorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.deactivategiftcardsafterdeletingorder', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.completeorderwhendelivered')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.completeorderwhendelivered', N'True', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[NewsComment]') and NAME='IsApproved')
BEGIN
	ALTER TABLE [NewsComment]
	ADD [IsApproved] bit NULL
END
GO

UPDATE [NewsComment]
SET [IsApproved] = 1
WHERE [IsApproved] IS NULL
GO

ALTER TABLE [NewsComment] ALTER COLUMN [IsApproved] bit NOT NULL
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditNewsComment')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditNewsComment', N'Edited a news comment', N'true')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'newssettings.newscommentsmustbeapproved')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'newssettings.newscommentsmustbeapproved', N'False', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogComment]') and NAME='IsApproved')
BEGIN
	ALTER TABLE [BlogComment]
	ADD [IsApproved] bit NULL
END
GO

UPDATE [BlogComment]
SET [IsApproved] = 1
WHERE [IsApproved] IS NULL
GO

ALTER TABLE [BlogComment] ALTER COLUMN [IsApproved] bit NOT NULL
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditBlogComment')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditBlogComment', N'Edited a blog comment', N'true')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'blogsettings.blogcommentsmustbeapproved')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'blogsettings.blogcommentsmustbeapproved', N'False', 0)
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[News]') and NAME='CommentCount')
BEGIN
	ALTER TABLE [News] DROP COLUMN [CommentCount]
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogPost]') and NAME='CommentCount')
BEGIN
	ALTER TABLE [BlogPost] DROP COLUMN [CommentCount]
END
GO

-- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'NewReturnRequest.CustomerNotification')
 BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'NewReturnRequest.CustomerNotification', NULL, N'%Store.Name%. New return request.', N'<p>' + @NewLine + '<a href="%Store.URL%">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Hello %Customer.FullName%!' + @NewLine + '<br />' + @NewLine + 'You have just submitted a new return request. Details are below:' + @NewLine + '<br />' + @NewLine + 'Request ID: %ReturnRequest.CustomNumber%' + @NewLine + '<br />' + @NewLine + 'Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%' + @NewLine + '<br />' + @NewLine + 'Reason for return: %ReturnRequest.Reason%' + @NewLine + '<br />' + @NewLine + 'Requested action: %ReturnRequest.RequestedAction%' + @NewLine + '<br />' + @NewLine + 'Customer comments:' + @NewLine + '<br />' + @NewLine + '%ReturnRequest.CustomerComment%' + @NewLine + '</p>' + @NewLine, 1, 0, 0, 0, 0)
 END
 GO

 --new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantityHistory]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[StockQuantityHistory]
    (
		[Id] int IDENTITY(1,1) NOT NULL,
        [ProductId] int NOT NULL,
        [CombinationId] int NULL,
        [WarehouseId] int NULL,
		[QuantityAdjustment] int NOT NULL,
        [StockQuantity] int NOT NULL,
        [Message] NVARCHAR (MAX) NULL,
		[CreatedOnUtc] datetime NOT NULL
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'StockQuantityHistory_Product' AND parent_object_id = Object_id('StockQuantityHistory') AND Objectproperty(object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE [dbo].StockQuantityHistory
    DROP CONSTRAINT StockQuantityHistory_Product
END
GO

ALTER TABLE [dbo].[StockQuantityHistory] WITH CHECK ADD CONSTRAINT [StockQuantityHistory_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO

--initial stock quantity history
DECLARE cur_initialhistory CURSOR FOR
SELECT [Product].Id, NULL, [Product].WarehouseId, [Product].StockQuantity, NULL
FROM [Product]
UNION ALL
SELECT [ProductAttributeCombination].ProductId, [ProductAttributeCombination].Id, NULL, [ProductAttributeCombination].StockQuantity, NULL
FROM [ProductAttributeCombination]
UNION ALL
SELECT [ProductWarehouseInventory].ProductId, NULL, [ProductWarehouseInventory].WarehouseId, [ProductWarehouseInventory].StockQuantity, [ProductWarehouseInventory].Id
FROM [ProductWarehouseInventory]

DECLARE @productId int
DECLARE @combinationId int
DECLARE @warehouseId int
DECLARE @quantity int
DECLARE @warehouseInventoryId int

OPEN cur_initialhistory
FETCH NEXT FROM cur_initialhistory INTO @productId, @combinationId, @warehouseId, @quantity, @warehouseInventoryId

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @warehouseId = 0
    BEGIN
        SET @warehouseId = NULL;
    END
    
    DECLARE @message nvarchar(200)
    SET @message = 'Initialization of history table (original quantity set) during upgrade from a previous version'
    IF @warehouseInventoryId IS NOT NULL
    BEGIN
        SET @message = 'Multiple warehouses. ' + @message;
    END

	IF (@quantity IS NOT NULL AND @quantity <> 0 AND 
        NOT EXISTS (SELECT 1 FROM [StockQuantityHistory] WHERE ProductId = @productId AND 
            (CombinationId = @combinationId OR (CombinationId IS NULL AND @combinationId IS NULL)) AND (WarehouseId = @warehouseId OR (WarehouseId IS NULL AND @warehouseId IS NULL))))
	BEGIN
		INSERT INTO [StockQuantityHistory]
		    ([ProductId], [CombinationId], [WarehouseId], [QuantityAdjustment], [StockQuantity], [Message], [CreatedOnUtc])
		VALUES
		    (@productId, @combinationId, @warehouseId, @quantity, @quantity, @message, GETUTCDATE())
	END

	FETCH NEXT FROM cur_initialhistory INTO @productId, @combinationId, @warehouseId, @quantity, @warehouseInventoryId
END

CLOSE cur_initialhistory
DEALLOCATE cur_initialhistory
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'producteditorsettings.stockquantityhistory')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'producteditorsettings.stockquantityhistory', N'False', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='RequireReLogin')
BEGIN
	ALTER TABLE [Customer]
	ADD [RequireReLogin] bit NULL
END
GO

UPDATE [Customer]
SET [RequireReLogin] = 0
WHERE [RequireReLogin] IS NULL
GO

ALTER TABLE [Customer] ALTER COLUMN [RequireReLogin] bit NOT NULL
GO


--delete setting
DELETE FROM [Setting]
WHERE [name] = N'rewardpointssettings.PointsForPurchases_Awarded'
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'rewardpointssettings.PointsForPurchases_Canceled'
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'ordersettings.GiftCards_Activated_OrderStatusId'
GO

--delete setting
DELETE FROM [Setting]
WHERE [name] = N'ordersettings.GiftCards_Deactivated_OrderStatusId'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.activategiftcardsaftercompletingorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.activategiftcardsaftercompletingorder', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.deactivategiftcardsaftercancellingorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.deactivategiftcardsaftercancellingorder', N'False', 0)
END
GO


--update a stored procedure
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[LanguagePackImport]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [LanguagePackImport]
GO
CREATE PROCEDURE [dbo].[LanguagePackImport]
(
	@LanguageId int,
	@XmlPackage xml,
	@UpdateExistingResources bit
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

		INSERT INTO #LocaleStringResourceTmp (LanguageId, ResourceName, ResourceValue)
		SELECT	@LanguageId, nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/LocaleResource') AS R(nref)

		DECLARE @ResourceName nvarchar(200)
		DECLARE @ResourceValue nvarchar(MAX)
		DECLARE cur_localeresource CURSOR FOR
		SELECT LanguageId, ResourceName, ResourceValue
		FROM #LocaleStringResourceTmp
		OPEN cur_localeresource
		FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageId=@LanguageId AND ResourceName=@ResourceName))
			BEGIN
				IF (@UpdateExistingResources = 1)
				BEGIN
					UPDATE [LocaleStringResource]
					SET [ResourceValue]=@ResourceValue
					WHERE LanguageId=@LanguageId AND ResourceName=@ResourceName
				END
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


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'paymentsettings.skippaymentInfostepforredirectionpaymentmethods')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'paymentsettings.skippaymentInfostepforredirectionpaymentmethods', N'False', 0)
END
GO


--updated some indexes (required for case sensitive SQL Server collations)
IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_NewsletterSubscription_Email_StoreId' and object_id=object_id(N'[dbo].[NewsLetterSubscription]'))
BEGIN
	DROP INDEX [IX_NewsletterSubscription_Email_StoreId] ON [NewsLetterSubscription]
END
GO
CREATE NONCLUSTERED INDEX [IX_NewsletterSubscription_Email_StoreId] ON [NewsLetterSubscription] ([Email] ASC, [StoreId] ASC)
GO

IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Product_ShowOnHomepage' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
	DROP INDEX [IX_Product_ShowOnHomepage] ON [Product]
END
GO
CREATE NONCLUSTERED INDEX [IX_Product_ShowOnHomepage] ON [Product] ([ShowOnHomePage] ASC)
GO

--update a stored procedure
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[DeleteGuests]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [DeleteGuests]
GO
CREATE PROCEDURE [dbo].[DeleteGuests]
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
	SELECT [Id] FROM [Customer] c with (NOLOCK)
	WHERE
	--created from
	((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
	AND
	--created to
	((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
	AND
	--shopping cart items
	((@OnlyWithoutShoppingCart=0) OR (NOT EXISTS(SELECT 1 FROM [ShoppingCartItem] sci with (NOLOCK) inner join [Customer] with (NOLOCK) on sci.[CustomerId]=c.[Id])))
	AND
	--guests only
	(EXISTS(SELECT 1 FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK) inner join [Customer] with (NOLOCK) on ccrm.[Customer_Id]=c.[Id] inner join [CustomerRole] cr with (NOLOCK) on cr.[Id]=ccrm.[CustomerRole_Id] WHERE cr.[SystemName] = N'Guests'))
	AND
	--no orders
	(NOT EXISTS(SELECT 1 FROM [Order] o with (NOLOCK) inner join [Customer] with (NOLOCK) on o.[CustomerId]=c.[Id]))
	AND
	--no blog comments
	(NOT EXISTS(SELECT 1 FROM [BlogComment] bc with (NOLOCK) inner join [Customer] with (NOLOCK) on bc.[CustomerId]=c.[Id]))
	AND
	--no news comments
	(NOT EXISTS(SELECT 1 FROM [NewsComment] nc  with (NOLOCK)inner join [Customer] with (NOLOCK) on nc.[CustomerId]=c.[Id]))
	AND
	--no product reviews
	(NOT EXISTS(SELECT 1 FROM [ProductReview] pr with (NOLOCK) inner join [Customer] with (NOLOCK) on pr.[CustomerId]=c.[Id]))
	AND
	--no product reviews helpfulness
	(NOT EXISTS(SELECT 1 FROM [ProductReviewHelpfulness] prh with (NOLOCK) inner join [Customer] with (NOLOCK) on prh.[CustomerId]=c.[Id]))
	AND
	--no poll voting
	(NOT EXISTS(SELECT 1 FROM [PollVotingRecord] pvr with (NOLOCK) inner join [Customer] with (NOLOCK) on pvr.[CustomerId]=c.[Id]))
	AND
	--no forum topics 
	(NOT EXISTS(SELECT 1 FROM [Forums_Topic] ft with (NOLOCK) inner join [Customer] with (NOLOCK) on ft.[CustomerId]=c.[Id]))
	AND
	--no forum posts 
	(NOT EXISTS(SELECT 1 FROM [Forums_Post] fp with (NOLOCK) inner join [Customer] with (NOLOCK) on fp.[CustomerId]=c.[Id]))
	AND
	--no system accounts
	(c.IsSystemAccount = 0)
	
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
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.sitemapcustomurls')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.sitemapcustomurls', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.cartssharedbetweenstores')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shoppingcartsettings.cartssharedbetweenstores', N'False', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='EmailToRevalidate')
BEGIN
	ALTER TABLE [Customer]
	ADD [EmailToRevalidate] nvarchar(1000) NULL
END
GO

-- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'Customer.EmailRevalidationMessage')
 BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'Customer.EmailRevalidationMessage', NULL, N'%Store.Name%. Email validation.', N'<p>' + @NewLine + '<a href="%Store.URL%">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Hello %Customer.FullName%!' + @NewLine + '<br />' + @NewLine + 'To validate your new email address <a href="%Customer.EmailRevalidationURL%">click here</a> .' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Store.Name%' + @NewLine + '</p>' + @NewLine, 1, 0, 0, 0, 0)
 END
 GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='RewardPointsHistoryEntryId')
BEGIN
	ALTER TABLE [Order]
	ADD [RewardPointsHistoryEntryId] int NULL
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='RewardPointsWereAdded')
BEGIN
    --column RewardPointsWereAdded was replaced with RewardPointsHistoryEntryId
    --ensure that the new column value is not null to specify that reward points were added (earned) to an order
    EXEC('
        UPDATE [Order]
        SET [RewardPointsHistoryEntryId] = 0
        WHERE [RewardPointsWereAdded] = 1')
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='RewardPointsWereAdded')
BEGIN
	ALTER TABLE [Order] DROP COLUMN [RewardPointsWereAdded]
END
GO


--update plugin locales (renamed)
UPDATE [LocaleStringResource]
SET [ResourceName] = REPLACE([ResourceName], 'Plugins.Feed.Froogle.','Plugins.Feed.GoogleShopping.')
WHERE [ResourceName] like 'Plugins.Feed.Froogle.%'
GO

--update settings
UPDATE [Setting]
SET [Name] = REPLACE([Name], 'frooglesettings.','googlesShoppingsettings.')
WHERE [Name] like 'frooglesettings.%'
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductTemplate]') and NAME='IgnoredProductTypes')
BEGIN
	ALTER TABLE [ProductTemplate]
	ADD [IgnoredProductTypes] nvarchar(MAX) NULL
END
GO

UPDATE [ProductTemplate]
SET [IgnoredProductTypes] = '10'
WHERE [ViewPath] = N'ProductTemplate.Simple'
GO

UPDATE [ProductTemplate]
SET [IgnoredProductTypes] = '5'
WHERE [ViewPath] = N'ProductTemplate.Grouped'
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ReturnRequest]') and NAME='UploadedFileId')
BEGIN
	ALTER TABLE [ReturnRequest]
	ADD [UploadedFileId] int NULL
END
GO

UPDATE [ReturnRequest]
SET [UploadedFileId] = 0
WHERE [UploadedFileId] IS NULL
GO

ALTER TABLE [ReturnRequest] ALTER COLUMN [UploadedFileId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.returnrequestsallowfiles')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.returnrequestsallowfiles', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.returnrequestsfilemaximumsize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'ordersettings.returnrequestsfilemaximumsize', N'2048', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReview]') and NAME='ReplyText')
BEGIN
	ALTER TABLE [ProductReview]
	ADD [ReplyText] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[BlogComment]') and NAME='StoreId')
BEGIN
   ALTER TABLE [dbo].[BlogComment]
   ADD [StoreId] int NULL
END
GO

DECLARE @DefaultStoreId int
SET @DefaultStoreId = (SELECT TOP (1) Id FROM [dbo].[Store]);
--set default value to store column
UPDATE [dbo].[BlogComment]
SET StoreId = @DefaultStoreId
WHERE StoreId IS NULL
GO
 
ALTER TABLE [dbo].[BlogComment] ALTER COLUMN [StoreId] int NOT NULL
GO
 
IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'BlogComment_Store' AND parent_object_id = Object_id('BlogComment') AND Objectproperty(object_id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[BlogComment]
DROP CONSTRAINT BlogComment_Store
GO
 
ALTER TABLE [dbo].[BlogComment] WITH CHECK ADD CONSTRAINT [BlogComment_Store] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Store] ([Id])
ON DELETE CASCADE
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'blogsettings.showblogcommentsperstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'blogsettings.showblogcommentsperstore', N'False', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[NewsComment]') and NAME='StoreId')
BEGIN
   ALTER TABLE [dbo].[NewsComment]
   ADD [StoreId] int NULL
END
GO

DECLARE @DefaultStoreId int
SET @DefaultStoreId = (SELECT TOP (1) Id FROM [dbo].[Store]);
--set default value to store column
UPDATE [dbo].[NewsComment]
SET StoreId = @DefaultStoreId
WHERE StoreId IS NULL
GO
 
ALTER TABLE [dbo].[NewsComment] ALTER COLUMN [StoreId] int NOT NULL
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'NewsComment_Store' AND parent_object_id = Object_id('NewsComment') AND Objectproperty(object_id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[NewsComment]
DROP CONSTRAINT NewsComment_Store
GO
 
ALTER TABLE [dbo].[NewsComment] WITH CHECK ADD CONSTRAINT [NewsComment_Store] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Store] ([Id])
ON DELETE CASCADE
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'newssettings.shownewscommentsperstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'newssettings.shownewscommentsperstore', N'False', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='AddressId')
BEGIN
	ALTER TABLE [Vendor]
	ADD [AddressId] int NULL
END
GO

UPDATE [Vendor]
SET [AddressId] = 0
WHERE [AddressId] IS NULL
GO

ALTER TABLE [Vendor] ALTER COLUMN [AddressId] int NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.failedpasswordallowedattempts')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.failedpasswordallowedattempts', N'0', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.failedpasswordlockoutminutes')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.failedpasswordlockoutminutes', N'30', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='FailedLoginAttempts')
BEGIN
	ALTER TABLE [Customer]
	ADD [FailedLoginAttempts] int NULL
END
GO

UPDATE [Customer]
SET [FailedLoginAttempts] = 0
WHERE [FailedLoginAttempts] IS NULL
GO

ALTER TABLE [Customer] ALTER COLUMN [FailedLoginAttempts] int NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='CannotLoginUntilDateUtc')
BEGIN
	ALTER TABLE [Customer]
	ADD [CannotLoginUntilDateUtc] datetime NULL
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fixedorbycountrystateziptaxsettings.countrystatezipenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'fixedorbycountrystateziptaxsettings.countrystatezipenabled', N'False', 0)
END
GO

--rename settings
UPDATE [Setting] 
SET [Name] = N'tax.taxprovider.fixedorbycountrystatezip.taxcategoryid' + SUBSTRING(name, 40, len(name))
WHERE [Name] like N'tax.taxprovider.fixedrate.taxcategoryid%'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='RegisteredInStoreId')
BEGIN
   ALTER TABLE [dbo].[Customer]
   ADD    [RegisteredInStoreId] int NULL
END
GO

declare @DefaultStoreId int;
if ((select count(id) from [dbo].[Store]) = 1)
set @DefaultStoreId = (select top(1) id from [dbo].[Store])
else
set @DefaultStoreId = 0;
--set default value to store column
UPDATE [dbo].[Customer] set [RegisteredInStoreId] = @DefaultStoreId where [RegisteredInStoreId] is NULL

ALTER TABLE [dbo].[Customer] ALTER COLUMN [RegisteredInStoreId] int NOT NULL
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.considerassociatedproductsdimensions')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'shippingsettings.considerassociatedproductsdimensions', N'True', 0)
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.bbcodeeditoropenlinksinnewwindow')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.bbcodeeditoropenlinksinnewwindow', N'false', 0)
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'paymentsettings.cancelrecurringpaymentsafterfailedpayment')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'paymentsettings.cancelrecurringpaymentsafterfailedpayment', N'False', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RecurringPayment]') and NAME='LastPaymentFailed')
BEGIN
	ALTER TABLE [RecurringPayment]
	ADD [LastPaymentFailed] bit NULL
END
GO

UPDATE [RecurringPayment]
SET [LastPaymentFailed] = 0
WHERE [LastPaymentFailed] IS NULL
GO

ALTER TABLE [RecurringPayment] ALTER COLUMN [LastPaymentFailed] bit NOT NULL
GO

-- new message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'RecurringPaymentCancelled.CustomerNotification')
BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
    INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
    VALUES (N'RecurringPaymentCancelled.CustomerNotification', NULL, N'%Store.Name%. Recurring payment cancelled', N'<p>' + @NewLine + '<a href=\"%Store.URL%\">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Hello %Customer.FullName%,' + @NewLine + '<br />' + @NewLine + '%if (%RecurringPayment.CancelAfterFailedPayment%) It appears your credit card didn''t go through for this recurring payment (<a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>)' + @NewLine + '<br />' + @NewLine + 'So your subscription has been canceled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) The recurring payment ID=%RecurringPayment.ID% was cancelled. endif%' + @NewLine + '</p>' + @NewLine, 1, 0, 0, 0, 0)
END
GO

-- new message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'RecurringPaymentFailed.CustomerNotification')
BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
    INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
    VALUES (N'RecurringPaymentFailed.CustomerNotification', NULL, N'%Store.Name%. Last recurring payment failed', N'<p>' + @NewLine + '<a href=\"%Store.URL%\">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Hello %Customer.FullName%,' + @NewLine + '<br />' + @NewLine + 'It appears your credit card didn''t go through for this recurring payment (<a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>)' + @NewLine + '<br /> %if (%RecurringPayment.RecurringPaymentType% == "Manual") ' + @NewLine + 'You can recharge balance and manually retry payment or cancel it on the order history page. endif% %if (%RecurringPayment.RecurringPaymentType% == "Automatic") ' + @NewLine + 'You can recharge balance and wait, we will try to make the payment again, or you can cancel it on the order history page. endif%' + @NewLine + '</p>' + @NewLine, 1, 0, 0, 0, 0)
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='CustomOrderNumber')
BEGIN
	ALTER TABLE [Order]
	ADD [CustomOrderNumber] nvarchar(MAX) NULL
END
GO

UPDATE [Order]
SET [CustomOrderNumber] = [id]
WHERE [CustomOrderNumber] IS NULL
GO

ALTER TABLE [Order] ALTER COLUMN [CustomOrderNumber] nvarchar(MAX) NOT NULL
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.customordernumbermask')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'ordersettings.customordernumbermask', N'{ID}', 0)
END
GO

 --new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPassword]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[CustomerPassword]
    (
		[Id] int IDENTITY(1,1) NOT NULL,
        [CustomerId] int NOT NULL,
		[Password] NVARCHAR (MAX) NULL,
        [PasswordFormatId] INT NOT NULL,
        [PasswordSalt] NVARCHAR (MAX) NULL,
		[CreatedOnUtc] datetime NOT NULL
		PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'CustomerPassword_Customer' AND parent_object_id = Object_id('CustomerPassword') AND Objectproperty(object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE [dbo].CustomerPassword
    DROP CONSTRAINT CustomerPassword_Customer
END
GO

ALTER TABLE [dbo].[CustomerPassword] WITH CHECK ADD CONSTRAINT [CustomerPassword_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO

--move customer passwords into a new table
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and (NAME='Password' or NAME='PasswordFormatId' or NAME='PasswordSalt'))
BEGIN
    EXEC('
        INSERT INTO [dbo].[CustomerPassword]([CustomerId], [Password], [PasswordFormatId], [PasswordSalt], [CreatedOnUtc])
        SELECT [Id], [Password], [PasswordFormatId], [PasswordSalt], [CreatedOnUtc]
        FROM [dbo].[Customer]')
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='Password')
BEGIN
	ALTER TABLE [Customer] DROP COLUMN [Password]
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='PasswordFormatId')
BEGIN
	ALTER TABLE [Customer] DROP COLUMN [PasswordFormatId]
END
GO

--drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') and NAME='PasswordSalt')
BEGIN
	ALTER TABLE [Customer] DROP COLUMN [PasswordSalt]
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.unduplicatedpasswordsnumber')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.unduplicatedpasswordsnumber', N'4', 0)
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.passwordlifetime')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordlifetime', N'90', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CustomerRole]') and NAME='EnablePasswordLifetime')
BEGIN
	ALTER TABLE [CustomerRole]
	ADD [EnablePasswordLifetime] bit NULL
END
GO

UPDATE [CustomerRole]
SET [EnablePasswordLifetime] = 0
WHERE [EnablePasswordLifetime] IS NULL
GO

ALTER TABLE [CustomerRole] ALTER COLUMN [EnablePasswordLifetime] bit NOT NULL
GO

-- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'Service.ContactUs')
 BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'Service.ContactUs', NULL, N'%Store.Name%. Contact us', N'<p>' + @NewLine + '%ContactUs.Body%' + @NewLine + '</p>' + @NewLine, 1, 0, 0, 0, 0)
 END
 GO

-- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'Service.ContactVendor')
 BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'Service.ContactVendor', NULL, N'%Store.Name%. Contact us', N'<p>' + @NewLine + '%ContactUs.Body%' + @NewLine + '</p>' + @NewLine, 1, 0, 0, 0, 0)
 END
 GO

 --now vendors have "Manage product reviews" permission
IF EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageProductReviews')
BEGIN
	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = (SELECT [Id] FROM [dbo].[PermissionRecord] WHERE [SystemName] = N'ManageProductReviews')

	--add it to vendor role by default
	DECLARE @VendorCustomerRoleId int
	SELECT @VendorCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Vendors'

	IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord_Role_Mapping]
		WHERE [PermissionRecord_Id] = @PermissionRecordId AND [CustomerRole_Id] = @VendorCustomerRoleId)
	BEGIN
		INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
		VALUES (@PermissionRecordId, @VendorCustomerRoleId)
	END
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'taxsettings.taxbasedonpickuppointaddress')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'taxsettings.taxbasedonpickuppointaddress', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.exportwithproducts')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'ordersettings.exportwithproducts', N'True', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='InteractionTypeId')
BEGIN
	ALTER TABLE [DiscountRequirement]
	ADD [InteractionTypeId] int NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='ParentId')
BEGIN
	ALTER TABLE [DiscountRequirement]
	ADD [ParentId] int NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='IsGroup')
BEGIN
	ALTER TABLE [DiscountRequirement]
	ADD [IsGroup] bit NULL
END
GO

UPDATE [DiscountRequirement]
SET [IsGroup] = 0
WHERE [IsGroup] IS NULL
GO

ALTER TABLE [DiscountRequirement] ALTER COLUMN [IsGroup] bit NOT NULL
GO

--add default requirement group for existing requirements
DECLARE cursor_defaultGroup CURSOR FOR SELECT [DiscountId] FROM [DiscountRequirement]
DECLARE @discountId int

OPEN cursor_defaultGroup
FETCH NEXT FROM cursor_defaultGroup INTO @discountId
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [DiscountRequirement] WHERE [DiscountId] = @discountId AND [ParentId] IS NULL AND [IsGroup] = 1)
    BEGIN
        INSERT INTO [DiscountRequirement]
		    ([DiscountId], [DiscountRequirementRuleSystemName], [InteractionTypeId], [ParentId], [IsGroup])
	    VALUES
		    (@discountId, 'Default requirement group', 0, NULL, 1);

        DECLARE @requirementId int = (SELECT SCOPE_IDENTITY());

        UPDATE [DiscountRequirement]
        SET [ParentId] = @requirementId, [InteractionTypeId] = NULL
        WHERE [DiscountId] = @discountId AND [Id] <> @requirementId
    END
	FETCH NEXT FROM cursor_defaultGroup INTO @discountId
END

CLOSE cursor_defaultGroup
DEALLOCATE cursor_defaultGroup
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.useisodatetimeconverterinjson')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'adminareasettings.useisodatetimeconverterinjson', N'True', 0)
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportCategories')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'ImportCategories', N'Categories were imported', N'True')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportManufacturers')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'ImportManufacturers', N'Manufacturers were imported', N'True')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportProducts')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'ImportProducts', N'Products were imported', N'True')
END
GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportStates')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'ImportStates', N'States and provinces were imported', N'True')
END
GO

--update DownloadActivationType according to the new enum value 
UPDATE [Product]
SET [DownloadActivationTypeId] = 0
WHERE [DownloadActivationTypeId] = 1
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Currency]') and NAME='RoundingTypeId')
BEGIN
	ALTER TABLE [Currency]
	ADD [RoundingTypeId] INT NULL
END
GO

UPDATE [Currency]
SET [RoundingTypeId] = 0
WHERE [RoundingTypeId] IS NULL
GO

-- Rounding with 1.00 intervals (The system used in Sweden since 30 September 2010. https://en.wikipedia.org/wiki/Cash_rounding#Rounding_with_1.00_intervals)
UPDATE [Currency]
SET [RoundingTypeId] = 60
WHERE [CurrencyCode] = 'SEK'
GO

ALTER TABLE [Currency] ALTER COLUMN [RoundingTypeId] INT NOT NULL
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.usericheditorinmessagetemplates')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'adminareasettings.usericheditorinmessagetemplates', N'False', 0)
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.azurecachecontrolheader')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'mediasettings.azurecachecontrolheader', N'', 0)
END
GO

--add stored procedure for getting category tree
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[nop_padright]') AND xtype IN (N'FN', N'IF', N'TF'))
DROP FUNCTION  [dbo].[nop_padright]
GO

CREATE FUNCTION [dbo].[nop_padright]
(
    @source INT, 
    @symbol NVARCHAR(MAX), 
    @length INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN RIGHT(REPLICATE(@symbol, @length)+ RTRIM(CAST(@source AS NVARCHAR(MAX))), @length)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CategoryLoadAllPaged]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CategoryLoadAllPaged]
GO

CREATE PROCEDURE [dbo].[CategoryLoadAllPaged]
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
    AS (SELECT [Category].[Id] AS [Id], dbo.[nop_padright] ([Category].[DisplayOrder], '0', @lengthOrder) + '-' + dbo.[nop_padright] ([Category].[Id], '0', @lengthId) AS [Order]
        FROM [Category] WHERE [Category].[ParentCategoryId] = 0
        UNION ALL
        SELECT [Category].[Id] AS [Id], [CategoryTree].[Order] + '|' + dbo.[nop_padright] ([Category].[DisplayOrder], '0', @lengthOrder) + '-' + dbo.[nop_padright] ([Category].[Id], '0', @lengthId) AS [Order]
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
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.usestoredprocedureforloadingcategories')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.usestoredprocedureforloadingcategories', N'False', 0)
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.displayminiprofilerforadminonly')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'storeinformationsettings.displayminiprofilerforadminonly', N'False', 0)
END
GO

--indicating whether to display default menu items
DECLARE @displayMenuItems bit
IF NOT EXISTS (SELECT 1 FROM [Category] where ParentCategoryId=0 and Deleted=0 and Published=1)
	set @displayMenuItems = N'True'
ELSE
    set @displayMenuItems = N'False'

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayhomepagemenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayhomepagemenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displaynewproductsmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displaynewproductsmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayproductsearchmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayproductsearchmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displaycustomerinfomenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displaycustomerinfomenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayblogmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayblogmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayforumsmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayforumsmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displaycontactusmenuitem ')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displaycontactusmenuitem ', @displayMenuItems, 0)
END
GO
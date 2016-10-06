--upgrade scripts from nopCommerce 3.80 to next version

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
  <LocaleResource Name=" Admin.Catalog.Categories.Fields.Parent.None">
    <Value>[None]</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.HideShippingTotal">
    <Value>Hide shipping total if shipping not required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.HideShippingTotal.Hint">
    <Value>Check if you want Hide ''Shipping total'' label if shipping not required.</Value>
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

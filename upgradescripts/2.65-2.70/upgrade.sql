--upgrade scripts from nopCommerce 2.65 to nopCommerce 2.70

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="GiftCardAttribute.For">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.For.Virtual">
    <Value><![CDATA[For: {0} <{1}>]]></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From.Virtual">
    <Value><![CDATA[From: {0} <{1}>]]></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.For.Physical">
    <Value>For: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From.Physical">
    <Value>From: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart">
    <Value>Move items from wishlist to cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart.Hint">
    <Value>Check to move products from wishlist to the cart when clicking "Add to cart" button. Otherwise, they are copied.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Result.Continue">
    <Value>Continue</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterForeignKeyEq">
    <Value>Is equal to</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterForeignKeyNe">
    <Value>Is not equal to</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterOr">
    <Value>Or</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterStringNotSubstringOf">
    <Value>Does not contain</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.Gender">
    <Value>Gender</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.AgeGroup">
    <Value>Age group</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.Color">
    <Value>Color</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.Size">
    <Value>Size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.RecurringPayment">
    <Value>Recurring payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.RecurringPayment.Hint">
    <Value>This is a recurring order. See the appropriate recurring payment record.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.IsEnabled">
    <Value>Is enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.IsEnabled.Hint">
    <Value>Indicates whether the plugin is enabled/active.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks">
    <Value>Schedule tasks</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Name.Required">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Seconds">
    <Value>Seconds (run period)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Seconds.Positive">
    <Value>Seconds should be positive</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Enabled">
    <Value>Enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.StopOnError">
    <Value>Stop on error</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.LastStart">
    <Value>Last start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.LastEnd">
    <Value>Last end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.LastSuccess">
    <Value>Last success date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.RestartApplication">
    <Value>Do not forgot to restart the application once a task has been modified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.BillingCountry">
    <Value>Billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.BillingCountry.Hint">
    <Value>Filter by order billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-product'' to make your page URL ''http://www.yourStore.com/the-best-product''. Leave empty to generate it automatically based on the name of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-category'' to make your page URL ''http://www.yourStore.com/the-best-category''. Leave empty to generate it automatically based on the name of the category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-manufacturer'' to make your page URL ''http://www.yourStore.com/the-best-manufacturer''. Leave empty to generate it automatically based on the name of the manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Common.PageTitleSeoAdjustment.PagenameAfterStorename">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Common.PageTitleSeoAdjustment.StorenameAfterPagename">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Seo.PageTitleSeoAdjustment.PagenameAfterStorename">
    <Value>Page name comes after store name</Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Seo.PageTitleSeoAdjustment.StorenameAfterPagename">
    <Value>Store name comes after page name</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.SendPM">
    <Value>Sent PM to customer (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.ContactUs">
    <Value>Used contact us form</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.AddToCompareList">
    <Value>Added a product to compare list (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.AddToShoppingCart">
    <Value>Added a product to shopping cart (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.AddToWishlist">
    <Value>Added a product to wishlist (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.Login">
    <Value>Login</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.Logout">
    <Value>Logout</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.AddProductReview">
    <Value>Added a product review (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.AddNewsComment">
    <Value>Added a news comment</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.PublicStore.AddBlogComment">
    <Value>Added a blog comment</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Manual.Fields.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CashOnDelivery.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CashOnDelivery.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CheckMoneyOrder.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.CheckMoneyOrder.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PayInStore.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PayInStore.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PurchaseOrder.AdditionalFeePercentage">
    <Value>Additinal fee. Use percentage</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payment.PurchaseOrder.AdditionalFeePercentage.Hint">
    <Value>Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SubjectToAcl">
    <Value>Subject to ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SubjectToAcl.Hint">
    <Value>Determines whether the product is subject to ACL (access control list).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Acl">
    <Value>ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles">
    <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the product will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Acl">
    <Value>ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AclCustomerRoles">
    <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the category will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.SubjectToAcl">
    <Value>Subject to ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.SubjectToAcl.Hint">
    <Value>Determines whether the category is subject to ACL (access control list).</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.StoreClosed">
    <Value>Store closed</Value>
  </LocaleResource>
  <LocaleResource Name="StoreClosed">
    <Value>This store is currently closed</Value>
  </LocaleResource>
  <LocaleResource Name="StoreClosed.Hint">
    <Value>Please check back in a little while.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FormFields">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.FormFields.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CustomerFormFields">
    <Value>Customer form fields</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CustomerFormFields.Description">
    <Value>You can create and manage the customer form fields available during registration below.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields">
    <Value>Address form fields</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.Description">
    <Value>You can create and manage the address form fields available during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyEnabled">
    <Value>''Company'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyEnabled.Hint">
    <Value>Set if ''Company'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyRequired">
    <Value>''Company'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyRequired.Hint">
    <Value>Check if ''Company'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressEnabled">
    <Value>''Street address'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressEnabled.Hint">
    <Value>''Street address'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressRequired">
    <Value>''Street address'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressRequired.Hint">
    <Value>Check if ''Street address'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Enabled">
    <Value>''Street address 2'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Enabled.Hint">
    <Value>Set if ''Street address 2'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Required">
    <Value>''Street address 2'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Required.Hint">
    <Value>Check if ''Street address 2'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeEnabled">
    <Value>''Zip / postal code'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeEnabled.Hint">
    <Value>Set if ''Zip / postal code'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeRequired">
    <Value>''Zip / postal code'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeRequired.Hint">
    <Value>Check if ''Zip / postal code'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityEnabled">
    <Value>''City'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityEnabled.Hint">
    <Value>Set if ''City'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityRequired">
    <Value>''City'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityRequired.Hint">
    <Value>Check if ''City'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountryEnabled">
    <Value>''Country'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountryEnabled.Hint">
    <Value>Set if ''Country'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StateProvinceEnabled">
    <Value>''State/province'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.StateProvinceEnabled.Hint">
    <Value>Set if ''State/province'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneEnabled">
    <Value>''Phone number'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneEnabled.Hint">
    <Value>Set if ''Phone number'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneRequired">
    <Value>''Phone number'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneRequired.Hint">
    <Value>Check if ''Phone number'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxEnabled">
    <Value>''Fax number'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxEnabled.Hint">
    <Value>Set if ''Fax number'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxRequired">
    <Value>''Fax number'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxRequired.Hint">
    <Value>Check if ''Fax number'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.EmailAccounts.Fields.Password.Change">
    <Value>Change password</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.EmailAccounts.Fields.Password.PasswordChanged">
    <Value>The password has been changed successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Password.Change">
    <Value>Change password</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase.Note">
    <Value>NOTE: Do not forget to backup your database before changing this option</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.DataHtml">
    <Value>Data</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost">
    <Value>Additional fixed cost</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost.Hint">
    <Value>Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don''t want an additional fixed cost to be applied.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.UsePercentage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.UsePercentage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal">
    <Value>Charge percentage (of subtotal)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal.Hint">
    <Value>Charge percentage (of subtotal).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit">
    <Value>Rate per weight unit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit.Hint">
    <Value>Rate per weight unit.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Formula">
    <Value>Formula to calculate rates</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Formula.Value">
    <Value>[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.LowerWeightLimit">
    <Value>Lower weight limit</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.LowerWeightLimit.Hint">
    <Value>Lower weight limit. This field can be used for "per extra weight unit" scenarios.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.GiftCards">
    <Value>Gift card(s)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics">
    <Value>Topics (pages)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpHostname">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpHostname.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpFilename">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpFilename.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpUsername">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpUsername.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpPassword">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpPassword.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.FtpUploadStatus">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Upload">
    <Value></Value>
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







IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[TaxRate]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	EXEC('ALTER TABLE [TaxRate] ALTER COLUMN [Percentage] decimal(18, 4) NOT NULL')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.moveitemsfromwishlisttocart')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.moveitemsfromwishlisttocart', N'true')
END
GO


--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageScheduleTasks')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Schedule Tasks', N'ManageScheduleTasks', N'Configuration')

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

--more SQL indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ActivityLog_CreatedOnUtc' and object_id=object_id(N'[ActivityLog]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
END
GO







--New search engine friendly URLs implementation

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'seosettings.reservedurlrecordslugs', N'admin,install,recentlyviewedproducts,newproducts,compareproducts,clearcomparelist,setproductreviewhelpfulness,login,register,logout,cart,wishlist,emailwishlist,checkout,onepagecheckout,contactus,passwordrecovery,subscribenewsletter,blog,boards,inboxupdate,sentupdate,news,sitemap,sitemapseo,search,config,eucookielawaccept')
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[UrlRecord]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[UrlRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityName] nvarchar(400) NOT NULL,
	[Slug] nvarchar(400) NOT NULL,
	[LanguageId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
--new indexes
IF EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_UrlRecord_Slug' and object_id=object_id(N'[UrlRecord]'))
BEGIN
	--this drop is only for users of BETA version of 2.70
	DROP INDEX [IX_UrlRecord_Slug] ON [UrlRecord]
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_UrlRecord_Slug' and object_id=object_id(N'[UrlRecord]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
END
GO


IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[temp_generate_sename]
GO
CREATE PROCEDURE [dbo].[temp_generate_sename]
(
    @table_name nvarchar(1000),
    @entity_id int,
    @language_id int = 0, --0 to process main sename column, --language id to process a localized value
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
	--get current name
	DECLARE @current_sename nvarchar(1000)
	DECLARE @sql nvarchar(4000)
	
	IF (@language_id = 0)
	BEGIN
		SET @sql = 'SELECT @current_sename = [SeName] FROM [' + @table_name + '] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT
		
		--if not empty, se name is already specified by a store owner. if empty, we should use product name
		IF (@current_sename is null or @current_sename = N'')
		BEGIN
			SET @sql = 'SELECT @current_sename = [Name] FROM [' + @table_name + '] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
			EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT		
		END
    END
    ELSE
    BEGIN
		SET @sql = 'SELECT @current_sename = [LocaleValue] FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT
		
		--if not empty, se name is already specified by a store owner. if empty, we should use poduct name
		IF (@current_sename is null or @current_sename = N'')
		BEGIN
			SET @sql = 'SELECT @current_sename = [LocaleValue] FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''Name'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
			EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT		
		END
		
		--if localized product name is also empty, we exit
		IF (@current_sename is null or @current_sename = N'')
			RETURN
    END
    
    --generate se name    
	DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars varchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@current_sename)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
		DECLARE @c nvarchar(1)
        SET @c = substring(@current_sename, @p, 1)
        IF CHARINDEX(@c,@allowed_se_chars) > 0
        BEGIN
			SET @new_sename = @new_sename + @c
		END
		SET @p = @p + 1
	END
	--replace spaces with '-'
	SELECT @new_sename = REPLACE(@new_sename,' ','-');
    WHILE CHARINDEX('--',@new_sename) > 0
		SELECT @new_sename = REPLACE(@new_sename,'--','-');
    WHILE CHARINDEX('__',@new_sename) > 0
		SELECT @new_sename = REPLACE(@new_sename,'__','_');
    --ensure not empty
    IF (@new_sename is null or @new_sename = '')
		SELECT @new_sename = ISNULL(CAST(@entity_id AS nvarchar(max)), '0');
    --lowercase
	SELECT @new_sename = LOWER(@new_sename)
	--ensure this sename is not reserved
	WHILE (1=1)
	BEGIN
		DECLARE @sename_is_already_reserved bit
		SET @sename_is_already_reserved = 0
		SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename AND [EntityId] <> ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0') + ')
					BEGIN
						SELECT @sename_is_already_reserved = 1
					END'
		EXEC sp_executesql @sql,N'@sename nvarchar(1000), @sename_is_already_reserved nvarchar(4000) OUTPUT',@new_sename,@sename_is_already_reserved OUTPUT
		
		IF (@sename_is_already_reserved > 0)
		BEGIN
			--add some digit to the end in this case
			SET @new_sename = @new_sename + '-1'
		END
		ELSE
		BEGIN
			BREAK
		END
	END
	
	--return
    SET @result = @new_sename
END
GO

--update [sename] column for products
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='SeName')
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Product]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Product'
		
		--main sename
		EXEC	[dbo].[temp_generate_sename]
				@table_name = @table_name,
				@entity_id = @sename_existing_entity_id,
				@language_id = 0,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 0)
		END		

		--localized values
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT [ID]
		FROM [Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			SET @sename = null -- clear cache (variable scope)
			
			EXEC	[dbo].[temp_generate_sename]
					@table_name = @table_name,
					@entity_id = @sename_existing_entity_id,
					@language_id = @ExistingLanguageID,
					@result = @sename OUTPUT
			IF (len(@sename) > 0)
			BEGIN
				
				DECLARE @sql nvarchar(4000)
				--insert
				SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
				BEGIN
					--update
					UPDATE [UrlRecord]
					SET [Slug] = @sename
					WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
					VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
				END
				'				
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
				
				
				--delete
				SET @sql = 'DELETE FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0')
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
			END

			--fetch next language identifier
			FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
		CLOSE cur_existinglanguage
		DEALLOCATE cur_existinglanguage
		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
	
	--drop SeName column
	EXEC('ALTER TABLE [Product] DROP COLUMN [SeName]')
	
END
GO



--update [sename] column for categories
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Category]') and NAME='SeName')
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Category]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Category'
		
		--main sename
		EXEC	[dbo].[temp_generate_sename]
				@table_name = @table_name,
				@entity_id = @sename_existing_entity_id,
				@language_id = 0,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 0)
		END		

		--localized values
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT [ID]
		FROM [Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			SET @sename = null -- clear cache (variable scope)
			
			EXEC	[dbo].[temp_generate_sename]
					@table_name = @table_name,
					@entity_id = @sename_existing_entity_id,
					@language_id = @ExistingLanguageID,
					@result = @sename OUTPUT
			IF (len(@sename) > 0)
			BEGIN
				
				DECLARE @sql nvarchar(4000)
				SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
				BEGIN
					--update
					UPDATE [UrlRecord]
					SET [Slug] = @sename
					WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
					VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
				END
				'
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
				
				
				--delete
				SET @sql = 'DELETE FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0')
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
			END
					

			--fetch next language identifier
			FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
		CLOSE cur_existinglanguage
		DEALLOCATE cur_existinglanguage
		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
	
	--drop SeName column
	EXEC('ALTER TABLE [Category] DROP COLUMN [SeName]')
END
GO




--update [sename] column for categories
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Manufacturer]') and NAME='SeName')
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Manufacturer]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Manufacturer'
		
		--main sename
		EXEC	[dbo].[temp_generate_sename]
				@table_name = @table_name,
				@entity_id = @sename_existing_entity_id,
				@language_id = 0,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 0)
		END		

		--localized values
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT [ID]
		FROM [Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			SET @sename = null -- clear cache (variable scope)
			
			EXEC	[dbo].[temp_generate_sename]
					@table_name = @table_name,
					@entity_id = @sename_existing_entity_id,
					@language_id = @ExistingLanguageID,
					@result = @sename OUTPUT
			IF (len(@sename) > 0)
			BEGIN
				
				DECLARE @sql nvarchar(4000)
				SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
				BEGIN
					--update
					UPDATE [UrlRecord]
					SET [Slug] = @sename
					WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
					VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
				END
				'
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
				
				
				--delete
				SET @sql = 'DELETE FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0')
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
			END
					

			--fetch next language identifier
			FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
		CLOSE cur_existinglanguage
		DEALLOCATE cur_existinglanguage
		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
	
	--drop SeName column
	EXEC('ALTER TABLE [Manufacturer] DROP COLUMN [SeName]')
END
GO
--drop temporary procedures & functions
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_generate_sename]
GO




--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.SendPM')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.SendPM', N'Public store. Send PM', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.ContactUs')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.ContactUs', N'Public store. Use contact us form', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.AddToCompareList')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.AddToCompareList', N'Public store. Add to compare list', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.AddToShoppingCart')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.AddToShoppingCart', N'Public store. Add to shopping cart', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.AddToWishlist')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.AddToWishlist', N'Public store. Add to wishlist', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.Login')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.Login', N'Public store. Login', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.Logout')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.Logout', N'Public store. Logout', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.AddProductReview')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.AddProductReview', N'Public store. Add product review', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.AddNewsComment')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.AddNewsComment', N'Public store. Add news comment', N'false')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.AddBlogComment')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'PublicStore.AddBlogComment', N'Public store. Add blog comment', N'false')
END
GO


--ACL for products
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [Product]
	ADD [SubjectToAcl] bit NULL
END
GO

UPDATE [Product]
SET [SubjectToAcl] = 0
WHERE [SubjectToAcl] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [SubjectToAcl] bit NOT NULL
GO


IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[AclRecord]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[AclRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityName] nvarchar(400) NOT NULL,
	[CustomerRoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_AclRecord_EntityId_EntityName' and object_id=object_id(N'[AclRecord]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_AclRecord_EntityId_EntityName] ON [AclRecord] ([EntityId] ASC, [EntityName] ASC)
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
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
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



		IF @SearchProductTags = 1
		BEGIN
			--product tag
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(pt.[Name], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, pt.[Name]) > 0 '

			--localized product tag
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name'''
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
	
	--show hidden
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
	
	--show hidden and ACL
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 FROM #FilteredSpecs [fs]
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
	DROP TABLE #FilteredCustomerRoleIds

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



--ACL for categories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Category]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [Category]
	ADD [SubjectToAcl] bit NULL
END
GO

UPDATE [Category]
SET [SubjectToAcl] = 0
WHERE [SubjectToAcl] IS NULL
GO

ALTER TABLE [Category] ALTER COLUMN [SubjectToAcl] bit NOT NULL
GO


--address form fields
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.companyenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.companyenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.streetaddressenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.streetaddressenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.streetaddressrequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.streetaddressrequired', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.streetaddress2enabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.streetaddress2enabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.zippostalcodeenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.zippostalcodeenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.zippostalcoderequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.zippostalcoderequired', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.cityenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.cityenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.cityrequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.cityrequired', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.countryenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.countryenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.stateprovinceenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.stateprovinceenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.phoneenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.phoneenabled', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.phonerequired')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.phonerequired', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.faxenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'addresssettings.faxenabled', N'true')
END
GO



--shipping by weight plugin
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--new [AdditionalFixedCost] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''AdditionalFixedCost'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [AdditionalFixedCost] decimal(18,2) NULL

		exec(''UPDATE [ShippingByWeight] SET [AdditionalFixedCost] = 0'')
		
		EXEC (''ALTER TABLE [ShippingByWeight] ALTER COLUMN [AdditionalFixedCost] decimal(18,2) NOT NULL'')
	END')

	--drop [UsePercentage] column
	EXEC ('IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''UsePercentage'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		DROP COLUMN [UsePercentage]
	END')
	
	--rename ShippingChargePercentage to PercentageRateOfSubtotal
	EXEC ('IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''ShippingChargePercentage'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [PercentageRateOfSubtotal] decimal(18,2) NULL

		exec(''UPDATE [ShippingByWeight] SET [PercentageRateOfSubtotal] = [ShippingChargePercentage]'')
		
		exec(''ALTER TABLE [ShippingByWeight] DROP COLUMN [ShippingChargePercentage]'')
	END')
	
	--rename ShippingChargeAmount to RatePerWeightUnit
	EXEC ('IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''ShippingChargeAmount'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [RatePerWeightUnit] decimal(18,2) NULL

		exec(''UPDATE [ShippingByWeight] SET [RatePerWeightUnit] = [ShippingChargeAmount]'')
		
		exec(''ALTER TABLE [ShippingByWeight] DROP COLUMN [ShippingChargeAmount]'')
	END')
	
	
	--new [LowerWeightLimit] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''LowerWeightLimit'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [LowerWeightLimit] decimal(18,2) NULL

		exec(''UPDATE [ShippingByWeight] SET [LowerWeightLimit] = 0'')
		
		EXEC (''ALTER TABLE [ShippingByWeight] ALTER COLUMN [LowerWeightLimit] decimal(18,2) NOT NULL'')
	END')
END
GO


--bundling
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.enablejsbundling')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'seosettings.enablejsbundling', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.log404errors')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'commonsettings.log404errors', N'true')
END
GO

--suffix deleted customers
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.suffixdeletedcustomers')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.suffixdeletedcustomers', N'false')
END
GO

--simplify DiscountRequirement table
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='BillingCountryId')
BEGIN
	DECLARE @entity_id int
	DECLARE cur_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [DiscountRequirement]
	WHERE [DiscountRequirementRuleSystemName] = N'DiscountRequirement.BillingCountryIs'
	OPEN cur_existing_entity
	FETCH NEXT FROM cur_existing_entity INTO @entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @settingname nvarchar(1000)	
		SET @settingname = N'DiscountRequirement.BillingCountry-' + CAST(@entity_id AS nvarchar(max))
		
		DECLARE @billingcountryid int
		SET @billingcountryid = 0
		DECLARE @sql nvarchar(1000)
		SET @sql = 'SELECT @billingcountryid = [BillingCountryId] FROM [DiscountRequirement] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@billingcountryid int OUTPUT',@billingcountryid OUTPUT
		
		
		IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = @settingname)
		BEGIN
			INSERT [Setting] ([Name], [Value])
			VALUES (@settingname, CAST(@billingcountryid AS nvarchar(max)))
		END

		--fetch next identifier
		FETCH NEXT FROM cur_existing_entity INTO @entity_id
	END
	CLOSE cur_existing_entity
	DEALLOCATE cur_existing_entity
	
	--drop BillingCountryId column
	EXEC('ALTER TABLE [DiscountRequirement] DROP COLUMN [BillingCountryId]')
END
GO



IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='ShippingCountryId')
BEGIN
	DECLARE @entity_id int
	DECLARE cur_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [DiscountRequirement]
	WHERE [DiscountRequirementRuleSystemName] = N'DiscountRequirement.ShippingCountryIs'
	OPEN cur_existing_entity
	FETCH NEXT FROM cur_existing_entity INTO @entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @settingname nvarchar(1000)	
		SET @settingname = N'DiscountRequirement.ShippingCountry-' + CAST(@entity_id AS nvarchar(max))
		
		DECLARE @shippingcountryid int
		SET @shippingcountryid = 0
		DECLARE @sql nvarchar(1000)
		SET @sql = 'SELECT @shippingcountryid = [ShippingCountryId] FROM [DiscountRequirement] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@shippingcountryid int OUTPUT',@shippingcountryid OUTPUT
		
		IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = @settingname)
		BEGIN
			INSERT [Setting] ([Name], [Value])
			VALUES (@settingname, CAST(@shippingcountryid AS nvarchar(max)))
		END

		--fetch next identifier
		FETCH NEXT FROM cur_existing_entity INTO @entity_id
	END
	CLOSE cur_existing_entity
	DEALLOCATE cur_existing_entity
	
	--drop ShippingCountryId column
	EXEC('ALTER TABLE [DiscountRequirement] DROP COLUMN [ShippingCountryId]')
END
GO


IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='RestrictedToCustomerRoleId')
BEGIN
	DECLARE @entity_id int
	DECLARE cur_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [DiscountRequirement]
	WHERE [DiscountRequirementRuleSystemName] = N'DiscountRequirement.MustBeAssignedToCustomerRole'
	OPEN cur_existing_entity
	FETCH NEXT FROM cur_existing_entity INTO @entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @settingname nvarchar(1000)	
		SET @settingname = N'DiscountRequirement.MustBeAssignedToCustomerRole-' + CAST(@entity_id AS nvarchar(max))
		
		DECLARE @RestrictedToCustomerRoleId int
		SET @RestrictedToCustomerRoleId = 0
		
		DECLARE @sql nvarchar(1000)
		SET @sql = 'SELECT @RestrictedToCustomerRoleId = [RestrictedToCustomerRoleId] FROM [DiscountRequirement] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@RestrictedToCustomerRoleId int OUTPUT',@RestrictedToCustomerRoleId OUTPUT
				
		IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = @settingname)
		BEGIN
			INSERT [Setting] ([Name], [Value])
			VALUES (@settingname, CAST(@RestrictedToCustomerRoleId AS nvarchar(max)))
		END

		--fetch next identifier
		FETCH NEXT FROM cur_existing_entity INTO @entity_id
	END
	CLOSE cur_existing_entity
	DEALLOCATE cur_existing_entity
	
	--drop RestrictedToCustomerRoleId column
	EXEC('ALTER TABLE [DiscountRequirement] DROP COLUMN [RestrictedToCustomerRoleId]')
END
GO


IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='SpentAmount')
BEGIN
	DECLARE @entity_id int
	DECLARE cur_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [DiscountRequirement]
	WHERE [DiscountRequirementRuleSystemName] = N'DiscountRequirement.HadSpentAmount'
	OPEN cur_existing_entity
	FETCH NEXT FROM cur_existing_entity INTO @entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @settingname nvarchar(1000)	
		SET @settingname = N'DiscountRequirement.HadSpentAmount-' + CAST(@entity_id AS nvarchar(max))
		
		DECLARE @SpentAmount int
		SET @SpentAmount = 0
		DECLARE @sql nvarchar(1000)
		SET @sql = 'SELECT @SpentAmount = [SpentAmount] FROM [DiscountRequirement] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@SpentAmount int OUTPUT',@SpentAmount OUTPUT
				
				PRINT(@sql)
				
		IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = @settingname)
		BEGIN
			INSERT [Setting] ([Name], [Value])
			VALUES (@settingname, CAST(@SpentAmount AS nvarchar(max)))
		END

		--fetch next identifier
		FETCH NEXT FROM cur_existing_entity INTO @entity_id
	END
	CLOSE cur_existing_entity
	DEALLOCATE cur_existing_entity
	
	--drop SpentAmount column
	EXEC('ALTER TABLE [DiscountRequirement] DROP COLUMN [SpentAmount]')
END
GO


IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[DiscountRequirement]') and NAME='RestrictedProductVariantIds')
BEGIN
	DECLARE @entity_id int
	DECLARE cur_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [DiscountRequirement]
	WHERE [DiscountRequirementRuleSystemName] = N'DiscountRequirement.HasAllProducts'
	or [DiscountRequirementRuleSystemName] = N'DiscountRequirement.HasOneProduct'
	or [DiscountRequirementRuleSystemName] = N'DiscountRequirement.PurchasedAllProducts'
	or [DiscountRequirementRuleSystemName] = N'DiscountRequirement.PurchasedOneProduct'
	OPEN cur_existing_entity
	FETCH NEXT FROM cur_existing_entity INTO @entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @settingname nvarchar(1000)	
		SET @settingname = N'DiscountRequirement.RestrictedProductVariantIds-' + CAST(@entity_id AS nvarchar(max))
		
		DECLARE @RestrictedProductVariantIds nvarchar(MAX)
		SET @RestrictedProductVariantIds = 0
		
		
		DECLARE @sql nvarchar(1000)
		SET @sql = 'SELECT @RestrictedProductVariantIds = [RestrictedProductVariantIds] FROM [DiscountRequirement] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@RestrictedProductVariantIds nvarchar(MAX) OUTPUT',@RestrictedProductVariantIds OUTPUT
		
		
		IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = @settingname)
		BEGIN
			INSERT [Setting] ([Name], [Value])
			VALUES (@settingname, @RestrictedProductVariantIds)
		END

		--fetch next identifier
		FETCH NEXT FROM cur_existing_entity INTO @entity_id
	END
	CLOSE cur_existing_entity
	DEALLOCATE cur_existing_entity
	
	--drop RestrictedProductVariantIds column
	EXEC('ALTER TABLE [DiscountRequirement] DROP COLUMN [RestrictedProductVariantIds]')
END
GO



--new 'HTML Editor. Manage files.' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'HtmlEditor.ManagePictures')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. HTML Editor. Manage pictures', N'HtmlEditor.ManagePictures', N'Configuration')

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

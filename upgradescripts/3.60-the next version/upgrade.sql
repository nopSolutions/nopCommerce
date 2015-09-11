--upgrade scripts from nopCommerce 3.60 to 3.70

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Products.MinimumQuantityNotification">
    <Value>This product has a minimum quantity of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart">
    <Value>Display tax/shipping info (shopping cart)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart.Hint">
    <Value>Check to display tax and shipping info on the shopping cart page. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.TaxShipping.ExclTax">
    <Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.TaxShipping.InclTax">
    <Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DefaultLanguage">
    <Value>Default language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DefaultLanguage.Hint">
    <Value>This property allows a store owner to specify a default language for a store. If not specified, then the default language display order will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Vendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Vendor.Hint">
    <Value>Search by a specific vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OverriddenGiftCardAmount">
    <Value>Overridden gift card amount</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.OverriddenGiftCardAmount.Hint">
    <Value>Enter gift card amount that can be used after purchase. If not specified, then product price will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateOfBirthMinimumAge">
    <Value>Customer minimum age</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.DateOfBirthMinimumAge.Hint">
    <Value>Enter minimum allowed age. Leave empty if customers of all ages are allowed.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.DateOfBirth.MinimumAge">
    <Value>You have to be {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation">
    <Value>Hide delivery information</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation.Hint">
    <Value>Check to hide delivery information as description of returned shipping methods.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Rental.StartDateShouldBeFuture">
    <Value>Rental start date should be the future date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.Picture.Hint">
    <Value>The vendor picture.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.VendorThumbPictureSize">
    <Value>Vendor thumbnail image size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.VendorThumbPictureSize.Hint">
    <Value>The default size (pixels) for vendor thumbnail images.</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Vendor.ImageAlternateTextFormat">
    <Value>Picture for vendor {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Vendor.ImageLinkTitleFormat">
    <Value>Show products of vendor {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductType.GroupedProduct">
    <Value>Grouped (product with variants)</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductType.SimpleProduct">
    <Value>Simple</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.AssociatedProducts">
    <Value>Associated products (variants)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToApplyForVendorAccount">
    <Value>Allow customers to apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToApplyForVendorAccount.Hint">
    <Value>Check to allow customers users to fill a form to become a new vendor. Then a store owner will have to manually approve it.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount">
    <Value>Apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Email.Hint">
    <Value>Enter your email address</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Email.Required">
    <Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Name">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Name.Hint">
    <Value>Enter vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Name.Required">
    <Value>Vendor name is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Submitted">
    <Value>Your request has been submitted successfully. We''ll contact you soon.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.Vendors.Apply">
    <Value>Apply for vendor account</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.Button">
    <Value>Submit</Value>
  </LocaleResource>
  <LocaleResource Name="Vendors.ApplyAccount.AlreadyApplied">
    <Value>You already applied for a vendor account. Please register as a new customer in order to apply for one more vendor account.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsAccumulatedForAllStores">
    <Value>Points accumulated for all stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PointsAccumulatedForAllStores.Hint">
    <Value>Check to accumulate all reward points in one balance for all stores so they can be used in any store. Otherwise, each store has its own rewards points and they can only be used in that store. WARNING: not recommended to change in production environment with several stores already created.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore.Hint">
    <Value>Choose a store. It''s useful only when you have "Points accumulated for all stores" setting disabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.AllowViewUnpublishedProductPage">
    <Value>Allow viewing of unpublished product details page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.AllowViewUnpublishedProductPage.Hint">
    <Value>Check to allow viewing of unpublished product details page. This way SEO won''t be affected by search crawlers when a product is temporary unpublished. Please note that a store owner always has access to unpublished products.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.CannotBeUsedAnymore">
    <Value>Sorry, you''ve used this discount already</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.CannotBeUsedWithGiftCards">
    <Value>Sorry, this discount cannot be used with gift cards in the cart</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.Expired">
    <Value>Sorry, this offer is expired</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.NotStartedYet">
    <Value>Sorry, this offer is not started yet</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HadSpentAmount.NotEnough">
    <Value>Sorry, this offer requires more money spent (previously placed orders)</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.AddToCart.Error">
    <Value>Some product(s) from wishlist could not be moved to the cart for some reasons.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Acl">
    <Value>Access control list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SubjectToAcl">
    <Value>Subject to ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SubjectToAcl.Hint">
    <Value>Determines whether the topic is subject to ACL (access control list).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AclCustomerRoles">
    <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the topic will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions">
    <Value>Back in stock subscriptions</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.BackInStockSubscriptions.CreatedOn">
    <Value>Subscribed on</Value>
  </LocaleResource>
  <LocaleResource Name="Search.Button">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Hint">
    <Value>Enable to combine (bundle) multiple CSS files into a single file. Do not enable if you''re running nopCommerce in IIS virtual directory. Note that this functionality requires significant server resources (not recommended to use with cheap shared hosting plans).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableJsBundling.Hint">
    <Value>Enable to combine (bundle) multiple JavaScript files into a single file. Note that this functionality requires significant server resources (not recommended to use with cheap shared hosting plans).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition">
    <Value>Condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.ViewLink">
    <Value>View/Edit condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Description">
    <Value>Conditional attributes appear if a previous attribute is selected, such as having an option for personalizing clothing with a name and only providing the text input box if the "Personalize" radio button is checked</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition">
    <Value>Enable condition</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition.Hint">
    <Value>Check to specify a condition (depending on other attribute) when this attribute should be enabled (visible).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Attributes">
    <Value>Attribute</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Attributes.Hint">
    <Value>Choose an attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DynamicPriceUpdateAjax">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DynamicPriceUpdateAjax.Hint">
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



IF NOT EXISTS (
    SELECT 1
    FROM [MessageTemplate]
    WHERE [Name] = N'OrderRefunded.CustomerNotification')
BEGIN
  INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
  VALUES (N'OrderRefunded.CustomerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% refunded', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href="%Store.URL%">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.<br /><br />Amount refunded: %Order.AmountRefunded%<br /><br />Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>', 0, 0, 0, 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfoshoppingcart')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfoshoppingcart', N'false', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='DefaultLanguageId')
BEGIN
	ALTER TABLE [Store]
	ADD [DefaultLanguageId] int NULL
END
GO

UPDATE [Store]
SET [DefaultLanguageId] = 0
WHERE [DefaultLanguageId] IS NULL
GO

ALTER TABLE [Store] ALTER COLUMN [DefaultLanguageId] int NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='OverriddenGiftCardAmount')
BEGIN
	ALTER TABLE [Product]
	ADD [OverriddenGiftCardAmount] decimal NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.dateofbirthminimumage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.dateofbirthminimumage', N'', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Vendor]') and NAME='PictureId')
BEGIN
	ALTER TABLE [Vendor]
	ADD [PictureId] int NULL
END
GO

UPDATE [Vendor]
SET [PictureId] = 0
WHERE [PictureId] IS NULL
GO

ALTER TABLE [Vendor] ALTER COLUMN [PictureId] int NOT NULL
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.vendorthumbpicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'mediasettings.vendorthumbpicturesize', N'450', 0)
END
GO

--rename some product templates
UPDATE [ProductTemplate]
SET [Name] = 'Grouped product (with variants)'
WHERE [ViewPath] = N'ProductTemplate.Grouped'
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowcustomerstoapplyforvendoraccount')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.allowcustomerstoapplyforvendoraccount', N'false', 0)
END
GO

--new topic
IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Topic]
  WHERE [SystemName] = N'ApplyVendor')
BEGIN
	INSERT [dbo].[Topic] ([SystemName], [TopicTemplateId], [IncludeInSitemap], [AccessibleWhenStoreClosed], [LimitedToStores], [IncludeInFooterColumn1], [IncludeInFooterColumn2], [IncludeInFooterColumn3], [IncludeInTopMenu], [IsPasswordProtected], [DisplayOrder] , [Title], [Body])
	VALUES (N'ApplyVendor', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, N'', N'<p>Put your apply vendor instructions here. You can edit this in the admin site.</p>')
END
GO


--'New vendor account submitted' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'VendorAccountApply.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
	VALUES (N'VendorAccountApply.StoreOwnerNotification', null, N'%Store.Name%. New vendor account submitted.', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just submitted for a vendor account. Details are below:<br />Vendor name: %Vendor.Name%<br />Vendor email: %Vendor.Email%<br /><br />You can activate it in admin area.</p>', 1, 0, 0, 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RewardPointsHistory]') and NAME='StoreId')
BEGIN
	ALTER TABLE [RewardPointsHistory]
	ADD [StoreId] int NULL
END
GO

--just use the first store
--we cannot find original store IDs of some orders
--furthermore, it won't work for points granted for registration (if enabled)
UPDATE [RewardPointsHistory]
SET [StoreId] = (SELECT TOP 1 [Id] from [Store])
WHERE [StoreId] IS NULL
GO


ALTER TABLE [RewardPointsHistory] ALTER COLUMN [StoreId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.pointsaccumulatedforallstores')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.pointsaccumulatedforallstores', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.allowviewunpublishedproductpage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.allowviewunpublishedproductpage', N'true', 0)
END
GO


--'Order refunded' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderRefunded.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores], [AttachedDownloadId])
	VALUES (N'OrderRefunded.StoreOwnerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% refunded', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Order #%Order.OrderNumber% has been just refunded<br /><br />Amount refunded: %Order.AmountRefunded%<br /><br />Date Ordered: %Order.CreatedOn%</p>', 0, 0, 0, 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [Topic]
	ADD [SubjectToAcl] bit NULL
END
GO

UPDATE [Topic]
SET [SubjectToAcl] = 0
WHERE [SubjectToAcl] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [SubjectToAcl] bit NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ScheduleTask]') and NAME='LeasedByMachineName')
BEGIN
	ALTER TABLE [ScheduleTask]
	ADD [LeasedByMachineName] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ScheduleTask]') and NAME='LeasedUntilUtc')
BEGIN
	ALTER TABLE [ScheduleTask]
	ADD [LeasedUntilUtc] datetime NULL
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_ProductAttribute_Mapping]') and NAME='ConditionAttributeXml')
BEGIN
	ALTER TABLE [Product_ProductAttribute_Mapping]
	ADD [ConditionAttributeXml] nvarchar(MAX) NULL
END
GO

--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'catalogsettings.dynamicpriceupdateajax'
GO


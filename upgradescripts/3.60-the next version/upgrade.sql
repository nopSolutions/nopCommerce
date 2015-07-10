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

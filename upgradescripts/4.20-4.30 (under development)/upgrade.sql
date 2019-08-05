--upgrade scripts from nopCommerce 4.20 to 4.30

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification">
    <Value>HTML minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Hint">
    <Value>
      Specification attributes are product features i.e, screen size, number of USB-ports, visible on product details page. Specification attributes can be used for filtering products on the category details page. Unlike product attributes, specification attributes are used for information purposes only.
      You can add attribute for your product using existing list of attributes, or if you need to create a new one go to Catalog &gt; Attributes &gt; Specification attributes.
    </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxCategory.Hint">
    <Value>Select default tax category for products. It''ll be pre-selected on the "Add new product" page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.TaxCategory.Hint">
    <Value>The tax classification for this attribute (used to calculate tax). You can manage tax categories by selecting Configuration &gt; Tax &gt; Tax Categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute.Hint">
    <Value>Choose a product specification attribute. You can manage specification attributes from Catalog &gt; Attributes &gt; Product Specifications.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderTotalDiscount.Hint">
    <Value>The total discount applied to this order. Manage your discounts from Promotions &gt; Discounts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PaymentMethod.Hint">
    <Value>The payment method used for this transaction. You can manage Payment Methods from Configuration &gt; Payment &gt; Payment Methods.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippingMethod.Hint">
    <Value>The customers chosen shipping method for this order. You can manage shipping methods from Configuration &gt; Shipping &gt; Shipping Methods.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Tax.Hint">
    <Value>Total tax applied to this order. Manage your tax settings from Configuration &gt; Tax.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue">
    <Value>Error while save attribute combinations. Attribute value not specified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum">
    <Value>Show on forum</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum.Hint">
    <Value>Check to show CAPTCHA on forum, when editing and creating a topic or post.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanDeliver">
    <Value>Delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanDeliver.Hint">
    <Value>Check to apply current date to delivery.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanShip">
    <Value>Shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanShip.Hint">
    <Value>Check to apply current date to shipment.</Value>
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
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.showonforum')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.showonforum', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.recaptcharequesttimeout')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.recaptcharequesttimeout', 20, 0)
END
GO
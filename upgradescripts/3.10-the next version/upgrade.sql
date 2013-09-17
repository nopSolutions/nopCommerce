--upgrade scripts from nopCommerce 3.10 to 3.20

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Orders.Shipments.List.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.Country.Hint">
    <Value>Search by a specific country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StateProvince.Hint">
    <Value>Search by a specific state.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.City">
    <Value>City</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.City.Hint">
    <Value>Search by a specific city.</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Product.ImageLinkTitleFormat.Details">
    <Value>Picture of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Product.ImageAlternateTextFormat.Details">
    <Value>Picture of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Category">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Category.Hint">
    <Value>Search in a specific category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Manufacturer">
    <Value>Manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Manufacturer.Hint">
    <Value>Search in a specific manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Cost">
    <Value>Cost</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Cost.Hint">
    <Value>The attribute value cost is the cost of all the different components which make up this value. This may either be the purchase price if the components are bought from outside suppliers, or the combined cost of materials and manufacturing processes if the component is made in-house.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderStatus.Change">
    <Value>Change status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderStatus.Change.ForAdvancedUsers">
    <Value>This option is only for advanced users (not recommended to change manually). All appropriate actions (such as invetory adjustment, sending notification emails, etc) should be also done manually in this case.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.PreOrderAvailabilityStartDateTimeUtc">
    <Value>Pre-order availability start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.PreOrderAvailabilityStartDateTimeUtc.Hint">
    <Value>The availability start date of the product configured for pre-order in Coordinated Universal Time (UTC). ''Pre-order'' button will automatically be changed to ''Add to cart'' at the monent.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.PurchasedWithOrders">
    <Value>Purchased with orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.PurchasedWithOrders.Hint">
    <Value>Here you can see a list of orders in which this product was purchased.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Orders.Order">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.OverriddenPrice">
    <Value>Overridden price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.OverriddenPrice.Hint">
    <Value>Override price for this attribute combination. This way a store owner can override the default product price when this attribute combination is added to the cart. For example, you can give a discount this way. Leave empty to ignore field. All other applied discounts will be ignored when this field is specified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing">
    <Value>Allow cart item editing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing.Hint">
    <Value>Check to allow customers to edit items already placed in the cart. It could be useful when your products have attributes or any other fields entered by a customer.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableJsBundling">
    <Value>JavaScript bundling and minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableJsBundling.Hint">
    <Value>Enable to combine (bundle) multiple JavaScript files into a single file. Don''t do it if you''re running nopCommerce in web farms or Windows Azure.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableCssBundling">
    <Value>CSS bundling and minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Hint">
    <Value>Enable to combine (bundle) multiple CSS files into a single file. Don''t do it if you''re running nopCommerce in web farms or Windows Azure. It also doesn''t work in virtual IIS directories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates">
    <Value>Delivery dates</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.AddNew">
    <Value>Add a new delivery date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.BackToList">
    <Value>back to delivery date list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.EditDeliveryDateDetails">
    <Value>Edit delivery date details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Added">
    <Value>The new delivery date has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Deleted">
    <Value>The delivery date has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Updated">
    <Value>The delivery date has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Fields.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Hint">
    <Value>Enter delivery date name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Required">
    <Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.DeliveryDates.Fields.DisplayOrder.Hint">
    <Value>The display order of this delivery date. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DeliveryDate">
    <Value>Delivery date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DeliveryDate.Hint">
    <Value>Choose a delivery date which will be displayed in the public store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.DeliveryDate.None">
    <Value>None</Value>
  </LocaleResource>
  <LocaleResource Name="Products.DeliveryDate">
    <Value>Delivery date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Report.Shipping">
    <Value>Shipping</Value>
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


--Add a reference for [StoreMapping] table
--but first, delete abandoned records
DELETE FROM [StoreMapping]
WHERE [StoreId] NOT IN (SELECT [Id] FROM [Store])
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'StoreMapping_Store'
           AND parent_object_id = Object_id('StoreMapping')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[StoreMapping]  WITH CHECK ADD  CONSTRAINT [StoreMapping_Store] FOREIGN KEY([StoreId])
	REFERENCES [dbo].[Store] ([Id])
	ON DELETE CASCADE
END
GO


--Add a reference for [AclRecord] table
--but first, delete abandoned records
DELETE FROM [AclRecord]
WHERE [CustomerRoleId] NOT IN (SELECT [Id] FROM [CustomerRole])
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'AclRecord_CustomerRole'
           AND parent_object_id = Object_id('AclRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[AclRecord]  WITH CHECK ADD  CONSTRAINT [AclRecord_CustomerRole] FOREIGN KEY([CustomerRoleId])
	REFERENCES [dbo].[CustomerRole] ([Id])
	ON DELETE CASCADE
END
GO

DELETE FROM [dbo].[PermissionRecord]
WHERE [SystemName] = N'ManageCustomerRoles'
GO


--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeValue]') and NAME='Cost')
BEGIN
	ALTER TABLE [ProductVariantAttributeValue]
	ADD [Cost] [decimal](18, 4) NULL
END
GO

UPDATE [ProductVariantAttributeValue]
SET [Cost] = 0
WHERE [Cost] IS NULL
GO

ALTER TABLE [ProductVariantAttributeValue] ALTER COLUMN [Cost] [decimal](18, 4) NOT NULL
GO


--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='PreOrderAvailabilityStartDateTimeUtc')
BEGIN
	ALTER TABLE [Product]
	ADD [PreOrderAvailabilityStartDateTimeUtc] datetime NULL
END
GO


--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeCombination]') and NAME='OverriddenPrice')
BEGIN
	ALTER TABLE [ProductVariantAttributeCombination]
	ADD [OverriddenPrice] decimal(18,4) NULL
END
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.allowcartitemediting')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shoppingcartsettings.allowcartitemediting', N'true', 0)
END
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.enablecssbundling')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'seosettings.enablecssbundling', N'false', 0)
END
GO


--delivery dates
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[DeliveryDate]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[DeliveryDate](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] nvarchar(400) NOT NULL,
		[DisplayOrder] int NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	)

	--create several sample options
	INSERT INTO [DeliveryDate] ([Name], [DisplayOrder])
	VALUES (N'1-2 days', 1)
	
	INSERT INTO [DeliveryDate] ([Name], [DisplayOrder])
	VALUES (N'3-5 days', 5)
	
	INSERT INTO [DeliveryDate] ([Name], [DisplayOrder])
	VALUES (N'1 week', 10)
END
GO


--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='DeliveryDateId')
BEGIN
	ALTER TABLE [Product]
	ADD [DeliveryDateId] int NULL
END
GO

UPDATE [Product]
SET [DeliveryDateId] = 0
WHERE [DeliveryDateId] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [DeliveryDateId] int NOT NULL
GO

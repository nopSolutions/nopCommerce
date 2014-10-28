--upgrade scripts from nopCommerce 3.40 to 3.50

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.Store">
    <Value>Limited to store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.Store.Hint">
    <Value>Choose a store which subscribers will get this email.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Plugins.Saved">
    <Value>The plugin has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.List.SearchName">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.List.SearchName.Hint">
    <Value>A vendor name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Incomplete.View">
    <Value>view all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInTopMenu">
    <Value>Include in top menu</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.IncludeInTopMenu.Hint">
    <Value>Check to include this topic in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots">
    <Value>Dots are not supported by tags.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ExportToCsv">
    <Value>Export states to CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ImportFromCsv">
    <Value>Import states from CSV</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.ImportSuccess">
    <Value>{0} states have been successfully imported</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.DisplayHowMuchWillBeEarned">
    <Value>Display how much will be earned</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.DisplayHowMuchWillBeEarned.Hint">
    <Value>Check to display how much point will be earned before checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.RewardPoints.WillEarn">
    <Value>You will earn</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Totals.RewardPoints.WillEarn.Point">
    <Value>{0} points</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.MultipleThumbDirectories">
    <Value>Multiple thumb directories</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.MultipleThumbDirectories.Hint">
    <Value>Check to enable multiple thumb directories. It can be helpful if your hosting company has some limitations to the number of allowed files per directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Affiliates.Fields.AdminComment.Hint">
    <Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.CacheProductPrices.Hint">
    <Value>Check to cache product prices. It can significantly improve performance. But you not should enable it if you use some complex discounts, discount requirement rules, or coupon codes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.MaximumDiscountedQuantity">
    <Value>Maximum discounted quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.MaximumDiscountedQuantity.Hint">
    <Value>Maximum product quantity which could be discounted. For example, you can have two products (the same) in the cart but only one of them will be discounted. It can be used for scenarios like "buy 2 get 1 free". Leave empty if any quantity could be discounted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CustomValues">
    <Value>Custom values</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.CustomValues.Hint">
    <Value>Custom values from the payment method.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShipSeparately">
    <Value>Ship separately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ShipSeparately.Hint">
    <Value>Check to mark a product as being able to be shipped by itself in a single box (separate shipment). This way shipping rates are calculated separately for this product regardless of what other products are also in the cart. Please note that if you have several quantities of this product in the cart, then all of them will be packed and shipped in a single box.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ShipSeparately">
    <Value>this product should be shipped separately!</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.ShipSeparately.Warning">
    <Value>Warning: </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountryRequired">
    <Value>''Country'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountryRequired.Hint">
    <Value>Check if ''Country'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StateProvinceRequired">
    <Value>''State/province'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.StateProvinceRequired.Hint">
    <Value>Check if ''State/province'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Address.SelectState">
    <Value>Select state</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Country.Required">
    <Value>Country is required</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.StateProvince.Required">
    <Value>State / province is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.SelectState">
    <Value>Select state</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UseMultipleWarehouses">
    <Value>Use multiple warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UseMultipleWarehouses.Hint">
    <Value>Check if you want to support shipping and inventory management from multiple warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory">
    <Value>Warehouses</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Hint">
    <Value>Manage inventory per warehouse.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.Warehouse.NotDefined">
    <Value>No warehouses defined</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.StockQuantity">
    <Value>Stock quantity</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.WarehouseUsed">
    <Value>Use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse.NotAvailabe">
    <Value>No warehouses available. You cannot ship this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse.ChooseQty">
    <Value>{0} ({1} qty in stock, {2} qty reserved)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description1">
    <Value>"Stock quantity" is total quantity. It''s reduced when a shipment is shipped.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description2">
    <Value>"Reserved qty" is product quantity that is ordered but not shipped yet.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Description3">
    <Value>"Planned qty" is product quantity that is ordered and already added to a shipment but not shipped yet.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Warehouses.Fields.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Warehouses.Fields.AdminComment.Hint">
    <Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.ReservedQuantity">
    <Value>Reserved qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductWarehouseInventory.Fields.PlannedQuantity">
    <Value>Planned qty</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequiredProductIds.AddNew">
    <Value>Add required product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.RequiredProductIds.Choose">
    <Value>Choose</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products.AddNew">
    <Value>Add product</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products.Choose">
    <Value>Choose</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products.AddNew">
    <Value>Add product</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products.Choose">
    <Value>Choose</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.AttributeType">
    <Value>Attribute type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.AttributeType.Hint">
    <Value>Choose attribute type</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.Value">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue.Hint">
    <Value>Custom value (text, hyperlink, etc).</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.Option">
    <Value>Option</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.CustomText">
    <Value>Custom text</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.CustomHtmlText">
    <Value>Custom HTML text</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.SpecificationAttributeType.Hyperlink">
    <Value>Hyperlink</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist">
    <Value>Display tax/shipping info (wishlist)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist.Hint">
    <Value>Check to display tax and shipping info on the wishlist page. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage">
    <Value>Display tax/shipping info (order details page)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage.Hint">
    <Value>Check to display tax and shipping info on the order details page. This option is used in Germany.</Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.TaxShipping.ExclTax">
    <Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.TaxShipping.InclTax">
    <Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Order.TaxShipping.ExclTax">
    <Value><![CDATA[All prices are entered excluding tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Order.TaxShipping.InclTax">
    <Value><![CDATA[All prices are entered including tax. Excluding <a href="{0}">shipping</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyName">
    <Value>Company name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyName.Hint">
    <Value>Enter your company name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyAddress">
    <Value>Company address</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyAddress.Hint">
    <Value>Enter your company address.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyPhoneNumber">
    <Value>Company phone number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyPhoneNumber.Hint">
    <Value>Enter your company phone number.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyVat">
    <Value>Company VAT</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.CompanyVat.Hint">
    <Value>Enter your company VAT (the European Union Value Added Tax)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Product.Hint">
    <Value>Search by a specific product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.BestByNumberOfOrders">
    <Value>Customers by number of orders</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Reports.BestBy.BestByOrderTotal">
    <Value>Customers by order total</Value>
  </LocaleResource>
  <LocaleResource Name="Order.PaymentMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.Payment">
    <Value>Payment</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Payment.Method">
    <Value>Payment Method</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Payment.Status">
    <Value>Payment Status</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShippingMethod">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipping">
    <Value>Shipping</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipping.Name">
    <Value>Shipping Method</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Shipping.Status">
    <Value>Shipping Status</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.PickUpInStoreFee">
    <Value>"Pick Up in Store" fee</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.PickUpInStoreFee.Hint">
    <Value>Specify "Pick Up in Store" fee.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStoreAndFee">
    <Value>In-Store Pickup ({0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow">
    <Value>Notify admin for quantity below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductVariantAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow.Hint">
    <Value>When the current stock quantity falls below (reaches) this quantity, a store owner will receive a notification.</Value>
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
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Campaign]') and NAME='StoreId')
BEGIN
	ALTER TABLE [Campaign]
	ADD [StoreId] int NULL
END
GO

     
UPDATE [Campaign]
SET [StoreId] = 0
WHERE [StoreId] IS NULL
GO

ALTER TABLE [Campaign] ALTER COLUMN [StoreId] int NOT NULL
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='IncludeInTopMenu')
BEGIN
	ALTER TABLE [Topic]
	ADD [IncludeInTopMenu] bit NULL
END
GO

UPDATE [Topic]
SET [IncludeInTopMenu] = 0
WHERE [IncludeInTopMenu] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [IncludeInTopMenu] bit NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.ignorelogwordlist')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'commonsettings.ignorelogwordlist', N'', 0)
END
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.displayhowmuchwillbeearned')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.displayhowmuchwillbeearned', N'true', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Affiliate]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Affiliate]
	ADD [AdminComment] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='MaximumDiscountedQuantity')
BEGIN
	ALTER TABLE [Discount]
	ADD [MaximumDiscountedQuantity] int NULL
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='ShipSeparately')
BEGIN
	ALTER TABLE [Product]
	ADD [ShipSeparately] bit NULL
END
GO

UPDATE [Product]
SET [ShipSeparately] = 0
WHERE [ShipSeparately] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [ShipSeparately] bit NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.countryrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.countryrequired', N'false', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.stateprovincerequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.stateprovincerequired', N'false', 0)
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='UseMultipleWarehouses')
BEGIN
	ALTER TABLE [Product]
	ADD [UseMultipleWarehouses] bit NULL
END
GO

UPDATE [Product]
SET [UseMultipleWarehouses] = 0
WHERE [UseMultipleWarehouses] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [UseMultipleWarehouses] bit NOT NULL
GO


--new table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductWarehouseInventory]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ProductWarehouseInventory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[WarehouseId] [int] NOT NULL,
	[StockQuantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO


IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductWarehouseInventory_Product'
           AND parent_object_id = Object_id('ProductWarehouseInventory')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.ProductWarehouseInventory
DROP CONSTRAINT ProductWarehouseInventory_Product
GO
ALTER TABLE [dbo].[ProductWarehouseInventory]  WITH CHECK ADD  CONSTRAINT [ProductWarehouseInventory_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO


IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ProductWarehouseInventory_Warehouse'
           AND parent_object_id = Object_id('ProductWarehouseInventory')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.ProductWarehouseInventory
DROP CONSTRAINT ProductWarehouseInventory_Warehouse
GO
ALTER TABLE [dbo].[ProductWarehouseInventory]  WITH CHECK ADD  CONSTRAINT [ProductWarehouseInventory_Warehouse] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouse] ([Id])
ON DELETE CASCADE
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ShipmentItem]') and NAME='WarehouseId')
BEGIN
	ALTER TABLE [ShipmentItem]
	ADD [WarehouseId] int NULL
END
GO

UPDATE [ShipmentItem]
SET [WarehouseId] = 0
WHERE [WarehouseId] IS NULL
GO

ALTER TABLE [ShipmentItem] ALTER COLUMN [WarehouseId] int NOT NULL
GO

UPDATE [ShipmentItem]
SET [WarehouseId] = (SELECT p.[WarehouseId] FROM [Product] p
INNER JOIN [OrderItem] oi ON p.[Id] = oi.[ProductId]
WHERE [oi].[Id] = [ShipmentItem].[OrderItemId])
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
	@ParentGroupedProductId	int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
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
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
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

		--SKU
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[Sku], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[Sku]) > 0 '
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
	
	--filter by parent grouped product identifer
	IF @ParentGroupedProductId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.ParentGroupedProductId = ' + CAST(@ParentGroupedProductId AS nvarchar(max))
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END
	
	--filter by parent product identifer
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
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
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 FROM #FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM Product_SpecificationAttribute_Mapping psam with (NOLOCK)
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
		
		--parent grouped product specified (sort associated products)
		IF @ParentGroupedProductId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' p.[DisplayOrder] ASC'
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


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Warehouse]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Warehouse]
	ADD [AdminComment] nvarchar(MAX) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductWarehouseInventory]') and NAME='ReservedQuantity')
BEGIN
	ALTER TABLE [ProductWarehouseInventory]
	ADD [ReservedQuantity] int NULL
END
GO

UPDATE [ProductWarehouseInventory]
SET [ReservedQuantity] = 0
WHERE [ReservedQuantity] IS NULL
GO

ALTER TABLE [ProductWarehouseInventory] ALTER COLUMN [ReservedQuantity] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.loadallsidecategorymenusubcategories')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.loadallsidecategorymenusubcategories', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_SpecificationAttribute_Mapping]') and NAME='AttributeTypeId')
BEGIN
	ALTER TABLE [Product_SpecificationAttribute_Mapping]
	ADD [AttributeTypeId] int NULL
END
GO

--"custom text" attribute type (if "CustomValue" column is specified)
UPDATE [Product_SpecificationAttribute_Mapping]
SET [AttributeTypeId] = 10
WHERE [AttributeTypeId] IS NULL AND [CustomValue] is not null
GO

UPDATE [Product_SpecificationAttribute_Mapping]
SET [AttributeTypeId] = 0
WHERE [AttributeTypeId] IS NULL
GO

ALTER TABLE [Product_SpecificationAttribute_Mapping] ALTER COLUMN [AttributeTypeId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfowishlist')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfowishlist', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytaxshippinginfoorderdetailspage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.displaytaxshippinginfoorderdetailspage', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyName')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyName] nvarchar(1000) NULL
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyAddress')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyAddress] nvarchar(1000) NULL
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyPhoneNumber')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyPhoneNumber] nvarchar(1000) NULL
END
GO
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') and NAME='CompanyVat')
BEGIN
	ALTER TABLE [Store]
	ADD [CompanyVat] nvarchar(1000) NULL
END
GO


--'Order paid' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderPaid.CustomerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'OrderPaid.CustomerNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% paid', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href="%Store.URL%">%Store.Name%</a>. Order #%Order.OrderNumber% has been just paid. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a href="%Order.OrderURLForCustomer%" target="_blank">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>', 0, 0, 0)
END
GO

--delete a setting
DELETE FROM [Setting]
WHERE [name] = N'commonsettings.sitemapincludetopics'
GO


--'Order paid' message template
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'OrderPaid.VendorNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'OrderPaid.VendorNotification', null, N'%Store.Name%. Order #%Order.OrderNumber% paid', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Order #%Order.OrderNumber% has been just paid. <br /><br />Order Number: %Order.OrderNumber%<br />Date Ordered: %Order.CreatedOn%<br /><br />%Order.Product(s)%</p>', 0, 0, 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.richeditoradditionalsettings')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'adminareasettings.richeditoradditionalsettings', N'', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.pickupinstorefee')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.pickupinstorefee', N'0', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeCombination]') and NAME='NotifyAdminForQuantityBelow')
BEGIN
	ALTER TABLE [ProductVariantAttributeCombination]
	ADD [NotifyAdminForQuantityBelow] int NULL
END
GO

UPDATE [ProductVariantAttributeCombination]
SET [NotifyAdminForQuantityBelow] = 1
WHERE [NotifyAdminForQuantityBelow] IS NULL
GO

ALTER TABLE [ProductVariantAttributeCombination] ALTER COLUMN [NotifyAdminForQuantityBelow] int NOT NULL
GO

--'Quantity below' message template for attribute combinations
IF NOT EXISTS (
		SELECT 1
		FROM [MessageTemplate]
		WHERE [Name] = N'QuantityBelow.AttributeCombination.StoreOwnerNotification')
BEGIN
	INSERT [MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId], [LimitedToStores])
	VALUES (N'QuantityBelow.AttributeCombination.StoreOwnerNotification', null, N'%Store.Name%. Quantity below notification. %Product.Name%', N'<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Product.Name% (ID: %Product.ID%) low quantity. <br />%AttributeCombination.Formatted%<br />Quantity: %AttributeCombination.StockQuantity%<br /></p>', 1, 0, 0)
END
GO
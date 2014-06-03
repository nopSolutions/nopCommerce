--upgrade scripts from nopCommerce 3.30 to 3.40

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore">
    <Value>"Pick Up in Store" enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore.Hint">
    <Value>A value indicating whether "Pick Up in Store" option is enabled during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore">
    <Value>In-Store Pickup</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore.Description">
    <Value>Pick up your items at the store (put your store address here)</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore.MethodName">
    <Value>In-Store Pickup</Value>
  </LocaleResource>
  <LocaleResource Name="BackInStockSubscriptions.OnlyRegistered">
    <Value>Only registered customers can use this feature</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AdminComment.Hint">
    <Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AdminComment.Button">
    <Value>Set admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForMobileDevices">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForMobileDevices.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.MobileDevicesSupported">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.MobileDevicesSupported.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Mobile.ViewFullSite">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Mobile.ViewMobileVersion">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.HeaderQuantity.Mobile">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.HeaderQuantity.Mobile">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops.GetMore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme">
    <Value>Default store theme</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.GetMore">
    <Value>You can get more themes on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.Hint">
    <Value>The public store theme. You can download themes from the extensions page at www.nopcommerce.com.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault">
    <Value>Newsletter ticked by default</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault.Hint">
    <Value>A value indicating whether ''Newsletter'' checkbox is ticked by default on the registration page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DynamicPriceUpdateAjax">
    <Value>Use AJAX to dynamically update prices</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DynamicPriceUpdateAjax.Hint">
    <Value>Check if you want to dynamically update prices using AJAX. This settings calculates prices more carefully (consider attribute combinations, discounts). It also updates SKU, MPN, GTIN values overridden in attribute combinations. But this method can slightly affect performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.AttributeControlType.ReadonlyCheckboxes">
    <Value>Read-only checkboxes</Value>
  </LocaleResource>
  <LocaleResource Name="Sitemap.Topics">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.ReplyTo">
    <Value>ReplyTo</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.ReplyTo.Hint">
    <Value>ReplyTo address (optional).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.ReplyToName">
    <Value>ReplyTo name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.ReplyToName.Hint">
    <Value>ReplyTo name (optional).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.Products.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.List.Warehouse.Hint">
    <Value>Load orders with products from a specified warehouse.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.Warehouse.Hint">
    <Value>Load shipments with products from a specified warehouse.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Hint">
    <Value>Enable to combine (bundle) multiple CSS files into a single file. Don''t enable if you''re running nopCommerce in web farms or Windows Azure. It also doesn''t work in virtual IIS directories. Note that this functionality requires significant server resources (not recommended to use with cheap shared hosting plans).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableJsBundling.Hint">
    <Value>Enable to combine (bundle) multiple JavaScript files into a single file. Don''t enable if you''re running nopCommerce in web farms or Windows Azure. Note that this functionality requires significant server resources (not recommended to use with cheap shared hosting plans).</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.YourAccountWillBeLinkedTo.Remove">
    <Value>(remove)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AddressOverride">
    <Value>Address override</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Fields.AddressOverride.Hint">
    <Value>For people who already have PayPal accounts and whom you already prompted for a shipping address before they choose to pay with PayPal, you can use the entered address instead of the address the person has stored with PayPal.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GenerateProductMetaDescription">
    <Value>Generate product META description</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GenerateProductMetaDescription.Hint">
    <Value>When enabled, product META descriptions will be automatically generated (if not specified on the product details page) based on product short description.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.DefaultCurrency">
    <Value>Default currency</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.DefaultCurrency.Hint">
    <Value>This property allows a store owner to specify a default currency for a language. If not specified, then the default currency display order will be used.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.List.RecipientName">
    <Value>Recipient name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.List.RecipientName.Hint">
    <Value>Search by recipient name. Leave empty to load all records.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags">
    <Value>Twitter META tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags.Hint">
    <Value>Check to generate Twitter META tags on the product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags">
    <Value>Open Graph META tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags.Hint">
    <Value>Check to generate Open Graph META tags on the product details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Menu">
    <Value>Menu</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers">
    <Value>Display shipment events (customers)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers.Hint">
    <Value>Check if you want your customers to see shipment events on their shipment details pages (if supported by your shipping rate computation method).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToStoreOwner">
    <Value>Display shipment events (store owner)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToStoreOwner.Hint">
    <Value>Check if you want a store owner to see shipment events on the shipment details pages of admin area (if supported by your shipping rate computation method).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShipmentStatusEvents">
    <Value>Shipment status events</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShipmentStatusEvents.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShipmentStatusEvents.Date">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShipmentStatusEvents.Event">
    <Value>Event</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.ShipmentStatusEvents.Location">
    <Value>Location</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.TrackingNumber.ViewOnline">
    <Value>View tracking info</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.AddMoreRecords">
    <Value>You can associate your account with some external authentication systems on the following page (login once using them):</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.Remove">
    <Value>Remove</Value>
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
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.allowpickupinstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.allowpickupinstore', N'true', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='PickUpInStore')
BEGIN
	ALTER TABLE [Order]
	ADD [PickUpInStore] bit NULL
END
GO

UPDATE [Order]
SET [PickUpInStore] = 0
WHERE [PickUpInStore] IS NULL
GO

ALTER TABLE [Order] ALTER COLUMN [PickUpInStore] bit NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Shipment]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Shipment]
	ADD [AdminComment] nvarchar(MAX) NULL
END
GO

--delete some settings
DELETE FROM [Setting]
WHERE [name] = N'storeinformationsettings.emulatemobiledevice'
GO

DELETE FROM [Setting]
WHERE [name] = N'storeinformationsettings.mobiledevicessupported'
GO

DELETE FROM [Setting]
WHERE [name] = N'storeinformationsettings.defaultstorethemeformobiledevices'
GO

UPDATE [GenericAttribute]
SET [key] = N'WorkingThemeName'
WHERE [key] = N'WorkingDesktopThemeName'
GO

UPDATE [Setting]
SET [name] = N'storeinformationsettings.defaultstoretheme'
WHERE [name] = N'storeinformationsettings.defaultstorethemefordesktops'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.newslettertickedbydefault')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.newslettertickedbydefault', N'true', 0)
END
GO

--rename setting
UPDATE [Setting]
SET [name] = N'catalogsettings.dynamicpriceupdateajax'
WHERE [name] = N'catalogsettings.enabledynamicskumpngtinupdate'
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='ReplyTo')
BEGIN
	ALTER TABLE [QueuedEmail]
	ADD [ReplyTo] nvarchar(500) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='ReplyToName')
BEGIN
	ALTER TABLE [QueuedEmail]
	ADD [ReplyToName] nvarchar(500) NULL
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
		SET @sql = @sql + '
		AND p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max))
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


--more SQL indexes
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PSAM_ProductId' and object_id=object_id(N'[Product_SpecificationAttribute_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PSAM_ProductId] ON [Product_SpecificationAttribute_Mapping] ([ProductId] ASC)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'paypalstandardpaymentsettings.addressoverride')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'paypalstandardpaymentsettings.addressoverride', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.generateproductmetadescription')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'seosettings.generateproductmetadescription', N'true', 0)
END
GO


--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Language]') and NAME='DefaultCurrencyId')
BEGIN
	ALTER TABLE [Language]
	ADD [DefaultCurrencyId] int NULL
END
GO

UPDATE [Language]
SET [DefaultCurrencyId] = 0
WHERE [DefaultCurrencyId] IS NULL
GO

ALTER TABLE [Language] ALTER COLUMN [DefaultCurrencyId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.twittermetatags')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'seosettings.twittermetatags', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.opengraphmetatags')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'seosettings.opengraphmetatags', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.displayshipmenteventstostoreowner')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.displayshipmenteventstostoreowner', N'false', 0)
END
GO

--upgrade scripts from nopCommerce 2.40 to nopCommerce 2.50

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackByDimensions">
    <Value>Pack by dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackByOneItemPerPackage">
    <Value>Pack by one item per package</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackByVolume">
    <Value>Pack by volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingType">
    <Value>Packing type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingType.Hint">
    <Value>Choose preferred packing type.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingPackageVolume">
    <Value>Package volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingPackageVolume.Hint">
    <Value>Enter your package volume.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnFrom">
    <Value>Created from</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnFrom.Hint">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnTo">
    <Value>Created to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnTo.Hint">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.ApproveSelected">
    <Value>Approve selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.DisapproveSelected">
    <Value>Disapprove selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.All">
    <Value>Export to XML (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.Selected">
    <Value>Export to XML (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.All">
    <Value>Export to Excel (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.Selected">
    <Value>Export to Excel (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExcelFile">
    <Value>Excel file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile">
    <Value>Xml file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile.Note1">
    <Value>NOTE: It can take up to several minutes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile.Note2">
    <Value>NOTE: DO NOT click twice.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.CsvFile">
    <Value>CSV file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.LoseUnsavedChanges">
    <Value>You are going to lose any unsaved changes. Are you sure?</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.Match">
    <Value>Specified store URL matches this store URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.NoMatch">
    <Value>Specified store URL ({0}) doesn''t match this store URL ({1})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.Set">
    <Value>Primary exchange rate currency is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.Rate1">
    <Value>Primary exchange rate currency. The rate should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.NotSet">
    <Value>Primary exchange rate currency is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PrimaryCurrency.Set">
    <Value>Primary store currency is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PrimaryCurrency.NotSet">
    <Value>Primary store currency is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.Set">
    <Value>Default weight is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.Ratio1">
    <Value>Default weight. The ratio should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.NotSet">
    <Value>Default weight is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.Set">
    <Value>Default dimension is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.Ratio1">
    <Value>Default dimension. The ratio should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.NotSet">
    <Value>Default dimension is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Shipping.OnlyOneOffline">
    <Value>Only one offline shipping rate computation method is recommended to use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PaymentMethods.OK">
    <Value>Payment methods are OK</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PaymentMethods.NoActive">
    <Value>You don''t have active payment methods</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.CantDeleteExchange">
    <Value>The primary exchange rate currency can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.CantDeletePrimary">
    <Value>The primary store currency can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.CantDeletePrimary">
    <Value>The primary weight can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.CantDeletePrimary">
    <Value>The primary dimension can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.States.CantDeleteWithAddresses">
    <Value>The state can''t be deleted. It has associated addresses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description">
    <Value>Manual plugin installation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step1">
    <Value>Upload the plugin to the /plugins folder in your nopCommerce directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step2">
    <Value>Restart your application (or click ''Reload list of plugins'' button).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step3">
    <Value>Scroll down through the list of plugins to find the newly installed plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step4">
    <Value>Click on the ''Install'' link to install the plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step5">
    <Value>Note: If you''re running nopCommerce in medium trust, then it''s recommended to clear your \Plugins\bin\ directory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing">
    <Value>Editing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing.Hint">
    <Value>This grid allows the bulk editing of the ''Friendly name'' and ''Display order'' fields. To enter edit mode just click a cell.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.Installation">
    <Value>Installation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage">
    <Value>Show on login page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage.Hint">
    <Value>Check to show CAPTCHA on login page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage">
    <Value>Show on ''email wishlist to a friend'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage.Hint">
    <Value>Check to show CAPTCHA on ''email wishlist to a friend'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage">
    <Value>Show on ''email product to a friend'' page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage.Hint">
    <Value>Check to show CAPTCHA on ''email product to a friend'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.Name">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.Price">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.NameAsc">
    <Value>Name: A to Z</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.NameDesc">
    <Value>Name: Z to A</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.PriceAsc">
    <Value>Price: Low to High</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.ProductSortingEnum.PriceDesc">
    <Value>Price: High to Low</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct">
    <Value>Display wishlist after adding product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct.Hint">
    <Value>If checked, a customer will be taken to the Wishlist page immediately after adding a product to their wishlist. If unchecked, a customer will stay on the same page that they are adding the product to the wishlist from.</Value>
  </LocaleResource>
  <LocaleResource Name="Products.ProductHasBeenAddedToTheWishlist">
    <Value>The product has been added to the wishlist</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers">
    <Value>Display shipment events</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers.Hint">
    <Value>Check if you want your customers to see shipment events on their order details pages (if supported by your shipping rate computation method).</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents">
    <Value>Shipment status events</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Event">
    <Value>Event</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Location">
    <Value>Location</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Order.ShipmentStatusEvents.Date">
    <Value>Date</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Departed">
    <Value>Departed</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.ExportScanned">
    <Value>Export scanned</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.OriginScanned">
    <Value>Origin scanned</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Arrived">
    <Value>Arrived</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.NotDelivered">
    <Value>Not delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Booked">
    <Value>Booked</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.UPS.Tracker.Departed.Delivered">
    <Value>Delivered</Value>
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
				[LanguageID],
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

--Customer currency rate issue fix
ALTER TABLE [dbo].[Order] ALTER COLUMN [CurrencyRate] decimal(18, 8) NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytierpriceswithdiscounts')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.displaytierpriceswithdiscounts', N'true')
END
GO
--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fedexsettings.packingpackagevolume')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'fedexsettings.packingpackagevolume', N'5184')
END
GO


--Update stored procedure according to the sort options
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryId			int = 0,
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	DECLARE @SearchKeywords bit
	SET @SearchKeywords = 1
	IF (@Keywords IS NULL OR @Keywords = N'')
		SET @SearchKeywords = 0

	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',');
	
	DECLARE @SpecAttributesCount int	
	SELECT @SpecAttributesCount = COUNT(1) FROM #FilteredSpecs

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

	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM Product p with (NOLOCK) 
	LEFT OUTER JOIN Product_Category_Mapping pcm with (NOLOCK) ON p.Id=pcm.ProductId
	LEFT OUTER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.Id=pmm.ProductId
	LEFT OUTER JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON p.Id=pptm.Product_Id
	LEFT OUTER JOIN ProductVariant pv with (NOLOCK) ON p.Id = pv.ProductId
	--searching of the localized values
	--comment the line below if you don't use it. It'll improve the performance
	LEFT OUTER JOIN LocalizedProperty lp with (NOLOCK) ON p.Id = lp.EntityId AND lp.LanguageId = @LanguageId AND lp.LocaleKeyGroup = N'Product'
	WHERE 
		(
		   (
				@CategoryId IS NULL OR @CategoryId=0
				OR (pcm.CategoryId=@CategoryId AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerId IS NULL OR @ManufacturerId=0
				OR (pmm.ManufacturerId=@ManufacturerId AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagId IS NULL OR @ProductTagId=0
				OR pptm.ProductTag_Id=@ProductTagId
			)
		AND	(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Deleted = 0
			)
		AND (
				--min price
				(@PriceMin IS NULL OR @PriceMin=0)
				OR 
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.SpecialPrice >= @PriceMin)
				)
				OR 
				(
					--regular price (price isn't specified or date range isn't valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.Price >= @PriceMin)
				)
			)
		AND (
				--max price
				(@PriceMax IS NULL OR @PriceMax=2147483644) -- max value
				OR 
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.SpecialPrice <= @PriceMax)
				)
				OR 
				(
					--regular price (price isn't specified or date range isn't valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, '1/1/1900') AND isnull(pv.SpecialPriceEndDateTimeUtc, '1/1/2999')))
					AND
					(pv.Price <= @PriceMax)
				)
			)
		AND	(
				@SearchKeywords = 0 or 
				(
					-- search standard content
					patindex(@Keywords, p.name) > 0
					or patindex(@Keywords, pv.name) > 0
					or patindex(@Keywords, pv.sku) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pv.Description) > 0)					
					--searching of the localized values
					--comment the lines below if you don't use it. It'll improve the performance
					or (lp.LocaleKey = N'Name' and patindex(@Keywords, lp.LocaleValue) > 0)
					or (@SearchDescriptions = 1 and lp.LocaleKey = N'ShortDescription' and patindex(@Keywords, lp.LocaleValue) > 0)
					or (@SearchDescriptions = 1 and lp.LocaleKey = N'FullDescription' and patindex(@Keywords, lp.LocaleValue) > 0)
				)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTimeUtc, '1/1/1900') and isnull(pv.AvailableEndDateTimeUtc, '1/1/2999'))
			)
		AND
			(
				--filter by specs
				@SpecAttributesCount = 0
				OR
				(
					NOT EXISTS(
						SELECT 1 
						FROM #FilteredSpecs [fs]
						WHERE [fs].SpecificationAttributeOptionId NOT IN (
							SELECT psam.SpecificationAttributeOptionId
							FROM dbo.Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
							)
						)
					
				)
			)
		)
	ORDER BY 
		--category position
		CASE WHEN @OrderBy = 0 AND @CategoryId IS NOT NULL AND @CategoryId > 0
		THEN pcm.DisplayOrder END ASC,
		--manufacturer position
		CASE WHEN @OrderBy = 0 AND @ManufacturerId IS NOT NULL AND @ManufacturerId > 0
		THEN pmm.DisplayOrder END ASC,
		--sort by name (there's no any position if category or manufactur is not specified)
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		--Name: A to Z
		CASE WHEN @OrderBy = 5
		THEN p.[Name] END ASC, --THEN dbo.[nop_getnotnullnotempty](pl.[Name],p.[Name]) END ASC,
		--Name: Z to A
		CASE WHEN @OrderBy = 6
		THEN p.[Name] END DESC, --THEN dbo.[nop_getnotnullnotempty](pl.[Name],p.[Name]) END DESC,
		--Price: Low to High
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		--Price: High to Low
		CASE WHEN @OrderBy = 11
		THEN pv.Price END DESC,
		--Created on
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOnUtc END DESC

	DROP TABLE #FilteredSpecs

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	SET ROWCOUNT @RowsToReturn
	
	DROP TABLE #DisplayOrderTmp

	--return products (returned properties should be synchronized with 'Product' entity)
	SELECT  
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		IndexId
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.displaywishlistafteraddingproduct')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.displaywishlistafteraddingproduct', N'true')
END
GO

--more SQL indexes
IF NOT EXISTS (SELECT 1 from dbo.sysindexes WHERE [NAME]=N'IX_Product_Deleted_and_Published' and id=object_id(N'[dbo].[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_Deleted_and_Published] ON [dbo].[Product] 
	(
		[Published] ASC,
		[Deleted] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from dbo.sysindexes WHERE [NAME]=N'IX_Product_Published' and id=object_id(N'[dbo].[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_Published] ON [dbo].[Product] 
	(
		[Published] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from dbo.sysindexes WHERE [NAME]=N'IX_Product_ShowOnHomepage' and id=object_id(N'[dbo].[Product]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Product_ShowOnHomepage] ON [dbo].[Product] 
	(
		[ShowOnHomepage] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from dbo.sysindexes WHERE [NAME]=N'IX_ProductVariant_ProductId_2' and id=object_id(N'[dbo].[ProductVariant]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId_2] ON [dbo].[ProductVariant] 
	(
		[ProductId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from dbo.sysindexes WHERE [NAME]=N'IX_PCM_Product_and_Category' and id=object_id(N'[dbo].[Product_Category_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PCM_Product_and_Category] ON [dbo].[Product_Category_Mapping] 
	(
		[CategoryId] ASC,
		[ProductId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from dbo.sysindexes WHERE [NAME]=N'IX_PMM_Product_and_Manufacturer' and id=object_id(N'[dbo].[Product_Manufacturer_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PMM_Product_and_Manufacturer] ON [dbo].[Product_Manufacturer_Mapping] 
	(
		[ManufacturerId] ASC,
		[ProductId] ASC
	)
END
GO



--New fast [ProductLoadAllPaged] stored procedure
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryId			int = 0,
	@ManufacturerId		int = 0,
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(MAX) = null,
	@SearchDescriptions bit = 0,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
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
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		SET @Keywords = isnull(@Keywords, '')
		SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'
		
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.name) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.sku) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
			
		IF @SearchDescriptions = 1 SET @sql = @sql + '
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.ShortDescription) > 0
		UNION
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE PATINDEX(@Keywords, p.FullDescription) > 0
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE PATINDEX(@Keywords, pv.Description) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''ShortDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''FullDescription''
			AND PATINDEX(@Keywords, lp.LocaleValue) > 0'
		
		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(MAX)', @Keywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@FilteredSpecs, ',')	
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)

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
	
	IF @CategoryId > 0
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
	IF @CategoryId > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId = ' + CAST(@CategoryId AS nvarchar(max))
		
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
	
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 
			FROM
				#FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM dbo.Product_SpecificationAttribute_Mapping psam
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
		IF @CategoryId > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
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
	
	PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredSpecs

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
		INNER JOIN Product p on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

--new shipping setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.displaytrackingurltocustomers')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shippingsettings.displaytrackingurltocustomers', N'false')
END
GO
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.displayshipmenteventstocustomers')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shippingsettings.displayshipmenteventstocustomers', N'false')
END
GO

--'New order note' message template
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[MessageTemplate]
		WHERE [Name] = N'Customer.NewOrderNote')
BEGIN
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [EmailAccountId])
	VALUES (N'Customer.NewOrderNote', null, N'%Store.Name%. New order note has been added', N'<p><a href="%Store.URL%">%Store.Name%</a> <br /><br />Hello %Customer.FullName%, <br />New order note has been added to your account:<br />"%Order.NewNoteText%".<br /><a target="_blank" href="%Order.OrderURLForCustomer%">%Order.OrderURLForCustomer%</a></p>', 1, 0)
END
GO
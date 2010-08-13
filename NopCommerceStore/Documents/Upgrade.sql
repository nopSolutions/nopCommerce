IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.HideNewsletterBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.HideNewsletterBox', N'False', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='IsPasswordProtected')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD [IsPasswordProtected] bit NOT NULL CONSTRAINT [DF_Nop_Topic_IsPasswordProtected] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='Password')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD [Password] nvarchar(200) NOT NULL CONSTRAINT [DF_Nop_Topic_Password] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLetterSubscriptionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_NewsLetterSubscriptionLoadAll]
(
	@Email		nvarchar(200),
	@ShowHidden bit = 0
)
AS
BEGIN
	
	SET @Email = isnull(@Email, '')
	SET @Email = '%' + rtrim(ltrim(@Email)) + '%'


	SET NOCOUNT ON
	SELECT 
		nls.NewsLetterSubscriptionId,
		nls.NewsLetterSubscriptionGuid,
		nls.Email,
		nls.Active,
		nls.CreatedOn
	FROM
		[Nop_NewsLetterSubscription] nls
	LEFT OUTER JOIN 
		Nop_Customer c 
	ON 
		nls.Email=c.Email
	WHERE 
		(patindex(@Email, isnull(nls.Email, '')) > 0) AND
		(nls.Active = 1 OR @ShowHidden = 1) AND 
		(c.CustomerID IS NULL OR (c.Active = 1 AND c.Deleted = 0))
	ORDER BY nls.Email
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Forums.CustomersAllowedToManageSubscriptions')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Forums.CustomersAllowedToManageSubscriptions', N'False', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='DisplayStockQuantity')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD DisplayStockQuantity bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_DisplayStockQuantity] DEFAULT ((0))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.AllowOutOfStockOrders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SEO.EnableUrlRewriting')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SEO.EnableUrlRewriting', N'True', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='RequirementSpentAmount')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [RequirementSpentAmount] money NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementSpentAmount] DEFAULT ((0))
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 30)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (30, N'Had spent x.xx amount')
END
GO

IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='Backorders')
BEGIN

	ALTER TABLE [dbo].[Nop_ProductVariant] DROP CONSTRAINT [DF_Nop_ProductVariant_AllowOutOfStockOrders]

	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD Backorders int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_Backorders] DEFAULT ((0))

	EXEC ('UPDATE [dbo].[Nop_ProductVariant] SET [Backorders]=[AllowOutOfStockOrders]')

	ALTER TABLE [dbo].[Nop_ProductVariant] DROP COLUMN AllowOutOfStockOrders
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


IF NOT EXISTS (
		SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='GiftCardType')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD GiftCardType int NOT NULL CONSTRAINT [DF_Nop_ProductVariant_GiftCardType] DEFAULT ((0))

END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.GiftCardType,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Product_Nop_ProductType'
           AND parent_obj = Object_id('Nop_Product')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Product
DROP CONSTRAINT FK_Nop_Product_Nop_ProductType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ProductType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_ProductType]
ALTER TABLE [dbo].[Nop_Product] DROP CONSTRAINT [DF_Nop_Product_ProductTypeID]
ALTER TABLE [dbo].[Nop_Product] DROP COLUMN ProductTypeID
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@Keywords			nvarchar(MAX),	
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	SET @PriceMin = isnull(@PriceMin, 0)
	SET @PriceMax = isnull(@PriceMax, 2147483644)
	
	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL,
		[Name] nvarchar(400) not null,
		[Price] money not null,
		[DisplayOrder1] int,
		[DisplayOrder2] int,
	)

	INSERT INTO #DisplayOrderTmp ([ProductID], [Name], [Price], [DisplayOrder1], [DisplayOrder2])
	SELECT p.ProductID, p.Name, pv.Price, pcm.DisplayOrder, pmm.DisplayOrder 
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				pv.Price BETWEEN @PriceMin AND @PriceMax
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pv.Description, '')) > 0)					
				-- search language content
				or patindex(@Keywords, isnull(pl.name, '')) > 0
				or patindex(@Keywords, isnull(pvl.name, '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pvl.Description, '')) > 0)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END,
		CASE WHEN @OrderBy = 0
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #FilteredSpecs
	DROP TABLE #DisplayOrderTmp
	DROP TABLE #PageIndex
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductAlsoPurchasedLoadByProductID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
GO
CREATE PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
(
	@ProductID			int,
	@ShowHidden			bit,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductID int NOT NULL,
		ProductsPurchased int NOT NULL,
	)

	INSERT INTO #PageIndex (ProductID, ProductsPurchased)
	SELECT p.ProductID, SUM(opv.Quantity) as ProductsPurchased
	FROM    
		dbo.Nop_OrderProductVariant opv WITH (NOLOCK)
		INNER JOIN dbo.Nop_ProductVariant pv ON pv.ProductVariantId = opv.ProductVariantId
		INNER JOIN dbo.Nop_Product p ON p.ProductId = pv.ProductId
	WHERE
		opv.OrderID IN 
		(
			/* This inner query should retrieve all orders that have contained the productID */
			SELECT 
				DISTINCT OrderID
			FROM 
				dbo.Nop_OrderProductVariant opv2 WITH (NOLOCK)
				INNER JOIN dbo.Nop_ProductVariant pv2 ON pv2.ProductVariantId = opv2.ProductVariantId
				INNER JOIN dbo.Nop_Product p2 ON p2.ProductId = pv2.ProductId			
			WHERE 
				p2.ProductID = @ProductID
		)
		AND 
			(
				p.ProductId != @ProductID
			)
		AND 
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted = 0
			)
		AND 
			(
				@ShowHidden = 1
				OR
				GETUTCDATE() BETWEEN ISNULL(pv.AvailableStartDateTime, '1/1/1900') AND ISNULL(pv.AvailableEndDateTime, '1/1/2999')
			)
	GROUP BY
		p.ProductId
	ORDER BY 
		ProductsPurchased desc


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex

END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider1.Classname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider1.Classname', N'NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates.EcbExchangeRateProvider, Nop.BusinessLogic', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider2.Classname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider2.Classname', N'NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates.McExchangeRateProvider, Nop.BusinessLogic', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider3.Classname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider3.Classname', N'', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'ExchangeRateProvider.Current')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'ExchangeRateProvider.Current', N'1', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_SMSProvider]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_SMSProvider](
	[SMSProviderId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ClassName] [nvarchar](500) NOT NULL,
	[SystemKeyword] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SMSProvider] PRIMARY KEY CLUSTERED 
(
	[SMSProviderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Cache.SMSManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Cache.SMSManager.CacheEnabled', N'True', N'')
END
GO

IF EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.IsEnabled')
BEGIN
	DELETE FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.IsEnabled'
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.Clickatell.PhoneNumber')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Mobile.SMS.Clickatell.PhoneNumber', N'', N'')
END
GO

IF EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.AdminPhoneNumber')
BEGIN
	DELETE FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.AdminPhoneNumber'
END
GO

IF NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditSMSProviders')
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditSMSProviders', N'Edit SMS provider settings', 1)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageSMSProviders')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage SMS Providers', N'ManageSMSProviders', N'',305)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_SMSProvider]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.BusinessLogic.Messages.ClickatellSMSProvider, Nop.BusinessLogic')
BEGIN
	INSERT [dbo].[Nop_SMSProvider] ([Name], [ClassName], [SystemKeyword], [IsActive]) 
	VALUES (N'Clickatell', N'NopSolutions.NopCommerce.BusinessLogic.Messages.ClickatellSMSProvider, Nop.BusinessLogic', N'SMSPROVIDERS_CLICKATELL', 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Country]') and NAME='SubjectToVAT')
BEGIN
	ALTER TABLE [dbo].[Nop_Country] 
	ADD [SubjectToVAT] bit NOT NULL CONSTRAINT [DF_Nop_Country_SubjectToVAT] DEFAULT ((0))
END
GO

UPDATE		Nop_Country
SET			SubjectToVAT = 1
WHERE		(TwoLetterISOCode IN (N'AT', N'BE', N'BG', N'CY', N'CZ', N'DE', N'DK', N'EE', N'ES', N'FI', N'FR', N'GB', N'GR', N'HU', N'IE', N'IT', N'LT', N'LU', N'LV', N'MT', N'NL', N'PL', N'PT', N'RO', N'SE', N'SI', N'SK'))
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='VatNumber')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [VatNumber] nvarchar(100) NOT NULL CONSTRAINT [DF_Nop_Order_VatNumber] DEFAULT ((''))
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewVATSubmitted.StoreOwnerNotification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewVATSubmitted.StoreOwnerNotification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewVATSubmitted.StoreOwnerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'', N'New VAT number is submitted.',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
<br />
%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number (%Customer.VatNumber%).
</p>')
	END
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_SMSProvider]
		WHERE [ClassName] = N'NopSolutions.NopCommerce.BusinessLogic.Messages.VerizonSMSProvider, Nop.BusinessLogic')
BEGIN
	INSERT [dbo].[Nop_SMSProvider] ([Name], [ClassName], [SystemKeyword], [IsActive]) 
	VALUES (N'Verizon', N'NopSolutions.NopCommerce.BusinessLogic.Messages.VerizonSMSProvider, Nop.BusinessLogic', N'SMSPROVIDERS_VERIZON', 0)
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Mobile.SMS.Verizon.Email')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Mobile.SMS.Verizon.Email', N'yournumber@vtext.com', N'')
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionLoadNonEmpty]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
BEGIN
DROP PROCEDURE [dbo].[Nop_CustomerSessionLoadNonEmpty]
END
GO

CREATE PROCEDURE [dbo].[Nop_CustomerSessionLoadNonEmpty]
AS
BEGIN
	SET NOCOUNT OFF
		
	SELECT
		*
	FROM 
		[Nop_CustomerSession] cs
	WHERE 
		CustomerSessionGUID 
	IN
	(
		SELECT DISTINCT sci.CustomerSessionGUID FROM [Nop_ShoppingCartItem] sci
	)
	ORDER BY cs.LastAccessed desc
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPHostname')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPHostname', N'ftp://uploads.google.com', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPFilename')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPFilename', N'', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPUsername')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPUsername', N'', N'')
END
GO

IF NOT EXISTS ( SELECT 1 FROM [dbo].[Nop_Setting] WHERE [Name] = N'Froogle.FTPPassword')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description]) VALUES (N'Froogle.FTPPassword', N'', N'')
END
GO

UPDATE [dbo].[Nop_DiscountType] 
SET [Name] = N'Assigned to order total'
WHERE [DiscountTypeID] = 1
GO

--return requests
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ReturnRequest]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_ReturnRequest](
	[ReturnRequestId] [int] IDENTITY(1,1) NOT NULL,
	[OrderProductVariantId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ReasonForReturn] [nvarchar](400) NOT NULL,
	[RequestedAction] [nvarchar](400) NOT NULL,
	[CustomerComments] [nvarchar](MAX) NOT NULL,
	[StaffNotes] [nvarchar](MAX) NOT NULL,
	[ReturnStatusId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Nop_ReturnRequest] PRIMARY KEY CLUSTERED 
(
	[ReturnRequestId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ReturnRequest_Nop_OrderProductVariant'
           AND parent_obj = Object_id('Nop_ReturnRequest')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ReturnRequest
DROP CONSTRAINT FK_Nop_ReturnRequest_Nop_OrderProductVariant
GO
ALTER TABLE [dbo].[Nop_ReturnRequest]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ReturnRequest_Nop_OrderProductVariant] FOREIGN KEY([OrderProductVariantId])
REFERENCES [dbo].[Nop_OrderProductVariant] ([OrderProductVariantId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ReturnRequest_Nop_Customer'
           AND parent_obj = Object_id('Nop_ReturnRequest')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ReturnRequest
DROP CONSTRAINT FK_Nop_ReturnRequest_Nop_Customer
GO
ALTER TABLE [dbo].[Nop_ReturnRequest]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ReturnRequest_Nop_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Nop_Customer] ([CustomerId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ReturnRequests.Enable')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ReturnRequests.Enable', N'True', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ReturnRequests.ReturnReasons')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ReturnRequests.ReturnReasons', N'Received Wrong Product, Wrong Product Ordered, There Was A Problem With The Product', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'ReturnRequests.ReturnActions')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'ReturnRequests.ReturnActions', N'Repair, Replacement, Store Credit', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_CustomerAction]
		WHERE [SystemKeyword] = N'ManageReturnRequests')
BEGIN
	INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
	VALUES (N'Manage Return Requests', N'ManageReturnRequests', N'',45)
END
GO

IF NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'EditReturnRequest')
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'EditReturnRequest', N'Edit a return request', 1)
END
GO

IF NOT EXISTS(SELECT 1 FROM [Nop_ActivityLogType] WHERE [SystemKeyword] = N'DeleteReturnRequest')
BEGIN
	INSERT INTO [Nop_ActivityLogType] ([SystemKeyword], [Name], [Enabled]) VALUES (N'DeleteReturnRequest', N'Delete a return request', 1)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Customer.FormatNameMaxLength')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Customer.FormatNameMaxLength', N'0', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Shipping.EstimateShipping.Enabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Shipping.EstimateShipping.Enabled', N'true', N'')
END
GO



IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@Keywords			nvarchar(MAX),	
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	SET @PriceMin = isnull(@PriceMin, 0)
	SET @PriceMax = isnull(@PriceMax, 2147483644)
	
	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL,
		[Name] nvarchar(400) not null,
		[Price] money not null,
		[DisplayOrder1] int,
		[DisplayOrder2] int,
	)

	INSERT INTO #DisplayOrderTmp ([ProductID], [Name], [Price], [DisplayOrder1], [DisplayOrder2])
	SELECT p.ProductID, p.Name, pv.Price, pcm.DisplayOrder, pmm.DisplayOrder 
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
			(
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
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				pv.Price BETWEEN @PriceMin AND @PriceMax
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pv.Description, '')) > 0)					
				-- search language content
				or patindex(@Keywords, isnull(pl.name, '')) > 0
				or patindex(@Keywords, isnull(pvl.name, '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pvl.Description, '')) > 0)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END,
		CASE WHEN @OrderBy = 0
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #FilteredSpecs
	DROP TABLE #DisplayOrderTmp
	DROP TABLE #PageIndex
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductAlsoPurchasedLoadByProductID]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
GO
CREATE PROCEDURE [dbo].[Nop_ProductAlsoPurchasedLoadByProductID]
(
	@ProductID			int,
	@ShowHidden			bit,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductID int NOT NULL,
		ProductsPurchased int NOT NULL,
	)

	INSERT INTO #PageIndex (ProductID, ProductsPurchased)
	SELECT p.ProductID, SUM(opv.Quantity) as ProductsPurchased
	FROM    
		dbo.Nop_OrderProductVariant opv WITH (NOLOCK)
		INNER JOIN dbo.Nop_ProductVariant pv ON pv.ProductVariantId = opv.ProductVariantId
		INNER JOIN dbo.Nop_Product p ON p.ProductId = pv.ProductId
	WHERE
		opv.OrderID IN 
		(
			/* This inner query should retrieve all orders that have contained the productID */
			SELECT 
				DISTINCT OrderID
			FROM 
				dbo.Nop_OrderProductVariant opv2 WITH (NOLOCK)
				INNER JOIN dbo.Nop_ProductVariant pv2 ON pv2.ProductVariantId = opv2.ProductVariantId
				INNER JOIN dbo.Nop_Product p2 ON p2.ProductId = pv2.ProductId			
			WHERE 
				p2.ProductID = @ProductID
		)
		AND 
			(
				p.ProductId != @ProductID
			)
		AND 
			(
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
		AND 
			(
				@ShowHidden = 1
				OR
				GETUTCDATE() BETWEEN ISNULL(pv.AvailableStartDateTime, '1/1/1900') AND ISNULL(pv.AvailableEndDateTime, '1/1/2999')
			)
	GROUP BY
		p.ProductId
	ORDER BY 
		ProductsPurchased desc


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex

END
GO

ALTER TABLE [dbo].[Nop_Order] ALTER COLUMN [CardName] nvarchar(1000) NOT NULL
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='TaxRates')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [TaxRates] nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_Order_TaxRates] DEFAULT ((''))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='TaxRatesInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [TaxRatesInCustomerCurrency] nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_Order_TaxRatesInCustomerCurrency] DEFAULT ((''))
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'OnlineUserManager.Enabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'OnlineUserManager.Enabled', N'False', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.DownloadableProductsTab')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.DownloadableProductsTab', N'True', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='RefundedAmount')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [RefundedAmount] money NOT NULL CONSTRAINT [DF_Nop_Order_RefundedAmount] DEFAULT ((0))

	exec ('UPDATE [dbo].[Nop_Order]
	SET [RefundedAmount]=[OrderTotal]
	WHERE [PaymentStatusID]=40')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_PaymentStatus]
		WHERE [PaymentStatusID] = 35)
BEGIN
	INSERT [dbo].[Nop_PaymentStatus] ([PaymentStatusID], [Name])
	VALUES (35, N'PartiallyRefunded')
END
GO

DELETE FROM [dbo].[Nop_Setting] 
WHERE [Name] = N'Tax.TaxProvider.FixedRate.Rate'
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Checkout.DiscountCouponBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Checkout.DiscountCouponBox', N'True', N'')
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Checkout.GiftCardBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Checkout.GiftCardBox', N'True', N'')
END
GO

--"call for price" option

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_ProductVariant]') and NAME='CallForPrice')
BEGIN
	ALTER TABLE [dbo].[Nop_ProductVariant] 
	ADD [CallForPrice] bit NOT NULL CONSTRAINT [DF_Nop_ProductVariant_CallForPrice] DEFAULT ((0))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductVariantLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_ProductVariantLoadAll]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@Keywords			nvarchar(MAX),
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--display order
	CREATE TABLE #DisplayOrder
	(
		ProductID int not null PRIMARY KEY,
		DisplayOrder int not null
	)	

	IF @CategoryID IS NOT NULL AND @CategoryID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pcm.ProductID, pcm.DisplayOrder 
			FROM [Nop_Product_Category_Mapping] pcm WHERE pcm.CategoryID = @CategoryID
		END
    ELSE IF @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		BEGIN
			INSERT #DisplayOrder 
			SELECT pmm.ProductID, pmm.Displayorder 
			FROM [Nop_Product_Manufacturer_Mapping] pmm WHERE pmm.ManufacturerID = @ManufacturerID
		END
	ELSE
		BEGIN
			INSERT #DisplayOrder 
			SELECT p.ProductID, 1 
			FROM [Nop_Product] p
			ORDER BY p.[Name]
		END
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		ProductVariantID int NOT NULL,
		DisplayOrder int NOT NULL,
	)
	INSERT INTO #PageIndex (ProductVariantID, DisplayOrder)
	SELECT DISTINCT pv.ProductVariantID, do.DisplayOrder
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	JOIN #DisplayOrder do on p.ProductID = do.ProductID
	WHERE 
		(
			(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				p.Deleted=0 and pv.Deleted=0 and pv.ProductVariantID is not null
			)
		AND (
				@CategoryID IS NULL OR @CategoryID=0
				OR pcm.CategoryID=@CategoryID
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR pmm.ManufacturerID=@ManufacturerID
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		)
	ORDER BY do.DisplayOrder

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		pv.ProductVariantId,
		pv.ProductId,
		pv.Name,
		pv.SKU,
		pv.Description,
		pv.AdminComment,
		pv.ManufacturerPartNumber,
		pv.IsGiftCard,
		pv.GiftCardType,
		pv.IsDownload,
		pv.DownloadId,
		pv.UnlimitedDownloads,
		pv.MaxNumberOfDownloads,
		pv.DownloadExpirationDays,
		pv.DownloadActivationType,
		pv.HasSampleDownload,
		pv.SampleDownloadId,
		pv.HasUserAgreement,
		pv.UserAgreementText,
		pv.IsRecurring,
		pv.CycleLength,
		pv.CyclePeriod,
		pv.TotalCycles,
		pv.IsShipEnabled,
		pv.IsFreeShipping,
		pv.AdditionalShippingCharge,
		pv.IsTaxExempt,
		pv.TaxCategoryId,
		pv.ManageInventory,
		pv.StockQuantity,
		pv.DisplayStockAvailability,
		pv.DisplayStockQuantity,
		pv.MinStockQuantity,
		pv.LowStockActivityId,
		pv.NotifyAdminForQuantityBelow,
		pv.Backorders,
		pv.OrderMinimumQuantity,
		pv.OrderMaximumQuantity,
		pv.WarehouseId,
		pv.DisableBuyButton,
		pv.CallForPrice,
		pv.Price,
		pv.OldPrice,
		pv.ProductCost,
		pv.CustomerEntersPrice,
		pv.MinimumCustomerEnteredPrice,
		pv.MaximumCustomerEnteredPrice,
		pv.Weight,
		pv.Length,
		pv.Width,
		pv.Height,
		pv.PictureId,
		pv.AvailableStartDateTime,
		pv.AvailableEndDateTime,
		pv.Published,
		pv.Deleted,
		pv.DisplayOrder,
		pv.CreatedOn,
		pv.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_ProductVariant pv on pv.ProductVariantID = [pi].ProductVariantID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #DisplayOrder
	DROP TABLE #PageIndex
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_BlogPost]') and NAME='Tags')
BEGIN
	ALTER TABLE [dbo].[Nop_BlogPost] 
	ADD [Tags] nvarchar(4000) NOT NULL CONSTRAINT [DF_Nop_BlogPost_Tags] DEFAULT ((''))
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_BlogPostLoadAll]
(
	@LanguageID	int,
	@PageSize			int = 2147483644,
	@PageIndex			int = 0, 
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		BlogPostID int NOT NULL,
	)

	INSERT INTO #PageIndex (BlogPostID)
	SELECT
		bp.BlogPostID
	FROM 
	    Nop_BlogPost bp 
	WITH 
		(NOLOCK)
	WHERE
		@LanguageID IS NULL OR @LanguageID = 0 OR LanguageID = @LanguageID
	ORDER BY 
		CreatedOn 
	DESC


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		bp.[BlogPostId],
		bp.[LanguageId],
		bp.[BlogPostTitle],
		bp.[BlogPostBody],
		bp.[BlogPostAllowComments],
		bp.[Tags],
		bp.[CreatedById],
		bp.[CreatedOn]
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_BlogPost bp on bp.BlogPostID = [pi].BlogPostID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0
END
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_TopicLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_TopicLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_TopicLoadAll]
(
	@ForumID			int,
	@UserID				int,
	@Keywords			nvarchar(MAX),	
	@SearchType			int = 0,
	@LimitDate			datetime,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		TopicID int NOT NULL,
		TopicTypeID int NOT NULL,
		LastPostTime datetime NULL,
	)

	INSERT INTO #PageIndex (TopicID, TopicTypeID, LastPostTime)
	SELECT DISTINCT
		ft.TopicID, ft.TopicTypeID, ft.LastPostTime
	FROM Nop_Forums_Topic ft with (NOLOCK) 
	LEFT OUTER JOIN Nop_Forums_Post fp with (NOLOCK) ON ft.TopicID = fp.TopicID
	WHERE  (
				@ForumID IS NULL OR @ForumID=0
				OR (ft.ForumID=@ForumID)
			)
		AND (
				@UserID IS NULL OR @UserID=0
				OR (ft.UserID=@UserID)
			)
		AND	(
				((@SearchType = 0 or @SearchType = 10) and patindex(@Keywords, ft.Subject) > 0)
				OR
				((@SearchType = 0 or @SearchType = 20) and patindex(@Keywords, fp.Text) > 0)
			)
		AND (
				@LimitDate IS NULL
				OR (DATEDIFF(second, @LimitDate, ft.LastPostTime) > 0)
			)
	ORDER BY ft.TopicTypeID desc, ft.LastPostTime desc, ft.TopicID desc

	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		ft.TopicID as ForumTopicId,
		ft.ForumId,
		ft.UserId,
		ft.TopicTypeId,
		ft.Subject,
		ft.NumPosts,
		ft.Views,
		ft.LastPostId,
		ft.LastPostUserId,
		ft.LastPostTime,
		ft.CreatedOn,
		ft.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Forums_Topic ft on ft.TopicID = [pi].TopicID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostDelete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostDelete]
GO
CREATE PROCEDURE [dbo].[Nop_Forums_PostDelete]
(
	@PostID int
)
AS
BEGIN
	SET NOCOUNT ON

	declare @UserID int
	declare @ForumID int
	declare @TopicID int
	SELECT 
		@UserID = fp.UserID,
		@ForumID = ft.ForumID,
		@TopicID = ft.TopicID
	FROM
		[Nop_Forums_Topic] ft
		INNER JOIN 
		[Nop_Forums_Post] fp
		ON ft.TopicID=fp.TopicID
	WHERE
		fp.PostID = @PostID 
	
	DELETE
	FROM [Nop_Forums_Post]
	WHERE
		PostID = @PostID

	--update stats/info
	exec [dbo].[Nop_Forums_TopicUpdateCounts] @TopicID
	exec [dbo].[Nop_Forums_ForumUpdateCounts] @ForumID
	exec [dbo].[Nop_CustomerUpdateCounts] @UserID
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 15)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name])
	VALUES (15, N'N Times Only')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountLimitation]
		WHERE [DiscountLimitationID] = 25)
BEGIN
	INSERT [dbo].[Nop_DiscountLimitation] ([DiscountLimitationID], [Name])
	VALUES (25, N'N Times Per Customer')
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='LimitationTimes')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [LimitationTimes] int NOT NULL CONSTRAINT [DF_Nop_Discount_LimitationTimes] DEFAULT ((1))
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@RelatedToProductID	int = 0,
	@Keywords			nvarchar(MAX),
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	SET @PriceMin = isnull(@PriceMin, 0)
	SET @PriceMax = isnull(@PriceMax, 2147483644)
	
	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
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
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL,
		[Name] nvarchar(400) not null,
		[Price] money not null,
		[DisplayOrder1] int,
		[DisplayOrder2] int,
		[DisplayOrder3] int,
		[CreatedOn] datetime
	)

	INSERT INTO #DisplayOrderTmp ([ProductID], [Name], [Price], [DisplayOrder1], [DisplayOrder2], [DisplayOrder3], [CreatedOn])
	SELECT p.ProductID, p.Name, pv.Price, pcm.DisplayOrder, pmm.DisplayOrder, rp.DisplayOrder, p.CreatedOn
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_RelatedProduct rp with (NOLOCK) ON p.ProductID=rp.ProductID2
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
			(
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
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				@RelatedToProductID IS NULL OR @RelatedToProductID=0
				OR rp.ProductID1=@RelatedToProductID
			)
		AND (
				pv.Price BETWEEN @PriceMin AND @PriceMax
			)
		AND	(
				-- search standard content
				patindex(@Keywords, isnull(p.name, '')) > 0
				or patindex(@Keywords, isnull(pv.name, '')) > 0
				or patindex(@Keywords, isnull(pv.sku , '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(p.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pv.Description, '')) > 0)					
				-- search language content
				or patindex(@Keywords, isnull(pl.name, '')) > 0
				or patindex(@Keywords, isnull(pvl.name, '')) > 0
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.ShortDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pl.FullDescription, '')) > 0)
				or (@SearchDescriptions = 1 and patindex(@Keywords, isnull(pvl.Description, '')) > 0)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
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
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @RelatedToProductID IS NOT NULL AND @RelatedToProductID > 0
		THEN rp.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOn END DESC

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #FilteredSpecs
	DROP TABLE #DisplayOrderTmp
	DROP TABLE #PageIndex
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Products.DisplayCartAfterAddingProduct')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Products.DisplayCartAfterAddingProduct', N'True', N'')
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_BlogPostLoadAll]
(
	@LanguageID	int,
	@DateFrom			datetime,
	@DateTo				datetime,
	@PageSize			int = 2147483644,
	@PageIndex			int = 0, 
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		BlogPostID int NOT NULL,
	)

	INSERT INTO #PageIndex (BlogPostID)
	SELECT
		bp.BlogPostID
	FROM 
	    Nop_BlogPost bp 
	WITH 
		(NOLOCK)
	WHERE
		(@LanguageID IS NULL OR @LanguageID = 0 OR bp.LanguageID = @LanguageID)
		AND
		(@DateFrom is NULL or @DateFrom <= bp.CreatedOn)
		AND
		(@DateTo is NULL or @DateTo >= bp.CreatedOn)
	ORDER BY 
		bp.CreatedOn 
	DESC


	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		bp.[BlogPostId],
		bp.[LanguageId],
		bp.[BlogPostTitle],
		bp.[BlogPostBody],
		bp.[BlogPostAllowComments],
		bp.[Tags],
		bp.[CreatedById],
		bp.[CreatedOn]
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_BlogPost bp on bp.BlogPostID = [pi].BlogPostID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 60)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (60, N'Billing country is')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 70)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (70, N'Shipping country is')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='RequirementBillingCountryIs')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [RequirementBillingCountryIs] int NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementBillingCountryIs] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Discount]') and NAME='RequirementShippingCountryIs')
BEGIN
	ALTER TABLE [dbo].[Nop_Discount] 
	ADD [RequirementShippingCountryIs] int NOT NULL CONSTRAINT [DF_Nop_Discount_RequirementShippingCountryIs] DEFAULT ((0))
END
GO

--Cross-sells
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_CrossSellProduct]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_CrossSellProduct](
	[CrossSellProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId1] [int] NOT NULL,
	[ProductId2] [int] NOT NULL,
 CONSTRAINT [PK_CrossSellProduct] PRIMARY KEY CLUSTERED 
(
	[CrossSellProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_CrossSellProduct_Nop_Product'
           AND parent_obj = Object_id('Nop_CrossSellProduct')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_CrossSellProduct
DROP CONSTRAINT FK_Nop_CrossSellProduct_Nop_Product
GO
ALTER TABLE [dbo].[Nop_CrossSellProduct]  WITH CHECK ADD  CONSTRAINT [FK_Nop_CrossSellProduct_Nop_Product] FOREIGN KEY([ProductId1])
REFERENCES [dbo].[Nop_Product] ([ProductId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'IX_Nop_CrossSellProduct_Unique'
           AND parent_obj = Object_id('Nop_CrossSellProduct')
           AND Objectproperty(id,N'IsConstraint') = 1)
ALTER TABLE dbo.Nop_CrossSellProduct
DROP CONSTRAINT IX_Nop_CrossSellProduct_Unique
GO
ALTER TABLE [dbo].[Nop_CrossSellProduct]  WITH CHECK ADD CONSTRAINT [IX_Nop_CrossSellProduct_Unique]  UNIQUE NONCLUSTERED
(
	[ProductId1] ASC,
	[ProductId2] ASC
)
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='IncludeInSitemap')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD [IncludeInSitemap] bit NOT NULL CONSTRAINT [DF_Nop_Topic_IncludeInSitemap] DEFAULT ((0))
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Sitemap.IncludeTopics' 
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'SEO.Sitemaps.IncludeTopics'
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Customer]') and NAME='DateOfBirth')
BEGIN
	ALTER TABLE [dbo].[Nop_Customer] 
	ADD [DateOfBirth] datetime
END
GO


CREATE TABLE #DateOfBirthImportTmp
	(
		[CustomerID] [int] NOT NULL,
		[DateOfBirthXml] xml NOT NULL
	)



INSERT INTO #DateOfBirthImportTmp (CustomerId, DateOfBirthXml)
SELECT	ca.CustomerId, ca.[Value]
FROM	Nop_CustomerAttribute ca
WHERE	ca.[Key] = N'DateOfBirth'

DECLARE @CustomerID int
DECLARE @DateOfBirthXml xml
DECLARE cur_attributes CURSOR FOR
SELECT CustomerID, DateOfBirthXml
FROM #DateOfBirthImportTmp
OPEN cur_attributes
FETCH NEXT FROM cur_attributes INTO @CustomerID, @DateOfBirthXml
WHILE @@FETCH_STATUS = 0
BEGIN

BEGIN TRY
--limit to 10 chars (datetime only)
UPDATE Nop_Customer
SET [DateOfBirth] = CAST(@DateOfBirthXml.value('dateTime[1]', 'nvarchar(10)') as datetime)
WHERE CustomerID = @CustomerID
END TRY
BEGIN CATCH
SELECT ERROR_MESSAGE()
END CATCH

FETCH NEXT FROM cur_attributes INTO @CustomerID, @DateOfBirthXml
END
CLOSE cur_attributes
DEALLOCATE cur_attributes

DROP TABLE #DateOfBirthImportTmp
GO

DELETE FROM Nop_CustomerAttribute
WHERE [Key] = N'DateOfBirth'
GO




IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerLoadAll]
GO
CREATE PROCEDURE [dbo].[Nop_CustomerLoadAll]
(
	@StartTime				datetime = NULL,
	@EndTime				datetime = NULL,
	@Email					nvarchar(200),
	@Username				nvarchar(200),
	@DontLoadGuestCustomers	bit = 0,
	@DateOfBirthMonth		int = 0,
	@DateOfBirthDay			int = 0,
	@PageSize				int = 2147483644,
	@PageIndex				int = 0, 
	@TotalRecords			int = null OUTPUT
)
AS
BEGIN

	SET @Email = isnull(@Email, '')
	SET @Email = '%' + rtrim(ltrim(@Email)) + '%'

	SET @Username = isnull(@Username, '')
	SET @Username = '%' + rtrim(ltrim(@Username)) + '%'


	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	DECLARE @TotalThreads int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		CustomerID int NOT NULL,
		RegistrationDate datetime NOT NULL,
	)

	INSERT INTO #PageIndex (CustomerID, RegistrationDate)
	SELECT DISTINCT
		c.CustomerID, c.RegistrationDate
	FROM [Nop_Customer] c with (NOLOCK)
	WHERE 
		(@StartTime is NULL or @StartTime <= c.RegistrationDate) and
		(@EndTime is NULL or @EndTime >= c.RegistrationDate) and 
		(patindex(@Email, c.Email) > 0) and
		(patindex(@Username, c.Username) > 0) and
		(@DontLoadGuestCustomers = 0 or c.IsGuest = 0) and 
		(@DateOfBirthMonth = 0 or (c.DateOfBirth is not null and DATEPART(month, c.DateOfBirth) = @DateOfBirthMonth)) and 
		(@DateOfBirthDay = 0 or (c.DateOfBirth is not null and DATEPART(day, c.DateOfBirth) = @DateOfBirthDay)) and 
		c.deleted=0
	order by c.RegistrationDate desc 

	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	SELECT  
		c.CustomerId,
		c.CustomerGuid,
		c.Email,
		c.Username,
		c.PasswordHash,
		c.SaltKey,
		c.AffiliateId,
		c.BillingAddressId,
		c.ShippingAddressId,
		c.LastPaymentMethodId,
		c.LastAppliedCouponCode,
		c.GiftCardCouponCodes,
		c.CheckoutAttributes,
		c.LanguageId,
		c.CurrencyId,
		c.TaxDisplayTypeId,
		c.IsTaxExempt,
		c.IsAdmin,
		c.IsGuest,
		c.IsForumModerator,
		c.TotalForumPosts,
		c.Signature,
		c.AdminComment,
		c.Active,
		c.Deleted,
		c.RegistrationDate,
		c.TimeZoneId,
		c.AvatarId,
		c.DateOfBirth
	FROM
		#PageIndex [pi]
		INNER JOIN [Nop_Customer] c on c.CustomerID = [pi].CustomerID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
	
END
GO

--Multiple email senders
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_EmailAccount]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_EmailAccount](
	[EmailAccountId] int IDENTITY(1,1) NOT NULL,
	[Email] nvarchar(255) NOT NULL,
	[DisplayName] nvarchar(255) NOT NULL,
	[Host] nvarchar(255) NOT NULL,
	[Port] int NOT NULL,
	[Username] nvarchar(255) NOT NULL,
	[Password] nvarchar(255) NOT NULL,
	[EnableSSL] bit NOT NULL,
	[UseDefaultCredentials] bit NOT NULL,
 CONSTRAINT [PK_EmailAccount] PRIMARY KEY CLUSTERED 
(
	[EmailAccountId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO

--migrate default email accont details
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_EmailAccount])
BEGIN

declare @Email nvarchar(255)
declare @DisplayName nvarchar(255)
declare @Host nvarchar(255)
declare @Port int
declare @Username nvarchar(255)
declare @Password nvarchar(255)


SELECT @Email = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailAddress'
SELECT @DisplayName = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailDisplayName'
SELECT @Host = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailHost'
SELECT @Port = isnull([Value], 25) FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPort'
SELECT @Username = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailUser'
SELECT @Password = isnull([Value], N'') FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPassword'

INSERT INTO [Nop_EmailAccount]([Email], [DisplayName], [Host], [Port], [Username], [Password], [EnableSSL], [UseDefaultCredentials])
VALUES (isnull(@Email, N''), isnull(@DisplayName, N''), isnull(@Host, N''), isnull(@Port, 25), isnull(@Username, N''), isnull(@Password, N''), 0, 0)

DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailAddress'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailDisplayName'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailHost'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPort'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailUser'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailPassword'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailUseDefaultCredentials'
DELETE FROM Nop_Setting WHERE [Name]=N'Email.AdminEmailEnableSsl'

END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_MessageTemplateLocalized]') and NAME='EmailAccountId')
BEGIN
	ALTER TABLE [dbo].[Nop_MessageTemplateLocalized] 
	ADD [EmailAccountId] int NOT NULL CONSTRAINT [DF_Nop_MessageTemplateLocalized_EmailAccountId] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_QueuedEmail]') and NAME='EmailAccountId')
BEGIN
	ALTER TABLE [dbo].[Nop_QueuedEmail] 
	ADD [EmailAccountId] int NOT NULL CONSTRAINT [DF_Nop_QueuedEmail_EmailAccountId] DEFAULT ((0))
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SEONames.ConvertNonWesternChars')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SEONames.ConvertNonWesternChars', N'True', N'')
END
GO
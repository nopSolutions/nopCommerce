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

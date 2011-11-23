--upgrade scripts from nopCommerce 2.30 to nopCommerce 2.40

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreDiscounts.Hint">
    <Value>Check to ignore discounts (sitewide). It can significantly improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.IgnoreFeaturedProducts.Hint">
    <Value>Check to ignore featured products (sitewide). It can significantly improve performance.</Value>
  </LocaleResource>
  <LocaleResource Name="Messages.Order.Products(s).Download">
    <Value>Download</Value>
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



--Update stored procedure according to new special price properties
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
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
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
		CASE WHEN @OrderBy = 0 AND @CategoryId IS NOT NULL AND @CategoryId > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerId IS NOT NULL AND @ManufacturerId > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 5
		--THEN dbo.[nop_getnotnullnotempty](pl.[Name],p.[Name]) END ASC,
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
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
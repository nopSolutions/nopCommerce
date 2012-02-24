CREATE FUNCTION [dbo].[nop_splitstring_to_table]
(
    @string NVARCHAR(1000),
    @delimiter CHAR(1)
)
RETURNS @output TABLE(
    data NVARCHAR(256)
)
BEGIN
    DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string)

    WHILE @start < LEN(@string) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@string) + 1

        INSERT INTO @output (data) 
        VALUES(SUBSTRING(@string, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END
    RETURN
END
GO



CREATE FUNCTION [dbo].[nop_getnotnullnotempty]
(
    @p1 nvarchar(max) = null, 
    @p2 nvarchar(max) = null
)
RETURNS nvarchar(max)
AS
BEGIN
    IF @p1 IS NULL
        return @p2
    IF @p1 =''
        return @p2

    return @p1
END
GO



CREATE PROCEDURE [dbo].[ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(300) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
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

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM dbo.[nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

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
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
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
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
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
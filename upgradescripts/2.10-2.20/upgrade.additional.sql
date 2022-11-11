--additional upgrade scripts from nopCommerce 2.10 to nopCommerce 2.20
--Execure it if your database supports stored procedures and functions. Note that SQL Compact doesn't support them.


--SEO friendly name (products).
CREATE FUNCTION dbo.RemoveSpecialSeoChars (@s varchar(1024)) RETURNS varchar(1024)
   WITH SCHEMABINDING
BEGIN
   IF @s is null
      RETURN null
   DECLARE @s2 varchar(1024)
   SET @s2 = ''
   DECLARE @l int
   SET @l = len(@s)
   DECLARE @p int
   SET @p = 1
   while @p <= @l BEGIN
      DECLARE @c int
      SET @c = ascii(substring(@s, @p, 1))
      IF @c between 48 and 57 or @c between 65 and 90 or @c between 97 and 122 or @c = 45
         SET @s2 = @s2 + char(@c)
      SET @p = @p + 1
      END
   IF len(@s2) = 0
      RETURN null
   RETURN @s2
   END
GO
  
DECLARE cur CURSOR
FOR SELECT 
  Product_Picture_Mapping.PictureId, 
  CASE WHEN Product.SeName = '' OR Product.SeName IS NULL THEN Product.Name ELSE Product.SeName END
FROM
  Product_Picture_Mapping INNER JOIN
  Product ON Product_Picture_Mapping.ProductId = Product.Id

DECLARE @name nvarchar(300);
DECLARE @id int;
OPEN cur

FETCH NEXT FROM cur INTO @id, @name
WHILE (@@fetch_status <> -1)
BEGIN
  IF (@@fetch_status <> -2)
  BEGIN
    DECLARE @seourl nvarchar(300)
    --Removes diacritics
    SET @seourl =
        REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        REPLACE(REPLACE(@name COLLATE Latin1_General_CI_AI
        ,'a','a'),'b','b'),'c','c'),'d','d'),'e','e'),'f', 'f'),'g','g'),'h','h'),'i','i'),'j','j'),'k','k'),'l', 'l')
        ,'m','m'),'n','n'),'o','o'),'p','p'),'q','q'),'r', 'r'),'s','s'),'t','t'),'u','u'),'v','v'),'w','w'),'x', 'x')
        ,'y','y'),'z','z')
    
    SET @seourl = REPLACE(@seourl, ' ', '-');--Replaces spaces with -
    SET @seourl = dbo.RemoveSpecialSeoChars(@seourl);--Removes special chars
    
    
    SET @seourl = REPLACE(@seourl, '-', '_');
    SET @seourl = REPLACE(@seourl, '__', '_');
    SET @seourl = REPLACE(@seourl, '__', '_');--Repeat to remove series like ____
    SET @seourl = REPLACE(@seourl, '__', '_');--Repeat to remove series like ____
    PRINT @seourl
    
    UPDATE dbo.Picture
    SET SeoFilename = @seourl
    WHERE Id = @id
  END
  FETCH NEXT FROM cur INTO @id, @name
END
CLOSE cur
DEALLOCATE cur
GO

DROP FUNCTION dbo.RemoveSpecialSeoChars
GO




--load products stored procedure
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[nop_splitstring_to_table]') AND [type] in (N'FN', N'IF', N'TF'))
DROP FUNCTION  [dbo].[nop_splitstring_to_table]
GO
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

IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[nop_getnotnullnotempty]') AND [type] in (N'FN', N'IF', N'TF'))
DROP FUNCTION  [dbo].[nop_getnotnullnotempty]
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



IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[dbo].[ProductLoadAllPaged]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
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
				@PriceMin IS NULL OR @PriceMin=0
				OR pv.Price >= @PriceMin	
			)
		AND (
				@PriceMax IS NULL OR @PriceMax=2147483644 -- max value
				OR pv.Price <= @PriceMax
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
		p.Id,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.ProductTemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SeName,
		p.AllowCustomerReviews,
		p.ApprovedRatingSum,
		p.NotApprovedRatingSum,
		p.ApprovedTotalReviews,
		p.NotApprovedTotalReviews,
		p.Published,
		p.Deleted,
		p.CreatedOnUtc,
		p.UpdatedOnUtc
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
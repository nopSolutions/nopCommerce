--upgrade scripts from nopCommerce 2.70 to nopCommerce 2.80

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Acl">
    <Value>ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AclCustomerRoles">
    <Value>Customer roles</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AclCustomerRoles.Hint">
    <Value>Select customer roles for which the manufacturers will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.SubjectToAcl">
    <Value>Subject to ACL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.SubjectToAcl.Hint">
    <Value>Determines whether the manufacturers is subject to ACL (access control list).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.OrderIdent">
    <Value>Order ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.OrderIdent.Hint">
    <Value>Set the order ID counter. This is useful if you want your orders to start at a certain number. This only affects orders created going forward. The value must be greater than the current maximum order ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AcceptPrivacyPolicyEnabled">
    <Value>''Accept privacy policy'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AcceptPrivacyPolicyEnabled.Hint">
    <Value>Ask customers to accept privacy policy during registration.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.AcceptPrivacyPolicy">
    <Value>I accept privacy policy</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.AcceptPrivacyPolicy.Read">
    <Value>(read)</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.AcceptPrivacyPolicy.Required">
    <Value>Please accept privacy policy</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue">
    <Value>Custom value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue.Hint">
    <Value>Enter custom value or leave empty. If entered, it''ll be used instead of the selected attribute option. Important note: ensure that ''Allow filtering'' is not enabled if custom value is entered.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.SeName">
    <Value>Search engine friendly page name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-news'' to make your page URL ''http://www.yourStore.com/the-best-news''. Leave empty to generate it automatically based on the title of the news.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.SeName">
    <Value>Search engine friendly page name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-blog-post'' to make your page URL ''http://www.yourStore.com/the-best-blog-post''. Leave empty to generate it automatically based on the title of the blog post.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Sku">
    <Value>Sku</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Sku.Hint">
    <Value>Product stock keeping unit (SKU). Your internal unique identifier that can be used to track this attribute combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber">
    <Value>Manufacturer part number</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.ManufacturerPartNumber.Hint">
    <Value>The manufacturer''s part number for this attribute combination.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Gtin">
    <Value>GTIN</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.Gtin.Hint">
    <Value>Enter global trade item number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames">
    <Value>Search engine friendly page names</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.DeleteSelected">
    <Value>Delete selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Name.Hint">
    <Value>A name to find.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.EntityId">
    <Value>Entity ID</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.EntityName">
    <Value>Entity name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Language">
    <Value>Language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Language.Standard">
    <Value>Standard</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy">
    <Value>Copy product variant</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Name">
    <Value>New product variant name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Name.Hint">
    <Value>The name of the new product variant.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Published">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Copy.Published.Hint">
    <Value>Check to mark a product variant as published.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Catalog.AttributeControlType.ColorSquares">
    <Value>Color squares</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb">
    <Value>RGB color</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb.Hint">
    <Value>Choose color to be used with the color squares attribute control.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.ColorSquaresRgb">
    <Value>RGB color</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.ColorSquaresRgb.Hint">
    <Value>Choose color to be used with the color squares attribute control.</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Title">
    <Value>Newsletter</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Button">
    <Value>Subscribe</Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Unsubscribe">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.UnsubscribeEmailSent">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Email">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Newsletter.Subscribe">
    <Value>Sign up for our newsletter</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.CustomerNameFormat.ShowFirstName">
    <Value>Show first name</Value>
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




--Full-text issue fixed
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
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
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


		--product variant name
		SET @sql = @sql + '
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(pv.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Name]) > 0 '


		--SKU
		SET @sql = @sql + '
		UNION
		SELECT pv.ProductId
		FROM ProductVariant pv with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(pv.[Sku], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Sku]) > 0 '


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


			--product variant description
			SET @sql = @sql + '
			UNION
			SELECT pv.ProductId
			FROM ProductVariant pv with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(pv.[Description], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, pv.[Description]) > 0 '


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
	
	--show hidden
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
	
	--show hidden and ACL
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
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
					FROM Product_SpecificationAttribute_Mapping psam
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
	DROP TABLE #FilteredCustomerRoleIds

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
		FROM [Product_SpecificationAttribute_Mapping] [psam]
		WHERE [psam].[AllowFiltering] = 1
		AND [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi])

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(1000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

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


--URL records enhancements
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[UrlRecord]') and NAME='IsActive')
BEGIN
	ALTER TABLE [UrlRecord]
	ADD [IsActive] bit NULL
END
GO

UPDATE [UrlRecord]
SET [IsActive] = 1
WHERE [IsActive] IS NULL
GO

ALTER TABLE [UrlRecord] ALTER COLUMN [IsActive] bit NOT NULL
GO

--ACL on manufactures
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Manufacturer]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [Manufacturer]
	ADD [SubjectToAcl] bit NULL
END
GO

UPDATE [Manufacturer]
SET [SubjectToAcl] = 0
WHERE [SubjectToAcl] IS NULL
GO

ALTER TABLE [Manufacturer] ALTER COLUMN [SubjectToAcl] bit NOT NULL
GO


--privacy policy checkbox
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.acceptprivacypolicyenabled')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'customersettings.acceptprivacypolicyenabled', N'false')
END
GO


--custom specification values
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product_SpecificationAttribute_Mapping]') and NAME='CustomValue')
BEGIN
	ALTER TABLE [Product_SpecificationAttribute_Mapping]
	ADD [CustomValue] nvarchar(4000) NULL
END
GO



IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[temp_generate_sename]
GO
CREATE PROCEDURE [dbo].[temp_generate_sename]
(
    @current_sename nvarchar(1000),
    @entity_id int,
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
	--get current name
	DECLARE @sql nvarchar(4000)
	    
    --generate se name    
	DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars varchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@current_sename)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
		DECLARE @c nvarchar(1)
        SET @c = substring(@current_sename, @p, 1)
        IF CHARINDEX(@c,@allowed_se_chars) > 0
        BEGIN
			SET @new_sename = @new_sename + @c
		END
		SET @p = @p + 1
	END
	--replace spaces with '-'
	SELECT @new_sename = REPLACE(@new_sename,' ','-');
    WHILE CHARINDEX('--',@new_sename) > 0
		SELECT @new_sename = REPLACE(@new_sename,'--','-');
    WHILE CHARINDEX('__',@new_sename) > 0
		SELECT @new_sename = REPLACE(@new_sename,'__','_');
    --ensure not empty
    IF (@new_sename is null or @new_sename = '')
		SELECT @new_sename = ISNULL(CAST(@entity_id AS nvarchar(max)), '0');
    --lowercase
	SELECT @new_sename = LOWER(@new_sename)
	--ensure this sename is not reserved
	WHILE (1=1)
	BEGIN
		DECLARE @sename_is_already_reserved bit
		SET @sename_is_already_reserved = 0
		SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename AND [EntityId] <> ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0') + ')
					BEGIN
						SELECT @sename_is_already_reserved = 1
					END'
		EXEC sp_executesql @sql,N'@sename nvarchar(1000), @sename_is_already_reserved nvarchar(4000) OUTPUT',@new_sename,@sename_is_already_reserved OUTPUT
		
		IF (@sename_is_already_reserved > 0)
		BEGIN
			--add some digit to the end in this case
			SET @new_sename = @new_sename + '-1'
		END
		ELSE
		BEGIN
			BREAK
		END
	END
	
	--return
    SET @result = @new_sename
END
GO

--sename for news
DECLARE @sename_existing_news_id int
DECLARE cur_sename_existing_news CURSOR FOR
SELECT [Id]
FROM [News]
OPEN cur_sename_existing_news
FETCH NEXT FROM cur_sename_existing_news INTO @sename_existing_news_id
WHILE @@FETCH_STATUS = 0
BEGIN		
	DECLARE @original_newsitem_title nvarchar(1000)
	SELECT @original_newsitem_title = [Title] FROM [News] WHERE [Id] = @sename_existing_news_id
	DECLARE @language_id int
	SELECT @language_id = [LanguageId] FROM [News] WHERE [Id] = @sename_existing_news_id
	
	DECLARE @sename nvarchar(1000)	
	SET @sename = null -- clear cache (variable scope)
	
	EXEC	[dbo].[temp_generate_sename]
			@current_sename = @original_newsitem_title,
			@entity_id = @sename_existing_news_id,
			@result = @sename OUTPUT
	
	DECLARE @sql nvarchar(4000)
	--insert
	SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''NewsItem'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_news_id AS nvarchar(max)), '0') + ')
	BEGIN
		--update
		UPDATE [UrlRecord]
		SET [Slug] = @sename
		WHERE [EntityName]=''NewsItem'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_news_id AS nvarchar(max)), '0') + '
	END
	ELSE
	BEGIN
		--insert
		INSERT INTO [UrlRecord] ([EntityId], [EntityName], [IsActive],[Slug], [LanguageId])
		VALUES (' + ISNULL(CAST(@sename_existing_news_id AS nvarchar(max)), '0') +',''NewsItem'',1, @sename, ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0')+ ')
	END
	'				
	EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
	
	--fetch next identifier
	FETCH NEXT FROM cur_sename_existing_news INTO @sename_existing_news_id
END
CLOSE cur_sename_existing_news
DEALLOCATE cur_sename_existing_news	
GO

--sename for blog posts
DECLARE @sename_existing_blogpost_id int
DECLARE cur_sename_existing_blogpost CURSOR FOR
SELECT [Id]
FROM [BlogPost]
OPEN cur_sename_existing_blogpost
FETCH NEXT FROM cur_sename_existing_blogpost INTO @sename_existing_blogpost_id
WHILE @@FETCH_STATUS = 0
BEGIN		
	DECLARE @original_blogpost_title nvarchar(1000)
	SELECT @original_blogpost_title = [Title] FROM [BlogPost] WHERE [Id] = @sename_existing_blogpost_id
	DECLARE @language_id int
	SELECT @language_id = [LanguageId] FROM [BlogPost] WHERE [Id] = @sename_existing_blogpost_id
	
	DECLARE @sename nvarchar(1000)	
	SET @sename = null -- clear cache (variable scope)
	
	EXEC	[dbo].[temp_generate_sename]
			@current_sename = @original_blogpost_title,
			@entity_id = @sename_existing_blogpost_id,
			@result = @sename OUTPUT
	
	DECLARE @sql nvarchar(4000)
	--insert
	SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''BlogPost'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_blogpost_id AS nvarchar(max)), '0') + ')
	BEGIN
		--update
		UPDATE [UrlRecord]
		SET [Slug] = @sename
		WHERE [EntityName]=''BlogPost'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_blogpost_id AS nvarchar(max)), '0') + '
	END
	ELSE
	BEGIN
		--insert
		INSERT INTO [UrlRecord] ([EntityId], [EntityName], [IsActive],[Slug], [LanguageId])
		VALUES (' + ISNULL(CAST(@sename_existing_blogpost_id AS nvarchar(max)), '0') +',''BlogPost'',1, @sename, ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0')+ ')
	END
	'				
	EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
	
	--fetch next identifier
	FETCH NEXT FROM cur_sename_existing_blogpost INTO @sename_existing_blogpost_id
END
CLOSE cur_sename_existing_blogpost
DEALLOCATE cur_sename_existing_blogpost	
GO


--drop temporary procedures & functions
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_generate_sename]
GO

--new attribute combination properties
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeCombination]') and NAME='Sku')
BEGIN
	ALTER TABLE [ProductVariantAttributeCombination]
	ADD [Sku] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeCombination]') and NAME='ManufacturerPartNumber')
BEGIN
	ALTER TABLE [ProductVariantAttributeCombination]
	ADD [ManufacturerPartNumber] nvarchar(400) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeCombination]') and NAME='Gtin')
BEGIN
	ALTER TABLE [ProductVariantAttributeCombination]
	ADD [Gtin] nvarchar(400) NULL
END
GO


--display currency label?
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'currencysettings.displaycurrencylabel')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'currencysettings.displaycurrencylabel', N'false')
END
GO


--color squares attribute type
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductVariantAttributeValue]') and NAME='ColorSquaresRgb')
BEGIN
	ALTER TABLE [ProductVariantAttributeValue]
	ADD [ColorSquaresRgb] nvarchar(100) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttributeValue]') and NAME='ColorSquaresRgb')
BEGIN
	ALTER TABLE [CheckoutAttributeValue]
	ADD [ColorSquaresRgb] nvarchar(100) NULL
END
GO

ALTER TABLE [ProductVariantAttributeValue] ALTER COLUMN [Name] nvarchar(400) NOT NULL
GO

--delete obosolete settings
DELETE FROM [Setting]
WHERE [Name] = N'customersettings.acceptprivacypolicyenabled'
GO

--obslete message template
DELETE FROM [MessageTemplate]
WHERE [Name]=N'NewsLetterSubscription.DeactivationMessage'
GO
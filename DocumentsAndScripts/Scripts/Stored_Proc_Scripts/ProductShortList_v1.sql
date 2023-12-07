USE [onjobsupport]
GO

/****** Object:  StoredProcedure [dbo].[ProductShortList_v1]    Script Date: 12/7/2023 4:24:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- I short Listed profiles
-- exec [ProductShortList_v1] NULL, 2, 263 ,0,0,100

-- product id : 133 for customer id:61
-- The below should give who short listed me (product id)
-- exec [ProductShortList_v1] 133,2,61,0,0,100

CREATE PROCEDURE [dbo].[ProductShortList_v1]
(
	@ProductIds			nvarchar(MAX) = null,--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ShoppingCartTypeId	int = 0, -- shortlist = 2, ShortListedMe =3, InterestSent = 4, InterestReceived = 5,
	@CustomerId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN	
	
	DECLARE
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by Product IDs
	SET @ProductIds = isnull(@ProductIds, '')	
	CREATE TABLE #FilteredProductIds
	(
		ProductId int not null
	)
	INSERT INTO #FilteredProductIds (ProductId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@ProductIds, ',')	
	DECLARE @ProductIdsCount int	
	SET @ProductIdsCount = (SELECT COUNT(1) FROM #FilteredProductIds)

	CREATE TABLE #Products 
	(
		[ProductId] int NOT NULL
	)

	SET @sql = '
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'

	-- filter products by customerid
	IF @CustomerId > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN ShoppingCartItem sci with (NOLOCK)
				ON p.Id = sci.ProductId
		WHERE CustomerId='+  CAST(@CustomerId AS nvarchar(max))
	END

	-- filter products by shopping cart typeid
	IF @ShoppingCartTypeId > 0
	BEGIN
		SET @sql = @sql + '
		AND ShoppingCartTypeId='+ CAST(@ShoppingCartTypeId AS nvarchar(max))
	END
	
	-- get published products
	SET @sql = @sql + '
		AND p.Deleted = 0'

   -- I short Listed profiles
   -- exec [ProductShortList_v2] NULL,2,61,0,0,1000

	--filter by Product ids
	IF @ProductIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND p.Id IN ('
		SET @sql = @sql + + CAST(@ProductIds AS nvarchar(max))
		SET @sql = @sql + ')'
	END
	
	
    SET @sql = '
    INSERT INTO #Products ([ProductId])' + @sql

	-- PRINT (@sql)
	EXEC sp_executesql @sql
	
	--SELECT '#Products',* FROM #Products;

	DROP TABLE #FilteredProductIds

	CREATE TABLE #ProductSpecs 
	(
		[ProductId] int NOT NULL,
		[PrimaryTechnology] varchar(1000) NULL,
		[SecondaryTechnology] varchar(1000) NULL,
		[CurrentAvalibility] varchar(1000) NULL
	)

	SELECT 
		p.ProductId as Id,
		sa.Name,
		Technology = STRING_AGG(sao.Name,' , ')
	INTO #ProductSpecsTemp
		FROM [dbo].[Product_SpecificationAttribute_Mapping] PS
		JOIN #Products p on p.ProductId=PS.ProductId
		--JOIN [Product] p on p.Id=PS.ProductId
		JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
		JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
		GROUP BY p.ProductId,sa.name

	--SELECT * FROM #ProductSpecsTemp

	-- I short Listed profiles
	-- exec [ProductShortList_v1] NULL,2,263,0,0,1000

 INSERT INTO #ProductSpecs ([ProductId],[PrimaryTechnology],[SecondaryTechnology],[CurrentAvalibility])
	SELECT
		[PS].*
	FROM
		#ProductSpecsTemp 
		PIVOT (Max([Technology]) FOR [Name]
		IN (PrimaryTechnology,SecondaryTechnology,CurrentAvalibility)) 
	AS [PS] ;

	 SELECT '#ProductSpecs',* FROM #ProductSpecs

	SELECT 
		 G.[Key]
		,G.[Value]
		,P.* 
		,C.LastLoginDateUtc
		,C.LastActivityDateUtc
	  INTO #CustomerTemp
	FROM 
	--Product p with (NOLOCK)
	#ProductSpecs P with (NOLOCK)
	LEFT JOIN Customer C with (NOLOCK) on P.ProductId = [C].[VendorId]
	LEFT JOIN  [GenericAttribute] G on G.EntityId= C.Id
	WHERE G.[KEY] in ('FirstName','LastName','Phone','Gender','Company',
					  'CountryId','StateProvinceId','LanguageId','TimeZoneId',
					  'AvatarPictureId','CustomerProfileTypeId','City')

     SELECT '#CustomerTemp',* FROM #CustomerTemp
	 -- exec [ProductShortList_v1] NULL,2,263,0,0,1000


	 -- create #CustomerOrderTemp table
	SELECT  Distinct(P.Id) as ProductId,
		CAST(CASE WHEN OI.ProductId=16 AND O.PaidDateUtc >= GETUTCDATE()-90 THEN 1
				  WHEN OI.ProductId=17 AND O.PaidDateUtc >= GETUTCDATE()-180 THEN 1
				  WHEN OI.ProductId=18 AND O.PaidDateUtc >= GETUTCDATE()-365 THEN 1
				  ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #CustomerOrderTemp
		FROM [Order] O 
		INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
		INNER JOIN [Product] P ON P.VendorId=O.CustomerId
		WHERE O.OrderStatusId=30 -- Order Paid


	SELECT 
		p.ProductId as Id,
		p.FirstName,
		p.LastName,
		p.Phone,
		p.Gender,
		p.Company,
		p.CountryId,
		p.StateProvinceId,
		p.City,
		p.LanguageId,
		p.TimeZoneId,
		p.AvatarPictureId,
		p.CustomerProfileTypeId,
		ps.PrimaryTechnology,
		ps.SecondaryTechnology,
		ps.CurrentAvalibility,
		--p.*
		--ps.*,
		C.Name AS Country,
		sp.Name AS StateProvince,
		la.Name AS Language,
		url.Slug AS Slug,
		P.LastLoginDateUtc,
		P.LastActivityDateUtc,
		CAST(CASE WHEN sci.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS ProfileShortListed, 
		CAST(CASE WHEN sc.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS InterestSent,
		CAST(CASE WHEN co.PremiumCustomer=1 THEN 1 ELSE 0 END AS BIT) AS PremiumCustomer,
		(Select VendorId FROM Product WHERE Id=p.ProductId) AS VendorId
	  INTO #FinalProductsTemp
	FROM
		#CustomerTemp AB
		PIVOT (Max([Value]) FOR [Key]
		IN (FirstName,LastName,Phone,Gender,Company,
			CountryId, StateProvinceId,LanguageId,
			TimeZoneId,AvatarPictureId,CustomerProfileTypeId,City)) 
	AS P
	LEFT JOIN #ProductSpecs ps on ps.ProductId=p.ProductId
	LEFT JOIN [Country] C on C.Id=p.CountryId
	LEFT JOIN [StateProvince] sp on sp.Id=p.StateProvinceId
	LEFT JOIN [Language] la on la.Id=p.LanguageId
	LEFT JOIN [UrlRecord] url on url.EntityId=p.ProductId AND url.EntityName='Product'
	LEFT JOIN [ShoppingCartItem] sci on sci.ProductId=p.ProductId
									AND sci.ShoppingCartTypeId=2 -- Is Profile ShortListed?
								    AND sci.CustomerId =@CustomerId
	LEFT JOIN [ShoppingCartItem] sc on sc.ProductId=p.ProductId  -- Is Interest Sent?
									AND sc.ShoppingCartTypeId=4
								    AND sc.CustomerId =@CustomerId
	LEFT JOIN #CustomerOrderTemp co	on co.ProductId	= ps.ProductId

   -- WHERE CAST(P.CustomerProfileTypeId as INT)= @WarehouseId;
    --WHERE sc.CustomerId=@CustomerId


	-- I short Listed profiles
	-- exec [ProductShortList_v2] null,2,61,0,0,100

	-- SELECT * FROM #FinalProductsTemp

	--return products
	SELECT
		p.*
	FROM
		#FinalProductsTemp p with (NOLOCK)

	DROP TABLE #ProductSpecs
	DROP TABLE #ProductSpecsTemp
	DROP TABLE #FinalProductsTemp
END


GO



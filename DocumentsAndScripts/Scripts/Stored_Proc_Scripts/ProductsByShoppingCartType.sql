USE [onjobsupport]
GO

/****** Object:  StoredProcedure [dbo].[ProductsByShoppingCartType]    Script Date: 12/7/2023 4:23:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- I short Listed profiles
-- exec [ProductShortList_v1] NULL, 2, 263 ,0,0,100

--who shortlisted me
 -- exec [ProductsByShoppingCarttype] 10,2,0,0,0,100

-- product id : 133 for customer id:61
-- The below should give who short listed me (product id)
-- exec [ProductShortList_v1] 133,2,61,0,0,100

CREATE PROCEDURE [dbo].[ProductsByShoppingCartType]
(
	@ProductId			nvarchar(MAX) = null,--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ShoppingCartTypeId	int = 0, -- shortlist = 2, ShortListedMe =3, InterestSent = 4, InterestReceived = 5,
	@CustomerId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN	
	
	SET @CustomerId=(SELECT Id From Customer Where VendorId=@ProductId)

	-- Get customer/product ids who shortlisted the product id 14

	--SELECT C.VendorId as ProductId
	--FROM [ShoppingCartItem] sci
	--INNER JOIN [Customer] C on C.Id=sci.CustomerId
	--WHERE 
	--	sci.ProductId=10
	--	AND sci.ShoppingCartTypeId=2

	 -- SELECT *FROM [dbo].[ShoppingCartItem] where CustomerId=263 and ProductId=14

	CREATE TABLE #Products 
	(
		[ProductId] int NOT NULL
	)

	INSERT INTO #Products ([ProductId])
		SELECT C.VendorId as ProductId
		FROM [ShoppingCartItem] sci
		INNER JOIN [Customer] C on C.Id=sci.CustomerId
		WHERE sci.ProductId=@ProductId	
			  AND sci.ShoppingCartTypeId=@ShoppingCartTypeId

	-- SELECT '#Products', * FROM #Products

	CREATE TABLE #ProductSpecs 
	(
		[ProductId] int NOT NULL,
		[PrimaryTechnology] varchar(1000) NULL,
		[SecondaryTechnology] varchar(1000) NULL,
		[CurrentAvalibility] varchar(1000) NULL,
		[ProfileType] varchar(100) NULL
	)

	SELECT 
		p.id as ProductId,
		sa.Name,
		Technology = STRING_AGG(sao.Name,' , ')
	INTO #ProductSpecsTemp
		FROM [Product_SpecificationAttribute_Mapping] PS
		JOIN [Product] p on p.Id=PS.ProductId
		JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
		JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
		GROUP BY p.id,sa.name
		Order By p.id 

	--SELECT '##ProductSpecsTemp', * FROM #ProductSpecsTemp

 INSERT INTO #ProductSpecs ([ProductId],[PrimaryTechnology],[SecondaryTechnology],[CurrentAvalibility],[ProfileType])
	SELECT
		[PS].*
	FROM
		#ProductSpecsTemp 
		PIVOT (Max([Technology]) FOR [Name]
		IN (PrimaryTechnology,SecondaryTechnology,CurrentAvalibility,ProfileType)) 
	AS [PS] ;

	--SELECT '#ProductSpecs', * FROM #ProductSpecs

	SELECT 
		 G.[Key]
		,G.[Value]
		,P.* 
		,C.LastLoginDateUtc
		,C.LastActivityDateUtc
	INTO #CustomerTemp
		FROM #Products [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
		LEFT JOIN Customer C with (NOLOCK) on P.Id = [C].[VendorId]
		LEFT JOIN  [GenericAttribute] G on G.EntityId= C.Id
		WHERE G.[KEY] in ('FirstName','LastName','Phone','Gender','Company',
						  'CountryId','StateProvinceId','LanguageId','TimeZoneId',
						  'AvatarPictureId','CustomerProfileTypeId','City')
	
    --SELECT '#CustomerTemp', * FROM #CustomerTemp

	-- create #CustomerOrderTemp table
	SELECT  
		Distinct(C.VendorId) AS ProductId,
		CAST(CASE WHEN OI.ProductId=16 AND O.PaidDateUtc >= GETUTCDATE()-90 THEN 1
				  WHEN OI.ProductId=17 AND O.PaidDateUtc >= GETUTCDATE()-180 THEN 1
				  WHEN OI.ProductId=18 AND O.PaidDateUtc >= GETUTCDATE()-365 THEN 1
				  ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #CustomerOrderTemp
		FROM [Order] O 
		INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
		INNER JOIN [Product] P ON P.Id=OI.ProductId
		INNER JOIN [Customer] C ON C.Id=O.CustomerId
		WHERE O.OrderStatusId=30 -- Order Paid

   -- SELECT '#CustomerOrderTemp', * FROM #CustomerOrderTemp

	SELECT 
		p.*,
		ps.*,
		C.Name AS Country,
		sp.Name AS StateProvince,
		la.Name AS Language,
		url.Slug AS Slug,
		CAST(CASE WHEN sci.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS ProfileShortListed, 
		CAST(CASE WHEN sc.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS InterestSent,
		CAST(CASE WHEN co.PremiumCustomer=1 THEN 1 ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #FinalProducts
	FROM
		#CustomerTemp P
		PIVOT (Max([Value]) FOR [Key]
		IN (FirstName,LastName,Phone,Gender,Company,
			CountryId, StateProvinceId,LanguageId,
			TimeZoneId,AvatarPictureId,CustomerProfileTypeId,City)) 
	AS P
	LEFT JOIN #ProductSpecs ps on p.id=ps.ProductId
	LEFT JOIN [country] C on C.Id=p.CountryId
	LEFT JOIN [StateProvince] sp on sp.Id=p.StateProvinceId
	LEFT JOIN [Language] la on la.Id=p.LanguageId
	LEFT JOIN [UrlRecord] url on url.EntityId=p.Id AND url.EntityName='Product'
	LEFT JOIN [ShoppingCartItem] sci on sci.ProductId=p.Id 
									AND sci.ShoppingCartTypeId=2 -- shortlisted
									AND sci.CustomerId =@CustomerId
	LEFT JOIN [ShoppingCartItem] sc on sc.ProductId=p.Id  -- InterestSent
									AND sc.ShoppingCartTypeId=4
									AND sc.CustomerId =@CustomerId
    LEFT JOIN #CustomerOrderTemp co	on co.ProductId	= ps.ProductId						

	-- SELECT '#FinalProducts Table', * FROM #FinalProducts

	--return all products
	SELECT 
		P.*
	FROM
		#FinalProducts P

	DROP TABLE #Products
	DROP TABLE #ProductSpecs
	DROP TABLE #ProductSpecsTemp
	DROP TABLE #FinalProducts
	DROP TABLE #CustomerTemp
	DROP TABLE #CustomerOrderTemp
END


GO



USE [itjobsupport]
GO

/****** Object:  StoredProcedure [dbo].[ProductLoadAllPaged_V5]    Script Date: 11-08-2021 17:31:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--execution exampls
-- exec ProductLoadAllPaged_V5 @ProductIds=N'10,11,13',@VendorId=263,@WarehouseId=1
-- exec ProductLoadAllPaged_V5 @ProductIds=N'10,11,13',@VendorId=1402,@WarehouseId=1

-- update the "ProductLoadAllPaged" stored procedure
ALTER PROCEDURE [dbo].[ProductLoadAllPaged_V5]
(
	@ProductIds		nvarchar(MAX) = null, --a list of product IDs (comma-separated list). e.g. 1,2,3
	@VendorId		int = 0,
	@WarehouseId	int = 0
)
AS
BEGIN	

	DECLARE @ProfileType varchar(100);

	IF @WarehouseId=1
		BEGIN
			SET @ProfileType='GiveSupport';
		END;
	ELSE
		BEGIN
			SET @ProfileType='TakeSupport';
		END;

	CREATE TABLE #Products 
	(
		[ProductId] int NOT NULL
	)

	INSERT INTO #Products ([ProductId])
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@ProductIds, ',')	

	--SELECT '#PageIndex', * FROM #Products

	CREATE TABLE #ProductSpecs 
	(
		[ProductId] int NOT NULL,
		[PrimaryTechnology] varchar(1000) NULL,
		[SecondaryTechnology] varchar(1000) NULL,
		[CurrentAvalibility] varchar(1000) NULL,
		[ProfileType] varchar(100) NULL
		--[SpecificationAttributeOptionId] varchar(100) NULL
		
	)

	SELECT 
		p.id as ProductId,
		sa.Name,
		--sao.Id as SpecificationAttributeOptionId,
		Technology = STRING_AGG(sao.Name,' , ')
	INTO #ProductSpecsTemp
		FROM .[dbo].[Product_SpecificationAttribute_Mapping] PS
		JOIN Product p on p.Id=PS.ProductId
		JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
		JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
		--WHERE sao.Id=@WarehouseId
		GROUP BY p.id,sa.name,sao.Id;

	--SELECT '##ProductSpecsTemp', * FROM #ProductSpecsTemp

	-- exec ProductLoadAllPaged_V5 @ProductIds=N'10,11,13',@VendorId=1402,@WarehouseId=1

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
						'AvatarPictureId','CustomerProfileTypeId')
	
    --SELECT '#CustomerTemp', * FROM #CustomerTemp


	SELECT 
		p.*,
		ps.*,
		C.Name AS Country,
		sp.Name AS StateProvince,
		la.Name AS Language,
		url.Slug AS Slug,
		CAST(CASE WHEN sci.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS ProfileShortListed, 
		CAST(CASE WHEN sc.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS InterestSent
	INTO #FinalProducts
	FROM
		#CustomerTemp P
		PIVOT (Max([Value]) FOR [Key]
		IN (FirstName,LastName,Phone,Gender,Company,
			CountryId, StateProvinceId,LanguageId,
			TimeZoneId,AvatarPictureId,CustomerProfileTypeId)) 
	AS P
	LEFT JOIN #ProductSpecs ps on p.id=ps.ProductId
	LEFT JOIN [country] C on C.Id=p.CountryId
	LEFT JOIN [StateProvince] sp on sp.Id=p.StateProvinceId
	LEFT JOIN [Language] la on la.Id=p.LanguageId
	LEFT JOIN [UrlRecord] url on url.EntityId=p.Id AND url.EntityName='Product'
	LEFT JOIN [ShoppingCartItem] sci on sci.ProductId=p.Id 
									AND sci.ShoppingCartTypeId=2 -- shortlisted
									AND sci.CustomerId =@VendorId
	LEFT JOIN [ShoppingCartItem] sc on sc.ProductId=p.Id  -- InterestSent
									AND sc.ShoppingCartTypeId=4
									AND sc.CustomerId =@VendorId
    -- WHERE CAST(P.CustomerProfileTypeId as INT)= @WarehouseId;

	--SELECT '#FinalProducts Table', * FROM #FinalProducts

	-- exec ProductLoadAllPaged_V5 @ProductIds=N'10,11,13',@VendorId=1402,@WarehouseId=1

	--return all products
	SELECT 
		P.*
	FROM
		#FinalProducts P
	--WHERE P.ProfileType=@ProfileType

	DROP TABLE #Products
	DROP TABLE #ProductSpecs
	DROP TABLE #ProductSpecsTemp
	DROP TABLE #FinalProducts
	DROP TABLE #CustomerTemp

END


GO



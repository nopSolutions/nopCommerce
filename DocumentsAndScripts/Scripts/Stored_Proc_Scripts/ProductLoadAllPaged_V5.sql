USE [onjobsupport]
GO

/****** Object:  StoredProcedure [dbo].[ProductLoadAllPaged_V5]    Script Date: 12/7/2023 4:25:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--execution exampls
-- exec ProductLoadAllPaged_V5 @ProductIds=N'5',@CustomerId=263,@ProfileTypeId=2
-- exec ProductLoadAllPaged_V5 @ProductIds=N'5,',@CustomerId=263,@ProfileTypeId=1

-- update the "ProductLoadAllPaged" stored procedure
CREATE PROCEDURE [dbo].[ProductLoadAllPaged_V5]
(
	@ProductIds		nvarchar(MAX) = null, --a list of product IDs (comma-separated list). e.g. 1,2,3
	@CustomerId		int = 0, -- customer id
	@ProfileTypeId	int = 0
)
AS
BEGIN	

	DECLARE @ProfileType varchar(100);

	IF @ProfileTypeId=1
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
		[ProfileType] varchar(100) NULL,
		[MotherTongue] varchar(100) NULL,
		[WorkExperience] varchar(100) NULL
	)

	SELECT 
		p.Id as ProductId,
		sa.Name,
		--sao.Id as SpecificationAttributeOptionId,
		Technology = STRING_AGG(sao.Name,' , ')
	INTO #ProductSpecsTemp
		FROM [Product_SpecificationAttribute_Mapping] PS
		JOIN [Product] p on p.Id=PS.ProductId
		JOIN [SpecificationAttributeOption] sao on sao.id=ps.SpecificationAttributeOptionId
		JOIN [SpecificationAttribute] sa on sa.id=sao.SpecificationAttributeId
		GROUP BY p.id,sa.name
		Order By p.id 
		--,sao.Id;

	-- SELECT '##ProductSpecsTemp', * FROM #ProductSpecsTemp

	-- exec ProductLoadAllPaged_V5 @ProductIds=N'4,11,13,14,15,16,17,18',@CustomerId=263,@ProfileTypeId=2

 INSERT INTO #ProductSpecs ([ProductId],[PrimaryTechnology],[SecondaryTechnology],[CurrentAvalibility],[ProfileType],[MotherTongue],[WorkExperience])
	SELECT
		[PS].*
	FROM
		#ProductSpecsTemp 
		PIVOT (Max([Technology]) FOR [Name]
		IN ([Primary Technology],[Secondary Technology],[Current Avalibility],ProfileType,[Mother Tongue],[Relavent Experiance])) 
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

	-- create CustomerOrderTemp table
	SELECT  
		Distinct(C.VendorId) AS ProductId,
		CAST(CASE WHEN OI.ProductId=1 AND O.PaidDateUtc >= GETUTCDATE()-90  THEN 1	 -- 1 Month Subscription
				  WHEN OI.ProductId=2 AND O.PaidDateUtc >= GETUTCDATE()-180 THEN 1	 -- 6 Month Subscription
				  WHEN OI.ProductId=3 AND O.PaidDateUtc >= GETUTCDATE()-365 THEN 1	 -- 1 Year subscription
				  ELSE 0 END AS BIT) AS PremiumCustomer
	INTO #CustomerOrderTemp
		FROM [Order] O 
		INNER JOIN [OrderItem] OI ON OI.OrderId=O.Id -- AND O.OrderStatusId=30 
		INNER JOIN [Product] P ON P.Id=OI.ProductId
		INNER JOIN [Customer] C ON C.Id=O.CustomerId
		WHERE O.OrderStatusId=30 -- Order Paid

  -- SELECT '#CustomerOrderTemp', * FROM #CustomerOrderTemp
  -- exec ProductLoadAllPaged_V5 @ProductIds=N'10,11,13,14,15,16,17,18',@CustomerId=263,@ProfileTypeId=1

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
    -- WHERE CAST(P.CustomerProfileTypeId as INT)= @WarehouseId;

	-- SELECT '#FinalProducts Table', * FROM #FinalProducts

	-- exec ProductLoadAllPaged_V5 @ProductIds=N'4,10,11,13,14,15,16,17,18',@CustomerId=263,@ProfileTypeId=2

	--return all products
	SELECT 
		P.*
	FROM
		#FinalProducts P
	WHERE REPLACE(P.ProfileType,' ','')= @ProfileType

	DROP TABLE #Products
	DROP TABLE #ProductSpecs
	DROP TABLE #ProductSpecsTemp
	DROP TABLE #FinalProducts
	DROP TABLE #CustomerTemp
	DROP TABLE #CustomerOrderTemp
	

END


GO


